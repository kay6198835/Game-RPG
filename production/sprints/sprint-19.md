# Sprint 19 — 2026-10-19 to 2026-10-23

**Status: OPEN.** Opened 2026-10-19 (`pm-weekly-kickoff`, autonomous run — no owner present; the run
fired Monday 2026-10-19 instead of Sunday 22:00, so the sprint starts today).
Branch `sprint-19`, created on the remote from the `sprint-18` tip (`ea11e017`, "chore(standup): daily
standup 2026-10-16"). `gh` CLI unavailable (`gh: command not found`) — draft PR **not** auto-created;
run manually:

```bash
gh pr create --draft --base sprint-18 --head sprint-19 --title "Sprint 19"
```

**Review mode:** lean — producer feasibility gate skipped.
**Tracker:** `sprint-19-daily-plan.md`.
**Inputs:** `sprint-18.md` (Closure), `sprint-18-daily-plan.md`, `production/qa/bug-inbox.md`,
`production/qa/playtests/playtest-2026-10-17.md` (empty), `git log --all` since 2026-10-12.
No milestone file and no risk register exist — risks are tracked here.

> ⚠️ The Saturday 2026-10-17 `/weekly-wrapup` and `/doc-sync` did not run (no run-log row, no commit).
> There was no inbox triage this week, so **no new bug is confirmed**; the four open notes stay notes.

---

## Sprint Goal

**Make `sprint-19` compile and get one played session on record.** Playtest first on Monday (16 weeks
without runtime evidence), commit the owner's working-tree build fix, then the dungeon-loop blockers
(BUG-096, BUG-097) and the Inspector / data block.

---

## Capacity

- Total days: 5
- Buffer (20%): 1 day
- Available: 4 days

Planned-work velocity: Sprint 16 ≈0.01, Sprint 17 0.05, Sprint 18 **0.0**. Off-plan owner output
≈2.5–3d/week. Planned load is kept at ≈1.4d Must and ≈1.0d Should; owner feature work stays an explicit
story capped at 0.5d, gated behind S19-01 **and** S19-02.

0.1d is reserved on **Saturday 2026-10-24** for the weekly playtest (`tests/smoke/critical-paths.md`
+ the `playtest-sheet.sh` sheet).

---

## Carryover from Previous Sprint

| Task | Times carried | Reason | New Estimate |
|------|---------------|--------|--------------|
| Playtest / Play Mode smoke (S18-01) | **16th** | Never run ahead of feature work; tip broken all week | 0.3d — Mon, first |
| Build-fix commit (NOTE-20261013-1, tracker task) | 2nd | Fix exists only in owner's working tree | 0.05d |
| BUG-096 zero-spawn branch (S18-02) | 3rd | Not started | 0.1d |
| BUG-097 (S18-03) | 3rd | Not started | 0.05–0.2d |
| Editor block (S18-04) | 3rd (BUG-072 6th) | BUG-072 done in unpushed `cae75317` only | 0.1d |
| BUG-092 residual (S18-05) | 5th | In code on `sprint-18`, unverified | 0.05d (verify only) |
| BUG-066 + BUG-070 (S18-06) | 10th / 8th | Getter fallback in; seeding audit open | 0.1d |
| BUG-065 + BUG-086 (S18-07) | 10th / 5th | Not started | 0.2d |
| Process gate decision (S18-08) | **8th** (TD-048 30th+) | Not started; 4th build break this sprint | 0.1d |
| S18-09…S18-14 (Should) | 2nd–6th | Not started | see Should Have |

---

## Tasks

### Must Have (Critical Path)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S19-01 | **Weekly playtest / Play Mode smoke — first task Monday (16th carry).** Run `tests/smoke/critical-paths.md` through the menu (`MainGamePlay` → New Game → `LoadRandomMap` + `GameplayUI`) and fill a fresh sheet (`playtest-2026-10-17.md` may be reused). The owner's working tree compiles, so it may be played before S19-02 — note the commit/working-tree state on the sheet | owner (Kay) | 0.3 | None | Sheet filled with PASS/FAIL/SKIP per row, or a signed accepted-risk note here. **No feature commit before this** |
| S19-02 | **Commit + push the build fix** (NOTE-20261013-1 working-tree edits to `AbilityHolderBase.cs`, `WeaponHolderBase.cs`, `IWeaponHolder.cs` and friends) and bring it onto `sprint-19` | owner (Kay) | 0.05 | None | `sprint-19` tip compiles (`compile-check.sh` PASS or `Editor.log` 0 `error CS` on a clean checkout) |
| S19-03 | **BUG-096 (S1)** — zero-spawn branch in `RoomGeneraterController.LoadRoom()` so Rest / Shop / Buff / Boss rooms open | owner + gameplay-programmer | 0.1 | S19-02 | Every non-combat room opens on load; on `sprint-19` |
| S19-04 | **BUG-097 (S2)** — start/boss order (reorder `Maze_Storage.asset` or select by `RoomType`; closes Bug #16) | owner (Kay) | 0.05–0.2 | None | Run starts in `StartRoom_Entrance`, ends in `BossRoom_ThroneArena` |
| S19-05 | **Editor / data block** — push `cae75317` (BUG-072 `layerMask` already set) onto `sprint-19`; BUG-095 `Collider2D` on `Arrow.prefab`; **BUG-105 (S2)** `[FormerlySerializedAs("attackDamege")]` on `AttackSO.attackDamage` (+ re-save `SnS_State1-3.asset`) | owner (Kay) | 0.15 | S19-02 | All three visible in `git diff` on `sprint-19`; sword deals 55 + PhysicalDamage |
| S19-06 | **BUG-092 + BUG-066/070 verification** — confirm a DoT ticks N times; audit `Reborn()` seeding against the new max-value fallback in `VitalStatsBase.GetCurrentStatValue()` | gameplay-programmer | 0.15 | S19-02 | DoT ticks N times in Play Mode; seeding audit recorded; no `KeyNotFoundException` |
| S19-07 | **BUG-065 + BUG-086** — stop movement on death; emit `ON_PLAYER_DEATH` once; keep `RealPlayerDataProvider.Respawn()` working | gameplay-programmer | 0.2 | S19-02 | No slide; event once; Respawn still works |
| S19-08 | **Process gate decision (8th carry)** — merge to `sprint-NN` only after one Editor compile + menu → dungeon load; TD-048 pre-push hook or an explicit, signed drop | producer / owner | 0.1 | None | Decision recorded in this file |
| — | Saturday 2026-10-24 weekly playtest | owner (Kay) | 0.1 | — | Sheet filled before `/weekly-wrapup` |

Must-Have total ≈ **1.2–1.35d** → plan **1.4d**.

### Should Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S19-09 | **BUG-100 (S2)** — timed-buff expiry strips every modifier (shared source `this` in `VitalStatsBase`) | gameplay-programmer | 0.2 | S19-06 (same file) | Item buff survives a timed-buff expiry |
| S19-10 | BUG-071 residual — coroutine handle; stop HoT/DoT on death / disable | gameplay-programmer | 0.15 | S19-06 | DoT does not survive death or respawn |
| S19-11 | BUG-068 `:28` + BUG-102 (confirm-panel `?.Invoke()`, `SetConfirm()` labels) | gameplay / ui-programmer | 0.1 | S19-02 | No NRE on pooled enemy; no NRE without `GameplayUI` |
| S19-12 | BUG-103 (ranged `RecoveryTime` gate) + BUG-104 (guard the `PlayerState.Enter()` log) | gameplay-programmer | 0.1 | S19-02 | Ranged fire respects `RecoveryTime`; no per-state log in builds |
| S19-13 | BUG-073 + BUG-090 — remove / repoint missing-script assets | owner (Editor) | 0.15 | None | No missing-script warnings |
| S19-14 | BUG-087 design note — `GameManager` + player `Reborn()`, starting from `RealPlayerDataProvider.Respawn()` | game-designer / producer | 0.3 | S19-07 | Short design note / story |

Should-Have total ≈ **1.0d**. Must + Should ≈ 2.4d of 4.0d.

### Feature story (owner, capped)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S19-F1 | Class abilities / Champion equip — bring Rain Arrows (`5c8245c9`), assets (`cae75317`) and the Champion equip flow onto `sprint-19` | owner (Kay) | **0.5 cap** | **S19-01, S19-02** | Merged to `sprint-19` and compiling; covered by the Saturday playtest |

### Nice to Have / Deferred

| ID | Task | Agent/Owner | Est. Days | Note |
|----|------|-------------|-----------|------|
| S19-N1 | Git LFS decision for `.fbx` | owner / technical-director | 0.1 | 3rd carry, demoted |
| S19-N2 | TD-040 ADR — v1 vs v2 abilities | technical-director | 0.3 | — |
| S19-N3 | BUG-084 decision — test location + `.asmdef` | technical-director | 0.3 | — |
| S19-N4 | BUG-083 `HoldTime` / `HoldRatio` | gameplay-programmer | 0.2 | — |
| S19-N5 | Weapon architecture review (`b364c303`, feature branch) | lead-programmer | 0.2 | Owner asked "need review" in the commit message |
| S19-N6 | BUG-098 / BUG-099 / BUG-101 doc drift | PM (`/doc-sync`) | 0.2 | Saturday doc-sync missed |

---

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| Playtest slips a 16th time | High | High | S19-01 first on Monday; may run on the compiling working tree |
| `sprint-19` tip stays uncompilable | High | High | S19-02 is 0.05d and gates every other code task |
| Off-plan feature work again outruns Must (9th time) | High | Medium | S19-F1 capped, gated on S19-01 + S19-02 |
| Large unpushed / unmerged owner work (`cae75317`, `5c8245c9`, `b364c303`, working tree) | High | Medium | S19-02 / S19-05 / S19-F1 bring it onto the sprint branch in order |
| Another committed build break | Medium | High | S19-08 |
| Weekend routines (wrap-up, doc-sync) not firing | Known | Medium | Check `pm-weekly-wrapup` / `pm-weekly-doc-truth` schedules; this week's inbox is untriaged |
| No `gh` CLI | Known | Low | Manual PR command above |

## Dependencies on External Factors

- Owner Unity Editor time (S19-01, S19-02, S19-05, S19-13) — nothing can be verified without it.
- `gh` CLI install for automated PRs.

---

## Definition of Done for this Sprint

- [ ] S19-01 playtest sheet filled (or accepted-risk signed) — Monday
- [ ] S19-02 `sprint-19` tip compiles
- [ ] S19-03, S19-04 on `sprint-19` (dungeon loop unblocked)
- [ ] S19-05 Inspector / data steps committed
- [ ] S19-06, S19-07 committed and checked in Play Mode
- [ ] S19-08 decision recorded
- [ ] Saturday 2026-10-24 playtest sheet filled
- [ ] No S1 bugs open on the dungeon loop

## QA Plan

> ⚠️ **No QA Plan** (`production/qa/qa-plan-sprint-19.md` not found). Autonomous run: option [B]
> recorded by default. S19-01 and the Saturday playtest are the de facto QA gate. Run `/qa-plan sprint`
> in an owner session.

## Notes

- Open bug-inbox notes (NOTE-20261013-1/2/3, NOTE-20261015-1) are **not** tasks; they wait for the
  Saturday 2026-10-24 triage. S19-02 is the tracker's carried build-fix task, not a bug task.
- `production/sprint-status.yaml` not updated (`.md`-only run).
