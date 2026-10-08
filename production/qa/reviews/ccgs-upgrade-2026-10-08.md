# CCGS Framework Comparison & Merge — 2026-10-08

> **Source**: `.claude/Claude-Code-Game-Studios-main/Claude-Code-Game-Studios-main/` — Claude Code Game
> Studios **v1.1.2** (released 2026-09-29), added by the owner in commit `df1d09d3` "update skill studio"
> (484 files). Referred to below as **upstream**.
> **Local**: the framework copy imported on 2026-05-01 (`489c5510`, pre-v1.1 / v1.0 era) plus the
> project's own additions.
> **Method**: strategy A2 from upstream `UPGRADING.md` (selective copy, no shared history), done by file
> copy from the vendored folder. Every comparison below was produced by `diff --strip-trailing-cr` and
> `git log` per file. Nothing is committed.

## 1. Headline

| Area | Local before | Upstream v1.1.2 | Local after merge |
|---|---|---|---|
| Skills | 79 (72 template + 7 project-own) | 74 | **81** (74 upstream + 7 project-own) |
| Agents | 26 | 49 (incl. 10 Godot/Unreal) | **39** (Godot/Unreal specialists skipped) |
| `.claude/docs` files | 51 | 105 | 107 (upstream + 2 project-only kept) |
| `.claude/scripts` | none | 10 helpers | 10 |
| Rules | 14 project rules | 13 generic rules | 16 (14 project + 2 upstream); local frontmatter converted `globs:` → `paths:` (see §9) |
| Hooks | 13 (Unity-adapted) | 14 (`yaml-helper.sh`, `log-instructions.sh` new) | **staged in `.ccgs-staging/`, owner installs** (see §9) |
| Config | `production/review-mode.txt` + `technical-preferences.md` | `project.yaml` | `project.yaml` created and **finalized** (`review-mode.txt` deleted; `technical-preferences.md` kept by design) |
| Settings | local `settings.json` | adds `statusLine`, `Bash\|PowerShell` matcher, `$CLAUDE_PROJECT_DIR` paths, ~80 deny patterns | **staged in `.ccgs-staging/settings.json`, owner installs** |
| Unity engine reference | `VERSION.md` only | 16 files for Unity 6.3 | 15 files added with a 2022.3 filter (see §9) |

## 2. Were local framework files customised?

Decisive for what could be overwritten. `git log 489c5510..HEAD` per path:

| Group | Customised after import? | Decision |
|---|---|---|
| 72 template skills | **No** — one commit each (the import) | Overwritten with upstream |
| 26 agents | **No** | Overwritten with upstream; 13 missing agents added |
| `.claude/docs/technical-preferences.md` | Yes (`60d23ef9`, `9bebfffb`) — the project's real engine/naming/budget values | **Kept local**; values migrated into `project.yaml` |
| `.claude/docs/director-gates.md` | Yes (`3e0ee09d`) — a 9-line note that `architecture.md` and `session-state/active.md` do not exist | Overwritten (upstream rewrote the file, 875-line diff). The note's fact is unchanged and recorded in `skill-audit-2026-10-08.md` §4.2 |
| `.claude/docs/review-workflow.md`, `templates/skill-test-spec.md` | Local-only | Kept |
| Project-own skills `daily-standup`, `weekly-kickoff`, `weekly-wrapup`, `weekly-sprint`, `module-quality-audit`, `doc-sync`, `ui-screen` | Local-only | Kept untouched |
| Rules (14) | Heavily project-specific (`map-code`, `weapon-skill-code`, `manager-event-code`, …) | Kept; see §5 |
| Hooks `validate-commit.sh`, `detect-gaps.sh`, `lint-mermaid.py` | Yes (Mermaid lint, `changelog/` exclusion) | Not changed (blocked); merge notes in §6 |

## 3. Skills — detailed comparison

### 3.1 New in upstream

| Skill | Purpose | Relevance here |
|---|---|---|
| `settings` | View / set merged config (`project.yaml` + `project.local.yaml`) | Needed to manage the new config |
| `vertical-slice` | End-to-end pre-production validation build | Low — project is in Production |

### 3.2 Changed in every template skill (common to most of the 72)

- **Config resolution** through `.claude/hooks/yaml-helper.sh resolve_config` (`project.yaml` → `project.local.yaml` → legacy). **68 of 74 upstream skills call it** — see §7, blocker 1.
- **Code-root resolution** (`.claude/docs/code-root-resolution.md`): Unity → `Assets/`. This removes the
  `src/` assumption found in 14 local skills (skill audit §4.3).
- **`NOT ASSESSED` verdict** added to every gating skill (`gate-check`, `architecture-review`,
  `story-readiness`, `ux-review`, `test-evidence-review`, `prototype`) — a check that could not run no
  longer reports a pass. Codified in the new rule `skill-authoring.md`.
- **Automation modes** (`collaborative` / `guided` / `autonomous`, `.claude/docs/automation-modes.md`) —
  default `collaborative`; 68 skills honour it.
- **Rigor tiers** (`minimal` / `standard` / `full`) drive review depth, doc density, QA level, story
  granularity. This project is set to `standard` (= old behaviour, review mode `lean`).
- **Director gates** split into 32 per-gate files under `.claude/docs/director-gates/`.
- **Contracts**: 7 pipeline skills ship a `CONTRACT.md` (inputs/outputs/effects):
  `architecture-decision`, `create-epics`, `create-stories`, `dev-story`, `gate-check`, `story-done`,
  `story-readiness`.
- **Helper scripts** (`artifact-check.sh`, `story-status.sh`, `review-receipts.sh`, `review-scope.sh`,
  `project-coherence.sh`, `adr-dep-graph.sh`, `gdd-structure-check.sh`, `rotate-session-state.sh`) emit
  observations, never verdicts.
- Size: upstream skills are ~40–100% longer (e.g. `dev-story` 323 → 676 lines, `prototype` 157 → 636,
  `setup-engine` 715 → 1416). One shrank: `ux-design` 975 → 822.

### 3.3 Skill-audit findings resolved by the merge

| Finding (`skill-audit-2026-10-08.md`) | Status after merge |
|---|---|
| §4.1 — 13 missing agents, 6 team skills broken | ✅ **Resolved** — 0 dangling `subagent_type` references |
| §4.3 — 14 skills assume `src/` | ✅ **Resolved** via code-root resolution (once `yaml-helper.sh` is present) |
| §4.4 — `session-start.sh` double-counts bugs | ⏳ Upstream hook rewritten; pending hook merge |
| F-5 — checks that cannot run report pass | ✅ Addressed framework-wide (`NOT ASSESSED`) |
| F-1 bug-ID authority, F-2 doc-claim sweep, F-3 regression sweep, F-4 critical-path file | ❌ Not addressed upstream — project-own process gaps; still open |
| §4.2 `design/registry/entities.yaml` missing for `consistency-check` | ❌ Still missing |

### 3.4 Project-own skills vs upstream

No upstream equivalent for the PM routines (`daily-standup`, `weekly-*`, `module-quality-audit`) or
`doc-sync` / `ui-screen`. They are self-contained: none invokes a template skill as a sub-step (only
`module-quality-audit` *recommends* `/consistency-check`, `/architecture-review`,
`/propagate-design-change`). Scheduled autonomous runs are therefore not affected by the template
skills' new interactive defaults.

## 4. Agents — detailed comparison

| Change | Agents |
|---|---|
| Updated (all 26 local) | ai-programmer, art-director, creative-director, engine-programmer, game-designer, gameplay-programmer, lead-programmer, level-designer, performance-analyst, producer, prototyper (largest change, 356-line diff), qa-lead, qa-tester, release-manager, sound-designer, systems-designer, technical-artist, technical-director, tools-programmer, ui-programmer, unity-addressables-specialist, unity-dots-specialist, unity-shader-specialist, unity-specialist, unity-ui-specialist, ux-designer |
| Added (13) | accessibility-specialist, analytics-engineer, audio-director, community-manager, devops-engineer, economy-designer, live-ops-designer, localization-lead, narrative-director, network-programmer, security-engineer, world-builder, writer |
| Skipped (10, engine mismatch) | godot-csharp-specialist, godot-gdextension-specialist, godot-gdscript-specialist, godot-shader-specialist, godot-specialist, ue-blueprint-specialist, ue-gas-specialist, ue-replication-specialist, ue-umg-specialist, unreal-specialist |

`project.yaml` `specialists:` now names `unity-specialist` (code), `unity-shader-specialist`,
`unity-ui-specialist`, `unity-addressables-specialist`.

## 5. Rules

Upstream rules use Claude Code's `paths:` frontmatter (loaded only when matching files are touched).
**Local rules use `globs:` / `description:` (Cursor-style), which Claude Code does not read as a path
filter, so all 14 local rules load into every session.** Several local globs are also stale
(`Assets/Script/Skill_Ability/**` moved to `System/` on 2026-09-03).

| Upstream rule | Decision | Reason |
|---|---|---|
| `skill-authoring.md` | **Added** | Governs `.claude/skills/**`; applies to the 7 project-own skills |
| `agent-memory.md` | **Added** | Governs `.claude/agent-memory/**` (project has `lead-programmer/MEMORY.md`) |
| `ai-code`, `design-docs`, `engine-code`, `gameplay-code`, `prototype-code`, `test-standards`, `ui-code` | Not taken | Local versions carry project-specific, verified content; upstream versions are generic |
| `shader-code.md` | Not taken | Duplicates local `shader-vfx-code.md` |
| `data-files.md` | Not taken | Mandates snake_case JSON keys and lowercase file names; conflicts with existing room JSON (`CombatRoom_*.json`, camelCase `LevelData`) |
| `narrative.md`, `network-code.md` | Not taken | Out of scope (no narrative design, no networking) |

Recommended (not done): convert local rules from `globs:` to `paths:` with corrected globs. This changes
when rules load, so it is an owner decision.

## 6. Hooks and settings — NOT merged (blocked)

Copying upstream hooks into `.claude/hooks/` was refused by the session's auto-mode permission
classifier as self-modification. Nothing under `.claude/hooks/` or `.claude/settings.json` changed.

Hook-by-hook plan for the owner:

| Hook | Upstream change | Local customisation to re-apply |
|---|---|---|
| `yaml-helper.sh` (new, 66 KB library) | Config + code-root resolution used by 68 skills | — (**required**) |
| `log-instructions.sh` (new) | Logs instruction-file loads | Register in settings if wanted |
| `validate-commit.sh` 4.3 KB → 37.8 KB | Engine-aware code-root, GDD section checks via `gdd-structure-check.sh`, many more checks | Re-add the Mermaid lint block (local lines calling `lint-mermaid.py`) |
| `detect-gaps.sh` 3.6 KB → 15.3 KB | Engine-aware | Check `changelog/` subfolders are excluded from GDD/ADR counts (local edit in `7d1b5c79`) |
| `session-start.sh` 2.1 KB → 15.9 KB | Rewritten; bug counter reworked | Verify the "Open bugs" count no longer double-counts |
| `validate-push.sh`, `validate-assets.sh`, `validate-skill-change.sh`, `notify.sh`, `pre/post-compact.sh`, `session-stop.sh`, `log-agent*.sh` | Rewritten, `jq` optional | None (local copies are import-time originals) |
| `lint-mermaid.py` | Local-only | Keep |

`settings.json`: take upstream `hooks` block (paths via `$CLAUDE_PROJECT_DIR`, `Bash|PowerShell`
matcher, optional `statusLine` → `.claude/statusline.sh`, already copied) and its larger `deny` list;
**keep** the local `allow` list (git commit/checkout/fetch, the 9 routine skills, Notion and calendar
MCP servers). Note: a project `statusLine` overrides any user-level status line.

## 7. Open blockers after this merge

1. **`yaml-helper.sh` missing → upstream skills cannot resolve config.** Until it is copied into
   `.claude/hooks/`, the 68 dependent skills will fail at their first `resolve_config` call (they are
   written to report `NOT ASSESSED` rather than guess, but they will not do useful work). The 7
   project-own skills are unaffected.
2. **Migration not finalized.** `project.yaml` was generated by `migrate-v1-config.sh` and then completed
   by hand (nulls filled, `modes.rigor: standard` replacing the explicit `modes.review_mode: lean`, which
   `standard` expands to anyway). Because of that edit, `--finalize` will refuse with a mismatch on
   `modes.review_mode`; either re-add `modes.review_mode: lean` before finalizing or keep
   `production/review-mode.txt` as an inert legacy file. `technical-preferences.md` is never deleted by
   the script (it holds Forbidden Patterns and Allowed Libraries).
3. **Upstream Unity engine-reference docs not taken.** Upstream `docs/engine-reference/unity/` targets
   **Unity 6.3 LTS**; this project pins **2022.3.62f3**. Its breaking-changes / deprecated-API /
   module docs would steer agents to Unity 6 APIs. They were copied and then removed; local
   `VERSION.md` is unchanged.
4. **Upstream template defect**: `patch-notes` skill references `.claude/docs/templates/patch-notes-template.md`,
   which upstream does not ship.
5. **`CLAUDE.md` not restructured.** Upstream's template imports `.claude/docs/directory-structure.md`,
   `coordination-rules.md` and `coding-standards.md` and the engine `VERSION.md`. The project's
   `CLAUDE.md` already imports `VERSION.md`; adding the other three is optional and increases per-session
   context.
6. **Vendored folder.** `.claude/Claude-Code-Game-Studios-main/` (484 files, includes its own `CLAUDE.md`,
   `.github/`, Godot/Unreal references) stays in the repo. Keep it as the upgrade source, or remove it
   after the hook merge; it is not loaded as skills or agents.

## 8. Files touched by this merge

- Overwritten: 72 skill `SKILL.md` (+ `CONTRACT.md` / `references/` where shipped), 26 agents,
  48 `.claude/docs` files (some differ only in line endings).
- Added: skills `settings`, `vertical-slice`; 13 agents; 56 `.claude/docs` files (incl.
  `director-gates/`, guidance templates, `effects-map.md`, `automation-modes.md`,
  `code-root-resolution.md`, `config-resolution.md`); `.claude/scripts/` (10); `.claude/statusline.sh`;
  rules `skill-authoring.md`, `agent-memory.md`; `CCGS Skill Testing Framework/` (128 files, required by
  `skill-test` / `skill-improve`); `project.yaml`; `production/migration-report.md`.
- Unchanged: `.claude/hooks/*`, `.claude/settings.json`, `.claude/settings.local.json`, the 14 local rules,
  the 7 project-own skills, `CLAUDE.md`, `docs/engine-reference/unity/VERSION.md`,
  `.claude/docs/technical-preferences.md`.

## 9. Addendum — owner approval pass, 2026-10-08 (later the same day)

The owner approved the remaining items and asked for two changes of direction. What was done:

| Item | Result |
|---|---|
| Hooks + `settings.json` | **Still blocked.** The session's auto-mode classifier refused the copy again ("Self-Modification"); a chat approval does not lift it. Not worked around. Instead the merged result was **staged** in `.ccgs-staging/` for the owner to install (below). |
| Staged hooks | All 14 upstream hooks, plus the two local customisations re-applied: `detect-gaps.sh` excludes `*/changelog/*` from the GDD and ADR counts; `validate-commit.sh` runs `lint-mermaid.py` on staged `.md` files and blocks on a finding (inserted before the warnings block). `bash -n` clean on both. Dry run from the staging folder: `session-start.sh` reports **Open bugs: 41** (the old hook said 82). The staged hooks source `.claude/hooks/yaml-helper.sh` by its installed path, so they only read `project.yaml` once installed; called directly, `yaml-helper.sh resolve_config` resolves `engine: Unity 2022.3.62f3`, `rigor: standard`, `review_mode: lean`, `project.stage: Production`. |
| Staged `settings.json` | Upstream structure (hooks via `$CLAUDE_PROJECT_DIR`, `Bash\|PowerShell` matcher, `statusLine`) + **union** of permissions: 30 allow (all local entries kept: git commit/checkout/fetch, 9 routine skills, Notion + calendar MCP), 131 deny. |
| Rules frontmatter | All 13 local rules with `globs:` converted to `paths:` with current-tree globs (e.g. `Skill_Ability/` → `System/Skill_Ability/`, `System/Abilities/`, `System/Item/`, `System/StatSystem/` added to gameplay; `UIFlow/` added to UI; `LevelEdit/` and room JSON to map). The old `description:` line is kept as a comment. `language-reporting.md` has no frontmatter and still loads every session. **Effect:** these rules now load only when a matching file is touched, instead of every session. |
| Upstream rules not merged | Full item-by-item comparison written: `ccgs-rules-comparison-2026-10-08.md`. Awaiting owner choice. |
| Unity engine reference | Reversed decision per owner: the 15 upstream Unity files (all except `VERSION.md`) were copied into `docs/engine-reference/unity/` and **filtered**: a banner on every file plus 66 inline markers (⛔ not in 2022.3 / ✅ OK in 2022.3 / ❌ incorrect / ➖ not used), and a summary table `UNITY-2022.3-FILTER.md`. Notable corrections: RenderGraph and `linearVelocity` are Unity 6-only; "UGUI deprecated" is wrong (UIFlow is UGUI); `[GenerateSerializer]` is not a Unity API; the physics module is 3D-only. `VERSION.md` links the set and warns that `/setup-engine refresh` rewrites every file in the folder. |
| Migration finalize | `modes.review_mode: lean` re-added (equal to the `standard` expansion) so the verifier matches; `--finalize` ran: `production/review-mode.txt` deleted. `technical-preferences.md` intentionally kept (Forbidden Patterns, Allowed Libraries). The legacy `/weekly-sprint` skill still reads `review-mode.txt`; it is superseded by the dated routines. |

### Owner install step (one command, from the repo root)

```bash
cp .ccgs-staging/hooks/* .claude/hooks/ && cp .ccgs-staging/settings.json .claude/settings.json
```

Then start a new session (hooks and settings load at session start) and check the banner shows
`Review mode: lean` and `Open bugs: 41`. `.ccgs-staging/` is not committed and can be deleted afterwards.
To roll back: `git checkout HEAD -- .claude/hooks .claude/settings.json`.

## 10. Addendum — duplicate removal, 2026-10-08

Owner asked to remove skills and agents duplicated by the merge.

- **Removed** `.claude/Claude-Code-Game-Studios-main/Claude-Code-Game-Studios-main/.claude/skills/` and
  `.../.claude/agents/` (140 files). Verified first with `cmp`: 130 files were byte-identical to the live
  copies in `.claude/skills/` and `.claude/agents/`; the other 10 were the Godot/Unreal specialists that were
  deliberately not merged. Live set unchanged: 81 skills, 39 agents.
- **Kept** `/weekly-sprint`, although `docs/skill-reference.md` calls it superseded by `/weekly-kickoff` +
  `/weekly-wrapup`: the scheduled routines `pm-weekly-wrapup` (Sat 22:00, "the backbone") and
  `weekly-monday-kickoff` still invoke it. Removing it would break those runs. Retire it only after those
  routine prompts are rewritten.
- The rest of the vendored folder (`.claude/docs`, `hooks`, `rules`, `scripts`, `settings.json`, root docs,
  `CCGS Skill Testing Framework/`, Godot/Unreal engine references) is unchanged and remains the source for the
  next upgrade.
