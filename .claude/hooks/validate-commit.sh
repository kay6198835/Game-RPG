#!/bin/bash
# Claude Code PreToolUse hook: Validates git commit commands
# Unity adaptation: checks Assets/Script/ instead of src/
# Exit 0 = allow, Exit 2 = block

INPUT=$(cat)

if command -v jq >/dev/null 2>&1; then
    COMMAND=$(echo "$INPUT" | jq -r '.tool_input.command // empty')
else
    COMMAND=$(echo "$INPUT" | grep -oE '"command"[[:space:]]*:[[:space:]]*"[^"]*"' | sed 's/"command"[[:space:]]*:[[:space:]]*"//;s/"$//')
fi

if ! echo "$COMMAND" | grep -qE '^git[[:space:]]+commit'; then
    exit 0
fi

STAGED=$(git diff --cached --name-only 2>/dev/null)
if [ -z "$STAGED" ]; then
    exit 0
fi

WARNINGS=""

# Check design documents for required sections
DESIGN_FILES=$(echo "$STAGED" | grep -E '^design/gdd/')
if [ -n "$DESIGN_FILES" ]; then
    while IFS= read -r file; do
        if [[ "$file" == *.md ]] && [ -f "$file" ]; then
            for section in "Overview" "Player Fantasy" "Detailed" "Formulas" "Edge Cases" "Dependencies" "Tuning Knobs" "Acceptance Criteria"; do
                if ! grep -qi "$section" "$file"; then
                    WARNINGS="$WARNINGS\nDESIGN: $file missing section: $section"
                fi
            done
        fi
    done <<< "$DESIGN_FILES"
fi

# Validate JSON ScriptableObject data files
DATA_FILES=$(echo "$STAGED" | grep -E '^Assets/ScriptableObjects/.*\.json$')
if [ -n "$DATA_FILES" ]; then
    PYTHON_CMD=""
    for cmd in python python3 py; do
        if command -v "$cmd" >/dev/null 2>&1; then
            PYTHON_CMD="$cmd"
            break
        fi
    done
    while IFS= read -r file; do
        if [ -f "$file" ] && [ -n "$PYTHON_CMD" ]; then
            if ! "$PYTHON_CMD" -m json.tool "$file" > /dev/null 2>&1; then
                echo "BLOCKED: $file is not valid JSON" >&2
                exit 2
            fi
        fi
    done <<< "$DATA_FILES"
fi

# Check for hardcoded gameplay values in Unity scripts
# Unity gameplay code lives in Assets/Script/
GAMEPLAY_FILES=$(echo "$STAGED" | grep -E '^Assets/Script/(Character|Weapons|Skill_Ability)/')
if [ -n "$GAMEPLAY_FILES" ]; then
    while IFS= read -r file; do
        if [ -f "$file" ]; then
            if grep -nE '(damage|health|speed|rate|chance|cost|duration)[[:space:]]*=[[:space:]]*[0-9]+[^;]*;' "$file" 2>/dev/null | grep -v '//' | grep -qE '=[[:space:]]*[0-9]+'; then
                WARNINGS="$WARNINGS\nCODE: $file may have hardcoded gameplay values. Use ScriptableObjects."
            fi
        fi
    done <<< "$GAMEPLAY_FILES"
fi

# Check for TODO/FIXME without assignee in any C# file
CS_FILES=$(echo "$STAGED" | grep -E '^Assets/Script/.*\.cs$')
if [ -n "$CS_FILES" ]; then
    while IFS= read -r file; do
        if [ -f "$file" ]; then
            if grep -nE '(TODO|FIXME|HACK)[^(]' "$file" 2>/dev/null | grep -v '//.*TODO(' | grep -qE '(TODO|FIXME|HACK)[^(]'; then
                WARNINGS="$WARNINGS\nSTYLE: $file has TODO/FIXME without owner. Use TODO(name) format."
            fi
        fi
    done <<< "$CS_FILES"
fi

if [ -n "$WARNINGS" ]; then
    echo -e "=== Commit Validation Warnings ===$WARNINGS\n================================" >&2
fi

exit 0
