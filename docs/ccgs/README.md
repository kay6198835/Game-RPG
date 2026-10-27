# CCGS Framework Reference

This folder holds reference material from **Claude Code Game Studios (CCGS) v1.1.2** (released
2026-09-29, MIT License — see [LICENSE](LICENSE)), the template this project's `.claude/` skills, agents,
hooks and docs come from. It was added in `df1d09d3` as a vendored copy under `.claude/`, merged on
2026-10-08, and then dissolved: live framework files went to their working locations, and the
reference-only files landed here.

Nothing in this folder is loaded automatically by Claude Code. Live framework files are under `.claude/`.

## Contents

| File | What it is | Use it when |
|---|---|---|
| [upstream-README.md](upstream-README.md) | The template's own README | Understanding what CCGS provides |
| [CHANGELOG.md](CHANGELOG.md) | CCGS release notes, up to 1.1.2 | Checking what a framework version changed |
| [UPGRADING.md](UPGRADING.md) | Upgrade strategies (A: git remote merge, A2: selective checkout) | The next CCGS upgrade |
| [migration-guide-v1.1.md](migration-guide-v1.1.md) | v1.0 legacy config → `project.yaml` | Reference only; this project is migrated and finalized |
| [WORKFLOW-GUIDE.md](WORKFLOW-GUIDE.md) | End-to-end guide to the CCGS workflow | Learning the intended skill sequence |
| [skill-flow-diagrams.md](skill-flow-diagrams.md) | Mermaid diagrams of how skills chain | Seeing which skill feeds which |
| [COLLABORATIVE-DESIGN-PRINCIPLE.md](COLLABORATIVE-DESIGN-PRINCIPLE.md) | Question → Options → Decision → Draft → Approval protocol | Cited by `.claude/docs/effects-map.md` |
| [CLAUDE.template.md](CLAUDE.template.md) | The template's root `CLAUDE.md` (renamed so it is not loaded) | Comparing with the project `CLAUDE.md` |
| [project.template.yaml](project.template.yaml) | The template's empty `project.yaml` | Comparing with the project `project.yaml` |
| [rules-upstream/](rules-upstream/) | The 11 upstream rules **not** merged into `.claude/rules/` | Deciding which bullets to merge — see `production/qa/reviews/ccgs-rules-comparison-2026-10-08.md` |
| [CONTRIBUTING.md](CONTRIBUTING.md), [SECURITY.md](SECURITY.md) | Upstream repository policies | Contributing back to CCGS |

## Where the rest of the vendored copy went

| Vendored path | Now |
|---|---|
| `.claude/skills/`, `.claude/agents/` | Live in `.claude/skills/` (81) and `.claude/agents/` (39); vendored copies were byte-identical and removed |
| `.claude/docs/`, `.claude/scripts/`, `.claude/statusline.sh` | Live under `.claude/` (identical, except the project-kept `technical-preferences.md`) |
| `.claude/hooks/`, `.claude/settings.json` | Live under `.claude/`, with project customisations (Mermaid lint, `changelog/` exclusion, local permissions) |
| `.claude/rules/skill-authoring.md`, `agent-memory.md` | Live in `.claude/rules/` |
| `.claude/rules/` (other 11) | [rules-upstream/](rules-upstream/) |
| `CCGS Skill Testing Framework/` | Repo root (identical) |
| `docs/CLAUDE.md`, `design/CLAUDE.md` | `docs/CLAUDE.md`, `design/CLAUDE.md` — adapted (Unity engine reference; project rule on 8 GDD sections) |
| `docs/architecture/tr-registry.yaml` | `docs/architecture/tr-registry.yaml` (empty template; used by `/architecture-review`, `/create-stories`, `/story-done`, `/story-readiness`) |
| `design/registry/entities.yaml` | `design/registry/entities.yaml` (empty template; primary input of `/consistency-check`) |
| `docs/engine-reference/unity/` | Filtered copies in `docs/engine-reference/unity/` (see `UNITY-2022.3-FILTER.md`) |
| `.gitattributes`, `.gitignore` | Relevant lines appended to the project's root files (LF for `*.sh` and `.claude/**/*.md`; `project.local.yaml`, `CLAUDE.local.md`) |
| `docs/engine-reference/godot/`, `unreal/`, `src/`, `.github/`, `docs/registry/architecture.yaml` | **Not kept** — Godot/Unreal only, upstream repository files, or superseded by the project's own `docs/registry/architecture.yaml`. Available from the upstream repository |
