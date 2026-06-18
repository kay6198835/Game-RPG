# Sprint 2 — Daily Plan & Progress Tracker

> **Sprint**: 2026-06-15 (Mon) → 2026-06-19 (Fri)
> **Companion to**: `sprint-02.md` (formal plan) — this file is the day-by-day breakdown + live tracker
> **Maintained by**: PM assistant — updated each session/standup
> **Routines (Claude Code web app)**:
>   - **Mon–Fri 10:00** → `/daily-standup` — summarizes/analyzes yesterday from
>     git + this tracker, updates statuses below, lists today's tasks with estimates.
>   - **Sat 22:00** → `/weekly-wrapup` — end-of-week close: code-review of the
>     week's `.cs`, playtest log, bug-triage, light retro; finalizes the weekly
>     verdict and records carry-over + velocity.
>   - **Sun 22:00** → `/weekly-kickoff` — closes last sprint, auto-creates the
>     upcoming week's sprint (`sprint-NN.md` + `sprint-NN-daily-plan.md`), ready
>     before Monday morning.
> **Last updated**: 2026-06-18 (Thu)

---

## Status Verdict: 🔴 AT-RISK

**Why**: 3 of 4 capacity days elapsed (Mon/Tue/Wed). S2-02 (decouple Weapon) and S2-05 (melee damage — Bug #4) still **not started**. S2-04 (Bug #9 anim double-reg) still **not started**. Only 1 working day (Thu) before Friday wrapup. Developer diverged to combo-attack input buffer work (valid infrastructure, but off-plan for the sprint must-haves).

**Recommended action (Thu 18/06)**:
1. Commit the dirty working tree first (finish S2-01).
2. Land S2-05 (WeaponMelee damage, 0.25d) **immediately** — combat must be testable today.
3. Fix S2-04 (Bug #9, 0.25d).
4. Fix S2-03 LINQ→foreach (0.25d).
5. Time-box S2-02: start but expect it to slip to next sprint.
6. **Cut S2-06** (EditMode test) — no capacity left.

---

## Burn Summary

| Metric | Value |
|--------|-------|
| Total work estimated | 3.5 days |
| Capacity (sprint) | 4 days (5 − 20% buffer) |
| Days elapsed | 3 (Mon, Tue, Wed) |
| Days remaining | 1 (Thu) + Fri wrapup |
| Work committed/done | ~0.5 days (S2-03 partial, StatusAnimation infra) |
| Work remaining | ~3.0 days |
| Slack | −2.0 days (over budget) |

---

## Task Estimates (from sprint-02.md)

| ID | Task | Est (d) | Priority | Status |
|----|------|---------|----------|--------|
| S2-01 | Stabilize + commit the 28-file working tree (clean base) | 0.5 | Must (blocker) | 🟡 In progress — 3 commits landed (Mon/Tue/Wed), working tree still dirty |
| S2-02 | Decouple `Weapon` ↔ `WeaponHolder`/`AbilityHolder` (push-on-equip) | 1.0 | Must | ⬜ Not started |
| S2-03 | `Core.GetCoreComponent<T>()` + self-register + lazy-cache (OCP) | 1.0 | Must (cut-first if slipping) | 🟡 In progress — API exists, named props commented out; uses LINQ (violates criteria) |
| S2-04 | Fix Bug #9 — AnimationPlayerController double-registration | 0.25 | Must | ⬜ Not started |
| S2-05 | Fix Bug #4 — `WeaponMelee.Attack()` empty foreach | 0.25 | Must | ⬜ Not started |
| S2-06 | One EditMode test for equip→ability path | 0.5 | Should | ✂️ Cut — no capacity |

Status legend: ⬜ Not started · 🟡 In progress · ✅ Done · ⏸️ Blocked · ✂️ Cut

---

## Day-by-Day Breakdown

### Mon 15/06 — elapsed
- Planned: S2-01 (0.5d) + start S2-02
- Actual: Sprint started (`start sprint 2`). `bdd70b7` "refactor component and state player" — initial base cleanup.
- Note: clean base not fully committed.

### Tue 16/06 — elapsed
- Planned: S2-02 (decouple Weapon)
- Actual: `5ce1037` "refactor component and state player" (09:15). `c7d3fa5` "fix state and logic flow attack combo" (16:11) — `PlayerInputHandle`, `PlayerState`, `PlayerAttackState`, `PlayerBasicState`, `WeaponMelee`, `Weapon`.
- Note: Work shifted toward combo-attack flow fix (Problem 2 from sprint-02.md added note). S2-02 not started.

### Wed 17/06 — elapsed
- Planned: S2-01 commit + S2-05 melee damage + start S2-02
- Actual: `3c9f9b7` "coding" (09:39) — large player state refactor:
  - `StatusAnimation` enum added (`Start`, `StartRangeTrigger`, `OnActivate`, `OffActivate`, `EndRangeTrigger`, `End`)
  - `PlayerState.Enter()` caches all core components centrally (replaces per-state init)
  - `PlayerUseWeaponState` exits on `StatusAnimation.EndRangeTrigger` (replaces legacy anim-finish bool)
  - `PlayerAttackState` — combo-cancel window using animation range triggers
  - `Core.cs` named properties commented out (S2-03 partial progress)
  - `SetLevel.unity` scene reorganized
  - Working tree still dirty post-commit: `bufferIsAttack`, `SetBufferAttack()`, `GetBufferAttack()` input buffer in `PlayerInputHandle` uncommitted
- Note: S2-05 and S2-04 still not done. Combo input buffer infrastructure laid.

### Thu 18/06 — TODAY 🎯
**Goal: land S2-05 (combat testable) + S2-04 + commit dirty tree.**
| Order | Task | Est | Why now |
|-------|------|-----|---------|
| 1 | Commit dirty working tree (finish S2-01) | 0.25d | Must happen before any new edits |
| 2 | S2-05 — `WeaponMelee.Attack()` add `INegativeReceiver.TakeDamage()` | 0.25d | **Last chance to make combat testable this sprint** |
| 3 | S2-04 — Fix Bug #9: `AnimationPlayerController` line 21 `StartAnimation` → `EndAnimation` + mirror `OnDisable` | 0.25d | Unblocks ability Exit verification |
| 4 | S2-03 — Replace LINQ `OfType<T>().FirstOrDefault()` with foreach in `Core.GetCoreComponent` | 0.25d | Acceptance criteria violation |
| 5 | S2-02 — Start decouple Weapon ↔ WeaponHolder/AbilityHolder | 0.25d start | Time-boxed; expect carry to Sprint 3 |
- Day load: ~1.0–1.25d. Tight. S2-02 is aspirational today.

### Fri 19/06
| Order | Task | Est | Notes |
|-------|------|-----|-------|
| 1 | Continue S2-02 if not done Thu | ≤0.5d | **Cut if still not started** |
| 2 | Smoke-check: equip → attack → skill loop | — | Per Definition of Done |
| 3 | `/weekly-wrapup` — code review, playtest log, retrospective | — | Friday ritual |

---

## Risks (live)
| Risk | Status | Mitigation |
|------|--------|------------|
| S2-05 not done (combat not testable) | 🔴 Critical | Must land Thu; highest priority |
| S2-02 not started after 3 days | 🔴 Active | Time-box to Thu afternoon; carry to Sprint 3 if needed |
| S2-03 LINQ violates acceptance criteria | 🟡 Open | Replace with foreach Thu — small fix |
| Combo input buffer untested (Play Mode needed) | 🟡 Watch | Static analysis only; mark as ADVISORY |
| S2-06 (EditMode test) cut | 🔴 Active | TD-014 (zero automated tests) persists; add to Sprint 3 backlog |
| 5th consecutive week with unplayable combat | 🔴 Active | S2-05 Thu is the line in the sand |

---

## Daily Log
> Owner reports what was done; PM updates estimates/status here each session.

- **2026-06-17 (Wed)**: Tracker created. Sprint flagged AT-RISK.
- **2026-06-18 (Thu 02:00 standup)**: Reviewed Wed commits. `3c9f9b7` confirmed (StatusAnimation infra, Core.GetCoreComponent partial). S2-05 and S2-04 still open. Working tree dirty. S2-06 cut. Plan revised — S2-05 is Thu's #1 priority. See standup digest below.

---

## Standup Digest — 2026-06-18 (Thu)

### Hôm qua làm gì (Yesterday — Wed 17/06)

**Commit `3c9f9b7` "coding" (09:39 UTC+7)** — Player State Machine refactor lớn:
- Thêm `StatusAnimation` enum (state machine) với các mốc animation (marker): `StartRangeTrigger`, `OnActivate`, `OffActivate`, `EndRangeTrigger`
- `PlayerState.Enter()` giờ cache tất cả core component (không còn khởi tạo riêng ở từng state con)
- `PlayerUseWeaponState` thoát (exit) bằng `StatusAnimation.EndRangeTrigger` thay vì flag cũ
- `PlayerAttackState` thêm combo-cancel window: cho phép buffer (đệm) input attack trong cửa sổ animation
- `Core.cs`: xóa named properties, chuyển sang `GetCoreComponent<T>()` — tiến độ S2-03

**Uncommitted changes (working tree)**: `PlayerInputHandle.cs` đã có `bufferIsAttack`, `SetBufferAttack()`, `GetBufferAttack()` nhưng chưa commit (staged chưa).

**Lưu ý**: Developer tập trung vào combo-attack input buffering (Problem 2 từ sprint-02.md Added Note), không phải S2-02/S2-05 như plan. Hợp lý về kỹ thuật nhưng S2-05 vẫn chưa xong.

### Hôm nay làm gì (Today — Thu 18/06) + Estimates

| # | Việc cần làm | Est | Ghi chú |
|---|--------------|-----|---------|
| 1 | Commit dirty working tree (S2-01 close-out) | 0.25d | Prerequisite cho mọi thứ |
| 2 | **S2-05** — Thêm `INegativeReceiver.TakeDamage()` vào `WeaponMelee.Attack()` foreach | 0.25d | **CRITICAL — không có cái này combat không testable** |
| 3 | **S2-04** — Sửa Bug #9: `AnimationPlayerController.cs` dòng 21 `StartAnimation` → `EndAnimation`; fix `OnDisable` mirror | 0.25d | Unblocks ability Exit verification |
| 4 | **S2-03** — Thay LINQ (`OfType<T>().FirstOrDefault()`) bằng `foreach` trong `Core.GetCoreComponent` | 0.25d | Acceptance criteria violation hiện tại |
| 5 | **S2-02** — Bắt đầu decouple `Weapon` ↔ `WeaponHolder`/`AbilityHolder` | 0.25d (start) | Time-box; carry to Sprint 3 nếu không xong hôm nay |

### Blockers & Rủi ro

- 🔴 **S2-05 chưa xong** — combat (chiến đấu) không testable (không thể kiểm tra) sau 4 ngày sprint. Đây là line in the sand (ranh giới không thể vượt) của ngày hôm nay.
- 🔴 **S2-02 chưa bắt đầu** — decouple (tách biệt) Weapon ↔ WeaponHolder là must-have nhưng khả năng cao sẽ carry (mang sang) Sprint 3.
- 🟡 **S2-03 LINQ violation** — `Core.GetCoreComponent` dùng LINQ thay vì `foreach` — vi phạm acceptance criteria và `engine-code.md` (zero-alloc hot path). Cần sửa Thu.
- 🟡 **S2-06 cut** (✂️) — EditMode test bị cắt vì không còn capacity (năng lực). TD-014 (zero automated tests) vẫn là technical debt mang sang Sprint 3.
- 🟡 **Combo input buffer** (`bufferIsAttack`) chưa tested trong Play Mode — static analysis only, mức ADVISORY.
