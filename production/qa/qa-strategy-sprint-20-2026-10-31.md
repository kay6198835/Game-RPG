# QA Strategy: Sprint 20 (2026-10-26 .. 2026-10-30)

Author: QA Lead. Written 2026-10-31. Config: qa.level standard, team.size individual, review_mode lean, stage Production.
Source: `production/sprints/sprint-20.md` (Tasks tables). No separate story files exist. No final verdict is given here.

## Header lines

- **Smoke Check:** UNKNOWN (source: no report found; `production/qa/smoke-*.md` absent). Checklist exists at `tests/smoke/critical-paths.md`; playtest sheet `production/qa/playtests/playtest-2026-10-24.md` is empty.
- **Open bugs in scope:**
  - S1: BUG-108, BUG-096, BUG-087 (partial), BUG-092 (partial).
  - S2: BUG-065, 066, 070, 071, 072, 073, 083, 084, 086, 095, 097, 100, 105, 106, 109, 110, 113, 114, 115; BUG-107 partial.
  - Targeted by in-scope stories: 108, 096, 106, 107, 110, 097, 105, 095, 072, 065, 086, 092, 066, 070, 115, 104, 103, 102, 068, 073, 090, 112.
  - Not targeted (deferred D-stories): 071, 100, 113, 114, 087, 109, 083, 084. These stay open through the sprint.
- **Test infrastructure:** zero automated tests, no `.asmdef` (BUG-084). No Logic story can produce automated evidence this sprint. Every "Automated Required" below is therefore unmet by construction. See Concerns.

## Summary table

| Story | Type | Automated | Manual | Blocker? | AC testable as written? |
|---|---|---|---|---|---|
| S20-01 Playtest / Play Mode smoke | Integration | No (not automatable) | Required | Yes, gates S20-F1/S20-08/S20-09 and is the only QA gate | Yes (sheet filled, or signed accepted-risk note) |
| S20-02 BUG-108 FormerlySerializedAs | Config/Data (S1 severity) | Not required (advisory) | Required (Play Mode: Knight enemies spawn, `Stats` non-null) | Yes for the dungeon loop (S1) | Yes |
| S20-03 BUG-096 zero-spawn branch | Logic | Required, impossible (BUG-084) | Required (one room of each type: Start, Rest, Shop, Buff, Boss) | Yes (S1; evidence gap) | Yes, but name the room list |
| S20-04 Weapon trio (106/107/110) | Integration (Logic inside) | Required for 107 null-guard, impossible | Required (enemy attacks; Mage confirm; pickup weapon attacks) | Yes (S2 x3; evidence gap) | Partly: "picked-up weapon is the one that attacks" needs an observable |
| S20-05 BUG-097 room order | Logic if selected by `RoomType`; Config/Data if asset reordered. Owner to confirm | Required if code path, impossible | Required (3 runs, start and end room) | No (S2) | Yes |
| S20-06 Editor/data block (105, 095, 072) | Config/Data | Not required | Required (sword 55 + PhysicalDamage; arrow hit registers; Lightning damages) | No (advisory gate) | Yes for 105; 095 and 072 have no stated AC |
| S20-07 Process gate decision | Not testable code (decision) | N/A | N/A | No | No (see below) |
| S20-08 BUG-065 + 086 death | Logic (state machine/event) + Integration (Respawn) | Required, impossible | Required (die 3 times; count emits; Respawn) | No (Should; S2) | Partly ("No slide" needs a measure) |
| S20-09 BUG-092 + 066/070 verify | Logic (verification) | Required for DoT tick count, impossible | Required | No (Should) | No (see below) |
| S20-10 BUG-115 runtime copy | Logic | Required, impossible | Required (`git status` clean after Play Mode) | No (Should; S2) | Yes |
| S20-11 One-liners (104, 103, 102, 068) | Mixed: 104 and 103 Logic, 068 Logic, 102 UI | Required for 103 and 068, impossible | Required; UI screenshot for 102 | UI screenshot is BLOCKING for 102 (standard level) | Partly (see below) |
| S20-12 Missing-script assets (073/090/112) | Config/Data | Not required | Required (Console, 0 Missing Script) | No (advisory) | Yes |
| S20-F1 Land `feature/add-effect-component` | Integration | Not required (compile check script) | Required (Saturday playtest covers Archer/Mage abilities) | Yes (it rewrites `VitalStatsBase`; invalidates S20-09/10 evidence taken earlier) | Yes (compile 0 `error CS`) |

## Acceptance-criteria findings (shift-left)

- **S20-07**: a decision, not code. Its AC ("Decision recorded in this file") is verifiable as a document edit only. No test type applies. Proposed rewrite for the author to confirm: "Sprint file contains a dated entry choosing one of: pre-push hook installed, or signed drop of TD-048." Not a QA blocker.
- **S20-09**: AC "Recorded in the bug files" is not a testable outcome. Proposed rewrite: "DoT asset applies N ticks at interval T in Play Mode (N, T stated per asset); `Reborn()` seeds HP key before first `GetCurrentStatValue`; result and build recorded in BUG-092/066/070 files." Also note: reading against `feature/add-effect-component` first means this story is effectively dependent on S20-F1, not only S20-01.
- **S20-08**: "No slide" is unmeasured. Proposed: "player position delta is 0 between death-state enter and Respawn (Rigidbody2D velocity = 0)"; "ON_PLAYER_DEATH log count = 1 per death".
- **S20-04**: "picked-up weapon is the one that attacks" proposed rewrite: "after pickup, the damage dealt equals the picked-up weapon's stage damage, and the old weapon object is released".
- **S20-06**: no AC stated for BUG-095 or BUG-072. Proposed: "Arrow hit on a target reduces its HP; Lightning reduces HP of an enemy in radius."
- **S20-03**: AC "Every non-combat room opens" needs the room list (Start, Rest, Shop, Buff, Boss) stated, since Start also depends on the Champion path (BUG-096 note).
- **S20-11**: AC "Each visible in `git diff`" is a code-presence check, not a behaviour check. For 103 add "ranged weapon fires at most once per RecoveryTime". For 102 a screenshot of the confirm panel (title/message/button labels) is required evidence.
- **S20-01**: valid, but it is also the sole manual QA gate and has been carried 21 times. An accepted-risk note is an allowed outcome of the AC; QA records that outcome as "no manual evidence" rather than as a pass.

## Manual QA sessions (estimate)

| # | Session | Covers | Est. |
|---|---|---|---|
| 1 | Baseline playtest: critical-paths through menu to dungeon (before any feature commit) | S20-01, establishes pre-fix failures | 0.3d |
| 2 | Regression after S20-02/03/04 land: Knight spawn, non-combat rooms open, enemy attacks, Mage confirm, pickup | S20-02, 03, 04 | 0.2d |
| 3 | Data/Editor pass: sword damage, Arrow/Lightning hits, room order x3 runs, Missing Script check | S20-05, 06, 12 | 0.2d |
| 4 | Should-have pass: death x3, DoT ticks, `git status` after Play Mode, UI screenshot for 102, 103 fire rate | S20-08, 09, 10, 11 | 0.25d |
| 5 | Post-merge compile + Saturday playtest incl. Archer/Mage abilities | S20-F1 + full regression | 0.1d reserved (sprint), realistically 0.3d |

Total: 5 sessions, about 1.2d of owner Editor time. The sprint's own plan reserves 0.3d + 0.1d for playtests, so sessions 2-4 are not budgeted as QA time.

## Evidence plan (qa.level standard)

- Logic stories (S20-03, 05 if code, 08, 09, 10, 11-103/068): automated tests are the BLOCKING evidence and cannot exist. Recommend one of: (a) record each as "evidence gap, manual playtest substituted" with owner sign-off, or (b) un-defer BUG-084 decision (S20-D6) so one EditMode test can land. QA does not accept a silent pass.
- Integration (S20-01, 04, F1): documented playtest accepted as evidence; sheet must name the build/commit.
- UI (S20-11 BUG-102): retained screenshot of the confirm panel in `production/qa/evidence/`.
- Config/Data (S20-02, 06, 12): smoke check pass, advisory.
- Smoke check must pass before sessions 2-5. It is UNKNOWN now; run `/smoke-check` first.

## Risks to evidence quality

- Working tree is dirty on `origin/feature/add-effect-component` with 14 modified `.cs` files in Character/Base, Stats, Abilities and Player states. These overlap S20-04 (`AbilityHolderBase`), S20-09/10 (`BaseStatsSO`, `StatHandlerBase`). Any playtest on this tree cannot be attributed to a committed fix.
- S20-F1 rewrites `VitalStatsBase`; evidence for S20-09/10 taken before the merge must be repeated after it.
- Branch name is a feature branch, not `sprint-20`; the sprint file assumes work lands on `sprint-20`.

## Concerns (for caller)

See handback list.
