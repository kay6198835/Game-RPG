# Sprint 20 — 2026-10-26 to 2026-10-30

**Status: OPEN.** Opened 2026-10-25 (`pm-weekly-kickoff`, autonomous run — no owner present).
Branch `sprint-20`, created on the remote from the `sprint-19` tip (`566a684d`, "chore(wrapup): weekly
wrap-up 2026-10-25"). `gh` CLI unavailable (`gh: program not found`) — draft PR **not** auto-created;
run manually:

```bash
gh pr create --draft --base sprint-19 --head sprint-20 --title "Sprint 20"
```

**Review mode:** lean — producer feasibility gate skipped.
**Tracker:** `sprint-20-daily-plan.md`.
**Inputs:** `sprint-19.md`, `sprint-19-daily-plan.md` (Week close), `production/qa/bug-inbox.md`,
`production/qa/bugs/BUG-106…115.md`, `production/qa/playtests/playtest-2026-10-24.md` (empty),
`git log --all` since 2026-10-19. No milestone file and no risk register exist — risks are tracked here.

---

## Sprint 19 Closure (recorded by this kickoff)

- **Verdict:** SLIPPED (from the 2026-10-25 wrap-up). Must-Have 1/9 (S19-02 only).
- **Planned velocity:** 0.05d / 4d = **0.01**. Sprint 16 ≈0.01, 17 0.05, 18 0.0, 19 0.01.
- **Off-plan owner output:** ≈3d (regen, wallet / level-up, GameplayUI stats panel, `ICharacter`
  covariance, Erika / Archer / Mage animation and ability assets, stat-reduction epic docs).
- **Playtest done:** no — `playtest-2026-10-24.md` empty. S19-01 carried for the 20th time.
- **New confirmed bugs (2026-10-25 triage):** BUG-110…115; BUG-107 → PARTIAL; BUG-104 regressed.
- **After the wrap-up:** owner commit `786c031b` "add effec component" on
  `feature/add-effect-component` (not on `sprint-19`) rewrites `VitalStatsBase.cs` and the stat
  effects (new `EffectReceiverBase`, `IEffectReceiver`). Stat / vitals bug work (BUG-071, BUG-100,
  BUG-113, BUG-114) is deferred until that branch lands, so it is not fixed twice.

---

## Sprint Goal

**One played session on record, then a dungeon run that can be cleared.** Playtest first on Monday
(21st carry — play it or sign an accepted-risk note), then the two S1 blockers (BUG-108 enemy stats,
BUG-096 sealed rooms) and the weapon trio in one `WeaponHolderBase` session.

---

## Capacity

- Total days: 5
- Buffer (20%): 1 day
- Available: 4 days

Planned-work velocity has been ≤0.05 for four sprints. Planned load stays at ≈1.4d Must and ≈1.0d
Should; owner feature work is an explicit story capped at 0.5d, gated behind S20-01.

0.1d is reserved on **Saturday 2026-10-31** for the weekly playtest (`tests/smoke/critical-paths.md`
+ the `playtest-sheet.sh` sheet).

---

## Carryover from Previous Sprint

| Task | Times carried | Reason | New Estimate |
|------|---------------|--------|--------------|
| Playtest / Play Mode smoke (S19-01) | **21st** | Never run ahead of feature work | 0.3d — Mon, first (S20-01) |
| BUG-096 zero-spawn branch (S19-03) | 4th | Not started | 0.1d (S20-03) |
| BUG-097 (S19-04) | 4th | Not started | 0.1d (S20-05) |
| Editor / data block (S19-05) | 4th | Not started | 0.15d (S20-06) |
| BUG-092 + BUG-066/070 verify (S19-06) | 6th / 11th / 9th | Not started | 0.15d (Should, S20-09) |
| BUG-065 + BUG-086 (S19-07) | 11th / 6th | Not started | 0.2d (Should, S20-08) |
| Process gate decision (S19-08) | **10th** (TD-048 30th+) | Not started; 5 build breaks in 3 weeks | 0.1d (S20-07) |
| S19-09 BUG-100, S19-10 BUG-071 | 3rd / 7th | `VitalStatsBase` being rewritten on a feature branch | Deferred (S20-D1) |
| S19-11…S19-13 | 2nd–7th | Not started | Should (S20-11, S20-12) |
| S19-14 BUG-087 design note | 5th | Not started | Deferred (S20-D3) |
| S19-F1 class abilities | 2nd | Gate never opened; assets landed off-gate | Folded into S20-F1 |

---

## Tasks

### Must Have (Critical Path)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S20-01 | **Weekly playtest / Play Mode smoke — first task Monday (21st carry).** Run `tests/smoke/critical-paths.md` through the menu (`MainGamePlay` → New Game → `LoadRandomMap` + `GameplayUI`); fill `playtest-2026-10-24.md` (reuse it) incl. section E | owner (Kay) | 0.3 | None | Sheet filled with PASS/FAIL/SKIP per row, **or** a signed accepted-risk note in this file. No feature commit before this |
| S20-02 | **BUG-108 (S1)** — restore `[FormerlySerializedAs]` on `CharacterData` (`statsSO`, `AbilityBindings` backing field, weapon field) so the five `Enemy - Knight*` assets load their stats | owner + gameplay-programmer | 0.1 | None | Knight enemies have non-null `Stats`; no `Instantiate(null)` in Play Mode |
| S20-03 | **BUG-096 (S1)** — zero-spawn branch in `RoomGeneraterController.LoadRoom()` so Rest / Shop / Buff / Boss rooms open | owner + gameplay-programmer | 0.1 | None | Every non-combat room opens on load |
| S20-04 | **Weapon trio, one session** — BUG-106 (weapon + abilities only set by `SetCharacterData()`), BUG-107 item 2 (guard `Data.Weapon`, e.g. `Mage.asset`), **BUG-110** (drop / floor pickup never updates or injects `WeaponHolderBase.weapon`) | gameplay-programmer | 0.3 | S20-02 (same data class) | Enemies attack; Mage confirm does not throw; picked-up weapon is the one that attacks |
| S20-05 | **BUG-097 (S2)** — start / boss order (reorder `Maze_Storage.asset` or select by `RoomType`; closes Bug #16) | owner (Kay) | 0.1 | None | Run starts in `StartRoom_Entrance`, ends in `BossRoom_ThroneArena` |
| S20-06 | **Editor / data block** — BUG-105 `[FormerlySerializedAs("attackDamege")]` on `AttackSO.attackDamage`; BUG-095 `Collider2D` on `Arrow.prefab`; confirm BUG-072 `layerMask` on `Lightning.prefab` / its successor on `sprint-20` | owner (Kay) | 0.15 | None | Visible in `git diff`; sword hit deals 55 + PhysicalDamage |
| S20-07 | **Process gate decision (10th carry)** — merge to `sprint-NN` only after one Editor compile + menu → dungeon load; TD-048 pre-push hook, or an explicit signed drop | producer / owner | 0.1 | None | Decision recorded in this file |
| — | Saturday 2026-10-31 weekly playtest | owner (Kay) | 0.1 | — | Sheet filled before `/weekly-wrapup` |

Must-Have total ≈ **1.25d** → plan **1.3d**.

### Should Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S20-08 | BUG-065 + BUG-086 — stop movement on death; emit `ON_PLAYER_DEATH` once; keep `RealPlayerDataProvider.Respawn()` working | gameplay-programmer | 0.2 | S20-01 | No slide; event once; Respawn works |
| S20-09 | BUG-092 + BUG-066/070 verification — DoT ticks N times; audit `Reborn()` seeding vs the max-value fallback. Re-read against `feature/add-effect-component` first | gameplay-programmer | 0.15 | S20-01 | Recorded in the bug files |
| S20-10 | **BUG-115 (S2)** — stop the player mutating the committed `BaseStatsSO` (runtime copy); revert the leaked modifiers in the four player stat assets | gameplay-programmer + owner | 0.3 | None | Play Mode leaves `Assets/SO/Stat/*.asset` clean in `git status` |
| S20-11 | One-liners — BUG-104 (regressed log lines `PlayerState.cs:34,42`), BUG-103 (`RecoveryTime` gate), BUG-102 (`?.Invoke()`, `SetConfirm()` labels), BUG-068 `:28` | gameplay / ui-programmer | 0.15 | None | Each visible in `git diff` |
| S20-12 | Missing-script assets — BUG-073 + BUG-090 + **BUG-112** (`Merchant.prefab`, `NPC.prefab`) | owner (Editor) | 0.2 | None | No Missing Script warnings |

Should-Have total ≈ **1.0d**. Must + Should ≈ 2.3d of 4.0d.

### Feature story (owner, capped)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S20-F1 | Land `feature/add-effect-component` (`786c031b`) onto `sprint-20` with one Editor compile; Archer / Mage abilities from `05dcf630` covered by the Saturday playtest | owner (Kay) | **0.5 cap** | **S20-01** | Merged, compiling (`compile-check.sh` or `Editor.log` 0 `error CS`) |

### Nice to Have / Deferred

| ID | Task | Agent/Owner | Est. Days | Note |
|----|------|-------------|-----------|------|
| S20-D1 | BUG-100 + BUG-071 residual | gameplay-programmer | 0.35 | Wait for S20-F1 (`VitalStatsBase` rewrite) |
| S20-D2 | BUG-113 + BUG-114 (stat-reduction epic stories 002 / 003) | gameplay-programmer | 0.5 | Wait for S20-F1 |
| S20-D3 | BUG-087 design note — `GameManager` + player `Reborn()` | game-designer / producer | 0.3 | 5th carry, demoted |
| S20-D4 | BUG-109 Rain Arrows `_context` / `Destroy` on pooled object | gameplay-programmer | 0.15 | Archer feature, after S20-F1 |
| S20-D5 | BUG-111 `RegenComponent` defects | gameplay-programmer | 0.15 | Dormant (no caller) |
| S20-D6 | Git LFS for `.fbx`, TD-040 ADR, BUG-084 decision, BUG-083 | technical-director | 0.9 | Carried from S19-N1…N4 |
| S20-D7 | Weapon architecture review (`b364c303`) | lead-programmer | 0.2 | Carried from S19-N5 |

---

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| Playtest slips a 21st time | High | High | S20-01 first Monday; second skipped Saturday = owner decision (play or sign accepted risk), not a silent carry |
| Off-plan feature work outruns Must (10th time) | High | Medium | S20-F1 capped, gated on S20-01 |
| `feature/add-effect-component` conflicts with S20-04 / S20-09 / S20-10 | Medium | Medium | Stat / vitals bugs deferred until it lands; S20-04 touches `WeaponHolderBase` only |
| Another committed build break (5 in 3 weeks) | Medium | High | S20-07 |
| BUG-115 keeps corrupting committed stat assets during dev | High | Medium | Check `git status` for dirty `Assets/SO/Stat/*.asset` after every Play Mode session until S20-10 lands |
| No `gh` CLI | Known | Low | Manual PR command above |

## Dependencies on External Factors

- Owner Unity Editor time (S20-01, S20-05, S20-06, S20-12) — nothing can be verified without it.
- `gh` CLI install for automated PRs.

---

## Definition of Done for this Sprint

- [ ] S20-01 playtest sheet filled (or accepted-risk signed) — Monday
- [ ] S20-02, S20-03 on `sprint-20` — no S1 open on the dungeon loop
- [ ] S20-04 enemies attack; weapon pickup works
- [ ] S20-05, S20-06 committed
- [ ] S20-07 decision recorded
- [ ] Saturday 2026-10-31 playtest sheet filled

## QA Plan

> ⚠️ **No QA Plan** (`production/qa/qa-plan-sprint-20.md` not found). Autonomous run: option [B]
> recorded by default. S20-01 and the Saturday playtest are the de facto QA gate.

## Notes

- Open bug-inbox note NOTE-20261019-2 (Needs owner, `BaseStatsSO.cs:243` NRE) is **not** a task.
- `production/sprint-status.yaml` not updated (`.md`-only run).
