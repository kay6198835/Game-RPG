# Sprint 16 — Daily Plan & Progress Tracker

> **Sprint**: 2026-09-28 (Mon) -> 2026-10-02 (Fri)
> **Companion to**: `sprint-16.md` — day-by-day breakdown + live tracker
> **Daily routine: 10:00 → `/daily-standup`** · Sat 22:00 `/weekly-wrapup` · Sun 22:00 `/weekly-kickoff`
> **Opened**: 2026-09-28 (kickoff, autonomous). Branch `sprint-16` from `sprint-15` tip (`5b035b7`).
> Sprint 15 closed PARTIAL (Must-Have 1/9, ≈0.6d planned velocity, large off-plan output). See `sprint-15.md`.

---

## Status Verdict

**ON TRACK (day 0).** Nothing started. HEAD compiles; working tree clean.

## Burn Summary

| Bucket | Est. | Done | Remaining |
|--------|------|------|-----------|
| Must Have | 1.1d | 0 | 1.1d |
| Should Have | 0.9d | 0 | 0.9d |
| Reserved (owner feature work) | ≈1.6d | — | — |

## Task Estimates

| ID | Task | Est. | Status |
|----|------|------|--------|
| S16-01 | BUG-092 residual (serialize `perTime`/`timeCount`) | 0.05d | NOT STARTED |
| S16-02 | BUG-066+070 vitals guard | 0.15d | NOT STARTED |
| S16-03 | BUG-065 stop movement on death | 0.1d | NOT STARTED |
| S16-04 | BUG-086 death event once | 0.1d | NOT STARTED |
| S16-05 | BUG-064-7 RangeWeapon DI verify | 0.15d → 0.05d | CODE DONE — Play Mode verify only (see 09-28 standup: `RegisterComponentInHierarchy<RangeWeapon>` was already removed in `5b035b7`) |
| S16-15 | BUG-093 projectile aim regression (added 09-28 standup) | 0.05d | NOT STARTED |
| S16-06 | BUG-072 `Lightning.prefab` layerMask | 0.05d | NOT STARTED |
| S16-07 | Play Mode smoke | 0.3d | NOT STARTED |
| S16-08 | TD-048 pre-push check | 0.1d | NOT STARTED |
| S16-09 | BUG-071 coroutine handle | 0.15d | NOT STARTED |
| S16-10..14 | Should Have | 0.9d | NOT STARTED |

---

## Day-by-Day Breakdown

### Mon 2026-09-28 — Crash/lock fixes on the live path
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S16-01 BUG-092 residual | 0.05d | NOT STARTED | Every HoT/DoT asset is silently broken; 2 lines |
| S16-02 BUG-066+070 | 0.15d | NOT STARTED | One site now; on the live death path |

### Tue 2026-09-29 — Death path
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S16-03 BUG-065 | 0.1d | NOT STARTED | Same file as S16-04 |
| S16-04 BUG-086 | 0.1d | NOT STARTED | Must precede any `ON_PLAYER_DEATH` subscriber (BUG-087) |
| S16-09 BUG-071 residual | 0.15d | NOT STARTED | Same file as S16-02 |

### Wed 2026-09-30 — Editor day + smoke
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S16-06 BUG-072 layerMask | 0.05d | NOT STARTED | Needed for the smoke to mean anything |
| S16-10 BUG-073+090 assets | 0.15d | NOT STARTED | Same Editor session |
| S16-07 Play Mode smoke (+ S16-05 ranged weapon check) | 0.35d | NOT STARTED | 9th carry; all preconditions done by now |

### Thu 2026-10-01 — Process + decisions
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S16-08 TD-048 hook | 0.1d | NOT STARTED | Two compile breaks in four days |
| S16-12 review-gate decision | 0.1d | NOT STARTED | |
| S16-11 BUG-068 `:28` | 0.05d | NOT STARTED | |

### Fri 2026-10-02 — Design + buffer
| Task | Est. | Status | Why now |
|------|------|--------|---------|
| S16-14 BUG-087 design note | 0.3d | NOT STARTED | Unblocked once S16-04 lands |
| S16-13 TD-040 ADR | 0.3d | NOT STARTED | |

---

## Risks (live)

- Ranged weapon is injected at equip via `resolver?.InjectGameObject` (`5b035b7`) — silent if the holder
  itself was never injected; confirm in the smoke (S16-05).
- Play Mode smoke at its 9th carry — accept a signed accepted-risk note rather than a 10th carry.
- No pre-push compile check; two compile breaks landed in four days.
- `gh` unavailable — draft PR for `sprint-16` not opened.

---

## Daily Log

_(appended by `/daily-standup`)_

### Mon 2026-09-28 - Daily Standup (autonomous, no owner present)

Branch `sprint-16` (opened by the kickoff run that executed concurrently with this standup), HEAD
`5b035b7`. No Thu 09-24 / Fri 09-25 standups were logged in Sprint 15.

**Since last standup (Wed 09-23 → Mon 09-28), assessed against source:**
- 09-24/25 ADR-0005 Amendments 1-3 — closed BUG-043/074/080/081/082/088/091; introduced BUG-092.
- `e1dcee0` (09-25) + `5b035b7` (09-28) — **range weapon feature**, off-plan, 27+3 files, no `/code-review`:
  - ✅ BUG-064 item 7 code-complete: `RangeWeapon.Construct(IObjecPoolService)` `[Inject]`; every weapon is
    injected on equip by `WeaponHolderBase.Equid_UnEquid()` → `IObjectResolver.InjectGameObject()`.
    ⚠️ **Correction to the kickoff risk:** the `RegisterComponentInHierarchy<RangeWeapon>()` line from
    `e1dcee0` was **removed** in `5b035b7` — no `Awake()` throw risk at HEAD. S16-05 shrinks to a Play Mode check.
  - ✅ BUG-092 compile half fixed (`perTime` declared). Residual unchanged → S16-01. BUG-092.md updated.
  - Undocumented architecture change: **Player is spawned at runtime** by `PlayerManager.SpawnPlayer()`
    (`resolver.Instantiate`); `GameLifetimeScope` no longer registers `Player`/`StatHandler`/`AbilityHolder`;
    `IPlayerStatService` comes from `PlayerManager.StatService`. CLAUDE.md DI section, ADR-0004 and the
    `AbilityHolder.cs:1` comment are now stale → needs `/doc-sync`.
  - v1 trimmed: `WeaponStats.AbilityWeapon/SkillWeapon`, `AttackSO.ability` deleted,
    `Weapon.currentAbilitySO` commented out (`EntityWeapon.currentAbilitySO` remains). Feeds S16-13.
  - 🐞 **New BUG-093 (S2)** — `SpawnEffectBase.cs:19-22` commented out the `Forward → dir` computation;
    `SpawnProjectileEffect.Angle()` falls back to `Vector2.right` and the value persists on the shared SO,
    so Blessed Slash always fires right. Filed `production/qa/bugs/BUG-093.md`, tracked as S16-15.

**Today (Mon 2026-09-28), estimates:**

| # | Task | Est. | Complexity / Risk |
|---|------|------|-------------------|
| 0 | Open Unity, confirm compile green (BUG-092 fix verified by static read only) | 5min | Gate for everything |
| 1 | S16-15 BUG-093 restore `dir` from `context.Forward` | 0.05d | Trivial; S2 regression from today's commit |
| 2 | S16-01 BUG-092 residual `[SerializeField]` ×2, delete nothing else | 0.05d | Trivial |
| 3 | S16-02 BUG-066+070 guard `VitalStatsBase` (7 sites) + audit `Reborn()` seeding | 0.15d | Low-Med; live death path |
| 4 | S16-05 Play Mode: ranged weapon fires, no VContainer exception | 0.05d | Owner-in-Editor |

Total ≈ 0.3d. Day-0 status: 0 Must-Have done; S16-05 mostly pre-done.

**Blockers:** all verification needs an owner Unity session; BUG-084 still blocks any EditMode test;
`gh` unavailable.

**Risks:**
- Third silent/compile regression in 7 days (BUG-088, BUG-092, BUG-093), none caught pre-push → S16-08 (TD-048).
- Runtime-spawned Player: any scene without a `PlayerManager` whose `playerPrefab` is set has no player;
  `Camera.main` is re-parented inside `SpawnPlayer()` (NRE if no MainCamera tag). Undocumented.
- Feature stream keeps landing un-reviewed multi-file changes (S16-12 decision).

## Carry-over Watchlist

- Play Mode smoke: 9th sprint
- BUG-065 / BUG-066: 6th carry
- Pre-push hook: 26th+ carry
- QA plan: 33rd+ cycle — owner decision
- First EditMode test: 31st+ — blocked by BUG-084
