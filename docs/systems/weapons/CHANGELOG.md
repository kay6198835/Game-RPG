# Weapons + Projectiles — Changelog

Newest first. Entries before 2026-10-05 reconstructed from `CLAUDE.md` history, `docs/CHANGELOG-DOCS.md` and `git log`.

## 2026-09-28 — Ranged weapon rebuilt on ProjectileBody; v1 ability fields removed
- **Commits:** `5b035b73`, `b7a0af5e`, `7c637c0e`
- **Changed:** `RangeWeapon`, `RangeAttackSO`, `RangeWeaponStats`, `Weapon`, `WeaponStats`, `AttackSO`, `WeaponHolderBase`; `bullet.cs` and `BulletDataSO.cs` deleted
- **From → To:**
  - `RangeWeapon.poolManager` never assigned (silent no-fire) → `[Inject] Construct(IObjecPoolService)`; `WeaponHolderBase.Equid()` injects every weapon via `IObjectResolver.InjectGameObject`
  - `Transform firePoint` → `Vector2 firePoint` computed from owner position + aim × `spawnOffset`
  - `bullet` prefab + `BulletDataSO` (type, lifetime, dmg, speed, targetMask) → `ProjectileBody` + `ProjectileConfig` (speed, lifetime, targetMask, blockMask); damage from `currentStage.attackDamage` through `RangeWeapon.OnHit()`
  - `RangeAttackSO` private fields (`bulletPrefab`, `bulletData`, …) → auto-properties (`ProjectilePrefab`, `ProjectileCount`, `SpreadAngle`, `RecoveryTime`, `ProjectileConfig`)
  - `CanAttack()` checked `firePoint`, `poolManager` and `Time.time >= nextFireTime` → checks `StatsRange` and `_objecPoolService` only
  - `Weapon.currentAbilitySO`, `WeaponStats.AbilityWeapon` / `.SkillWeapon`, `AttackSO.ability` (Abilities v1) → removed / commented out
  - `Weapon` → `[RequireComponent(Collider2D)]`
- **Why:** "update architecture projectile and logic spawn by weapon and ability"; abilities no longer come from the weapon (ADR-0005).
- **Bugs:** closes BUG-064 (sub-item 7); opens BUG-093 (cooldown check dropped).

## 2026-09-25 — WeaponHolderBase; enemies use real weapons
- **Commits:** `59d871a9`, `f7d98b19`, `b0b13379`
- **Changed:** `WeaponHolderBase` + `IWeaponHolder`; `Weapon` decoupled from the player; `RangeWeapon.poolManager` lost its inert `[SerializeField]`
- **From → To:** player-only `WeaponHolder` → shared base for player and enemy; enemy `EntityAttack` → weapon prefab
- **Why:** ADR-0005 Amendment 2-3.
- **Bugs:** closes BUG-043; BUG-064 sub-item 7 re-scoped.

## 2026-09-01 — `attackDamege` renamed (silent data loss)
- **Commit:** `ddcc0a5c` ("coding")
- **Changed:** `AttackSO.attackDamege` → `attackDamage`
- **From → To:** serialized key `attackDamege` → `attackDamage`, with no `[FormerlySerializedAs]`
- **Why:** typo fix (inferred; commit message gives none).
- **Bugs:** opens BUG-095 (filed 2026-10-05) — `SnS_State1-3.asset` were never re-saved and load `attackDamage = 0`, so the sword deals `PhysicalDamage` only.

## 2026-08-28 — Melee combo chain
- **Commit:** `da7b3ebe`
- **Changed:** combo chain start/advance
- **From → To:** chain did not start/advance reliably → `CanChain()` / `AttackStages` drive it
- **Why:** combo attack (demo checklist item 12).
- **Bugs:** none.
