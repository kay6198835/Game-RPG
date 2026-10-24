#!/usr/bin/env bash
# fix-survival.sh [days=30] — is each recent bug fix still in the code? Read-only.
#
# For every bug file, collect the commit hashes named on its **Title**, **Status** and **Fixed in**
# lines. For each hash that is a real commit inside the window and touches .cs files:
#   - ANCESTOR: is it in HEAD's history?
#   - SURVIVAL: share of its added, non-trivial .cs lines that still exist in the same file at HEAD.
#   - LATER: commits after it that touched the same .cs files.
# Verdict: PRESENT (>=80%) · PARTIAL (30-79%) · GONE (<30%) · NOT ANCESTOR.
# GONE / NOT ANCESTOR is never acted on automatically: it becomes a bug-inbox note for the
# Saturday wrap-up (.claude/rules/bug-inbox.md).
set -u
DAYS="${1:-30}"
REPO="${REPO:-.}"
cd "$REPO" || exit 2
BUGS="production/qa/bugs"
SINCE=$(date -d "-$DAYS days" +%s 2>/dev/null || echo 0)
TMPD=$(mktemp -d); trap 'rm -rf "$TMPD"' EXIT

echo "## Fix survival — last $DAYS days (HEAD \`$(git rev-parse --short HEAD)\`)"
echo
echo "| Bug | Bug status | Fix commit | Date | Ancestor | Survival | Verdict | Later commits on same files |"
echo "|---|---|---|---|---|---|---|---|"

rows=0
for f in "$BUGS"/BUG-*.md; do
  id=$(basename "$f" .md)
  status=$(grep -m1 -E '^\*\*Status\*\*' "$f" | sed -E 's/^\*\*Status\*\*:? *//' | cut -c1-40 | tr '|' '/')
  lines=$(grep -E '^\*\*(Title|Status|Fixed in)\*\*' "$f")
  heads=$(printf '%s' "$lines" | grep -oE 'HEAD `[0-9a-f]{7,40}`' | grep -oE '[0-9a-f]{7,40}')
  hashes=$(printf '%s' "$lines" | grep -oE '`[0-9a-f]{7,40}`' | tr -d '`' | sort -u | grep -vxF -e "${heads:-__none__}")
  for h in $hashes; do
    git cat-file -e "$h^{commit}" 2>/dev/null || continue
    ts=$(git log -1 --format=%ct "$h")
    [ "$ts" -lt "$SINCE" ] && continue
    files=$(git show --format= --name-only "$h" -- '*.cs')
    [ -z "$files" ] && continue
    date=$(git log -1 --format=%ad --date=short "$h")
    if git merge-base --is-ancestor "$h" HEAD 2>/dev/null; then anc=yes; else anc=no; fi

    total=0; kept=0
    for p in $files; do
      added=$(git show --format= -U0 "$h" -- "$p" | grep -E '^\+[^+]' | sed -E 's/^\+[[:space:]]*//; s/[[:space:]]+$//' \
              | grep -vE '^(\{|\}|//.*|)$' | awk 'length($0) >= 8')
      [ -z "$added" ] && continue
      key=$(printf %s "$p" | tr "/ " "__")
      cache="$TMPD/$key"
      [ -f "$cache" ] || git show "HEAD:$p" 2>/dev/null | sed -E 's/^[[:space:]]*//; s/[[:space:]]+$//' > "$cache"
      while IFS= read -r line; do
        total=$((total + 1))
        grep -qxF -- "$line" "$cache" && kept=$((kept + 1))
      done <<< "$added"
    done
    [ "$total" -eq 0 ] && continue
    pct=$((kept * 100 / total))
    if [ "$anc" = no ]; then verdict="NOT ANCESTOR"
    elif [ "$pct" -ge 80 ]; then verdict=PRESENT
    elif [ "$pct" -ge 30 ]; then verdict=PARTIAL
    else verdict="**GONE**"; fi
    later=$(git log --format=%h "$h..HEAD" -- $files 2>/dev/null | head -5 | tr '\n' ' ')
    echo "| $id | $status | \`$(git rev-parse --short "$h")\` | $date | $anc | $kept/$total ($pct%) | $verdict | ${later:-—} |"
    rows=$((rows + 1))
  done
done
[ "$rows" -eq 0 ] && echo "| — | — | — | — | — | — | no fix commits in window | — |"
echo
echo "_A row per commit named on a bug's Title/Status/Fixed-in line. A revert commit cited in a title"
echo "(e.g. \"fix X reverted by Y\") is listed too; read the verdict of the **fix** row._"
