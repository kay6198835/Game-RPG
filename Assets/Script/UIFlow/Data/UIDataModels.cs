using System;
using System.Collections.Generic;
using UnityEngine;

// Các lớp dữ liệu "thuần" mà UI hiển thị. UI chỉ biết tới những lớp này,
// không biết tới Player / BaseStatsSO / ItemSO của gameplay cũ.
// Bản Real của mỗi provider có nhiệm vụ "dịch" dữ liệu gameplay sang các lớp này.
namespace UIFlow
{
    public enum ItemSlotType { Weapon, Armor, Helmet, Accessory, Consumable, Material }

    public enum ItemRarity { Common, Uncommon, Rare, Epic, Legendary }

    public enum ServerStatus { Online, Busy, Full, Maintenance }

    [Serializable]
    public class StatLine
    {
        public string statName;
        public float value;

        public StatLine(string statName, float value)
        {
            this.statName = statName;
            this.value = value;
        }
    }

    [Serializable]
    public class ItemData
    {
        public string id;
        public string displayName;
        [TextArea] public string description;
        public Sprite icon;                 // null trong mock → UI tô màu iconColor thay thế
        public Color iconColor = Color.white;
        public ItemSlotType slotType;
        public ItemRarity rarity;
        public List<StatLine> stats = new();
        public int buyPrice;
        public int sellPrice;
        public int stackCount = 1;

        public bool IsEquipment => slotType != ItemSlotType.Consumable && slotType != ItemSlotType.Material;

        public ItemData Clone()
        {
            ItemData copy = (ItemData)MemberwiseClone();
            copy.stats = new List<StatLine>();
            foreach (StatLine line in stats) copy.stats.Add(new StatLine(line.statName, line.value));
            return copy;
        }

        /// <summary>Tìm giá trị một chỉ số; không có thì trả 0 (dùng khi so sánh 2 món đồ).</summary>
        public float GetStat(string statName)
        {
            foreach (StatLine line in stats)
            {
                if (line.statName == statName) return line.value;
            }
            return 0f;
        }
    }

    [Serializable]
    public class PlayerStatsData
    {
        public string characterName;
        public string className;
        public int level;
        public float currentHP;
        public float maxHP;
        public float currentMana;
        public float maxMana;
        public int exp;
        public int expToNextLevel;
        public List<StatLine> attributes = new();
    }

    [Serializable]
    public class SaveSlotData
    {
        public int slotIndex;
        public string characterName;
        public string className;
        public int level;
        public float playTimeSeconds;
        public string locationName;
        public string lastSavedText;   // Chuỗi ngày giờ đã format, để JsonUtility lưu được

        public string PlayTimeText
        {
            get
            {
                TimeSpan time = TimeSpan.FromSeconds(playTimeSeconds);
                return $"{(int)time.TotalHours:00}:{time.Minutes:00}:{time.Seconds:00}";
            }
        }
    }

    [Serializable]
    public class QuestObjective
    {
        public string description;
        public int current;
        public int required;

        public bool IsDone => current >= required;
    }

    [Serializable]
    public class QuestData
    {
        public string id;
        public string title;
        [TextArea] public string description;
        public List<QuestObjective> objectives = new();
        public bool isTracked;
        public bool isCompleted;
    }

    [Serializable]
    public class ServerInfo
    {
        public string serverName;
        public ServerStatus status;
        public int pingMs;
    }

    [Serializable]
    public class CharacterClassInfo
    {
        public string id;
        public string displayName;
        [TextArea] public string description;
        public Color tint = Color.white;
        // 8 sprite theo quy ước DirectionResolver của project: 0 = dưới-trái, đi theo chiều kim đồng hồ.
        // Để trống thì preview chỉ hiện ô màu + số hướng.
        public Sprite[] directionSprites = new Sprite[8];
    }

    [Serializable]
    public class NewCharacterRequest
    {
        public string characterName;
        public string classId;
        public int hairIndex;
        public int skinIndex;
        public int colorIndex;
    }

    [Serializable]
    public class SkillNodeData
    {
        public string id;
        public string displayName;
        [TextArea] public string description;
        public int tier;               // Hàng trong cây kỹ năng (0 = hàng đầu)
        public int cost = 1;           // Số điểm kỹ năng cần
        public string prerequisiteId;  // Rỗng = không cần kỹ năng trước
        public bool unlocked;
    }

    [Serializable]
    public class SkillSlotData
    {
        public string keyLabel;        // Phím hiển thị trên hotbar, ví dụ "1", "RMB"
        public string skillName;
        public Color iconColor = Color.white;
        public float cooldownSeconds;
    }

    [Serializable]
    public class DialogueLine
    {
        public string speaker;
        [TextArea] public string text;
    }
}
