# Bug Triage — 2026-11-15

> `/weekly-wrapup --auto` (routine `pm-weekly-wrapup`, Saturday slot fired Sunday 2026-11-15).
> Read at `origin/sprint-22` `ff7f3ecb`; owner branch `docs/gdd-pathfinding-ability` `3b02df7d` reviewed
> read-only (not merged). Evidence: RUNTIME (Friday automated QA run `20261114-2357`, no playtest sheet)
> + LOG + STATIC.

## Inbox outcomes (18 notes)

| Outcome | Count | Notes |
|---|---|---|
| Confirmed | 4 | NOTE-20261115-1 → **BUG-125**, NOTE-20261115-4 → **BUG-126**, NOTE-20261115-3 → **BUG-127**, NOTE-20261107-4 → **BUG-128** (+ BUG-073 evidence) |
| Resolved in week | 7 | NOTE-20261104-3, NOTE-20261107-3, NOTE-20261111-1, NOTE-20261112-1, NOTE-20261112-2, NOTE-20261113-1, NOTE-20261115-8 |
| Duplicate | 1 | NOTE-20261115-2 → BUG-120 |
| Not a bug / by design | 2 | NOTE-20261114-1 (hint, acted on), NOTE-20261115-5 (intentional field removal; re-save chore) |
| Not reproduced | 1 | NOTE-20261107-2 |
| Needs owner | 3 | NOTE-20261104-4 (third carry), NOTE-20261115-6, NOTE-20261115-7 (unmerged WIP branch) |

IDs allocated in one pass with `bug-id.sh next` → `BUG-125` (the script reads the owner's folder,
which has no new files, so the four were numbered consecutively from it).

## New bugs

| ID | Sev | Pri | Title | Evidence |
|---|---|---|---|---|
| BUG-125 | S2 | 1 | Melee hits add the stage damage twice (`Weapon.FinalDamage()`, `38d40925`) | RUNTIME (QA-CMB-01.Paladin 2/2) + STATIC |
| BUG-126 | S2 | 2 | `TMP_TextInfoDebugTool.cs:4` unguarded `using UnityEditor;` — Player build break | STATIC (QA-STATIC-05); no Player build run |
| BUG-127 | S3 | 2 | Mage bolt `speed: 1` / `lifetime: 4` — basic attack barely reaches | RUNTIME (QA-CMB-04/05.Mage 2/2) + asset read |
| BUG-128 | S4 | 3 | 18 sprite capture scenes reference an uncommitted capture-tool script | RUNTIME (automated static case) |

## Status changes

| ID | From | To | Evidence |
|---|---|---|---|
| BUG-063 | ACCEPTED (deferred) | FIXED in code — awaiting Play Mode check | `a0e250fb`: `Stat` no longer serialized; QA-STATIC-04 PASS |
| BUG-084 | Open | FIXED | asmdefs (`9c11c672`, `e555e9da`, `d9776bb8`); 64 EditMode tests PASS in run `20261114-2357` |
| BUG-109 | Open | FIXED in code — awaiting hand check | `285e6900`; QA-ABL-01.Archer.Ultimate PASS |
| BUG-115 | PARTIAL | FIXED in code and data — awaiting Play Mode check | `a0e250fb`; QA-STATIC-04 PASS, `StatSheetTests` PASS |
| BUG-124 | Open | FIXED in code — awaiting runtime check | `b33dedf9` (`Pool.Reload()` order) |

Evidence appended without status change: BUG-073, BUG-117 (QA-ENT-14 confirms the sign inversion;
owner branch serializes `VitalModifier` without the sign fix — merge risk), BUG-120 (QA-ITEM-03 NRE,
owner branch de-abstracts the two subclasses).

QA known-issues to review on `qa/weekly-automation`: QA-CMB-01.Paladin should map to **BUG-125** (not
BUG-105); QA-CMB-04/05.Mage to **BUG-127**; QA-STATIC-05 to **BUG-126**; QA-STATIC-01 capture scenes
to **BUG-128**. No closed bug this run is mapped by a case except BUG-115 (QA-STATIC-04 already PASS)
— remove its mapping if any remains.

## Priority vs severity — blockers for sprint 23

| Rank | ID | Sev | Why first |
|---|---|---|---|
| 1 | BUG-122 | S1 | Player death never reaches Game Over (QA-VIT-07 KNOWN, every run since 2026-11-01). Carried, untouched this sprint |
| 2 | BUG-125 | S2 | One line; every melee balance number is wrong until fixed |
| 3 | BUG-096 / BUG-097 | S1 / S2 | Boss room sealed; run starts in the boss room. 7th carry |
| 4 | BUG-126 | S2 | One line; blocks any demo build |
| 5 | BUG-117 + BUG-120 + BUG-123 | S2 | Must be fixed together before the owner's branch (partial fixes) merges |
| 6 | BUG-065 + BUG-086 | S2 | Same file as BUG-122 |

## Fix survival (`fix-survival.sh 30`)

0 GONE, 0 NOT ANCESTOR. PRESENT: BUG-106 (`6094aa10`), BUG-108 (`fc98fa2a`). PARTIAL: BUG-107
(`1370136c` 75%, `3d768b3d` 37%), BUG-115 / 116 / 121 (`1370136c` 75%). Spot re-read: the PARTIAL
rows all cite the multi-file commit `1370136c`, re-touched by the stat split `a0e250fb` and
`b33dedf9`. BUG-116 (`RuntimeAnimatorController`) and BUG-121 (`[FormerlySerializedAs]`) unchanged
since the 2026-11-10 spot check; BUG-115 superseded by `StatSheet`. No inbox note.

## Doc / ID drift (for `/doc-sync` 23:00)

`bug-id.sh audit`: 0 bug files missing `**Status**`; 0 CLAUDE.md status mismatches; 6 historical IDs
(BUG-010…046) cited without files (pre-register, allowed). `doc-claims.sh`: 779 claims, 115 STALE
(mostly `ON_CLEAR_ENEMY`, pre-move paths, old line citations). New since last week: `Stat.cs:63`
citations (file is 48 lines after the stat split), `Assets/Tests/PlayMode` in `test-standards.md`.
