#!/usr/bin/env python3
"""Static lint for Mermaid blocks in Markdown files.

Catches the syntax mistakes that have actually broken diagrams in this repo, without needing
Node / mermaid-cli. It is a pattern check, not a full parser: a clean run does not prove a
diagram renders, but every finding is a real render failure (or very close to one).

Usage:
    py .claude/hooks/lint-mermaid.py [file.md ...]      # no args = every tracked *.md
Exit code: 0 = clean, 1 = findings printed as  path:line: [rule] message
"""
import io
import re
import subprocess
import sys

DIAGRAM_TYPES = ("sequenceDiagram", "flowchart", "graph", "classDiagram", "stateDiagram",
                 "stateDiagram-v2", "erDiagram", "gantt", "pie", "journey", "mindmap", "timeline")

# Unquoted node label: ID[...], ID(...), ID{...}, ID([...]), ID[[...]] — content not starting with "
NODE_LABEL = re.compile(r'\b[A-Za-z_][\w-]*\s*(\[\[|\[\(|\(\[|\(\(|\[|\(|\{)(?!")([^\]\)\}]*)')
GENERIC_ANGLE = re.compile(r'\b\w+<\w[\w, ]*>')


def lint_block(path, start, kind, lines, out):
    """lines: list of (lineno, text) inside one ```mermaid block (fences excluded)."""
    for n, raw in lines:
        s = raw.strip()
        if not s or s.startswith("%%"):
            continue

        if kind == "sequenceDiagram":
            # ';' ends a statement in a sequence diagram: "A->>B: x; y" is cut after "x".
            if ";" in s:
                out.append((path, n, "seq-semicolon",
                            "';' ends a statement in sequenceDiagram — use ',' or '#59;'"))
            # Unbalanced braces/parens are fine here; '#' starts an entity code (#59;, #35;).
            if re.search(r'#(?!\d+;|[a-z]+;)', s.split(":", 1)[-1]) and ":" in s:
                out.append((path, n, "seq-hash",
                            "bare '#' in a message starts an entity code — write '#35;'"))

        elif kind in ("flowchart", "graph"):
            # Quoted labels may contain anything; check only what is outside "…" and |…|.
            bare = re.sub(r'"[^"]*"', '""', s)
            bare = re.sub(r'\|[^|]*\|', '||', bare)
            for m in NODE_LABEL.finditer(bare):
                opener, text = m.group(1), m.group(2)
                if any(c in text for c in "()[]{}"):
                    out.append((path, n, "flow-unquoted-label",
                                f"label after '{opener}' has brackets but is not quoted — wrap it in \"…\""))
                    break
            # 'end' as a bare node id breaks flowcharts (it closes a subgraph)
            if re.search(r'(^|[\s>-])end\s*(-->|---|-\.|==>)', s):
                out.append((path, n, "flow-end-node", "node id 'end' is reserved — rename it"))

        elif kind == "classDiagram":
            relation = re.search(r'(<\|--|--\|>|\*--|--\*|o--|--o|-->|<--|\.\.>|<\.\.|\.\.\|>|<\|\.\.|--|\.\.)', s)
            stereotype = s.startswith("<<") or "<<" in s and ">>" in s
            if not relation and not stereotype and GENERIC_ANGLE.search(s):
                out.append((path, n, "class-generic-angle",
                            "generic written with <> — classDiagram needs ~ (List~T~)"))


def lint_file(path, out):
    try:
        text = io.open(path, encoding="utf-8").read()
    except (OSError, UnicodeDecodeError):
        return
    in_block = False
    block, kind, start = [], None, 0
    for i, line in enumerate(text.split("\n"), 1):
        stripped = line.strip()
        if not in_block and stripped.startswith("```mermaid"):
            in_block, block, kind, start = True, [], None, i
            continue
        if in_block and stripped.startswith("```"):
            lint_block(path, start, kind, block, out)
            in_block = False
            continue
        if in_block:
            if kind is None and stripped and not stripped.startswith("%%"):
                kind = next((t for t in DIAGRAM_TYPES if stripped.startswith(t)), "unknown")
                if kind.startswith("stateDiagram"):
                    kind = "stateDiagram"
                continue
            block.append((i, line))
    if in_block:
        out.append((path, start, "unclosed-fence", "```mermaid block is never closed"))


def main(argv):
    files = argv[1:]
    if not files:
        files = subprocess.run(["git", "ls-files", "--cached", "--others", "--exclude-standard", "*.md"],
                               capture_output=True, text=True, encoding="utf-8").stdout.splitlines()
    out = []
    for f in files:
        if f.endswith(".md"):
            lint_file(f, out)
    for path, n, rule, msg in out:
        print(f"{path}:{n}: [{rule}] {msg}")
    return 1 if out else 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))
