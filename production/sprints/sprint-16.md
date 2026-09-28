# Sprint 16 — 2026-09-28 to 2026-10-02

**Status: OPEN.** Opened 2026-09-28 (`pm-weekly-kickoff`, autonomous run — no owner present; the run
fired on Monday, so the sprint starts today).
Branch `sprint-16`, created from `sprint-15` tip (`5b035b7`, "range weapon").
`gh` CLI unavailable (`gh: command not found`) — draft PR **not** auto-created; run manually:

```bash
gh pr create --draft --base sprint-15 --head sprint-16 --title "Sprint 16"
```

**Review mode:** lean — producer feasibility gate skipped.

---

## Sprint Goal

**Make the project safe to play-test, then play-test it.** HEAD compiles again (BUG-092 compile half
fixed), the ADR-0005 refactor has landed, and the v2 damage path is code-complete — but nothing has
been verified in Play Mode for nine sprints. This sprint: (1) close the small crash/lock defects on the
live combat path (vitals guard, death state), (2) confirm the new ranged weapon fires (DI is now spawn-time),
(3) run **one** Play Mode smoke session over the four Paladin abilities, melee, and the new ranged
weapon, and record the result.

---

## Capacity

- Total days: 5
- Buffer (20%): 1 day
- Available: 4 days

Must-Have load ≈ **1.05d**. Planned-work velocity last sprint was ≈0.6d, so the Must set is kept small
on purpose; the remaining capacity is **explicitly reserved for the owner's feature stream** rather than
filled with tasks that history says will not move.

---

## Carryover from Sprint 15

Full detail: `sprint-15.md` (Closure block), `production/qa/open-issues-2026-09-25.md`.

| Carried item | Times carried | Note |
|---|---|---|
| BUG-065 (`PlayerDeathState.Enter()` stop movement) | 6th | |
| BUG-066 + BUG-070 (vitals dictionary guard) | 6th / 4th | Now ONE site: `VitalStatsBase.cs` |
| BUG-064 item 7 (RangeWeapon DI) | 6th | Code done `e1dcee0` + `5b035b7`; Play Mode verify only |
| BUG-071 (HoT/DoT no stop handle) | 2nd | Partial |
| BUG-072 `layerMask` on `Lightning.prefab` | 2nd | Owner Inspector step, ~5 min |
| BUG-073 + BUG-090 missing-script assets | 2nd | Bundle |
| Play Mode smoke | **9th** | Now unblocked — HEAD compiles |
| Review-gate decision | 4th | |
| Pre-push hook / TD-048 compile check | 26th+ | Two compile breaks committed in four days |
| TD-040 ADR (Abilities v1 vs v2) | 3rd | Partly overtaken by ADR-0005 (enemies now cast v2) |
| First EditMode test (TD-014) | 31st+ | Blocked by BUG-084 |
| BUG-063 | — | **Not carried** — owner accepted/deferred to demo prep (2026-09-22) |

---

## Tasks

### Must Have (Critical Path)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S16-01 | **BUG-092 residual** — serialize `perTime` and `timeCount` on `RecoveryReductionPerTimeForDuration` (both are private, unserialized → `0` on every asset); delete unused `duration` | gameplay-programmer | 0.05 | None | Both fields visible in Inspector; a DoT asset ticks N times |
| S16-02 | **BUG-066 + BUG-070** — guard `currentStats[statType]` in `VitalStatsBase.cs:28,39,41,45,52,54,58`. Audit `Reborn()` seeding so a missing HP key cannot read as `0` = death | ai-programmer | 0.15 | None | No `KeyNotFoundException` from `IVitalComponent`; missing HP key logged, not lethal |
| S16-03 | **BUG-065** — `PlayerDeathState.Enter()` stops `PlayerMovement` | gameplay-programmer | 0.1 | None | Player does not slide during death animation |
| S16-04 | **BUG-086** — `PlayerDeathState.LogicUpdate()` consumes `Status` so `ON_PLAYER_DEATH` fires once | gameplay-programmer | 0.1 | S16-03 (same file) | Event emitted exactly once per death |
| S16-05 | **BUG-064 item 7 close-out** — code done (`e1dcee0` + `5b035b7`, spawn-time injection at `WeaponHolderBase.cs:60`). `resolver?.` is silent when the holder itself was never injected, so confirm in Play Mode that the ranged weapon fires for the player, then close the bug file | owner (Kay) | 0.05 | S16-07 session | Ranged weapon fires; BUG-064 closed |
| S16-06 | **BUG-072** — set `layerMask` on `Lightning.prefab` | owner (Kay) | 0.05 | None | Consecrate lightning removes enemy HP |
| S16-07 | **Play Mode smoke (9th carry → do it or sign it)** — one session: 4 Paladin abilities, melee, ranged weapon, enemy weapon attack, player death. Log in `production/qa/playtests/` | owner (Kay) | 0.3 | S16-01..06 | Written result per item, or a signed accepted-risk note in this file |
| S16-08 | **TD-048** — pre-push compile check (or, at minimum, the placeholder hook + a written "open Editor before push" rule) | producer / owner | 0.1 | None | Hook file exists and is documented |
| S16-09 | **BUG-071 residual** — keep a coroutine handle so HoT/DoT stops on death / disable | gameplay-programmer | 0.15 | S16-02 (same file) | DoT does not survive death or pooled respawn |

Must-Have total ≈ **1.05d**.

### Should Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S16-10 | BUG-073 + BUG-090 — remove/repoint missing-script refs (`ShootSpirit.asset`, `SpiritBomd.asset`, `Has Enough Mana Condition.asset`) | owner (Editor) | 0.15 | None | No missing-script warnings |
| S16-11 | BUG-068 `:28` — null-guard `CurrentActivationType` (enemies now run it on pooled objects) | gameplay-programmer | 0.05 | None | No NRE on a freshly spawned enemy |
| S16-12 | Review-gate decision — record policy for multi-file refactors like ADR-0005 landing mid-sprint | producer / owner | 0.1 | None | Written decision here |
| S16-13 | TD-040 ADR — v1 vs v2 (re-scope: ADR-0005 already moved enemies onto v2) | technical-director | 0.3 | None | ADR under `docs/architecture/` |
| S16-14 | BUG-087 design — `GameManager` + `ON_PLAYER_DEATH` subscriber + player `Reborn()` (design only this sprint) | game-designer / producer | 0.3 | S16-04 | Short design note / story |

Should-Have total ≈ 0.9d. Must + Should ≈ 1.95d of 4.0d.

### Nice to Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S16-N1 | BUG-084 decision — where tests live, runtime + test `.asmdef` | technical-director | 0.3 | None | Decision recorded |
| S16-N2 | BUG-083 — `HoldTime`/`HoldRatio` always `0f` (relevant if Consecrate goes Hold) | gameplay-programmer | 0.2 | None | Values update while held |

### Reserved

≈1.6d reserved for owner-driven feature work (ranged weapon, ADR-0005 follow-ups). Record it in the
tracker as it lands so velocity reflects reality.

---

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| Third compile break in two weeks (no pre-push check) | Medium | High | S16-08; open the Editor before every push |
| `WeaponHolderBase.cs:60` uses `resolver?.InjectGameObject` — a holder never injected fails silently and the ranged weapon never fires | Low-Medium | Medium | S16-05 in the smoke session |
| Play Mode smoke slips a 10th time | High | High | S16-07 accepts a signed accepted-risk note as a valid close |
| Must set loses to feature work a 6th time | High | Medium | Must kept at 1.05d, all items ≤0.15d except the smoke |
| No `gh` CLI | Known | Low | Manual PR command above |

---

## Definition of Done

- [ ] S16-01..S16-05, S16-09 committed
- [ ] S16-06 Inspector step done
- [ ] S16-07 smoke logged or accepted-risk signed
- [ ] S16-08 hook/rule exists
- [ ] QA plan gate resolved (below)

## QA Plan

No QA plan exists for Sprint 16 (`production/qa/qa-plan-sprint-16.md` not found) — 33rd+ consecutive
cycle. S16-07 is the de facto QA gate this sprint. Deferred to owner.
