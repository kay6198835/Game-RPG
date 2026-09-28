using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UIFlow
{
    /// <summary>
    /// Save "thật" tối thiểu: ghi danh sách slot ra một file JSON trong Application.persistentDataPath.
    /// Project chưa có hệ thống save gameplay nào để bọc, nên bản này chỉ lưu thông tin hiển thị của slot.
    /// TODO: nối logic thật — khi gameplay có dữ liệu cần lưu (vị trí phòng, túi đồ, chỉ số), mở rộng
    /// SaveSlotData hoặc gọi hệ thống save của gameplay tại SaveCurrent().
    /// </summary>
    public class RealSaveProvider : ISaveProvider
    {
        [Serializable]
        private class SaveFile
        {
            public List<SaveSlotData> slots = new();
        }

        private const string FileName = "ui_saves.json";
        private SaveFile _data;

        public int SelectedSlot { get; set; } = -1;

        private string FilePath => Path.Combine(Application.persistentDataPath, FileName);

        public bool HasAnySave => Data.slots.Count > 0;

        public IReadOnlyList<SaveSlotData> GetSlots() => Data.slots;

        public SaveSlotData GetMostRecent()
        {
            SaveSlotData newest = null;
            foreach (SaveSlotData slot in Data.slots)
            {
                if (newest == null || string.CompareOrdinal(slot.lastSavedText, newest.lastSavedText) > 0) newest = slot;
            }
            return newest;
        }

        public SaveSlotData CreateNewSave(NewCharacterRequest request)
        {
            int nextIndex = 0;
            foreach (SaveSlotData slot in Data.slots) nextIndex = Math.Max(nextIndex, slot.slotIndex + 1);

            SaveSlotData save = new()
            {
                slotIndex = nextIndex,
                characterName = request.characterName,
                className = request.classId,
                level = 1,
                locationName = "Phòng khởi đầu",
                lastSavedText = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
            };
            Data.slots.Add(save);
            SelectedSlot = save.slotIndex;
            WriteToDisk();
            return save;
        }

        public bool SaveCurrent()
        {
            SaveSlotData current = Data.slots.Find(slot => slot.slotIndex == SelectedSlot);
            if (current == null) return false;

            // TODO: nối logic thật — đọc level / vị trí / thời gian chơi từ gameplay rồi ghi vào current.
            current.lastSavedText = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            return WriteToDisk();
        }

        public void Delete(int slotIndex)
        {
            Data.slots.RemoveAll(slot => slot.slotIndex == slotIndex);
            if (SelectedSlot == slotIndex) SelectedSlot = -1;
            WriteToDisk();
        }

        // Đọc file lười: chỉ đọc lần đầu cần tới.
        private SaveFile Data
        {
            get
            {
                if (_data != null) return _data;
                _data = new SaveFile();
                try
                {
                    if (File.Exists(FilePath)) _data = JsonUtility.FromJson<SaveFile>(File.ReadAllText(FilePath)) ?? new SaveFile();
                }
                catch (Exception e)
                {
                    // File hỏng không được làm sập menu: báo lỗi và coi như chưa có save.
                    Debug.LogWarning("[RealSaveProvider] Không đọc được file save: " + e.Message);
                }
                return _data;
            }
        }

        private bool WriteToDisk()
        {
            try
            {
                File.WriteAllText(FilePath, JsonUtility.ToJson(Data, true));
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning("[RealSaveProvider] Không ghi được file save: " + e.Message);
                return false;
            }
        }
    }
}
