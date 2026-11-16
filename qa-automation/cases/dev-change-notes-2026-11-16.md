# Dev change notes for QA automation — 2026-11-16

**From:** owner (dev), via the sprint 23 fix branch `origin/feature/fix-bug-sprint-23` (PR #69, base `sprint-23`).
**To:** the Friday QA automation task and whoever maintains `Assets/QA/` and `qa-automation/`.
**Why:** the game logic changed this sprint. Several automated cases still test the old contracts and will report
false FAIL / false KNOWN once the branch reaches `sprint-23`. Update the suites, the case catalog and
`config/known-issues.json` to match the code as it is now.

Evidence tier: STATIC (read from code and asset YAML at `179cf065`). Nothing below was run in Unity.
Bug status is still changed only by the Saturday triage; this file does not close any bug.

## 1. Must update — tests that will break

### 1.1 QA-ENT-13 — `IEffectReceiver.AdjustMaxStats()` changed signature and source

- Commits: `25d869fb`, `8236f9d1`.
- **Was:** `AdjustMaxStats(StatModifierGroup group, AbilityContext context, object source, Action callback = null)` —
  the caller chose the modifier source.
- **Now:** `AdjustMaxStats(StatModifierGroup group, AbilityContext context = null, Action callback = null)`. The
  receiver spawns a pooled `SourceEffect` instance (`EffectReceiverBase.SetEffectSource()`) and uses **that
  instance** as the modifier source. The instance is not returned to the caller.
- All four `IEffectReceiver` methods now default `context` to `null`.
- **Breaks:** `Assets/QA/Runtime/Suites/QAEnemySuite.Behaviour.cs:481-486` still creates `object source = new()`
  and calls `_stats.RemoveModifiersFromSource(source)`. Nothing is removed, so the check "removing the source
  restores the stat" FAILs, and the Defense +5 modifier stays on the enemy and skews the
  `AdjustMaxForDuration` check that follows (its `before` value is no longer clean).
- **Update to:** read the source another way, or assert on the stat only and clear with
  `StatSheet.ClearModifiers()` / a fresh enemy between steps. If dev later makes `AdjustMaxStats` return its
  source (proposed, not done), use the returned value.
- **Also check:** with `context == null` the `SourceEffect` spawns at world (0, 0). Since `cfb6a230` a
  `SourceEffect` with `lifetime > 0` (timed buffs) is released to the pool when its lifetime ends; one spawned by
  `AdjustMaxStats` (`lifetime = 0`, permanent item buffs) is never released. A case that counts live pooled
  objects will see one per `AdjustMaxStats` call.

### 1.2 QA-ENT-14 — vital modifier sign is fixed (BUG-117)

- Commits: `f528c087`, `25d869fb`.
- `EffectReceiverBase.AdjustCurrentStats()` now passes `Math.Abs(modifier.Value)` to `Reduction()` / `Recovery()`;
  the direction comes from the sign (`VitalModifier.ImpactType`). `VitalModifier.Value` and `StatModifier.Value`
  return the signed value again.
- The existing case logic (+chunk heals, −chunk damages by exactly chunk) now matches the code.
- **Update:** remove the `QA-ENT-14` → `BUG-117` mapping from `config/known-issues.json` so a PASS is reported as
  PASS (or a regression as FAIL), not KNOWN. Report it as "fixed in code, PASS" for the triage.
- Note: `ModifierType` is ignored for vital modifiers — every value is applied as flat. Keep building test
  modifiers as `ModifierType.Flat`.

### 1.3 QA-ITEM-03 — item pickup should now work (BUG-120)

- Commits: `25d869fb` (code), `25d2d5e4` (data).
- `ModifierMaxStatsEffect.Apply()` → `IEffectReceiver.AdjustMaxStats(group)`;
  `ModifierCurrentStatsEffect.Apply()` → `AdjustCurrentStats(group)`.
- Assets rebound to concrete scripts:
  - `SO/Item/Effect/New Stat Modifier Effect Definition.asset` → `ModifierMaxStatsEffect` (STR +3, INT +1, flat),
    used by item `BaseOrds`.
  - `… - Copy.asset` → `ModifierMaxStatsEffect` (STR +5, flat), used by item `Test`.
  - `New Recovery Effect Definition.asset` → `ModifierCurrentStatsEffect` (HP +10 vital), not used by any item.
- Both `BaseOrds` and `Test` are in `Depot Item 1`, the depot QA-ITEM-02 rolls from.
- **Expected now:** Q on the dropped item collects it, `ON_COLLECT_ITEM` fires once, no
  `NullReferenceException` at `ItemController.cs:39`.
- **Update:** remove the `QA-ITEM-03` → `BUG-120` mapping; consider adding an assertion that the picked item's
  stat modifiers reached the player (e.g. `BaseOrds`: STR +3 and INT +1 on the player's `StatSheet`).

### 1.4 QA-CMB-01 / melee damage — `Weapon.FinalDamage()` removed (BUG-125)

- Commit: `b92b6f2a`.
- `MeleeWeapon` now calls `receiver.TakeDamage(finalDamage, …)` with
  `WeaponHolderBase.CalculateCurrentDamage()` = `PhysicalDamage + CurrentStage.attackDamage` (+ crit). Stage damage
  is added **once** (it was added twice).
- The catalog expectation for QA-CMB-01 already states the correct formula; any case or tolerance that was tuned
  to the doubled value must be re-tuned. Enemy melee follows the same path.
- **Ranged basic attacks now use the same formula** (owner decision 2026-11-16, inbox NOTE-20261104-4):
  `PhysicalDamage + CurrentStage.attackDamage` (+ crit, rolled once per volley). `RangeWeapon.OnActivate()`
  stores that value in a field and `OnHit()` deals it (owner `3a2659ce`). A projectile still in flight when the
  next volley fires deals the newer volley's value — do not assert per-projectile crit independence. Re-tune QA-CMB-04 / QA-CMB-05 (Archer, Mage,
  enemy ranged) expectations from stage-only damage to the stat-scaled value.
- **Damage model rule:** basic attacks (melee and ranged) scale with `PhysicalDamage`; `MagicDamage` is used only
  by skills that use the magic stat (today `SpawnSummonEffect`). Do not expect `MagicDamage` on a weapon hit.

### 1.5 QA-CMB-04.Archer — Arrow has a collider (BUG-095)

- Commit: `25d869fb`. `Arrow.prefab` now has a `BoxCollider2D` (1 × 1) on the `ProjectileBody` GameObject;
  `ProjectileBody.Awake()` sets it to trigger.
- `ProjectileBody` now has `[RequireComponent(typeof(Collider2D))]` and reads the collider **without a null
  check** — a projectile prefab with no collider throws in `Awake()`. All six current `ProjectileBody` prefabs
  have one.
- **Update:** remove the `QA-CMB-04.Archer` → `BUG-095` mapping.

### 1.6 QA-STATIC-03 — Knight enemy assets deleted (BUG-108)

- Commit: `041bf198`. Deleted: `SO/Enemy/Enemy - Knight{,1-4}.asset`, `SO/Skill/Enemy - Knight.asset`,
  `SO/Enemy/Knight/`, `SO/Enemy/Weapon/` (whole folder), `SO/Weapons/KnightWeapon/`,
  `Prefab/Weapon/{KnightWeapon,WeaponEntity}.prefab`, `SO/Database/EnemyPrefab.prefab` (an unused copy — the live
  enemy prefab `Prefab/Enemy/EnemyPrefab.prefab` is unchanged),
  `AnimationController/2 Direction/Enemy/Knight_Controller.overrideController`.
- No committed asset stores `weaponSO:` or `aima:` any more.
- **Update:** remove the `QA-STATIC-03` `weaponSO` → `BUG-108` mapping. Any static check, fixture or scene builder
  that names a Knight asset path must drop it. (Paladin player art is also named `Knight_*` and was not touched.)

### 1.7 QA-STATIC-01 / QA-STATIC-02 — ShootSpirit assets deleted (BUG-073, BUG-090)

- Deleted: `Assets/SO/Skill/ShootSpirit/` (`ShootSpirit.asset`, `SpiritBomd.asset`). `PlayerTest.prefab`
  `AbilityHolder.abilityBindings` is now `[]` (it was overwritten at runtime from `PlayerData` anyway).
- No asset references the missing script guid `ac9ac7c011812d042ac992007bc0cf48` or the deleted condition asset
  guid `02cbacd1d0342774abb852a2ed63f9b4` any more.
- **Update:** remove the `QA-STATIC-01` → `BUG-073` and `QA-STATIC-02` → `BUG-090` mappings; both checks should now
  PASS on the ShootSpirit findings. Any fixture naming `ShootSpirit` / `SpiritBomd` must drop it.

## 2. New behaviour to cover (new cases suggested)

| Area | What changed | Suggested case |
|---|---|---|
| Coin item effect (BUG-123) | `CoinModifierEffect` is concrete with menu `Item SO/Effect/Coin/ Adjust Coins`; asset `SO/Item/Effect/Coin Effect.asset` (`coinAmount: 10`, `ResourceType: Coin`). Not attached to any item yet. | When an item carries it: pickup raises `IWallet` coins by exactly `coinAmount`. Until then, a static check that the asset loads with a non-null script. |
| Recovery clamp (BUG-119) | `VitalStatsBase.Recovery()` clamps at the max stat; `Reduction()` is guarded and clamps at 0 (merge `8459e384`). | QA-VIT-05 should now PASS — remove its `BUG-119` known-issue mapping after the first green run. |
| Dash (merge `8459e384`) | New `PlayerDashState` (Space), animator bool `Move`, flag `IsDash`; `PlayerBasicState.LogicUpdate()` checks `IsDash` **before** `IsDeath`; dash speed uses `MoveSpeed`. | Dash moves the player and returns to Idle/Move; a lethal hit during a dash still reaches `PlayerDeathState`. |
| Stat item buffs stack | Each `AdjustMaxStats` call now has its own source instance. | Pick up two stat items: both bonuses apply and add up. |

## 3. Design decisions the suites must respect

- **Player is unarmed until a Champion is picked** (owner, NOTE-20261116-3). Weapon and attack style come from the
  Champion's `CharacterData`. A player case that attacks must pick a Champion first; "player has a weapon at
  spawn" is not an expectation. The enemy half of BUG-106 (QA-ENT-02) is unchanged and fixed.
- **Weapon pickup / drop / swap is deferred** (BUG-110). Do not write or expect cases for it this phase.
- **`SourceEffect` release (`cfb6a230`).** Timed buffs release their source after `lifetime = duration`; permanent
  item buffs (`lifetime = 0`) are never released, so a pooled source is never reused while it still owns
  modifiers. Regression case to add: apply a timed buff and a permanent item buff, let the timed one expire,
  assert the item buff is still there.

## 4. Checklist for the QA update

- [ ] `QAEnemySuite.Behaviour.cs` QA-ENT-13: stop removing by a caller-made source (1.1).
- [ ] `config/known-issues.json`: drop mappings for QA-ENT-14 (BUG-117), QA-ITEM-03 (BUG-120),
      QA-CMB-04.Archer (BUG-095), QA-STATIC-03 `weaponSO` (BUG-108), QA-STATIC-01 (BUG-073) and QA-STATIC-02
      (BUG-090) ShootSpirit findings; QA-VIT-05 (BUG-119) after a green run.
- [ ] `cases/case-catalog.md`: update QA-ENT-13 / QA-ITEM-03 expectations; add the new cases from section 2.
- [ ] Re-tune any melee damage tolerance built on the doubled stage damage (1.4).
- [ ] Remove Knight asset paths from static checks and fixtures (1.6).
- [ ] Player-flow cases: pick a Champion before attacking; no weapon pickup cases (section 3).
