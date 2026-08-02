# Sprint 7 — 2026-07-27 to 2026-07-31

**Status: CLOSED — verdict CONCERNS** (2026-08-02 Saturday wrap-up, autonomous). 6/9 of Sprint 6's
compile-blocking S1 bugs confirmed fixed (BUG-024/025/027/029/030/031) — component hub structurally
sound. Sprint's own stated goal ("verify enemy death chain end-to-end in Play Mode") **not met**:
combat confirmed non-functional in both directions at close (BUG-041 player attack unwired, BUG-042
enemy TakeDamage still throws NotImplementedException). Off-plan Pathfinding/Base-refactor work
recurred a 7th consecutive cycle, directly against this sprint's own "do not start" instruction;
S7-D4 root-cause conversation not held for the 2nd sprint running. Full detail:
`production/qa/bug-triage-2026-08-02.md`, `production/retros/retro-sprint-07-2026-08-02.md`, and the
closure section at the bottom of `sprint-07-daily-plan.md`. Carry-over list recorded there — Sprint 8
plan created separately at Sunday `/weekly-kickoff`.

**Opened:** 2026-07-26 Sunday 22:00 kickoff (autonomous scheduled run). Branch `sprint-07`, created
from `sprint-06` tip (`a27cb34`). `gh` CLI unavailable in this environment — draft PR
(`--base sprint-06 --head sprint-07`, title `Sprint 7`) was **not** auto-created; run manually if
desired:
`gh pr create --draft --base sprint-06 --head sprint-07 --title "Sprint 7"`

**Review mode:** lean (from `production/review-mode.txt`) — producer feasibility gate (PR-SPRINT)
skipped per lean-mode rule.

⚠️ **Working tree note at kickoff time**: `Assets/Script/Manager/EventManager.cs` was modified and
`Character/Base/Interface/ICharacter.cs.meta` / `Player/States/PlayerDeathState.cs.meta` were
untracked-but-present in the working tree when this branch was cut. This may be a partial fix already
in progress for BUG-029/BUG-026 below — verify current state before starting S7-00.

---

## Sprint Goal

**Stabilization sprint — get the branch compiling again, then verify the enemy death chain end-to-end
in Play Mode.** Sprint 6 closed CONCERNS: 9 S1 (compile-blocking or functionally-dead) bugs were found
in the Saturday wrap-up review, all tracing to the half-finished Base/CoreBase hub refactor and
Pathfinding work that shipped past the sprint's planned scope. No new feature work starts until
`Core.GetCoreComponent<T>()` compiles and is confirmed working for both Player and Entity.

---

## Capacity

- Total days: 5
- Buffer (20%): 1 day
- Available: 4 days

Must-Have load ≈ 2.65d — comfortable fit, but every item is a blocking-bug fix, not new scope. No
feature work should be scheduled this sprint until Must-Have closes.

---

## Carry-Over From Sprint 6

| Item | Type | Priority | Origin |
|------|------|----------|--------|
| BUG-024 — `[SerializeField]` on auto-property (CS0592), `CoreComponentBase.cs:5` | Bug (S1) | P0 | New, Sat wrap-up review |
| BUG-025 — bare `EndRangeTrigger` out of scope (CS0103), `PlayerDisadvantageState.cs:20` | Bug (S1) | P0 | New, Sat wrap-up review |
| BUG-026 — enum used as bool (CS0029), `PlayerDeathState.cs:17,21` | Bug (S1) | P0 | New, Sat wrap-up review |
| BUG-027 — `Transform`/position used as bool condition, `EntityMovement.cs:53` | Bug (S1) | P0 | New, Sat wrap-up review |
| BUG-028 — dead `entity` field ref + invalid `Transform - Vector2` operator, `EntityInput.cs:80,82,99,103` | Bug (S1) | P0 | New, Sat wrap-up review |
| BUG-029 — duplicate `ON_PLAYER_DEATH` enum member (CS0102), `EventManager.cs:42` vs `53` | Bug (S1) | P0 | New, Sat wrap-up review |
| BUG-030 — `Core.cs`/`EntityCore.cs` `Awake()` hides not overrides `CoreBase.Awake()` — `GetCoreComponent<T>()` silently always fails | Bug (S1) | P0 | New, Sat wrap-up review |
| BUG-031 — `CoreComponentBase.Setup()` override commented out — `Core` back-ref always null, 5+ NullRef call sites | Bug (S1) | P0 | New, Sat wrap-up review |
| BUG-032 — `EntityWeaponMelee.SetAbility()` `input` field assignment commented out — enemy skill NullRef every call (regression) | Bug (S1) | P1 | New, Sat wrap-up review |
| BUG-033 (was BUG-ES-1) — `EnemySpawner.cs:62` null-check order still wrong, now masked (looks guarded, isn't) | Bug (S1, regressed) | P1 | 5th carry |
| Bug #6 — `NegativeReciver.currentHealth` is a separate never-initialized field, disconnected from `PlayerData.currentHealth`; no `ON_PLAYER_DEATH` listener anywhere | Bug (S1, regressed 2nd way) | P1 | 8th carry |
| ADR-0002 (`EnemyManager` singleton) Proposed→Accepted | Decision | P2 | 5th carry |
| S4-05/S4-06 keep-or-cut call | Decision | P3 | 6th carry — force decision this cycle |
| Bug #13 — start-room teleport dead code, still commented out | Bug (S1, carried) | P2 | 3rd+ carry |
| Bug #15 — `File.ReadAllText(Application.dataPath...)` on core runtime path, breaks Player build | Bug (S1, carried) | P2 | 2nd carry, confirmed worse this cycle |
| Off-plan-work root cause — 3 consecutive sprints of late-week unplanned architecture work shipping uncompiled | Process | P1 | Sprint 5 retro asked for this; still not held |
| No QA plan — 5 consecutive cycles | Risk | P1 | Flagged again below |
| First playtest — 8 cycles with zero sessions | Milestone | P2 | Blocked on compile status |

---

## Tasks

### Must Have (P0/P1)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S7-00 | Fix BUG-024 — `CoreComponentBase.cs:5` `[SerializeField]` on auto-property → backing field or drop attribute | lead-programmer | 0.15 | None | Console: zero CS0592 |
| S7-01 | Fix BUG-025 — `PlayerDisadvantageState.cs:20` bare `EndRangeTrigger` — resolve correct identifier/scope | gameplay-programmer | 0.1 | None | Console: zero CS0103 at this site |
| S7-02 | Fix BUG-026 — `PlayerDeathState.cs:17,21` `if (StatusAnimation.Start)` enum-as-bool → correct comparison | gameplay-programmer | 0.15 | None | Console: zero CS0029 at this site |
| S7-03 | Fix BUG-027 — `EntityMovement.cs:53` position used as bool condition → correct null/distance check | ai-programmer | 0.15 | None | Compiles; logic reviewed against original intent |
| S7-04 | Fix BUG-028 — `EntityInput.cs:80,82,99,103` remove dead `entity` field ref, fix `Transform - Vector2` operator | ai-programmer | 0.25 | None | Compiles; FOV/target math verified in Play Mode |
| S7-05 | Fix BUG-029 — `EventManager.cs:42` duplicate `ON_PLAYER_DEATH` — remove dup, keep line 53 | lead-programmer | 0.1 | None | Console: zero CS0102; single `ON_PLAYER_DEATH` member |
| S7-06 | Fix BUG-030 — `Core.cs:7`/`EntityCore.cs:17` `Awake()` must `override` not hide `CoreBase.Awake()` | lead-programmer | 0.3 | S7-00 | `Setup()`/component registration runs on scene load, confirmed via log |
| S7-07 | Fix BUG-031 — `CoreComponentBase.cs:17-21` restore `Setup()` override so `Core` back-ref is populated | lead-programmer | 0.3 | S7-06 | `Core` non-null at all 5 flagged call sites (`Interact.cs:30`, `PlayerInputHandle.cs:96-97`, `AbilityHolder.cs:37`, `NegativeReciver.cs:10`, `EntityInput.cs:60`) |
| S7-08 | **Verify** — Play Mode smoke check: `Core.GetCoreComponent<T>()` resolves correctly for both Player and Entity prefabs | lead-programmer | 0.2 | S7-00..S7-07 | Zero Console errors on scene load; component hub resolves for at least one Player and one Entity instance |
| S7-09 | Fix BUG-032 — `EntityWeaponMelee.cs:26,49` restore `input` field assignment (was `holder.EntityCore.Entity.Input.Skill`) | ai-programmer | 0.2 | S7-08 | Enemy skill use in Play Mode: no NullReferenceException |
| S7-10 | Fix BUG-033/BUG-ES-1 — `EnemySpawner.cs:62`, `RoomModel.cs:16` — correct null-check order (`set == null` before `.Count`) | gameplay-programmer | 0.15 | None | Empty-pool room spawns without NullReferenceException |
| S7-11 | **Re-scope Bug #6** as its own story — write-through `NegativeReciver`→`PlayerData.currentHealth`, add `ON_PLAYER_DEATH` listener, EditMode test `TakeDamage_BelowZero_TriggersDeathState` | gameplay-programmer | 0.4 | S7-08 | Test passes; single HP source of truth; `Reborn()` contract intact; listener confirmed firing |
| S7-12 | Flip ADR-0002 (`EnemyManager` singleton) `Proposed → Accepted` | producer | 0.1 | None | ADR-0002 Status reads Accepted; sign-off note recorded |
| S7-13 | **Forced decision** — S4-05/S4-06 keep-or-cut (6th carry, no further silent re-carry) | producer | 0.1 | None | Written decision recorded; removed from tracker if cut, re-estimated into Sprint 8 if kept |

Must-Have total ≈ **2.65d** (0.15+0.1+0.15+0.15+0.25+0.1+0.3+0.3+0.2+0.2+0.15+0.4+0.1+0.1) — comfortable
against 4 days available, but sequencing matters: S7-06/S7-07/S7-08 gate S7-09 and S7-11.

### Should Have (P2/P3)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S7-D1 | Fix Bug #13 — uncomment start-room teleport call site or wire `RoomGeneraterController.OnDoneLoadRoomGrid()` in | lead-programmer | 0.25 | S7-08 | Player spawns at start-room `StartDoorPosition` on new run |
| S7-D2 | Fix Bug #15 — replace `File.ReadAllText(Application.dataPath...)` with `TextAsset` refs or StreamingAssets for room JSON load | lead-programmer | 0.5 | None | Room load works from a Player build, not just Editor |
| S7-D3 | Process change — file individual `production/qa/bugs/BUG-NNN.md` reports going forward instead of tracker-prose | qa-lead | 0.2 | None | At least the 9 new S1 bugs from this sprint have individual files |
| S7-D4 | **Hold the off-plan-work root-cause conversation** — Sprint 5 retro asked for this, still open after 2 cycles; producer-facilitated, written outcome | producer | 0.3 | None | Written root-cause doc with a concrete process change (e.g., pre-push compile check), not just a restated observation |
| S7-D5 | BUG-034 — wire `SlashAbility.cs:35` / `Projectile.cs:55` to the unused `Poolable/` system instead of raw `Instantiate`/`Destroy` | gameplay-programmer | 0.4 | None | Both call sites use `ObjectPoolManager`; zero raw `Instantiate`/`Destroy` for pooled types |

**Explicitly deferred, do not start this sprint**: BUG-035/036/037 (Pathfinding correctness/perf) — per
Sat wrap-up recommendation #2, no further work on Pathfinding or Base/CoreBase until the hub refactor
is confirmed compiling and working (S7-08 gate).

### Nice to Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S7-N1 | First full playtest session — once S7-08/S7-09/S7-11 land | producer / owner | — | S7-08, S7-09, S7-11 | `/playtest-report` filed; first session since `playtest-2026-06-12`, 8th cycle attempt |
| S7-N2 | Start `EnemyManager` lifecycle body (aliveCount, `ON_CLEAR_ENEMY`/`ON_ROOM_CLEAR`) — only if Must-Have closes early | ai-programmer | 1.0 | S7-08, S7-12 | Stretch only; defer cleanly to Sprint 8 if not started |

---

## Definition of Done for This Sprint

- [ ] Zero compile errors in Unity Editor Console (BUG-024 through BUG-029 confirmed fixed)
- [ ] `Core.GetCoreComponent<T>()` verified working in Play Mode for Player and Entity (S7-08)
- [ ] Enemy skill use (BUG-032) verified no NullReferenceException in Play Mode
- [ ] Empty-pool spawn (BUG-033) verified no exception
- [ ] Bug #6 closed with a passing EditMode test, not an opportunistic fix
- [ ] ADR-0002 status resolved to Accepted
- [ ] S4-05/S4-06 forced decision recorded
- [ ] Off-plan-work root-cause conversation held and documented
- [ ] QA Plan gate resolved (see below)

---

## QA Plan

⚠️ **No QA plan exists** for Sprint 7 (`production/qa/qa-plan-sprint-07.md` not found) — **5th
consecutive sprint cycle** without one. This kickoff ran autonomously (no user present); per the QA
plan gate, the choice requiring judgment (full plan now vs. defer) is deferred to the owner rather
than decided unattended.

> ⚠️ This sprint was started without a QA plan. Run `/qa-plan sprint` before the last story is
> implemented. The Production → Polish gate requires a QA sign-off report, which requires a QA plan.
> Given this sprint is entirely bug-fix work with two EditMode-test-gated items (S7-08, S7-11), a QA
> plan run early would meaningfully de-risk sign-off later.

---

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| Off-plan work recurs a 4th time | High (3 consecutive prior cycles) | High | S7-D4 explicitly scheduled to address root cause, not just re-flag it. Sprint scope is narrow (2.65d) specifically so a partial week still closes Must-Have. |
| Fixes to BUG-024..031 uncover further compile errors not caught by this cycle's review (agents review a diff, not a full solve-and-rebuild) | Medium | High | S7-08 is an explicit Play-Mode verification gate, not just "compiles" — treat it as the real Definition of Done checkpoint before S7-09/S7-11 start |
| Bug #6 fails a 3rd time | Medium | Medium | Re-scoped as its own story with a mandatory EditMode test this time (S7-11), per test-standards.md and the wrap-up's explicit recommendation |
| No QA plan — 5th consecutive cycle | Confirmed | Medium | Flagged explicitly above; deferred to owner rather than silently dropped |
| No Unity CLI in this environment | Known constraint | Low | All Play Mode smoke checks (S7-08, S7-09, S7-10) are manual in-Editor by the owner |

---

## Dependencies on External Factors

- No Unity CLI — Play Mode smoke checks require manual in-Editor confirmation by the owner.
- `gh` CLI unavailable — draft PR for `sprint-07` (base `sprint-06`) not auto-created; run manually:
  `gh pr create --draft --base sprint-06 --head sprint-07 --title "Sprint 7"`.

---

## Next Sprint Outlook (Sprint 8)

- If Must-Have closes clean: resume Pathfinding correctness/perf work (BUG-035/036/037) and
  `EnemyManager` lifecycle (aliveCount, room-clear) now that the component hub is verified working.
- Bug #13 (start-room teleport) and Bug #15 (build-safe JSON load) if not completed as Should-Have here.
- First full playtest, if not already run in S7-N1.
- HUD health bar + between-room upgrade cards, once Bug #6's death chain is confirmed stable.
