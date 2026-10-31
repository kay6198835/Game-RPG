# Object Pooling — Changelog

Newest first. Entries before 2026-10-05 reconstructed from `CLAUDE.md` history and `git log`.

## 2026-10-31 — Pool interface renamed (doc-truth re-sync)
- **Commit:** `cae75317` (on `origin/sprint-20`)
- **Changed:** `IObjecPoolService` → `IObjectPoolService`; file name kept.
- **From → To:** "typo in name is intentional" → interface fixed, only the file keeps the typo.
- **Why:** not recorded in the commit.
- **Bugs:** none.

## 2026-09-28 — Release reparent fix
- **Commit:** `b7a0af5e`
- **Changed:** `Pool.Release()`
- **From → To:** `gameObject.transform.parent.SetParent(...)` (moved the object's **parent** into the pool) → `gameObject.transform.SetParent(...)` (moves the object itself)
- **Why:** released objects' parents (e.g. a room or the weapon owner) were being reparented under the pool (inferred from the diff; commit message "update architecture projectile").
- **Bugs:** none filed — found and fixed in the same commit.

## 2026-09-04 / 09-09 — Rename and DI injection
- **Commits:** `06866196`, `853fe39b`, `4e4eff59`
- **Changed:** folder and injection
- **From → To:** `Assets/Script/Poolable/` → `Assets/Script/System/PoolableService/`; pool instances injected by `resolver.InjectGameObject` on first instantiate
- **Why:** VContainer adoption (ADR-0004) — pooled objects cannot be registered, so they are injected at spawn.
- **Bugs:** none.
