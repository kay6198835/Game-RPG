# Sprint 14 — Daily Plan & Progress Tracker

> **Sprint**: 2026-09-14 (Mon) → 2026-09-18 (Fri)
> **Companion to**: `sprint-14.md` (formal plan) — this file is the day-by-day breakdown + live tracker
> **Maintained by**: PM assistant — updated each session/standup
> **Routines (Claude Code)**:
>   - **Mon–Fri 10:00** → `/daily-standup`
>   - **Sat 22:00** → `/weekly-wrapup`
>   - **Sun 22:00** → `/weekly-kickoff`
> **Opened**: 2026-09-13 (Sunday 22:00 kickoff, on-slot) — autonomous scheduled run, no owner present.
> Branch `sprint-14` created from `sprint-13` tip (`c4be3c7`). Sprint 13 closed **FAIL** — 0/6 Must-Have
> met, 2/6 Should-Have landed (the two largest, not the smallest). 4 new S1/S2 bugs (BUG-067/068/069/070)
> shipped via unplanned off-plan work, caught only by the wrap-up's code review. See `sprint-13.md`
> closure block and `retro-sprint-13-2026-09-13.md` for full detail.

---

## Hard Gate (new this cycle — replaces the "schedule it first" note that failed 3 sprints running)

Per the Sprint 13 retro's explicit recommendation: this sprint is **not considered to have started
substantive work** until S14-01 through S14-06 exist as committed diffs. Every standup below checks
these six items, in this order, before reviewing anything else — including any unplanned work that
may have landed instead.

| # | Task | Est. |
|---|------|------|
| 1 | S14-01 (BUG-067 heal/damage swap) | 0.05d |
| 2 | S14-02 (BUG-068 unbound-slot NRE) | 0.05d |
| 3 | S14-03 (BUG-063 `[SerializeField]` removal) | 0.05d |
| 4 | S14-04 (BUG-064 item 7, `RangeWeapon.cs` DI wiring) | 0.1d |
| 5 | S14-05 (BUG-065, `PlayerDeathState` movement stop) | 0.1d |
| 6 | S14-06 (BUG-066 + BUG-070 dictionary guard) | 0.2d |

Combined: 0.55d — under one day's capacity.

---

## Day-by-Day Plan

### Mon 2026-09-14 — The six-item gate, first commit of the sprint, no exceptions

| Task | Est. | Status | Notes |
|------|------|--------|-------|
| S14-01 (BUG-067) | 0.05d | ❌ NOT DONE | Verified against source 2026-09-15: `ResourceReceiver.cs:17-23` still swapped — `ReceverRecovery()` calls `vitalStatsComponent.ReceiveReduction()`, `ReceiveReduction()` calls `.ReceiverRecovery()`. Unchanged |
| S14-02 (BUG-068) | 0.05d | ❌ NOT DONE | `AbilityHolder.cs:89-94` — `GetAbility()` still dereferences `currentAbility.Definition.AnimatorOverride` with no null-guard after a `TryGetValue` miss. Unchanged |
| S14-03 (BUG-063) | 0.05d | ❌ NOT DONE | `Stat.cs:63-66` — `#if UNITY_EDITOR [SerializeField]` still wraps `modifiers`, directly contradicting its own comment block ("KHÔNG BAO GIỜ thêm [SerializeField]" / "NEVER add [SerializeField]" two lines above). Unchanged |
| S14-04 (BUG-064 item 7) | 0.1d | ❌ NOT DONE | `RangeWeapon.cs:7` — `poolManager` still `[SerializeField] private IObjecPoolService`, no `[Inject]`. Unchanged |
| S14-05 (BUG-065) | 0.1d | ❌ NOT DONE | `PlayerDeathState.cs:10-12` — `Enter()` still only calls `base.Enter()`. Unchanged |
| S14-06 (BUG-066 + BUG-070) | 0.2d | ❌ NOT DONE | `EntityVitalStats.cs` and `VitalComponent.cs` both still index `currentStats[statType]` raw, no `TryGetValue` guard. Unchanged |

**Gate result: 0/6 — the hard gate did not hold.** This is the **4th consecutive sprint** (11, 12,
13, 14) in which these zero-dependency, fully-diagnosed, sub-0.05–0.2d fixes lost the session to
unplanned work. What actually landed Monday instead (`926eb6e` "coding", `edd7454` "add effect,
ability paladin asset", merged via `d65d68f`): a full new Paladin ability kit under
`Assets/SO/Skill/Paladin/` (Avatar of Light, Blessed Slash, Blessing, Consecrate — each a new
`AbilityDefinition` + effect assets) plus four new/changed runtime files (`RecoveryStatEffect.cs`,
`GainStatsForDuration.cs`, `ConsecratProjectile.cs`, `SlashProjectile.cs`, `SpawnMono.cs`) and edits
to `AbilityHolder.cs`, `PlayerData.cs` (−27 lines), `PlayerInputHandle.cs`, `StatHandler.cs`,
`VitalComponent.cs`, `Weapon.cs`. Note: the source branch is named `feature/fix-player-control` but
contains no player-control fix and none of the six gated bug fixes — branch name does not match
content. None of this work has had a same-day `/code-review` pass (S14-08, itself still open,
would have required one).

### Tue 2026-09-15 — RE-SEQUENCED: retry the six-item gate first (Monday's gate did not hold)

| Task | Est. | Status | Notes |
|------|------|--------|-------|
| S14-01 (BUG-067) | 0.05d | ⬜ NOT STARTED | Carried from Mon — retry as literal first commit of today's session |
| S14-02 (BUG-068) | 0.05d | ⬜ NOT STARTED | Carried from Mon |
| S14-03 (BUG-063) | 0.05d | ⬜ NOT STARTED | Carried from Mon — 27th+ carry overall |
| S14-04 (BUG-064 item 7) | 0.1d | ⬜ NOT STARTED | Carried from Mon — 4th carry overall |
| S14-05 (BUG-065) | 0.1d | ⬜ NOT STARTED | Carried from Mon — 4th carry overall |
| S14-06 (BUG-066 + BUG-070) | 0.2d | ⬜ NOT STARTED | Carried from Mon |
| S14-07 — Owner-in-Editor Play Mode smoke session | 0.2d | ⬜ NOT STARTED | **Pushed behind the gate retry** — do not attempt before S14-01–06 land; 7th consecutive sprint asking for this |
| S14-08 (off-plan review gate decision) | 0.1d | ⬜ NOT STARTED | Producer decision — retro action item #4. Monday's off-plan Paladin-kit landing without review is a live argument for this decision, not a hypothetical |
| S14-09 (pre-push hook placeholder) | 0.15d | ⬜ NOT STARTED | 22nd+ carry |

Goal: land the six-item gate today since Monday did not — nothing else on today's plan should be
attempted before it, matching Monday's own hard-gate rule now applied a day late.

### Wed 2026-09-16 — RE-SEQUENCED AGAIN: retry the six-item gate first (Tuesday's gate did not hold either)

| Task | Est. | Status | Notes |
|------|------|--------|-------|
| S14-01 (BUG-067) | 0.05d | ❌ NOT DONE | Re-verified 2026-09-16: `ResourceReceiver.cs:17-23` still swapped. Unchanged |
| S14-02 (BUG-068) | 0.05d | ❌ NOT DONE | `AbilityHolder.cs:89-95` still dereferences `currentAbility.Definition.AnimatorOverride` with no null-guard after a `TryGetValue` miss. Unchanged (Tuesday's `9d9169a` touched this file but not this bug) |
| S14-03 (BUG-063) | 0.05d | ❌ NOT DONE | `Stat.cs:63-66` still wraps `modifiers` in `#if UNITY_EDITOR [SerializeField]`. Unchanged |
| S14-04 (BUG-064 item 7) | 0.1d | ❌ NOT DONE | `RangeWeapon.cs:7` still `[SerializeField] private IObjecPoolService poolManager`, no `[Inject]`. Unchanged |
| S14-05 (BUG-065) | 0.1d | ❌ NOT DONE | `PlayerDeathState.cs:10-13` `Enter()` still only calls `base.Enter()`. Unchanged |
| S14-06 (BUG-066 + BUG-070) | 0.2d | ❌ NOT DONE | `EntityVitalStats.cs` and `VitalComponent.cs` both still raw-index `currentStats[statType]` (7 sites each, no `TryGetValue`). Unchanged |

**Gate result: 0/6 — 3rd consecutive day the gate has not held, 5th consecutive sprint (11→14, twice
within 14) of this exact pattern.** Tuesday 22:08 landed `9d9169a` "Clearn ability system hold and
activate" instead — touches `AbilityHolder.cs`, `PlayerInputHandle.cs`, `PlayerSkillWeaponState.cs`,
`AbilityDefinition.cs`, `AbilityEffectDefinition.cs`, `AbilityInstance.cs` (48+/26− lines), merged via
`922030a` from the same `origin/feature/fix-player-control` branch that still contains no
player-control fix and none of the six gated bug fixes. No same-day `/code-review` ran on it — S14-08
(the decision meant to require this) is now its 3rd cycle open with zero movement. Note: at the time
of this standup, `origin/feature/fix-player-control` had two more live commits not yet merged into
`sprint-14` (`1f61753` "Update logic SO effect stats impact", `1a3947a` "fix conflict") — an active
session appears to be continuing work on that branch outside the sprint-14 gate entirely.

| S14-10 (S4-05/S4-06 forced decision) | 0.1d | ⬜ NOT STARTED | 19th+ carry — deferred behind gate retry |
| S14-11 (ADR-0002 → Accepted) | 0.1d | ⬜ NOT STARTED | Re-verified: `docs/architecture/adr-0002-enemymanager-singleton-exception.md:4` still reads `Proposed`. 15th+ carry — deferred behind gate retry |

### Thu 2026-09-17 — Architecture decision + test + cooldown fix

| Task | Est. | Status | Notes |
|------|------|--------|-------|
| S14-01 (BUG-067) | 0.05d | ⚠️ APPARENTLY FIXED (incidental, unverified) | `1f61753` (off-plan, `feature/fix-player-control`, merged `4f31cb6` this morning) renamed + correctly paired `Recovery`/`Reduction` — see BUG-067.md Resolution Note. Not a gate commit, no review, no test, no Play Mode confirm |
| S14-02 (BUG-068) | 0.05d | ❌ NOT DONE | `AbilityHolder.cs:89-93` `GetAbility()` still does `TryGetValue` then immediately dereferences `currentAbility.Definition.AnimatorOverride` unguarded. Unchanged |
| S14-03 (BUG-063) | 0.05d | ❌ NOT DONE | `Stat.cs:63-66` still wraps `modifiers` in `#if UNITY_EDITOR [SerializeField]`. Unchanged |
| S14-04 (BUG-064 item 7) | 0.1d | ❌ NOT DONE | `RangeWeapon.cs:7` still `[SerializeField] private IObjecPoolService poolManager`, no `[Inject]`. Unchanged |
| S14-05 (BUG-065) | 0.1d | ❌ NOT DONE | `PlayerDeathState.cs:10-13` `Enter()` still only calls `base.Enter()`. Unchanged |
| S14-06 (BUG-066 + BUG-070) | 0.2d | ❌ NOT DONE | `EntityVitalStats.cs` (lines 39,49,61,67) and `VitalComponent.cs` both still raw-index `currentStats[statType]`, no guard. Unchanged |
| S14-12 (TD-040 ADR — Abilities v1/v2 convergence) | 0.3d | ❌ NOT DONE | No file under `docs/architecture/` for TD-040. Widened scope of BUG-052 this cycle — a 4th sprint of v2 work must not ship with this still open |
| S14-13 (first EditMode test, vitals guard) | 0.3d | ❌ NOT DONE | `tests/EditMode/` still `.gitkeep`-only. Pairs with S14-06 |
| S14-14 (BUG-069 cooldown wiring) | 0.15d | ❌ NOT DONE | Not checked in source this pass — carried |

**Gate result: 1/6 (best case) — 4th consecutive day the hard gate has not held through intentional
work; the one item that now reads fixed (BUG-067) landed as a side effect of unrelated, unreviewed,
untested off-plan work, not the gate itself.** This is now the mitigation's 5th failure out of 5 days
this sprint if BUG-067 is not counted as a genuine gate pass (it wasn't attempted as one). Only one day
of capacity remains (Fri). Per the Sprint 13 retro's own escalation rule (restated at Wed's standup):
remaining capacity should go to forcing S14-02 through S14-06 directly rather than any Should-Have or
Nice-to-Have item.

### Fri 2026-09-18 — Stretch + wrap prep

| Task | Est. | Notes |
|------|------|-------|
| S14-N2 (re-verify older bug set against the S14-07 smoke session) | 0.2d | Only if S14-07 landed Tuesday as planned |
| S14-N1 (first playtest) | — | Only if S14-07 confirmed stable — last log 2026-06-12 |
| Friday wrap-up prep | — | Feeds into Sat 22:00 `/weekly-wrapup` |

---

## Standup Log

### Thu 2026-09-17 — Daily Standup (autonomous, no owner present)

Checked the six-item hard gate first, per its own rule, before reviewing anything else. Result:
**1/6 arguably landed, but not as a gate pass** — 5 still open, verified directly against source
(not commit messages):

- ⚠️ S14-01/BUG-067 — **now reads fixed in source.** `ResourceReceiver.Recovery()` /
  `.Reduction()` correctly call `VitalStatsComponent.Recovery()` / `.Reduction()` (heal increases,
  damage decreases); `RecoveryEffectDefinition.Apply()` confirmed calling the recovery path. The fix
  landed via `1f61753` ("Update logic SO effect stats impact", Wed 22:55, off-plan on
  `origin/feature/fix-player-control`, merged into `sprint-14` at `4f31cb6` this morning) while
  renaming the interface methods for unrelated reasons — not a targeted S14-01 commit. No same-day
  review, no EditMode test, no Play Mode confirmation. Full detail logged in
  `production/qa/bugs/BUG-067.md`'s new Resolution Note; left as "apparently fixed, unverified"
  rather than closed
- ❌ S14-02/BUG-068 — `AbilityHolder.cs:89-93` `GetAbility()` still does `TryGetValue` then
  immediately dereferences `currentAbility.Definition.AnimatorOverride` with no null-guard.
  Unchanged
- ❌ S14-03/BUG-063 — `Stat.cs:63-66` still wraps `modifiers` in `#if UNITY_EDITOR [SerializeField]`,
  still directly contradicting its own explanatory comment. Unchanged. 28th+ consecutive carry
- ❌ S14-04/BUG-064 item 7 — `RangeWeapon.cs:7` still `[SerializeField] private IObjecPoolService
  poolManager`, no `[Inject]`. Unchanged
- ❌ S14-05/BUG-065 — `PlayerDeathState.cs` `Enter()` still only calls `base.Enter()`. Unchanged
- ❌ S14-06/BUG-066+070 — `EntityVitalStats.cs` and `VitalComponent.cs` both still raw-index
  `currentStats[statType]` with no `TryGetValue` guard. Unchanged

New commits on `sprint-14` since Wed's standup: `1a3947a` "fix conflict" (already flagged Wed as an
in-progress uncommitted change on the parallel branch), `1f61753` (see above), a merge (`a36f266`),
`e02bf3b` "add lightning - rune circle animation, done blessing ability" (VFX/animation assets +
ability SO renames — `Blessed Slash`, `Blessing`, buff/debuff and recovery/reduction effect asset
renames — plus a `LoadRandomMap.unity` scene change), and a second merge (`4f31cb6`). All four
non-merge commits are on `origin/feature/fix-player-control`, the same branch flagged every prior
standup this sprint as containing none of the six gated fixes and no player-control work despite its
name. This is now the **5th consecutive day** (Mon–Thu) the hard gate introduced Sunday specifically
to break the Sprint 11–13 pattern has not held through intentional, reviewed work.

**Escalation, per the Sprint 13 retro's own rule and Wed's standup note:** two days of the mitigation
window are gone; one remains (Fri). Recommending Friday's session (owner or autonomous) spend its
entire budget forcing S14-02 through S14-06 through directly as isolated commits — each is
sub-0.2d, zero-dependency, and fully diagnosed with exact file/line locations already recorded three
standups running — rather than attempting S14-12/13/14 (Should-Have) or any further off-plan work.
If S14-02–06 still do not land Friday, Sprint 14 closes with the hard-gate mitigation itself having
failed 5/5 days, which the retro should treat as evidence the mitigation's design (not the fixes'
difficulty) is the problem — e.g., the fixes need to be forced as the literal first tool calls of a
session with no other work permitted until they land, rather than "checked first, then whatever
happens happens."

Also re-verified: ADR-0002 (S14-11) still reads `Status: Proposed` — 16th+ carry. No
`docs/architecture/` file exists for TD-040 (S14-12). `.git/hooks/pre-push` still absent (S14-09) —
23rd+ carry. `tests/EditMode/` still `.gitkeep`-only (S14-13/TD-014). No owner-in-Editor Play Mode
session has occurred (S14-07) — would be the 7th consecutive sprint ask if it slips again; only one
day of capacity remains to attempt it, and it depends on S14-02/04 which are still open. QA plan:
still missing, 29th+ consecutive cycle.

Today's plan: force S14-02 through S14-06 as literal next commits (5th attempt this sprint); if they
land, attempt S14-07 same day.

---

### Wed 2026-09-16 — Daily Standup (autonomous, no owner present)

Checked the six-item hard gate first, per its own rule, before reviewing anything else — **0/6
landed, 3rd consecutive day.** Verified each item directly against current `sprint-14` source:

- ❌ S14-01/BUG-067 — `ResourceReceiver.cs:17-23` still has recovery/reduction calls swapped
- ❌ S14-02/BUG-068 — `AbilityHolder.cs:89-95` still dereferences `currentAbility.Definition`
  unguarded (Tuesday's commit edited this file for unrelated hold/activate cleanup, not this bug)
- ❌ S14-03/BUG-063 — `Stat.cs:63-66` still serializes `modifiers` under `#if UNITY_EDITOR`
- ❌ S14-04/BUG-064 item 7 — `RangeWeapon.cs:7` still Inspector-serialized, not DI-wired
- ❌ S14-05/BUG-065 — `PlayerDeathState.cs` `Enter()` still only calls `base.Enter()`
- ❌ S14-06/BUG-066+070 — `EntityVitalStats.cs` / `VitalComponent.cs` both still raw-index
  `currentStats[statType]`, 7 sites each, no guard

Only new commit on `sprint-14` since yesterday's standup: `9d9169a` "Clearn ability system hold and
activate" (22:08 Tue, merged via `922030a`) — ability-hold/activate cleanup across `AbilityHolder.cs`,
`PlayerInputHandle.cs`, `PlayerSkillWeaponState.cs`, `AbilityDefinition.cs`,
`AbilityEffectDefinition.cs`, `AbilityInstance.cs`. Off-plan relative to the gate, same as Monday and
Tuesday's landings; no same-day code review recorded.

**Process note:** during this standup's git inspection, the working directory was briefly on a
different local branch (`origin/feature/fix-player-control`) with live uncommitted changes to
`VitalComponent.cs` / `IVitalComponent.cs` / two Ability effect files — evidence of an actively
running session (not this one) continuing work outside `sprint-14`. No files were touched by this
standup; the checkout to `sprint-14` was retried only after that branch's tree went clean on its own
(commit `1a3947a` "fix conflict" landed mid-check). Flagging because it means Abilities v2 work is
being actively extended on a third branch (`origin/feature/fix-player-control`) concurrently with
both `sprint-14`'s stalled gate and the still-open S14-08/S14-12 decisions about exactly this kind of
parallel, unreviewed ability-system work.

Also re-verified the two Should-Have sign-off items originally scheduled for today: S14-11
(`ADR-0002` → Accepted) still reads `Proposed` at line 4 — 15th+ carry, no technical blocker. Pre-push
hook (S14-09) still absent (`.git/hooks/pre-push` does not exist) — 22nd+ carry. No `docs/architecture/`
file exists yet for TD-040 (S14-12). Deferred both S14-10/S14-11 behind another gate retry, consistent
with the sprint's own hard-gate rule.

**This is now a 4-for-5 failure of the hard-gate mitigation itself** (introduced Sunday specifically
to break the Sprint 11-13 pattern; held on 0 of 3 days so far this sprint). Two days of capacity
remain (Thu, Fri). Escalating per the Sprint 13 retro's own recommendation: if S14-01–06 do not land
Thursday, Friday should not be spent on Should-Have items — it should be spent forcing the six fixes
through directly, since every "schedule it first" and "re-sequence tomorrow" mitigation tried this
sprint (and the two before it) has failed identically.

Today's plan: retry S14-01 through S14-06 as literal first commits (4th attempt); if they land, proceed
to S14-07 (owner Play Mode session, 7th consecutive sprint ask) same day. QA plan: still missing, 28th+
cycle.

---

### Tue 2026-09-15 — Daily Standup (autonomous, no owner present)

Checked the six-item hard gate first, per its own rule, before reviewing anything else — **0/6
landed**. Verified each item directly against current source (not commit messages):

- ❌ S14-01/BUG-067 — `ResourceReceiver.cs:17-23` still has the recovery/reduction calls swapped
- ❌ S14-02/BUG-068 — `AbilityHolder.cs:89-94` still dereferences `currentAbility.Definition`
  unguarded after a possible `TryGetValue` miss
- ❌ S14-03/BUG-063 — `Stat.cs:63-66` still serializes `modifiers` under `#if UNITY_EDITOR`
- ❌ S14-04/BUG-064 item 7 — `RangeWeapon.cs:7` still Inspector-serialized, not DI-wired
- ❌ S14-05/BUG-065 — `PlayerDeathState.cs` `Enter()` still only calls `base.Enter()`
- ❌ S14-06/BUG-066+070 — `EntityVitalStats.cs` / `VitalComponent.cs` both still raw-index the
  `currentStats` dictionary

This is the **4th consecutive sprint** (11 → 14) this exact pattern has repeated. What landed
instead: a full Paladin ability kit (`Assets/SO/Skill/Paladin/` — Avatar of Light, Blessed Slash,
Blessing, Consecrate) plus supporting runtime code (`RecoveryStatEffect.cs`,
`GainStatsForDuration.cs`, `ConsecratProjectile.cs`, `SlashProjectile.cs`, `SpawnMono.cs`,
`AbilityHolder.cs`, `PlayerData.cs`, `PlayerInputHandle.cs`, `StatHandler.cs`, `VitalComponent.cs`,
`Weapon.cs`), merged from a branch named `feature/fix-player-control` that contains no player-control
fix. No same-day code review ran on it (S14-08, the review-gate decision meant to require this, is
itself still open — 2nd cycle unresolved).

Re-sequenced today's plan: S14-01 through S14-06 retried as today's literal first six commits before
S14-07/08/09 are attempted (see Tue table above). S14-07 (owner Play Mode session) is now a candidate
for the retro's "stop re-asking, record as accepted risk" recommendation if it slips again — this
would be sprint 7 of that ask.

No new commits to review beyond what's captured above (checked `git log` since 2026-09-14 00:00 —
only the two coding commits + merge, all already accounted for). QA plan: still missing, 27th+ cycle.

---

### Sun 2026-09-13 — Weekly Kickoff (autonomous, no owner present)

Sprint 13 closed FAIL (0/6 Must-Have met, 2/6 Should-Have landed) — already finalized in Saturday's
`pm-weekly-wrapup` run (2026-09-13), this kickoff only opened Sprint 14. Branch `sprint-14` created
cleanly from `sprint-13` tip (`c4be3c7`) — no stale local branch conflict this cycle (unlike Sprint 13's
kickoff). Re-verified the carried-forward state directly against `retro-sprint-13-2026-09-13.md` and
`production/qa/bug-triage-2026-09-13.md` (both written same-day by the wrap-up run) rather than
re-deriving from source:

- 🆕 **BUG-067** — `ResourceReceiver` heal/damage swap, live regression, found in wrap-up code review.
  Fix first. S14-01.
- 🆕 **BUG-068** — `AbilityHolder.GetAbility()` unguarded null deref, NRE on unbound ability slot, found
  in wrap-up code review. Fix second. S14-02.
- ❌ **BUG-063** — `Stat.cs:63-66` still wraps `modifiers` in `#if UNITY_EDITOR` / `[SerializeField]`.
  26th+ consecutive carry. S14-03.
- ❌ **BUG-064 item 7** — `RangeWeapon.cs:7` still has no `[Inject]` wiring. 3rd carry. S14-04.
- ❌ **BUG-065** — `PlayerDeathState.Enter()` still only calls `base.Enter()`. 3rd carry. S14-05.
- ❌ **BUG-066** + 🆕 **BUG-070** — both `EntityVitalStats` and the new `VitalStatsComponent` twin index
  a dictionary with no guard. Bundle into one fix. S14-06.
- ✅ **S13-11 (batch bug-file generation) — resolved at this kickoff.** All 9 open bugs now have
  individual files (10 on disk incl. closed BUG-053). No longer carried.
- ❌ **S13-06 → S14-09** (pre-push hook) — still no `.git/hooks/pre-push`. 21st+ carry.
- ❌ **S13-08 → S14-10** (S4-05/S4-06 decision) — 18th+ carry, oldest unresolved item in the project.
- ❌ **S13-09 → S14-11** (ADR-0002) — still `Proposed`. 14th+ carry.
- 🆕 **S14-12** — TD-040 (Abilities v1/v2 convergence ADR) — `BUG-052`'s scope widened this cycle; needed
  before a 4th sprint of Abilities v2 work ships with the decision still open.
- 🆕 **S14-08** — producer decision on whether off-plan feature work needs a same-day review gate —
  retro action item #4, deferred from Sprint 13.

`gh` CLI still unavailable — draft PR not auto-created, manual command left in `sprint-14.md`. No QA
plan exists for the 26th+ consecutive cycle — flagged, deferred to owner per every prior cycle's
handling. No owner-in-Editor Play Mode session has occurred at any point across Sprint 8 through
Sprint 13 (6 sprints) — carried forward as S14-07, named explicitly. Per the Sprint 13 retro's explicit
recommendation, **a hard gate replaces the schedule-note mitigation this cycle** (see "Hard Gate"
section above) — three consecutive "schedule the cheap fix first" notes did not hold, so this kickoff
is trying a mechanical check instead of a fourth note.

---

## Carry-Over Watch List (re-verify every standup)

- **BUG-067 — ⚠️ apparently fixed in source as of 2026-09-17 (incidental, unverified — see
  `production/qa/bugs/BUG-067.md`).** BUG-068 — still open, S1, live and player-reachable. Fix
  before anything else — both threaten the Play Mode smoke session (S14-07) directly.
- **BUG-063 (`Stat.cs` `[SerializeField]` regression)** — 27th+ consecutive carry on a one-line fix with
  an explanatory comment already in the file. No technical blocker has ever existed for this item.
- **BUG-064 item 7 — `RangeWeapon.cs` DI wiring.** 4th carry. Fix pattern already proven same-repo
  (`ItemSpawner.cs:8-10`).
- **BUG-065 / BUG-066 / BUG-070** — small isolated fixes with no dependencies, unchanged since
  introduced, re-verified against source 2026-09-15.
- **Owner-in-Editor Play Mode session (S14-07)** — has not happened once across Sprint 8 through
  Sprint 13 (6 sprints); still not attempted in Sprint 14 as of Tue standup. Per the retro: if it does
  not happen this cycle either, Sprint 15 should stop scheduling it as a task and instead record it as
  an explicit accepted-risk decision.
- **S14-08 (off-plan review gate decision)** — retro action item #4, 2nd cycle open. Monday's
  unreviewed Paladin-kit landing is now a concrete example of the exact risk this decision is meant to
  close, not a hypothetical.
- **S14-10 (S4-05/S4-06)** — 19th+ carry, zero movement any cycle. Decision-avoidance, not an
  estimation problem.
- **S14-11 (ADR-0002 Accept)** — 15th+ carry, trivial sign-off-only change.
- **S14-12 (TD-040 — Abilities v1/v2 convergence)** — widened further Monday: a 4th sprint of
  Abilities v2 work (the Paladin kit) shipped with this decision still open, exactly the risk this
  item warns about.
- **S14-09 process gate** — now 22nd carry, same underlying pattern since Sprint 6/9.
- **BUG-069 (ability cooldown never enforced)** — every ability still spammable; unrelated to Monday's
  new ability content, which adds more spammable abilities to the surface.
- **NEW RISK (2026-09-15)** — the six-item hard gate introduced this sprint specifically to break a
  3-sprint failure pattern did not hold on its first day. If S14-01–06 do not land Tuesday either,
  this becomes a 4-for-4 failure of the mitigation itself, not just of the underlying tasks — worth
  flagging to the owner as a process-level issue, not just a scheduling one.
- QA plan — 27th+ consecutive cycle with none. Flagged in `sprint-14.md`, deferred to owner.
