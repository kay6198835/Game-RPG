---
name: doc-sync
description: "Phân tích trạng thái hiện tại của dự án từ git log và source code, sau đó cập nhật CLAUDE.md (Repository Layout, Known Bugs, Demo Checklist, Event System) và memory/project_state.md để đồng bộ với code thực tế. Chạy khi user nói 'cập nhật docs', 'sync document', 'update project docs', 'cập nhật tài liệu dự án'."
argument-hint: "[--dry-run]"
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
- Nếu để trống: chạy đầy đủ, hỏi xác nhận trước khi ghi.

---

## Phase 1: Xác định phạm vi thay đổi

Đọc git log từ context (đã tự động chạy). Xác định những commit mới nhất chưa được
phản ánh trong tài liệu bằng cách đọc timestamp cuối cập nhật trong
`memory/project_state.md` (dòng `cập nhật YYYY-MM-DD`).

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
- Glob `Assets/Script/**/*.cs` và so sánh với Repository Layout trong `CLAUDE.md`.
- Glob `Assets/SO/**/*.cs` và `Assets/Editor/**/*.cs` cho các folder mới.
- Liệt kê mọi file/folder tồn tại trong code nhưng chưa có trong CLAUDE.md.

### 2b. Trạng thái bug hiện tại
Đọc từng file được liệt kê trong bảng Known Bugs của `CLAUDE.md`.
Với mỗi bug ⚠️ OPEN:
- Đọc file và dòng được chỉ định.
- Xác định: bug còn tồn tại không? Code đã thay đổi chưa?
- Kết quả: `STILL OPEN` / `FIXED` / `CHANGED` (cần mô tả lại).

### 2c. EventID enum
Đọc `Assets/Script/Manager/EventManager.cs`.
Liệt kê tất cả giá trị trong enum `EventID` — so sánh với danh sách trong CLAUDE.md.

### 2d. Tính năng mới
Dựa trên code đọc được, xác định hệ thống mới chưa có trong CLAUDE.md:
- Class mới không có trong Repository Layout
- Pattern kiến trúc mới (SO mới, Manager mới, tool Editor mới)
- Vấn đề mới (singleton vi phạm, import sai, stub chưa implement)

---

## Phase 3: So sánh và tạo danh sách thay đổi

Đọc `CLAUDE.md` hiện tại đầy đủ.
Đọc `memory/project_state.md` hiện tại đầy đủ.

Tạo diff report:

```
=== THAY ĐỔI CẦN CẬP NHẬT ===

CLAUDE.md — Repository Layout:
  + Thêm: [file/folder mới và mô tả]
  ~ Sửa: [entry cần cập nhật ghi chú ⚠️/✅]

CLAUDE.md — Known Bugs:
  ✅ Đánh dấu FIXED: Bug #[N] — [mô tả]
  + Thêm bug mới: #[N] [severity] [mô tả] [file:line]

CLAUDE.md — Demo Checklist:
  ✅ Đánh dấu done: [task]
  + Thêm task mới: [task]

CLAUDE.md — Event System:
  + EventID mới: [tên]
  + EventID còn thiếu: [tên]

memory/project_state.md:
  ~ Cập nhật: [mô tả thay đổi]
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

Nếu không có `--dry-run`, hỏi user:

> Tôi sẽ cập nhật các file sau:
> - `CLAUDE.md` — [N] thay đổi
> - `memory/project_state.md` — cập nhật toàn bộ
>
> Tiếp tục?
> [A] Có, ghi tất cả
> [B] Chỉ ghi CLAUDE.md
> [C] Chỉ ghi memory
> [D] Không — tôi sẽ tự xử lý

Nếu user chọn [D]: dừng, không ghi file.

---

## Phase 5: Ghi cập nhật

### 5a. Cập nhật CLAUDE.md

Dùng **Edit** (không phải Write) — chỉ thay đổi đúng phần cần cập nhật:

**Repository Layout:**
- Thêm entry cho file/folder mới với mô tả ngắn và ký hiệu ✅/⚠️.
- Không xóa entry cũ — chỉ thêm hoặc cập nhật ghi chú.

**Known Bugs:**
- Đổi `⚠️ OPEN` → `✅ FIXED` cho bug đã được fix, kèm ngày fix.
- Thêm hàng mới cho bug mới phát hiện với format:
  `| [N] | [COMPILE/LOGIC/BUILD/ARCH] | ⚠️ OPEN | [mô tả] | [File.cs:line] |`
- Giữ nguyên các bug đã FIXED ở bảng để có lịch sử.

**Demo Checklist:**
- Đánh dấu ~~strikethrough~~ ✅ Done cho task hoàn thành.
- Thêm task mới ở cuối danh sách cho tính năng mới cần làm.

**Event System:**
- Cập nhật danh sách EventID đã có và còn thiếu.

### 5b. Cập nhật memory/project_state.md

Viết lại hoàn toàn file này với:
- Timestamp mới: `cập nhật YYYY-MM-DD` (dùng ngày hôm nay).
- Danh sách hệ thống mới hoàn thành (kể từ lần cập nhật trước).
- Bảng bug với trạng thái hiện tại (chỉ bug còn OPEN).
- EventID enum hiện tại.
- Danh sách stub/file chưa implement.
- Thứ tự ưu tiên sửa cho demo.

---

## Phase 6: Xác nhận kết quả

Sau khi ghi file, báo cáo:

```
=== DOC SYNC HOÀN THÀNH ===

CLAUDE.md:
  ✅ Repository Layout: [N] entries thêm/sửa
  ✅ Known Bugs: [N] fixed, [N] mới
  ✅ Demo Checklist: [N] task done, [N] task mới
  ✅ Event System: [N] EventID cập nhật

memory/project_state.md:
  ✅ Cập nhật — [N] hệ thống mới, [N] bug open

Verdict: SYNCED
```

---

## Quy tắc ghi

- **Không xóa lịch sử**: bug đã FIXED vẫn giữ trong bảng, task done vẫn giữ
  với strikethrough.
- **Giữ nguyên typo có chủ đích**: `attackDamege`, `Resgister`, `UnResgister` —
  đây là tên thật trong source, không sửa.
- **Không suy đoán**: nếu không đọc được file thực tế, không ghi.
- **File paths trong CLAUDE.md**: dùng relative path từ `Assets/` (không có leading slash).
- **Ký hiệu**: ✅ = hoàn thành/clean, ⚠️ = cần chú ý/open bug.
