using System;
using System.Collections.Generic;
using UnityEngine;

namespace UIFlow
{
    /// <summary>
    /// Nhân vật giả. Tên/class/level lấy từ save đang chọn (nếu có) để luồng menu → game nhất quán.
    /// Các hàm Debug* chỉ dành cho GameplayMockController để test HUD.
    /// TODO: nối logic thật — RealPlayerDataProvider đọc IVitalComponent / StatHandler của Player.
    /// </summary>
    public class MockPlayerDataProvider : IPlayerDataProvider
    {
        private readonly List<SkillSlotData> _hotbar = MockCatalog.CreateHotbar();
        private readonly List<SkillNodeData> _skillTree = MockCatalog.CreateSkillTree();
        private PlayerStatsData _stats;
        private bool _isDead;

        public event Action<PlayerStatsData> StatsChanged;
        public event Action PlayerDied;
        public event Action HotbarChanged { add { } remove { } }   // Hotbar mock cố định

        public int SkillPoints { get; private set; } = 4;

        public PlayerStatsData GetStats()
        {
            if (_stats == null) _stats = BuildStatsFromSelectedSave();
            return _stats;
        }

        public IReadOnlyList<SkillSlotData> GetHotbar() => _hotbar;

        public IReadOnlyList<SkillNodeData> GetSkillTree() => _skillTree;

        public bool TryUnlockSkill(string skillId)
        {
            SkillNodeData node = _skillTree.Find(n => n.id == skillId);
            if (node == null || node.unlocked || SkillPoints < node.cost) return false;

            if (!string.IsNullOrEmpty(node.prerequisiteId))
            {
                SkillNodeData required = _skillTree.Find(n => n.id == node.prerequisiteId);
                if (required != null && !required.unlocked) return false;
            }

            node.unlocked = true;
            SkillPoints -= node.cost;
            return true;
        }

        public void Respawn()
        {
            PlayerStatsData stats = GetStats();
            stats.currentHP = stats.maxHP;
            stats.currentMana = stats.maxMana;
            _isDead = false;
            StatsChanged?.Invoke(stats);
        }

        /// <summary>Gọi khi đổi save (chọn save khác / chơi mới) để đọc lại tên, class, level.</summary>
        public void ResetForNewSave()
        {
            _stats = null;
            _isDead = false;
        }

        public void DebugChangeHP(float amount)
        {
            if (_isDead) return;
            PlayerStatsData stats = GetStats();
            stats.currentHP = Mathf.Clamp(stats.currentHP + amount, 0f, stats.maxHP);
            StatsChanged?.Invoke(stats);

            if (stats.currentHP > 0f) return;
            _isDead = true;          // Cờ này đảm bảo PlayerDied chỉ bắn đúng một lần
            PlayerDied?.Invoke();
        }

        public void DebugChangeMana(float amount)
        {
            PlayerStatsData stats = GetStats();
            stats.currentMana = Mathf.Clamp(stats.currentMana + amount, 0f, stats.maxMana);
            StatsChanged?.Invoke(stats);
        }

        public void DebugAddExp(int amount)
        {
            PlayerStatsData stats = GetStats();
            stats.exp += amount;
            while (stats.exp >= stats.expToNextLevel)
            {
                stats.exp -= stats.expToNextLevel;
                stats.level++;
                SkillPoints++;
            }
            StatsChanged?.Invoke(stats);
        }

        private PlayerStatsData BuildStatsFromSelectedSave()
        {
            string characterName = "Aria";
            string className = "Paladin";
            int level = 12;

            if (UIServices.Save is MockSaveProvider saves)
            {
                SaveSlotData slot = saves.Find(saves.SelectedSlot);
                if (slot != null)
                {
                    characterName = slot.characterName;
                    className = slot.className;
                    level = slot.level;
                }
            }
            return MockCatalog.CreatePlayerStats(characterName, className, level);
        }
    }
}
