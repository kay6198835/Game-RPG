using System.Collections.Generic;

namespace UIFlow
{
    /// <summary>
    /// Đọc / ghi file save. UI (menu chính, chọn save, menu tạm dừng) chỉ gọi qua interface này.
    /// Bản cài đặt: RealSaveProvider (JSON ở persistentDataPath).
    /// </summary>
    public interface ISaveProvider
    {
        /// <summary>Danh sách slot đã có save (không chứa slot trống).</summary>
        IReadOnlyList<SaveSlotData> GetSlots();

        bool HasAnySave { get; }

        /// <summary>Save mới nhất, dùng cho nút "Tiếp tục". Null nếu chưa có save nào.</summary>
        SaveSlotData GetMostRecent();

        /// <summary>Slot người chơi đang chơi; -1 nếu chưa chọn.</summary>
        int SelectedSlot { get; set; }

        /// <summary>Tạo save mới từ màn tạo nhân vật, trả về slot vừa tạo.</summary>
        SaveSlotData CreateNewSave(NewCharacterRequest request);

        /// <summary>Lưu slot đang chơi (menu tạm dừng gọi). Trả về false nếu lưu thất bại.</summary>
        bool SaveCurrent();

        void Delete(int slotIndex);
    }
}
