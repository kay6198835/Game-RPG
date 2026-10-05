# System Documentation Index

One folder per gameplay/engine system. Each folder holds exactly two files:

| File | Role |
|------|------|
| `README.md` | **The current official document** for the system. Describes the code as it is at the HEAD named in its header — nothing planned, nothing historical. When it disagrees with a GDD, the code wins and the GDD is out of date |
| `CHANGELOG.md` | **Append-only history** of the system. One entry per change: date, commit, what changed (**from → to**), and why |

Created 2026-10-05 (HEAD `93ba6d8e`). Before this date, system history lived only inside
`CLAUDE.md` history blocks, `docs/CHANGELOG-DOCS.md` (per-document trail, still maintained) and
the bug files under `production/qa/bugs/`. Changelog entries dated before 2026-10-05 were
reconstructed from those sources and from `git log`; where a commit message gave no reason, the
entry says so rather than inventing one.

## Systems

| System | Folder | Code location | Design doc |
|--------|--------|---------------|------------|
| Character core (shared base + player) | [character/](character/README.md) | `Assets/Script/Character/Base/`, `Character/Player/` | `design/gdd/character-system.md`, ADR-0005 |
| Enemy (AI + spawning) | [enemy/](enemy/README.md) | `Character/Entity/`, `System/Enemy/` | `design/gdd/enemy-spawn-system.md`, ADR-0002, ADR-0003 |
| Abilities (v2 + legacy v1) | [abilities/](abilities/README.md) | `System/Abilities/`, `System/Skill_Ability/` | `design/gdd/skill-ability-system.md` (v1 only) |
| Weapons + projectiles | [weapons/](weapons/README.md) | `Weapons/`, `System/Abilities/Runtime/SpawnMono/ProjectileBody.cs` | `design/gdd/weapons-system.md` |
| Stats | [stats/](stats/README.md) | `System/StatSystem/` | `design/gdd/stat-system.md`, ADR-0001 |
| Map / dungeon / level editor | [map/](map/README.md) | `Map/`, `LevelEdit/`, `Assets/Data/Json/Room/` | `design/gdd/map-system.md` |
| Pathfinding | [pathfinding/](pathfinding/README.md) | `System/Pathfinding/` | none (BUG-052) |
| Object pooling | [object-pooling/](object-pooling/README.md) | `System/PoolableService/` | none |
| Dependency injection | [dependency-injection/](dependency-injection/README.md) | `System/LifetimeScope/`, `System/PlayerSystem/` | ADR-0004 |
| Event system | [event-system/](event-system/README.md) | `Manager/EventManager.cs` | none |
| Items | [items/](items/README.md) | `System/Item/` | none |
| UI (UIFlow + legacy) | [ui/](ui/README.md) | `UIFlow/`, `UI/` | `docs/ui/ui-ux-flow.md` |

## Other living documents (GDD, ADR, diagrams, UI, references)

These keep their **original path** — hooks, skills and `@`-imports depend on it — and get a change
log in a `changelog/` folder **beside** them, named `<doc>.CHANGELOG.md`. Each document carries a
`📜 Change log:` link under its title.

| Document group | Documents | Change logs |
|----------------|-----------|-------------|
| GDDs | `design/gdd/*.md` (10) | `design/gdd/changelog/` |
| ADRs + architecture notes | `docs/architecture/adr-000{1..5}*.md`, `character-architecture-analysis.md`, `character-migration-plan.md` | `docs/architecture/changelog/` |
| Diagrams | `docs/diagrams/*.md` (2) | `docs/diagrams/changelog/` |
| UI flow | `docs/ui/ui-ux-flow.md` | `docs/ui/changelog/` |
| References | `docs/skill-reference.md`, `docs/tech-debt-register.md` | `docs/changelog/` |
| Engine | `docs/engine-reference/unity/VERSION.md` | `docs/engine-reference/unity/changelog/` |

**Not covered (dated snapshots, never rewritten):** sprint plans and daily plans, retros, weekly
/ monthly reports (`bug-triage-DATE`, `module-health-MONTH`, `open-issues-DATE`), playtests,
session logs and state, `gdd-cross-review-DATE`, `architecture-review-DATE`,
`combat-balance-DATE`, `docs/archive/`. Also not covered: `.claude/rules/`, `production/` briefs,
epics and bug files.

A change log that opens with **`⚠️ Out of date against code`** means the document has known drift
that has not been fixed yet; the entry lists every stale statement with its line.

`changelog/` folders are excluded from the GDD section check in `.claude/hooks/validate-commit.sh`
and from the document counts in `.claude/hooks/detect-gaps.sh`.

## How to update

1. Change code.
2. Append an entry to that system's `CHANGELOG.md` (or the document's `changelog/<doc>.CHANGELOG.md`) (newest first) using the entry template below.
3. Edit that system's `README.md` so it describes the new code. Bump its `Last verified` line.
4. If the document has a Mermaid block, run `py .claude/hooks/lint-mermaid.py <file>` (rules: `.claude/rules/mermaid-diagrams.md`). The commit hook blocks on a finding.
5. If a bug status changed, update `CLAUDE.md` → Known Bugs and the bug file in `production/qa/bugs/`.

Entry template:

```markdown
## YYYY-MM-DD — <short title>
- **Commit:** `<hash>`
- **Changed:** <what>
- **From → To:** <old behaviour/shape> → <new behaviour/shape>
- **Why:** <reason, or "not recorded in the commit">
- **Bugs:** <ids opened / closed, or none>
```

`/doc-sync` maintains these folders together with `CLAUDE.md` and `memory/project_state.md`.
