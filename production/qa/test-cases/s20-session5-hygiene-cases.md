# Sprint 20 — Session 5: Hygiene — Manual Test Cases

**Stories**: S20-11 (BUG-104, BUG-103, BUG-102, BUG-068), S20-12 (BUG-073, BUG-090, BUG-112)
**Story type**: S20-11 Logic + UI — BUG-103/068 need automated tests (impossible, BUG-084; manual substitute), BUG-102 UI screenshot is BLOCKING (`production/qa/evidence/s20-11/`); S20-12 Config/Data (ADVISORY)
**Evidence**: this sheet; BUG-102 screenshot; `git diff` for each one-liner

## Common preconditions

- Session 1 PASS on the same commit. Commit: `________`
- Console: Clear on Play ON, Collapse OFF; all log levels shown.

---

### S5-01 — No unguarded state-change log in code
- **Story**: S20-11 (BUG-104)
- **Preconditions**: Fix committed.
- **Steps**:
  1. Open `Assets/Script/Character/Player/PlayerState.cs`; inspect `Enter()` and `Exit()`.
- **Expected Result**: No `Debug.Log(... "Enter" / "Exit" ...)` line, or every such line is inside `#if UNITY_EDITOR`.
- **Actual Result**:
- **Pass/Fail**:

### S5-02 — Console not flooded by state changes
- **Story**: S20-11 (BUG-104)
- **Preconditions**: S5-01; run in progress.
- **Steps**:
  1. Move, attack 3 combos, take a hit (≈20 state changes).
  2. Count Console lines ending `: Enter` / `: Exit`.
- **Expected Result**: 0 lines if the logs were deleted. If guarded by `#if UNITY_EDITOR` they may appear in the Editor; then also confirm a Development Build `Player.log` has 0 such lines, or mark that half NOT RUN.
- **Actual Result**:
- **Pass/Fail**:

### S5-03 — RecoveryTime gates ranged fire
- **Story**: S20-11 (BUG-103)
- **Preconditions**: Ranged weapon equipped; on a scratch copy (not committed) set stage-1 `RecoveryTime` = 2 s, or use the shipped value and record it.
- **Steps**:
  1. Hold / spam LMB for 10 s.
  2. Count shots (or timestamps of arrow spawns).
- **Expected Result**: Consecutive shots are ≥ `RecoveryTime` apart (10 s at 2 s → at most 6 shots).
- **Actual Result**:
- **Pass/Fail**:

### S5-04 — Ranged weapon still fires with RecoveryTime 0
- **Story**: S20-11 (BUG-103, regression)
- **Preconditions**: Stage `RecoveryTime` = 0 (scratch copy) or a weapon whose value is 0.
- **Steps**:
  1. Fire 5 times at an enemy.
- **Expected Result**: Every press fires (rate bounded only by the attack animation); first shot is never blocked.
- **Actual Result**:
- **Pass/Fail**:

### S5-05 — Confirm panel texts correct (screenshot)
- **Story**: S20-11 (BUG-102)
- **Preconditions**: Menu → New Game; first room with champion monuments.
- **Steps**:
  1. Interact (G) with a champion monument.
  2. Capture a screenshot of the open confirm panel; save to `production/qa/evidence/s20-11/bug-102-confirm-panel.png`.
- **Expected Result**: Title shows a title, message shows the question; Confirm and Cancel button labels show on the buttons — not "Xác nhận" as title and "Hủy" as message. Screenshot file exists.
- **Actual Result**:
- **Pass/Fail**:

### S5-06 — Confirm / Cancel behaviour
- **Story**: S20-11 (BUG-102)
- **Preconditions**: S5-05 panel open.
- **Steps**:
  1. Press Cancel.
  2. Interact again; press Confirm.
- **Expected Result**: Cancel closes the panel with no character change; Confirm applies the champion once and opens the doors; 0 exceptions.
- **Actual Result**:
- **Pass/Fail**:

### S5-07 — Champion interact without GameplayUI does not throw
- **Story**: S20-11 (BUG-102)
- **Preconditions**: Open `Assets/Scenes/Main/LoadRandomMap.unity` alone (no `GameplayUI` loaded).
- **Steps**:
  1. Enter Play Mode; walk to a champion monument; press G.
- **Expected Result**: 0 `NullReferenceException` from `UIEvents` (panel may not appear — no subscriber).
- **Actual Result**:
- **Pass/Fail**:

### S5-08 — Ability keys with no ability equipped do not throw
- **Story**: S20-11 (BUG-068)
- **Preconditions**: New run, champion not yet picked (no ability equipped), or a slot left unbound.
- **Steps**:
  1. Press and hold `1`, `2`, `3`, `4` in turn.
- **Expected Result**: Nothing casts; 0 NRE from `AbilityHolderBase.CurrentActivationType`.
- **Actual Result**:
- **Pass/Fail**:

### S5-09 — Enemy ability holder before first cast does not throw
- **Story**: S20-11 (BUG-068)
- **Preconditions**: A room with an enemy that has `EntityAbilityHolder` (if none exists in content, mark SKIP with reason).
- **Steps**:
  1. Enter the room; let the enemy reach cast range and act for 15 s.
- **Expected Result**: 0 NRE from `CurrentActivationType`; enemy either casts or attacks normally.
- **Actual Result**:
- **Pass/Fail**:

### S5-10 — ShootSpirit assets have no missing script
- **Story**: S20-12 (BUG-073, BUG-090)
- **Preconditions**: Fix committed (assets re-pointed or deliberately deleted).
- **Steps**:
  1. Open `Assets/SO/Skill/ShootSpirit/MainEffect/ShootSpirit.asset`, `Assets/SO/Skill/ShootSpirit/SpiritBomd.asset`, `Assets/Prefab/Bullet/SpiritProjcetile.prefab` (skip any deleted; record deletion).
  2. Search all `.asset` / `.prefab` files for GUIDs `ac9ac7c011812d042ac992007bc0cf48`, `405e5c1b`, `02cbacd1d0342774abb852a2ed63f9b4`.
- **Expected Result**: No "The associated script can not be loaded" / "Missing (Mono Script)"; no `None (Missing)` entry in any Conditions list; 0 GUID hits in assets (or each GUID resolves to an existing `.meta`).
- **Actual Result**:
- **Pass/Fail**:

### S5-11 — Merchant / NPC prefabs have no missing script
- **Story**: S20-12 (BUG-112)
- **Preconditions**: Fix committed.
- **Steps**:
  1. Open `Assets/Prefab/NPC/Merchant.prefab` and `Assets/Prefab/NPC/NPC.prefab`.
  2. `git grep` for `fc1ed29122892f547a5676c047746b53` and `7a5a50304ffbae14788a47bb36ced296`.
- **Expected Result**: No "Missing (Mono Script)" component; each GUID either resolves to a committed `.cs.meta` or no longer appears in the prefabs.
- **Actual Result**:
- **Pass/Fail**:

### S5-12 — Full run Console has 0 missing-script warnings
- **Story**: S20-12
- **Preconditions**: S5-10, S5-11 PASS.
- **Steps**:
  1. Menu → New Game → visit at least the start room, one combat room and the Shop room (Merchant).
  2. Search Console for "script on this Behaviour is missing" and "Missing".
- **Expected Result**: 0 matches.
- **Actual Result**:
- **Pass/Fail**:
