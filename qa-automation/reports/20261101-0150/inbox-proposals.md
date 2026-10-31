# Bug-inbox proposals — QA run 20261101-0150

Defects found by the automated run that no open bug file covers. Proposed rows for the **Open notes**
table of `production/qa/bug-inbox.md` (`.claude/rules/bug-inbox.md`). No bug ID is allocated here; the
note numbers are given when the rows are written into the inbox. Code at `origin/sprint-20` `09ca5d19`.

| Proposed note | Source | Sev guess | Where | What | Evidence tier |
|---|---|---|---|---|---|
| A | session (QA automation) | S1 | `Assets/Script/Character/Player/States/PlayerDeathState.cs:16`; `Assets/Animation/Player/Paladin/Basic/Die/Knight_Die_dir 0-7.anim` | QA-VIT-07: after lethal damage the Paladin enters `PlayerDeathState` with HP 0, but `ON_PLAYER_DEATH` is never emitted (4 s watched, game time fixed 60 fps). The state emits only on `StatusAnimation.EndRangeTrigger`; all 8 Paladin Die clips carry one event, `AnimationOnAction` (→ `OnActivate`). Result: no Game Over / respawn path for a Paladin. Relates to BUG-086 / BUG-087 (both assume the event fires) | RUNTIME + STATIC |
| B | session (QA automation) | S1 | `Assets/Script/Character/Entity/States/EntityBasicState.cs:31-36`, `EntityTakeDamageState` | QA-ENT-04: Knight enemy (pool-spawned `EnemyPrefab`) hit to HP 0 stays in `EntityMoveState` and never emits `ON_ENEMY_DEATH` (5 s watched). The death check runs only while `entityInput.IsTakeDamage` is set; the lethal hit landed ~2 frames after a previous hit (enemy in hit-stun), so the flag was consumed without the HP check. A room whose last enemy dies this way never opens its doors. Reproduces in all 3 runs | RUNTIME |
| C | session (QA automation) | S2 | `Assets/SO/Skill/Paladin/Ability/Avatar of Light/` (ability + `Recovery Stat Effect`, `Gain For Time Effect`) | QA-ABL-01.Paladin.Ultimate: key 4 enters `PlayerSkillWeaponState` but the ability never reaches Do — no `AbilityCooldownStarted`, no stat change, no recovery within 2.5 s; 3 runs out of 3. It passed on 2026-10-31 in the owner's working tree (with uncommitted changes to `AbilityHolderBase` and the effects), so the committed `sprint-20` state differs | RUNTIME |
