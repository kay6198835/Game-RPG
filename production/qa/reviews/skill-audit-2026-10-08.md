# Skill & Tooling Audit — 2026-10-08

> Phase R1 of `production/review-flow.md`. Branch `sprint-17`, HEAD `1b24ef7b`. Review-only: no file under
> `.claude/` was changed. Every finding below was produced by a scripted check over `.claude/` or by
> reading the named file; commands are summarised in §7 so the audit can be re-run.

## 1. Inventory

| Item | Count | Note |
|---|---|---|
| Skills (`.claude/skills/*/SKILL.md`) | **79** | `docs/skill-reference.md` says 80 (re-synced 2026-09-11) — off by one |
| Agents (`.claude/agents/*.md`) | 26 | Unchanged since 2026-08-21 |
| Rules (`.claude/rules/*.md`) | 13 | |
| Hooks (`.claude/hooks/*`) | 13 | 10 wired in `settings.json` |
| Skills pre-approved in `settings.json` | 9 | The recurring PM routines + code-review / bug-triage / sprint tools |
| Largest skills | `ux-design` 975 lines, `design-system` 841, `setup-engine` 715, `architecture-review` 639, `review-all-gdds` 628 | |

All 79 skills have a `SKILL.md` and an `allowed-tools` line. Only `ui-screen` ships supporting files.

## 2. Verdict

**CONCERNS.** The project-specific skills that actually run (daily standup, weekly wrap-up / kickoff,
monthly module audit, doc-sync, bug-triage, code-review) are mature and produce high-quality,
evidence-cited output. The weakness is the **seams between them** and a large body of generic
template skills that still point at files this project never created. Three seams have already
produced wrong project state (§3, F-1 to F-3).

## 3. Findings — process seams (highest impact)

| ID | Sev | Finding | Evidence | Effect already observed |
|---|---|---|---|---|
| F-1 | S2 | **No single bug-ID authority.** `/doc-sync` wrote BUG-093/094/095 into `CLAUDE.md` for three defects (`RangeWeapon.nextFireTime`, `PlayerState.Enter()` log, `attackDamege` rename) while `production/qa/bugs/BUG-093..095.md` already described three different defects (ability aim, `RangeWeapon` root rotation, `Arrow.prefab` collider). `/bug-report` additionally prescribes a 4-digit `BUG-[NNNN]` format; the project uses 3 digits | `CLAUDE.md` Known Bugs rows BUG-093..095 vs the three bug files' titles; `bug-report/SKILL.md:35` | Two registers disagree on what BUG-093..095 are. Filed as **BUG-101** |
| F-2 | S2 | **No step verifies documentation claims against code.** `/doc-sync` writes docs from git history; nothing re-reads source to confirm a claimed fix still exists | BUG-099 (doc-sync `7d1b5c79` recorded reverted BUG-096 fix as done) | An open S1 was hidden from planning for a day |
| F-3 | S2 | **No regression sweep for recent fixes.** `/weekly-wrapup` reviews new diffs but does not re-check fixes closed in the last N days | BUG-096: fix `40d2c793` reverted by `ac13ee4` the same day | Caught 2 days later by luck of the diff touching the same file |
| F-4 | S2 | **Runtime verification has no owner skill that actually runs.** `/smoke-check` needs `tests/smoke/critical-paths.md` (absent) and a test suite (0 tests, 0 `.asmdef` — BUG-084). Latest file in `production/qa/playtests/` is **2026-06-12** | `ls production/qa/playtests` | Every verdict for ~4 months is static analysis; S17-04 smoke carried 11 times |
| F-5 | S3 | **No compile gate.** Nothing in hooks compiles C#; `validate-commit.sh` checks GDD sections, JSON, Mermaid, hardcoded numbers, TODO owners only | `.claude/hooks/validate-commit.sh` | BUG-088 and BUG-092 compile breaks committed in 4 days (TD-048, TD-051) |
| F-6 | S3 | **Cadence documentation is stale.** `production/review-schedule.md` prescribes Mon/Fri runs and `/weekly-sprint`; the routines that run are daily 10:00 standup, Sat 22:00 wrap-up, Sun 22:00 kickoff, monthly first-Monday audit. Two slots were missed recently (Sat 2026-10-03 wrap-up, September monthly audit) with no alert | `review-schedule.md`; `bug-triage-2026-10-05.md` header; `module-health-2026-10.md` header | Review windows silently doubled |
| F-7 | S3 | **No orchestrated "full review" entry point.** `/project-stage-detect`, `/milestone-review`, `/gate-check`, `/module-quality-audit` each cover a slice | — | Addressed by `production/review-flow.md` (design only) |

## 4. Findings — broken references

### 4.1 Missing agents (6 skills cannot run as written)

Re-verified: **13** agent names referenced via `subagent_type` have no file in `.claude/agents/`.
Unchanged since first recorded in `docs/skill-reference.md` on 2026-08-21.

| Skill | Missing agents |
|---|---|
| `team-audio` | audio-director |
| `team-level` | accessibility-specialist, narrative-director, world-builder |
| `team-live-ops` | analytics-engineer, community-manager, economy-designer, live-ops-designer, narrative-director, writer |
| `team-narrative` | localization-lead, narrative-director, world-builder, writer |
| `team-release` | analytics-engineer, community-manager, devops-engineer, network-programmer, security-engineer |
| `team-ui` | accessibility-specialist |

`team-combat`, `team-qa`, `team-polish`, `hotfix` resolve fully. For a solo combat-demo project,
`team-live-ops`, `team-narrative` and `team-audio` are out of scope; `team-ui` and `team-release`
matter later.

### 4.2 Required inputs that do not exist

A scripted check found ~50 repo paths named in skills that do not exist. Most are template examples
(`production/epics/combat/story-001.md`, `docs/engine-reference/godot/...`) and harmless. These are
**required inputs** — the skill silently degrades without them:

| Missing path | Skill(s) | Consequence |
|---|---|---|
| `design/registry/entities.yaml` | `consistency-check` | Core input of the skill. The project's registry is `docs/registry/architecture.yaml` (architecture stances, not entities). A run either fails or checks nothing |
| `docs/architecture/architecture.md`, `control-manifest.md`, `tr-registry.yaml`, `architecture-traceability.md` | `architecture-review`, `create-control-manifest`, `create-epics`, `create-stories`, `story-readiness`, `dev-story` | The story pipeline (`create-epics → create-stories → story-readiness → dev-story → story-done`) has no master architecture to read; `production/epics/` holds only `.gitkeep` |
| `tests/smoke/critical-paths.md`, `tests/regression-suite.md` | `smoke-check`, `regression-suite` | See F-4 |
| `production/stage.txt`, `production/project-stage-report.md` | `project-stage-detect`, `gate-check`, `help`, `start` | Stage is inferred each run, never recorded |
| `production/session-state/active.md` | `session-start.sh`, several skills | Session hand-off file never written; `production/session-logs/session-log.md` (377 KB) is used instead |
| `design/gdd/game-pillars.md`, `design/player-journey.md` | `review-all-gdds`, `ux-design`, `design-review` | Pillar-drift check has no pillar source other than `game-concept.md` |

### 4.3 Template residue

- **14 skills** reference `src/` as the code root (`adopt`, `content-audit`, `dev-story`, `gate-check`,
  `help`, `localize`, `onboard`, `project-stage-detect`, `reverse-document`, `security-audit`,
  `setup-engine`, `sprint-status`, `start`, `story-done`). Unity code lives in `Assets/Script/`; an
  unadapted run reports "no source code".
- **20 skills** carry Godot / Unreal branches. Harmless but inflates context; the engine is pinned.
- Dangling skill references: `/command`, `/schedule`, `/skill-name`, `/validate` appear as slash
  references and resolve to no project skill (`/schedule` is a harness skill, the rest are
  placeholders).

### 4.4 Hooks

| Hook | Defect |
|---|---|
| `session-start.sh:29-34` | Bug counter sums `production/qa/bugs` and `production` recursively, so each bug file counts twice and closed bugs count as open (banner says 78 for ~39 files). Known since 2026-09-25, unfixed |
| `validate-commit.sh` | Hardcoded-value scan globs `Assets/Script/(Character\|Weapons\|Skill_Ability)/`; `Skill_Ability` moved to `System/` on 2026-09-03, and `System/Abilities/` (the live ability code) is not scanned at all |
| `validate-commit.sh` | JSON check targets `Assets/ScriptableObjects/*.json`; the project's JSON lives in `Assets/Data/Json/Room/` — room data is never validated |
| `validate-skill-change.sh` | Advises `/skill-test static`; no record that it was ever run (no skill-test reports in repo) |

### 4.5 Rules drift (spotted while reading, not a full sweep)

- `.claude/rules/weapon-skill-code.md` still tells authors that `IAbilityServices` exposes
  `Pool, Stats, ResourceReceiver, Vital, NegativeReceiver`; since ADR-0005 (2026-09-25) it is `Pool` only.
- `.claude/rules/map-code.md` still lists Bug #13 (start-room teleport) as open; fixed `9154763f`.
- `.claude/rules/manager-event-code.md` "UIManager Completion" section describes a stub deleted in `cb0de496`.

## 5. Overlap and redundancy

| Pair / group | Observation | Recommendation |
|---|---|---|
| `/weekly-sprint` vs `/weekly-kickoff` + `/weekly-wrapup` | Reference itself calls `/weekly-sprint` legacy | Mark deprecated in its description |
| `/bug-triage` vs `/weekly-wrapup` | Wrap-up embeds triage; both write `bug-triage-*.md` | Keep; wrap-up is the caller |
| `/module-quality-audit` vs `/consistency-check` + `/content-audit` + `/tech-debt` | Audit already embeds them inline (by design) | Keep; fix `consistency-check` input first (4.2) |
| `/project-stage-detect` vs `/adopt` vs `/help` vs `/start` | Four orientation skills; all assume `src/` | Adapt one (`project-stage-detect`), leave others |
| Concept / pre-production skills (`brainstorm`, `art-bible`, `map-systems`, `prototype`, `create-architecture`) | Not useful in Production phase except `map-systems` for index refresh | No action |

## 6. What works well (keep)

- Recurring PM skills cite `file:line` at a named HEAD and separate "static" from "verified".
- `/module-quality-audit` change-impact table (touched Approved/Designed modules) is the best
  design-drift detector in the repo.
- `docs/CHANGELOG-DOCS.md` cause-first logging and the "snapshots are immutable" convention.
- Mermaid linter wired into the commit hook (2026-10-05).

## 7. Re-run recipe

All checks are read-only one-liners from the repo root:

- Agents: extract every `subagent_type` value from `.claude/skills/**`, diff against `ls .claude/agents`.
- Paths: extract `(docs|design|production|tests|.claude)/…(.md|.yaml|.txt)` tokens from skills, test `-e`.
- `src/` residue: `grep -rlE '(^|[ \`(])src/' .claude/skills`.
- Bug-ID audit: for each `BUG-NNN` in `CLAUDE.md`, compare the row text with the `# Title` of the bug file.

## 8. Recommendations (owner decision; nothing actioned)

See `production/review-flow.md` §9 for the ranked list. Highest value first: F-1 (ID rule), F-2/F-3
(claim + regression sweeps), F-4 (critical-path file + one real playtest), §4.2 registry for
`consistency-check`, §4.4 hook fixes.
