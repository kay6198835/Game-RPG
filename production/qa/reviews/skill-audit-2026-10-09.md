# Skill & Tooling Audit — 2026-10-09 (post CCGS v1.1.2 install)

> Phase R1 of `production/review-flow.md` (v2). Branch `origin/feature/update-bug-document`, HEAD
> `221d54be`. Review-only: no file under `.claude/`, no hook, no setting and no `.cs` file was changed.
> Re-run because `.claude/` changed after the previous audit (`skill-audit-2026-10-08.md`, HEAD
> `1b24ef7b`): CCGS v1.1.2 skills/agents (`959268bd`), hooks + settings installed (`7c90520b`), vendored
> folder dissolved (`d64047a8`, `ec44e9be`). 257 files under `.claude/` changed, +34,020 / −6,045 lines.

## 1. Inventory

| Item | 2026-10-08 | 2026-10-09 | Note |
|---|---|---|---|
| Skills | 79 | **81** | +`settings`, +`vertical-slice`. 74 upstream + 7 project-own |
| Agents | 26 | **39** | 13 added; Godot/Unreal specialists skipped |
| Rules | 13 | **16** | +`skill-authoring.md`, +`agent-memory.md`, +`mermaid-diagrams.md` counted; local rules now use `paths:` |
| Hooks | 13 | **15 files** | `yaml-helper.sh`, `log-instructions.sh` new; installed in `7c90520b` |
| Skills calling `yaml-helper.sh resolve_config` | — | 68 | Helper is now installed, so config resolution works |
| Config | `review-mode.txt` | `project.yaml` (finalized) | `stage: Production`, `rigor: standard`, `review_mode: lean` |
| Scheduled routines | 4 | 4 | `pm-daily-standup`, `pm-weekly-wrapup`, `pm-weekly-kickoff`, `pm-monthly-module-audit` |

## 2. Verdict

**CONCERNS (improved).** The framework-level defects found yesterday are resolved by the upgrade:
0 dangling agents, code-root resolution replaces the `src/` assumption, gating skills now emit
`NOT ASSESSED` instead of a false pass, and `design/registry/entities.yaml` exists. **None of the four
project-process seams (F-1…F-4) is closed** — they live in the 7 project-own skills, which the upgrade
did not touch and which have not been edited since June–September.

## 3. Status of the 2026-10-08 findings

| ID | Finding | Status 2026-10-09 | Evidence |
|---|---|---|---|
| F-1 | No single bug-ID authority | ❌ Open | `bug-report/SKILL.md:44,170-172` still prescribe `BUG-[NNNN]` (4 digits); no project-own skill contains a "next ID = max + 1" rule (grep `max.*\+ ?1\|next (free )?id` = 0 hits in `daily-standup`, `weekly-*`, `doc-sync`, `module-quality-audit`, `bug-triage`). `CLAUDE.md` BUG-093/094/095 rows still mismatch the bug files (BUG-101 open) |
| F-2 | No doc-claim verification step | ❌ Open | `doc-sync/SKILL.md` unchanged since 2026-10-05; no claim sweep |
| F-3 | No regression sweep of recent fixes | ❌ Open | `weekly-wrapup/SKILL.md` (92 lines, last edit 2026-06-17) has 0 occurrences of "regress" |
| F-4 | Runtime verification has no runnable owner | ❌ Open | `tests/smoke/` absent; 0 `.asmdef`; last playtest file `playtest-2026-06-12-*` (**119 days**) |
| F-5 | No compile gate | ⚠️ Partial | Upstream `validate-commit.sh` (39 KB) is engine-aware but does not compile C#. Still manual |
| F-6 | Cadence documentation stale | ❌ Open, widened — see §4 N-3 |
| F-7 | No orchestrated full-review entry point | ⚠️ Partial | `review-flow.md` exists (design); no `/full-review` skill |
| §4.1 | 13 missing agents | ✅ Closed | Scripted `subagent_type` check: 0 missing |
| §4.2 | `design/registry/entities.yaml` missing | ✅ Closed (empty) | File exists with header only — `/consistency-check` now runs but has no entities to compare |
| §4.2 | `production/review-mode.txt` | ✅ Replaced by `project.yaml`; legacy `/weekly-sprint` still reads it (`weekly-sprint/SKILL.md:8,42`) and falls back silently |
| §4.3 | 14 skills assume `src/` | ✅ Closed via `code-root-resolution.md` |
| §4.4 | `session-start.sh` double-counts bugs | ⚠️ Partial — see N-1 |
| §4.4 | `validate-commit.sh` stale gameplay glob | ✅ Replaced by upstream engine-aware hook (Mermaid lint re-applied) |
| §4.5 | Rules drift (`weapon-skill-code.md` IAbilityServices, `map-code.md` Bug #13, `manager-event-code.md` UIManager) | ❌ Open — rule bodies unchanged; only frontmatter converted |

## 4. New findings

| ID | Sev | Finding | Evidence | Effect |
|---|---|---|---|---|
| N-1 | S3 | Session banner `Open bugs: 41` counts **files**, not open bugs. The rewritten hook no longer double-counts, but still has no status filter | `session-start.sh:164-172` (`find … -name "BUG-*.md" \| wc -l`). Actual register: 41 files = 19 open, 4 partial, 1 accepted, 2 fixed-pending-runtime, 15 closed | Banner overstates open work ≈1.7× |
| N-2 | S3 | Two bug files have no `**Status**` line in the standard position, so any status-parsing tool skips them | `BUG-098.md`, `BUG-099.md` — first bold field is `**System**` | Status-based counters (N-1 fix, `/bug-triage`) will mis-count |
| N-3 | S3 | Documented cadence ≠ actual cron. `pm-daily-standup` fires **Mon–Fri 02:00** (`0 2 * * 1-5`); `review-flow.md` §7 and `docs/skill-reference.md` say 10:00. `pm-monthly-module-audit` fires **every Monday** and relies on an in-prompt first-Monday guard | `list_scheduled_tasks` output 2026-10-09 | A missed guard runs a full monthly audit weekly; docs mislead on when state is refreshed |
| N-4 | S3 | `pm-weekly-wrapup` routine still invokes the legacy `/weekly-sprint` (2 references), which reads the deleted `production/review-mode.txt` | `~/.claude/scheduled-tasks/pm-weekly-wrapup/SKILL.md`; `weekly-sprint/SKILL.md:8` | Saturday backbone runs on a deprecated skill with a missing input; works only through its fallback |
| N-5 | S4 | Rule path globs that match nothing: `prototype-code.md` (`prototypes/**` — only `.meta` + README), `shader-vfx-code.md` (no shader assets), `test-standards.md` (`Assets/Tests/**`, BUG-084) | `git ls-files ':(glob)<pattern>'` per rule | Harmless today; `test-standards.md` will not load when tests are first written outside `Assets/Tests/` |
| N-6 | S4 | Required-input paths still absent (skills degrade to `NOT ASSESSED`): `design/game-brief.md` (74 refs), `production/session-state/active.md` (47), `docs/architecture/control-manifest.md` (44), `production/stage.txt` (32, superseded by `project.yaml`), `docs/architecture/architecture.md` (18), `design/gdd/game-pillars.md`, `design/player-journey.md`, `tests/regression-suite.md`, `tests/smoke/critical-paths.md` | Scripted path extraction over `.claude/skills/**` | The story pipeline (`create-epics → … → story-done`) and pillar checks cannot reach a verdict |
| N-7 | S4 | Project-own skills are the oldest code in `.claude/`: `weekly-sprint` 2026-06-15, `weekly-kickoff`/`weekly-wrapup` 2026-06-17, `daily-standup` 2026-07-09, `ui-screen` 2026-08-21, `module-quality-audit` 2026-09-11, `doc-sync` 2026-10-05. They do not use `resolve_config`, `NOT ASSESSED`, or the evidence tiers the upgrade introduced | `git log -1 -- .claude/skills/<s>` | The routines that actually run are now less rigorous than the template skills they sit beside |

## 5. Skill-to-phase map (all 81 skills)

How each skill participates in the full review flow. "Core" = run in every full review; "Conditional" =
run when the trigger holds; "Remediation" = consumes review output, not part of review; "Out of scope" =
not useful for this project in the Production phase.

| Phase | Core | Conditional (trigger) |
|---|---|---|
| R0 Snapshot | — (git via `rtk proxy`) | `/sprint-status` (sprint context) |
| R1 Tooling | `/skill-test audit` | `/skill-test static <skill>` (skill edited), `/skill-improve` (score dropped), `/settings` (config changed) |
| R2 Build | — (Unity Editor, manual) | — |
| R3 Code | `/code-review`, `/bug-triage` | `/bug-report` (new defect), `/tech-debt` (monthly), `/security-audit` (save/login code touched), `/perf-profile` (hot path touched) |
| R4 Runtime | `/smoke-check`, `/playtest-report` | `/test-setup` (once, after BUG-084), `/qa-plan` (sprint start), `/regression-suite` (fixed bugs > tests), `/test-evidence-review`, `/soak-test` (pre-demo), `/test-helpers`, `/test-flakiness` (only once CI exists), `/team-qa` (milestone) |
| R5 Design | `/module-quality-audit` | `/consistency-check` (Approved module code changed), `/architecture-review` (ADR Proposed-but-implemented or amended), `/content-audit` (milestone counts), `/design-review` (GDD edited), `/review-all-gdds` (≥2 GDDs changed), `/propagate-design-change` (GDD changed after ADR), `/balance-check` (stat/drop data changed), `/asset-audit` (binary growth), `/ux-review` (UIFlow screen added) |
| R6 Docs | claim sweep (manual, §4.6 of the flow), then `/doc-sync` | `/reverse-document` (live system with no GDD) |
| R7 Verdict | `/milestone-review`, `/scope-check` | `/gate-check` (phase transition asked), `/project-stage-detect`, `/retrospective` (sprint end), `/estimate` (sizing ranked actions) |
| R8 Record | `/sprint-plan update` | `/changelog` |
| Routines (carry phases) | `/daily-standup`, `/weekly-wrapup`, `/weekly-kickoff` | `/weekly-sprint` (legacy, still called by `pm-weekly-wrapup`) |

**Remediation (19):** `/architecture-decision`, `/create-architecture`, `/create-control-manifest`,
`/create-epics`, `/create-stories`, `/story-readiness`, `/dev-story`, `/story-done`, `/hotfix`,
`/quick-design`, `/design-system`, `/map-systems` (index refresh), `/ux-design`, `/ui-screen`,
`/team-combat`, `/team-ui`, `/team-polish`, `/team-level`, `/localize` (hardcoded strings found by R3).

**Out of scope now (19):** `/brainstorm`, `/prototype`, `/vertical-slice`, `/art-bible`, `/asset-spec`,
`/setup-engine` (dangerous: `refresh` rewrites the filtered Unity reference), `/start`, `/help`,
`/onboard`, `/adopt`, `/launch-checklist`, `/release-checklist`, `/day-one-patch`, `/patch-notes`,
`/team-audio`, `/team-narrative`, `/team-live-ops`, `/team-release`, `/skill-improve` outside R1.

Coverage: every phase has at least one core skill except **R2** (no compile skill — TD-048) and the
**R6 claim sweep** (no skill — F-2). These two gaps are why the flow keeps manual steps.

## 6. Recommendations (owner decision; nothing actioned)

Ranked by how much wrong project state each prevents:

1. **Patch `weekly-wrapup`** with the R3.2 regression sweep (F-3) and the ID rule (F-1). It is the
   Saturday backbone and the only routine that reads every diff.
2. **Patch `doc-sync`** to run the R6 claim sweep first and feed `STALE`/`FALSE` rows into its edits (F-2).
3. **Rewrite `pm-weekly-wrapup`'s prompt** to call `/weekly-wrapup` directly, then retire `/weekly-sprint` (N-4).
4. **Fix the banner** to count by `**Status**` and normalise BUG-098/099 headers (N-1, N-2).
5. **Write `tests/smoke/critical-paths.md`** (the S17-04 list from `review-flow.md` R4) so `/smoke-check`
   has an input even before BUG-084 is solved (F-4).
6. Align cadence docs with cron, or cron with docs (N-3).
7. Port the project-own skills onto `resolve_config` + `NOT ASSESSED` (N-7).
8. Correct the three stale rule bodies (§4.5).

## 7. Re-run recipe

- Agents: `grep -rhoE 'subagent_type[":= ]+[a-z0-9-]+' .claude/skills`, test each `.claude/agents/<name>.md`.
- Skill refs: extract `/name` tokens from `SKILL.md`, test `.claude/skills/<name>/`.
- Paths: extract `(docs|design|production|tests|.claude)/….(md|yaml|txt|json)` tokens, test `-e`.
- Rule globs: for each `paths:` entry, `git ls-files ':(glob)<pattern>' | wc -l` (plain `git ls-files
  <pattern>` does not expand `**` and gives false zeros).
- Banner: compare `session-start.sh` count with a status-parsed count of `production/qa/bugs/BUG-*.md`.
- Cadence: `mcp__scheduled-tasks__list_scheduled_tasks` cron vs `review-flow.md` §7.
- Note: `session-start.sh` takes >120 s when run outside a session; read its logic instead of running it.
