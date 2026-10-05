# Enemy — Changelog

Newest first. Entries before 2026-10-05 reconstructed from `CLAUDE.md` history, `docs/CHANGELOG-DOCS.md` and `git log`.

## 2026-09-28 … 10-05 — Pool-safe restart, serialized state machine
- **Commits:** `5b035b73`, `b7a0af5e`, `0bc36406`
- **Changed:** `Entity`, `EntityStateMachine`, `EntityVitalStats`
- **From → To:**
  - `stateMachine = new EntityStateMachine()` field initialiser → serialized field, `[Serializable]` class
  - No re-init on re-enable → `Entity.OnEnable()` re-initialises to `EntityIdleState` unless already there
- **Why:** pooled enemies came back in their last state (inferred from the code; commit message "range weapon").
- **Bugs:** none.

## 2026-09-25 — No weapon, no attack (ADR-0005 Amendment 3)
- **Commit:** `f7d98b19`, `b0b13379`
- **Changed:** `EntityAttack.cs` deleted; `EntityMovement.SetPositionToCheck()` lerp range
- **From → To:**
  - `EntityAttack.Attack()` hardcoding `TakeDamage(10, …)` → attack only via `EntityWeaponHolder` + `Weapon` prefab
  - `Random.Range(10,100)/100` (integer division, always 0) → `Random.Range(30,75)/100f`
- **Why:** one attack path for player and enemy; remove hardcoded damage.
- **Bugs:** closes BUG-043, BUG-088.

## 2026-09-24 / 09-25 — Shared bases; enemies can cast abilities
- **Commits:** `8c3c350a`, `59d871a9`, `83954bc6`, `4e05da46`, `e6d9603a`, `38c4b620`, `05e88133`
- **Changed:** enemy components rebuilt on `CharacterBase` / `VitalStatsBase` / `NegativeReceiverBase` / `WeaponHolderBase` / `AbilityHolderBase` / `MovementBase` / `CharacterInputBase`; `EntityData : CharacterData`; `EntityAbilityHolder` added
- **From → To:** enemy-only implementations → thin subclasses of shared bases
- **Why:** ADR-0005 — one contract for player and enemy.
- **Bugs:** BUG-066 merged with BUG-070 (same lines).

## 2026-09-06 — Enemy damage chain fixed
- **Commit:** `f3f5f08`
- **Changed:** `EntityNegativeReciver` rewritten
- **From → To:** player-only logic (PlayerInputHandler, ON_PLAYER_DEATH) on an enemy → Defense-aware `DamageCalculate()` → `EntityVitalStats` → `EntityUIController`
- **Why:** Sprint 12 entity/stat refactor.
- **Bugs:** closes BUG-053, BUG-042, NEW-2.
