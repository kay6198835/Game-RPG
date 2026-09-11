# [NOTE] Animation/Player vs Animation/Player 1 — Duplicate Folders

> **Re-checked 2026-09-11** (full documentation/code re-synchronisation). Both folders still exist
> and still hold 155 `.anim` files each. **Unresolved** — this note has been open since it was
> written and nothing in the 13 sprints since has acted on it.
>
> Rewritten in English on 2026-09-11 per `.claude/rules/language-reporting.md` (stored `.md` files
> are English-only). The original Vietnamese text is preserved verbatim at the bottom.

The two folders have identical structure and an identical file count — **155 `.anim` files each**.

## What needs confirming

This cannot be resolved by reading the repository: `.anim` files do not record which Animator
Controller references them. It needs the Unity Editor.

In the Project window:

- Which Animator Controller references `Animation/Player/`?
- Which references `Animation/Player 1/`?

Quick check:

1. Click any `.anim` file in `Animation/Player/`
2. Inspector → "Used by"
3. Repeat for `Animation/Player 1/`

## Action after confirming

- **If one is unused** → delete that folder.
- **If both are used by different Animators** → keep both and rename them meaningfully
  (e.g. `Player_Knight_v1` and `Player_Knight_v2`), because "Player 1" carries no information
  about what distinguishes it.

Do not delete either folder based on the file listing alone. Identical file *names* do not prove
identical *content*, and a broken Animator reference surfaces as missing animation at runtime
rather than as a compile error.

---

## Original note (Vietnamese, as written)

> Hai folder này có cấu trúc và số lượng file .anim **hoàn toàn giống nhau** (155 file mỗi folder).
>
> ## Cần xác nhận
>
> Mở Unity Editor, kiểm tra trong Project window:
> - Animator Controller nào đang reference folder `Animation/Player/`?
> - Animator Controller nào đang reference folder `Animation/Player 1/`?
>
> Cách kiểm tra nhanh:
> 1. Click vào một file `.anim` trong `Animation/Player/`
> 2. Nhìn vào Inspector → xem "Used by"
> 3. Làm tương tự với `Animation/Player 1/`
>
> ## Hành động sau khi xác nhận
>
> Nếu một trong hai không được dùng → xóa folder đó.
> Nếu cả hai đều được dùng bởi các Animator khác nhau → giữ cả hai và đặt tên rõ hơn (ví dụ: `Player_Knight_v1` và `Player_Knight_v2`).
