# Changelog — `ability-system-diagrams.md`

Change log for [`docs/diagrams/ability-system-diagrams.md`](../ability-system-diagrams.md). The document itself is the **current official version**;
this file records **when** it changed, **what** changed (**from → to**) and **why**. Newest first.

Built 2026-10-05 from `git log --follow` and the per-document trail in `docs/CHANGELOG-DOCS.md`.
Where neither source states a reason, the entry says so instead of guessing. From 2026-10-05 on,
append entries by hand (template in `docs/systems/README.md`).

## 2026-10-05 — Re-synced against code (doc-sync)
- **Commit:** not yet committed (working tree on `origin/feature/synce-doc-and-code`, base HEAD `93ba6d8e`)
- **Changed:** corrected every statement found stale against HEAD `93ba6d8e`. Each item reads *was stale → now says*:
  - **From → To:** `:197` says `AbilityHolder` is registered with `RegisterComponentInHierarchy<AbilityHolder>()` — removed 2026-09-28; the player hierarchy is injected by `PlayerManager` via `resolver.Instantiate`
  - **From → To:** `:828`, `:974` draw `SpawnProjectileBase.OnTriggerEnter(Collider) :29` — the projectile now detects hits in `ProjectileBody.OnTriggerEnter2D` and calls `SpawnProjectileBase.OnHit()`. §6-§9 kept as dated snapshots; new **§10 Current version** added with the current damage path, and a pointer to §10 under the title
- **Why:** code changed in sprints 16-17 (`ddcc0a5c`, `5b035b73`, `b7a0af5e`, `9154763f`, `0bc36406`, `3a395fe9`, `d3400ee7`, `40d2c793`) without a matching doc edit
- **Bugs:** BUG-095 filed; Bug #12, Bug #13, BUG-064, TD-023, TD-050 recorded as closed where the document tracked them

## 2026-09-22 — docs(qa): re-verify the bug register against 2a83469 and the owner review
- **Commit:** `e306c639` (+133 / −1 lines)
- **Why:** (third pass, same day) — Post-fetch re-verification against HEAD `2a83469` (`docs/CHANGELOG-DOCS.md`)
- **Changed:** `docs/diagrams/ability-system-diagrams.md` | **§1–§8 left unedited.** New **§9** appended: what the two commits did, the build break, a redrawn activation-path diagram showing the single `TryStart()` gate and BUG-089's fall-through, a corrections table against §6/§8, revised strengths/weaknesses, and a revised fix order with BUG-088 as step 0. Navigation block at the top updated to point at §9 first
- **Why:** (later same day) — Owner review: three findings corrected or retracted (`docs/CHANGELOG-DOCS.md`)
- **Changed:** `docs/diagrams/ability-system-diagrams.md` | **§6 and §7 left unedited.** New **§8 Owner review** appended: corrections table, a corrected damage-path diagram (contract → two independent gaps), a corrected Consecrate sequence diagram (telegraph → payload), and revisions to §7 — weakness #1 withdrawn, #2 softened, #3 split, #5 re-aimed, a new strength added for the one-off/channelled cost distinction, and a revised …
- **Why:** Bug-documentation re-verification pass (`docs/CHANGELOG-DOCS.md`)
- **Changed:** Severity lowered S2 → S3. Also propagated: `docs/diagrams/ability-system-diagrams.md` lines 226
- **Changed:** `docs/diagrams/ability-system-diagrams.md` | **Additive only — §1–§5 untouched.** Pointer block added under the existing defect list, then **§6 Current Version** (five stale §1–§3 annotations tabulated, plus three new source-drawn diagrams: the two-list cost model, the damage dead end, the `SpawnSummonEffect` double spawn) and **§7 System assessment** (7 strengths / 11 weaknesses, ranked, with a recommended fix …
- **Changed:** **`docs/diagrams/ability-system-diagrams.md`** — **addressed 2026-09-22 by addition, not rewrite**,
- **Sections touched:** “Ability System — Diagrams”, “8.4 Corrections to §7”
- **From → To:** (nothing) → `` > - **§9 — Post-fetch re-verification (same day, HEAD `2a83469`)**: two commits landed after §8 was ``
- **From → To:** `` (per dispatch is frame-order dependent; per second would not be). Both belong in the Abilities v2 `` → `` (per dispatch vs per second — ✏️ **corrected 2026-09-22: the dispatch is NOT frame-order dependent.** `AbilityHolder.HandleInput()` is … ``

## 2026-09-22 — update docs bug
- **Commit:** `56f7ea92` (+372 / −0 lines)
- **Sections touched:** “Ability System — Diagrams”, “5. Class Diagram — Full Stereotypes (Standard Mermaid)”
- **From → To:** (nothing) → `` > ### ➕ ADDED 2026-09-22 — read §6 and §7 for the current version ``
- **From → To:** (nothing) → `` --- ``
- **Why:** commit message: “update docs bug”

## 2026-09-21 — update doc for ability system, update 1-7 dir animation clip, done animation for ability consecrae
- **Commit:** `ac347d89` (+223 / −120 lines)
- **Why:** Abilities v2 re-synchronisation + Paladin direction renumber (`docs/CHANGELOG-DOCS.md`)
- **Changed:** `docs/diagrams/ability-system-diagrams.md` | **§1–§3 rewritten from source.** The 2026-09-11 pass corrected this file's status banner but left the diagrams describing the deleted Spirit Orb prototype. New architecture flowchart, activation sequence and lifecycle state diagram, each annotated with the open bug that breaks that step. §4–§5 kept and explicitly marked HISTORICAL. Deletion table added at the top.
- **Sections touched:** “Ability System — Diagrams”, “1. Architecture Overview”, “2. Activation Flow (Sequence Diagram)”, “3. Ability Lifecycle (State Diagram)”
- **From → To:** `` > Diagrams authored: 2026-05-20 · Status re-verified: **2026-09-11** `` → `` > Diagrams authored: 2026-05-20 · **§1–§3 rewritten 2026-09-21** against ``
- **From → To:** `` > ✅ **STATUS INVERTED 2026-09-11 — these diagrams now describe the ability system the PLAYER runs.** `` → `` > ⚠️ **REWRITTEN 2026-09-21 — §1–§3 previously described code that no longer exists.** ``

## 2026-09-11 — docs: full documentation/code re-synchronisation against HEAD 6d6a8e4
- **Commit:** `bbfb3028` (+47 / −23 lines)
- **Sections touched:** “Ability System — Diagrams”
- **From → To:** `` > Source: `prototypes/skill-enhance-abilities/Scripts/` (was `Assets/Skill Enhance/Scripts/` until 2026-08-22) `` → `` > Source: **`Assets/Script/System/Abilities/`** (originally `Assets/Skill Enhance/Scripts/`; ``
- **From → To:** `` > ⚠️ **These diagrams do NOT describe the ability system the game runs** (verified 2026-08-21). `` → `` > ✅ **STATUS INVERTED 2026-09-11 — these diagrams now describe the ability system the PLAYER runs.** ``
- **Why:** commit message: “docs: full documentation/code re-synchronisation against HEAD 6d6a8e4”

## 2026-08-26 — coding
- **Commit:** `6afefdec` (+3 / −3 lines)
- **Sections touched:** “4. Class Diagram — Full System (Draw.io Compatible)”, “5. Class Diagram — Full Stereotypes (Standard Mermaid)”
- **From → To:** `` +RuntimeStat MaxMana `` → `` +RuntimeStat Mana ``
- **From → To:** `` +RuntimeStat MaxMana `` → `` +RuntimeStat Mana ``
- **From → To:** `` MaxMana `` → `` Mana ``
- **Why:** commit message: “coding”

## 2026-08-22 — chore: park the Skill Enhance ability framework under prototypes/
- **Commit:** `eda31a1e` (+14 / −6 lines)
- **Sections touched:** “Ability System — Diagrams”
- **From → To:** `` > Source: `Assets/Skill Enhance/Scripts/` `` → `` > Source: `prototypes/skill-enhance-abilities/Scripts/` (was `Assets/Skill Enhance/Scripts/` until 2026-08-22) ``
- **From → To:** `` > They accurately describe the 17 files in `Assets/Skill Enhance/Scripts/Abilities/` — but that `` → `` > They accurately describe the 17 files now in `prototypes/skill-enhance-abilities/Scripts/Abilities/` ``
- **From → To:** `` > Owner decision owed: adopt this framework, relocate it under `prototypes/` per `` → `` > ✅ **Resolved 2026-08-22 (owner decision): kept, and relocated to `prototypes/`** per ``
- **Why:** commit message: “chore: park the Skill Enhance ability framework under prototypes/”

## 2026-08-21 — docs: second-pass audit of the four documents the first pass never opened
- **Commit:** `3e0ee09d` (+42 / −20 lines)
- **Sections touched:** “Ability System — Diagrams”, “1. Architecture Overview”, “2. Spirit Orb Activation Flow (Sequence Diagram)”, “3. Ability Lifecycle (State Diagram)”, “5. Class Diagram — Full Stereotypes (Standard Mermaid)”
- **From → To:** (nothing) → `` > ⚠️ **These diagrams do NOT describe the ability system the game runs** (verified 2026-08-21). ``
- **From → To:** `` ## 1. Kiến trúc tổng thể (Architecture Overview) `` → `` ## 1. Architecture Overview ``
- **From → To:** `` ORB["SpiritOrbProjectile\nRigidbody2D.velocity = dir × 10\nDestroy after 8s nếu miss"] `` → `` ORB["SpiritOrbProjectile\nRigidbody2D.velocity = dir × 10\nDestroy after 8s if it misses"] ``
- … 12 more hunks — `git show 3e0ee09d -- docs/diagrams/ability-system-diagrams.md`
- **Why:** commit message: “docs: second-pass audit of the four documents the first pass never opened”

## 2026-05-20 — Add Section 5 — full stereotype class diagram (interface, ScriptableObject, MonoBehaviour)
- **Commit:** `71de6e27` (+235 / −0 lines)
- **Sections touched:** “4. Class Diagram — Full System (Draw.io Compatible)”
- **From → To:** (nothing) → `` --- ``
- **Why:** commit message: “Add Section 5 — full stereotype class diagram (interface, ScriptableObject, MonoBehaviour)”

## 2026-05-20 — Remove duplicate Section 5, rename Section 4 to draw.io compatible
- **Commit:** `60179d76` (+1 / −198 lines)
- **Sections touched:** “4. Class Diagram — Full System (Draw.io Compatible)”
- **From → To:** `` ## 4. Class Diagram — Full System `` → `` ## 4. Class Diagram — Full System (Draw.io Compatible) ``
- **From → To:** `` ## 5. Class Diagram — Draw.io Compatible `` → (removed)
- **Why:** commit message: “Remove duplicate Section 5, rename Section 4 to draw.io compatible”

## 2026-05-20 — Add draw.io-compatible class diagram (Section 5)
- **Commit:** `1c526004` (+198 / −0 lines)
- **Sections touched:** “4. Class Diagram — Full System”
- **From → To:** (nothing) → `` ```mermaid ``
- **Why:** commit message: “Add draw.io-compatible class diagram (Section 5)”

## 2026-05-20 — Add ability system Mermaid diagrams (architecture, sequence, state, class)
- **Commit:** `4dc8439e` (+311 / −0 lines)
- **Changed (from → to):** document created (+311 lines)
- **Why:** commit message: “Add ability system Mermaid diagrams (architecture, sequence, state, class)”
