#!/bin/bash
# Claude Code PostToolUse hook: Validates Unity asset files after Write/Edit
# Checks naming conventions for Unity assets — Sprites, Prefabs, ScriptableObjects
# Exit 0 = advisory, Exit 1 = blocking error

INPUT=$(cat)

if command -v jq >/dev/null 2>&1; then
    FILE_PATH=$(echo "$INPUT" | jq -r '.tool_input.file_path // empty')
else
    FILE_PATH=$(echo "$INPUT" | grep -oE '"file_path"[[:space:]]*:[[:space:]]*"[^"]*"' | sed 's/"file_path"[[:space:]]*:[[:space:]]*"//;s/"$//')
fi

FILE_PATH=$(echo "$FILE_PATH" | sed 's|\\|/|g')

# Only check files in Assets/ (not Assets/Script/ — that's code, not assets)
if ! echo "$FILE_PATH" | grep -qE '(^|/)Assets/'; then
    exit 0
fi
if echo "$FILE_PATH" | grep -qE '(^|/)Assets/Script/'; then
    exit 0
fi

FILENAME=$(basename "$FILE_PATH")
WARNINGS=""
ERRORS=""

# Unity Prefabs and ScriptableObjects should use PascalCase (Unity convention)
# Only warn for files that look like they have spacing issues (spaces in filename)
if echo "$FILENAME" | grep -qE ' '; then
    WARNINGS="$WARNINGS\n  NAMING: '$FILENAME' has spaces — use PascalCase or underscores for Unity assets"
fi

# Validate JSON ScriptableObject data files
if echo "$FILE_PATH" | grep -qE '(^|/)Assets/ScriptableObjects/.*\.json$'; then
    if [ -f "$FILE_PATH" ]; then
        PYTHON_CMD=""
        for cmd in python python3 py; do
            if command -v "$cmd" >/dev/null 2>&1; then
                PYTHON_CMD="$cmd"
                break
            fi
        done
        if [ -n "$PYTHON_CMD" ]; then
            if ! "$PYTHON_CMD" -m json.tool "$FILE_PATH" > /dev/null 2>&1; then
                ERRORS="$ERRORS\n  FORMAT: $FILE_PATH is not valid JSON"
            fi
        fi
    fi
fi

if [ -n "$WARNINGS" ]; then
    echo -e "=== Asset Validation: Warnings ===$WARNINGS\n(Advisory — fix before final commit.)" >&2
fi

if [ -n "$ERRORS" ]; then
    echo -e "=== Asset Validation: ERRORS ===$ERRORS\nFix before proceeding." >&2
    exit 1
fi

exit 0
