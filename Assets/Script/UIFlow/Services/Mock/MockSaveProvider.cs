using System;
using System.Collections.Generic;

namespace UIFlow
{
    /// <summary>
    /// Save giả: 3 file mẫu nằm trong RAM, mất khi thoát Play Mode.
    /// TODO: nối logic thật — thay bằng RealSaveProvider khi project có hệ thống save.
    /// </summary>
    public class MockSaveProvider : ISaveProvider
    {
        private readonly List<SaveSlotData> _slots = MockCatalog.CreateSaves();

        public int SelectedSlot { get; set; } = -1;

        public bool HasAnySave => _slots.Count > 0;

        public IReadOnlyList<SaveSlotData> GetSlots() => _slots;

        public SaveSlotData GetMostRecent()
        {
            SaveSlotData newest = null;
            foreach (SaveSlotData slot in _slots)
            {
                // Chuỗi "yyyy-MM-dd HH:mm" so sánh theo thứ tự chữ cũng đúng thứ tự thời gian.
                if (newest == null || string.CompareOrdinal(slot.lastSavedText, newest.lastSavedText) > 0) newest = slot;
            }
            return newest;
        }

        public SaveSlotData CreateNewSave(NewCharacterRequest request)
        {
            int nextIndex = 0;
            foreach (SaveSlotData slot in _slots) nextIndex = Math.Max(nextIndex, slot.slotIndex + 1);

            SaveSlotData save = new()
            {
                slotIndex = nextIndex,
                characterName = request.characterName,
                className = request.classId,
                level = 1,
                playTimeSeconds = 0,
                locationName = "Phòng khởi đầu",
                lastSavedText = Now(),
            };
            _slots.Add(save);
            SelectedSlot = save.slotIndex;
            return save;
        }

        public bool SaveCurrent()
        {
            SaveSlotData current = Find(SelectedSlot);
            if (current == null) return false;
            current.lastSavedText = Now();
            return true;
        }

        public void Delete(int slotIndex)
        {
            _slots.RemoveAll(slot => slot.slotIndex == slotIndex);
            if (SelectedSlot == slotIndex) SelectedSlot = -1;
        }

        public SaveSlotData Find(int slotIndex)
        {
            foreach (SaveSlotData slot in _slots)
            {
                if (slot.slotIndex == slotIndex) return slot;
            }
            return null;
        }

        private static string Now() => DateTime.Now.ToString("yyyy-MM-dd HH:mm");
    }
}
