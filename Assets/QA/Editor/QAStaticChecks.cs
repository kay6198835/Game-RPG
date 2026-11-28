using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace QA.Automation.Editor
{
    // Edit Mode checks on assets and source text — no Play Mode, no scene. Group STATIC.
    // Each finding is a class of defect this project has shipped before (see the case catalog).
    public static class QAStaticChecks
    {
        private const string Group = "STATIC";

        // Unity's own keys in a MonoBehaviour / ScriptableObject YAML block; never orphans.
        private static readonly HashSet<string> UnityKeys = new()
        {
            "m_ObjectHideFlags", "m_CorrespondingSourceObject", "m_PrefabInstance", "m_PrefabAsset", "m_GameObject",
            "m_Enabled", "m_EditorHideFlags", "m_Script", "m_Name", "m_EditorClassIdentifier", "serializedVersion",
            "references", "version", "RefIds", "rid", "type", "data", "class", "ns", "asm",
        };

        private static readonly Regex YamlKey = new(@"^(\s*)(?:-\s+)?([A-Za-z_<>][A-Za-z0-9_<>]*):", RegexOptions.Compiled);
        private static readonly Regex ScriptRef = new(@"m_Script:\s*\{fileID:\s*11500000,\s*guid:\s*([0-9a-f]{32})", RegexOptions.Compiled);
        private static readonly Regex AnyGuid = new(@"guid:\s*([0-9a-f]{32})", RegexOptions.Compiled);

        public static QAResultFile Run(QARunConfig config)
        {
            QAResultFile file = new() { group = Group, scene = "(Edit Mode)", started = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
            Add(file, config, "QA-STATIC-01", QAPriority.P0, "No asset or prefab references a missing script", MissingScripts);
            Add(file, config, "QA-STATIC-02", QAPriority.P1, "No ScriptableObject under Assets/SO references a deleted asset", MissingAssetRefs);
            Add(file, config, "QA-STATIC-03", QAPriority.P1, "No serialized key left behind by a field rename (missing [FormerlySerializedAs])", OrphanKeys);
            Add(file, config, "QA-STATIC-04", QAPriority.P0, "No runtime stat modifier saved into a stat asset", SavedModifiers);
            Add(file, config, "QA-STATIC-05", QAPriority.P0, "Runtime scripts do not use UnityEditor outside #if UNITY_EDITOR", EditorUsings);
            Add(file, config, "QA-STATIC-06", QAPriority.P0, "Every enabled build scene exists", BuildScenes);
            Add(file, config, "QA-STATIC-07", QAPriority.P1, "Every enemy role has a weapon, a move speed and a consistent attack range", EnemyRoleData);
            Add(file, config, "QA-STATIC-08", QAPriority.P1, "Every enemy role weapon can hit the player: attack clips carry the damage event, masks include the player hurtbox", EnemyAttackWiring);
            Add(file, config, "QA-STATIC-09", QAPriority.P1, "Every droppable item has a name, a sprite and at least one loadable effect; every item effect changes something", ItemData);
            Add(file, config, "QA-STATIC-10", QAPriority.P1, "Every drop table is consistent: no empty slot, every item's tier has a rate, rates sum to at most 100", DropTables);
            Add(file, config, "QA-STATIC-11", QAPriority.P1, "Every playable class has stats, an animator, a weapon and one ability per slot", ClassData);
            Add(file, config, "QA-STATIC-12", QAPriority.P1, "Every ability bound to a class has an id, a name, a cooldown, valid costs and at least one effect", BoundAbilities);
            Add(file, config, "QA-STATIC-13", QAPriority.P1, "Every class death clip sends AnimationOnAction (PlayerDeathState emits ON_PLAYER_DEATH on it)", DeathClipEvents);
            file.finished = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return file;
        }

        private static void Add(QAResultFile file, QARunConfig config, string id, QAPriority priority, string title, Action<QACaseResult> body)
        {
            if (!config.Selected("STATIC", priority)) return;
            QACaseResult result = new() { id = id, title = title, system = "STATIC", priority = priority.ToString(), group = Group };
            float start = Time.realtimeSinceStartup;
            try
            {
                body(result);
            }
            catch (Exception e)
            {
                Fail(result, $"exception {e.GetType().Name}: {e.Message}");
            }
            result.seconds = Time.realtimeSinceStartup - start;
            file.cases.Add(result);
            Debug.Log($"[QA] {result.verdict} {result.id} — {result.title}");
        }

        private static void Fail(QACaseResult r, string message)
        {
            r.verdict = nameof(QAVerdict.FAIL);
            r.notes.Add("FAIL: " + message);
        }

        private static string Root => QARunConfig.ProjectRoot;

        private static IEnumerable<string> Files(string folder, params string[] patterns)
        {
            string dir = System.IO.Path.Combine(Root, folder);
            if (!Directory.Exists(dir)) return Enumerable.Empty<string>();
            return patterns.SelectMany(p => Directory.GetFiles(dir, p, SearchOption.AllDirectories)).OrderBy(f => f, StringComparer.Ordinal);
        }

        private static string Rel(string full) => full.Substring(Root.Length + 1).Replace('\\', '/');

        private static bool GuidExists(string guid) => !string.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(guid));

        private static void Report(QACaseResult r, List<string> findings, string okMessage, int limit = 2000)
        {
            if (findings.Count == 0)
            {
                r.notes.Add("ok: " + okMessage);
                return;
            }
            Fail(r, $"{findings.Count} finding(s)");
            r.notes.AddRange(findings.Take(limit));
            if (findings.Count > limit) r.notes.Add($"... {findings.Count - limit} more");
        }

        private static void MissingScripts(QACaseResult r)
        {
            List<string> findings = new();
            int scanned = 0;
            foreach (string file in Files("Assets", "*.asset", "*.prefab", "*.unity"))
            {
                if (Rel(file).StartsWith("Assets/QA/")) continue;
                scanned++;
                foreach (Match m in ScriptRef.Matches(File.ReadAllText(file)))
                    if (!GuidExists(m.Groups[1].Value)) findings.Add($"{Rel(file)}: script guid {m.Groups[1].Value}");
            }
            Report(r, findings.Distinct().ToList(), $"{scanned} files, every m_Script resolves");
        }

        private static void MissingAssetRefs(QACaseResult r)
        {
            List<string> findings = new();
            int scanned = 0;
            foreach (string file in Files("Assets/SO", "*.asset"))
            {
                scanned++;
                string text = File.ReadAllText(file);
                foreach (Match m in AnyGuid.Matches(text))
                {
                    string guid = m.Groups[1].Value;
                    if (guid.StartsWith("0000000000000000")) continue;
                    if (!GuidExists(guid)) findings.Add($"{Rel(file)}: guid {guid}");
                }
            }
            Report(r, findings.Distinct().ToList(), $"{scanned} SO assets, every guid resolves");
        }

        // A YAML key that no serialized property of the asset's type carries is data Unity no longer reads:
        // the field was renamed or removed without [FormerlySerializedAs].
        private static void OrphanKeys(QACaseResult r)
        {
            List<string> findings = new();
            int scanned = 0;
            foreach (string file in Files("Assets/SO", "*.asset"))
            {
                string rel = Rel(file);
                ScriptableObject asset = AssetDatabase.LoadMainAssetAtPath(rel) as ScriptableObject;
                if (asset == null) continue;
                scanned++;

                HashSet<string> known = new(StringComparer.Ordinal);
                SerializedObject so = new(asset);
                SerializedProperty it = so.GetIterator();
                while (it.Next(true)) known.Add(it.name);
                known.UnionWith(FormerNames(asset.GetType()));

                HashSet<string> orphans = new(StringComparer.Ordinal);
                bool inBlock = false;
                foreach (string line in File.ReadLines(file))
                {
                    if (line.StartsWith("--- ")) { inBlock = false; continue; }
                    if (line.StartsWith("MonoBehaviour:")) { inBlock = true; continue; }
                    if (!inBlock) continue;
                    Match m = YamlKey.Match(line);
                    if (!m.Success) continue;
                    string key = m.Groups[2].Value;
                    if (UnityKeys.Contains(key) || key.StartsWith("m_") || known.Contains(key)) continue;
                    orphans.Add(key);
                }
                // One line per key, so the static baseline and the known-issue patterns can match each one.
                foreach (string key in orphans.OrderBy(k => k, StringComparer.Ordinal)) findings.Add($"{rel} ({asset.GetType().Name}): {key}");
                if (scanned % 50 == 0) EditorUtility.UnloadUnusedAssetsImmediate();
            }
            Report(r, findings, $"{scanned} SO assets, no orphaned keys");
        }

        // Old names declared with [FormerlySerializedAs] are still read by Unity, so they are not orphans.
        private static readonly Dictionary<Type, HashSet<string>> FormerNamesCache = new();

        private static HashSet<string> FormerNames(Type type, int depth = 0)
        {
            if (FormerNamesCache.TryGetValue(type, out HashSet<string> cached)) return cached;
            HashSet<string> names = new(StringComparer.Ordinal);
            FormerNamesCache[type] = names;
            const System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public |
                                                         System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.DeclaredOnly;
            for (Type t = type; t != null && t != typeof(ScriptableObject) && t != typeof(object); t = t.BaseType)
            {
                foreach (System.Reflection.FieldInfo field in t.GetFields(flags))
                {
                    foreach (UnityEngine.Serialization.FormerlySerializedAsAttribute a in field.GetCustomAttributes(typeof(UnityEngine.Serialization.FormerlySerializedAsAttribute), true))
                        names.Add(a.oldName);
                    Type fieldType = field.FieldType.IsArray ? field.FieldType.GetElementType()
                        : field.FieldType.IsGenericType ? field.FieldType.GetGenericArguments()[0] : field.FieldType;
                    if (depth < 3 && fieldType != null && fieldType.IsSerializable && !fieldType.IsPrimitive && fieldType != typeof(string)
                        && !typeof(UnityEngine.Object).IsAssignableFrom(fieldType) && fieldType.Assembly == type.Assembly)
                        names.UnionWith(FormerNames(fieldType, depth + 1));
                }
            }
            return names;
        }

        // BUG-063 / BUG-115: Stat.modifiers is serialized in the Editor, so a runtime buff can be saved into
        // a committed stat asset. A non-empty `modifiers:` list under Assets/SO/Stat is that leak.
        private static void SavedModifiers(QACaseResult r)
        {
            List<string> findings = new();
            foreach (string file in Files("Assets/SO", "*.asset"))
            {
                string[] lines = File.ReadAllLines(file);
                if (!lines.Any(l => l.Contains("baseValue:"))) continue;
                int count = 0;
                for (int i = 0; i < lines.Length; i++)
                {
                    Match m = Regex.Match(lines[i], @"^(\s*)modifiers:\s*$");
                    if (!m.Success) continue;
                    string indent = m.Groups[1].Value;
                    for (int j = i + 1; j < lines.Length; j++)
                    {
                        if (lines[j].StartsWith(indent + "- ")) count++;
                        else if (!lines[j].StartsWith(indent + "  ")) break;
                    }
                }
                if (count > 0) findings.Add($"{Rel(file)}: {count} saved modifier(s)");
            }
            Report(r, findings, "no stat asset carries saved modifiers");
        }

        private static void EditorUsings(QACaseResult r)
        {
            List<string> findings = new();
            foreach (string file in Files("Assets", "*.cs"))
            {
                string rel = Rel(file);
                // Assets/Tests/ is the Editor-only GameRPG.EditModeTests assembly (includePlatforms: Editor), never in a build.
                if (rel.Contains("/Editor/") || rel.StartsWith("Assets/QA/") || rel.StartsWith("Assets/Plugins/") || rel.StartsWith("Assets/Tests/")) continue;
                int depth = 0;
                bool editorBlock = false;
                Stack<bool> guards = new();
                int lineNo = 0;
                foreach (string raw in File.ReadLines(file))
                {
                    lineNo++;
                    string line = raw.Trim();
                    if (line.StartsWith("#if"))
                    {
                        guards.Push(line.Contains("UNITY_EDITOR"));
                        depth++;
                    }
                    else if (line.StartsWith("#endif") && guards.Count > 0)
                    {
                        guards.Pop();
                        depth--;
                    }
                    editorBlock = guards.Any(g => g);
                    if (!editorBlock && Regex.IsMatch(line, @"^using\s+UnityEditor(\.|;)"))
                        findings.Add($"{rel}:{lineNo}: {line}");
                }
            }
            Report(r, findings, "no unguarded `using UnityEditor` in runtime scripts");
        }

        private static void BuildScenes(QACaseResult r)
        {
            List<string> findings = new();
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            foreach (EditorBuildSettingsScene scene in scenes.Where(s => s.enabled))
                if (!File.Exists(System.IO.Path.Combine(Root, scene.path))) findings.Add($"missing: {scene.path}");
            r.notes.Add("build scenes: " + string.Join(", ", scenes.Where(s => s.enabled).Select(s => s.path)));
            if (!scenes.Any(s => s.enabled)) findings.Add("no enabled build scene");
            Report(r, findings, $"{scenes.Count(s => s.enabled)} enabled scenes exist");
        }

        // Since 6094aa10 enemy move speed and attack range come from the role's stat asset
        // (StatType.MoveSpeed / MaxRangeAttack / MinRangeAttack) and detection from EntityData.rangeCheckFieldOfView.
        // A zero there means an enemy that never moves, never attacks or never sees the player.
        private static void EnemyRoleData(QACaseResult r)
        {
            List<string> findings = new();
            List<string> info = new();
            string[] guids = AssetDatabase.FindAssets("t:EntityData", new[] { "Assets/SO/Enemy/Role" });
            if (guids.Length == 0)
            {
                r.verdict = nameof(QAVerdict.SKIP);
                r.notes.Add("SKIP: no EntityData under Assets/SO/Enemy/Role");
                return;
            }
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                EntityData data = AssetDatabase.LoadAssetAtPath<EntityData>(path);
                if (data == null) continue;
                string name = System.IO.Path.GetFileNameWithoutExtension(path);
                if (data.Weapon == null) findings.Add($"{name}: no Weapon");
                if (data.Stats == null)
                {
                    findings.Add($"{name}: no stat asset");
                    continue;
                }
                StatSheet sheet = new StatSheet(data.Stats);
                float move = sheet.GetStatValue(StatType.MoveSpeed);
                float max = sheet.GetStatValue(StatType.MaxRangeAttack);
                float min = sheet.GetStatValue(StatType.MinRangeAttack);
                float fov = data.RangeView(sheet);
                info.Add($"{name}: MoveSpeed {move:0.##}, range {min:0.##}-{max:0.##}, FOV {fov:0.##}");
                if (move <= 0f) findings.Add($"{name}: MoveSpeed {move:0.##} (never moves)");
                if (max <= 0f) findings.Add($"{name}: MaxRangeAttack {max:0.##} (never attacks)");
                if (min < 0f || min >= max) findings.Add($"{name}: MinRangeAttack {min:0.##} not below MaxRangeAttack {max:0.##}");
                if (fov < max) findings.Add($"{name}: view range {fov:0.##} below MaxRangeAttack {max:0.##}");
            }
            Report(r, findings, $"{guids.Length} role EntityData assets consistent");
            if (findings.Count == 0) r.notes.AddRange(info);
        }

        private const string PlayerPrefabPath = "Assets/Prefab/Player/PlayerTest.prefab";

        // The static half of QA-ENT-08. Weapon.OnAttackEnter() swaps the animator to the stage's
        // directionAttackAnimatorOV and plays its "Attack" clips; EntityAttackState deals damage only on the
        // clip's AnimationOnAction event and leaves only on AnimationFinishTrigger / AnimationEnd.
        // MeleeWeapon hits WeaponStats.LayerMask; RangeWeapon's projectile hits ProjectileConfig.targetMask.
        private static void EnemyAttackWiring(QACaseResult r)
        {
            List<string> findings = new();
            List<string> info = new();
            string[] guids = AssetDatabase.FindAssets("t:EntityData", new[] { "Assets/SO/Enemy/Role" });
            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            NegativeReciver hurtbox = playerPrefab != null ? playerPrefab.GetComponentInChildren<NegativeReciver>(true) : null;
            if (guids.Length == 0 || hurtbox == null)
            {
                r.verdict = nameof(QAVerdict.SKIP);
                r.notes.Add(guids.Length == 0 ? "SKIP: no EntityData under Assets/SO/Enemy/Role" : $"SKIP: no NegativeReciver on {PlayerPrefabPath}");
                return;
            }
            int playerLayer = hurtbox.gameObject.layer;
            int playerBit = 1 << playerLayer;
            info.Add($"player hurtbox layer {LayerMask.LayerToName(playerLayer)} ({playerLayer})");

            foreach (string path in guids.Select(AssetDatabase.GUIDToAssetPath).OrderBy(p => p, StringComparer.Ordinal))
            {
                EntityData data = AssetDatabase.LoadAssetAtPath<EntityData>(path);
                if (data == null) continue;
                string role = System.IO.Path.GetFileNameWithoutExtension(path);
                if (data.Weapon == null)
                {
                    findings.Add($"{role}: no Weapon (see QA-STATIC-07)");
                    continue;
                }
                WeaponStats stats = data.Weapon.Stats;
                if (stats == null || stats.StageCount == 0)
                {
                    findings.Add($"{role}: weapon {data.Weapon.name} has no WeaponStats or no attack stage");
                    continue;
                }
                if (stats.Type == WeaponType.MeleeWP && (stats.LayerMask.value & playerBit) == 0)
                    findings.Add($"{role}: melee {stats.name} LayerMask {stats.LayerMask.value} does not include the player hurtbox layer");

                int stageIndex = 0;
                foreach (AttackSO stage in stats.AttackStages)
                {
                    string where = $"{role}: {stats.name} stage {stageIndex++}";
                    if (stage == null)
                    {
                        findings.Add($"{where} is empty");
                        continue;
                    }
                    if (stage is RangeAttackSO ranged)
                    {
                        if ((ranged.ProjectileConfig.targetMask.value & playerBit) == 0)
                            findings.Add($"{where} ({stage.name}): projectile targetMask {ranged.ProjectileConfig.targetMask.value} does not include the player hurtbox layer");
                        if (ranged.ProjectilePrefab == null)
                            findings.Add($"{where} ({stage.name}): no ProjectilePrefab");
                        else if (ranged.ProjectilePrefab.GetComponentInChildren<Collider2D>(true) == null)
                            findings.Add($"{where} ({stage.name}): projectile {ranged.ProjectilePrefab.name} has no Collider2D (never triggers a hit)");
                    }
                    AnimatorOverrideController ov = stage.directionAttackAnimatorOV;
                    if (ov == null)
                    {
                        findings.Add($"{where} ({stage.name}): no directionAttackAnimatorOV (the animator is set to null on attack)");
                        continue;
                    }
                    List<AnimationClip> clips = Utility.GetOverrideClips(ov, "Attack")
                        .Select(p => p.Value != null ? p.Value : p.Key).Where(c => c != null).Distinct().ToList();
                    if (clips.Count == 0)
                    {
                        findings.Add($"{where} ({stage.name}): {ov.name} has no clip named *Attack*");
                        continue;
                    }
                    foreach (AnimationClip clip in clips)
                    {
                        HashSet<string> events = new(AnimationUtility.GetAnimationEvents(clip).Select(e => e.functionName));
                        if (!events.Contains("AnimationOnAction"))
                            findings.Add($"{where}: clip {clip.name} has no AnimationOnAction event (attack plays, no damage)");
                        if (!events.Contains("AnimationFinishTrigger") && !events.Contains("AnimationEnd"))
                            findings.Add($"{where}: clip {clip.name} has no AnimationFinishTrigger / AnimationEnd event (enemy stuck in EntityAttackState)");
                    }
                    info.Add($"{where} ({stage.name}): {clips.Count} attack clip(s) checked in {ov.name}");
                }
            }
            Report(r, findings, $"{guids.Length} role weapons can hit the player");
            if (findings.Count == 0) r.notes.AddRange(info);
        }

        private static List<T> LoadAll<T>(string folder) where T : UnityEngine.Object =>
            AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { folder })
                .Select(AssetDatabase.GUIDToAssetPath).Distinct().OrderBy(p => p, StringComparer.Ordinal)
                .Select(AssetDatabase.LoadAssetAtPath<T>).Where(a => a != null).ToList();

        private static string PathOf(UnityEngine.Object asset) => AssetDatabase.GetAssetPath(asset);

        // ItemController.Interact() applies every effect of the item with no null guard, then emits
        // ON_COLLECT_ITEM; ItemSpawner shows ItemSprite and renames the drop to ItemName. Only items a drop table
        // can roll are judged; the others are listed as info.
        private static void ItemData(QACaseResult r)
        {
            List<string> findings = new();
            List<string> info = new();
            HashSet<ItemSO> droppable = new(LoadAll<DepotItem>("Assets/SO")
                .SelectMany(d => d.listItems).Where(s => s != null && s.DepotItem != null).Select(s => s.DepotItem));
            List<ItemSO> items = LoadAll<ItemSO>("Assets/SO").Where(i => i is not WeaponSO).ToList();
            foreach (ItemSO item in items)
            {
                string where = $"{PathOf(item)}";
                if (!droppable.Contains(item))
                {
                    info.Add($"{where}: in no drop table (not judged)");
                    continue;
                }
                if (string.IsNullOrWhiteSpace(item.ItemName)) findings.Add($"{where}: ItemName is empty (the drop shows no name)");
                if (item.ItemSprite == null) findings.Add($"{where}: ItemSprite is not set (the drop is invisible)");
                if (item.Effects == null || item.Effects.Count == 0) findings.Add($"{where}: no effect (picking it up does nothing)");
                else
                {
                    int missing = item.Effects.Count(e => e == null);
                    if (missing > 0) findings.Add($"{where}: {missing} effect slot(s) empty or bound to a missing / abstract script (ItemController.Interact throws NullReferenceException)");
                }
            }

            foreach (ItemEffectDefinitionSO effect in LoadAll<ItemEffectDefinitionSO>("Assets/SO"))
            {
                string where = PathOf(effect);
                SerializedObject so = new(effect);
                if (effect is CoinModifierEffect)
                {
                    SerializedProperty amount = so.FindProperty("coinAmount");
                    if (amount == null) findings.Add($"{where}: CoinModifierEffect has no serialized coinAmount (field renamed?)");
                    else if (amount.intValue <= 0) findings.Add($"{where}: coinAmount {amount.intValue} (gives no coin)");
                    else info.Add($"{where}: +{amount.intValue} coin");
                    continue;
                }
                if (effect is not StatModifierEffect) continue;
                // Since the 2026-11-23 stat refactor the group has one list (statsModifiers). ModifierMaxStatsEffect
                // applies every entry; ModifierCurrentStatsEffect (AdjustCurrentStats) skips entries whose target is
                // not a VitalStatType member.
                SerializedProperty used = so.FindProperty("statModifierGroup.statsModifiers");
                if (used == null)
                {
                    findings.Add($"{where}: statModifierGroup.statsModifiers not found (field renamed?)");
                    continue;
                }
                bool max = effect is ModifierMaxStatsEffect;
                if (used.arraySize == 0)
                    findings.Add($"{where} ({effect.GetType().Name}): no modifier (the effect changes nothing)");
                for (int i = 0; i < used.arraySize; i++)
                {
                    SerializedProperty m = used.GetArrayElementAtIndex(i);
                    float value = m.FindPropertyRelative("value")?.floatValue ?? 0f;
                    int target = m.FindPropertyRelative("targetStatType")?.intValue ?? -1;
                    if (Mathf.Approximately(value, 0f)) findings.Add($"{where}: modifier {i} has value 0");
                    if (!max && target == (int)VitalStatType.Coin)
                        findings.Add($"{where}: vital modifier {i} targets Coin; VitalStatsBase never seeds Coin, so it does nothing (use CoinModifierEffect)");
                    if (!max && Enum.IsDefined(typeof(VitalStatType), target) == false)
                        findings.Add($"{where}: modifier {i} targets {(StatType)target}, not a VitalStatType (AdjustCurrentStats ignores it)");
                    info.Add($"{where}: {(max ? ((StatType)target).ToString() : ((VitalStatType)target).ToString())} {value:+0.##;-0.##}");
                }
            }
            Report(r, findings, $"{droppable.Count} droppable item(s) and every item effect are usable");
            r.notes.AddRange(info);
        }

        private static void DropTables(QACaseResult r)
        {
            List<string> findings = new();
            List<string> info = new();
            List<DepotItem> depots = LoadAll<DepotItem>("Assets/SO");
            if (depots.Count == 0)
            {
                r.verdict = nameof(QAVerdict.SKIP);
                r.notes.Add("SKIP: no DepotItem under Assets/SO");
                return;
            }
            foreach (DepotItem depot in depots)
            {
                string where = PathOf(depot);
                int emptySlots = depot.listItems.Count(s => s == null || s.DepotItem == null);
                if (emptySlots > 0) findings.Add($"{where}: {emptySlots} slot(s) with no item");
                float declared = 0f;
                foreach (DropRateItemSlot tier in depot.DeclaredTiers)
                {
                    if (tier == null) { findings.Add($"{where}: empty tier row"); continue; }
                    if (tier.TierRate < 0f) findings.Add($"{where}: tier {tier.RarityTier} has a negative rate {tier.TierRate:0.##}");
                    declared += Mathf.Max(0f, tier.TierRate);
                    if (tier.TierRate > 0f && !depot.HasItemOfTier(tier.RarityTier))
                        findings.Add($"{where}: tier {tier.RarityTier} has rate {tier.TierRate:0.##}% but no item (that share of rolls drops nothing)");
                }
                if (declared > 100.001f) findings.Add($"{where}: declared tier rates sum to {declared:0.##}% (> 100; the last tiers can never be rolled)");
                foreach (RarityTierItem tier in depot.listItems.Where(s => s != null && s.DepotItem != null).Select(s => s.RarityTier).Distinct())
                {
                    if (!depot.DeclaredTiers.Any(t => t != null && t.RarityTier == tier && t.TierRate > 0f))
                        findings.Add($"{where}: items of tier {tier} have no tier rate (they can never drop)");
                }
                var dupes = depot.listItems.Where(s => s != null && s.DepotItem != null).GroupBy(s => s.DepotItem).Where(g => g.Count() > 1);
                foreach (var dupe in dupes) info.Add($"{where}: {dupe.Key.name} listed {dupe.Count()} times (doubles its chance in its tier)");
                info.Add($"{where}: {depot.listItems.Count} slot(s), drop rate {depot.TotalDropRate:0.##}%");
            }
            Report(r, findings, $"{depots.Count} drop table(s) consistent");
            r.notes.AddRange(info);
        }

        private const string PlayerClassFolder = "Assets/SO/Player";

        // The classes the Champion monuments apply: PlayerData assets in a per-class sub folder. BaseData (the
        // pre-pick template at the folder root) is listed as info only.
        private static List<PlayerData> PlayableClasses(List<string> info)
        {
            List<PlayerData> classes = new();
            foreach (PlayerData data in LoadAll<PlayerData>(PlayerClassFolder))
            {
                string path = PathOf(data);
                if (System.IO.Path.GetDirectoryName(path)?.Replace('\\', '/') == PlayerClassFolder)
                {
                    info?.Add($"{path}: template, not judged");
                    continue;
                }
                classes.Add(data);
            }
            return classes;
        }

        private static void ClassData(QACaseResult r)
        {
            List<string> findings = new();
            List<string> info = new();
            List<PlayerData> classes = PlayableClasses(info);
            if (classes.Count == 0)
            {
                r.verdict = nameof(QAVerdict.SKIP);
                r.notes.Add($"SKIP: no PlayerData in a sub folder of {PlayerClassFolder}");
                return;
            }
            foreach (PlayerData data in classes)
            {
                string name = data.name;
                if (data.Stats == null) findings.Add($"{name}: no stat asset");
                else
                {
                    StatSheet sheet = new(data.Stats);
                    float hp = sheet.GetStatValue(StatType.HP), move = sheet.GetStatValue(StatType.MoveSpeed);
                    if (hp <= 0f) findings.Add($"{name}: HP {hp:0.##} (dies on spawn)");
                    if (move <= 0f) findings.Add($"{name}: MoveSpeed {move:0.##} (cannot move)");
                    info.Add($"{name}: HP {hp:0.#}, Mana {sheet.GetStatValue(StatType.Mana):0.#}, PhysicalDamage {sheet.GetStatValue(StatType.PhysicalDamage):0.#}, MoveSpeed {move:0.##}");
                }
                if (data.animationController == null) findings.Add($"{name}: no animationController");
                if (data.Weapon == null) findings.Add($"{name}: no default Weapon (the class cannot attack)");
                else if (data.Weapon.Stats == null || data.Weapon.Stats.StageCount == 0) findings.Add($"{name}: weapon {data.Weapon.name} has no WeaponStats or no attack stage");
                List<AbilityBinding> bindings = data.AbilityBindings ?? new List<AbilityBinding>();
                foreach (AbilitySlot slot in Enum.GetValues(typeof(AbilitySlot)))
                {
                    int count = bindings.Count(b => b != null && b.Slot == slot);
                    if (count == 0) findings.Add($"{name}: no ability bound to {slot} (key does nothing)");
                    else if (count > 1) findings.Add($"{name}: {count} abilities bound to {slot} (only one can be used)");
                }
                int nulls = bindings.Count(b => b == null || b.Ability == null);
                if (nulls > 0) findings.Add($"{name}: {nulls} binding(s) with no AbilityDefinition");
            }
            Report(r, findings, $"{classes.Count} classes complete");
            r.notes.AddRange(info);
        }

        private static void BoundAbilities(QACaseResult r)
        {
            List<string> findings = new();
            List<string> info = new();
            Dictionary<string, string> ids = new(StringComparer.Ordinal);
            List<PlayerData> classes = PlayableClasses(null);
            foreach (PlayerData data in classes)
            {
                foreach (AbilityBinding binding in data.AbilityBindings ?? new List<AbilityBinding>())
                {
                    if (binding == null || binding.Ability == null) continue;
                    AbilityDefinition a = binding.Ability;
                    string where = $"{data.name}.{binding.Slot} ({PathOf(a)})";
                    if (string.IsNullOrWhiteSpace(a.Id)) findings.Add($"{where}: no Id");
                    else if (ids.TryGetValue(a.Id, out string other) && other != PathOf(a)) findings.Add($"{where}: Id {a.Id} also used by {other}");
                    else ids[a.Id] = PathOf(a);
                    if (string.IsNullOrWhiteSpace(a.DisplayName)) findings.Add($"{where}: no DisplayName (blank hotbar slot)");
                    if (a.Cooldown <= 0f) findings.Add($"{where}: Cooldown {a.Cooldown:0.##} (can be cast every frame)");
                    if (a.Effects == null || a.Effects.Count == 0) findings.Add($"{where}: no effect (the cast does nothing)");
                    else if (a.Effects.Any(e => e == null)) findings.Add($"{where}: {a.Effects.Count(e => e == null)} empty effect slot(s)");
                    if (a.Conditions != null && a.Conditions.Any(c => c == null)) findings.Add($"{where}: empty condition slot");
                    if (a.ActivationType == AbilityActivationType.Hold && a.MaxHoldTime <= 0f) info.Add($"{where}: Hold ability with MaxHoldTime 0");
                    foreach (StatCost cost in a.Costs ?? new List<StatCost>())
                    {
                        if (cost == null) { findings.Add($"{where}: empty cost row"); continue; }
                        if (cost.value < 0f) findings.Add($"{where}: negative {cost.statType} cost {cost.value:0.##} (a cast pays the caster)");
                        if (cost.statType == VitalStatType.Coin) findings.Add($"{where}: Coin cost (VitalStatsBase never seeds Coin, so the cast is always refused)");
                    }
                    // GetCostValues() returns only the first cost of a stat; a second one is ignored when paying.
                    foreach (var dupe in (a.Costs ?? new List<StatCost>()).Where(c => c != null).GroupBy(c => c.statType).Where(g => g.Count() > 1))
                        findings.Add($"{where}: {dupe.Count()} {dupe.Key} costs (only the first is read by GetCostValues)");
                    if (a.Icon == null) info.Add($"{where}: no Icon (hotbar shows no picture)");
                    info.Add($"{where}: {a.DisplayName}, {a.ActivationType}, cooldown {a.Cooldown:0.##}s, costs " +
                             $"{string.Join(" + ", (a.Costs ?? new List<StatCost>()).Where(c => c != null).Select(c => $"{c.value:0.##} {c.statType}"))}, {a.Effects?.Count ?? 0} effect(s)");
                }
            }
            Report(r, findings, $"every ability bound to {classes.Count} classes is complete");
            r.notes.AddRange(info);
        }

        // BUG-122 static half: since 49c58e35 PlayerDeathState emits ON_PLAYER_DEATH from its AnimationOnAction()
        // override, i.e. only when the death clip sends the AnimationOnAction event (was: EndRangeTrigger).
        private static void DeathClipEvents(QACaseResult r)
        {
            List<string> findings = new();
            List<string> info = new();
            foreach (PlayerData data in PlayableClasses(null))
            {
                if (data.animationController == null) continue;
                List<AnimationClip> clips = data.animationController.animationClips
                    .Where(c => c != null && (c.name.IndexOf("die", StringComparison.OrdinalIgnoreCase) >= 0 || c.name.IndexOf("death", StringComparison.OrdinalIgnoreCase) >= 0))
                    .Distinct().OrderBy(c => c.name, StringComparer.Ordinal).ToList();
                if (clips.Count == 0)
                {
                    findings.Add($"{data.name}: no clip named *Die* / *Death* in {data.animationController.name}");
                    continue;
                }
                List<string> missing = clips.Where(c => !AnimationUtility.GetAnimationEvents(c).Any(e => e.functionName == "AnimationOnAction"))
                    .Select(c => c.name).ToList();
                if (missing.Count > 0)
                    findings.Add($"{data.name}: {missing.Count} of {clips.Count} death clip(s) have no AnimationOnAction event (no ON_PLAYER_DEATH, no Game Over): {string.Join(", ", missing)}");
                else info.Add($"{data.name}: {clips.Count} death clip(s) send AnimationOnAction");
            }
            Report(r, findings, "every class death clip sends AnimationOnAction");
            r.notes.AddRange(info);
        }
    }
}
