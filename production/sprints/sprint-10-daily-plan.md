# Sprint 10 — Daily Plan & Progress Tracker

> **Sprint**: 2026-08-17 (Mon) → 2026-08-21 (Fri)
> **Companion to**: `sprint-10.md` (formal plan) — this file is the day-by-day breakdown + live tracker
> **Maintained by**: PM assistant — updated each session/standup
> **Routines (Claude Code)**:
>   - **Mon–Fri 10:00** → `/daily-standup`
>   - **Sat 22:00** → `/weekly-wrapup`
>   - **Sun 22:00** → `/weekly-kickoff`
> **Opened**: 2026-08-16 (Sunday 22:00 kickoff, on-slot) — autonomous scheduled run, no owner present.
> Branch `sprint-10` created from `sprint-09` tip (`7eb3378`), after `git fetch origin sprint-09`
> confirmed the local ref was current (process fix from `retro-sprint-09-2026-08-15.md`). Sprint 9
> closed CONCERNS with 2/6 Must-Have items code-complete, 0/6 Play-Mode-confirmed — see `sprint-09.md`
> closure section for full detail. Before this kickoff ran, uncommitted WIP found on the unrelated
> `origin/feature/fix-player-control` branch was preserved via `git stash push -u` (not applied to
> `sprint-10`) — see `sprint-10.md`'s "New finding" section and **S10-11** below.

---

## Status Verdict: 🟡 OPEN (2026-08-17, Mon standup) — S10-01 (the sprint's literal Day-1 task, BUG-042/053/054) still **zero movement** — now a 5th consecutive cycle. Unplanned StatSystem/UI/combo work landed instead (see standup log below), ahead of S10-02's process gate, which is exactly the sequencing risk the sprint's own Risks table called out. S10-02, S10-04, S10-05 all still open too. Owner-in-Editor session still needed for S10-03 (Play Mode verify), unreached for 3 consecutive sprints (S7-08, S8-12, S9-12).

---

## Day-by-Day Plan

### Mon 2026-08-17 — Enemy combat chain (the sprint's one true blocker)

| Task | Est. | Status (as of Mon standup) | Notes |
|------|------|------|-------|
| S10-01 (BUG-042 + BUG-053 + BUG-054, `EntityCore.TakeDamage()` chain) | 0.3d | ⚠️ OPEN — untouched | P0 — literal first task, per retro Action Item #1. Implement for real; delete `EntityNegativeReciver.cs`, don't patch it. **5th consecutive cycle at zero movement — still today's #1 priority.** |
| S10-04 (BUG-033, one-line fix) | 0.1d | ⚠️ OPEN — untouched | Trivial, 9th carry — zero excuse remains |
| S10-05 (BUG-044, PlayerDeathState orphaned) | 0.15d | ⚠️ OPEN — untouched, but unblocked | 6th carry. Note: this morning's merge wired `StatusAnimation.Start`/`.End` anim events for the first time (`Player.AnimationStart()`/`AnimationEnd()`) — the exact triggers this fix's commented-out body checks. Worth confirming the anim-event plumbing before assuming this is still a from-scratch task. |
| S10-02 (S9-00 process gate, enforced version) | 0.15d | ⚠️ OPEN — untouched | 6th carry — land as a real pre-push hook this time, not another written-policy draft. **Now higher-priority than originally scoped**: today's unplanned merge (see standup log) landed 43 files onto `sprint-10` before this gate existed, which is the exact scenario S10-02 exists to catch. |

Goal (carried unchanged from kickoff, still valid): land the sprint's largest and longest-stalled item
today, before anything else competes for branch time. Today's unplanned StatSystem/UI merge (see
Standup Log) did **not** touch any of these four — none of Monday's Must-Have scope moved.

### Tue 2026-08-18 — Verification gate + forced decision

| Task | Est. | Notes |
|------|------|-------|
| S10-03 (Play Mode verify, both attack directions + statusAnimation buffer-gate) | 0.2d | **Gate** — owner confirms in-Editor; do not treat Mon's S10-01 as done without this. 4th attempt after S7-08/S8-12/S9-12 all went unreached. |
| S10-06 (S4-05/S4-06 forced decision) | 0.1d | 11th carry — make the call, do not carry a 12th time |
| S10-12 (BUG-046, `OverlapCircle`→`OverlapCircleNonAlloc`) | 0.15d | Should-Have, quick, independent |

Goal: S10-03 actually confirmed this time — 3 consecutive prior sprints closed without any Play Mode
confirmation of anything.

### Wed 2026-08-19 — Should-Have: Bug #6 + BUG-043

| Task | Est. | Notes |
|------|------|-------|
| S10-08 (Bug #6 / S7-11, player HP write-through) | 0.4d | 11th carry — do not close without the EditMode test `TakeDamage_BelowZero_TriggersDeathState` |
| S10-07 (BUG-043 consolidation) | 0.3d | Depends on S10-01 landing first |

Goal: single HP source of truth confirmed via passing EditMode test; enemy attack path consolidated.

### Thu 2026-08-20 — WIP reconciliation + process cleanup

| Task | Est. | Notes |
|------|------|-------|
| S10-11 (reconcile `origin/feature/fix-player-control` stashed WIP) | 0.3d | Gated on S10-02 (process gate) landing first — reconciliation itself must go through the new gate, not bypass it |
| S10-09 (ADR-0002 Accepted) | 0.1d | 8th carry, trivial |
| S10-10 (individual `BUG-NNN.md` files) | 0.2d | Should-Have |
| Buffer / catch-up | — | Reserved for Must-Have slippage if S10-01/S10-03 ran long |

### Fri 2026-08-21 — Nice-to-Have stretch + wrap prep

| Task | Est. | Notes |
|------|------|-------|
| S10-N1 (Bug #14, missing `return`) | 0.1d | If Must-Have closed clean |
| S10-N2 (Bug #15, build-safe JSON load) | 0.5d | If time remains |
| S10-N3 (first playtest) | — | Only if S10-01/S10-03 confirmed stable — 11th cycle attempt |
| S10-N4 (BUG-032 explicit re-verify) | 0.1d | If S10-03 only partially covered prefab wiring |
| Friday wrap-up prep | — | Feeds into Sat 22:00 `/weekly-wrapup` |

---

## Standup Log

### Mon 2026-08-17 — Daily Standup (autonomous, no owner present)

**Yesterday (Sunday 2026-08-16):** kickoff only, no dev work — sprint just opened.

**Early this morning (Mon 2026-08-17, ~09:25–09:26, ~2h before this standup):** Kay merged
`origin/feature/fix-player-control` into `sprint-10` (commit `d430899` "update logic data logic, UI
stats system" + merge `1f111fa`), then continued on top of it. 43 files, +3805/-313 — this is the
same WIP that Sunday's kickoff **stashed and explicitly deferred as S10-11**, gated behind S10-02
(process gate) landing first. **S10-02 has not landed** (no pre-push hook exists yet — only Git's
`pre-push.sample` is present) — so this reconciliation happened *before* its own gate, which is
exactly the "Medium/Medium" risk the sprint plan named for S10-11. Not a compile-check failure (no
CI here to confirm either way), just a sequencing note (rule tracking, không phải lỗi — the owner is
free to override the plan; flagging for visibility per the PM role (vai trò PM)).

Content of the merge (verified via `git diff`, not just the commit message):
- `Assets/Script/StatSystem/`: `Stat.cs`, `StatModifier.cs` reworked; new `StatModifierGroupSO.cs` /
  `StatModifierGroup.cs` (SO "Game/Stat Modifier Group") for authored equip/buff modifier bundles;
  `StatsSO.cs` gains batched `AddModifiersFromSource()`. `ADR-0001` and `CLAUDE.md` StatSystem section
  updated to match — docs kept in sync with code, good practice (thực hành tốt).
  - New `Assets/Script/UI/StatSlot.cs` + `StatsUI.cs`, new `StatsSystem` prefabs (Primary/Derived stat
  slots), new UI Toolkit panel settings, 2 new `EventID` entries
  (`ON_OPEN_STATS_PLAYER_UI`/`ON_CLOSE_STATS_PLAYER_UI`) — a new player-facing stats panel (bảng chỉ
  số người chơi).
- `Weapon.cs`: equip/unequip now calls `stats.modifiers.ApplyTo/RemoveFrom(Player.Stats, this)` — gear
  stat modifiers actually apply on equip now.
- `Player.cs` / `PlayerState.cs`: new `AnimationStart()`/`AnimationEnd()` anim-event handlers wired to
  `StatusAnimation.Start`/`.End` (previously only `StartRangeTrigger`/`OnActivate`/`OffActivate`/
  `EndRangeTrigger` existed). **Relevant to S10-05** — `PlayerDeathState.LogicUpdate()`'s commented-out
  body checks exactly `StatusAnimation.Start`/`.End`; the animation-event plumbing it needs may now
  actually exist. Worth a quick look before treating S10-05 as a from-scratch 0.15d task.
- 3 `Knight_ComboAttack` animation clips + `LoadRandomMap.unity` scene changes — combo-attack polish,
  same area Sprint 9's weapon-architecture refactor touched.

**Checked against the tracker — none of Monday's planned Must-Have items landed in this merge**
(verified by reading the current file contents, not just the diff):
- ❌ **S10-01** (BUG-042/053/054) — `EntityCore.TakeDamage()` still `throw new NotImplementedException()`
  verbatim; `EntityNegativeReciver.cs` (the duplicate receiver) still present, not deleted. **5th
  consecutive cycle at zero movement.**
- ❌ **S10-02** (process gate) — no pre-push hook in `.git/hooks/` (only the stock `.sample`).
- ❌ **S10-04** (BUG-033) — `EnemySpawner.cs:62` still reads `set.Count == 0 || set == null` (wrong
  order, NullReferenceException risk unfixed). 9th carry.
- ❌ **S10-05** (BUG-044) — `PlayerDeathState.LogicUpdate()` body still fully commented out.
- ADR-0002 — still `Status: Proposed`, not flipped to Accepted (S10-09, Should-Have).

**Housekeeping note:** `git stash list` shows 5 separate `WIP on origin/feature/fix-player-control: ...`
entries (`stash@{1}`–`stash@{4}`, `stash@{6}`) accumulated across recent sprint switches, not just the
one Sunday's kickoff created. None were touched by this morning's merge (it came from the branch
directly, not a stash pop) — recommend the owner review and clear the stale ones once confirmed
superseded, so a future `git stash list` scan doesn't have to re-triage all of them.

---

### Sun 2026-08-16 — Weekly Kickoff (autonomous, no owner present)

Sprint 9 closed CONCERNS (2/6 Must-Have code-complete: BUG-041, BUG-032; 0/6 Play-Mode-confirmed) —
already finalized in last night's Saturday `pm-weekly-wrapup` run, this kickoff only opened Sprint 10.
`git fetch origin sprint-09` confirmed the local ref matched before branching (process fix adopted from
the retro). Found and stashed (not lost, not applied) 13+ uncommitted files on the unrelated
`origin/feature/fix-player-control` branch, likely newer attack/combo-flow + StatSystem work beyond
what already landed via `sprint-09`'s weapon-architecture merge — flagged as **S10-11**, deliberately
gated behind **S10-02** so reconciliation doesn't bypass the process gate it depends on. `gh` CLI still
unavailable — draft PR not auto-created, manual command left in `sprint-10.md`. No QA plan exists for
the 10th consecutive cycle — flagged, deferred to owner per every prior cycle's handling.

---

## Carry-Over Watch List (re-verify every standup)

- **BUG-042/BUG-053/BUG-054 — P0/S1, combat non-functional enemy→player.** Zero code movement across
  all 5 of Sprint 9's scheduled days, **now 5th carry (confirmed still zero movement Mon standup)**. The
  sprint's literal first task (S10-01), still untouched. Prior retros' standing recommendation: a single
  dedicated session likely resolves this faster than continued distributed autonomous check-ins.
- **S10-02 process gate** — **now 6th carry**, same underlying pattern since Sprint 4. Retro Action
  Item #3 specifically asks for an *enforced* version this cycle, not another written note. Mon standup
  found the gate landed *after* the exact scenario it exists to catch (see S10-11 below) — raises this
  item's urgency, not just its carry count.
- **S10-11 (WIP from `origin/feature/fix-player-control`)** — ✅ merged onto `sprint-10` Mon 2026-08-17
  ~09:26 (commits `d430899`/`1f111fa`), by the owner directly, **ahead of S10-02** landing. Content
  verified (StatSystem rework, new stats UI panel, weapon-stat hookup on equip, new animation-event
  triggers, combo-attack anim polish) — see Mon standup log for full breakdown. Sequencing risk the
  sprint plan flagged for S10-11 has materialized; no evidence of harm found in this review, but S10-02
  landing is now overdue rather than merely carried.
- **S10-06 (S4-05/S4-06)** — 11th carry, zero movement any cycle. Decision-avoidance, not an estimation
  problem — recommend the owner just make the call.
- **S10-03 Play Mode verify gate** — unreached 3 consecutive sprints (S7-08, S8-12, S9-12). No Unity CLI
  in this automated environment; requires an owner-in-Editor session specifically. This morning's merge
  changes the player attack/combo/animation-event surface again — one more reason S10-03 needs a real
  in-Editor pass rather than being assumed stable.
- QA plan — 10 consecutive cycles with none. Flagged in `sprint-10.md`, deferred to owner.
