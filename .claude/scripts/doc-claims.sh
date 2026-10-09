#!/usr/bin/env bash
# doc-claims.sh — mechanical check of checkable claims in the living docs against HEAD. Read-only.
#
# Checks, per living doc:
#   PATH    every backticked `Assets/...` path (no wildcard/placeholder) exists in the tree
#   CITE    every `File.cs:NN` citation names an existing file with at least NN lines
#   EVENT   every ON_UPPER_SNAKE token is a member of `enum EventID` (EventManager.cs)
# Output: STALE rows only (doc:line, kind, claim, why). TRUE claims are counted, not listed.
# Historical text inside a living doc ("Previous entry", "was:") also matches; a STALE row there is
# expected — the doc-sync pass decides whether to rewrite or leave history alone.
set -u
REPO="${REPO:-.}"
cd "$REPO" || exit 2

LIVING=$(ls CLAUDE.md .claude/rules/*.md design/gdd/*.md docs/systems/*/README.md \
            docs/ui/ui-ux-flow.md docs/architecture/adr-*.md 2>/dev/null | grep -v 'cross-review')
TREE=$(mktemp); trap 'rm -f "$TREE"' EXIT
git ls-files > "$TREE"
EVENTS=$(awk '/enum EventID/{f=1;next} f&&/}/{exit} f' Assets/Script/Manager/EventManager.cs \
         | grep -oE '[A-Z][A-Z0-9_]+' | sort -u)

ok=0; stale=0
echo "## Doc claims vs HEAD \`$(git rev-parse --short HEAD)\`"
echo
echo "| Doc:line | Kind | Claim | Why stale |"
echo "|---|---|---|---|"

for doc in $LIVING; do
  # PATH
  while IFS=: read -r ln path; do
    case "$path" in *'*'*|*'{'*|*'<'*|*'['*|*'NN'*|*'…'*|*'...'*) continue ;; esac
    path=${path%/}
    if grep -qxF -- "$path" "$TREE" || grep -q -- "^$path/" "$TREE"; then ok=$((ok+1))
    else echo "| $doc:$ln | PATH | \`$path\` | not in tree |"; stale=$((stale+1)); fi
  done < <(grep -noE '`Assets/[^` ]+`' "$doc" | sed -E 's/`//g' | sed -E 's/:([0-9]+)?(-[0-9]+)?$//' | sed -E 's/(\.cs|\.unity|\.prefab|\.asset|\.json):[0-9].*$/\1/')

  # CITE
  while IFS=: read -r ln file num; do
    num=${num%%[-,]*}
    case "$file" in */*) cand=$(grep -E "(^|/)$file$" "$TREE" | head -2) ;;
                     *)  cand=$(grep -E "/$file$" "$TREE" | head -2) ;; esac
    n=$(printf '%s\n' "$cand" | grep -c .)
    if [ "$n" -eq 0 ]; then echo "| $doc:$ln | CITE | \`$file:$num\` | file not found |"; stale=$((stale+1)); continue; fi
    [ "$n" -gt 1 ] && { ok=$((ok+1)); continue; }   # ambiguous basename: not checkable
    lines=$(wc -l < "$cand")
    if [ "$num" -le $((lines + 1)) ]; then ok=$((ok+1))
    else echo "| $doc:$ln | CITE | \`$file:$num\` | file has $lines lines |"; stale=$((stale+1)); fi
  done < <(grep -noE '[A-Za-z0-9_./-]+\.cs:[0-9]+' "$doc" | awk -F: '{print $1":"$2":"$3}')

  # EVENT
  while IFS=: read -r ln ev; do
    if printf '%s\n' "$EVENTS" | grep -qxF "$ev"; then ok=$((ok+1))
    else echo "| $doc:$ln | EVENT | \`$ev\` | not in \`enum EventID\` |"; stale=$((stale+1)); fi
  done < <(grep -noE '\bON_[A-Z][A-Z0-9_]+' "$doc")
done

echo
echo "**Checked:** $((ok + stale)) claims · **TRUE:** $ok · **STALE:** $stale"
