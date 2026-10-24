#!/usr/bin/env bash
# bug-id.sh — the single bug-ID authority helper. Read-only.
#
#   bug-id.sh next    Print the next free ID (max BUG-NNN in production/qa/bugs/ + 1, 3 digits).
#                     Only /weekly-wrapup may use this (see .claude/rules/bug-inbox.md).
#   bug-id.sh audit   Markdown report: IDs cited in living docs with no file, bug files with no
#                     **Status** line, CLAUDE.md Known-Bugs rows whose status word disagrees with the file.
#
# Run from the repo root (or pass REPO=<path>).
set -u
REPO="${REPO:-.}"
BUGS="$REPO/production/qa/bugs"

max_id() {
  ls "$BUGS" 2>/dev/null | sed -n 's/^BUG-\([0-9][0-9]*\)\.md$/\1/p' | sed 's/^0*//' | sort -n | tail -1
}

status_of() { # $1 = bug file; prints the **Status** value (first 60 chars) or empty
  grep -m1 -E '^\*\*Status\*\*' "$1" 2>/dev/null | sed -E 's/^\*\*Status\*\*:? *//' | cut -c1-60
}

classify() { # status text -> the status keyword that appears FIRST (later text is usually history)
  printf '%s' "$1" | tr '[:lower:]' '[:upper:]' | awk '
    { best=""; bp=1e9
      n=split("ACCEPTED:ACCEPTED DEFER:ACCEPTED PARTIAL:PARTIAL CLOSED:CLOSED FIXED:FIXED REOPEN:OPEN OPEN:OPEN CONFIRMED:OPEN", kv, " ")
      for (i=1;i<=n;i++){ split(kv[i],p,":"); k=index($0,p[1]); if (k>0 && k<bp){bp=k; best=p[2]} }
      print (best=="" ? "?" : best) }'
}

cmd_next() {
  local m; m=$(max_id); m=${m:-0}
  printf 'BUG-%03d\n' $((m + 1))
}

cmd_audit() {
  local living
  living=$(ls "$REPO"/CLAUDE.md "$REPO"/.claude/rules/*.md "$REPO"/design/gdd/*.md \
              "$REPO"/docs/systems/*/README.md "$REPO"/docs/ui/ui-ux-flow.md \
              "$REPO"/docs/architecture/adr-*.md 2>/dev/null | grep -v 'cross-review')

  echo "## Bug-ID audit"
  echo
  echo "Next free ID: \`$(cmd_next)\` · bug files: $(ls "$BUGS"/BUG-*.md 2>/dev/null | wc -l | tr -d ' ')"
  echo
  echo "### 1. IDs cited in living docs with no bug file"
  echo
  echo "| ID | Cited in |"
  echo "|---|---|"
  local found=0
  for id in $(grep -ohE 'BUG-[0-9]{3}' $living 2>/dev/null | sort -u); do
    if [ ! -f "$BUGS/$id.md" ]; then
      echo "| $id | $(grep -lE "$id\b" $living | sed "s#^$REPO/##" | tr '\n' ' ') |"
      found=1
    fi
  done
  [ $found -eq 0 ] && echo "| — | none |"

  echo
  echo "### 2. Bug files with no \`**Status**\` line"
  echo
  found=0
  for f in "$BUGS"/BUG-*.md; do
    [ -z "$(status_of "$f")" ] && { echo "- $(basename "$f" .md)"; found=1; }
  done
  [ $found -eq 0 ] && echo "- none"

  echo
  echo "### 3. CLAUDE.md Known-Bugs status vs bug file"
  echo
  echo "| ID | CLAUDE.md says | File says | File status |"
  echo "|---|---|---|---|"
  found=0
  grep -E '^\s*\| BUG-[0-9]{3} \|' "$REPO/CLAUDE.md" 2>/dev/null | while IFS= read -r row; do
    id=$(printf '%s' "$row" | grep -oE 'BUG-[0-9]{3}' | head -1)
    [ -f "$BUGS/$id.md" ] || continue
    # third cell = status column
    cell=$(printf '%s' "$row" | awk -F'|' '{print $4}')
    doc=$(classify "$cell")
    fst=$(status_of "$BUGS/$id.md")
    file=$(classify "$fst")
    if [ "$doc" != "$file" ] && [ "$doc" != "?" ] && [ "$file" != "?" ]; then
      echo "| $id | $doc | $file | ${fst//|/\\|} |"
    fi
  done
  echo
  echo "_Rows whose status cannot be classified are skipped. Title mismatches (two defects under one ID)"
  echo "need a human read: compare the CLAUDE.md row with the file's \`**Title**\`._"
}

case "${1:-}" in
  next) cmd_next ;;
  audit) cmd_audit ;;
  *) echo "usage: bug-id.sh next|audit" >&2; exit 2 ;;
esac
