# Brief animation — bản chi tiết (dùng cho mọi loại động tác)

Điền xong đưa Claude, Claude sẽ chuyển thành `animation-spec.yaml`. Mục nào không cần thì bỏ trống;
**mục có dấu ★ là bắt buộc**. Có sẵn ví dụ đã điền: `examples/brief_archer_quick_shot.md`.

Cách viết để ra kết quả giống Mixamo:
- Mô tả **pose đích** (tay ở đâu, khớp gập bao nhiêu độ) thay vì chỉ "ít/vừa/nhiều".
- Hướng ghi **theo hệ quy chiếu** của mục 3 ("về phía mục tiêu", "ra sau theo hướng chạy"). Không ghi
  theo trái/phải màn hình.
- Các pha phải **chồng lên nhau**; chỉ dừng ở **pose giữ có chủ đích**.
- Mỗi động tác ghi **một kiểu ease**.

---

## 1. Thông tin chung ★
- Tên animation (tên file, snake_case):
- Thư mục dự án:
- FPS: 30
- Độ dài: ___ s (= ___ frame)
- Loại clip: one-shot / loop / đoạn trong combo (đòn thứ __ trên __)
- Nối từ clip: ___ — pose đầu phải khớp với pose cuối của clip đó? có / không
- Nối sang clip: ___ — kết thúc ở trạng thái: **đứng yên** / **đang chuyển động** (nối mượt sang clip sau)

## 2. Nhân vật & prop ★
- Model: Mixamo "___" / file `models/___.fbx`
- Chiều cao nhân vật (nếu biết): ___ m
- Tay thuận / tay cầm prop: trái / phải. Prop: ___ (cung, kiếm, khiên…), gắn vào bone: ___
- Prop đặc thù cần để ý: (vd. cung phải đứng thẳng khi ngắm; mũi tên rời đang nằm dưới chân, cần ẩn)

## 3. Hướng & hệ quy chiếu ★
- Hướng chính của động tác: **trước mặt root** (mặc định, = -Y trong Blender, = +Z khi vào Unity) / khác: ___
- Mục tiêu / hướng di chuyển nằm ở: ___ (vd. "mục tiêu ngang tầm vai, thẳng trước root")
- Đứng: quay mặt vào hướng chính / nghiêng ngang (vai chĩa vào hướng chính) / khác: ___
- Root có di chuyển không: tại chỗ (in place) / có — đi ___ m theo hướng ___

## 4. Base animation
- Tên trên Mixamo: ___ · Mô tả phân biệt (nếu trùng tên): ___
- Nguồn: tải Mixamo / file có sẵn `models/___.fbx`
- In Place: có / không · Slider khác mặc định 50: ___
- Chỉ dùng đoạn: frame ___ → ___ (bỏ trống = cả clip), nén/giãn về ___ s
- **Đặc điểm tư thế của base** (rất quan trọng: track sẽ đặt chồng lên tư thế này):
  - Thân: thẳng mặt / quay ngang ___° / cúi ___°
  - Tay trái: ___ (buông, gập khuỷu ___°, cầm ___)
  - Tay phải: ___
  - Chân: đứng song song / tấn trước-sau / ___
- Animation tham chiếu (nếu có, Claude đo timing từ đây): Mixamo "___", đoạn frame ___–___

## 5. Phân pha ★
Mỗi pha một dòng. Cột "Chồng" = pha này bắt đầu sớm bao nhiêu trước khi pha trước kết thúc
(**0 = nối đuôi = dễ khựng**). Pha nào không có thì bỏ.

| Pha | Mục đích | Bắt đầu | Kết thúc | Chồng với pha trước | Ease vào → ra | Có giữ yên? |
|---|---|---|---|---|---|---|
| Chuẩn bị (anticipation) | lấy đà, ngược hướng hành động | | | — | chậm-nhanh | |
| Hành động chính | | | | | nhanh-chậm | |
| Điểm nhấn (impact / release / contact) | khoảnh khắc người xem phải thấy | | | | snap | |
| Follow-through | quán tính sau điểm nhấn | | | | nhanh-chậm | |
| Giữ (hold) | cho mắt đọc kịp pose | | | | — | có, ___ s |
| Hồi / chuyển tiếp | về idle hoặc sang clip sau | | | | chậm-nhanh | |

## 6. Key pose — giá trị đích ★
Mỗi pose ghi những gì **đo được**. Ô nào giống pose trước thì ghi "như trên".

| # | Tên pose | Thời điểm | Hông / trọng tâm | Thân (xoay, cúi) | Đầu / mắt | Tay trái (hướng, khuỷu) | Tay phải (hướng, khuỷu, bàn tay ở đâu) | Chân | Prop |
|---|---|---|---|---|---|---|---|---|---|
| 1 | Pose đầu | 0.00 s | như base | | | | | | |
| 2 | | | | | | | | | |
| 3 | | | | | | | | | |
| 4 | Pose cuối | | | | | | | | |

Cách ghi giá trị đích:
- Hướng tay: "chĩa vào mục tiêu", "ra sau dọc đường tên", "chếch lên 30° so với ngang vai".
- Khớp: "khuỷu 125°" (0° = duỗi thẳng), "gối gập 30°".
- Vị trí: "bàn tay ở má, cách đầu 15 cm", "tay ngang hông", "hai tay cách nhau 0.75 m".
- Thân: "xoay 15° quanh trục đứng", "cúi 10° về phía mục tiêu", "đường vai chĩa vào mục tiêu".
- Hông: "hạ 5 cm", "dồn trọng tâm 70% lên chân trước".

## 7. Timeline động tác chi tiết ★
Một dòng = **một** chuyển động liền mạch của một bộ phận. Đừng chẻ một chuyển động thành nhiều dòng
liên tiếp: mỗi điểm nối là một chỗ dễ khựng.

| # | Bộ phận | Động tác | Hướng (theo mục 3) | Giá trị đích | Bắt đầu | Đỉnh | Giữ đến | Kết thúc | Ease | Dẫn / theo sau | Ghi chú |
|---|---|---|---|---|---|---|---|---|---|---|---|
| 1 | | | | | | | | | | | |
| 2 | | | | | | | | | | | |

- **Bộ phận**: hông, thân dưới, thân trên/ngực, cổ, đầu; vai, cánh tay, cẳng tay, bàn tay, ngón
  (trái/phải); đùi, cẳng chân, bàn chân (trái/phải); prop.
- **Ease**:
  - `nhanh-chậm` (ease-out): xuất phát mạnh, vào pose êm. Dùng cho hầu hết chuyển động tới pose.
  - `chậm-nhanh` (ease-in): tăng tốc, kết thúc gắt. Dùng cho chuẩn bị trước điểm nhấn, rơi.
  - `mềm` (ease-in-out): chậm-nhanh-chậm.
  - `đều` (linear): hiếm dùng.
  - `snap`: 1–3 frame.
- **Dẫn / theo sau**: thứ tự các bộ phận. Ví dụ "hông dẫn, thân theo sau 2 frame, tay theo sau
  3 frame", hoặc "bắt đầu khi #2 mới đi được 30%".

## 8. Trọng tâm, chân, root
- Hông lên/xuống: ___ cm lúc ___ · Dồn trọng tâm: chân ___ lúc ___
- Chân: đứng yên tuyệt đối / được trượt / có bước (chân ___ bước ___ cm lúc ___)
- Giới hạn hiện tại của pipeline: track chỉ **xoay** bone. Dịch hông và giữ bàn chân dính đất (IK) chưa
  làm được. Cứ ghi vào; Claude sẽ báo phần nào phải lấy từ base animation hoặc làm tay.

## 9. Chuyển động phụ (secondary / overlap)
- Đầu: trễ thân ___ frame / giữ nhìn ___ suốt clip
- Tay/prop: rung, lắc sau điểm nhấn ___ (biên độ, số lần, tắt dần sau ___ s)
- Tóc/áo/dây: (pipeline chưa mô phỏng — chỉ ghi nếu rig có bone riêng)

## 10. Giới hạn & va chạm ★
- Khớp tối đa: khuỷu ≤ ___° (mesh Mixamo bắt đầu bẹp từ ~140°), gối ≤ ___°, vặn vai ≤ ___°, cúi ≤ ___°
- Không xuyên: ___ (vd. tay/prop qua đầu, vai, thân; prop xuống dưới sàn)
- Giữ cố định: ___ (vd. hướng cơ thể luôn về mục tiêu, bàn chân không trượt)

## 11. Cảm giác ★
- Tốc độ tổng thể: chậm / vừa / nhanh / rất nhanh. Trọng lượng: nhẹ / vừa / nặng
- Điểm dừng có chủ đích (**chỉ những chỗ này được đứng yên**): ___ (vd. "anchor 0.30–0.33 s", "follow-through 0.37–0.60 s")
- Điểm nhấn chính: ___ s. Phải rõ nhờ: tương phản tốc độ / biên độ / giữ yên sau đó
- Điều tránh: ___ (vd. reset về idle, vung tay quá to, đứng cứng như tượng)

## 12. Đầu ra ★
- Góc preview: 3/4 (mặc định) / chính diện / ngang / ___°
- Góc kiểm tra thêm: từ phía mục tiêu / từ trên / ___ (để soát prop thẳng, tay đúng hướng)
- Độ phân giải: 512 · Số frame trong ảnh preview: 8 · Video mp4: có / không
- Xuất FBX: có · Xuất skeleton để dùng cho nhân vật khác: có / không

## 13. Tự kiểm trước khi gửi
- [ ] Mỗi key pose có ít nhất 1 giá trị đo được (góc, cm, hướng theo mục tiêu)
- [ ] Không có 2 pha nào nối đuôi với "Chồng" = 0, trừ chỗ cố ý
- [ ] Đã liệt kê hết các điểm dừng; ngoài chúng ra không còn chỗ nào đứng yên
- [ ] Điểm nhấn ngắn (≤ 3 frame) và có pha giữ hoặc giảm tốc ngay sau
- [ ] Hông/thân có chuyển động, không chỉ tay
- [ ] Đã mô tả tư thế base (mục 4) nếu base không phải T-pose / idle thẳng
- [ ] Đã ghi trạng thái cuối: đứng yên hay đang chuyển động

---

## Phụ lục — thước đo tham khảo ở 30 fps
Các số có ghi nguồn "Mixamo Shooting Arrow" là số **đo thật** từ animation Mixamo. Các số còn lại là
kinh nghiệm hoạt hình chung, dùng làm điểm khởi đầu rồi chỉnh theo mắt.

| Hạng mục | Giá trị gợi ý | Nguồn |
|---|---|---|
| Điểm nhấn (release / impact) | 2–3 frame chuyển động, biên độ nhỏ (vd. tay lùi 6–7 cm) | Mixamo Shooting Arrow f96–98 |
| Giữ sau điểm nhấn (follow-through) | 0.3–0.6 s (bản đầy đủ), 0.1–0.25 s (combo nhanh) | Mixamo: 0.6 s (f99–117) |
| Giữ ngắm / giữ pose trước điểm nhấn | 0.3 s (bản đầy đủ), 1–3 frame (combo nhanh) | Mixamo: f85–95 |
| Hai pha liên tiếp chồng nhau | 10–30% thời lượng pha trước | Mixamo: kéo dây bắt đầu trước khi nâng cung xong 4 frame |
| Hông lên/xuống khi cúi, lấy đà | 3–8 cm | Mixamo: 7 cm khi lắp tên |
| Thân cúi khi thao tác tay ở thấp | 10–20° | Mixamo: 21° |
| Ngực xoay khi đổi tư thế tay | 15–40° | Mixamo: 40° |
| Khuỷu khi kéo cung / ôm vật sát người | 115–130° | Mixamo: 125° |
| Tay duỗi "thẳng" nhìn tự nhiên | còn gập 5–15° | Mixamo: tay cung 15° |
| Chuẩn bị (anticipation) | 15–25% thời lượng hành động chính, ngược hướng | kinh nghiệm chung |
| Chuyển động phụ (đầu, prop) trễ sau thân | 1–3 frame | kinh nghiệm chung |
| Khớp bắt đầu làm mesh Mixamo bẹp | khuỷu/gối > ~140°, vặn vai > ~70° | đo trong pipeline này |
