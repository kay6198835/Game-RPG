# Studio Skill Reference

> Quick-lookup guide to all Claude Code skills available in this project, grouped by
> development phase. Generated 2026-06-08 from `.claude/skills/`.
>
> Invoke any skill with `/skill-name [argument]`. See `production/review-schedule.md`
> for which skills run on a recurring cadence (weekly/monthly).

---

## 1. Onboarding & Orientation

| Skill | Purpose | When to use |
|---|---|---|
| `/start` | Asks where you are, then routes you to the right workflow — no assumptions | First time in the project, unsure where to begin |
| `/onboard` | Generates a contextual onboarding doc for a new contributor or agent, scoped to a role/area | A new team member or agent joins |
| `/project-stage-detect` | Analyzes project state, detects current stage, identifies gaps, recommends next steps | "Where are we in development", "full project audit" |
| `/adopt` | Audits existing artifacts for template format compliance (deeper than stage-detect — checks if what exists actually *works* with the template's skills) | Joining an in-progress project, or upgrading template version |
| `/help` | Analyzes completed work + your question, suggests the next concrete action | Stuck, unsure what to do next |

## 2. Concept & Design (Pre-production)

| Skill | Purpose | When to use |
|---|---|---|
| `/brainstorm` | Guided ideation from zero idea to a structured game concept document | Starting a new concept |
| `/map-systems` | Decomposes a concept into systems, maps dependencies, prioritizes design order | After brainstorm, before detailed GDDs |
| `/art-bible` | Section-by-section Art Bible authoring — the visual identity spec that gates asset production | After brainstorm approval, before map-systems/GDDs |
| `/design-system` | Section-by-section GDD authoring for one system, following the required 8-section format | Writing a detailed design doc for a specific system |
| `/quick-design` | Lightweight design spec for small changes (tuning, balance) — skips full GDD authoring | Small change that doesn't warrant a full GDD |
| `/design-review` | Reviews a GDD for completeness, internal consistency, implementability | Before handing a GDD to programmers |
| `/review-all-gdds` | Cross-reviews ALL GDDs simultaneously — finds contradictions, pillar drift (expensive — monthly only) | After all MVP GDDs are written, before architecture |
| `/consistency-check` | Scans GDDs against the entity registry for cross-document conflicts (stats, formulas, values) | After code/design changes that could cause drift |
| `/propagate-design-change` | When a GDD changes, scans ADRs/traceability index for stale decisions and produces an impact report | After a significant GDD revision |
| `/content-audit` | Compares GDD-specified content counts vs. what's actually implemented | Checking content progress (enemy count, rooms, items, etc.) |
| `/reverse-document` | Generates design/architecture docs backwards from existing implementation | Code exists but planning docs are missing (used to create this project's GDDs) |
| `/balance-check` | Analyzes balance data/formulas for outliers, dominant strategies, economy imbalance | After modifying any balance-related data |

## 3. Architecture

| Skill | Purpose | When to use |
|---|---|---|
| `/create-architecture` | Authors the master architecture document from all GDDs + engine reference | Before writing code, after GDDs are approved |
| `/architecture-decision` | Creates one ADR (Architecture Decision Record) documenting a significant technical decision | Every major technical choice (this project currently has **0 ADRs** — flagged gap) |
| `/architecture-review` | Builds a traceability matrix mapping GDD requirements to ADRs, finds coverage gaps/conflicts | Monthly review (Part 2) |
| `/create-control-manifest` | Produces a flat, actionable rules sheet for programmers extracted from ADRs | After architecture is complete |

## 4. Production Planning

| Skill | Purpose | When to use |
|---|---|---|
| `/create-epics` | Translates approved GDDs + architecture into epics (one per architectural module) | After architecture is approved |
| `/create-stories` | Breaks one epic into implementable story files | After `/create-epics`, per epic |
| `/story-readiness` | Validates a story is implementation-ready → READY / NEEDS WORK / BLOCKED | Before starting work on a story |
| `/sprint-plan [new\|update\|status]` | Creates/updates a sprint plan based on milestone, capacity, and backlog | Start of sprint, or when re-planning is needed |
| `/sprint-status` | Fast situational snapshot of the current sprint — burndown, risks | Anytime mid-sprint, especially Monday kickoff |
| `/estimate` | Estimates task effort from complexity, dependencies, velocity, risk | Need an effort estimate for new work |
| `/scope-check [name]` | Compares current scope vs. original plan, flags creep, quantifies bloat | Weekly kickoff, mid-sprint when new work appears |

## 5. Implementation

| Skill | Purpose | When to use |
|---|---|---|
| `/dev-story [story-file]` | Reads a story, loads full context (GDD + ADR + manifest), implements code + test | Core implementation skill — after `/story-readiness` |
| `/story-done [story-file]` | End-of-story review: verifies acceptance criteria, prompts code review, updates status | When a story is believed complete |
| `/code-review [files]` | Architectural + quality review: coding standards, SOLID, testability, performance | After code is written, before merge |
| `/prototype` | Rapid prototyping workflow — relaxed standards, throwaway code | Validating a concept/mechanic quickly in pre-production |

## 6. QA & Testing

| Skill | Purpose | When to use |
|---|---|---|
| `/qa-plan [sprint\|feature]` | Generates a QA test plan: classifies stories by type, defines automated/manual test scope | Before a sprint begins (this project currently has **no QA plan**) |
| `/team-qa [sprint]` | Orchestrates the full QA cycle: strategy → test plan → test cases → smoke check → manual QA → sign-off | End of sprint, once there is implemented code to test (cannot run meaningfully on Day 1 of an empty sprint) |
| `/smoke-check [sprint]` | Runs the critical-path smoke test gate before QA hand-off — PASS/FAIL | After stories are implemented, before manual QA |
| `/test-setup` | Scaffolds the test framework + CI/CD for the project's engine (run once) | Technical Setup phase, before first sprint (this project currently has **zero test files** — TD-014) |
| `/test-helpers` | Generates engine-specific test helper libraries (assertions, factories, mocks) | After `/test-setup`, to reduce boilerplate |
| `/test-evidence-review` | Quality review of test files/manual evidence — ADEQUATE / INCOMPLETE / MISSING | Before QA sign-off |
| `/regression-suite` | Maps test coverage to GDD critical paths, flags fixed bugs without regression tests | After a bug fix, or before a release gate |
| `/test-flakiness` | Detects non-deterministic tests from CI run history | Polish phase, after multiple CI runs |
| `/bug-report` | Creates a structured bug report with repro steps and severity assessment | When a new bug is found |
| `/bug-triage` | Re-evaluates all open bugs, re-prioritizes, assigns to sprints, surfaces systemic trends | Sprint start, or when bug backlog grows |
| `/playtest-report` | Generates/standardizes a structured playtest report | After each playtest session (Friday wrap-up) |

## 7. Asset & UX/UI

| Skill | Purpose | When to use |
|---|---|---|
| `/asset-spec` | Generates per-asset visual specs + AI generation prompts | After art bible + GDD/level design approval, before production |
| `/asset-audit` | Audits assets for naming, size budget, format, orphaned references | Periodic asset pipeline quality check |
| `/ux-design` | Section-by-section UX spec authoring for a screen/flow/HUD | Designing new UI/UX |
| `/ux-review` | Validates a UX spec/HUD — APPROVED / NEEDS REVISION / MAJOR REVISION | Before implementing UI |

## 8. Performance & Tech Debt

| Skill | Purpose | When to use |
|---|---|---|
| `/perf-profile` | Structured performance profiling — bottlenecks vs. budgets, optimization recs | Monthly review (Part 3) |
| `/tech-debt` | Scans, categorizes, prioritizes technical debt; maintains the debt register | Monthly review (Part 2) — register currently has 20 items |
| `/security-audit` | Audits vulnerabilities: save tampering, cheat vectors, network exploits, data exposure | Before any public release or multiplayer launch |
| `/soak-test` | Generates a protocol for extended play sessions — leaks, fatigue effects, edge cases | Polish/Release phase |

## 9. Recurring Reviews & Process

| Skill | Purpose | When to use |
|---|---|---|
| `/milestone-review` | Reviews feature completeness, quality metrics, risk, go/no-go verdict | Milestone checkpoints (monthly review Part 4) |
| `/gate-check [phase]` | Validates readiness to advance phases (e.g., Production → Polish) — PASS/CONCERNS/FAIL | "Are we ready to move to X" |
| `/retrospective` | Generates a sprint/milestone retrospective — velocity, blockers, patterns | End of sprint/milestone |
| `/doc-sync` | Analyzes git log + code, updates CLAUDE.md (Layout, Known Bugs, Checklist, Events) | "Update docs", "sync project documentation" |
| `/changelog` | Auto-generates a changelog from commits, sprint data, and design docs | Need to summarize change history |

## 10. Release & Live-Ops

| Skill | Purpose | When to use |
|---|---|---|
| `/release-checklist` | Pre-release validation: build verification, certification, store metadata | Before a release |
| `/launch-checklist` | Comprehensive launch readiness across every department, with go/no-go sign-offs | Before public launch |
| `/day-one-patch` | Plans + QA-gates a day-one patch as a mini-sprint with rollback plan | Right after gold master / launch |
| `/hotfix` | Emergency fix workflow with full audit trail, bypassing normal sprint process | Critical issue needs immediate fix |
| `/patch-notes` | Generates player-facing patch notes from git history + internal changelogs | Releasing an update |
| `/localize` | Full localization pipeline: string extraction, translation, cultural review, RTL/VO | Preparing for multiple languages |

## 11. Team Orchestration (multi-agent, parallel coordination)

| Skill | Coordinates |
|---|---|
| `/team-combat` | game-designer + gameplay-programmer + ai-programmer + technical-artist + sound-designer + qa-tester |
| `/team-level` | level-designer + narrative-director + world-builder + art-director + systems-designer + qa-tester |
| `/team-narrative` | narrative-director + writer + world-builder + level-designer |
| `/team-audio` | audio-director + sound-designer + technical-artist + gameplay-programmer |
| `/team-ui` | Full UX/UI pipeline: spec → visual design → implementation → review → polish |
| `/team-qa` | qa-lead + qa-tester (see section 6) |
| `/team-polish` | performance-analyst + technical-artist + sound-designer + qa-tester |
| `/team-release` | release-manager + qa-lead + devops-engineer + producer |
| `/team-live-ops` | live-ops-designer + economy-designer + analytics-engineer + community-manager + writer + narrative-director |

## 12. Engine Setup & Skill Meta

| Skill | Purpose |
|---|---|
| `/setup-engine` | Configures engine + version, detects knowledge gaps, populates engine reference docs |
| `/skill-improve` | Improves a skill via a test-fix-retest loop |
| `/skill-test` | Validates skill files: static (linter), spec (behavioral), audit (coverage) |

---

## Suggested Order for Sprint 1 (current sprint as of 2026-06-08)

```
/qa-plan sprint        → define test case requirements BEFORE implementation
/dev-story [S1-01..05] → implement each Must Have task
/code-review [files]   → review each change before merge
/story-done [story]    → close out each task
/team-qa sprint        → run full QA cycle once Must Haves have commits to test
```
