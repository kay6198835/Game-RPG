# Bug Triage — 2026-11-20

> `/weekly-wrapup --auto` (routine `pm-weekly-wrapup`). **The scheduler fired on Friday 2026-11-20**,
> one day before the planned Saturday slot; today's session date is used for every file name.
> Read at `origin/sprint-23` `0069a40c`. Evidence: STATIC + LOG (+ RUNTIME from the Thursday automated
> QA run `20261119-1638`; no Friday run yet, no playtest sheet filled).

## Inbox outcomes (31 notes: 28 open + 3 written tonight)

| Outcome | Count | Notes |
|---|---|---|
| Confirmed | 6 | NOTE-20261120-7 → **BUG-129**, NOTE-20261117-3 → **BUG-130**, NOTE-20261120-6 → **BUG-131**, NOTE-20261120-4 → **BUG-132**, NOTE-20261120-8 → **BUG-133**, NOTE-20261117-5 → **BUG-134** |
| Resolved in week | 11 | NOTE-20261104-4, NOTE-20261115-6, NOTE-20261115-7 (items 1-2), NOTE-20261116-1, NOTE-20261116-2, NOTE-20261117-1 (session), NOTE-20261117-7, NOTE-20261117-8, NOTE-20261118-1, NOTE-20261118-2, NOTE-20261120-9 |
| Duplicate | 5 | NOTE-20261117-6 → BUG-127, NOTE-20261119-1 → BUG-096, NOTE-20261120-1 → BUG-122, NOTE-20261120-3 → BUG-122, NOTE-20261120-5 → BUG-120 |
| Not a bug / by design (incl. hints acted on) | 8 | NOTE-20261116-3, -4, -5, -6, -7, NOTE-20261117-2, NOTE-20261118-3, NOTE-20261120-2 |
| Needs owner (carried) | 2 | NOTE-20261115-7 item 3 (stats key C), NOTE-20261117-4 (Move-state spawn, `AimPoint` death facing) |

ID collision fixed: the standup pair `NOTE-20261117-1` / `-2` renumbered to `NOTE-20261117-7` / `-8`
(NOTE-20261118-2); the session pair keeps its IDs.

IDs allocated one at a time with `REPO=D:/Fork/Game-RPG-pm bash .claude/scripts/bug-id.sh next`
(without `REPO` the script reads the owner's folder and kept returning BUG-129).

## New bugs

| ID | Sev | Pri | Title | Evidence |
|---|---|---|---|---|
| BUG-129 | S1 | 1 | Champion confirm does nothing — `19540dff` drops the confirm callback; start room never opens | STATIC (new today, after the last QA run) |
| BUG-130 | S2 | 1 | Dash soft lock — class animator controllers have no Dash state | RUNTIME (QA-PLY-06, 3 classes × 3 runs) + asset grep |
| BUG-131 | S3 | 2 | `Interact.FindNearestObject()` reads stale buffer slots | STATIC |
| BUG-132 | S4 | 3 | `Test.asset` item has no name or sprite | STATIC (asset) |
| BUG-133 | S4 | 3 | Knockback pulls the victim toward the attacker (`19540dff`) | STATIC |
| BUG-134 | S4 | 3 | `EntityMoveState` timer never reset, literal `10` | RUNTIME (QA-ENT-09, 1 run) + STATIC |

## Status changes

| ID | From | To | Evidence |
|---|---|---|---|
| BUG-066 / BUG-070 | Open | FIXED in code | Every `currentStats` read in `VitalStatsBase` uses `TryGetValue` |
| BUG-071 | PARTIAL | FIXED in code (not observable until BUG-118) | `16f61da7`, `b5fb3581`, `0626dd59` |
| BUG-073 | Open | FIXED in data | ShootSpirit / SpiritProjcetile deleted (`e6c826e5`, `1d05475a`) |
| BUG-090 | Open | FIXED in data | SpiritBomd deleted (`e6c826e5`) |
| BUG-097 | Open | FIXED in code and data — awaiting Play Mode | `e2b7efee`, `a92a0c7b`, `3015e9cf` |
| BUG-101 | Open | CLOSED | Orphans filed; last citations corrected 2026-11-16 |
| BUG-106 | PARTIAL | CLOSED | Owner: player half by design (NOTE-20261116-3) |
| BUG-110 | Open | Open — deferred | Owner: weapon swap out of scope this phase |
| BUG-118 | Open | Open — deferred post-demo | Owner decision 2026-11-17 |
| BUG-119 | Open | FIXED in code | Clamp at `VitalStatsBase.cs:92` |
| BUG-120 | Open | PARTIAL | QA-ITEM-03 PASS (`20261119-1638`); residual null-element guard |
| BUG-122 | Open | PARTIAL | `49c58e35` landed; death state re-entry + Archer / Mage clips without event |
| BUG-125 | Open | FIXED in code | `b92b6f2a`; QA-CMB-01.Paladin PASS (`20261119-1638`) |

Evidence appended without status change: BUG-096 (spawn early returns), BUG-102 (re-verified; item 2
gone), BUG-127 (Archer has the same `speed: 1`).

Not moved tonight (no triage hint, not re-read): BUG-117 and BUG-123 (S23-08 code on 2026-11-16, per
the tracker), BUG-100 / 113 / 114 (hints of NOTE-20261116-6), BUG-095 (collider added; remaining
failure is BUG-127). Next triage should re-read them.

S1 count 5 → 6 (BUG-129 added); `CLAUDE.md` "Bugs:" line updated.

## QA known-issues to review (`qa/weekly-automation`, QA work — not done here)

- QA-CMB-01.Paladin: BUG-125 is FIXED — remove any remaining mapping.
- QA-CMB-04.Archer: map to **BUG-127** instead of BUG-095.
- QA-STATIC-01 / QA-STATIC-02: drop the ShootSpirit mappings (BUG-073, BUG-090 FIXED).
- QA-STATIC-03 / QA-STATIC-05: the report flags mappings to BUG-105 / BUG-121 / BUG-116 "whose file
  says fixed"; QA-STATIC-05 belongs to BUG-126 only.
- QA-PLY-06: map to **BUG-130**. QA-ENT-09: map to **BUG-134**.
- QA-STATIC-12 (empty Archer / Mage abilities): baseline — by design this phase.

## Priority vs severity — blockers for sprint 24

| Rank | ID | Sev | Why first |
|---|---|---|---|
| 1 | BUG-129 | S1 | One line; without it no run leaves the start room — regression from today's `19540dff` |
| 2 | BUG-122 (+ BUG-086, BUG-065) | S1 | Death → Game Over: add the empty `LogicUpdate()` override, then events on Archer / Mage clips |
| 3 | BUG-130 | S2 | One Space press freezes the player; blocks the playtest and QA cases after a dash |
| 4 | BUG-126 | S2 | One line; blocks any Player build (6th day untouched) |
| 5 | BUG-096 | S1 | Latent for committed data; emit the door event on every spawn early return |
| 6 | BUG-127 | S3 | Data only; Archer and Mage basic attacks barely reach |

## Fix survival (`fix-survival.sh 30`)

0 GONE, 0 NOT ANCESTOR. PRESENT: BUG-063, 084, 106, 108, 109, 115 (`a0e250fb`), 124, 125. PARTIAL:
BUG-107 / 115 / 116 / 121 on `1370136c` (67 %, was 75 % last week). `1370136c` is a 47-file commit;
this week's drop comes from the Knight asset deletion (`041bf198`, BUG-108) and the map restructure
(`e16111d3`, `a48ed6c7`, `e2b7efee` on `RoomGeneraterController.cs` / `EnemySpawner.cs`), files
outside those four fixes. No inbox note.

## Doc / ID drift (for `/doc-sync`)

`bug-id.sh audit`: 0 bug files missing `**Status**`; 0 bug rows in `CLAUDE.md`; 6 historical IDs
(BUG-010…046) cited without files (pre-register, allowed); `open-bugs.md` regenerated tonight.
`doc-claims.sh`: 485 claims, 59 STALE (mostly `ON_CLEAR_ENEMY`, pre-move paths, `Stat.cs:63`,
`LevelManagerEditor.cs:23` — file deleted in the `LevelEditorTool` split).
