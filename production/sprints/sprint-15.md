# Sprint 15 — 2026-09-21 to 2026-09-25

## Closure — 2026-09-28 (weekly kickoff, autonomous)

**Status: COMPLETE — closed PARTIAL.** Final HEAD `5b035b7` ("range weapon", 2026-09-28).
No Saturday wrap-up ran for this sprint (no `retro-sprint-15-*.md`); this block is the close-out of record.
Source re-verified against HEAD for every item below.

| ID | Final status | Evidence |
|----|--------------|----------|
| S15-01 BUG-063 | ⏸️ CUT — owner accepted/deferred to demo prep (2026-09-22) | `Stat.cs:63-65` unchanged by decision |
| S15-02 BUG-064 item 7 | ✅ CODE DONE (Play Mode verify pending) | `e1dcee0` added `[Inject] Construct(IObjecPoolService)`; `5b035b7` replaced the interim `RegisterComponentInHierarchy<RangeWeapon>()` with spawn-time injection (`WeaponHolderBase.cs:60` → `resolver?.InjectGameObject`), matching ADR-0004. Not an isolated commit. Verify folded into S16-07 |
| S15-03 BUG-065 | ❌ CARRIED (6th) | `PlayerDeathState.Enter()` still only `base.Enter()` |
| S15-04 BUG-066+070 | ❌ CARRIED (6th / 4th) | Now one site: `VitalStatsBase.cs:28,39,41,45,52,54,58` |
| S15-05 BUG-073 | ❌ CARRIED (2nd) | Bundle with BUG-090 |
| S15-06 Play Mode decision | ❌ CARRIED (9th) | No decision text written |
| S15-07 review-gate decision | ❌ CARRIED (4th) | Off-plan ADR-0005 refactor (~20 commits) landed without it |
| S15-08 BUG-071 | ⚠️ PARTIAL | `b0b1337` added `StartCoroutine`; no stop handle remains |
| S15-09 hook placeholder | ❌ CARRIED (26th+) | `.git/hooks/` has no `pre-push`; now subsumed by TD-048 |
| S15-10 BUG-072 | ⚠️ CODE DONE | `layerMask` on `Lightning.prefab` still `0` — owner Inspector step |
| S15-11 TD-040 ADR | ❌ CARRIED (3rd) | |
| S15-12 first EditMode test | ❌ BLOCKED | BUG-084 (no `.asmdef`) |
| S15-13 sign-off pass | ❌ CARRIED | |
| S15-14 `/doc-sync` | ✅ DONE | `6ca82f3` + CLAUDE.md re-verified at `c0067f4` |

**Velocity (planned work):** ≈0.6d of planned estimate delivered (S15-02 0.1, S15-08 ~0.1, S15-10 0.2,
S15-14 0.2) against 4.0d capacity and a 1.05d Must-Have load → **Must-Have 1/9 fully done**. Fifth
consecutive sprint near 0% on its own plan.

**Off-plan throughput was high:** BUG-088 (compile break), BUG-075/072 (v2 damage path), the ADR-0005
unified-character refactor (Amendments 1-3, ~20 commits), and closure of BUG-043/074/076/077/080/081/
082/085/088/091. Introduced BUG-092 (second compile break in four days); its compile half is fixed at
HEAD (`perTime` field restored), the serialization half is not.

**Verdict:** the codebase moved a lot; the sprint plan did not. The plan is no longer describing the
work. Sprint 16 plans the carry-over as a smaller Must set and explicitly makes room for the owner's
feature stream instead of pretending it will not happen.

---

**Status at open (historical): OPEN.** Opened 2026-09-20 (Sunday 22:00 `pm-weekly-kickoff`, autonomous run — no owner present).
Branch `sprint-15`, created from `sprint-14` tip (`15242e6`, "chore(wrapup): weekly wrap-up 2026-09-19").
`gh` CLI unavailable (`gh: command not found`) — draft PR **not** auto-created; run manually:

```bash
gh pr create --draft --base sprint-14 --head sprint-15 --title "Sprint 15"
```

**Review mode:** lean — producer feasibility gate skipped.

---

## Sprint Goal

**Stop the reporting-only gate; make the small fixes structurally unskippable.** Sprint 14 closed FAIL
(0/9 Must-Have planned, 4th sprint in a row at ~0%). A checklist that only reports cannot block work.
This sprint: (1) land the diagnosed sub-0.2d fixes as one dedicated **opening commit block** *before
any feature work merges into `sprint-15`*, (2) fix the new S2 Abilities v2 bugs (BUG-073/071/072)
**before any new Paladin/ability content**, (3) convert the 8-sprint Play Mode smoke ask into an
explicit owner decision instead of a 9th silent task.

---

## Capacity

- Total days: 5
- Buffer (20%): 1 day
- Available: 4 days

Must-Have load ≈ 1.05d.

---

## Carryover from Sprint 14

Full detail: `sprint-14.md` (closure block), `production/retros/retro-sprint-14-2026-09-19.md`,
`production/qa/bug-triage-2026-09-19.md`. Open bugs: 13 (BUG-052, 063-066, 070-074 + verify-only 067/069).

| Carried item | Times carried | Note |
|---|---|---|
| BUG-063 (`Stat.cs:63-66` `[SerializeField]`) | 29th+ | One-line fix, zero blocker |
| BUG-064 item 7 (`RangeWeapon.cs` DI wiring) | 5th | Pattern: `ItemSpawner.cs:8-10` |
| BUG-065 (`PlayerDeathState.Enter()` stop movement) | 5th | |
| BUG-066 + BUG-070 (vitals dictionary guard) | 5th / 3rd | Bundle |
| BUG-073 (new) missing-script refs, `ShootSpirit.asset` / prefab | 1st | Asset-only, S2 |
| BUG-071 (new) HoT/DoT coroutines never started | 1st | S2 |
| BUG-072 (new) summon damage has no target resolution | 1st | S2, PLAUSIBLE |
| BUG-074 (new) `StatEffectBase` delta vs. current | 1st | S3, design intent unclear |
| BUG-067 / BUG-069 | - | Apparently fixed incidentally; verify only, need Play Mode |
| BUG-068 | - | Downgraded S3, dormant (`GetAbility()` zero callers) |
| S14-07 Play Mode smoke | 8th | Stop scheduling as task; accepted-risk decision (S15-06) |
| S14-08 review-gate decision | 3rd | |
| S14-09 pre-push hook | 25th+ | Placeholder only |
| S14-10 / S14-11 | 19th+ / 18th+ | S4-05/S4-06 keep-or-cut; ADR-0002 -> Accepted |
| S14-12 TD-040 ADR (Abilities v1 vs v2) | 2nd | Required before more v2 work |
| S14-13 first EditMode test (TD-014) | 30th+ | Pair with BUG-066/070 |
| Doc drift (`CLAUDE.md` deleted v2 effects, `SkillState` -> `AbilityState`) | - | `/doc-sync` |
| QA plan | 31st+ | Owner decision |

---

## Tasks

### Must Have (Critical Path)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S15-01 | **Opening block, commit 1** — BUG-063: remove `#if UNITY_EDITOR` / `[SerializeField]` on `Stat.modifiers` (`Stat.cs:63-66`), isolated commit | gameplay-programmer | 0.05 | None | `Stat.modifiers` not serialized under any symbol |
| S15-02 | **Opening block, commit 2** — BUG-064 item 7: `RangeWeapon` `[Inject] Construct(IObjecPoolService)` | gameplay-programmer | 0.1 | None | `poolManager` non-null via DI, no serialized interface field |
| S15-03 | **Opening block, commit 3** — BUG-065: `PlayerDeathState.Enter()` resolves `PlayerMovement` and stops it | gameplay-programmer | 0.1 | None | No sliding in death state |
| S15-04 | **Opening block, commit 4** — BUG-066 + BUG-070: `TryGetValue` guard in `EntityVitalStats` and `VitalStatsComponent` (shared helper optional) | ai-programmer | 0.2 | None | No raw `KeyNotFoundException` from either public API |
| S15-05 | BUG-073 — repoint/remove missing-script refs in `ShootSpirit.asset` and `SpiritProjcetile.prefab` (owner-assisted in Editor if needed) | gameplay-programmer / owner | 0.15 | None | No missing-script warnings on those two assets |
| S15-06 | **Forced decision** — Play Mode smoke session: either happens this week, or owner signs an explicit accepted-risk note; stop re-asking as a task | owner (Kay) | 0.1 | None | Written decision in this file |
| S15-07 | **Forced decision** — off-plan review gate (S14-08): do multi-file changes to actively-developed systems need a same-day `/code-review` before merge to `sprint-15`? Also decide policy for `feature/fix-player-control` (content no longer matches name) | producer | 0.1 | None | Written decision in this file |
| S15-08 | BUG-071 — `StartCoroutine` for `RecoveryPerTime`/`ReductionPerTime` in `VitalStatsComponent` (HoT/DoT) | gameplay-programmer | 0.15 | S15-04 (same file) | Heal/damage-over-time effects tick |
| S15-09 | Pre-push hook placeholder (`exit 0` + TODO), 25th+ carry | producer / owner | 0.1 | None | Hook file exists and is documented |

Must-Have total ≈ **1.05d**.

### Should Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S15-10 | BUG-072 — resolve summon target in `SpawnSummonEffect.SummonExecute` | gameplay-programmer | 0.2 | None | Consecrate/Lightning summon damages enemies in range |
| S15-11 | TD-040 ADR — Abilities v1 vs v2 convergence (or explicit "stays dual-path") | technical-director | 0.3 | None | ADR under `docs/architecture/` |
| S15-12 | First EditMode test (TD-014): absent `StatType` on both vitals components does not throw framework exception | ai-programmer / qa-lead | 0.3 | S15-04 | 1+ passing test |
| S15-13 | S4-05/S4-06 keep-or-cut decision + ADR-0002 -> Accepted (one owner sign-off pass) | owner / producer | 0.15 | None | Both written |
| S15-14 | `/doc-sync` — CLAUDE.md drift (deleted v2 effects, `AbilityState`) | producer | 0.2 | None | CLAUDE.md matches source |

### Nice to Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S15-N1 | BUG-074 — clarify intent (delta vs. absolute) with owner, then fix | gameplay-programmer | 0.1 | owner input | Decision + fix |
| S15-N2 | Verify BUG-067 / BUG-069 in Play Mode (only if S15-06 chooses to run the session) | owner | 0.1 | S15-06 | Both confirmed |
| S15-N3 | Housekeeping: stale log string "ShootSpiritOrbEffect", unused `using Unity.Mathematics`, skill input hardcoded to `AbilitySlot.Utility`; ~230-binary merge size/LFS decision | gameplay-programmer | 0.1 | None | Cleaned / decided |

---

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| Opening block loses to off-plan Paladin/feature work a 5th time | High | High | Merge policy in S15-07: no feature merge into `sprint-15` until S15-01..04 are committed; if it fails again, record as accepted risk in the next retro rather than carry a 6th time |
| Play Mode session never happens (9th sprint) | High | High | S15-06 converts it to a signed decision |
| More v2 content ships on an open v1/v2 decision and 4 open S2 bugs | Medium-High | High | Bug fixes (S15-05/08/10) precede new ability content |
| No Unity CLI / no `gh` | Known | Low | Manual follow-ups noted above |

---

## Definition of Done

- [ ] S15-01..S15-04 committed as separate isolated commits
- [ ] S15-05 BUG-073 assets clean
- [ ] S15-06 Play Mode decision recorded
- [ ] S15-07 review-gate decision recorded
- [ ] S15-08 BUG-071 fixed
- [ ] S15-09 hook placeholder exists
- [ ] QA plan gate resolved (below)

## QA Plan

No QA plan exists for Sprint 15 (`production/qa/qa-plan-sprint-15.md` not found) — 32nd+ consecutive
cycle. Deferred to owner (autonomous run, judgment call).
