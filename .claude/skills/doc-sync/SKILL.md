---
name: doc-sync
description: "Đối chiếu lời khẳng định trong tài liệu với code (Phase 0 claim sweep), phân tích trạng thái hiện tại của dự án từ git log và source code, sau đó cập nhật file chủ của từng loại dữ liệu (production/qa/open-bugs.md sinh tự động, production/demo-checklist.md, docs/systems/event-system/README.md), chỉ sửa dòng mốc + dòng tóm tắt trong CLAUDE.md, memory/project_state.md, docs/systems/<system>/ (README + CHANGELOG), các tài liệu sống (GDD, ADR, diagrams, ui-ux-flow, skill-reference, tech-debt-register, VERSION.md) cùng changelog/<doc>.CHANGELOG.md bên cạnh, và docs/CHANGELOG-DOCS.md để đồng bộ với code thực tế. Chạy khi user nói 'cập nhật docs', 'sync document', 'update project docs', 'cập nhật tài liệu dự án'."
argument-hint: "[--dry-run | --auto]"
user-invocable: true
allowed-tools: Read, Glob, Grep, Bash, Write, Edit
context: |
  !git log --oneline -20 2>/dev/null
---

# Doc Sync

Đọc trạng thái thực tế của code, so sánh với tài liệu hiện có, và cập nhật tất cả
tài liệu `.md` để khớp với code. Không suy đoán — chỉ ghi những gì đọc được từ file.

**Argument:** `$ARGUMENTS[0]`
- Nếu là `--dry-run`: chỉ hiển thị những gì sẽ thay đổi, không ghi file.
- Nếu là `--auto`: chạy không người (routine `pm-weekly-doc-truth`, Thứ Bảy 23:00). Bỏ bước hỏi ở
  Phase 4 (coi như chọn [A]), tuân thủ `.claude/docs/pm-routine-protocol.md`: chỉ ghi `*.md`, làm
  trong worktree `D:/Fork/Game-RPG-pm`, mọi sửa đổi tài liệu sống đi qua nhánh
  `pm/doc-sync-<YYYY-MM-DD>` rồi merge vào `sprint-NN`, ghi một dòng vào
  `production/session-state/routine-log.md`. Không bao giờ hỏi.
- Nếu để trống: chạy đầy đủ, hỏi xác nhận trước khi ghi.

> **Bug ID**: skill này **không bao giờ cấp số bug** và không tạo file `production/qa/bugs/BUG-*.md`.
> Lỗi code nghi ngờ phát hiện khi đọc source → ghi chú vào `production/qa/bug-inbox.md`
> (`.claude/rules/bug-inbox.md`). Mọi tài liệu chỉ được nhắc tới ID đã có file.

---

## Phase 0: Claim sweep — đối chiếu tài liệu với code (bắt buộc, chạy trước mọi phase)

Mục đích: không ghi lại một lời khẳng định sai chỉ vì commit message nói vậy (BUG-099: doc-sync
từng ghi BUG-096 là "đã sửa" trong khi fix đã bị xoá cùng ngày).

1. `bash .claude/scripts/doc-claims.sh` — đường dẫn `Assets/…`, trích dẫn `File.cs:NN`, tên
   `ON_*` trong tài liệu sống, so với HEAD. Mỗi dòng `STALE` là một việc phải xử lý ở Phase 5.
2. `bash .claude/scripts/bug-id.sh audit` — ID không có file, file thiếu `**Status**`, hàng bug bị chép
   vào `CLAUDE.md` (phải là 0), `production/qa/open-bugs.md` còn khớp không. **File bug luôn đúng.**
3. Đọc lại source cho mọi bug ghi FIXED/PARTIAL (file bug) mà file code được trích dẫn đã đổi kể từ
   mốc cập nhật cuối (tối đa 15 dòng): fix còn trong code không? Hết dòng nào thì ghi `UNVERIFIABLE`.
4. Ghi kết quả vào `production/qa/doc-truth-<YYYY-MM-DD>.md`: bảng `Claim | Doc:line | Verdict
   (TRUE/STALE/FALSE/UNVERIFIABLE) | Evidence`. Các phase sau **phải** dùng danh sách này: mọi dòng
   STALE/FALSE hoặc được sửa, hoặc được ghi `⚠️ Out of date` kèm lý do không sửa.
5. Câu khẳng định nằm trong phần lịch sử (`Previous entry`, `*Was:*`, `(was: …)`) là lịch sử — để
   nguyên, không tính là lỗi.

---

## Phase 1: Xác định phạm vi thay đổi

Đọc git log từ context (đã tự động chạy). Xác định những commit mới nhất chưa được
phản ánh trong tài liệu bằng cách đọc mốc cập nhật cuối:
- `CLAUDE.md` dòng `> **Last updated:** YYYY-MM-DD (HEAD `<hash>` …)` (chỉ có một dòng này) — **mốc chính**, dùng
  `git log <hash>..HEAD`.
- `memory/project_state.md` dòng `Updated **YYYY-MM-DD**`.
- `docs/systems/*/README.md` dòng `**Last verified:** YYYY-MM-DD, HEAD `<hash>``.
Nếu các mốc lệch nhau, lấy mốc **cũ nhất** — file nào bị bỏ qua ở lần trước phải được bù.

Với mỗi commit chưa được document:
- Chạy `git show --stat [hash]` để xem danh sách file thay đổi.
- Nhóm các file thay đổi theo hệ thống: Map/, Character/, Weapons/, Manager/, v.v.

Liệt kê cho user:
```
Commits chưa được document: [N]
  [hash] [message] — ảnh hưởng: [hệ thống A], [hệ thống B]
  ...
```

---

## Phase 2: Đọc trạng thái thực tế

Với mỗi file source bị ảnh hưởng bởi commit chưa được document, đọc file đó.
Tập trung vào:

### 2a. Cấu trúc thư mục mới
- Glob `Assets/Script/**/` (mức folder) và so sánh với bảng "Code map" trong `CLAUDE.md`; chi tiết file/class so với `docs/systems/<system>/README.md`.
- Glob `Assets/SO/**/*.cs` và `Assets/Editor/**/*.cs` cho các folder mới.
- Liệt kê folder mới chưa có trong Code map, và file/class mới chưa có trong README hệ thống tương ứng.

### 2b. Trạng thái bug hiện tại
Đọc mọi file `production/qa/bugs/BUG-*.md` có `**Status**` chưa FIXED/CLOSED (danh sách: `production/qa/open-bugs.md`).
Với mỗi bug còn mở:
- Đọc file và dòng được chỉ định.
- Xác định: bug còn tồn tại không? Code đã thay đổi chưa?
- Kết quả: `STILL OPEN` / `FIXED` / `CHANGED` (cần mô tả lại).

### 2c. EventID enum
Đọc `Assets/Script/Manager/EventManager.cs`.
Liệt kê tất cả giá trị trong enum `EventID` — so sánh với bảng trong `docs/systems/event-system/README.md`.

### 2d. Tính năng mới
Dựa trên code đọc được, xác định hệ thống mới chưa có trong `docs/systems/` (và Code map của CLAUDE.md):
- Class mới không có trong Repository Layout
- Pattern kiến trúc mới (SO mới, Manager mới, tool Editor mới)
- Vấn đề mới (singleton vi phạm, import sai, stub chưa implement)
- **Rename field có `[SerializeField]` / public trên SO hoặc MonoBehaviour**: nếu không có
  `[FormerlySerializedAs]`, grep `.asset` / `.prefab` tìm key cũ — asset còn key cũ = dữ liệu mất
  im lặng (bài học BUG-105)
- **Xác nhận ngày fix bằng `git log -S"<chuỗi>"`**, không ghi ngày của lần doc-sync làm ngày fix

### 2e. Tài liệu sống bị lệch (drift)
Bộ tài liệu sống (giữ nguyên path, mỗi file có `changelog/<tên>.CHANGELOG.md` cạnh nó):
`design/gdd/*.md` (trừ `gdd-cross-review-*`), `docs/architecture/adr-*.md`,
`docs/architecture/character-architecture-analysis.md`, `character-migration-plan.md`,
`docs/diagrams/*.md`, `docs/ui/ui-ux-flow.md`, `docs/skill-reference.md`,
`docs/tech-debt-register.md`, `docs/engine-reference/unity/VERSION.md`.

**KHÔNG thuộc phạm vi** (snapshot theo ngày/tuần/tháng — không bao giờ viết lại):
`production/sprints/*`, `production/retros/*`, `production/qa/bug-triage-*`,
`module-health-*`, `open-issues-*`, `production/qa/playtests/*`, `production/session-*`,
`*-review-YYYY-MM-DD.md`, `combat-balance-*`, `docs/archive/*`.

Với mỗi file/class/field bị đổi tên, xóa hoặc thay hành vi trong các commit chưa document:
- `grep` tên cũ trong bộ tài liệu sống ở trên.
- Với mỗi hit: xác định câu đó còn đúng không. Câu mang tính lịch sử (trong section có ngày,
  "Was:", "Original entry:") thì giữ nguyên.
- Ghi lại: `file:line — câu cũ — sự thật hiện tại — commit gây ra`.

### 2f. docs/systems/
Với mỗi hệ thống bị ảnh hưởng (map theo bảng trong `docs/systems/README.md`), so sánh
`README.md` của hệ thống đó với code. Hệ thống mới chưa có folder → đề xuất tạo folder mới.

---

## Phase 3: So sánh và tạo danh sách thay đổi

Đọc `CLAUDE.md` hiện tại đầy đủ.
Đọc `memory/project_state.md` hiện tại đầy đủ.

Tạo diff report:

```
=== THAY ĐỔI CẦN CẬP NHẬT ===

CLAUDE.md (chỉ mục, ≤ 200 dòng):
  ~ Dòng mốc Last updated
  ~ Code map: [folder mới/xóa] · Current state: [dòng tóm tắt đổi]

production/qa/open-bugs.md:
  ~ Sinh lại bằng `bug-id.sh open` (không sửa tay)

production/demo-checklist.md:
  - Xóa mục đã xong: [task] (ghi vào CHANGELOG-DOCS)
  + Thêm mục mới: [task]

docs/systems/event-system/README.md:
  + EventID mới / ~ producer-consumer đổi: [tên]

memory/project_state.md:
  ~ Cập nhật: [mô tả thay đổi]

docs/systems/<system>/:
  ~ README: [section cần sửa]
  + CHANGELOG entry: [ngày — tiêu đề — commit]
  + Folder mới: [system]

Tài liệu sống bị lệch:
  ~ [file:line] — [câu cũ] → [sự thật hiện tại] ([commit])

docs/CHANGELOG-DOCS.md:
  + Entry [YYYY-MM-DD] — nguyên nhân + bảng tài liệu đã sửa
```

Nếu không có gì thay đổi, báo:
```
Verdict: UP TO DATE — Không có thay đổi cần cập nhật.
```
và dừng.

---

## Phase 4: Xác nhận trước khi ghi

Nếu `--dry-run`: hiển thị diff report và dừng với:
```
Verdict: DRY RUN COMPLETE — Dùng /doc-sync (không có --dry-run) để áp dụng.
```

Nếu `--auto`: không hỏi — áp dụng [A] (ghi tất cả) theo protocol, rồi sang Phase 5.

Nếu không có `--dry-run` và không có `--auto`, trình bày diff report trước, rồi hỏi user
("May I write these updates?"):

> Tôi sẽ cập nhật các file sau:
> - `CLAUDE.md` — dòng mốc + [N] dòng tóm tắt
> - `production/qa/open-bugs.md` (sinh lại), `production/demo-checklist.md`
> - `memory/project_state.md` — cập nhật toàn bộ
> - `docs/systems/` — [N] hệ thống
> - Tài liệu sống — [N] file (+ changelog tương ứng)
> - `docs/CHANGELOG-DOCS.md` — 1 entry
>
> Tiếp tục?
> [A] Có, ghi tất cả
> [B] Chỉ ghi CLAUDE.md + open-bugs + demo-checklist + memory
> [C] Chỉ ghi docs/systems + tài liệu sống
> [D] Không — tôi sẽ tự xử lý

Nếu user chọn [D]: dừng, không ghi file.

---

## Phase 5: Ghi cập nhật

### 5a. Dữ liệu dự án → file chủ; CLAUDE.md chỉ là chỉ mục

> **CLAUDE.md không chứa dữ liệu** (từ 2026-11-19). File này nạp vào context ở **mọi** session, nên chỉ
> giữ quy ước, cảnh báo, và **một dòng tóm tắt + path** cho mỗi loại dữ liệu. Dữ liệu ghi vào file chủ:

| Dữ liệu | File chủ — ghi vào đây | Cách ghi |
|---|---|---|
| Bug đang mở | `production/qa/open-bugs.md` | `bash .claude/scripts/bug-id.sh open > production/qa/open-bugs.md` — **sinh tự động**, không sửa tay. Trạng thái chỉ đổi trong file bug (bởi `/weekly-wrapup`) |
| Demo checklist | `production/demo-checklist.md` | Edit: xóa mục xong (ghi vào CHANGELOG-DOCS), thêm mục mới |
| EventID | `docs/systems/event-system/README.md` | Edit bảng cho khớp enum |
| Cấu trúc file / class | `docs/systems/<system>/README.md` (5c) | Edit |
| Quy tắc code mới | `.claude/rules/<area>.md` (có `paths:`) | Edit; quy tắc dùng mọi nơi mới lên CLAUDE.md "Coding conventions", 1 dòng |
| Lịch sử thay đổi | `docs/CHANGELOG-DOCS.md` (5e), `docs/systems/<system>/CHANGELOG.md` (5c) | Append |

**CLAUDE.md** — dùng **Edit**, chỉ được sửa:
- Dòng `> **Last updated:** YYYY-MM-DD (HEAD `<hash>`, branch …)` (một dòng, không thêm câu lịch sử).
- Bảng "Code map" khi có folder mới / bị xóa (mức folder, kèm path doc).
- Mục "Current state — one line each": mỗi mục **một dòng** tóm tắt + path (vd: số bug S1 mở).
- Bảng "Where things live" khi có file chủ mới.

**Cấm** trong CLAUDE.md: bảng bug, bảng EventID, checklist, danh sách file/class, khối "Previous entry",
câu `*Was:*` / `(was: …)`, `@import` (import cũng tính vào context).

**Kiểm ngân sách (bắt buộc, báo trong Phase 6):** `wc -l CLAUDE.md` ≤ **200** (chuẩn trong docs chính thức
Claude Code). Hook commit chặn khi vượt; vượt thì chuyển nội dung sang file chủ, không được báo `SYNCED`.

### 5b. Cập nhật memory/project_state.md

Viết lại hoàn toàn file này (tiếng Anh — quy tắc `language-reporting.md`) với:
- Timestamp mới: `Updated **YYYY-MM-DD**` (dùng ngày hôm nay) + HEAD hash.
- Danh sách hệ thống mới hoàn thành (kể từ lần cập nhật trước).
- Bảng bug với trạng thái hiện tại (chỉ bug còn OPEN).
- EventID enum hiện tại.
- Danh sách stub/file chưa implement.
- Thứ tự ưu tiên sửa cho demo.

### 5c. Cập nhật docs/systems/

Với mỗi hệ thống bị ảnh hưởng:
1. **Append** entry vào đầu `docs/systems/<system>/CHANGELOG.md` (mới nhất ở trên), đúng template
   trong `docs/systems/README.md`: `## YYYY-MM-DD — <tiêu đề>` + Commit / Changed / From → To /
   Why / Bugs. "Why" lấy từ commit message hoặc code comment; không có thì ghi
   "not recorded in the commit" — **không bịa lý do**. Lý do suy luận phải ghi "(inferred)".
2. **Sửa** `docs/systems/<system>/README.md` cho khớp code hiện tại (README là bản chính thức,
   không giữ lịch sử trong đó). Cập nhật dòng `**Last verified:** YYYY-MM-DD, HEAD `<hash>``.
3. Hệ thống mới: tạo folder `docs/systems/<system>/` với README + CHANGELOG, thêm 1 dòng vào bảng
   trong `docs/systems/README.md`.

### 5d. Cập nhật tài liệu sống + changelog cạnh nó

Với mỗi drift ở Phase 2e:
1. Sửa nội dung tài liệu bằng **Edit** (giữ CRLF/LF như file gốc). Banner `Re-synced YYYY-MM-DD`
   ở đầu file tóm tắt thay đổi. Câu cũ quan trọng thì giữ dạng ~~gạch~~ hoặc `*Was:*`.
   - **ADR**: không sửa phần Decision — thêm banner Status + section `## Amendment N — YYYY-MM-DD`.
   - **Tài liệu có section theo ngày** (vd `ability-system-diagrams.md`): không sửa section cũ —
     thêm section `Current version — YYYY-MM-DD` mới và trỏ tới nó dưới tiêu đề.
2. **Append** entry vào đầu `changelog/<tên>.CHANGELOG.md` cạnh tài liệu (cùng template).
   Mỗi câu đã sửa là một dòng `From → To`.
3. Tài liệu sống mới (GDD/ADR mới): tạo `changelog/<tên>.CHANGELOG.md` và thêm dòng
   `> 📜 Change log: [changelog/<tên>.CHANGELOG.md](changelog/<tên>.CHANGELOG.md)` dưới tiêu đề H1.
4. Không tìm được bằng chứng để sửa → **không sửa**, ghi entry
   `⚠️ Out of date against code (found by doc-sync, not yet fixed)` liệt kê câu sai + dòng.

### 5d′. Kiểm Mermaid (bắt buộc)

Sau khi ghi xong mọi file `.md`, chạy:
```bash
py .claude/hooks/lint-mermaid.py
```
Exit khác 0 → sửa từng `path:line` được báo theo `.claude/rules/mermaid-diagrams.md` rồi chạy lại,
cho tới khi sạch. Lỗi hay gặp: `;` trong message của `sequenceDiagram`, label flowchart có `()`
mà không đặt trong `"…"`, generic `List<T>` trong `classDiagram` (phải là `List~T~`).
Hook commit chặn commit nếu còn lỗi.

### 5e. docs/CHANGELOG-DOCS.md

Thêm entry mới ở đầu (sau phần giới thiệu): `## YYYY-MM-DD — <tiêu đề> (HEAD `<hash>`)`,
dòng **Cause.** liệt kê commit, bảng `| Document | Change |`, và mục phát hiện đáng chú ý.

---

## Phase 6: Xác nhận kết quả

Sau khi ghi file, báo cáo:

```
=== DOC SYNC HOÀN THÀNH ===

CLAUDE.md:
  ✅ Dòng mốc + [N] dòng tóm tắt · kích thước [N] dòng (≤ 200)

production/qa/open-bugs.md:
  ✅ Sinh lại — [N] bug mở ([N] S1) · `bug-id.sh audit` mục 3 = none, mục 4 = current

production/demo-checklist.md:
  ✅ [N] mục xóa, [N] mục mới

memory/project_state.md:
  ✅ Cập nhật — [N] hệ thống mới, [N] bug open

docs/systems/:
  ✅ [N] README sửa, [N] CHANGELOG entry, [N] folder mới

Tài liệu sống:
  ✅ [N] file sửa, [N] changelog entry
  ⚠️ [N] file còn lệch (ghi Out of date)

docs/CHANGELOG-DOCS.md:
  ✅ 1 entry

Mermaid lint:
  ✅ py .claude/hooks/lint-mermaid.py — exit 0

Verdict: SYNCED
```

**Next step:** commit theo protocol (`--auto`: nhánh `pm/doc-sync-<date>` merge vào `sprint-NN`);
dòng nào còn `⚠️ Out of date` hoặc `UNVERIFIABLE` → `/weekly-wrapup` tuần sau xem lại; lỗi code
nghi ngờ đã nằm trong `production/qa/bug-inbox.md` chờ triage Thứ Bảy.

---

## Quy tắc ghi

- **CLAUDE.md không chứa dữ liệu hay lịch sử**: dữ liệu ở file chủ (bảng 5a), lịch sử ở bug file,
  `docs/CHANGELOG-DOCS.md`, `docs/systems/*/CHANGELOG.md` (những nơi này không xóa lịch sử).
- **Giữ nguyên typo có chủ đích**: `Resgister`, `UnResgister`, `IObjecPoolService` —
  đây là tên thật trong source, không sửa. (`attackDamege` đã đổi thành `attackDamage` trong code
  từ 2026-09-01 — xem BUG-105; asset cũ vẫn lưu key cũ.)
- **CHANGELOG chỉ append**: không sửa/xóa entry cũ trong `docs/systems/*/CHANGELOG.md` và
  `changelog/*.CHANGELOG.md`; entry `⚠️ Out of date` được thay bằng entry "Re-synced" khi đã sửa.
- **Snapshot theo ngày không bao giờ viết lại** (sprint, daily plan, retro, triage, playtest,
  module-health, open-issues, review có ngày).
- **Không suy đoán**: nếu không đọc được file thực tế, không ghi.
- **File paths**: relative path từ repo root (không có leading slash).
- **Ký hiệu**: ✅ = hoàn thành/clean, ⚠️ = cần chú ý/open bug.
