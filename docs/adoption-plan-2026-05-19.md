# Adoption Plan

> **Generated**: 2026-05-19
> **Project phase**: Production
> **Engine**: Unity 2022.3.62f3 LTS
> **Template version**: v1.0+

Work through these steps in order. Check off each item as you complete it.
Re-run `/adopt` anytime to check remaining gaps.

---

## Step 1: Fix Blocking Gaps

No blocking gaps. Your project has no malformed artifacts — it has no artifacts at all.
Proceed directly to Step 2.

---

## Step 2: Fix High-Priority Gaps

### 2a. Create game concept document

The template assumes `design/gdd/game-concept.md` as the foundation for GDD authoring.
You have a clear concept (Unity roguelike, Cult of the Lamb combat, procedural rooms) — it just isn't written down.

**Fix**: Run `/reverse-document design Assets/Script/` to extract a concept from your code,
OR write it manually from the CLAUDE.md overview.

**Time**: 30 min
- [ ] `design/gdd/game-concept.md` created

---

### 2b. Create system GDDs (5 systems)

Without GDDs, `/create-stories`, `/story-readiness`, and `/architecture-review` cannot function.
Your code implies 5 systems that need design documents.

Run `/reverse-document design Assets/Script/Character` → `design/gdd/character-system.md`
Run `/reverse-document design Assets/Script/Weapons` → `design/gdd/weapons-system.md`
Run `/reverse-document design Assets/Script/Skill_Ability` → `design/gdd/skill-ability-system.md`
Run `/reverse-document design Assets/Script/Map` → `design/gdd/map-system.md`
Run `/reverse-document design Assets/Script/Manager` → `design/gdd/event-manager-system.md`

Each GDD needs all 8 required sections (Overview, Player Fantasy, Detailed Rules,
Formulas, Edge Cases, Dependencies, Tuning Knobs, Acceptance Criteria).

**Time**: 1 session each (~5 sessions total)
- [ ] `design/gdd/character-system.md` — 8 sections complete
- [ ] `design/gdd/weapons-system.md` — 8 sections complete
- [ ] `design/gdd/skill-ability-system.md` — 8 sections complete
- [ ] `design/gdd/map-system.md` — 8 sections complete
- [ ] `design/gdd/event-manager-system.md` — 8 sections complete

---

### 2c. Create systems index

`design/gdd/systems-index.md` is required by `/gate-check`, `/create-epics`, and
`/architecture-review`. It maps every system to its priority, layer, and design status.

**Fix**: Run `/map-systems` after `game-concept.md` exists.

**Time**: 30 min
- [ ] `design/gdd/systems-index.md` created with columns: System, Layer, Priority, Status

---

### 2d. Create Architecture Decision Records

Without ADRs, `/story-readiness` ADR checks silently pass everything (unsafe).
Your CLAUDE.md already lists 5 enforced architectural decisions — these need to be ADRs.

Run `/architecture-decision` for each:
1. State machine pattern for all characters
2. ScriptableObject-first data model
3. EventManager static bus for cross-system events
4. INegativeReceiver interface for all damage
5. No new singletons beyond MazeController

Each ADR requires: `## Status`, `## ADR Dependencies`, `## Engine Compatibility`,
`## GDD Requirements Addressed`, `## Performance Implications`.

**Time**: ~30 min each (~2.5 hrs total)
- [ ] `docs/architecture/adr-0001-state-machine.md`
- [ ] `docs/architecture/adr-0002-scriptableobject-data.md`
- [ ] `docs/architecture/adr-0003-event-manager.md`
- [ ] `docs/architecture/adr-0004-damage-interface.md`
- [ ] `docs/architecture/adr-0005-singleton-restriction.md`

---

### 2e. Create engine reference docs

Without `docs/engine-reference/unity/`, ADR engine compatibility checks are blind.
Skills like `/architecture-decision` and `/create-architecture` need this to flag
post-knowledge-cutoff API risks.

**Fix**: Run `/setup-engine` — populates `docs/engine-reference/unity/VERSION.md`.

**Time**: 5 min
- [ ] `docs/engine-reference/unity/VERSION.md` created

---

## Step 3: Bootstrap Infrastructure

### 3a. Register existing requirements (creates tr-registry.yaml)
Run `/architecture-review` after Steps 2b + 2d are done.
This bootstraps `docs/architecture/tr-registry.yaml` from your GDDs and ADRs.

**Time**: 1 session
- [ ] `docs/architecture/tr-registry.yaml` created

### 3b. Create control manifest
Run `/create-control-manifest` — compiles all ADR decisions into a flat rules sheet for programmers.

**Time**: 30 min
- [ ] `docs/architecture/control-manifest.md` created

### 3c. Create sprint plan and tracking file
Run `/sprint-plan` — creates `production/sprint-status.yaml` and a sprint markdown plan.
Organise the 8 remaining demo tasks from CLAUDE.md into stories.

**Time**: 30 min
- [ ] `production/sprints/sprint-01.md` created
- [ ] `production/sprint-status.yaml` created

### 3d. Set authoritative project stage
Run `/gate-check production` after completing Step 2 to validate readiness and
write `production/stage.txt` authoritatively.

**Time**: 5 min
- [ ] `production/stage.txt` written

---

## Step 4: Medium-Priority Gaps

### 4a. Architecture traceability matrix
`docs/architecture/architecture-traceability.md` is produced automatically by
`/architecture-review` (Step 3a). No separate action needed.
- [ ] Created as part of Step 3a

---

## What to Expect from Existing Stories

No stories exist yet. When you create stories via `/create-stories`, they will
automatically include TR-IDs, ADR references, and manifest version stamps because
they'll be generated fresh — no retrofitting needed.

---

## Re-run

Run `/adopt` again after completing Step 3 to verify all high-priority gaps are resolved.
