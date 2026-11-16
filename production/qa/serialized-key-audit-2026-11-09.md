# Serialized-key audit — field renames without `[FormerlySerializedAs]`

**Date:** 2026-11-09 · **Branch:** `origin/feature/polish-game` (after the stat split) · **Evidence tier:** STATIC
**Tool:** `qa-automation/scripts/audit-serialized-keys.py` (new, read-only) · **Fix tool:** `qa-automation/scripts/migrate-serialized-keys.py` (rules added)

## Method

For every `MonoBehaviour` / `ScriptableObject` block in `Assets/**/*.asset|prefab|unity` (448 files), resolve the
script GUID to its C# class, collect the class and base-class serialized fields (including
`[field: SerializeField]` properties and `[FormerlySerializedAs]` aliases), then walk the YAML as a tree and compare
every key with the declared type, recursing into nested structs and lists. A key the type no longer declares is data
Unity silently stops reading.

Limits: regex-based C# parsing; a nested key renamed to a name that exists elsewhere is not detected; a rename
whose old key was already dropped by a re-save is invisible (needs git history). `QA-STATIC-03` in the Editor
(`QAStaticChecks.OrphanKeys`, `Assets/SO` only) remains the authoritative in-Editor check.

## Result

| Class | Kinds of key | File hits | Action |
|---|---|---|---|
| **Renamed, live data lost** — `ItemSO` / `WeaponSO`: `itemSprite` → `<ItemSprite>k__BackingField`, `nameItem` → `<ItemName>k__BackingField` | 2 (×2 scripts) | 7 (`PlatiumOre 0-5`, `EnemySword&Shiled`) | **Migrated** (14 keys), see below |
| Removed on purpose, nothing to carry over: `dropChance`, `value`, `style` on `ItemSO` / `WeaponSO` | 3 | 7 | Dropped on the next Editor re-save |
| Stale after the 2026-11-09 stat split: `EntityData.rangeView / angleView / idleDurationTime / moveDurationTime` (now computed), `StatHandler.statsSO`, `StatsUIController.statsSO` | 6 | 28 | None; computed from stats now |
| Removed with Abilities v1 (2026-11-04): `AttackSO.ability`, `MeleeWeaponStats.AbilityWeapon / SkillWeapon`, `MeleeWeapon.currentAbilitySO`, `AbilityHolder.*` on `SnS.prefab` | 9 | 16 | None; v1 is gone |
| Removed stat-era fields, replaced by stat assets: `EntityData.movementVelocities / rangeCheckAttack / maxHealth` | 3 | 14 | None; values live in the stat assets |
| Removed, value was empty: `AttackSO.enemyLayers` (Paladin `SnS_State1-3`, `m_Bits: 0`), `MeleeWeapon` prefab refs with `fileID: 0` | 6 | 8 | None |
| Retired content: `Enemy - Knight*` EntityData (`weaponSO`), `SO/Database/EnemyPrefab.prefab`, `Weapon/MeleeWp/SnS.prefab`, `Pistol` / `Rifle`, `Scenes/Main/Test/*` | ~30 | ~80 | None; see TD note below |
| Third party: TextMeshPro examples, VContainer `LifetimeScope` | 10 | ~20 | None |

No orphan key remains in a live role EntityData, a Paladin / Archer / Mage asset, an ability effect asset or a
stat asset other than the stale cached view fields above. Verified: ability effect fields (`statModifier`,
`impactType`) are valid (`[field: SerializeField]` on a plain field; `impactType` was replaced by the sign of the value).
`DepotItem` nested keys (`<DepotItem>k__BackingField`, `<RarityTier>k__BackingField`, `<TierRate>k__BackingField`) match their properties.

## Migration applied

`py -I qa-automation/scripts/migrate-serialized-keys.py --apply` renamed 14 keys in 7 assets
(`Assets/SO/Item/PlatiumOre 0-5.asset`, `Assets/SO/Enemy/Weapon/EnemySword&Shiled.asset`). The six PlatiumOre items had
no sprite since `ItemSO` moved to `[field: SerializeField]` properties. Open Unity once and re-save the assets.

## Follow-ups (for the weekly triage, not bug IDs)

- The audit script has no baseline: run it after any serialized-field rename.
- Consider deleting the retired content listed above (TD-043 style cleanup); it keeps producing orphan keys and Missing Script references (73 unresolved script GUIDs, most in the same retired files).
- The Editor still needs one pass: `QA-STATIC-03` and a re-save of the migrated assets.
