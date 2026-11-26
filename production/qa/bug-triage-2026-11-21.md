# Bug Triage — 2026-11-21 (Saturday wrap-up, delta run)

> `/weekly-wrapup --auto`, Saturday 22:00 slot. The full weekly triage already ran on Friday
> 2026-11-20 (`bug-triage-2026-11-20.md`, scheduler fired a day early). This run covers only what
> changed since that wrap-up commit `8f3f37ff`: four owner commits (`be973605`, `8005d27e`,
> `9e5921c0`, `67c188b3`), three automated QA runs on Saturday, and the inbox notes written since.
> Code re-read at `origin/sprint-23` `68631d3f`. Evidence: RUNTIME (automated QA `20261121-2106`)
> + STATIC + LOG. No playtest sheet filled.

## Inbox outcomes

| Note | Outcome | Result |
|---|---|---|
| NOTE-20261121-1 | Confirmed | **BUG-135** (S1, P1) — hurtboxes no longer implement `INegativeReceiver` |
| NOTE-20261121-2 | Duplicate | Appended to **BUG-130** (dash self re-entry, `IsDash` before `IsDeath`, hurtbox not restored) |
| NOTE-20261121-3 | Duplicate | Appended to **BUG-135** (`hurtbox` null / wrong collider) |
| NOTE-20261121-4 | Resolved in week | Transient mid-edit compile errors of `8005d27e` |
| NOTE-20261121-5 | Not a bug / by design | Owner renamed the coin Create-menu path (`8236f9d1`); no reopen of BUG-120 |
| NOTE-20261115-7 | Needs owner (carried) | Stats key C still commented out |
| NOTE-20261117-4 | Needs owner (carried) | Move-state spawn / `AimPoint` death facing intent |

Totals: 7 notes — 1 confirmed, 2 duplicate, 1 resolved, 1 by design, 2 needs owner.

## Status changes

| Bug | From | To | Evidence |
|---|---|---|---|
| BUG-129 | Open (S1) | FIXED in code, awaiting a hand check | `be973605` sets `ConfirnData.callback`; QA-PLY-05 PASS for all three classes in runs `20261121-2050` / `-2106` |
| BUG-135 | — | Open (S1, new) | QA-ENT-08 x10, QA-VIT-06 / 13 / 14 / 15 FAIL in 3 of 3 runs; re-read `NegativeReceiverBase.cs:9` |
| BUG-130 | Open | Open (evidence added) | Still KNOWN (QA-PLY-06 x3); new code-side cause in `PlayerBasicState.cs:31` |

S1 count unchanged at 6 (BUG-129 out, BUG-135 in).

## Priority vs severity

| Bug | Sev | Priority | Why |
|---|---|---|---|
| BUG-135 | S1 | 1 | One-line header fix; while open, no damage reaches anyone, so rooms never clear and death (BUG-122 / 087) cannot even be reached. Fix before any other combat or death work |
| BUG-130 | S2 | 1 | First Space press soft-locks; now two causes (controllers + self re-entry). Fix together with the hurtbox restore in `Exit()` |
| BUG-122 / 087 / 086 / 065 | S1 / S2 | 2 | Death → Game Over chain, testable only after BUG-135 |
| BUG-126 | S2 | 2 | One line; blocks any Player build |

## Blockers for next sprint

1. **BUG-135** — first task Monday (≈0.05d), then rerun the automated suite the same day.
2. **BUG-130** — dash (≈0.15d incl. controllers).
3. S23-02 remainder (BUG-122 / 086 / 065) and S23-05 (BUG-126) carry as before.

## QA follow-up (for `qa/weekly-automation`, not done here)

- `qa-automation/config/known-issues.json`: no case maps to BUG-129, so nothing to remove. After the
  BUG-135 fix, QA-ENT-08.* / QA-VIT-06 / 13 / 14 / 15 should turn PASS; until then they can map to
  BUG-135.
- QA-ENT-10 (45 s real-time timeout, new FAIL in `-2106`) is a flaky suspect, not triaged as a bug.

## Scripts

- `fix-survival.sh 30` (HEAD `36d0b0db`, owner folder): 1 GONE (BUG-120 `aa045f79`, see
  NOTE-20261121-5), 11 PARTIAL rows — all multi-file commits with later edits on the same files; no
  further note.
- `bug-id.sh audit`: next free ID `BUG-135` before allocation; 6 historical IDs without a file
  (BUG-010, 033, 042, 043, 044, 046 — predate the register); 0 files without Status; 0 bug rows in
  `CLAUDE.md`.
- `doc-claims.sh`: 485 claims, 59 STALE → `/doc-sync --auto` 23:00.
- Scripts read the owner folder (`REPO=.`), which is on `qa/weekly-automation` `36d0b0db`; the bug
  register there matches `sprint-23` (74 files). `open-bugs.md` was regenerated from that output with
  tonight's two changes (BUG-129 out, BUG-135 in) applied, because the routine cannot set `REPO`.
