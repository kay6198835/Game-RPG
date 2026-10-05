# Changelog — `tech-debt-register.md`

Change log for [`docs/tech-debt-register.md`](../tech-debt-register.md). The document itself is the **current official version**;
this file records **when** it changed, **what** changed (**from → to**) and **why**. Newest first.

Built 2026-10-05 from `git log --follow` and the per-document trail in `docs/CHANGELOG-DOCS.md`.
Where neither source states a reason, the entry says so instead of guessing. From 2026-10-05 on,
append entries by hand (template in `docs/systems/README.md`).

## 2026-10-05 — Re-synced against code (doc-sync)
- **Commit:** not yet committed (working tree on `origin/feature/synce-doc-and-code`, base HEAD `93ba6d8e`)
- **Changed:** corrected every statement found stale against HEAD `93ba6d8e`. Each item reads *was stale → now says*:
  - **From → To:** TD-023 (`:36`, `LevelManager` singleton) is still listed open — resolved in `0bc36406` (2026-10-02)
  - **From → To:** TD-010 (`:52`) lists `attackDamege` as a preserved typo — renamed in code 2026-09-01 (BUG-095)
  - **From → To:** TD-050 (orphan `.meta`) open → closed: `NegativeReceiverBase.cs.meta` exists since `5b035b73`
  - **From → To:** Header 'Last updated 2026-09-22, 47 items' → '2026-10-05, 53 items'; new TD-052 (orphan `IReasourceReceiver` file) and TD-053 (UIFlow static locator + static event bus)
- **Why:** code changed in sprints 16-17 (`ddcc0a5c`, `5b035b73`, `b7a0af5e`, `9154763f`, `0bc36406`, `3a395fe9`, `d3400ee7`, `40d2c793`) without a matching doc edit
- **Bugs:** BUG-095 filed; Bug #12, Bug #13, BUG-064, TD-023, TD-050 recorded as closed where the document tracked them

## 2026-09-25 — docs(qa): re-verify the bug register against HEAD c0067f4 after the ADR-0005 refactor
- **Commit:** `6ca82f3b` (+7 / −5 lines)
- **Why:** Post-pull re-verification against HEAD `c0067f4` (ADR-0005 refactor + two fix commits) (`docs/CHANGELOG-DOCS.md`)
- **Changed:** `docs/tech-debt-register.md` | **TD-050** (orphan `.meta` from the `NegativeReceiverBase` rename) and **TD-051** (BUG-092 as TD-048's recurrence) added. TD-042, TD-047, TD-049 marked **Closed**; TD-041 marked merged; TD-045 scope reduced. All five keep their original text after `**Original text follows.**`
- **Sections touched:** “Technical Debt Register”
- **From → To:** `` | TD-041 | Code Quality | **NEW 2026-09-11 (= BUG-066).** `EntityVitalStats` indexes `currentStats[statType]` with no key-existence guard. … `` → `` | TD-041 | Code Quality | **MERGED 2026-09-25 (still open).** The ADR-0005 refactor moved the unguarded dictionary into the shared … ``
- **From → To:** `` | TD-045 | Architecture | **NEW 2026-09-22 (= BUG-087).** Player death has no recovery path and no owner. `ON_PLAYER_DEATH` has zero … `` → `` | TD-045 | Architecture | **SCOPE REDUCED 2026-09-25 (still open).** The player now inherits `Reborn()` from the shared … ``

## 2026-09-22 — docs(qa): re-verify the bug register against 2a83469 and the owner review
- **Commit:** `e306c639` (+2 / −0 lines)
- **Why:** (third pass, same day) — Post-fetch re-verification against HEAD `2a83469` (`docs/CHANGELOG-DOCS.md`)
- **Changed:** `docs/tech-debt-register.md` | **TD-048** (no compile gate anywhere — the root cause of BUG-088) and **TD-049** (the three loose ends from `2a83469`) added. Nothing removed
- **Why:** (later same day) — Owner review: three findings corrected or retracted (`docs/CHANGELOG-DOCS.md`)
- **Changed:** `docs/tech-debt-register.md` | TD-046 rewritten as WITHDRAWN/Void with the reason, marked *do not action this entry*
- **Why:** Bug-documentation re-verification pass (`docs/CHANGELOG-DOCS.md`)
- **Changed:** `docs/tech-debt-register.md` | Header date and counts updated (43 → 47 items). TD-014 re-scoped as blocked by TD-044. TD-044 (asmdef/test pipeline), TD-045 (death recovery path), TD-046 (`NegativeReceiver` shared mutable state / callback signature), TD-047 (duplicate player `INegativeReceiver`) added
- **Sections touched:** “Technical Debt Register”
- **From → To:** (nothing) → `` | TD-048 | Process | **NEW 2026-09-22 (root cause of BUG-088).** Nothing in this project compiles the code except a human opening the Unity … ``

## 2026-09-22 — update docs bug
- **Commit:** `56f7ea92` (+8 / −3 lines)
- **Sections touched:** “Technical Debt Register”
- **From → To:** `` Last updated: **2026-09-11** (full documentation/code re-synchronisation; every entry re-checked `` → `` Last updated: **2026-09-22** (bug-documentation re-verification pass; four new entries, TD-014 re-scoped). ``
- **From → To:** `` Total items: 43 | Closed/void this pass: 3 | Corrected this pass: 5 | New this pass: 4 `` → `` Total items: 47 | Closed/void this pass: 0 | Corrected this pass: 1 (TD-014) | New this pass: 4 (TD-044…TD-047) ``
- **From → To:** `` | TD-014 | Test Debt | 0 test files (EditMode or PlayMode) — no regression safety net for any system | All | XL | High | 3 | 2026-05-31 | … `` → `` | TD-014 | Test Debt | 0 test files (EditMode or PlayMode) — no regression safety net for any system. ⚠️ **Re-scoped 2026-09-22: this is … ``
- … 1 more hunks — `git show 56f7ea92 -- docs/tech-debt-register.md`
- **Why:** commit message: “update docs bug”

## 2026-09-11 — docs: full documentation/code re-synchronisation against HEAD 6d6a8e4
- **Commit:** `bbfb3028` (+21 / −11 lines)
- **Why:** Full documentation/code re-synchronisation (`docs/CHANGELOG-DOCS.md`)
- **Changed:** All source code | 194 `.cs` | **This pass was documentation-only by instruction.** Every defect found in code — BUG-063, BUG-065, BUG-066, BUG-064 sub-item 7, the orphan directories, the `AbilityHolder` null dereference — was **documented, not fixed**, and is tracked in `CLAUDE.md`, `docs/tech-debt-register.md` and `production/qa/bugs/`.
- **Sections touched:** “Technical Debt Register”
- **From → To:** `` Last updated: 2026-08-21 (documentation audit + owner decisions C1–C5. Every entry re-verified `` → `` Last updated: **2026-09-11** (full documentation/code re-synchronisation; every entry re-checked ``
- **From → To:** `` | TD-011 | Code Quality | **STILL OPEN — 12 weeks.** `get => ModifiersAmor;` plus a setter that also reads and writes itself → … `` → `` | TD-011 | Code Quality | **VOID 2026-09-11** — the file is gone. `EntityStatsSO.cs` was deleted in `b0512f4` as part of the entity/stat … ``

## 2026-08-21 — docs(adr): amend ADR-0003 to the real budget behaviour, and correct my own bad statistic
- **Commit:** `66b7604c` (+5 / −4 lines)
- **Sections touched:** “Technical Debt Register”
- **From → To:** `` Last updated: 2026-08-21 (documentation audit — every entry re-verified against `Assets/Script/`. `` → `` Last updated: 2026-08-21 (documentation audit + owner decisions C1–C5. Every entry re-verified ``
- **From → To:** (nothing) → `` | TD-039 | Architecture | **NEW 2026-08-21 — ACCEPTED, not scheduled** (owner decision C3: docs follow code for now). … ``
- **Why:** commit message: “docs(adr): amend ADR-0003 to the real budget behaviour, and correct my own bad statistic”

## 2026-08-21 — docs(stats): record the ratified StatModifierGroup shape and the C1 fix
- **Commit:** `610551f3` (+1 / −1 lines)
- **Sections touched:** “Technical Debt Register”
- **From → To:** `` | TD-038 | Data | **NEW 2026-08-20, re-verified 2026-08-21 against `sprint-10` HEAD.** `Stat.modifiers` is `[SerializeField]` although its … `` → `` | TD-038 | Data | **CLOSED 2026-08-21.** `Stat.modifiers` carried `[SerializeField]` against its own doc-comment and ADR-0001, so runtime … ``
- **Why:** commit message: “docs(stats): record the ratified StatModifierGroup shape and the C1 fix”

## 2026-08-21 — docs: correct three audit findings overtaken by sprint-10
- **Commit:** `f5042d67` (+4 / −3 lines)
- **Sections touched:** “Technical Debt Register”
- **From → To:** `` Last updated: 2026-08-20 (documentation audit — every entry re-verified against `Assets/Script/`. `` → `` Last updated: 2026-08-21 (documentation audit — every entry re-verified against `Assets/Script/`. ``
- **From → To:** `` | TD-038 | Data | **NEW 2026-08-20.** `Stat.modifiers` is `[SerializeField]` although its own doc-comment and ADR-0001 both require … `` → `` | TD-038 | Data | **NEW 2026-08-20, re-verified 2026-08-21 against `sprint-10` HEAD.** `Stat.modifiers` is `[SerializeField]` although its … ``
- **Why:** commit message: “docs: correct three audit findings overtaken by sprint-10”

## 2026-08-21 — docs(tech-debt): reconcile the register with source (38 items, 9 closed)
- **Commit:** `e32e134c` (+39 / −31 lines)
- **Sections touched:** “Technical Debt Register”
- **From → To:** `` Last updated: 2026-07-29 (TD-034 / TD-035 added — deferred enemy-count scaling & horde-pathfinding solutions surfaced during the … `` → `` Last updated: 2026-08-20 (documentation audit — every entry re-verified against `Assets/Script/`. ``
- **From → To:** `` | TD-001 | Architecture | `PlayerUserItemState`, `EntityDeathState` extend `MonoBehaviour` instead of base state class — not wired into … `` → `` | TD-001 | Architecture | **PARTIALLY RESOLVED 2026-08-20** — `EntityDeathState` now extends `EntityBasicState` (`EntityDeathState.cs:1`) … ``
- **From → To:** `` | TD-005 | Code Quality | `Physics2D.OverlapCircle` (allocating) in hot paths — runs every frame in `EntityWeaponMelee` and … `` → `` | TD-005 | Code Quality | `Physics2D.OverlapCircle` (allocating) in hot paths — `EntityWeaponMelee.Attack()` (per-attack, = BUG-046) and … ``
- … 5 more hunks — `git show e32e134c -- docs/tech-debt-register.md`
- **Why:** commit message: “docs(tech-debt): reconcile the register with source (38 items, 9 closed)”

## 2026-07-29 — docs(tech-debt): log deferred enemy-scaling & horde-pathfinding solutions
- **Commit:** `46f1ef31` (+4 / −2 lines)
- **Sections touched:** “Technical Debt Register”
- **From → To:** `` Last updated: 2026-07-13 (TD-031 duplicate ID fixed — the ObjectPooling row is now TD-033; TD-031 enemy-spawn row description refreshed … `` → `` Last updated: 2026-07-29 (TD-034 / TD-035 added — deferred enemy-count scaling & horde-pathfinding solutions surfaced during the … ``
- **From → To:** (nothing) → `` | TD-034 | Performance | Enemy-count scaling package for wave rooms (~15 typical, 20 hard ceiling, midcore) — deferred while demo enemy … ``
- **Why:** commit message: “docs(tech-debt): log deferred enemy-scaling & horde-pathfinding solutions”

## 2026-07-11 — docs(enemy-spawn): re-sync GDD/epic/ADR to actual code + renew sprint-05
- **Commit:** `e600eb24` (+3 / −3 lines)
- **Sections touched:** “Technical Debt Register”
- **From → To:** `` Last updated: 2026-07-02 `` → `` Last updated: 2026-07-13 (TD-031 duplicate ID fixed — the ObjectPooling row is now TD-033; TD-031 enemy-spawn row description refreshed … ``
- **From → To:** `` | TD-031 | Architecture | Enemy-spawn prototype diverges from approved GDD/ADR-0002: `RoomModel`/`MapModel`/`EnemyModal` use … `` → `` | TD-031 | Architecture | Enemy-spawn prototype diverges from the 2026-07-08 GDD target, and (updated 2026-07-13) has kept diverging … ``
- **From → To:** `` | TD-031 | Architecture | `ObjectPooling` is slash-VFX-specific with fake singleton (`instance => this`); no generic pool — blocks fixing … `` → `` | TD-033 | Architecture | `ObjectPooling` is slash-VFX-specific with fake singleton (`instance => this`); no generic pool — blocks fixing … ``
- **Why:** commit message: “docs(enemy-spawn): re-sync GDD/epic/ADR to actual code + renew sprint-05”

## 2026-07-09 — docs(enemy-spawn): reverse-sync GDD to prototype code + propagate
- **Commit:** `950ae319` (+2 / −1 lines)
- **Sections touched:** “Technical Debt Register”
- **From → To:** `` | TD-030 | Architecture | Enemy data model split: `EnemySO` not consumed by `Entity` (which uses `EntityData`); `EnemySO` lacks a … `` → `` | TD-030 | Architecture | Enemy data model split: `EnemySO` not consumed by `Entity` (which uses `EntityData`); `EnemySO` lacks a … ``
- **Why:** commit message: “docs(enemy-spawn): reverse-sync GDD to prototype code + propagate”

## 2026-07-03 — docs(debt): add TD-021..TD-032 from room-system audit + codebase scan
- **Commit:** `b098f6f4` (+14 / −2 lines)
- **Sections touched:** “Technical Debt Register”
- **From → To:** `` Last updated: 2026-05-31 `` → `` Last updated: 2026-07-02 ``
- **From → To:** (nothing) → `` | TD-021 | Code Quality | Player damage endpoint `NegativeReciver.TakeDamage()` throws NotImplementedException — enemy hits produce … ``
- **From → To:** (nothing) → `` | TD-025 | Architecture | `RoomType` enum never read at runtime — start/end rooms forced by list position `room[0]`/`room[last]`; … ``
- … 2 more hunks — `git show b098f6f4 -- docs/tech-debt-register.md`
- **Why:** commit message: “docs(debt): add TD-021..TD-032 from room-system audit + codebase scan”

## 2026-05-31 — chore: add tech debt register from monthly review scan (2026-05-31)
- **Commit:** `6addf4d9` (+26 / −0 lines)
- **Changed (from → to):** document created (+26 lines)
- **Why:** commit message: “chore: add tech debt register from monthly review scan (2026-05-31)”

## 2026-10-05 — Doc sync against HEAD `93ba6d8e` (38 commits, sprints 16-17) + per-system layout (no matching commit on that date)
- **Commit:** none found for this date in `git log --follow` (likely committed on a later date or as part of a merge)
- **Changed (from → to):** `tech-debt-register.md`, `VERSION.md`) kept their paths and got `changelog/<doc>.CHANGELOG.md`
- **Why:** see that section of `docs/CHANGELOG-DOCS.md`
