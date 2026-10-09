#!/usr/bin/env bash
# editor-log.sh — read the Unity Editor logs for runtime evidence. Read-only.
#
# Unity rewrites Editor.log every Editor launch and keeps the previous one as Editor-prev.log
# (%LOCALAPPDATA%\Unity\Editor on Windows). This script reports, per log: last write time, whether
# the log belongs to this project, compile errors, Play Mode entries, and distinct exceptions whose
# stack trace has a frame in Assets/Script/. Evidence tier: LOG.
set -u
DIR="${UNITY_LOG_DIR:-${LOCALAPPDATA:-$HOME/AppData/Local}/Unity/Editor}"
PROJECT="${PROJECT_PATH:-D:/Fork/Game-RPG}"

echo "## Unity Editor log scan"
echo
for name in Editor.log Editor-prev.log; do
  f="$DIR/$name"
  echo "### $name"
  echo
  if [ ! -f "$f" ]; then echo "Not found (\`$f\`) — tier stays STATIC."; echo; continue; fi
  mtime=$(date -r "$f" '+%Y-%m-%d %H:%M' 2>/dev/null || stat -c %y "$f" | cut -c1-16)
  proj=$(head -200 "$f" | tr '\\' '/' | grep -ciF "$PROJECT")
  cs=$(grep -cE 'error CS[0-9]{4}' "$f")
  play=$(grep -cE 'Reloading assemblies for play mode|EnterPlayMode' "$f")
  echo "| Field | Value |"
  echo "|---|---|"
  echo "| Last written | $mtime |"
  echo "| This project | $([ "$proj" -gt 0 ] && echo yes || echo 'unknown / other project') |"
  echo "| Compile errors (\`error CS\`) | $cs |"
  echo "| Play Mode entries | $play |"
  echo
  if [ "$cs" -gt 0 ]; then
    echo "Compile errors (distinct, max 10):"
    echo
    grep -oE '[^ ]*\.cs\([0-9]+,[0-9]+\): error CS[0-9]{4}: .*' "$f" | sort -u | head -10 | sed 's/^/- `/; s/$/`/'
    echo
  fi
  # exceptions: an "...Exception" line followed (within 15 lines) by an Assets/Script frame
  ex=$(awk '
    /Exception/ && !/^UnityEngine\.|^  at |DebugLogHandler|Logger:LogException/ { msg=$0; left=15; next }
    left>0 { left--; if (match($0, /Assets\/Script\/[^:)]+\.cs:[0-9]+/)) { print substr(msg,1,140) " @ " substr($0, RSTART, RLENGTH); left=0 } }
  ' "$f" | sort | uniq -c | sort -rn | head -10)
  if [ -n "$ex" ]; then
    echo "Exceptions with a project frame (count, message @ first project frame):"
    echo
    printf '%s\n' "$ex" | sed -E 's/^ *([0-9]+) (.*)$/- \1× `\2`/'
  else
    echo "No exception with an \`Assets/Script/\` frame."
  fi
  echo
done
echo "_Every finding above is a bug-inbox note, never a bug ID (.claude/rules/bug-inbox.md)._"
