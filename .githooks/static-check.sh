#!/usr/bin/env bash
# static-check.sh — process gate, part C (S21-09). Seconds, no Unity needed.
#
#   static-check.sh <base> <tip>    check the .cs files changed between two commits
#   static-check.sh --all [<tip>]   check every runtime .cs file (Editor-API check only)
#
# Check 1 — serialized rename: a serialized field that disappears from a .cs file while a committed
#   .asset / .prefab / .unity still stores its key, with no [FormerlySerializedAs("old")] on the new
#   field. Unity then loads the default value silently (BUG-105, BUG-108, BUG-121).
# Check 2 — Editor API in runtime code: `using UnityEditor` or `UnityEditor.` outside an
#   `#if UNITY_EDITOR` block, in a file not under an Editor/ folder. The Editor compiles it; a Player
#   build does not (BUG-116).
#
# Exit 0 = pass, 1 = at least one ERROR. Reads git objects only; never touches the working tree.
set -u

# Prints one serialized YAML key per line for the C# source on stdin.
extract_fields() {
  awk '
    function trim(s) { sub(/^[ \t\r]+/, "", s); sub(/[ \t\r]+$/, "", s); return s }
    /^[ \t]*\/\// { next }
    /^[ \t]*#/    { next }
    {
      line = $0
      sub(/\/\/.*$/, "", line)
      t = trim(line)
      if (t == "") next
      if (t ~ /^\[.*\]$/) { attrs = attrs " " t; next }
      all = attrs " " t
      attrs = ""
      if (all ~ /NonSerialized/) next
      decl = t
      while (match(decl, /\[[^]]*\]/)) decl = substr(decl, 1, RSTART - 1) " " substr(decl, RSTART + RLENGTH)
      decl = trim(decl)
      if (all ~ /field:[ \t]*SerializeField/ && decl ~ /\{/) {
        head = decl; sub(/\{.*/, "", head); head = trim(head)
        n = split(head, tk, /[ \t]+/)
        if (n >= 2 && tk[n] ~ /^[A-Za-z_][A-Za-z0-9_]*$/) print "<" tk[n] ">k__BackingField"
        next
      }
      if (decl ~ /=>/) next
      cut = decl
      p = index(cut, "="); q = index(cut, ";")
      if (p == 0 && q == 0) next
      if (p == 0 || (q > 0 && q < p)) cut = substr(cut, 1, q - 1); else cut = substr(cut, 1, p - 1)
      if (cut ~ /[({]/) next
      cut = trim(cut)
      n = split(cut, tk, /[ \t]+/)
      if (n < 2) next
      if (cut ~ /(^|[ \t])(static|const|readonly|event|abstract|override|using|return|delegate|namespace)([ \t]|$)/) next
      name = tk[n]
      if (name !~ /^[A-Za-z_][A-Za-z0-9_]*$/) next
      if (all ~ /Serialize(Field|Reference)/ || tk[1] == "public") print name
    }' | sort -u
}

# Prints every [FormerlySerializedAs("x")] name in the C# source on stdin.
extract_formerly() {
  grep -oE 'FormerlySerializedAs\("[^"]+"\)' | sed -E 's/.*\("([^"]+)"\)/\1/' | sort -u
}

# Prints "line: text" for each Editor-API use outside #if UNITY_EDITOR in the C# source on stdin.
editor_api_hits() {
  awk '
    /^[ \t]*#[ \t]*if/    { d++; ed[d] = ($0 ~ /UNITY_EDITOR/ && $0 !~ /!UNITY_EDITOR/); next }
    /^[ \t]*#[ \t]*elif/  { ed[d] = ($0 ~ /UNITY_EDITOR/ && $0 !~ /!UNITY_EDITOR/); next }
    /^[ \t]*#[ \t]*else/  { ed[d] = !ed[d]; next }
    /^[ \t]*#[ \t]*endif/ { if (d > 0) d--; next }
    /^[ \t]*\/\// { next }
    {
      g = 0; for (i = 1; i <= d; i++) if (ed[i]) g = 1
      code = $0; sub(/\/\/.*$/, "", code)
      if (!g && code ~ /(using[ \t]+UnityEditor|UnityEditor\.)/) { s = $0; gsub(/^[ \t]+|\r$/, "", s); print NR ": " s }
    }'
}

# Project gameplay code only; vendor folders (TextMesh Pro samples, Plugins) are out of scope.
# Script GUIDs of a .cs file and of the files declaring its subclasses (two levels), so an asset of
# PlayerData counts as an asset of CharacterData.
script_guids() {
  local tip="$1" files="$2" classes level
  for level in 1 2; do
    classes=$(printf '%s\n' "$files" | while IFS= read -r f; do git show "$tip:$f" 2>/dev/null; done \
              | grep -oE 'class[[:space:]]+[A-Za-z_][A-Za-z0-9_]*' | awk '{print $2}' | sort -u | paste -sd'|')
    [ -z "$classes" ] && break
    files=$( { printf '%s\n' "$files"; git grep -lE "class[[:space:]]+[A-Za-z_][A-Za-z0-9_]*[[:space:]]*:[^{]*\\b($classes)\\b" "$tip" -- '*.cs' | sed "s/^[^:]*://"; } | sort -u)
  done
  printf '%s\n' "$files" | while IFS= read -r f; do git show "$tip:$f.meta" 2>/dev/null | grep -m1 -oE '[0-9a-f]{32}'; done
}

# Serialized fields declared by the base classes of a .cs file (three levels), so a field moved up
# into a parent class (same key, still loads) is not reported as removed.
ancestor_fields() {
  local tip="$1" src="$2" bases level f
  for level in 1 2 3; do
    bases=$(printf '%s\n' "$src" | grep -oE 'class[[:space:]]+[A-Za-z_][A-Za-z0-9_]*(<[^>]*>)?[[:space:]]*:[[:space:]]*[A-Za-z_][A-Za-z0-9_]*' \
            | sed -E 's/.*:[[:space:]]*//' | sort -u)
    [ -z "$bases" ] && break
    src=$(for b in $bases; do
            for f in $(git grep -lE "class[[:space:]]+$b\\b" "$tip" -- '*.cs' | sed "s/^[^:]*://"); do git show "$tip:$f"; done
          done)
    [ -z "$src" ] && break
    printf '%s\n' "$src" | extract_fields
  done | sort -u
}

is_runtime() { case "$1" in Assets/Script/*) case "$1" in */Editor/*) return 1 ;; *) return 0 ;; esac ;; *) return 1 ;; esac; }

errors=0
warns=0
err()  { echo "  ERROR  $*"; errors=$((errors + 1)); }
warn() { echo "  warn   $*"; warns=$((warns + 1)); }

if [ "${1:-}" = "--all" ]; then
  tip="${2:-HEAD}"
  echo "static-check: Editor API in runtime code, whole tree at $(git rev-parse --short "$tip")"
  while IFS= read -r f; do
    is_runtime "$f" || continue
    hits=$(git show "$tip:$f" 2>/dev/null | editor_api_hits)
    [ -n "$hits" ] && while IFS= read -r h; do err "$f:$h  (Player build fails — wrap in #if UNITY_EDITOR, see BUG-116)"; done <<< "$hits"
  done < <(git ls-tree -r --name-only "$tip" -- Assets | grep '\.cs$')
  echo "static-check: $errors error(s)"
  [ "$errors" -eq 0 ]; exit $?
fi

base="${1:?usage: static-check.sh <base> <tip> | --all [<tip>]}"
tip="${2:?usage: static-check.sh <base> <tip> | --all [<tip>]}"
echo "static-check: $(git rev-parse --short "$base")..$(git rev-parse --short "$tip")"

while IFS=$'\t' read -r status p1 p2; do
  case "$status" in
    D*) continue ;;
    R*) old="$p1"; new="$p2" ;;
    A*) old=""; new="$p1" ;;
    *)  old="$p1"; new="$p1" ;;
  esac
  newsrc=$(git show "$tip:$new" 2>/dev/null) || continue

  if is_runtime "$new"; then
    hits=$(printf '%s\n' "$newsrc" | editor_api_hits)
    [ -n "$hits" ] && while IFS= read -r h; do err "$new:$h  (Player build fails — wrap in #if UNITY_EDITOR, see BUG-116)"; done <<< "$hits"
  fi

  [ -z "$old" ] && continue
  oldsrc=$(git show "$base:$old" 2>/dev/null) || continue
  # Keys the old file could load: its fields plus the old names it still accepted (BUG-108 deleted those).
  removed=$(comm -23 <( { printf '%s\n' "$oldsrc" | extract_fields; printf '%s\n' "$oldsrc" | extract_formerly; } | sort -u) \
                     <(printf '%s\n' "$newsrc" | extract_fields))
  [ -z "$removed" ] && continue
  formerly=$(printf '%s\n' "$newsrc" | extract_formerly)
  inherited=$(ancestor_fields "$tip" "$newsrc")
  while IFS= read -r key; do
    [ -z "$key" ] && continue
    printf '%s\n' "$formerly" | grep -qxF "$key" && continue
    printf '%s\n' "$inherited" | grep -qxF "$key" && continue
    # Deliberate deletion: GATE_ALLOW_REMOVED="duration,perTime" git push
    case ",${GATE_ALLOW_REMOVED:-}," in *",$key,"*) echo "  allow  $new: '$key' removed on purpose (GATE_ALLOW_REMOVED)"; continue ;; esac
    esc=$(printf '%s' "$key" | sed 's/[][\.*^$<>]/\\&/g')
    assets=$(git grep -lE "^[[:space:]-]*${esc}:" "$tip" -- '*.asset' '*.prefab' '*.unity' 2>/dev/null | sed "s/^[^:]*://")
    if [ -n "$assets" ]; then
      # Name match only. Downgrade when no matching asset is bound to this script AND another class
      # still declares the key (e.g. two effect classes both owning `statModifierGroup`).
      guids=$(script_guids "$tip" "$new")
      own=""
      [ -n "$guids" ] && own=$(printf '%s\n' "$assets" | while IFS= read -r a; do git show "$tip:$a" | grep -m1 'm_Script' | grep -qF -f <(printf '%s\n' "$guids") && echo "$a"; done)
      if [ -z "$own" ] && git grep -lw "$key" "$tip" -- '*.cs' | sed "s/^[^:]*://" | grep -vxF "$new" \
           | while IFS= read -r o; do git show "$tip:$o" | extract_fields; done | grep -qxF "$key"; then
        warn "$new: serialized field '$key' removed or renamed; assets store a '$key' key, but another class still declares it — check by hand. e.g. $(printf '%s\n' "$assets" | head -2 | tr '\n' ' ')"
        continue
      fi
      count=$(printf '%s\n' "$assets" | grep -c .)
      err "$new: serialized field '$key' was removed or renamed, but $count asset(s) still store it — add [FormerlySerializedAs(\"$key\")] to the new field (BUG-105/108/121), or if deleted on purpose push with GATE_ALLOW_REMOVED=$key. e.g. $(printf '%s\n' "$assets" | head -3 | tr '\n' ' ')"
    else
      warn "$new: serialized field '$key' removed or renamed; no committed asset stores it"
    fi
  done <<< "$removed"
done < <(git diff --name-status -M "$base" "$tip" -- '*.cs')

echo "static-check: $errors error(s), $warns warning(s)"
[ "$errors" -eq 0 ]
