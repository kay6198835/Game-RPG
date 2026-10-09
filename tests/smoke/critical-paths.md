# Critical Paths — Weekly Smoke List

> The fixed part (~10 min) of the Saturday playtest. Same list every week so results compare week to
> week. Read by `.claude/scripts/playtest-sheet.sh` (rows starting with `| <number> |`) and by
> `/smoke-check`. Change a row only when the game's critical path changes, and log the change in
> `docs/CHANGELOG-DOCS.md`.
>
> Start from `MainGamePlay` (build index 0) in Play Mode, not from `LoadRandomMap`, so the menu,
> loading and `GameplayUI` scenes are exercised.

| # | Step | Expected |
|---|---|---|
| 1 | Enter Play Mode on `MainGamePlay`; splash → main menu | Menu shows; no exception in Console |
| 2 | New Game → loading → `LoadRandomMap` + `GameplayUI` | Dungeon and HUD (HP/Mana bars, 4-slot hotbar) appear |
| 3 | First room: interact (G) with a champion monument, confirm | Confirm panel opens; character data changes; doors of the first room open |
| 4 | Melee attack (LMB) an enemy until it dies | Hit reaction, HP bar drops, enemy death animation, no exception |
| 5 | Press abilities `1` `2` `3` `4` | Each casts; hotbar shows cooldown; Mana/HP cost applied |
| 6 | Ranged weapon (if equipped) fires at an enemy | Projectile hits and deals damage |
| 7 | Clear all enemies in a combat room | Doors open |
| 8 | Walk through a door | Next room loads; player placed at the entry door; minimap avatar moves |
| 9 | Let the player die | Death animation; game-over panel; no Console spam |
| 10 | Respawn / return from game-over | Scene reloads to a playable state |
