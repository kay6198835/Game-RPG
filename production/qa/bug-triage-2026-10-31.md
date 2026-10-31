# Bug Triage — 2026-10-31 (weekly wrap-up, Sprint 20)

> Routine `pm-weekly-wrapup` (`/weekly-wrapup --auto`). HEAD `origin/sprint-20` `2276049e`.
> Evidence: `STATIC` + `LOG` (no filled playtest sheet → no `RUNTIME`).

## Inputs

| Input | Result |
|---|---|
| Week's commits (since `566a684d`) | 17 commits; 51 `.cs` files changed on `sprint-20` (effect component, champion pick, CombatTest harness) |
| Playtest sheet `playtest-2026-10-31.md` | **NOT RUN — no sheet** (in the PM worktree or the owner's folder) |
| `editor-log.sh` | `Editor.log` 2026-10-31 20:17: 0 `error CS`, 14 Play Mode, no `Assets/Script/` exception; `Editor-prev.log` 17:44: 0 / 2 / none |
| `fix-survival.sh 30` | 3 PRESENT, 1 GONE (BUG-096 `40d2c793` — already known, NOTE-20261009-1; no new note) |
| `bug-id.sh audit` | 6 historical IDs without files (pre-register, expected); 1 status mismatch (BUG-107: CLAUDE.md OPEN vs file PARTIAL) |
| `doc-claims.sh` | 645 claims, 102 STALE → `/doc-sync` 23:00 |

## Inbox outcomes

| Note | Outcome | Result |
|---|---|---|
| NOTE-20261019-2 | Not reproduced — closed | No hit since 2026-10-22 |
| NOTE-20261026-1 | Confirmed | **BUG-116** S1 |
| NOTE-20261026-2 | Resolved in week | COMPILED `e156bdf4`, `beca1bcc` |
| NOTE-20261026-3 | Not reproduced — closed | — |
| NOTE-20261029-1 | Resolved in week | merge `2276049e` |
| NOTE-20261029-2 | Not reproduced — closed | logs stay under BUG-104 |
| NOTE-20261031-1 | Confirmed | **BUG-117** S2 |
| NOTE-20261031-2 | Confirmed | **BUG-118** S3 |
| NOTE-20261031-3 | Confirmed | **BUG-119** S3 |
| NOTE-20261031-4 | Duplicate | BUG-100 |
| NOTE-20261031-5 | Confirmed | **BUG-120** S2 |
| NOTE-20261031-6 | Confirmed | **BUG-121** S2 |

Needs owner: none. IDs BUG-116…121 allocated consecutively in one pass (`bug-id.sh next` = BUG-116
before the pass).

## New bugs — priority vs severity

| Bug | Sev | Pri | Why this priority |
|---|---|---|---|
| BUG-116 | S1 | 1 | Every Player build fails; one-line type change |
| BUG-121 | S2 | 1 | One attribute; restores the Paladin weapon bonus and Blessed Slash data; fix **before** anyone re-saves the assets |
| BUG-117 | S2 | 2 | Heal / damage stat effects are no-ops now, wrong sign once serializable |
| BUG-120 | S2 | 2 | Stat items do nothing |
| BUG-118 | S3 | 3 | Dormant — no asset uses the effect |
| BUG-119 | S3 | 3 | Fold into stat-reduction work with BUG-113 |

## Observations (no status change)

- **BUG-114 (weapon half):** `WeaponHolderBase.cs:83-86` now reads `IStatService.GetStatValue()`
  live instead of the `currentStats` snapshot (`6e484f0c`). Buffs still do not reach damage because
  `GetStatValue()` returns `AdjustedValue` (BUG-113). Leave BUG-114 open until BUG-113 is fixed.
- **BUG-108 (S1) looks partly fixed in code:** `CharacterData.cs:16` carries
  `[FormerlySerializedAs("statsSO")]` again, so Knight `EntityData` assets should load their stats.
  `abilityBindings` (`:17`) and `Weapon` (`:21`) still have no attribute for the old
  `abilityBindings` / `weaponSO` keys. Status left Open; the bug file should be re-verified with the
  Knight assets (one Play Mode check: Knight enemies take damage and attack) before it changes.
- **Third occurrence of the serialized-rename pattern** (BUG-105, BUG-108, BUG-121): a rename of a
  serialized field without `[FormerlySerializedAs]`. A pre-commit check for removed
  `FormerlySerializedAs` / renamed `[SerializeField]` fields would have caught all three.
- The owner's working tree has uncommitted edits to `EffectReceiverBase.cs`, `CharacterData.cs`,
  `BuffDebuffStatsForDuration.cs`, `RecoveryReductionPerTimeForDuration.cs` and others, plus untracked
  `production/qa/qa-*-sprint-20-2026-10-31.md`. Invisible to the routine by design; BUG-116/117/118/120
  should be re-read after the next push.

## Blockers for Sprint 21

S1: **BUG-116** (new), **BUG-108**, **BUG-096** (both carried, unfixed). Priority-1 S2: **BUG-121**.
