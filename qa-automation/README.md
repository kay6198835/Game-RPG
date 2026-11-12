# QA automation

Weekly automated test run for Game-RPG. It runs every **Friday 23:00**, before `/weekly-wrapup` (Saturday
22:00) reads its result.

Each run does three things:

- **A. Regression:** every P0 and P1 case.
- **B. Change scan:** works out what landed on the sprint branch since the last good run, then runs the P2
  cases of the systems that changed and of the systems wired to them. It also compares the results with
  the previous run.
- **C. New cases:** the Friday Claude task writes new cases for the week's new features.

## Folders

| Folder | Why it exists |
|---|---|
| `qa-automation/` | Everything Unity does not need: scripts, config, case catalog, manual checklist, reports, state. It sits outside `Assets/`, so Unity never imports it and the Editor is not slowed down. |
| `Assets/QA/` | Only what Unity must compile or load. `Editor/` holds the runner, the scene builder and the static checks. `Runtime/` holds the harness and the suites, all inside `#if UNITY_EDITOR` so they never ship in a build. `Scenes/` holds the QA scenes, rebuilt from code on every run and git-ignored. |

Assembly definitions (BUG-084, added on `chore/asmdef-test-setup`, 2026-11-12): gameplay code is the `GameRPG`
assembly (`Assets/Script/GameRPG.asmdef`), and EditMode unit tests live in `Assets/Tests/EditMode/`
(`GameRPG.EditModeTests`). The QA code under `Assets/QA/` still compiles into the default `Assembly-CSharp` /
`Assembly-CSharp-Editor` assemblies, which auto-reference `GameRPG`. No gameplay file, scene, prefab or
ScriptableObject is changed.

The weekly run tests the newest `origin/sprint-NN`. The asmdef files and `Assets/Tests/` only count once that
branch contains them; on an older sprint the unit-test step is skipped and logged.

**EditMode unit tests (step 5b of `run-weekly.ps1`).** After the compile check the script runs
`Unity -batchmode -runTests -testPlatform EditMode` when `Assets/Tests/EditMode/` exists. Every NUnit test
becomes one case `QA-UNIT.<Class>.<Test>` (system `UNIT`, group `UNIT`, priority P0, so it always runs). Failed
= FAIL (counts in the exit code), Skipped/Ignored = SKIP. If Unity writes no `unit-results.xml`, one
`QA-UNIT-RUNNER` FAIL is reported. Raw NUnit XML: `reports/<run>/unit-results.xml`.

| File | Role |
|---|---|
| `scripts/run-weekly.ps1` | The one entry point. Pass it `-Since <ref>` to override the baseline, `-Deep ALL` to run every P2, `-MaxPriority P2` to run everything. |
| `scripts/scan-changes.ps1` | Git range, then systems, then interaction risks, written to `weekly-change-impact.md` and `change-impact.json`. |
| `scripts/diff-results.ps1` | KNOWN mapping, diff with the previous run, flaky detection, written to `report.md`, `summary.json` and `state/history.jsonl`. |
| `config/impact-map.json` | Maps a path to a system, lists the interaction risks, and maps each system to its scene group. |
| `config/known-issues.json` | Maps a case to an open bug file, so the case reports KNOWN instead of FAIL. |
| `cases/case-catalog.md` | Every case ID, its priority and the expected result. |
| `manual/manual-weekly-checklist.md` | The cases you test by hand. |
| `state/last-success.json` | The sprint commit tested by the last good run. It is the baseline for the next change scan. |
| `state/latest.json` | Status of the last run, including SKIPPED and errors. |
| `state/history.jsonl` | One line per run. Used for the previous-run diff and for flaky detection. |
| `reports/<yyyymmdd-hhmm>/` | `report.md`, `results.json` (raw Unity output), `unity-report.md`, `summary.json`, `weekly-change-impact.md`, `change-impact.json`, `run-info.json`, `run.log`. |

## Running it by hand

Close the Unity Editor first. The script refuses to run while an Editor is open.

```powershell
git -C D:\Fork\Game-RPG show qa/weekly-automation:qa-automation/scripts/run-weekly.ps1 | Out-File -Encoding utf8 $env:TEMP\qa-run-weekly.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File $env:TEMP\qa-run-weekly.ps1
```

The script is copied out of the branch because your own branch may not contain it.

What the script does to your folder `D:\Fork\Game-RPG`:

1. If the working tree has changes, it commits them on your current branch as `coding need check`. The commit is local and never pushed.
2. It checks out `qa/weekly-automation` and merges the newest `origin/sprint-NN` into it, locally.
3. It runs the tests, then puts back any tracked file the run modified outside the QA folders.
4. It commits the report on `qa/weekly-automation`, checks out your branch again, and pushes
   `qa/weekly-automation` to `origin` as an off-machine backup (never forced; a failed push is logged
   and does not change the exit code).
5. When the tests ran (status PASS or FAIL, so the code compiled), it merges `qa/weekly-automation` into
   the sprint branch it tested, in the PM worktree `D:\Fork\Game-RPG-pm`, and pushes the sprint branch.
   Every new sprint is cut from the previous one, so it carries the QA code and reports. Never forced; a
   conflict, a dirty PM worktree or a twice-rejected push leaves the sprint branch untouched and is
   logged as `SPRINT MERGE FAILED`. `-NoSprintMerge` turns it off for one run.

Your own branch is never pushed.

To undo the auto-commit while keeping your changes, run this on your branch: `git reset --soft HEAD~1`.

From inside the Editor, use `Tools > QA > Automation > Run P0 + P1` or `Run Everything (P0-P2)`. Results go to
`qa-automation/reports/manual-<time>/`. `unity-report.md` holds the raw verdicts; the KNOWN mapping and the
diff come only from the script.

The Unity command line used by the script:

```text
Unity.exe -projectPath <project> -executeMethod QA.Automation.Editor.QARunner.RunFromCommandLine
  -qaOut <dir> -qaMax P1 [-qaDeep ABL,VIT|ALL] [-qaGroups STATIC,SYSTEMS,VITALS,ENEMY,PLAYER] [-qaSeed N] -logFile <log>
```

Do not pass `-batchmode`, because Play Mode needs the normal player loop. Do not pass `-quit`, because the
runner exits by itself.

## Exit codes

| Code | Meaning | What to do |
|---|---|---|
| 0 | No unknown FAIL. KNOWN, SKIP and OBSERVED cases are allowed. | Nothing. |
| 1 | At least one unknown FAIL. | Read "New FAIL" in `report.md`. If it is a defect, add an inbox note. |
| 2 | The runner itself failed: git, merge conflict, Unity crash or timeout, or no `results.json`. | Read `run.log` and `%LOCALAPPDATA%\GameRPG-QA\logs\`. |
| 3 | Skipped because a Unity Editor was open. Nothing was touched. | Close the Editor and run it by hand. |
| 4 | Compile error. No test ran. | `report.md` lists the `error CS` lines. Add an inbox note: an S1 compile break goes first in the standup. |

Unity itself exits with 0 (no FAIL) or 1 (FAIL in raw verdicts), or 2 if the runner fails.

## Reading the report

| Verdict | Meaning |
|---|---|
| PASS | Met the expected result. |
| FAIL | Did not meet it, and no open bug explains it. Needs a look. |
| KNOWN | Fails because of an open bug file named in the Bug column. Not counted in the exit code. |
| SKIP | Cannot be tested as built. The reason is in Details, for example "Effects list is empty". |
| OBSERVED | Behaviour recorded where the design is undecided. Never a failure. |

Sections of `report.md`:

1. Totals.
2. Compared with the previous run: new FAIL, fixed, still failing, flaky suspects.
3. Changed this week: the cases of the touched systems, and new scripts that have no case yet.
4. Hygiene: files the run modified and the runner reverted.
5. Every result, grouped by scene.

A KNOWN case whose bug file says **Fixed** turns back into FAIL, with the note "regression?".

## Adding a case

Follow "Adding a case" in `cases/case-catalog.md`:

1. Add a row to the catalog.
2. Add one `Case(...)` line to the suite.
3. Assert with `Check()`.
4. Use `Instantiate` copies of ScriptableObjects only, and clean up in `Cleanup()`.

For a new area that needs its own scene, add a group:

1. Add a build method in `QASceneBuilder`.
2. Add the group to `PlayModeGroups` and `QARunner.AllGroups`.
3. Map its systems in `config/impact-map.json` under `systemToGroup`.

## Bugs found by the run

The run never creates a bug ID. A real defect becomes a `NOTE-YYYYMMDD-N` row in
`production/qa/bug-inbox.md`, with Evidence tier `RUNTIME` for Play Mode cases and `STATIC` for asset
checks. The Friday Claude task writes it. Only `/weekly-wrapup` turns a note into `BUG-NNN`. After that,
add the case to `config/known-issues.json` so it reports KNOWN.

## Flaky cases

A case is a **flaky suspect** when its result flipped between pass and fail at least twice in the last 5
runs.

- Game time is fixed at 60 fps (`Time.captureFramerate`), and each case seeds `Random` from its ID, so a
  flip usually means real timing sensitivity in game code or an animation length change.
- To check, run the group three times from the Editor. If the result varies with no code change, make
  the wait condition-based (`WaitUntil` on state, not a fixed `Wait`). Note it in the case's catalog row.
- To quarantine a flaky case, lower it to P2 in the suite and the catalog. Never delete it.

## Determinism and hygiene rules

- Fixed seed per case. Game time is fixed at 60 fps. Waits time out in game time, and each case also has
  a 45 s real-time limit.
- ScriptableObjects are only ever used through `Object.Instantiate` copies. The player and enemies run on
  the real assets as the game does, and their in-memory changes are checked (QA-STAT-10, QA-ENT-06).
- After each run the script checks `git status`. Any tracked file modified outside `qa-automation/` and
  `Assets/QA/` is reverted and listed in the report.
