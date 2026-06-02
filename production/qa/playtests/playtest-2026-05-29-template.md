# Playtest Report

## Session Info
- **Date**: 2026-05-29
- **Build**: c333a95 (done load single map)
- **Duration**: [Time played]
- **Tester**: [Name/ID]
- **Platform**: PC
- **Input Method**: [KB+M / Gamepad]
- **Session Type**: [First time / Returning / Targeted test]

## Test Focus
[What specific features or flows were being tested — e.g., room navigation, melee combat, door transitions]

## First Impressions (First 5 minutes)
- **Understood the goal?** [Yes/No/Partially]
- **Understood the controls?** [Yes/No/Partially]
- **Emotional response**: [Engaged/Confused/Bored/Frustrated/Excited]
- **Notes**: [Observations]

## Gameplay Flow

### What worked well
- [Observation 1]

### Pain points
- [Issue 1 — Severity: High/Medium/Low]

### Confusion points
- [Where the player was confused and why]

### Moments of delight
- [What surprised or pleased the player]

## Bugs Encountered
| # | Description | Severity | Reproducible |
|---|-------------|----------|-------------|
| 1 | WeaponMelee.Attack() hits no enemies — foreach body empty | High | Always |
| 2 | Enemy NullRef if target lost mid-chase (EntityMoveState line 30) | High | Intermittent |
| 3 | Player does not die when health reaches 0 | High | Always |
| 4 | EndAnimation event never fires (AnimationPlayerController double-registers StartAnimation) | Medium | Always |

## Feature-Specific Feedback

### Room Navigation
- **Understood purpose?** [Yes/No]
- **Found engaging?** [Yes/No]
- **Suggestions**: [Tester suggestions]

### Melee Combat
- **Understood purpose?** [Yes/No]
- **Found engaging?** [Yes/No]
- **Suggestions**: [Tester suggestions]

## Quantitative Data (if available)
- **Deaths**: [Count and locations]
- **Time per area**: [Breakdown]
- **Items used**: [What and when]
- **Features discovered vs missed**: [List]

## Overall Assessment
- **Would play again?** [Yes/No/Maybe]
- **Difficulty**: [Too Easy / Just Right / Too Hard]
- **Pacing**: [Too Slow / Good / Too Fast]
- **Session length preference**: [Shorter / Good / Longer]

## Top 3 Priorities from this session
1. [Most important finding]
2. [Second priority]
3. [Third priority]
