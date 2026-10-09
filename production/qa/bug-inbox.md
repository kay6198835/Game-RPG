# Bug Inbox

> Suspected defects noted during the week. **No bug IDs are created here.** `/weekly-wrapup`
> triages every open note on Saturday night and is the only writer allowed to allocate a `BUG-NNN`.
> Rules: `.claude/rules/bug-inbox.md`. Started 2026-10-09.

## Open notes

| Note | Date | Source | Sev guess | Where | What | Evidence tier |
|---|---|---|---|---|---|---|
| NOTE-20261009-1 | 2026-10-09 | fix-survival | S1 | `Assets/Script/Map/Room/RoomGeneraterController.cs:134` | Fix `40d2c793` for BUG-096 (open doors at once in rooms with no spawn marker) is GONE at HEAD `221d54be`: 1 of 4 added lines remains. Removed by `ac13ee4f` "done load map." (owner, same evening, inside a large map commit). Intentional or accidental? Relates to BUG-096 / BUG-097 | STATIC |
| NOTE-20261009-2 | 2026-10-09 | editor-log | S3 | `Assets/Script/Character/Player/PlayerState.cs:35` | `NullReferenceException` with this as first project frame, once, in `Editor-prev.log` (session ending 2026-10-09 13:56). At HEAD line 35 is the `Debug.Log` in `PlayerState.Enter()`, which cannot throw — the trace is likely from an uncommitted version of the file; `player.Anim` (`:32`) is the plausible null | LOG |
| NOTE-20261009-3 | 2026-10-09 | editor-log | S3 | `Assets/Script/UIFlow/Gameplay/GameplayUIController.cs:55` | `error CS0246: 'InputSystemUIInputModule' could not be found` in `Editor-prev.log`. The 2026-10-09 17:00 batchmode compile of HEAD `221d54be` passed (0 errors), so this was likely a transient state of the owner's working tree — confirm it does not recur | LOG |

## Triaged notes

| Note | Triaged on | Outcome | Result (bug ID / commit / reason) |
|---|---|---|---|
