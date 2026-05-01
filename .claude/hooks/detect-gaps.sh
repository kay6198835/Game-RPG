#!/bin/bash
# Hook: detect-gaps.sh — SessionStart
# Detect missing documentation when Unity code/prototypes exist
# Unity paths: Assets/Script/ instead of src/

set +e

echo "=== Checking for Documentation Gaps ==="

FRESH_PROJECT=true

if [ -f ".claude/docs/technical-preferences.md" ]; then
    ENGINE_LINE=$(grep -E "^\- \*\*Engine\*\*:" .claude/docs/technical-preferences.md 2>/dev/null)
    if [ -n "$ENGINE_LINE" ] && ! echo "$ENGINE_LINE" | grep -q "TO BE CONFIGURED" 2>/dev/null; then
        FRESH_PROJECT=false
    fi
fi

if [ -f "design/gdd/game-concept.md" ]; then
    FRESH_PROJECT=false
fi

# Unity: check Assets/Script/ for .cs files
if [ -d "Assets/Script" ]; then
    SRC_CHECK=$(find "Assets/Script" -type f -name "*.cs" 2>/dev/null | head -1)
    if [ -n "$SRC_CHECK" ]; then
        FRESH_PROJECT=false
    fi
fi

if [ "$FRESH_PROJECT" = true ]; then
    echo ""
    echo "NEW PROJECT: No engine configured, no game concept, no source code."
    echo "   Run: /start"
    echo ""
    echo "To get a project analysis, run: /project-stage-detect"
    echo "==================================="
    exit 0
fi

# Count Unity C# files in Assets/Script/
if [ -d "Assets/Script" ]; then
    SRC_FILES=$(find "Assets/Script" -type f -name "*.cs" 2>/dev/null | wc -l)
else
    SRC_FILES=0
fi
SRC_FILES=$(echo "$SRC_FILES" | tr -d ' ')

if [ -d "design/gdd" ]; then
    DESIGN_FILES=$(find design/gdd -type f -name "*.md" 2>/dev/null | wc -l)
else
    DESIGN_FILES=0
fi
DESIGN_FILES=$(echo "$DESIGN_FILES" | tr -d ' ')

if [ "$SRC_FILES" -gt 30 ] && [ "$DESIGN_FILES" -lt 3 ]; then
    echo "GAP: Codebase ($SRC_FILES .cs files) but sparse design docs ($DESIGN_FILES files)"
    echo "    Suggested: /reverse-document design Assets/Script/[system]"
    echo "    Or run: /project-stage-detect"
fi

# Check for architecture docs
if [ -d "Assets/Script" ]; then
    if [ ! -d "docs/architecture" ]; then
        echo "GAP: Unity scripts exist but no docs/architecture/ directory"
        echo "    Suggested: /architecture-decision or /create-architecture"
    else
        ADR_COUNT=$(find docs/architecture -type f -name "*.md" 2>/dev/null | wc -l)
        ADR_COUNT=$(echo "$ADR_COUNT" | tr -d ' ')
        if [ "$ADR_COUNT" -lt 2 ]; then
            echo "GAP: Only $ADR_COUNT ADR(s) documented"
            echo "    Suggested: /reverse-document architecture Assets/Script/[system]"
        fi
    fi
fi

# Gameplay systems without design docs (Unity: Assets/Script/Character, etc.)
SYSTEMS_TO_CHECK=("Character" "Weapons" "Skill_Ability" "Map")
for system in "${SYSTEMS_TO_CHECK[@]}"; do
    system_dir="Assets/Script/$system"
    if [ -d "$system_dir" ]; then
        file_count=$(find "$system_dir" -type f -name "*.cs" 2>/dev/null | wc -l)
        file_count=$(echo "$file_count" | tr -d ' ')
        system_lower=$(echo "$system" | tr '[:upper:]' '[:lower:]')
        design_doc="design/gdd/${system_lower}-system.md"
        if [ "$file_count" -ge 5 ] && [ ! -f "$design_doc" ]; then
            echo "GAP: Assets/Script/$system/ ($file_count files) has no design doc at $design_doc"
        fi
    fi
done

if [ "$SRC_FILES" -gt 80 ]; then
    if [ ! -d "production/sprints" ] && [ ! -d "production/milestones" ]; then
        echo "GAP: Large codebase ($SRC_FILES files) but no production planning found"
        echo "    Suggested: /sprint-plan"
    fi
fi

echo ""
echo "For a full analysis, run: /project-stage-detect"
echo "==================================="
exit 0
