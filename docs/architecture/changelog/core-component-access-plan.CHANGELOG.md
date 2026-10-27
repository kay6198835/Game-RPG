# Changelog — core-component-access-plan.md

## 2026-10-27 — Created

- **What:** new plan for splitting core-component interfaces into a non-generic external interface and
  a generic `<out TCore>` internal interface, plus removing `ICore Core` from the non-generic `ICharacter`.
- **Why:** owner request — internal-only members were reachable through the same interfaces as the
  external API, and nothing tied a sibling lookup to its own core type.
- **From → to:** no document → plan approved by the owner, not implemented.
- **Cause:** design discussion read against source at HEAD `05dcf63`; no `.cs` file changed.
