# EditMode Tests — Location Note

Unity only compiles C# that lives under `Assets/` or `Packages/`. Anything in this
repo-root `tests/` folder is invisible to the Unity compiler and to the Test Runner.

This project also has **no assembly definition files** — all gameplay code is compiled
into the predefined `Assembly-CSharp` assembly. An `.asmdef` test assembly cannot
reference a predefined assembly, so tests in their own asmdef would not be able to see
`StatsSO`, `Stat`, `GameConstants`, etc.

The only working location is therefore a folder named `Editor`, whose contents Unity
compiles into `Assembly-CSharp-Editor` — that assembly already references both
`Assembly-CSharp` and the NUnit / Test Framework assemblies.

| Suite | Path |
|---|---|
| StatsSO (EditMode) | `Assets/Editor/Tests/StatSystem/StatsSOTests.cs` |
| Shared test factory | `Assets/Editor/Tests/StatSystem/StatsProfileFactory.cs` |

Run them via **Window ▸ General ▸ Test Runner ▸ EditMode**.

Tests marked `[Category("KnownBug")]` describe the *documented* behaviour and fail
today because of open bugs; they turn green when the bug is fixed. Exclude them from a
CI run with `-testCategory "!KnownBug"`.

If assembly definitions are ever introduced for `Assets/Script/`, these suites should be
moved into a proper `Tests.EditMode` asmdef and this note removed.
