# Periodic Review Schedule

> **Project**: Unity Action Roguelike RPG
> **Created**: 2026-05-19
> **Review mode**: lean

---

## Weekly Review

### Monday — Sprint Kickoff (~15 min)

Run at the start of each work week.

```
/sprint-status
```
→ Check what was completed last week and what carries over.

```
/scope-check
```
→ Verify no unplanned scope has crept in since last week.

---

### Friday — Wrap-Up (~20 min)

Run at the end of each work week.

```
/playtest-report
```
→ Log the week's Unity playtest session findings. Even a brief entry keeps a record.

```
/code-review Assets/Script/[files changed this week]
```
→ Review any `.cs` files modified during the week. Focus on the most complex changes.

```
/bug-triage
```
→ Re-prioritize open bugs. Assign blockers to next sprint, defer non-blockers.

---

## Monthly Review

Run on the 28th–30th of each month (~2 hours total).

### Part 1 — Design Health (~45 min)

```
/consistency-check
```
→ Verify GDDs don't contradict each other after any code or design changes.

```
/review-all-gdds
```
→ Cross-review all GDDs for new contradictions, balance issues, or pillar drift.
→ *Skip if no GDDs were added or significantly changed this month.*

---

### Part 2 — Architecture & Debt (~30 min)

```
/architecture-review
```
→ Check that ADRs still cover all GDD technical requirements.

```
/tech-debt
```
→ Scan codebase for accumulated technical debt. Prioritize what to repay next sprint.

---

### Part 3 — Performance (~15 min)

```
/perf-profile
```
→ Profile game in Unity Play Mode. Check FPS, frame time, memory against budgets:
- Target: 60 FPS, ≤16.7ms frame time, ≤256MB memory, ≤100 draw calls

---

### Part 4 — Milestone & Phase (~30 min)

```
/milestone-review
```
→ Feature completeness, quality metrics, risk assessment, go/no-go verdict.

```
/gate-check [current-phase]
```
→ Check if ready to advance to the next development phase.
→ Current phase: **Production** → next gate: **Polish**

---

## Quick Reference

| Cadence | When | Skills |
|---------|------|--------|
| Weekly Mon | Start of week | `/sprint-status`, `/scope-check` |
| Weekly Fri | End of week | `/playtest-report`, `/code-review`, `/bug-triage` |
| Monthly | Day 28-30 | `/consistency-check`, `/review-all-gdds`, `/architecture-review`, `/tech-debt`, `/perf-profile`, `/milestone-review`, `/gate-check` |

---

## Notes

- `/review-all-gdds` is expensive (reads all GDDs) — only run monthly or when a GDD changes significantly.
- `/playtest-report` can be a brief note if the session was short — consistency matters more than length.
- If a monthly review finds a blocking issue, open a bug in `production/qa/bugs/` immediately.
