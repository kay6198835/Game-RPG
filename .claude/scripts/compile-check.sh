#!/usr/bin/env bash
# compile-check.sh — headless compile of the owner's project with Unity batchmode. Evidence tier: COMPILED.
#
# Runs only when no Unity Editor process is running (the Editor holds the project lock, and the owner's
# open session must never be disturbed). Uses the owner's project folder because it already has a
# 6.4 GB Library/; a fresh folder would need a full re-import.
#
# It never commits, stages, reverts or edits anything. Unity may rewrite some files while it opens the
# project; the script lists any path whose git status changed during the run, for the owner to judge.
set -u
PROJECT="${PROJECT_PATH:-D:/Fork/Game-RPG}"
UNITY="${UNITY_EXE:-C:/Program Files/Unity/Hub/Editor/2022.3.62f3/Editor/Unity.exe}"
TIMEOUT="${COMPILE_TIMEOUT:-1800}"
OUT="${TEMP:-/tmp}/unity-compile-$(date +%Y%m%d-%H%M).log"

echo "## Headless compile check"
echo
if [ ! -f "$UNITY" ]; then echo "**Result: NOT RUN** — Unity not found at \`$UNITY\`."; exit 0; fi
if tasklist //FI "IMAGENAME eq Unity.exe" 2>/dev/null | grep -qi 'Unity.exe'; then
  echo "**Result: SKIPPED** — a Unity Editor is running (owner session). Tier stays LOG/STATIC."
  exit 0
fi

head=$(git -C "$PROJECT" rev-parse --short HEAD)
branch=$(git -C "$PROJECT" rev-parse --abbrev-ref HEAD)
before=$(git -C "$PROJECT" status --porcelain)
dirty=$([ -n "$before" ] && echo "yes ($(printf '%s\n' "$before" | grep -c .) paths)" || echo no)

start=$(date +%s)
timeout "$TIMEOUT" "$UNITY" -batchmode -nographics -quit -projectPath "$PROJECT" -logFile "$OUT" >/dev/null 2>&1
code=$?
secs=$(( $(date +%s) - start ))

errors=$(grep -oE '[^ ]*\.cs\([0-9]+,[0-9]+\): error CS[0-9]{4}: .*' "$OUT" 2>/dev/null | sort -u)
nerr=$(printf '%s' "$errors" | grep -c .)
after=$(git -C "$PROJECT" status --porcelain)
changed=$(diff <(printf '%s\n' "$before") <(printf '%s\n' "$after") | grep -E '^[<>]' | sed -E 's/^[<>] //' | sort -u)

if [ "$code" -eq 124 ]; then result="TIMEOUT after ${TIMEOUT}s"
elif [ "$nerr" -gt 0 ]; then result="**FAILED** — $nerr compile error(s)"
elif [ "$code" -ne 0 ]; then result="EXIT $code with no \`error CS\` line (read the log)"
else result="COMPILED"; fi

echo "| Field | Value |"
echo "|---|---|"
echo "| Result | $result |"
echo "| Project HEAD | \`$head\` on \`$branch\` |"
echo "| Uncommitted changes compiled too | $dirty |"
echo "| Duration | ${secs}s |"
echo "| Unity log | \`$OUT\` |"
echo
if [ "$nerr" -gt 0 ]; then
  echo "Errors (distinct, max 15):"
  echo
  printf '%s\n' "$errors" | head -15 | sed 's/^/- `/; s/$/`/'
  echo
fi
if [ -n "$changed" ]; then
  echo "⚠️ Files whose git status changed while Unity ran (not reverted — owner to judge):"
  echo
  printf '%s\n' "$changed" | head -20 | sed 's/^/- `/; s/$/`/'
  echo
fi
echo "_A compile failure is a bug-inbox note shown first in the standup digest; only the Saturday"
echo "wrap-up allocates a bug ID (.claude/rules/bug-inbox.md)._"
