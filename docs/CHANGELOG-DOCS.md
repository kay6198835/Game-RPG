# Documentation Changelog

Every documentation change in this project, with the code change that caused it.

**Why this file exists.** Between 2026-08-22 and 2026-09-09 four structural changes landed in
`Assets/Script/` with no documentation update. By 2026-09-11 that had produced 23 drifted and
9 outdated living documents, including rules files that instructed contributors to write code
that would not compile. This log exists so a documentation change is never again silent about
which code change caused it.

**Conventions used here**
- Every entry names the **cause** (a commit, a file, or a verified source read), not just the effect.
- **Nothing is hard-deleted.** Outdated documents move to `docs/archive/` with a `⚠️ DEPRECATED`
  banner and their original content untouched.
- Dated snapshots (sprint logs, retros, triage reports, playtests) are **historical records** and
  are never rewritten — they were accurate on their own date.

---

## 2026-09-11 — Full documentation/code re-synchronisation

**Scope:** all living documentation. **Verified against:** HEAD `6d6a8e4`, branch
`origin/feature/synce-doc-and-code`. **Code changed:** none — this was a documentation-only pass.

### Root causes

Four structural code changes drove almost every edit below.

| ID | Change | Commit(s) | Date |
|---|---|---|---|
| **R1** | Seven directories moved under `Assets/Script/System/` — `Enemy/`, `Pathfinding/`, `Poolable/`→`PoolableService/`, `StatSystem/`, `Skill_Ability/`, `Item/`, `LifetimeScope/`. 77 renames | `1c0742e` (also `853fe39`, `f7acffc`, `0686619`) | 2026-09-03 |
| **R2** | **VContainer 1.19.0** dependency injection adopted; `GameLifetimeScope` composition root | `aa4e620` | 2026-08-22 |
| **R3** | `StatsSO.cs` deleted → `BaseStatsSO` + `EnemyStatSO`; `StatPointAllocator` added | `b0512f4`, `1c0742e` | 2026-08-28 / 09-03 |
| **R4** | `prototypes/skill-enhance-abilities/Scripts/` (17 files) promoted into `Assets/Script/System/Abilities/`; `AbilityHolder` rewritten to drive it | `9b8d40f`, `5c7afba` | 2026-09-09 |

Secondary causes: `INegativeReceiver.TakeDamage` changed `int` → `float` (`f3f5f08`); `EventID`
grew 20 → 23; a boss system was added and reverted (`ffe1976` → `4421fdc`, `ff67f4d`).

---

### Updated

#### `CLAUDE.md` — rewritten (563 → 743 lines)
**Cause:** R1, R2, R3, R4 + three false bug statuses.
**Changed:**
- Repository Layout rebuilt against the real tree (R1), with a Was→Is migration table so older
  documents remain readable.
- New sections: **Dependency Injection** (R2), **Player stat / vitals split**, **Skill / Ability —
  TWO frameworks now coexist** (R4), **Orphan directories**.
- `EventID` corrected 20 → **23** (`ON_RESET_STATS_UI_SESSION`, `ON_DROP_ITEM`, `ON_COLLECT_ITEM`),
  with the count history extended rather than replaced.
- Damage Chain rewritten: `float` signature, BUG-053 path now working, Defense mitigation.
- Package list: added VContainer 1.19.0, Timeline 1.7.7; noted DOTween is an Asset Store import.
- **Three bug-status corrections** — each was actively misleading:
  - `BUG-053` listed OPEN → it is **FIXED** (`f3f5f08`; `production/qa/bugs/BUG-053.md` had said so
    since 2026-09-06 — this table never followed the bug file).
  - `BUG-044` claimed the fix "properly stops `PlayerMovement`" → **false**;
    `PlayerDeathState.Enter()` only calls `base.Enter()`. Split out as `BUG-065`.
  - `NEW-4` claimed `Stat.modifiers` is no longer serialized → **regressed**; `Stat.cs:63-65`
    re-serializes it behind `#if UNITY_EDITOR`. Now `BUG-063`.
- Resolved a self-contradiction: the header claimed `EntityFindTarget` was deleted while the
  wiring section required it. The file exists — the header was wrong.
- Demo checklist gained items 18–20 (ability-framework decision, BUG-063, zero tests).
**Evidence:** `EventManager.cs`; `INegativeReciver.cs:7`; `Stat.cs:63-65`; `PlayerDeathState.cs:10-13`;
`EntityNegativeReciver.cs:18-25`; `Player.cs:58`; `GameLifetimeScope.cs`.

#### `README.md` — links repaired
**Cause:** R1.
**Changed:** 7 deep links to `Assets/Script/{Pathfinding,StatSystem}/…` were **404 on GitHub** after
the move — on the repository's public-facing page. All repointed and each verified to resolve on
disk. Project-structure block rebuilt; size corrected `~10k lines / 174 files` → `~11k / 194`.
**Note:** the commit that broke these links was `f7acffc`, *"chore(portfolio): prep repo for clean
public release"*.

#### `memory/project_state.md` — rewritten
**Cause:** R1–R4; three sprints stale (last updated 2026-08-21).
**Changed:** every row of the open-bug table was wrong — 7 bugs listed as open are fixed, 2 pointed
at deleted files (`EntityStatsSO.cs`, `EntityWeaponMelee.cs`), and it stated `prototypes/` "does not
exist". Rewritten with a structural-changes table, a key-API-changes table, and a re-prioritised
fix list.

#### `.claude/rules/gameplay-code.md`
**Cause:** `f3f5f08` (signature), R2.
**Changed:** `TakeDamage(int …)` → **`float`** — this rule would have produced non-compiling code
for three sprints. Added a **Dependency Resolution** section distinguishing the three mechanisms
(Core hub / DI / Inspector), and a max-vs-current stat ownership rule.

#### `.claude/rules/weapon-skill-code.md`
**Cause:** `f3f5f08`, BUG-043/046 closure, R4.
**Changed:** `int` → `float`; removed guidance referencing the deleted `EntityWeaponMelee`;
**"Skill Lifecycle Contract (ActivateSkill)" rewritten as a two-framework section** — the player
no longer uses the lifecycle this rule described.

#### `.claude/rules/engine-code.md`
**Cause:** R2.
**Changed:** "Components discover siblings via `Core.GetCoreComponent<T>()`, **never direct field
references**" was contradicted by shipped DI. Amended to give DI and the Core hub disjoint scopes,
and to constrain `GameLifetimeScope` from becoming a hub.

#### `.claude/rules/ai-code.md`
**Cause:** Bug #7 closed long ago; BUG-053/066; R3.
**Changed:** removed the stale instruction *"`EntityDeathState` must extend `EntityState` — fix the
base class bug before shipping"*. Added a **Damage Reception** section (one `INegativeReceiver` per
enemy, Defense in `DamageCalculate()`, the BUG-066 key-guard warning) and corrected the stat source
to `EnemyStatSO` / `EntityStatsHandler`.

#### `.claude/rules/scriptableobject-data.md`
**Cause:** R3 + the Sprint 8–10 rename.
**Changed:** `WeaponMeleeStats` → `MeleeWeaponStats`; `StatsCharacter` base → **`BaseStatsSO`**.
Added a **Runtime vs Authored Serialization** table — the distinction behind the project's only
committed data-corruption incident, currently regressed as BUG-063.

#### `.claude/rules/manager-event-code.md`
**Cause:** `EventID` 20 → 23.
**Changed:** added an **EventID Count** section. Flagged explicitly that
`ON_PLAYER_TAKE_DAMAGE` — which this rule instructs the health bar to bind to — **has never
existed**, and pointed to `EntityUIController` as the working push-based pattern.

#### `.claude/rules/ui-code.md`
**Cause:** R2, R3, the stat/vitals split.
**Changed:** added a table of where player values actually come from (current → `VitalStatsComponent`,
max → `StatHandler`), noted `PlayerData.currentHealth` is not a valid source (Bug #6), documented
the two deliberately-unregistered DI components, and recorded that `StatsUIController` still writes
gameplay state — a standing `ui-code.md` violation that survived the DI refactor.

#### `.claude/rules/prototype-code.md`
**Cause:** R4.
**Changed:** added a **Promotion Log**. The 2026-09-09 promotion bypassed this file's own Promotion
Rules (rewrite to production standards), and the residual prototype-grade code is named.

#### `.claude/rules/map-code.md`
**Cause:** verification only.
**Changed:** a re-verification stamp. **All five bugs it lists were checked in source and are still
open exactly as described** — this file needed no correction.

#### `docs/engine-reference/unity/VERSION.md`
**Cause:** R2.
**Changed:** package table corrected from `manifest.json` — **VContainer 1.19.0 was missing
entirely**, plus Timeline, Test Framework, uGUI and a note that DOTween is vendored and invisible to
the manifest. The **"UI Toolkit … not recommended for runtime UI"** guidance was **reversed**: runtime
UI Toolkit had already shipped (`UIController.cs`), so the file was contradicting the codebase every
time it was read. Added a VContainer API-notes section and a git-URL supply-chain risk note.

#### `docs/architecture/adr-0001-statsystem-dual-data-structure.md`
**Cause:** R3.
**Changed:** header warning + a **2026-09-11 Amendment**. The class the ADR is written about
(`StatsSO`) no longer exists; `BaseStatsSO` keeps the same API surface, so **the decision itself
stands unmodified** — this was a rename, not a redesign. The amendment also records that the
data-corruption hazard the ADR warns about has **regressed** (BUG-063), and that the source comment
saying "NEVER add `[SerializeField]`" did not prevent it.

#### `docs/architecture/adr-0002-enemymanager-singleton-exception.md`
**Cause:** R1, R2.
**Changed:** second amendment — path move, and a note that **ADR-0004 supersedes this ADR's
premise**. A singleton is no longer *necessary* for cross-system access; the exception is retained
only because `EntityMovement` reads it on runtime-spawned enemies, the case DI handles worst.

#### `design/gdd/stat-system.md`
**Cause:** R3.
**Changed:** all `StatsSO` references → `BaseStatsSO` + a scope banner. **Rules, formulas and
acceptance criteria are unchanged and still valid** — only the type name and path moved. Banner also
flags what this GDD does *not* yet cover: `StatPointAllocator`, the max/current split, and BUG-063.

#### `design/gdd/skill-ability-system.md`
**Cause:** R4.
**Changed:** scope banner. This GDD is **not obsolete but now partial** — it documents v1
(`ActivateSkill`), which still drives weapons and enemies, while the **player** moved to v2. A
comparison table names which is which. v2 is deliberately *not* retro-designed here: one system =
one GDD (`.claude/rules/design-docs.md`), and no ADR has decided whether the two converge.

#### `design/gdd/character-system.md`
**Cause:** the Sprint 12 stat/vitals split; `f3f5f08`.
**Changed:** banner mapping the three-component split for both player and enemy, the `float`
signature, Defense mitigation, the ninth player state, and confirmation that death/restart remains
`[PLANNED]`.

#### `design/gdd/enemy-spawn-system.md`
**Cause:** BUG-033/BUG-053 closure, R1, R2.
**Changed:** status banner. **The algorithm did not drift** — `GetSpawnSet()` still matches. Four
status corrections (BUG-033 fixed, epic unblocked, path moved, DI-resolved); the three genuinely
open items are restated.

#### `design/gdd/weapons-system.md`, `attack-speed-system.md`, `map-system.md`, `animation-system.md`
**Cause:** R1, R3, `f3f5f08`; verification.
**Changed:** verification banners. `map-system.md` and `animation-system.md` had **no drift** — all
five map bugs re-confirmed open, and the `StatusAnimation` handoff unchanged. `weapons-system.md`
and `attack-speed-system.md` took `float` / `BaseStatsSO` / path corrections only.

#### `design/gdd/systems-index.md`
**Cause:** R2, R4, the Item system.
**Changed:** **three systems added** (24 DI, 25 Abilities v2, 26 Item) — all shipped without ever
entering the index. "Built without a GDD" corrected 4 → **7**. "Next Systems to Design" re-ordered:
the ability-framework decision is now the top blocker.

#### `docs/diagrams/ability-system-diagrams.md` — banner **inverted**
**Cause:** R4.
**Changed:** the banner said *"These diagrams do NOT describe the ability system the game runs"* and
*"ships no SO assets … nothing can instantiate it"*. **Both became false on 2026-09-09.** This is now
the most accurate description of the player's live ability system. Rewritten with the three deltas
that occurred during promotion (`AbilitySystem` absorbed into `AbilityHolder`;
`ShootSpiritOrbEffect` → `ShootObjectEffect`; `DamageInFrontEffect.Apply()` uncommented).
**Evidence that it is wired:** `Assets/SO/Skill/ShootSpirit/{ShootSpirit,SpiritBomd}.asset`,
`Assets/SO/Skill/Conditions/New Has Enough Mana Condition.asset`.

#### `prototypes/skill-enhance-abilities/README.md`
**Cause:** R4.
**Changed:** Result `[IN PROGRESS — never finished, never wired]` → **`[VALIDATED — promoted 2026-09-09]`**.
Original body retained unedited as the historical record; a **Promotion (2026-09-09)** section audits
the promotion against this README's own four "if you pick this back up" steps — steps 1–3 done,
step 4 (rewrite to production standards) **partial**.

#### `docs/tech-debt-register.md`
**Cause:** R1–R4 + source re-verification of every entry.
**Changed:** 39 → 43 items.
- **TD-011 → VOID** — carried at Critical/Priority-1 for 14 weeks against `EntityStatsSO.cs`,
  a file deleted in `b0512f4`.
- **TD-036 → CLOSED** — every sub-item verified fixed; two residuals re-filed as TD-041/TD-042
  rather than left buried.
- **TD-038 → REOPENED (regression)** — now BUG-063.
- **TD-005 re-scoped**, **TD-003 widened**, **TD-031** corrected (`prototypes/` does exist).
- **New:** TD-040 (two ability frameworks, no ADR), TD-041 (BUG-066 key guard),
  TD-042 (BUG-043 residual), TD-043 (orphan `.meta`-only directories).

#### `docs/registry/architecture.yaml`
**Cause:** R2 + a pre-existing syntax defect.
**Changed:** added the `vcontainer_dependency_injection` decision (ADR-0004). Corrected the
forbidden-patterns note: the allocating-`OverlapCircle` entry pointed at the **deleted**
`EntityWeaponMelee.cs`; `GameObject.Find` re-confirmed as the only remaining instance
(`FollowPlayer.cs:24`); the UI-mutates-state violation re-confirmed — it survived the DI refactor
and now writes through `IPlayerStatService` (`StatsUIController.cs:110,158,172`).
**Also fixed a pre-existing defect:** this file **did not parse as YAML** — an unquoted `:` inside a
`scope:` value, present at HEAD before this pass. A machine-readable registry that no parser can
read is worse than none. Converted to a block scalar; the file now parses.

#### `production/epics/index.md`
**Cause:** TD-036 closure; R2, R4, Item system.
**Changed:** the enemy-spawn epic's end-to-end blocker is recorded as gone. "Systems with code but
no epic" grew 4 → **7**; all three new entries shipped through un-storied commits.

#### `production/qa/bugs/BUG-052.md`
**Cause:** R2 (partial closure), R4, Item system.
**Changed:** one subsystem left the list (DI, now ADR-0004); three joined (Item, Abilities v2, UI).
The gap **grew rather than closed**.

#### `.claude/agent-memory/lead-programmer/MEMORY.md`
**Cause:** R1–R4.
**Changed:** re-synced. It had gone stale within three sprints of its last rewrite — noted in the
header as a recurring property of shortcut files, with a seven-line delta summary.

#### `docs/skill-reference.md`
**Cause:** file count drift.
**Changed:** 79 → **80** skills; **3 → 4 ADRs**, with ADR-0004's 20-day retroactive gap named.

#### `.claude/skills/module-quality-audit/SKILL.md`
**Cause:** R1.
**Changed:** path note. This skill's path-based scans would **silently return nothing** for moved
modules, reporting them as unchanged rather than as missing.

#### `Assets/Animation/DUPLICATE_NOTE.md`
**Cause:** `.claude/rules/language-reporting.md` (stored `.md` must be English) + re-verification.
**Changed:** rewritten in English with the original Vietnamese preserved verbatim below. Counts
**re-verified: 155 `.anim` in each folder**, still unresolved after 13 sprints. Added why this cannot
be settled from the repository alone (`.anim` files do not record their referencing Animator).

---

### Created

#### `docs/architecture/adr-0004-vcontainer-dependency-injection.md` — **new**
**Cause:** R2 — the largest architectural change in the project shipped with no ADR.
**Contents:** context (three competing resolution mechanisms, and the singleton pressure that
produced `LevelManager`/TD-023), the decision as implemented, five alternatives with reasons for
rejection — including `ScriptableObject`-as-service-locator, rejected because this project has
already been burned by SO persistence (TD-038/BUG-063) — consequences, the git-URL supply-chain
risk, a source-verified checklist, and three open questions.
**Status:** Accepted (retroactively documented). The 20-day gap between `aa4e620` and this ADR is
recorded in the ADR itself as the reason it exists.

#### `docs/CHANGELOG-DOCS.md` — this file.

---

### Archived (moved, not deleted)

| From | To | Reason |
|---|---|---|
| `docs/reviews/melee-combat-review.md` | `docs/archive/reviews/melee-combat-review.md` | Every class it reviews is deleted or rewritten. `WeaponMelee.Attack()` (section A) became `MeleeWeapon.OnActivate()` and is now the project's *reference* implementation — the opposite of the review's finding. `EntityWeaponMelee.cs` (section B) was **deleted**. Its header verdict ("both melee directions deal no reliable damage", "0 of 16 issues fixed") was true at HEAD `7995066` on 2026-07-30 and is false today. Content untouched; banner names each superseding document. |
| `docs/adoption-plan-2026-05-19.md` | `docs/archive/adoption-plan-2026-05-19.md` | A one-time `/adopt` onboarding checklist from project start, 13 sprints ago. Living in `docs/` invited reading it as an outstanding to-do list. Content untouched; banner points to the four documents that now carry its concerns. |

`docs/reviews/` is now empty and was removed. `docs/archive/` was created by this pass.

---

### Historical snapshots — banner added, content untouched

These are accurate records of their own date. They were **not rewritten**; each received a
`📅 HISTORICAL SNAPSHOT` banner because each was being read as a current assessment.

| Document | Why the banner |
|---|---|
| `docs/architecture/architecture-review-2026-07-13.md` | Its CONCERNS verdict and "8 GDDs · 3 ADRs" counts are from 2026-07-13; there are now 4 ADRs, ADR-0001's subject class is deleted, and three further systems shipped undocumented. |
| `production/qa/module-health-2026-08.md` | **Every** CRITICAL verdict in its scorecard has since been resolved; its "skill-ability-system HEALTHY" verdict is also stale after R4. |
| `design/balance/combat-balance-2026-07-07.md` | Two of its four cited source files no longer exist at those paths (`StatsSO.cs` deleted; `DerivedStatFormula.cs` moved). The **numbers were not re-derived** in this pass and may still be valid — the banner says so explicitly and names what to re-validate against. |

---

### Deliberately NOT changed

| Set | Count | Reason |
|---|---|---|
| `production/sprints/`, `production/retros/`, `production/qa/bug-triage-*`, `production/qa/playtests/`, `production/session-state/`, `design/gdd/gdd-cross-review-*` | ~59 | **Point-in-time records.** Their stale paths and bug statuses were correct on their own date. Rewriting them would destroy the project's history — the opposite of this pass's purpose. |
| `.claude/docs/*.md` (9 files), `.claude/docs/templates/` (38), `.claude/docs/hooks-reference/` (6) | 53 | Checked for drift (stale paths, `StatsSO`, `EntityWeaponMelee`, `TakeDamage(int`): **zero hits**. Generic tooling and blank templates, not project documentation. |
| `.claude/skills/` (79 of 80), `.claude/agents/` (26) | 105 | Engine- and project-agnostic. Only `module-quality-audit/SKILL.md` hardcoded project paths. |
| All source code | 194 `.cs` | **This pass was documentation-only by instruction.** Every defect found in code — BUG-063, BUG-065, BUG-066, BUG-064 sub-item 7, the orphan directories, the `AbilityHolder` null dereference — was **documented, not fixed**, and is tracked in `CLAUDE.md`, `docs/tech-debt-register.md` and `production/qa/bugs/`. |

---

### Findings that need a decision (documentation cannot resolve these)

1. **BUG-063** — `Stat.modifiers` re-serialized (`Stat.cs:63-65`). A one-line fix, no technical
   blocker, carried 24+ triage cycles, and it can corrupt committed `.asset` files. It has already
   happened once.
2. **Two ability frameworks, no ADR** (TD-040) — v1 drives weapons/enemies, v2 drives the player.
   Until this is decided, `design/gdd/skill-ability-system.md` cannot be made authoritative and
   Abilities v2 cannot be designed at all.
3. **The documentation gap is systemic, not incidental.** Four structural changes shipped across
   three sprints with zero doc updates, in a project that runs weekly wrap-ups and monthly module
   audits. Both rituals read documents rather than diffing the tree, so neither could see R1–R4.
   Consider a pre-commit or weekly check that diffs `Assets/Script/`'s directory list against
   `CLAUDE.md`'s Repository Layout.
