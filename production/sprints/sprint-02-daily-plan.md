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
> **Last updated**: 2026-06-19 (Fri)

---

## Status Verdict: 🔴 CRITICAL — SPRINT LAST DAY

**Why**: All 4 capacity days elapsed. ZERO code commits on Thu 18/06. Working tree still has 20 dirty files (same as Wed evening). S2-05 (melee damage, Bug #4), S2-04 (Bug #9), S2-03 (LINQ) all **not started** after full sprint week. Fri 19/06 is the final day — sprint closes today.

**Recommended action (Fri 19/06 — FINAL DAY)**:
1. **Commit the dirty working tree NOW** (S2-01 — blocks everything else).
2. **Land S2-05** (WeaponMelee.Attack() foreach body) — last chance to make combat testable this sprint.
3. **Fix S2-04** (AnimationPlayerController line 21+29 double-reg) — quick win.
4. **Fix S2-03** (Core.GetCoreComponent LINQ → foreach) — acceptance criteria violation.
5. **Cut S2-02** (decouple Weapon) — carry to Sprint 3. No capacity left.
6. Run `/weekly-wrapup` to close the sprint.

---

## Burn Summary

| Metric | Value |
|--------|-------|
| Total work estimated | 3.5 days |
| Capacity (sprint) | 4 days (5 − 20% buffer) |
| Days elapsed | 4 (Mon, Tue, Wed, Thu) |
| Days remaining | 0 (Fri = wrapup day) |
| Work committed/done | ~0.5 days (S2-03 partial, StatusAnimation infra) |
| Work remaining | ~3.0 days |
| Slack | −3.0 days (severely over budget) |

---

## Task Estimates (from sprint-02.md)

| ID | Task | Est (d) | Priority | Status |
|----|------|---------|----------|--------|
| S2-01 | Stabilize + commit the 28-file working tree (clean base) | 0.5 | Must (blocker) | 🟡 In progress — 3 commits landed (Mon/Tue/Wed), 20 files still dirty Thu/Fri |
| S2-02 | Decouple `Weapon` ↔ `WeaponHolder`/`AbilityHolder` (push-on-equip) | 1.0 | Must | ✂️ Cut — carry to Sprint 3 |
| S2-03 | `Core.GetCoreComponent<T>()` + self-register + lazy-cache (OCP) | 1.0 | Must (cut-first if slipping) | 🟡 In progress — API exists, named props commented out; still uses LINQ (violation) |
| S2-04 | Fix Bug #9 — AnimationPlayerController double-registration | 0.25 | Must | ⬜ Not started — final day |
| S2-05 | Fix Bug #4 — `WeaponMelee.Attack()` empty foreach | 0.25 | Must | ⬜ Not started — final day 🔴 |
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

### Thu 18/06 — elapsed
- **Planned**: S2-01 commit + S2-05 + S2-04 + S2-03.
- **Actual**: **0 code commits**. Working tree still has 20 dirty files. No sprint tasks completed.
- **Note**: Only activity was the automated standup commit `280f964`. Developer absent or did not commit. This is the second consecutive non-productive day. Sprint outcome now severely at risk.

### Fri 19/06 — TODAY 🔴 FINAL DAY
**Goal: Triage sprint — land S2-05 + S2-04 at minimum. Commit dirty tree. Run wrapup.**
| Order | Task | Est | Why now |
|-------|------|-----|---------|
| 1 | **Commit dirty working tree (S2-01)** | 0.25d | Prerequisite — 20 files uncommitted since Wed |
| 2 | **S2-05** — `WeaponMelee.Attack()` add `INegativeReceiver.TakeDamage()` inside foreach | 0.25d | 🔴 FINAL CHANCE — combat not testable without this |
| 3 | **S2-04** — Fix Bug #9: `AnimationPlayerController` line 21 `StartAnimation` → `EndAnimation`; fix `OnDisable` line 29 mirror | 0.25d | Quick 1-line fix; unblocks ability Exit |
| 4 | **S2-03** — Replace LINQ `OfType<T>().FirstOrDefault()` with foreach in `Core.GetCoreComponent` | 0.25d | Acceptance criteria violation + zero-alloc rule |
| 5 | Smoke-check: equip → attack → enemy hit | — | Per Definition of Done for S2-05 |
| 6 | `/weekly-wrapup` — close sprint, code review, retrospective | — | Friday end-of-sprint ritual |
- S2-02 officially **CUT** → Sprint 3 backlog.
- Day load: ~1.0d of actual coding. Achievable if developer is present.

---

## Risks (live)
| Risk | Status | Mitigation |
|------|--------|------------|
| S2-05 not done (combat not testable) | 🔴 CRITICAL | Must land Fri — **5th consecutive day of unplayable combat** |
| S2-01 dirty working tree uncommitted | 🔴 Active | 20 files since Wed; must commit before any new work |
| S2-02 not started after full sprint | 🔴 Active | ✂️ CUT → Sprint 3 backlog |
| S2-03 LINQ violates acceptance criteria | 🟡 Open | Replace with foreach Fri — still a quick fix |
| S2-04 Bug #9 AnimationPlayerController | 🟡 Open | 1-line fix; achievable today |
| Combo input buffer untested (Play Mode needed) | 🟡 Watch | `SetBufferAttack()`/`BufferIsAttack` present but untested in Play Mode |
| S2-06 (EditMode test) cut | 🔴 Active | TD-014 (zero automated tests) persists; Sprint 3 backlog |
| Developer absence Thu | 🔴 New | Zero commits on Thu. Sprint capacity effectively 3/5 days used productively |

---

## Daily Log
> Owner reports what was done; PM updates estimates/status here each session.

- **2026-06-17 (Wed)**: Tracker created. Sprint flagged AT-RISK.
- **2026-06-18 (Thu 02:00 standup)**: Reviewed Wed commits. `3c9f9b7` confirmed (StatusAnimation infra, Core.GetCoreComponent partial). S2-05 and S2-04 still open. Working tree dirty. S2-06 cut. Plan revised — S2-05 is Thu's #1 priority.
- **2026-06-19 (Fri 02:00 standup)**: Zero code commits on Thu. Working tree still has 20 dirty files. All must-haves (S2-05, S2-04, S2-03) still open. S2-02 officially cut → Sprint 3. Sprint closes today. See standup digest below.

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

---

## Standup Digest — 2026-06-19 (Fri) — NGÀY CUỐI SPRINT

### Hôm qua làm gì (Yesterday — Thu 18/06)

**Không có commit code nào.** Duy nhất là commit standup tự động `280f964 chore(standup)` lúc 12:04 UTC+7.

Working tree (cây công việc) vẫn có **20 file bị sửa nhưng chưa commit** — đúng như trạng thái cuối Wed 17/06:
- `PlayerInputHandle.cs` — `BufferIsAttack` property + `SetBufferAttack()` + combo logic trong `OnAttack()` ✅ đã viết nhưng chưa commit
- `WeaponMelee.cs` — foreach body **vẫn rỗng** (Bug #4 chưa fix) 🔴
- `AnimationPlayerController.cs` — dòng 21 **vẫn** đăng ký `StartAnimation` thay vì `EndAnimation` (Bug #9 chưa fix) 🔴
- `Core.cs` — `GetCoreComponent<T>()` **vẫn** dùng LINQ `OfType<T>().FirstOrDefault()` (vi phạm zero-alloc rule) 🟡

Developer vắng mặt (absent) cả ngày Thu — không có hoạt động code nào.

### Hôm nay làm gì (Today — Fri 19/06) + Estimates

Ngày cuối sprint. Mục tiêu: cứu vớt tối đa có thể.

| # | Việc cần làm | Est | Mức độ |
|---|--------------|-----|--------|
| 1 | **Commit dirty tree (S2-01)** — `git add` + commit 20 file còn lại | 0.25d | 🔴 PHẢI LÀM TRƯỚC |
| 2 | **S2-05** — Thêm `INegativeReceiver.TakeDamage(currrentSA.attackDamege, transform.position)` vào `WeaponMelee.Attack()` foreach | 0.25d | 🔴 CRITICAL — ngày cuối cùng để fix combat |
| 3 | **S2-04** — `AnimationPlayerController.cs` dòng 21: `StartAnimation` → `EndAnimation`; dòng 29 OnDisable: mirror fix | 0.25d | 🟡 Quick win, 2 dòng |
| 4 | **S2-03** — `Core.GetCoreComponent<T>()`: thay LINQ bằng `foreach` loop | 0.25d | 🟡 Acceptance criteria + zero-alloc |
| 5 | Smoke-check (kiểm tra nhanh): equip → attack → enemy hit | — | Advisory |
| 6 | `/weekly-wrapup` — đóng sprint, code review, retrospective | — | Cuối ngày |

### Blockers & Rủi ro hôm nay

- 🔴 **Combat vẫn không testable (không thể kiểm tra)** — WeaponMelee.Attack() foreach rỗng suốt cả sprint. Đây là ngày cuối cùng, không có ngày mai.
- 🔴 **S2-02 officially CUT** — decouple (tách) Weapon ↔ WeaponHolder/AbilityHolder mang sang Sprint 3 backlog (danh sách tồn).
- 🟡 **Working tree 20 files uncommitted** — developer cần commit trước khi làm bất cứ thứ gì khác.
- 🟡 **Sprint velocity (vận tốc)** thực tế ~0.5d / 4.0d ước tính = **12.5%** — cần retrospective (nhìn lại) nghiêm túc.
- 🟡 **Combo input buffer** (`SetBufferAttack`) đã viết nhưng chưa có Play Mode test — mang sang Sprint 3 để verify.
