# Abilities — Changelog

Newest first. Entries before 2026-10-05 reconstructed from `CLAUDE.md` history, `docs/CHANGELOG-DOCS.md`, the bug files and `git log`.

## 2026-11-26 — Spawn effect controllers: Exit transition no longer fires
- **Commit:** `sprint-24`, 2026-11-26 — the commit that adds this entry
- **Changed:** `CircleSummonEffect`, `RainArrows`, `RuneCircleEffect`, `LightningEffect` controllers: the one transition (state → Exit, no condition, ExitTime 0.75, 0.25 s) gets `m_HasExitTime` 1 → 0. With no condition and no exit time Unity ignores it (Editor shows a warning on that transition). `Trap.controller` has no transition; `Slash-VFX-GO` has no Animator.
- **From → To:** at 75 % of the clip the state left for Exit and replayed from the start, cutting the old clip 0.25 s later → `DespawnOneSelf` after that point never fired: CircleSummon (event 2.0 s, cut 1.775 s) and RainArrows (2.033 s, cut 1.80 s) never despawned, CircleSummon re-fired `Execute` every ~1.5 s (endless Rain Arrows), RainArrows replayed its waves and lost waves 23–25; RuneCircle (Consecrate) despawned just in time but flashed back to frame 0 → each clip plays once and despawns on its own event.
- **Why:** owner, 2026-11-26.
- **Evidence:** STATIC — not run. To check in Unity: one CircleSummon → one RainArrows, both gone after ~2 s; Consecrate circle no flash.
- **Bugs:** none opened or closed.

## 2026-11-26 — Consecrate player clips: no loop, end frame padded; Activate → Cast exit time 1
- **Commit:** `sprint-24`, 2026-11-26 — the commit that adds this entry
- **Changed:** `Paladin_Consecrate_State1_dir 0…7` and `Paladin_Consecrate_State3_dir 0…7` (16 clips): `m_LoopTime` 1 → 0, `m_StopTime` = last key + 1/30 (0.6333 → 0.6667, 1.2 → 1.2333). `Paladin.controller`: transition Activate → Cast `m_ExitTime` 0 → 1. State2 (Cast) clips keep their loop: each loop's `AnimationOnAction` drives `HandleInput` while a Hold ability is held.
- **From → To:** `8005d27e` (30 fps resample) snapped StopTime onto the last key, so with Loop Time on the clip wrapped to frame 0 at the end; `AnimationEnd` / `AnimationFinishTrigger` sat on the wrap point and `AnimationStart` could fire again → frame-0 flash and Do replaying from the start → the clip plays once and its end events sit inside the clip. ExitTime 0 on a looping clip fired at the wrap; 1 states "at the end" explicitly, for looping and non-looping clips alike.
- **Why:** owner, 2026-11-26 (Consecrate stutter / repeat).
- **Scope:** the transition is in the controller shared by every Paladin ability (Slash, Blessing, Dash, Block, combo); `Rain Arrows.asset` also uses `Paladin_Consecrate.overrideController`.
- **Evidence:** STATIC — not run. To check in Unity: Consecrate plays Activate → Cast → Do once, no frame-0 flash, returns to Idle / Move; Slash (Hold), Blessing, Dash unchanged.
- **Bugs:** none opened or closed.

## 2026-11-26 — SpawnSummonEffect: no spawn in TryCast; RainArrows clips end on a held frame
- **Commit:** `sprint-24`, 2026-11-26 — the commit that adds this entry
- **Changed:** `SpawnSummonEffect.TryCast` no longer calls `base.Apply` (commented out, PENDING — owner redesigns the Casting flow). The 8 `RainArrows_dir 0…7.anim` clips get a copy of their last frame at 2.0333 s (`m_StopTime` 2.0333 → 2.0667) and `DespawnOneSelf` moves from 2 s to 2.0333 s.
- **From → To:** a summon ability spawned twice per cast — `TryCast` (Casting) and `Apply` (Do) — so Rain Arrows / Consecrate hit and ran every SubEffect twice (since owner commits `8377ebaf`…`f75573c8` the TryCast circle also spawns the child) → one spawn, in `Apply`. RainArrows despawned at the start of its last frame → the last frame is shown for one frame, then it despawns.
- **Why:** owner, 2026-11-26: the Casting phase is the power build-up phase, so `TryCast` calling `Apply` is the wrong flow.
- **Evidence:** STATIC — not run. To check in Unity: one RainArrows / one Lightning per cast, target HP drops once per hit.
- **Bugs:** none opened or closed.

## 2026-11-26 — StatModifier: magnitude + serialized impactType (A1)
- **Commit:** `sprint-24`, 2026-11-26 — the commit that adds this entry
- **Changed:** `StatModifier`: `impactType` back as `[SerializeField]`, `value` `[Range(0.1, 999)]` (`MinValue` / `MaxValue`), constructor stores `Clamp(|value|, 0.1, 999)` and the given `impactType`, `WithSource` copies `impactType`. Readers apply the sign from `ImpactType`: `StatSheet.ApplyModifiers`, `QAEnemySuite.Items` (expected flats / vitals). `SyncModifiersFromStatusType` seeds new rows with `MinValue`. `EffectReceiverBase.AdjustCurrentStats` and `Utility.ModifierStatsCalculate` are the owner's version from `8377ebaf` (per row, amount from the current value, direction from `ImpactType`) — the session's own edits to those two files were dropped when that commit was pulled. README Open issues row and "Values and sign" block (was the 2026-11-25 temporary workaround) updated.
- **From → To:** `ImpactType` = sign of `value` (`85899a52`) → `value` is a magnitude, `impactType` is authored (GDD AB-E12).
- **Why:** owner decision 2026-11-26 (session item A1).
- **Assets:** none edited. Assets saved before have no `impactType` key on their rows and load as Recovery — every row in the project today is positive and meant as Recovery. `Gain For Time Effect.asset` (MagicDamage +100000, test value) is clamped to 999. Rows authored 0 (`Recovery Stat Effect`, both `Paladin Recovery Reduction Stats Effect`) now act as 0.1.
- **Evidence:** STATIC — not compiled, not run. To check in Unity: scripts compile; EditMode `ItemEffectDataTests`, `StatSheetTests` pass; a Reduction row lowers the stat / current HP.
- **Bugs:** none opened or closed.

## 2026-11-26 — Delivery / Payload, SelfApplyEffect, dead guard
- **Commit:** `85899a52` (`sprint-24`)
- **Changed:** new `Core/DeliveryEffectBase.cs` (holds `SubEffects` + `ApplyOnHitEffects`, both moved out of `AbilityEffectDefinition` / `SpawnEffectBase`), `Core/PayloadEffectBase.cs`, `Effects/SelfApplyEffect.cs`. `SpawnEffectBase : DeliveryEffectBase`; `DamageEffect`, `StatsEffectBase : PayloadEffectBase`. `DamageEffect`: `Reset()` → `Recipient = Target`, `OnValidate` warns on `Caster`, no target → silent return; `AbilityEffectDefinition.recipient` is `protected`. `EffectReceiverBase.IsDead` guards the four stat / vital entry points. Assets: `Paladin Blessing Self Apply Effect.asset`, `Avatar of Light Self Apply Effect.asset` (new); `Blessing.asset`, `Avatar of Light.asset` `Effects` → that one asset; their four payload assets `recipient` 0 → 1. README: code table, "Delivery and Payload" block, two Open issues rows. Also in the working tree, not part of this decision set: `StatModifier.impactType` removed (session item A1, owner still checking).
- **From → To:** see `design/gdd/changelog/abilities-v2-system.CHANGELOG.md` 2026-11-26.
- **Why:** owner decisions 2026-11-26 (session).
- **Evidence:** STATIC — no Unity in this session; not compiled, not run. To check in Unity: scripts compile; the 8 spawn effect assets still show their `SubEffects`; Blessing / Avatar of Light still buff the caster (GDD AC-49).
- **Bugs:** none opened or closed.

## 2026-11-25 — DamageEffect: ability damage through IEffectReceiver
- **Commit:** `b001d78e` (`sprint-24`)
- **Changed:** new `DamageEffect` SO (`Effects/DamageEffect.cs`); `IEffectReceiver.ApplyDamage` + `EffectReceiverBase.ApplyDamage` (forwards to the hurtbox `TakeDamage`); `SpawnProjectileEffect` / `SpawnSummonEffect` lose `baseDamage` and their damage code; `DamagesInManyPos` uses the callback and `DespawnOneSelf()`, `damagePerArrow` removed; `EffectReceiver` component moved onto the hurtbox GameObject in `PlayerTest.prefab` and `EnemyPrefab.prefab` (its own GameObject removed); `AbilityContext.TryGetRecipientComponent` resolves a Target on the hit collider first; `TrainingDummy` implements `IEffectReceiver`. Eight `<Ability> Damage Effect.asset` created and added to the `SubEffects` of the eight spawn effect assets.
- **From → To:** each spawn effect / spawned object computed its own damage (flat, `+ MagicDamage`, or fixed 10) and called `TakeDamage` → only `DamageEffect` deals hit damage (`baseDamage + coefficient × stat`), through `IEffectReceiver`; HP loss without a hit reaction stays on the `AdjustCurrent…` effects. Numbers unchanged.
- **Why:** owner decision 2026-11-25 (GDD AB-S7, AB-S11).
- **Evidence:** STATIC — no Unity in this session; not compiled, not run.
- **Bugs:** BUG-109 items 2 and 3 addressed in code (to verify); NOTE-20261019-4 addressed (to verify).

## 2026-11-25 — Status & stat effect decisions (documentation only)
- **Commit:** `b001d78e` (`sprint-24`)
- **Changed:** no code. README gains "Status & stat effects — decisions 2026-11-25 `[PLANNED]`" and six Open issues rows; GDD §7 gains AB-E8…AB-E14; `design/balance/status-debuff-stat-combos.md` header says the config is reference only.
- **From → To:** `StatusType` chosen on each stat effect, stats auto-filled from `Status Status Config.asset`, direction from the sign of the value (stat) or `ImpactType` (vital) → one `StatusType` per `AbilityDefinition`, passed at runtime through `AbilityContext`; `ImpactType` derived from the status; stats picked by hand from the balance doc; values always positive, signed once in the receiver (temporary); tick effects use a new `AdjustCurrentPerTimeForDuration`; a mixed status (Burn, Poison) = two effects in one ability; item effects get their own `StatusType`.
- **Why:** owner decisions 2026-11-25 (session). The status config bundled vital and stat reductions in one status (Burn, Poison), which no single effect class can apply.
- **Bugs:** none opened or closed. Findings listed in README Open issues (2026-11-25 rows), for the next triage.

## 2026-11-16 — ShootSpirit assets removed (BUG-073, BUG-090)
- **Commit:** `e6c826e5` (assets) and the follow-up commit on `origin/feature/fix-bug-sprint-23` (prefab, warnings, docs).
- **Changed:** deleted `Assets/SO/Skill/ShootSpirit/` and `Assets/Prefab/Bullet/SpiritProjcetile.prefab`; `PlayerTest.prefab` `abilityBindings` → `[]`; `SpawnEffectBase` warnings reworded. README Assets row and Open issues row updated.
- **From → To:** legacy ShootSpirit proof-of-wiring assets with missing-script references → gone; warnings named the deleted `ShootSpiritOrbEffect` / `SpiritOrbProjectile` → print the effect type and asset name.
- **Why:** owner request (2026-11-16).
- **Bugs:** BUG-073, BUG-090 — triage hint Resolved (NOTE-20261116-5).

## 2026-11-13 — Abilities v2 GDD written (documentation only)
- **Commit:** not committed (working tree on `sprint-22`, base `origin/sprint-22` `a8725a77`)
- **Changed:** no code. New `design/gdd/abilities-v2-system.md`; README header gains a **Design** pointer, the "No GDD" row and the Related line are updated, and a dated note records two bug rows that read differently in the files now (BUG-072, BUG-109).
- **From → To:** README said "No GDD for v2" and pointed at `skill-ability-system.md` (v1 only) → points at `abilities-v2-system.md`; `skill-ability-system.md` is marked superseded.
- **Why:** QA needs a checkable source for ability test cases (demo checklist item 21).
- **Bugs:** none opened or closed. Bug-inbox notes NOTE-20261113-6 … -11.

## 2026-11-05 — Abilities v1 deleted (doc-truth re-sync)
- **Commit:** `6094aa10` "fix bug, add asset room, enemy, data" (2026-11-04, merged into `sprint-21` by `ea00b3a4`); Archer / Mage assets `6c61ccdb`
- **Changed:** `System/Skill_Ability/` deleted (`ActivateSkill`, `AbstractSkillSO`, Dash / Slash / Block / Dual, `EffectSkill*`, `InternalSkillSO`) with its SO assets, plus `EntityWeapon`, `Spell`, `IEffectable`; `WeaponSO` moved to `System/Item/`. `AbilityHolderBase.SetAbility()` now clears `_equipped` and `currentAbility` first; new `ChangeNextState()`. `EffectReceiverBase.AdjustCurrentForDuration` / `AdjustCurrentPerTimeForDuration` renamed `AdjustMaxForDuration` / `AdjustMaxPerTimeForDuration` (the latter still throws, BUG-118).
- **From → To:** README status said "v1 deleted 2026-11-04 (working tree)" → committed on `sprint-21`; assets row lists Archer / Mage / ShootSpirit.
- **Why:** not recorded in the commit.
- **Bugs:** BUG-106 (enemy half), BUG-118.

## 2026-10-31 — Summon renames + Rain Arrows reach the sprint branch (doc-truth re-sync)
- **Commit:** `cae75317` "add asset", `5c8245c9` "add logic for rain arrow skill", `6e484f0c` "update logic effect" (on `origin/sprint-20`)
- **Changed:** `LightningController` → `DamageArea`, `RuneCircleController` → `CircleSummon`; `DamagesInManyPos` + `RainArrowsProjectile` added; `Runtime/ImpactMono/SourceEffect` added. Stat effects now apply through `IEffectReceiver` (`EffectReceiverBase`) instead of `IVitalComponent`.
- **From → To:** README runtime list named the old classes → names the current ones.
- **Why:** not recorded in the commit.
- **Bugs:** BUG-072 (now `DamageArea`), BUG-109 (Rain Arrows), BUG-117, BUG-118.

## 2026-09-28 — Compile restored (BUG-092 partial)
- **Commit:** `5b035b73` (verified with `git log -S"private float perTime"`)
- **Changed:** `private float duration;` → `private float perTime;`
- **From → To:** field `perTime` missing, two `CS0103` errors → field declared, project compiles
- **Why:** fix the build break from `c0067f4`.
- **Bugs:** BUG-092 build break fixed; serialization defect (`perTime`, `timeCount` private, unserialized) still open.

## 2026-09-28 — Shared projectile body for abilities and weapons
- **Commits:** `5b035b73`, `b7a0af5e`, `7c637c0e`
- **Changed:** `SpawnProjectileBase` rewritten; `ProjectileBody` + `ProjectileConfig` added; `IProjectilePayload` added; `SpawnEffectBase.dir` moved to `SpawnProjectileEffect`
- **From → To:**
  - `SpawnProjectileBase` owning `Rigidbody2D` + `CircleCollider2D` and its own trigger → `[RequireComponent(ProjectileBody)]`, implements `IProjectilePayload.OnHit()`; serialized `targetMask`, `blockMask`, `pierceCount`
  - Lifetime despawn in `SpawnMono` → owned by `ProjectileBody` (`SpawnMono` gets lifetime 0)
  - No target filtering → `targetMask` / `blockMask` filtering, release on hit
- **Why:** "update architecture projectile and logic spawn by weapon and ability" — one projectile for both paths.
- **Bugs:** closes the two BUG-075 follow-ups (no despawn on hit, no layer mask).

## 2026-09-25 — ADR-0005 damage contract change
- **Commits:** `8c3c350a`, `5007d3f6`, `83954bc6`, `4faf6233`, `b0b13379`, `c0067f47`
- **Changed:** `IAbilityServices`, `AbilityHolderBase`, effect lists
- **From → To:**
  - `IAbilityServices` = Pool / Stats / ResourceReceiver / Vital / NegativeReceiver → **Pool only**; hit target via `AbilityContext.Target`
  - Player-only `AbilityHolder` → `AbilityHolderBase` shared with `EntityAbilityHolder`
  - Abilities required an equipped weapon → independent of the weapon
  - `Utility.ModifierStatsCalculate` returned the total → returns the delta
  - `AbilityEffectDefinition` lists without initialisers → all `= new()`
  - `RecoveryReductionPerTimeForDuration.perTime` deleted (build break)
- **Why:** one `AbilityDefinition` for player or enemy.
- **Bugs:** closes BUG-074, BUG-082, BUG-068 (`GetAbility()` half); opens BUG-092; BUG-071 → PARTIAL.

## 2026-09-22 — Single gate; v2 damage path restored
- **Commits:** `723fab1a`, `2a83469a`, `8b23174b`, `379c267`
- **Changed:** `AbilityDefinition.TryStart()`; `Casting()` → `TryCast()`; conditions deleted; `SpawnProjectileBase` 2D trigger; `LightningController` overlap query
- **From → To:**
  - Three condition walks + ungated `TryPayCost()` → one generic gate `TryStart()` over `StatType`
  - `HasEnoughManaCondition` / `NotDeadCondition` → deleted
  - 3D `OnTriggerEnter(Collider)` → `OnTriggerEnter2D(Collider2D)`
  - Lightning target query as comments → `OverlapCircleNonAlloc`, set + invoke per target
- **Why:** BUG-076 / BUG-077 / BUG-085 / BUG-075 / BUG-072.
- **Bugs:** closes BUG-075, BUG-076, BUG-077, BUG-085, BUG-091; BUG-072 code complete (prefab mask open).

## 2026-09-15 … 09-21 — Effect layer replaced; Paladin set
- **Commits:** `8295539` … `7cceda2e`, `73ab8e77`, `e2cb75e4`, `e02bf3bc`, `edd7454d`
- **Changed:** effect and runtime layers rebuilt
- **From → To:** `DamageInFront` / `LungeForward` / `ShootObject` / `PlayDebugLog` effects + `SpiritOrbProjectile` → `SpawnEffectBase` / `StatsEffectBase` hierarchies + `SpawnMono` controllers; enum `SkillState` → `AbilityState` (no `None`); two disjoint cost lists
- **Why:** Paladin ability set (Consecrate, Blessed Slash, Blessing, Avatar of Light).
- **Bugs:** BUG-075 … BUG-079 filed.

## 2026-09-09 — Promoted from prototype
- **Commits:** `9b8d40f`, `5c7afba`
- **Changed:** 17 files moved from `prototypes/skill-enhance-abilities/` to `Assets/Script/System/Abilities/`; `AbilityHolder` rewritten to drive v2
- **From → To:** player abilities via v1 `ActivateSkill` → via v2 `AbilityDefinition`
- **Why:** composition over inheritance for abilities.
- **Bugs:** none at the time.
