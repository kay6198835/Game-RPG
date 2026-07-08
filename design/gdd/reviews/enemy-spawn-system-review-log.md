# Review Log — Enemy Spawn & Per-Room Management System

Revision history for `design/gdd/enemy-spawn-system.md`. Newest entry first.

---

## Review — 2026-07-08 — Verdict: NEEDS REVISION → revised same session (index marked Approved)
Scope signal: L
Specialists: systems-designer, game-designer, level-designer, qa-lead (ai-programmer terminated early — API session limit; creative-director synthesis by main reviewer)
Blocking items: 10 | Recommended: 8
Summary: Structurally complete (8/8 sections) with honest dependency accounting, but the selection algorithm had two real correctness holes — a non-terminating Phase-2 loop on `weight ≤ 0` and an undefined `argmin` tie-break that voided the doc's own determinism AC — plus an unspecified seed-injection path, an unfalsifiable "variety verified statistically" AC, and an under-specified spawn-placement pipeline (RoomType→RoomData mapping colliding with map-system Bug #16, `Tile_Spawn` parser unowned with markers absent from all 13 JSONs, no entry-safety/jitter fairness floors). Revised in-session: added `weight ≥ 1` hard invariant + termination guarantee, pinned tie-break (earliest in `idEnemy`), injected `System.Random rng` parameter, concrete 50-seed variety AC, RoomType→RoomData table with zero-budget non-combat rooms, entry-safety + bounded-jitter placement rules, foreign-event guard AC, and a full AC rewrite split by test type. Owner decisions: keep current greedy fill (composition-diversity → Should-Have), flat difficulty for demo (run-depth escalation → post-demo), RoomType→RoomData table. Two items deferred as decide-this-sprint: EnemyManager singleton ADR (Open Q#1, gates PlayMode test harness) and runtime seed source (Open Q#2) — both flagged in the Sprint 4 tracker for a daily nudge.
Prior verdict resolved: First review
