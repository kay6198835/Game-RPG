# Dev change notes for QA automation — 2026-11-28

**From:** owner (dev), via `sprint-24` (commit `bb142acc`, reviewed on branch `test-status`, PR #71).
**To:** the Friday QA automation task and whoever maintains `Assets/QA/` and `qa-automation/`.
**Why:** (1) timed stat effects now go through a status registry (B6). (2) The owner asks QA to check five
latent effect-flow issues and report their real state, so they can be fixed or closed on evidence.

Evidence tier: STATIC (read from code at `bb142acc`). Nothing below was run in Unity; `bb142acc` has not been
compiled yet — a compile error there is the first thing to report.
Bug status is still changed only by the Saturday triage; this file does not close any bug.

## 1. Status registry (B6, GDD OQ-4) — what changed

- New file `Assets/Script/System/Abilities/Core/ActiveStatus.cs`: `StatusRequest` (struct) and `ActiveStatus`.
- New `IEffectReceiver.ApplyStatus(StatusRequest)`. `EffectReceiverBase` keeps one `List<ActiveStatus>` and one
  `Update()`; no coroutine, no pooled `SourceEffect` per timed status.
- **Key** = (`ctx.AbilityInstance`, effect asset).
  - Same key → only the end time is refreshed (`EndTime = now + Duration`). Modifiers are not added again; the
    tick phase (`NextTick`) is kept.
  - Other key → its own status. Statuses coexist and add up. No cap, no floor (owner: none during the test
    phase; balance after the demo).
  - No `AbilityInstance` (items, legacy calls) → the key never matches → an independent status every call.
- **Modifier source** on the `StatSheet` is the `ActiveStatus` object itself (not a `SourceEffect`).
- **Expiry:** `Update()` removes the status at `EndTime` by its own source. **Cleanup:** `OnDisable()` and
  `ReloadComponent()` end every status by its own source (AB-E20); weapon / item modifiers are not touched.
- **Ticks:** `RecoveryReductionPerTimeForDuration` now works (before: empty body). One tick on the first `Update`
  after the apply, then every `perTime` seconds, `timeCount` ticks in all (`Duration = perTime × timeCount`).
  At most one tick per frame.
- `BuffDebuffStatsForDuration` → `ApplyStatus` with `MaxModifiers`; `RecoveryReductionPerTimeForDuration` →
  `ApplyStatus` with `TickModifiers`. `AdjustCurrentStats` (instant) and `AdjustMaxStats` (permanent, items)
  are unchanged and never enter the registry. `AdjustMaxStats` still spawns a `SourceEffect` that is never
  released (known, Q5 open).
- **Legacy wrappers kept:** `AdjustMaxForDuration(group, duration, ctx)` and
  `AdjustMaxPerTimeForDuration(group, perTime, duration, ctx)` build a request with no effect asset → an
  independent status every call. For the second one, `duration` is now **seconds** (it used to be passed a tick
  count by the effect, which no longer calls it).
- `QAProbeTarget` and `TrainingDummy` got an empty `ApplyStatus` (only change in `Assets/QA/`).
- A dead receiver (`input.IsDeath`) refuses `ApplyStatus`; its live statuses run on until the despawn clears them.

### 1.1 Existing cases — expected to still pass, comments are stale

- **QA-ENT-13** (`QAEnemySuite.Behaviour.cs:524-532`), **QA-ENT-16** (`:559-574`), **QA-ITEM-09**
  (`QAEnemySuite.Items.cs:452-460`) call `AdjustMaxForDuration` with no `AbilityInstance`, so each call is an
  independent status as before. Expected: PASS unchanged.
- Their comments say the timed buff uses "its own pooled SourceEffect" (`cfb6a230`). Since `bb142acc` the source
  is an `ActiveStatus`; `QA-ENT-16`'s step "a second timed buff on a reused source" no longer reuses anything.
  Update the comments / step names; the assertions stay valid.
- `QAProbeTarget.ApplyStatus` is a no-op: ability cases that hit the probe will not show stat statuses on it.

### 1.2 New cases to add (suggested IDs, rename as needed)

A real `AbilityInstance` is needed for the key: take one from a character's holder
(`AbilityHolderBase.GetAbility(slot)`), or build one with `new AbilityInstance(definition, owner, services)`.
Build the request directly, or apply a real `BuffDebuffStatsForDuration` / `RecoveryReductionPerTimeForDuration`
asset through an `Instantiate` copy (as QA-ENT-17 does), with `ctx.AbilityInstance` set.

| ID | Steps | Expected |
|---|---|---|
| QA-STS-01 | Same instance + same asset applied twice, 0.2 s apart | stat changed once; one modifier source more than before; expires `Duration` after the **second** apply |
| QA-STS-02 | Two different instances, same asset | stat changed twice (stacked); each expires on its own time |
| QA-STS-03 | Same instance, two different assets on the same stat | two statuses, both applied |
| QA-STS-04 | No `AbilityInstance` (legacy call) twice | two independent statuses (current QA-ENT-16 behaviour) |
| QA-STS-05 | Tick asset `perTime = 0.2`, `timeCount = 3` on HP | exactly 3 ticks; HP change = 3 × tick |
| QA-STS-06 | As 05, re-applied with the same key at t ≈ 0.3 | tick spacing stays 0.2 s (phase kept); more ticks until the new end (≈ 0.3 + 0.6) |
| QA-STS-07 | Enemy with a timed buff → despawn → respawn | after respawn the stat equals the clean value; no modifier source left from the old life |
| QA-STS-08 | Player with a weapon modifier on stat X + timed buff on X; let the buff expire; also call `ReloadComponent` | weapon modifier still there in both cases |
| QA-STS-09 | `ApplyStatus` on a dead receiver | refused, stat unchanged |
| QA-STS-10 | Tick that kills the receiver (HP ≤ tick) | no exception in the console; no tick after death |

## 2. Latent effect-flow issues — please check and report the real state

Report each one as **REPRODUCED / NOT REPRODUCED / CANNOT TEST**, with the evidence tier and the exact
observation (console lines, counts, values). Do not fix code.

### 2.1 Failed `TryCast` still applies the effect

- Code: `AbilityInstance.Casting` runs `effect.TryCast(ctx)` for each effect; on a fail it only calls
  `CancelHold`. `AbilityInstance.Execute` (Do phase) then calls `Apply` on **every** effect with no
  "cast ok" gate. On a fail the effect's own `Costs` are not paid.
- No effect asset in `Assets/SO/Skill/**/Effect` has `SubConditions` or `Costs` today, so it does not show in
  play. Test with an `Instantiate` copy of an ability whose effect has a failing `SubCondition` (or an
  unaffordable `Cost`).
- Expected if the issue is real: the effect still applies / spawns at the Do phase, and its cost is not paid.

### 2.2 `ctx.Target` is not restored after a multi-hit callback

- `SelfApplyEffect` restores `ctx.Target` after its callback; `DamageArea` and `DamagesInManyPos` set
  `ctx.Target = hit` for each hit and leave the **last hit** in it.
- Check: after one `DamageArea` hit on two probes, read `ctx.Target` (the ability's context) — expected if real:
  it is the last probe hit, not the value before the hit. Report whether any later effect of the same cast reads it.

### 2.3 Spawn VFX despawn on the exact last key (CircleSummon, RuneCircle, Lightning)

- The `DespawnOneSelf` animation event sits on the last key with no trailing frame and no Exit fallback. A spawn
  with `Lifetime > 0` also despawns from a coroutine, so both can fire.
- Check (Rain Arrows, Consecrate, any summon using these prefabs): each spawned object returns to the pool
  **exactly once**; no frozen last frame left in the scene; no pool warning / double-release message; the pool's
  active count goes back to its value before the cast.

### 2.4 Duplicated animation-event function warning (`Execute`)

- Spawn monos expose more than one method named `Execute` reachable from an animation event.
- Check: Editor.log / console during the ability cases — count warnings that name an animation event `Execute`
  (multiple functions / ambiguous). Report the exact text and which clips / prefabs.

### 2.5 Animation clip timing (static scan, optional)

- Scan `Assets/**/*.anim` (575 clips): list clips whose `m_SampleRate` is not 30, and clips with an animation
  event at time = clip length (event on the last key). Report counts and paths; no change to the clips.

## 3. Addendum (same day, after `78270214`) — cleanup on death, `SourceEffect` removed

Evidence: STATIC, not compiled.

- **Statuses are cleared only on death** (owner rule, GDD AB-E20 revised): `EffectReceiverBase.Update` ends every
  status when `IsDead`; `OnDisable` no longer clears. `ReloadComponent` (respawn) still clears. A disable that is
  not a death keeps the statuses; end times keep running in game time, so a status that expired while inactive
  ends on the first `Update` after re-enable (at most one late tick).
  - QA-STS-07 still holds (despawn after death → clean; respawn → clean). Add: **disable / enable without death**
    shorter than the remaining time → the status is still there; longer → it ends on the first frame after re-enable.
  - QA-STS-10: after the killing tick, the next `Update` clears every status.
- **`SourceEffect` is deleted** (script + prefab). `AdjustMaxStats` now uses **the receiver itself** as the
  modifier source: every permanent (item) modifier of a character shares one source.
  - QA-ENT-13 reads the source back from the added modifier → it is the receiver; removing it removes **every**
    permanent modifier of that character (fine on a clean enemy).
  - QA-ENT-16: two `AdjustMaxStats` calls still stack (+10); the "reused source" step has nothing pooled any more.
  - QA-ITEM-09 cleans up with "sources that are new in this case": if the receiver was already a source before
    the case, it is not new, so the item buff is **not removed** and can leak into the next case. Use a fresh
    character, or `RemoveModifiersFromSource(receiver)` explicitly.
  - No more pooled `SourceEffect` objects: a case that counted live pooled objects per `AdjustMaxStats` call
    should expect zero.
