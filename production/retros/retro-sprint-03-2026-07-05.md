# Sprint 3 Retrospective

> **Sprint**: 2026-06-23 to 2026-06-27
> **Retro date**: 2026-07-05 (wrap-up delayed 8 days — sprint ended 2026-06-27; wrap-up ran Saturday 2026-07-05)
> **Facilitator**: Automated PM (weekly-wrapup)
> **Branch**: sprint-03 @ ea9c534
> **Verdict**: FAIL — 0% Must-Have velocity; all 4 critical tasks carry to Sprint 4

---

## Sprint Goal Reminder

> Clear the P1 debt that is actively blocking combat verification: fix BUG-AH-1 (build-break), Bug #9 (animation stuck), complete `Core.GetCoreComponent<T>()` zero-alloc (S2-03), and land `WeaponMelee.Attack()` damage call (Bug #4) — so that by mid-week the equipped-weapon → attack → damage loop is playable.

**Outcome**: 0 of 4 Must-Have tasks completed. Sprint goal not met.

---

## Velocity

| Metric | Sprint 3 | Sprint 2 | Sprint 1 |
|--------|----------|----------|----------|
| Must-Have velocity | **0%** | 14% | ~20% |
| Commits | 13 (most: docs/debt/standup) | 9 | ~12 |
| Code commits (gameplay) | 2 (combo-attack, unplanned) | 3 | ~5 |
| P1 backlog age | **8th sprint** | 7th | 6th |
| Post-sprint gap (zero-commit days) | **12 days** | 8 days | — |

The same 4 P1 bugs have been open for 8 consecutive sprint cycles. This is no longer a sprint failure — it is a structural pattern.

---

## What Went Well

### 1. Stats system foundation arrived
Five new files under `Assets/Script/StatSystem/` constitute a well-designed stat architecture:
- `Stat.cs` — dirty-flag cached value, correct modifier layering (Flat → PercentAdd batch → PercentMult)
- `StatType.cs` — clean primary/derived split at enum value 100
- `StatModifier.cs` — immutable, source-tagged, sortable by Order
- `StatsSO.cs` — event-driven, `OnStatChanged`, `RecalculateDerived` with formula injection
- `DerivedStatFormula.cs` — linear combination formula (baseConstant + level×perLevel + Σ coefficient×stat)

The design is more robust than the simple SO field approach originally planned for S3-06. The modifier system (with `RemoveModifiersFromSource`) is production-quality.

### 2. RoomCell door bug fixed
`BUG-RC-1` — `RoomCell.GetDoor()` was using `new DoorController()` (illegal MonoBehaviour instantiation). Fixed via commit 05b76cc — now uses prefab Instantiate pattern. One real bug closed.

### 3. Combo-attack system polished and committed
The dirty working tree from end-of-Sprint 2 was committed (`2eb0765`, `a654831`). `PlayerAttackState` + `StatusAnimation` enum (`Start` / `EndRangeTrigger` / `None`) are stable. Input buffer (`BufferIsAttack`) is working. The combo foundation from Sprint 2 is now committed and not at risk of loss.

### 4. Technical debt documented
`docs(debt)` commit (b098f6f) added TD-021 through TD-032 from a room-system audit — 12 new debt items logged, not just discovered and forgotten. This is good PM hygiene.

### 5. Docs sync and GDD updated
`docs(gdd)` commit (50d1a28) updated `map-system.md` to match code state. `docs: sync CLAUDE.md` (c65b391) kept the master reference current. Docs are not drifting from code.

---

## What Did Not Go Well

### 1. Zero Must-Have velocity for the 3rd consecutive sprint
S3-01 → S3-04 are each 0.25d (2 hours) estimates. A combined 1 day of focused work would have closed all four. None were touched. The sprint's stated goal was not the sprint's actual work.

**Root cause (confirmed by commit log)**: Off-plan feature work (combo-attack polish, stats system architecture) consumed all available developer session time. The P1 backlog items are small and unglamorous; new architectural work is large and interesting. Developer attention gravitates to the latter every sprint.

### 2. 12-day post-sprint gap
The sprint closed 2026-06-27. The next commit was 2026-07-05 — 8 days later (standup + docs). No code commits in 8 days after sprint close. The weekly-wrapup was delayed by 8 days. Sprint 4 kickoff has not yet run as of this wrap-up.

### 3. Stats system shipped with 2 compile errors
`StatsSO.cs` references `StatTypes.Primary` and `StatTypes.Derived` which do not exist (BUG-SS-1 — compile error). `StatsSO.Start()` will never fire on a ScriptableObject (BUG-SS-2). The new system cannot function until both are fixed. Good architecture, broken wiring.

### 4. `NegativeReciver.TakeDamage()` is a crash stub
New file `NegativeReciver.cs` implements the `INegativeReceiver` interface but throws `NotImplementedException()`. If any enemy hits the player, the game crashes. This is a regression introduced this sprint.

### 5. P1 backlog deferred 8 consecutive sprints
| Bug | Age |
|-----|-----|
| BUG-04 `WeaponMelee.Attack()` empty | Sprint 1 (6+ weeks) |
| BUG-09 `AnimationPlayerController` double-reg | Sprint 2 (4+ weeks) |
| BUG-AH-1 `AbilityHolder` UnityEditor import | Sprint 2 (4+ weeks) |
| S2-03 `Core.GetCoreComponent` LINQ | Sprint 2 (4+ weeks) |

The melee damage call (BUG-04) is the top-priority blocker. It is a 4-line change. It has been deferred for 6+ weeks.

---

## Action Items for Sprint 4

| Priority | Action | Owner | Est |
|----------|--------|-------|-----|
| 1 | **Time-box rule**: Do NOT start any new feature/system until S4-01 → S4-04 are committed. Treat all 4 as Day 1 work. | Developer | 1d total |
| 2 | Fix BUG-SS-1: create `StatTypes` static class with `Primary` and `Derived` `IEnumerable<StatType>` | gameplay-programmer | 0.25d |
| 3 | Fix BUG-SS-2: remove `Start()` from `StatsSO`; add `OnEnable()` or lazy `Initialize()` call from `Get()`/`AddModifier()` | gameplay-programmer | 0.25d |
| 4 | Fix BUG-NR-1: implement `NegativeReciver.TakeDamage()` to call through to `PlayerData` health decrement | gameplay-programmer | 0.25d |
| 5 | Fix BUG-AH-1: remove `UnityEditor` import from `AbilityHolder.cs` | gameplay-programmer | 0.25d |
| 6 | Fix BUG-09: `AnimationPlayerController` line 21/29 double-registration | gameplay-programmer | 0.25d |
| 7 | Fix BUG-04: `WeaponMelee.Attack()` add `TakeDamage` call | gameplay-programmer | 0.25d |
| 8 | Split `Stat.cs` into separate files per class | lead-programmer | 0.25d |

**If all 8 actions above land in Sprint 4 Week 1**: the combat loop becomes playable for the first time. Run a playtest session immediately after — do not defer.

---

## Structural Pattern Note

Sprint 3 is the 8th consecutive sprint to close with the same P1 backlog unresolved. This retrospective cannot produce a process fix that was not already identified in Sprint 1 and Sprint 2 retros. The issue is not sprint planning, estimation, or tooling — it is developer session prioritization.

**Recommendation**: Sprint 4 kickoff should open with the 4 P1 fixes as its *only* Must-Have items. No Should-Have or Nice-to-Have items should appear until all 4 are done. If they slip again, consider a dedicated "debt day" as an explicit sprint event (block calendar time, treat it as non-negotiable).

---

## Playtest Status

No playtest session ran this sprint (2026-06-23 → 2026-06-27) or during the post-sprint gap (2026-06-28 → 2026-07-05). Last playtest: 2026-06-12.

Playtest is blocked until BUG-04 (`WeaponMelee.Attack()`) is fixed — melee damage does not apply to enemies, making combat unverifiable.

**Next playtest**: target immediately after BUG-04 + BUG-09 land in Sprint 4.
