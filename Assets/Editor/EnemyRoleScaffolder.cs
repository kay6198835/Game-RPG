using System;
using System.Collections.Generic;
using System.IO;
// System.IO.Path is written out in full: the pathfinding `Path` class in the global namespace shadows it.
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// Builds the per-role enemy asset set — controller + clips, attack override, weapon, EntityData — and adds the
/// role to the room presets. Clips keep the entity animation events and borrow Paladin sprites as placeholders.
/// Weapons and EntityData are copied from the TrashMelee (melee) / RangedCaster (ranged) templates, then tuned
/// from the <see cref="Role"/> row. Skips any role whose controller already exists, so it never overwrites
/// authored work: the Roles table is the record of every role's tuning, and re-running only builds new rows.
/// The role's stat profile (Assets/SO/Stat/Enemy/...) must be authored first, with MaxRangeAttack /
/// MinRangeAttack formulas.
/// </summary>
public static class EnemyRoleScaffolder
{
    const string Tag = "[EnemyRoleScaffolder]";

    const string BaseController = "Assets/AnimationController/Enemy.controller";
    const string EnemyPrefab = "Assets/Prefab/Enemy/EnemyPrefab.prefab";

    // Templates: live roles whose data is known to work (2026-11-04). Knight is retired and no longer a source.
    const string MeleeTemplate = "TrashMelee";
    const string RangedTemplate = "RangedCaster";

    // Enemy.controller's Attack slots point at deleted clips; Paladin combo stage 1 supplies their timing.
    const string AttackTimingSource = "Assets/Animation/Player/Paladin/Paladin_ComboAttack/Paladin_ComboAttack_State1/Paladin_ComboAttack_State1_dir {0}.anim";
    const string FallbackTimingSource = "Assets/Animation/Player/Paladin/Basic/Idle/Knight_Idle_dir0.anim";
    const string PaladinAnim = "Assets/Animation/Player/Paladin";

    const string AnimRoot = "Assets/Animation/Enemy/Role";
    const string ControllerRoot = "Assets/AnimationController/Enemy/Role";
    const string DataRoot = "Assets/SO/Enemy/Role";
    const string PrefabRoot = "Assets/Prefab/Enemy/Role";
    const string RoomRoot = "Assets/SO/Database/Room/Role";
    const string NormalRoom = RoomRoot + "/RoomModel_Roles.asset";
    const string BossRoom = RoomRoot + "/RoomModel_Boss.asset";

    // Layer 7 = Player (ProjectSettings/TagManager.asset).
    const int PlayerLayerBits = 1 << 7;

    static readonly HashSet<string> PlayerOnlyStates = new HashSet<string> { "Interactor", "Equid_UnEquid" };
    static readonly HashSet<string> EntityEventReceivers = new HashSet<string>
    {
        "AnimationStart", "AnimationTrigger", "AnimationOnAction",
        "AnimationOffAction", "AnimationFinishTrigger", "AnimationEnd",
    };

    /// <summary>One enemy role: spawn weight / rarity, sight, weapon stage and (ranged) projectile tuning.</summary>
    struct Role
    {
        public string Name, Stats;
        public bool Ranged, Boss;
        public int Weight;
        public RarityTierEnemy Rarity;
        public float Sight, Idle;                       // EntityData.rangeCheckFieldOfView / idleDurationTime
        public int Damage;                              // AttackSO.attackDamage
        public float Range, Rate;                       // AttackSO.attackRange / attackRate
        public int Shots;                               // ranged only: projectile count
        public float Spread, Speed, Lifetime;           // ranged only

        public static Role Melee(string name, string stats, int weight, RarityTierEnemy rarity, float sight, float idle,
            int damage, float range, float rate, bool boss = false) => new Role
        {
            Name = name, Stats = stats, Weight = weight, Rarity = rarity, Sight = sight, Idle = idle,
            Damage = damage, Range = range, Rate = rate, Boss = boss,
        };

        public static Role RangedRole(string name, string stats, int weight, RarityTierEnemy rarity, float sight, float idle,
            int damage, float range, float rate, int shots, float spread, float speed, float lifetime) => new Role
        {
            Name = name, Stats = stats, Ranged = true, Weight = weight, Rarity = rarity, Sight = sight, Idle = idle,
            Damage = damage, Range = range, Rate = rate, Shots = shots, Spread = spread, Speed = speed, Lifetime = lifetime,
        };
    }

    // Values match the assets as tuned on 2026-11-04 (docs/systems/enemy/README.md). Sight must stay >= the
    // role's MaxRangeAttack stat. Existing rows are skipped on a re-run; edit the assets, then this table.
    static readonly Role[] Roles =
    {
        //            name            stats                                         wt  rarity                    sight idle  dmg range rate
        Role.Melee("TrashMelee",  "Assets/SO/Stat/Enemy/TrashMeleeStats.asset",    5, RarityTierEnemy.Common,    5f,   1f,   10, 1f,   1f),
        Role.Melee("FastSwarm",   "Assets/SO/Stat/Enemy/FastSwarmStats.asset",     3, RarityTierEnemy.Common,    6.5f, 1f,   10, 1f,   1f),
        Role.Melee("Tank",        "Assets/SO/Stat/Enemy/TankStats.asset",          8, RarityTierEnemy.Rare,      4f,   1f,   10, 1f,   1f),
        Role.Melee("Assasin",     "Assets/SO/Stat/Enemy/AssasinStats.asset",       6, RarityTierEnemy.Rare,      7f,   1f,   10, 1f,   1f),
        Role.Melee("Brute",       "Assets/SO/Stat/Enemy/BruteStats.asset",         7, RarityTierEnemy.Rare,      4.5f, 1.5f, 20, 1.3f, 2.2f),
        Role.Melee("Elite",       "Assets/SO/Stat/Enemy/EliteStats.asset",        15, RarityTierEnemy.Epic,      7f,   0.6f, 15, 1.5f, 1.1f),
        Role.Melee("Boss",        "Assets/SO/Stat/Enemy/Boss/BossStats.asset",    50, RarityTierEnemy.Legendary, 10f,  1f,   10, 1f,   1f, boss: true),
        //                                                                                                                                   shots spread speed life
        Role.RangedRole("RangedCaster", "Assets/SO/Stat/Enemy/RangedCasterStats.asset", 5, RarityTierEnemy.Common, 8f,  1f,   35, 10f, 1f,   1, 0f,  2f,   4f),
        Role.RangedRole("Sniper",       "Assets/SO/Stat/Enemy/SniperStats.asset",       6, RarityTierEnemy.Rare,   11f, 1.2f, 48, 10f, 2.5f, 1, 0f,  6f,   3f),
        Role.RangedRole("Scatter",      "Assets/SO/Stat/Enemy/ScatterStats.asset",      5, RarityTierEnemy.Common, 6.5f, 1f,  27, 6f,  1.8f, 3, 45f, 2.5f, 2.5f),
    };

    [MenuItem("Tools/Enemy/Scaffold Role Assets")]
    public static void RunAll()
    {
        var created = new List<(Role role, EntityData data)>();
        foreach (Role role in Roles)
        {
            EntityData data = BuildRole(role);
            if (data != null) created.Add((role, data));
        }
        AddToRooms(created);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"{Tag} done, {created.Count} role(s) created.");
    }

    static EntityData BuildRole(Role role)
    {
        string controllerPath = $"{ControllerRoot}/{role.Name}/{role.Name}.controller";
        if (File.Exists(controllerPath))
        {
            Debug.Log($"{Tag} {role.Name}: controller exists, skipped.");
            return null;
        }
        var stats = AssetDatabase.LoadAssetAtPath<BaseStatsSO>(role.Stats);
        if (stats == null)
        {
            Debug.LogError($"{Tag} {role.Name}: stat profile '{role.Stats}' not found — author it first. Skipped.");
            return null;
        }
        WarnMissingRangeFormulas(role, stats);

        AnimatorController controller = BuildController(role, controllerPath, out List<AnimationClip> attackClips);
        AnimatorOverrideController attackOverride = BuildAttackOverride(role, controller, attackClips);
        GameObject weapon = role.Ranged ? BuildRangeWeapon(role, attackOverride) : BuildMeleeWeapon(role, attackOverride);
        return weapon != null ? BuildEntityData(role, stats, controller, weapon) : null;
    }

    static void WarnMissingRangeFormulas(Role role, BaseStatsSO stats)
    {
        SerializedProperty formulas = new SerializedObject(stats).FindProperty("statFormulas");
        bool hasMax = false, hasMin = false;
        for (int i = 0; formulas != null && i < formulas.arraySize; i++)
        {
            int target = formulas.GetArrayElementAtIndex(i).FindPropertyRelative("targetStat").intValue;
            hasMax |= target == (int)StatType.MaxRangeAttack;
            hasMin |= target == (int)StatType.MinRangeAttack;
        }
        if (!hasMax || !hasMin)
            Debug.LogWarning($"{Tag} {role.Name}: '{stats.name}' has no MaxRangeAttack/MinRangeAttack formula — the role " +
                             "will read 0 and never attack. Add both (see docs/systems/enemy/README.md).");
    }

    static AnimatorController BuildController(Role role, string controllerPath, out List<AnimationClip> attackClips)
    {
        EnsureFolder(System.IO.Path.GetDirectoryName(controllerPath));
        AssetDatabase.CopyAsset(BaseController, controllerPath);
        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);

        var cache = new Dictionary<AnimationClip, AnimationClip>();
        var usedNames = new HashSet<string>();
        var attacks = new List<AnimationClip>();
        foreach (AnimatorControllerLayer layer in controller.layers)
        {
            ForEachState(layer.stateMachine, state =>
            {
                if (PlayerOnlyStates.Contains(state.name))
                {
                    state.motion = null;
                    return;
                }
                state.motion = RebuildMotion(state.motion, role, state.name, 0f, cache, usedNames, attacks);
            });
        }
        PruneOrphanBlendTrees(controller);
        EditorUtility.SetDirty(controller);
        attackClips = attacks;
        return controller;
    }

    [MenuItem("Tools/Enemy/Prune Role Controllers")]
    public static void PruneRoleControllers()
    {
        foreach (Role role in Roles)
        {
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>($"{ControllerRoot}/{role.Name}/{role.Name}.controller");
            if (controller == null) continue;
            PruneOrphanBlendTrees(controller);
            EditorUtility.SetDirty(controller);
        }
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// Enemy.controller carries blend trees no state reaches (and the player-only states' trees once their
    /// motion is cleared). They never play but still pin player and Bat clips into the asset, so drop them.
    /// </summary>
    static void PruneOrphanBlendTrees(AnimatorController controller)
    {
        var reachable = new HashSet<BlendTree>();
        foreach (AnimatorControllerLayer layer in controller.layers)
        {
            ForEachState(layer.stateMachine, state => CollectTrees(state.motion, reachable));
        }
        int removed = 0;
        foreach (UnityEngine.Object sub in AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(controller)))
        {
            if (sub is BlendTree tree && !reachable.Contains(tree))
            {
                AssetDatabase.RemoveObjectFromAsset(tree);
                UnityEngine.Object.DestroyImmediate(tree, true);
                removed++;
            }
        }
        Debug.Log($"{Tag} {controller.name}: pruned {removed} unreachable blend tree(s).");
    }

    static void CollectTrees(Motion motion, HashSet<BlendTree> into)
    {
        if (!(motion is BlendTree tree) || !into.Add(tree)) return;
        foreach (ChildMotion child in tree.children) CollectTrees(child.motion, into);
    }

    static Motion RebuildMotion(Motion motion, Role role, string stateName, float threshold,
        Dictionary<AnimationClip, AnimationClip> cache, HashSet<string> usedNames, List<AnimationClip> attacks)
    {
        if (motion is BlendTree tree)
        {
            ChildMotion[] children = tree.children;
            for (int i = 0; i < children.Length; i++)
            {
                children[i].motion = RebuildMotion(children[i].motion, role, stateName, children[i].threshold,
                    cache, usedNames, attacks);
            }
            tree.children = children;
            EditorUtility.SetDirty(tree);
            return tree;
        }

        var source = motion as AnimationClip;
        bool isAttack = stateName == "Attack";
        if (source != null && cache.TryGetValue(source, out AnimationClip reused)) return reused;

        string suffix;
        AnimationClip timing = source;
        if (isAttack)
        {
            int dir = Mathf.RoundToInt(threshold);
            suffix = $"Attack_dir {dir}";
            if (timing == null)
                timing = AssetDatabase.LoadAssetAtPath<AnimationClip>(string.Format(AttackTimingSource, dir));
        }
        else if (source != null)
        {
            int cut = source.name.IndexOf('_');
            suffix = cut >= 0 ? source.name.Substring(cut + 1) : source.name;
        }
        else
        {
            suffix = $"{stateName.Replace(" ", "")}_{Mathf.RoundToInt(threshold)}";
            timing = AssetDatabase.LoadAssetAtPath<AnimationClip>(FallbackTimingSource);
            Debug.LogWarning($"{Tag} {role.Name}: state '{stateName}' slot {threshold} had a missing clip; " +
                             $"clip '{role.Name}_{suffix}' uses Idle timing and carries no events.");
        }

        string clipName = Unique($"{role.Name}_{suffix}", usedNames);
        string folder = $"{AnimRoot}/{role.Name}/{stateName.Replace(" ", "")}";
        EnsureFolder(folder);
        AnimationClip clip = MakeClip(timing, clipName, keepEvents: source != null || isAttack,
            AssetDatabase.LoadAssetAtPath<AnimationClip>(PaladinSpriteSource(suffix)));
        AssetDatabase.CreateAsset(clip, $"{folder}/{clipName}.anim");

        if (source != null) cache[source] = clip;
        if (isAttack) attacks.Add(clip);
        return clip;
    }

    /// <summary>
    /// The Paladin clip whose sprites stand in for a role clip with this suffix (e.g. "Run_dir 3"), or null.
    /// Placeholder art only — replace per role later; the clip's events do not depend on it.
    /// </summary>
    static string PaladinSpriteSource(string suffix)
    {
        int dirAt = suffix.LastIndexOf("dir", StringComparison.Ordinal);
        if (dirAt < 0) return null;
        string n = suffix.Substring(dirAt + 3).Trim();
        if (suffix.StartsWith("Idle_")) return $"{PaladinAnim}/Basic/Idle/Knight_Idle_dir{n}.anim";
        if (suffix.StartsWith("Walk_")) return $"{PaladinAnim}/Basic/Walk/Knight_Walk_dir{n}.anim";
        if (suffix.StartsWith("Run_")) return $"{PaladinAnim}/Basic/Run/Knight_Run_dir {n}.anim";
        if (suffix.StartsWith("Die_")) return $"{PaladinAnim}/Basic/Die/Knight_Die_dir {n}.anim";
        if (suffix.StartsWith("Take_Damage_")) return $"{PaladinAnim}/Basic/Take Damage/Knight_Take_Damage_dir {n}.anim";
        if (suffix.StartsWith("Attack_")) return string.Format(AttackTimingSource, n);
        if (suffix.StartsWith("SpinAttack_State"))
        {
            string k = suffix.Substring("SpinAttack_State".Length, 1);
            return $"{PaladinAnim}/Ability-Skill/SlashAbility(Hold Cast Skill)/Paladin_SpinAttack_State{k}/Paladin_SpinAttack_State{k}_dir {n}.anim";
        }
        return null;
    }

    /// <summary>
    /// Same timing, curves, settings and entity events as <paramref name="source"/>. Sprites come from
    /// <paramref name="sprites"/> when its key times match; otherwise every sprite key is left empty.
    /// </summary>
    static AnimationClip MakeClip(AnimationClip source, string name, bool keepEvents, AnimationClip sprites)
    {
        var clip = new AnimationClip { name = name };
        if (source == null) return clip;

        clip.frameRate = source.frameRate;
        foreach (EditorCurveBinding binding in AnimationUtility.GetObjectReferenceCurveBindings(source))
        {
            ObjectReferenceKeyframe[] keys = AnimationUtility.GetObjectReferenceCurve(source, binding);
            ObjectReferenceKeyframe[] art = sprites != null ? AnimationUtility.GetObjectReferenceCurve(sprites, binding) : null;
            bool match = art != null && art.Length == keys.Length;
            for (int i = 0; match && i < keys.Length; i++) match = Mathf.Abs(art[i].time - keys[i].time) < 0.0001f;
            if (sprites != null && !match)
                Debug.LogWarning($"{Tag} {name}: '{sprites.name}' key times differ — sprites left empty.");

            var result = new ObjectReferenceKeyframe[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                result[i] = new ObjectReferenceKeyframe { time = keys[i].time, value = match ? art[i].value : null };
            }
            AnimationUtility.SetObjectReferenceCurve(clip, binding, result);
        }
        foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(source))
        {
            AnimationUtility.SetEditorCurve(clip, binding, AnimationUtility.GetEditorCurve(source, binding));
        }
        AnimationUtility.SetAnimationClipSettings(clip, AnimationUtility.GetAnimationClipSettings(source));

        if (keepEvents)
        {
            var events = new List<AnimationEvent>();
            foreach (AnimationEvent e in AnimationUtility.GetAnimationEvents(source))
            {
                if (EntityEventReceivers.Contains(e.functionName)) events.Add(e);
            }
            AnimationUtility.SetAnimationEvents(clip, events.ToArray());
        }
        return clip;
    }

    static AnimatorOverrideController BuildAttackOverride(Role role, AnimatorController controller, List<AnimationClip> attackClips)
    {
        // Identity override of the role's own attack clips: the weapon swaps to it on attack and
        // Utility.DurationNextAttack() reads the overridden clips for the combo window.
        var ov = new AnimatorOverrideController(controller) { name = $"{role.Name}_AttackStage1" };
        var pairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        foreach (AnimationClip clip in attackClips) pairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, clip));
        ov.ApplyOverrides(pairs);
        AssetDatabase.CreateAsset(ov, $"{ControllerRoot}/{role.Name}/{role.Name}_AttackStage1.overrideController");
        return ov;
    }

    static string TemplateStage(string template) => $"{DataRoot}/{template}/Weapon/{template}AttackStage1.asset";
    static string TemplateStats(string template) => $"{DataRoot}/{template}/Weapon/{template}WeaponStats.asset";
    static string TemplateWeapon(string template) => $"{PrefabRoot}/{template}/{template}Weapon.prefab";

    static GameObject BuildMeleeWeapon(Role role, AnimatorOverrideController attackOverride)
    {
        var templateStage = AssetDatabase.LoadAssetAtPath<AttackSO>(TemplateStage(MeleeTemplate));
        var templateStats = AssetDatabase.LoadAssetAtPath<MeleeWeaponStats>(TemplateStats(MeleeTemplate));
        if (templateStage == null || templateStats == null) return MissingTemplate(role, MeleeTemplate);

        string folder = $"{DataRoot}/{role.Name}/Weapon";
        EnsureFolder(folder);

        var stage = ScriptableObject.CreateInstance<AttackSO>();
        EditorUtility.CopySerialized(templateStage, stage);
        TuneStage(stage, role, attackOverride);
        AssetDatabase.CreateAsset(stage, $"{folder}/{role.Name}AttackStage1.asset");

        var stats = ScriptableObject.CreateInstance<MeleeWeaponStats>();
        EditorUtility.CopySerialized(templateStats, stats);
        SetStages(stats, stage, PlayerLayerBits, $"{role.Name} Weapon");
        AssetDatabase.CreateAsset(stats, $"{folder}/{role.Name}WeaponStats.asset");

        return CopyWeaponPrefab(role, TemplateWeapon(MeleeTemplate), stats);
    }

    static GameObject BuildRangeWeapon(Role role, AnimatorOverrideController attackOverride)
    {
        var templateStage = AssetDatabase.LoadAssetAtPath<RangeAttackSO>(TemplateStage(RangedTemplate));
        var templateStats = AssetDatabase.LoadAssetAtPath<RangeWeaponStats>(TemplateStats(RangedTemplate));
        string templateProjectile = $"{PrefabRoot}/{RangedTemplate}/{RangedTemplate}Projectile.prefab";
        if (templateStage == null || templateStats == null || !File.Exists(templateProjectile))
            return MissingTemplate(role, RangedTemplate);

        string folder = $"{DataRoot}/{role.Name}/Weapon";
        EnsureFolder(folder);

        // The template projectile already carries its trigger collider (the old Arrow.prefab source had none).
        string prefabFolder = $"{PrefabRoot}/{role.Name}";
        EnsureFolder(prefabFolder);
        string projectilePath = $"{prefabFolder}/{role.Name}Projectile.prefab";
        AssetDatabase.CopyAsset(templateProjectile, projectilePath);
        var projectile = AssetDatabase.LoadAssetAtPath<GameObject>(projectilePath);

        var stage = ScriptableObject.CreateInstance<RangeAttackSO>();
        EditorUtility.CopySerialized(templateStage, stage);
        TuneStage(stage, role, attackOverride);
        var so = new SerializedObject(stage);
        so.FindProperty("<ProjectilePrefab>k__BackingField").objectReferenceValue = projectile;
        so.FindProperty("<ProjectileCount>k__BackingField").intValue = role.Shots;
        so.FindProperty("<SpreadAngle>k__BackingField").floatValue = role.Spread;
        so.FindProperty("<ProjectileConfig>k__BackingField.speed").floatValue = role.Speed;
        so.FindProperty("<ProjectileConfig>k__BackingField.lifetime").floatValue = role.Lifetime;
        so.FindProperty("<ProjectileConfig>k__BackingField.targetMask").intValue = PlayerLayerBits;
        so.ApplyModifiedPropertiesWithoutUndo();
        AssetDatabase.CreateAsset(stage, $"{folder}/{role.Name}AttackStage1.asset");

        var stats = ScriptableObject.CreateInstance<RangeWeaponStats>();
        EditorUtility.CopySerialized(templateStats, stats);
        SetStages(stats, stage, PlayerLayerBits, $"{role.Name} Weapon");
        AssetDatabase.CreateAsset(stats, $"{folder}/{role.Name}WeaponStats.asset");

        return CopyWeaponPrefab(role, TemplateWeapon(RangedTemplate), stats);
    }

    static GameObject MissingTemplate(Role role, string template)
    {
        Debug.LogError($"{Tag} {role.Name}: template role '{template}' assets not found — weapon and EntityData not built.");
        return null;
    }

    static void TuneStage(AttackSO stage, Role role, AnimatorOverrideController attackOverride)
    {
        stage.nameState = $"{role.Name}_Attack1";
        stage.directionAttackAnimatorOV = attackOverride;
        stage.attackDamage = role.Damage;
        stage.attackRange = role.Range;
        stage.attackRate = role.Rate;
    }

    static void SetStages(WeaponStats stats, AttackSO stage, int layerBits, string weaponName)
    {
        var so = new SerializedObject(stats);
        so.FindProperty("<LayerMask>k__BackingField").intValue = layerBits;
        SerializedProperty stages = so.FindProperty("<AttackStages>k__BackingField");
        stages.ClearArray();
        stages.InsertArrayElementAtIndex(0);
        stages.GetArrayElementAtIndex(0).objectReferenceValue = stage;
        SerializedProperty nameWeapon = so.FindProperty("nameWeapon");
        if (nameWeapon != null) nameWeapon.stringValue = weaponName;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    static GameObject CopyWeaponPrefab(Role role, string sourcePrefab, WeaponStats stats)
    {
        string folder = $"{PrefabRoot}/{role.Name}";
        EnsureFolder(folder);
        string path = $"{folder}/{role.Name}Weapon.prefab";
        AssetDatabase.CopyAsset(sourcePrefab, path);

        GameObject root = PrefabUtility.LoadPrefabContents(path);
        root.name = $"{role.Name}Weapon";
        var weapon = root.GetComponent<Weapon>();
        var so = new SerializedObject(weapon);
        so.FindProperty("stats").objectReferenceValue = stats;
        so.ApplyModifiedPropertiesWithoutUndo();
        PrefabUtility.SaveAsPrefabAsset(root, path);
        PrefabUtility.UnloadPrefabContents(root);
        return AssetDatabase.LoadAssetAtPath<GameObject>(path);
    }

    static EntityData BuildEntityData(Role role, BaseStatsSO stats, AnimatorController controller, GameObject weapon)
    {
        string template = role.Ranged ? RangedTemplate : MeleeTemplate;
        var source = AssetDatabase.LoadAssetAtPath<EntityData>($"{DataRoot}/{template}/{template}EntityData.asset");
        if (source == null)
        {
            MissingTemplate(role, template);
            return null;
        }

        var data = ScriptableObject.CreateInstance<EntityData>();
        EditorUtility.CopySerialized(source, data);

        var so = new SerializedObject(data);
        so.FindProperty("stats").objectReferenceValue = stats;
        so.FindProperty("animationController").objectReferenceValue = controller;
        so.FindProperty("<Weapon>k__BackingField").objectReferenceValue = weapon.GetComponent<Weapon>();
        so.FindProperty("abilityBindings").ClearArray();
        so.FindProperty("rangeCheckFieldOfView").floatValue = role.Sight;
        so.FindProperty("idleDurationTime").floatValue = role.Idle;
        so.ApplyModifiedPropertiesWithoutUndo();

        string folder = $"{DataRoot}/{role.Name}";
        EnsureFolder(folder);
        AssetDatabase.CreateAsset(data, $"{folder}/{role.Name}EntityData.asset");
        Debug.Log($"{Tag} {role.Name}: created.");
        return data;
    }

    /// <summary>
    /// Adds each new role to RoomModel_Roles (or RoomModel_Boss for the boss), creating the preset if it does
    /// not exist and skipping roles already listed. The themed RoomModel_T* presets are curated by hand.
    /// </summary>
    static void AddToRooms(List<(Role role, EntityData data)> created)
    {
        if (created.Count == 0) return;
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(EnemyPrefab);
        foreach ((Role role, EntityData data) in created)
        {
            RoomModel room = LoadOrCreateRoom(role.Boss ? BossRoom : NormalRoom, role.Boss ? "Boss" : "Roles");
            var so = new SerializedObject(room);
            SerializedProperty list = so.FindProperty("enemiesOfRoom");
            bool listed = false;
            for (int i = 0; i < list.arraySize && !listed; i++)
                listed = list.GetArrayElementAtIndex(i).FindPropertyRelative("Data").objectReferenceValue == data;
            if (listed) continue;

            int at = list.arraySize;
            list.InsertArrayElementAtIndex(at);
            SerializedProperty entry = list.GetArrayElementAtIndex(at);
            entry.FindPropertyRelative("Prefab").objectReferenceValue = prefab;
            entry.FindPropertyRelative("Data").objectReferenceValue = data;
            entry.FindPropertyRelative("weight").intValue = role.Weight;
            entry.FindPropertyRelative("rarityTier").intValue = (int)role.Rarity;
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(room);
            Debug.Log($"{Tag} {role.Name}: added to {room.name}.");
        }
    }

    static RoomModel LoadOrCreateRoom(string path, string label)
    {
        var room = AssetDatabase.LoadAssetAtPath<RoomModel>(path);
        if (room != null) return room;

        EnsureFolder(System.IO.Path.GetDirectoryName(path));
        room = ScriptableObject.CreateInstance<RoomModel>();
        AssetDatabase.CreateAsset(room, path);
        var so = new SerializedObject(room);
        so.FindProperty("id").intValue = Math.Abs(Guid.NewGuid().GetHashCode());
        so.FindProperty("nameEnity").stringValue = label;
        so.FindProperty("weightBudget").intValue = 50;
        so.FindProperty("enemiesOfRoom").ClearArray();
        so.ApplyModifiedPropertiesWithoutUndo();
        return room;
    }

    static void ForEachState(AnimatorStateMachine machine, Action<AnimatorState> action)
    {
        foreach (ChildAnimatorState child in machine.states) action(child.state);
        foreach (ChildAnimatorStateMachine sub in machine.stateMachines) ForEachState(sub.stateMachine, action);
    }

    static string Unique(string name, HashSet<string> used)
    {
        string candidate = name;
        for (int n = 2; !used.Add(candidate); n++) candidate = $"{name}_{n}";
        return candidate;
    }

    static void EnsureFolder(string path)
    {
        path = path.Replace('\\', '/');
        if (AssetDatabase.IsValidFolder(path)) return;
        string parent = System.IO.Path.GetDirectoryName(path).Replace('\\', '/');
        EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, System.IO.Path.GetFileName(path));
    }
}
