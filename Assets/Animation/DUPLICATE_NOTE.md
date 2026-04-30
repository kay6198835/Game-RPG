# [NOTE] Animation/Player vs Animation/Player 1 — Duplicate Folders

Hai folder này có cấu trúc và số lượng file .anim **hoàn toàn giống nhau** (155 file mỗi folder).

## Cần xác nhận

Mở Unity Editor, kiểm tra trong Project window:
- Animator Controller nào đang reference folder `Animation/Player/`?
- Animator Controller nào đang reference folder `Animation/Player 1/`?

Cách kiểm tra nhanh:
1. Click vào một file `.anim` trong `Animation/Player/`
2. Nhìn vào Inspector → xem "Used by"
3. Làm tương tự với `Animation/Player 1/`

## Hành động sau khi xác nhận

Nếu một trong hai không được dùng → xóa folder đó.
Nếu cả hai đều được dùng bởi các Animator khác nhau → giữ cả hai và đặt tên rõ hơn (ví dụ: `Player_Knight_v1` và `Player_Knight_v2`).
