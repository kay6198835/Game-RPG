# Abilities — Changelog

Newest first. Entries before 2026-10-05 reconstructed from `CLAUDE.md` history, `docs/CHANGELOG-DOCS.md`, the bug files and `git log`.

## 2026-09-28 — Compile restored (BUG-092 partial)
- **Commit:** `5b035b73` (verified with `git log -S"private float perTime"`)
- **Changed:** `private float duration;` → `private float perTime;`
- **From → To:** field `perTime` missing, two `CS0103` errors → field declared, project compiles
- **Why:** fix the build break from `c0067f4`.
- **Bugs:** BUG-092 build break fixed; serialization defect (`perTime`, `timeCount` private, unserialized) still open.

## 2026-09-28 — Shared projectile body for abilities and weapons
- **Commits:** `5b035b73`, `b7a0af5e`, `7c637c0e`
- **Changed:** `SpawnProjectileBase` rewritten; `ProjectileBody` + `ProjectileConfig` added; `IProjectilePayload` added; `SpawnEffectBase.dir` moved to `SpawnProjectileEffect`
- **From → To:**
  - `SpawnProjectileBase` owning `Rigidbody2D` + `CircleCollider2D` and its own trigger → `[RequireComponent(ProjectileBody)]`, implements `IProjectilePayload.OnHit()`; serialized `targetMask`, `blockMask`, `pierceCount`
  - Lifetime despawn in `SpawnMono` → owned by `ProjectileBody` (`SpawnMono` gets lifetime 0)
  - No target filtering → `targetMask` / `blockMask` filtering, release on hit
- **Why:** "update architecture projectile and logic spawn by weapon and ability" — one projectile for both paths.
- **Bugs:** closes the two BUG-075 follow-ups (no despawn on hit, no layer mask).

## 2026-09-25 — ADR-0005 damage contract change
- **Commits:** `8c3c350a`, `5007d3f6`, `83954bc6`, `4faf6233`, `b0b13379`, `c0067f47`
- **Changed:** `IAbilityServices`, `AbilityHolderBase`, effect lists
- **From → To:**
  - `IAbilityServices` = Pool / Stats / ResourceReceiver / Vital / NegativeReceiver → **Pool only**; hit target via `AbilityContext.Target`
  - Player-only `AbilityHolder` → `AbilityHolderBase` shared with `EntityAbilityHolder`
  - Abilities required an equipped weapon → independent of the weapon
  - `Utility.ModifierStatsCalculate` returned the total → returns the delta
  - `AbilityEffectDefinition` lists without initialisers → all `= new()`
  - `RecoveryReductionPerTimeForDuration.perTime` deleted (build break)
- **Why:** one `AbilityDefinition` for player or enemy.
- **Bugs:** closes BUG-074, BUG-082, BUG-068 (`GetAbility()` half); opens BUG-092; BUG-071 → PARTIAL.

## 2026-09-22 — Single gate; v2 damage path restored
- **Commits:** `723fab1a`, `2a83469a`, `8b23174b`, `379c267`
- **Changed:** `AbilityDefinition.TryStart()`; `Casting()` → `TryCast()`; conditions deleted; `SpawnProjectileBase` 2D trigger; `LightningController` overlap query
- **From → To:**
  - Three condition walks + ungated `TryPayCost()` → one generic gate `TryStart()` over `StatType`
  - `HasEnoughManaCondition` / `NotDeadCondition` → deleted
  - 3D `OnTriggerEnter(Collider)` → `OnTriggerEnter2D(Collider2D)`
  - Lightning target query as comments → `OverlapCircleNonAlloc`, set + invoke per target
- **Why:** BUG-076 / BUG-077 / BUG-085 / BUG-075 / BUG-072.
- **Bugs:** closes BUG-075, BUG-076, BUG-077, BUG-085, BUG-091; BUG-072 code complete (prefab mask open).

## 2026-09-15 … 09-21 — Effect layer replaced; Paladin set
- **Commits:** `8295539` … `7cceda2e`, `73ab8e77`, `e2cb75e4`, `e02bf3bc`, `edd7454d`
- **Changed:** effect and runtime layers rebuilt
- **From → To:** `DamageInFront` / `LungeForward` / `ShootObject` / `PlayDebugLog` effects + `SpiritOrbProjectile` → `SpawnEffectBase` / `StatsEffectBase` hierarchies + `SpawnMono` controllers; enum `SkillState` → `AbilityState` (no `None`); two disjoint cost lists
- **Why:** Paladin ability set (Consecrate, Blessed Slash, Blessing, Avatar of Light).
- **Bugs:** BUG-075 … BUG-079 filed.

## 2026-09-09 — Promoted from prototype
- **Commits:** `9b8d40f`, `5c7afba`
- **Changed:** 17 files moved from `prototypes/skill-enhance-abilities/` to `Assets/Script/System/Abilities/`; `AbilityHolder` rewritten to drive v2
- **From → To:** player abilities via v1 `ActivateSkill` → via v2 `AbilityDefinition`
- **Why:** composition over inheritance for abilities.
- **Bugs:** none at the time.
