# Bug Triage — 2026-10-25 (weekly wrap-up, Sprint 19)

> Routine `pm-weekly-wrapup`, `/weekly-wrapup --auto`. Saturday slot fired Sunday 2026-10-25.
> Code read at `origin/sprint-19` `05dcf630`. Evidence tier: `STATIC` + `LOG` (no playtest sheet filled).
> IDs allocated with `bash .claude/scripts/bug-id.sh next` (returned `BUG-110`; the script reads the
> owner's folder, which does not see files created in the PM worktree, so the six IDs were allocated
> consecutively in one pass as `.claude/rules/bug-inbox.md` requires).

## Inbox outcomes

| Note | Outcome | Result |
|---|---|---|
| NOTE-20261015-1 | Duplicate | BUG-104 (NRE not seen after `ddb1d702`) |
| NOTE-20261019-2 | Needs owner (2nd carry) | Stack trace + asset name needed; not seen 2026-10-24 |
| NOTE-20261020-1 (compile-check) | Resolved in week | `ddb1d702` / `8a36bc3e`; COMPILED `a3f50c5e` |
| NOTE-20261020-2 | Confirmed (narrowed) | **BUG-110** (S2) — default-weapon half resolved in `05dcf630` |
| NOTE-20261020-3 | Confirmed | **BUG-111** (S3, dormant) |
| NOTE-20261020-4 | Resolved in week | `8a36bc3e` |
| NOTE-20261020-5 (renumbered duplicate ID) | Confirmed | **BUG-112** (S3) |
| NOTE-20261021-1 | Confirmed | **BUG-113** (S2) |
| NOTE-20261021-2 | Confirmed | **BUG-114** (S2) |
| NOTE-20261021-3 | Confirmed | **BUG-115** (S2, lowered from S1) |
| NOTE-20261021-4 | Not a bug (design debt) | Covered by the stat-reduction epic; rest → tech-debt register |
| NOTE-20261025-1 (wrapup) | Duplicate | BUG-104 regressed in `05dcf630` |
| NOTE-20261025-2 (fix-survival) | Duplicate | BUG-064 evidence + BUG-110 |

Totals: 13 notes — 6 confirmed, 2 resolved in week, 3 duplicate, 1 not a bug, 1 needs owner.

## Status changes on existing bugs

| Bug | From | To | Evidence |
|---|---|---|---|
| BUG-107 | Open | PARTIAL | `3d768b3d` releases before nulling (`WeaponHolderBase.cs:47-51`); `Data.Weapon` null guard still missing (`:52`) |
| BUG-104 | Open | Open (regressed) | Removed in `ddb1d702`, two logs re-added in `05dcf630` (`PlayerState.cs:34,42`) |
| BUG-064 | FIXED in code | unchanged | fix-survival PARTIAL 77% explained by renames; pickup path split to BUG-110 |

No bug was closed: no playtest sheet, so nothing moved to a runtime-confirmed state. Section B of the
2026-10-24 sheet is empty.

## Priority vs severity

| Bug | Sev | Priority | Note |
|---|---|---|---|
| BUG-115 | S2 | 1 | Corrupts committed assets every Play Mode session; fix with the four-asset cleanup |
| BUG-110 | S2 | 2 | Weapon swap broken; small, same file family as BUG-106/107 — one session |
| BUG-113 | S2 | 2 | Precondition for BUG-114 and BUG-111 |
| BUG-114 | S2 | 2 | After BUG-113 |
| BUG-111 | S3 | 3 | Dormant until `RegenComponent` is put on a prefab |
| BUG-112 | S3 | 3 | Owner-only: commit the two NPC scripts or strip the components |

## Blockers for Sprint 20

- **BUG-108** (S1, open since 2026-10-19) — enemies load `Stats = null`; with S19-02 merged it is live on `sprint-19`.
- **BUG-096** (S1) — non-combat rooms sealed; S19-03 not started.
- **S19-01 playtest** — 19 carries; no runtime evidence for any fix this month.
- Weapon trio **BUG-106 / BUG-107 / BUG-110** — one `WeaponHolderBase` session.

## Drift (for `/doc-sync` 23:00)

- `doc-claims.sh`: 26 STALE claims at `05dcf630` (CLAUDE.md paths / cites, 5 in `.claude/rules/`).
- `bug-id.sh audit`: 6 cited IDs with no file (all historical below BUG-052 except `BUG-010`, cited in
  an empty location); 0 bug files without a `**Status**` line; 0 CLAUDE.md-vs-file status mismatches
  before this triage. CLAUDE.md Known Bugs needs rows BUG-110…115 and the BUG-107 / BUG-104 changes.
