# Demo Checklist — open items

> Owner of the demo checklist since 2026-11-19 (moved out of root `CLAUDE.md`; full history of done items:
> `docs/archive/CLAUDE-history-2026-11-19.md` › "Demo Completion Checklist").
> Demo target: start menu → dungeon run (movement + melee + 2 abilities + enemies + room progression) →
> death / restart. Remove an item when it is done and record it in `docs/CHANGELOG-DOCS.md`.
> Bug status always comes from `production/qa/bugs/` (index: `production/qa/open-bugs.md`).

1. **Player death → Game Over → restart:** BUG-122, then BUG-086, BUG-065, BUG-087 (`GameManager` as a
   DI-registered scene component, not a singleton).
2. **Between-room upgrade:** pause after room clear, offer 3 stat cards (`StatPointSession` mechanics).
3. **Player build:** run one Player build — checks the room JSON load (Bug #15 / TD-022, fixed in code
   `7a41d7c4`), the BUG-126 removal (`070bc0a5`, status per its file) and the pending BUG-116 check.
4. **Melee balance:** BUG-125.
5. **Abilities v2:** set `layerMask` on `Lightning.prefab` (BUG-072); Play Mode smoke of all four Paladin
   abilities; then BUG-071 / BUG-083 / BUG-068 / BUG-079. Write the v2 GDD + an ADR recording the v1 removal.
6. **Paladin ability animations:** Blessed Slash, Blessing, Avatar of Light per-direction clips.
7. **Enemy spawn:** `overflowPercent` unread; `retry > 4` fallback breaks the ADR-0003 budget.
8. **Range weapon cooldown:** BUG-103.
9. **Serialized-rename guard:** decide a guard (BUG-105 / 108 / 121 pattern).
10. **Play Mode checks pending:** BUG-063, BUG-064, BUG-107, BUG-109, BUG-115, BUG-124 (fixed in code);
    PlayMode test assembly (`Assets/Tests/PlayMode`) not created.
11. **HUD leftovers:** EXP bar hidden, minimap placeholder, skill tree has no gameplay source.
12. **Historical IDs still open:** #14 (`MazeController.Awake()` missing `return`), #16 (= BUG-097),
    #17 (dead door-gating code: `DoorController.OpenDoor()` / `RoomCell.UpdateStatusDoor()`).
