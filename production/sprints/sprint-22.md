# Sprint 22 — 2026-11-09 to 2026-11-13

**Status: OPEN.** Opened 2026-11-08 (`pm-weekly-kickoff`, autonomous run — no owner present).
Branch `sprint-22`, created on the remote from the `sprint-21` tip (`d2d2b22e`, "Merge
pm/doc-sync-2026-11-07 into sprint-21"). `gh` CLI unavailable (`gh: program not found`) — draft PR
**not** auto-created; run manually:

```bash
gh pr create --draft --base sprint-21 --head sprint-22 --title "Sprint 22"
```

**Review mode:** lean — producer feasibility gate skipped.
**Tracker:** `sprint-22-daily-plan.md`.
**Inputs:** `sprint-21.md`, `sprint-21-daily-plan.md` (wrap-up handoff 2026-11-07),
`production/qa/bug-triage-2026-11-07.md`, `production/qa/bug-inbox.md`, `git log --all` since
2026-11-02. No milestone file and no risk register exist — risks are tracked here.

---

## Sprint 21 Closure (recorded by this kickoff)

- **Verdict:** SLIPPED (2026-11-07 wrap-up, evidence RUNTIME (automated QA only) + LOG + STATIC).
- **Done (planned):** S21-02 BUG-116, S21-03 BUG-121, S21-09 process gate (C + B pre-push hook) = 0.3d.
- **Partial:** S21-05 (Shop / Rest / Buff open; Boss not), S21-06 (BUG-107 guard; enemy half of
  BUG-106), S21-08 (BUG-105 data keys), S21-12 (runtime stat clone).
- **Velocity:** 0.3d / 4d = **0.08**. Sprint 16 ≈0.01, 17 0.05, 18 0.0, 19 0.01, 20 0.13, 21 0.08.
- **Off-plan owner output:** ≈1.7d (enemy roles + rooms + Abilities v1 deletion `6094aa10`, death
  input flag `4097be4c` / `1c3a34fc`, data-key fix `fbab752a`, collider layer `6f5322f6`, `Maw` art).
- **Playtest done: no** — `playtest-2026-11-07.md` blank. S21-01 at its 24th carry.
- **New confirmed bugs (2026-11-07 triage):** BUG-122 (S1, P1), BUG-123 (S3, P3), BUG-124 (S3, P2).
  Seven bugs now "fixed in code, awaiting runtime" (BUG-105, 107, 116, 121; partial 106, 108, 115).
- **After the wrap-up (weekend, not on `sprint-21`):** owner commits on
  `origin/feature/polish-game` — `38d40925` "paladin combo attackcc" (13 files: `WeaponHolderBase`,
  `Weapon`, `PlayerAttackState`, `EntityFindTarget`, `EntityMovement`, `EntityData`, entity states,
  `AttackFlowDebugger` / new `AttackFlowEventProbe`) and `0f3644bd` "animation and add weapon attack
  mage". Not merged, not compiled by a routine. Recorded as S22-F1.

---

## Sprint Goal

**One recorded playtest, a player death that reaches Game Over, and a room path that starts in the
start room and can open the boss room.** Playtest first on Monday (25th carry), then BUG-122, then
the two room P1s (BUG-096, BUG-097).

---

## Capacity

- Total days: 5
- Buffer (20%): 1 day
- Available: 4 days

Planned-work velocity has been ≤0.13 for six sprints. Planned load is ≈1.1d Must and ≈0.8d Should;
owner feature work is an explicit story capped at 0.5d, gated behind S22-01.

0.1d is reserved on **Saturday 2026-11-14** for the weekly playtest (`tests/smoke/critical-paths.md`
+ the `playtest-sheet.sh` sheet). The Friday 23:00 QA automation run is an input to it, not a
substitute.

---

## Carryover from Previous Sprint

| Task | Times carried | Reason | New Estimate |
|------|---------------|--------|--------------|
| Playtest / Play Mode smoke (S21-01) | **25th** | Owner plays (21 Play Mode entries Sat), never records | 0.3d — Mon, first (S22-01) |
| BUG-096 Boss room (S21-05 rest) | 6th | Shop / Rest / Buff done; Boss not | 0.05d (S22-03) |
| BUG-097 (S21-07) | 6th | Not started | 0.1d (S22-04) |
| Weapon remainder BUG-106 player half / BUG-110 (S21-06) | 3rd | Enemy half done | 0.2d (S22-06) |
| Editor / data block remainder (S21-08) + BUG-108 dead key (S21-04) | 6th / 3rd | Data keys done; collider, `layerMask`, re-save open | 0.15d (S22-07) |
| BUG-117 + BUG-120 (S21-10) | 2nd | Not started; + BUG-123 | 0.35d (Should, S22-10) |
| BUG-065 + BUG-086 (S21-11) | **13th** / 8th | Not started; reachable once BUG-122 is fixed | 0.2d (Should, S22-09) |
| BUG-115 asset cleanup (S21-12 rest) | 3rd | Clone done; leaked modifiers in Archer / Mage assets | 0.1d (Should, S22-11) |
| One-liners (S21-13) | 4th–7th | Not started | 0.15d (Should, S22-12) |
| Effect-source work (S21-F1) | 2nd | Not touched since 2026-11-03 | Deferred (S22-D1) |
| S21-D1…D7 | — | Unchanged | Kept deferred (S22-D2…D7) |

---

## Tasks

### Must Have (Critical Path)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S22-01 | **Weekly playtest / Play Mode smoke — first task Monday (25th carry).** Run `tests/smoke/critical-paths.md` through the menu; fill a sheet in `production/qa/playtests/` incl. a row for each "fixed in code, awaiting runtime" bug (BUG-105, 107, 116 build, 121; 106, 108, 115) | owner (Kay) | 0.3 | None | Sheet filled with PASS/FAIL/SKIP per row, **or** a signed accepted-risk note in this file. No feature commit before this |
| S22-02 | **BUG-122 (S1, P1)** — player death reaches Game Over: `PlayerDeathState` must not wait for an `EndRangeTrigger` no Paladin Die clip sends (clip event or state change) | gameplay-programmer | 0.1 | None | `ON_PLAYER_DEATH` fires; Game Over panel shows; QA-VIT-07 PASS |
| S22-03 | **BUG-096 (S1, P1) remainder** — Boss room opens / is entered correctly on load | gameplay-programmer | 0.05 | None | Boss room not sealed on load |
| S22-04 | **BUG-097 (S2, P1)** — start / boss chosen by `RoomType`, not list position (closes Bug #16) | gameplay-programmer | 0.1 | None | Run starts in `StartRoom_Entrance`, ends in `BossRoom_ThroneArena` |
| S22-05 | **BUG-124 (S3, P2)** — pooled re-spawn: move before activate (or re-anchor after move) | gameplay-programmer | 0.05 | None | Re-spawned enemy wanders around its new spawn point |
| S22-06 | **Weapon remainder** — BUG-106 player half (armed without the Champion), BUG-110 drop / pickup bookkeeping | gameplay-programmer | 0.2 | None | Picked-up weapon is the one that attacks; player not unarmed on direct scene load |
| S22-07 | **Editor / data block** — BUG-095 `Collider2D` on `Arrow.prefab`; BUG-072 `layerMask` on `Lightning.prefab`; re-save the eight `fbab752a` assets; BUG-108 dead `weaponSO:` key on legacy Knight assets | owner (Kay) | 0.15 | None | Visible in `git diff`; arrows hit; Consecrate damages |
| — | Saturday 2026-11-14 weekly playtest | owner (Kay) | 0.1 | — | Sheet filled before `/weekly-wrapup` |

Must-Have total = **1.05d**.

### Should Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S22-09 | BUG-065 + BUG-086 — stop movement on death; emit `ON_PLAYER_DEATH` once. **Do in the same session as S22-02** — fixing BUG-122 makes BUG-086 fire every frame | gameplay-programmer | 0.2 | S22-02 | No slide; event once; `RealPlayerDataProvider.Respawn()` works |
| S22-10 | BUG-117 + BUG-120 + BUG-123 — vital modifiers serializable with the right sign; item stat effects apply; coin effect authorable | gameplay-programmer | 0.35 | None | Heal / damage effect changes HP correctly; a stat item changes the stat; a coin item adds coins |
| S22-11 | BUG-115 remainder — revert leaked modifiers in Archer / Mage stat assets (QA-STATIC-04) | owner (Kay) | 0.1 | None | QA-STATIC-04 PASS; `git status` clean after Play Mode |
| S22-12 | One-liners — BUG-104, BUG-103, BUG-102, BUG-068 `:28` | gameplay / ui-programmer | 0.15 | None | Each visible in `git diff` |

Should-Have total = **0.8d**. Must + Should = 1.85d of 4.0d.

### Feature story (owner, capped)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S22-F1 | Merge `feature/polish-game` (`38d40925`, `0f3644bd`: Paladin combo, Mage weapon attack, enemy AI move / find-target) into `sprint-22` with one Editor compile | owner (Kay) | **0.5 cap** | **S22-01** | Merged, compiling (`compile-check.sh` or `Editor.log` 0 `error CS`); `WeaponHolderBase` changes do not reopen BUG-107 / BUG-110 |

### Nice to Have / Deferred

| ID | Task | Agent/Owner | Est. Days | Note |
|----|------|-------------|-----------|------|
| S22-D1 | Effect-source work (`SourceEffect` / `VitalModifier`, plan `ef0d9c6f`) | owner (Kay) | ≤0.5 | Was S21-F1; not touched for 2 weeks |
| S22-D2 | BUG-092 + BUG-066/070 verify, BUG-118, BUG-119, BUG-113, BUG-114, BUG-100, BUG-071 | gameplay-programmer | 0.9 | Effect / vitals code |
| S22-D3 | Missing-script assets BUG-073 / 090 / 112 | owner (Editor) | 0.2 | 3rd carry |
| S22-D4 | BUG-087 design note — `GameManager` + player `Reborn()` | game-designer / producer | 0.3 | 7th carry; becomes urgent once S22-02 lands |
| S22-D5 | BUG-109 Rain Arrows / BUG-111 `RegenComponent` | gameplay-programmer | 0.3 | — |
| S22-D6 | Git LFS for `.fbx`, TD-040 ADR, BUG-084 decision, BUG-083 | technical-director | 0.9 | Carried since S19 |
| S22-D7 | Weapon architecture review (`b364c303`) | lead-programmer | 0.2 | Carried since S19 |

---

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| Playtest slips a 25th time | High | High | S22-01 first Monday. Seven bugs cannot leave "awaiting runtime" without it. Owner decision: play and record, or sign accepted risk |
| Off-plan feature work outruns Must (12th time) | High | Medium | S22-F1 capped, gated on S22-01; the polish branch already exists |
| Fixing BUG-122 exposes BUG-086 (event every frame) | High | Medium | Do S22-09 in the same session |
| `feature/polish-game` touches `WeaponHolderBase` / `Weapon` while S22-06 changes them | Medium | Medium | Merge S22-F1 before starting S22-06, or do S22-06 on the polish branch |
| Pre-push gate checks `main` only | Known | Medium | Accepted 2026-11-03; consider `git config gate.branches "main sprint-*"` |
| No `gh` CLI | Known | Low | Manual PR command above |

## Dependencies on External Factors

- Owner Unity Editor time (S22-01, S22-07, S22-11) — nothing can be verified without it.
- `gh` CLI install for automated PRs.

---

## Definition of Done for this Sprint

- [ ] S22-01 playtest sheet filled (or accepted-risk signed) — Monday
- [ ] S22-02 committed — QA-VIT-07 PASS
- [ ] S22-03, S22-04 on `sprint-22` — run starts in the start room; boss room reachable
- [ ] S22-05, S22-06, S22-07 committed
- [ ] Saturday 2026-11-14 playtest sheet filled

## QA Plan

> ⚠️ **No QA Plan** (`production/qa/qa-plan-sprint-22.md` not found). Autonomous run: option [B]
> recorded by default. S22-01, the Friday QA automation run and the Saturday playtest are the de
> facto QA gate.

## Notes

- Open / needs-owner inbox notes (NOTE-20261104-3/-4, NOTE-20261107-2/-3/-4) are **not** tasks.
- `production/sprint-status.yaml` not updated (`.md`-only run).
