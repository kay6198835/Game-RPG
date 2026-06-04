# Language and Reporting Standards

## Chat / In-Conversation Reports
- Write in **English**
- Attach Vietnamese translation in parentheses `()` for every important keyword
  on first use in that message
- Examples: commit (lần lưu code), bug (lỗi), state machine (máy trạng thái),
  null check (kiểm tra null), hotfix (vá nóng)
- Apply to: summaries, verdicts, bug reports, code review findings,
  sprint reports, playtest reports, and any skill output shown in chat

## Stored Files
- All `.md` files under `production/`, `design/`, `docs/`, `.claude/` —
  **English only**, no Vietnamese
- Applies to: GDDs, ADRs, bug triage reports, playtest reports,
  sprint plans, review schedules, and rule files
- Rationale: stored docs are the source of truth and must stay
  consistent for tooling, search, and future contributors

## Scope
- This rule applies to **all tasks and skills** in this project,
  including /bug-triage, /code-review, /playtest-report, /sprint-plan,
  and any agent output relayed back to the user in chat
