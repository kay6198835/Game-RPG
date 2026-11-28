---
# Mermaid diagram syntax rules for every Markdown document (GDDs, ADRs, diagrams, system docs)
paths:
  - "**/*.md"
---

# Mermaid Diagram Standards

A Mermaid syntax slip does not show as a warning — the whole diagram is replaced by a parse error.
These rules come from errors that actually shipped in this repo.

## Verify before you finish

- Run `py .claude/hooks/lint-mermaid.py <file.md>` (no argument = every Markdown file) after writing
  or editing any ```` ```mermaid ```` block. Exit 0 = no known pitfall found.
- The commit hook (`.claude/hooks/validate-commit.sh`) runs the same linter on staged `.md` files
  and **blocks the commit** on a finding.
- The linter is a pattern check, not a parser. A clean run does not prove the diagram renders; when
  you add an unusual construct, preview it (GitHub, VS Code Markdown preview or mermaid.live).

## sequenceDiagram

- **Never put `;` in a message or note.** `;` ends a statement, so `A->>B: x; y` is cut after `x`
  and `y` becomes a broken line (`Parse error … got 'NEWLINE'`). Use `,` or `—`, or `#59;`.
  *Shipped once:* `character-architecture-diagrams.md` §10, fixed 2026-10-05.
- A bare `#` in a message starts an entity code — write `#35;`.
- Every arrow needs a message: `A->>B: text`. Arrows: `->>` call, `-->>` return, `-)` async.
- `loop` / `alt` / `opt` / `par` / `rect` blocks must each close with `end`.

## flowchart / graph

- **Quote any node label that contains `( ) [ ] { }`**, or punctuation in general:
  `N["SpawnEffectBase.Apply()"]`, not `N[SpawnEffectBase.Apply()]`.
- Line breaks inside a label: `<br/>` inside a quoted label.
- Do not use `end` as a node id (it closes a `subgraph`); use `END_NODE` or `Done`.
- Edge labels: `A -->|"text"| B` — quote them if they contain brackets or `|`.
- Node ids: letters, digits, `_` only. Put the human text in the label.

## classDiagram

- **Generics use `~`, not `<>`:** `+List~AbilityBinding~ Bindings`. `List<T>` breaks the class body.
- Stereotypes are `<<interface>>` / `<<abstract>>` on their own line inside the class body.
- Method signatures: `+Apply(AbilityContext ctx) void` — no default values, no `=`.

## stateDiagram

- Use `stateDiagram-v2`. Start/end is `[*]`. State names with spaces need `state "Long name" as S1`.

## All diagrams

- Open with ```` ```mermaid ```` and close with ```` ``` ````; the first line inside must be the diagram
  type (`sequenceDiagram`, `flowchart TD`, `classDiagram`, …).
- Use straight quotes `"`, never curly quotes, inside Mermaid.
- Comments are `%% text` on their own line.
- Keep source line references (`File.cs:42`) in quoted labels or messages, never in node ids.
