using System.Collections.Generic;
using UnityEngine;

namespace UIFlow
{
    /// <summary>
    /// Kho dữ liệu mẫu cho mọi bản Mock. Gom về một chỗ để dễ chỉnh khi test UI.
    /// TODO: nối logic thật — khi có dữ liệu thật, các provider Real không dùng lớp này nữa.
    /// </summary>
    public static class MockCatalog
    {
        public static List<SaveSlotData> CreateSaves()
        {
            return new List<SaveSlotData>
            {
                new() { slotIndex = 0, characterName = "Aria", className = "Paladin", level = 12,
                        playTimeSeconds = 5 * 3600 + 23 * 60, locationName = "Hầm mộ tầng 3",
                        lastSavedText = "2026-09-27 21:14" },
                new() { slotIndex = 1, characterName = "Bram", className = "Ranger", level = 4,
                        playTimeSeconds = 48 * 60 + 10, locationName = "Phòng khởi đầu",
                        lastSavedText = "2026-09-20 18:02" },
                new() { slotIndex = 2, characterName = "Cyra", className = "Mage", level = 27,
                        playTimeSeconds = 19 * 3600 + 5 * 60, locationName = "Phòng Boss",
                        lastSavedText = "2026-09-25 23:47" },
            };
        }

        public static List<ServerInfo> CreateServers()
        {
            return new List<ServerInfo>
            {
                new() { serverName = "Asia - Hà Nội", status = ServerStatus.Online, pingMs = 24 },
                new() { serverName = "Asia - Singapore", status = ServerStatus.Busy, pingMs = 58 },
                new() { serverName = "EU - Frankfurt", status = ServerStatus.Full, pingMs = 210 },
                new() { serverName = "Test Server", status = ServerStatus.Maintenance, pingMs = 0 },
            };
        }

        public static PlayerStatsData CreatePlayerStats(string characterName, string className, int level)
        {
            return new PlayerStatsData
            {
                characterName = characterName,
                className = className,
                level = level,
                maxHP = 100 + level * 12,
                currentHP = 100 + level * 12,
                maxMana = 50 + level * 5,
                currentMana = 50 + level * 5,
                exp = 340,
                expToNextLevel = 1000,
                attributes = new List<StatLine>
                {
                    new("STR", 12), new("DEX", 9), new("INT", 7), new("VIT", 11), new("LUK", 5),
                    new("Physical Damage", 24), new("Magic Damage", 10), new("Defense", 8),
                    new("Crit Chance %", 5), new("Move Speed", 5.5f),
                },
            };
        }

        public static List<SkillSlotData> CreateHotbar()
        {
            return new List<SkillSlotData>
            {
                new() { keyLabel = "LMB", skillName = "Đánh thường", iconColor = new Color(0.8f, 0.8f, 0.8f), cooldownSeconds = 0.4f },
                new() { keyLabel = "RMB", skillName = "Blessed Slash", iconColor = new Color(1f, 0.85f, 0.3f), cooldownSeconds = 3f },
                new() { keyLabel = "E", skillName = "Consecrate", iconColor = new Color(0.4f, 0.8f, 1f), cooldownSeconds = 6f },
                new() { keyLabel = "Space", skillName = "Lướt", iconColor = new Color(0.6f, 1f, 0.6f), cooldownSeconds = 1.5f },
                new() { keyLabel = "R", skillName = "Avatar of Light", iconColor = new Color(1f, 0.5f, 0.3f), cooldownSeconds = 30f },
            };
        }

        public static List<SkillNodeData> CreateSkillTree()
        {
            return new List<SkillNodeData>
            {
                new() { id = "slash", displayName = "Blessed Slash", tier = 0, cost = 1, unlocked = true,
                        description = "Chém ra một luồng sáng bay thẳng." },
                new() { id = "bless", displayName = "Blessing", tier = 0, cost = 1,
                        description = "Hồi máu theo thời gian." },
                new() { id = "slash2", displayName = "Slash+ (xuyên)", tier = 1, cost = 2, prerequisiteId = "slash",
                        description = "Luồng sáng xuyên qua kẻ địch." },
                new() { id = "consecrate", displayName = "Consecrate", tier = 1, cost = 2, prerequisiteId = "bless",
                        description = "Gọi sét thánh xuống vùng chọn." },
                new() { id = "avatar", displayName = "Avatar of Light", tier = 2, cost = 3, prerequisiteId = "consecrate",
                        description = "Hóa thân ánh sáng: tăng mạnh mọi chỉ số, tốn 50 HP." },
            };
        }

        public static List<ItemData> CreateInventoryItems()
        {
            return new List<ItemData>
            {
                Item("sword_iron", "Kiếm sắt", ItemSlotType.Weapon, ItemRarity.Common, new Color(0.75f, 0.75f, 0.8f),
                     60, "Kiếm cơ bản.", new StatLine("Physical Damage", 12), new StatLine("Attack Speed", 1.0f)),
                Item("sword_holy", "Kiếm thánh", ItemSlotType.Weapon, ItemRarity.Epic, new Color(1f, 0.85f, 0.3f),
                     900, "Tỏa ánh sáng nhàn nhạt.", new StatLine("Physical Damage", 28), new StatLine("Magic Damage", 10),
                     new StatLine("Attack Speed", 0.9f)),
                Item("armor_leather", "Giáp da", ItemSlotType.Armor, ItemRarity.Uncommon, new Color(0.6f, 0.4f, 0.25f),
                     120, "Nhẹ, dễ di chuyển.", new StatLine("Defense", 6), new StatLine("Move Speed", 0.3f)),
                Item("helmet_iron", "Mũ sắt", ItemSlotType.Helmet, ItemRarity.Rare, new Color(0.5f, 0.55f, 0.65f),
                     300, "Nặng nhưng chắc.", new StatLine("Defense", 9), new StatLine("Move Speed", -0.2f)),
                Item("ring_luck", "Nhẫn may mắn", ItemSlotType.Accessory, ItemRarity.Legendary, new Color(0.3f, 1f, 0.6f),
                     2000, "Cỏ bốn lá khắc trên mặt nhẫn.", new StatLine("LUK", 8), new StatLine("Crit Chance %", 6)),
                Item("potion_hp", "Bình máu", ItemSlotType.Consumable, ItemRarity.Common, new Color(0.9f, 0.2f, 0.25f),
                     25, "Hồi 50 HP.", new StatLine("Heal", 50)),
                Item("potion_mp", "Bình mana", ItemSlotType.Consumable, ItemRarity.Common, new Color(0.25f, 0.4f, 0.95f),
                     25, "Hồi 30 Mana.", new StatLine("Mana", 30)),
                Item("ore_platinum", "Quặng bạch kim", ItemSlotType.Material, ItemRarity.Rare, new Color(0.85f, 0.9f, 0.95f),
                     80, "Nguyên liệu rèn."),
            };
        }

        public static List<ItemData> CreateEquipped()
        {
            return new List<ItemData>
            {
                Item("sword_rusty", "Kiếm gỉ", ItemSlotType.Weapon, ItemRarity.Common, new Color(0.6f, 0.45f, 0.35f),
                     20, "Còn dùng được.", new StatLine("Physical Damage", 8), new StatLine("Attack Speed", 1.1f)),
                Item("armor_cloth", "Áo vải", ItemSlotType.Armor, ItemRarity.Common, new Color(0.8f, 0.75f, 0.65f),
                     15, "Chỉ che được gió.", new StatLine("Defense", 2), new StatLine("Move Speed", 0.5f)),
            };
        }

        public static List<ItemData> CreateShopStock()
        {
            return new List<ItemData>
            {
                Item("potion_hp", "Bình máu", ItemSlotType.Consumable, ItemRarity.Common, new Color(0.9f, 0.2f, 0.25f),
                     25, "Hồi 50 HP.", new StatLine("Heal", 50)),
                Item("potion_mp", "Bình mana", ItemSlotType.Consumable, ItemRarity.Common, new Color(0.25f, 0.4f, 0.95f),
                     25, "Hồi 30 Mana.", new StatLine("Mana", 30)),
                Item("sword_steel", "Kiếm thép", ItemSlotType.Weapon, ItemRarity.Uncommon, new Color(0.8f, 0.85f, 0.9f),
                     250, "Sắc và bền.", new StatLine("Physical Damage", 18), new StatLine("Attack Speed", 1.0f)),
                Item("armor_chain", "Giáp xích", ItemSlotType.Armor, ItemRarity.Rare, new Color(0.55f, 0.6f, 0.7f),
                     480, "Chặn được phần lớn vết chém.", new StatLine("Defense", 12), new StatLine("Move Speed", -0.3f)),
            };
        }

        public static List<QuestData> CreateQuests()
        {
            return new List<QuestData>
            {
                new()
                {
                    id = "q_clear", title = "Dọn sạch hầm mộ", isTracked = true,
                    description = "Tiêu diệt quái trong 5 phòng để mở cửa xuống tầng dưới.",
                    objectives = new List<QuestObjective>
                    {
                        new() { description = "Dọn phòng", current = 2, required = 5 },
                        new() { description = "Hạ Golem", current = 0, required = 1 },
                    },
                },
                new()
                {
                    id = "q_ore", title = "Quặng cho thợ rèn", isTracked = true,
                    description = "Thợ rèn cần quặng bạch kim để rèn kiếm mới.",
                    objectives = new List<QuestObjective> { new() { description = "Nhặt quặng bạch kim", current = 3, required = 6 } },
                },
                new()
                {
                    id = "q_intro", title = "Bước chân đầu tiên", isCompleted = true,
                    description = "Nói chuyện với người gác cổng.",
                    objectives = new List<QuestObjective> { new() { description = "Nói chuyện với người gác cổng", current = 1, required = 1 } },
                },
            };
        }

        public static List<DialogueLine> CreateNpcDialogue()
        {
            return new List<DialogueLine>
            {
                new() { speaker = "Thợ rèn", text = "Lại là cậu à? Dưới hầm mộ dạo này quái đông lắm, cẩn thận đấy." },
                new() { speaker = "Thợ rèn", text = "Nếu tìm được quặng bạch kim, mang về đây, ta sẽ rèn cho cậu một thanh kiếm ra hồn." },
                new() { speaker = "Người chơi", text = "Có gì bán không?" },
                new() { speaker = "Thợ rèn", text = "Có chứ. Xem đi, giá phải chăng thôi." },
            };
        }

        public static readonly string[] LoadingTips =
        {
            "Mẹo: Giữ chuột phải để dùng kỹ năng của vũ khí.",
            "Mẹo: Dọn hết quái trong phòng thì cửa mới mở.",
            "Mẹo: Nhấn I để mở túi đồ, Q/E để chuyển tab.",
            "Mẹo: Rê chuột lên món đồ để so sánh với đồ đang mặc.",
            "Mẹo: Nhấn Esc để tạm dừng và lưu game.",
            "Mẹo: Avatar of Light tốn cả máu — đừng dùng khi sắp chết.",
        };

        private static ItemData Item(string id, string name, ItemSlotType slot, ItemRarity rarity, Color color,
                                     int buyPrice, string description, params StatLine[] stats)
        {
            return new ItemData
            {
                id = id, displayName = name, slotType = slot, rarity = rarity, iconColor = color,
                buyPrice = buyPrice, sellPrice = Mathf.Max(1, buyPrice / 2), description = description,
                stats = new List<StatLine>(stats),
            };
        }
    }
}
