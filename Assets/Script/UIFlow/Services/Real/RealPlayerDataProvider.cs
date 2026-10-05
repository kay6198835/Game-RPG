using System;
using System.Collections.Generic;
using UnityEngine;

namespace UIFlow
{
    /// <summary>
    /// Cầu nối tới nhân vật thật trong LoadRandomMap. Đã nối:
    /// - Máu / mana hiện tại (IVitalComponent) và tối đa + level + chỉ số (IStatService) → HUD, bảng Nhân vật.
    /// - Hotbar từ CharacterData.AbilityBindings (4 ô Primary/Secondary/Utility/Ultimate = phím 1–4) và
    ///   cooldown thật từ AbilityHolderBase.AbilityCooldownStarted.
    /// - Sự kiện chết (ON_PLAYER_DEATH) → Game Over.
    ///
    /// Cách tìm Player mà không dùng FindObjectOfType: Player tự báo ON_PLAYER_READY (kèm ICharacter gốc)
    /// sau khi máu/mana được nạp. Provider được tạo sẵn từ UIBootstrap ở scene Menu/Loading, nên luôn nghe kịp
    /// trước khi LoadRandomMap chạy Start(). Từ ICharacter gốc, lấy từng thành phần bằng GetComponentInChildren
    /// (đúng quy ước ADR-0005).
    ///
    /// Lưu ý: PlayerDeathState bắn ON_PLAYER_DEATH MỖI FRAME (BUG-086) → cờ _deathReported chỉ phản ứng một lần.
    /// </summary>
    public class RealPlayerDataProvider : IPlayerDataProvider, IDisposable
    {
        private const string DefaultName = "Paladin";

        // Phím của từng ô, theo PlayerInput.inputactions (map Control): Primary = 1 … Ultimate = 4.
        private static readonly string[] SlotKeyLabels = { "1", "2", "3", "4" };
        private static readonly AbilitySlot[] Slots = (AbilitySlot[])Enum.GetValues(typeof(AbilitySlot));
        private static readonly Color EmptySlotColor = new(1f, 1f, 1f, 0.08f);
        private static readonly Color BoundSlotColor = new(0.95f, 0.85f, 0.4f);

        private readonly PlayerStatsData _stats = new() { characterName = DefaultName, className = DefaultName, level = 1 };
        private readonly List<SkillSlotData> _hotbar = new();
        private readonly List<SkillNodeData> _skillTree = new();
        private IVitalComponent _vital;
        private IStatService _statService;
        private AbilityHolder _abilityHolder;
        private bool _deathReported;

        public event Action<PlayerStatsData> StatsChanged;
        public event Action PlayerDied;
        public event Action HotbarChanged;

        public int SkillPoints => 0;

        /// <summary>Đã gắn với một Player thật chưa (smoke test dùng).</summary>
        public bool IsBound => _vital != null;

        public RealPlayerDataProvider()
        {
            EventManager.Resgister(EventID.ON_PLAYER_DEATH, OnPlayerDeath);
            EventManager.Resgister(EventID.ON_PLAYER_READY, OnPlayerReady);
        }

        public void Dispose()
        {
            EventManager.UnResgister(EventID.ON_PLAYER_DEATH, OnPlayerDeath);
            EventManager.UnResgister(EventID.ON_PLAYER_READY, OnPlayerReady);
            Unbind();
        }

        public PlayerStatsData GetStats()
        {
            RefreshStats();
            return _stats;
        }

        public IReadOnlyList<SkillSlotData> GetHotbar() => _hotbar;

        // TODO: nối logic thật — project chưa có cây kỹ năng (chỉ có TalentManager hardcode).
        public IReadOnlyList<SkillNodeData> GetSkillTree() => _skillTree;

        public bool TryUnlockSkill(string skillId) => false;

        public void Respawn()
        {
            // Gameplay hiện không có đường hồi sinh tại chỗ (BUG-087), nên "hồi sinh" = load lại scene gameplay.
            // Player mới sẽ báo ON_PLAYER_READY và provider tự gắn lại.
            _deathReported = false;
            SceneFlow.EnterGameplay();
        }

        private void OnPlayerReady(object payload)
        {
            if (payload is not ICharacter character) return;
            // Chạy bên trong VitalStatsComponent.Reborn() của gameplay → không để lỗi UI ném ngược vào đó.
            try
            {
                Bind(character);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void Bind(ICharacter character)
        {
            Unbind();
            Transform root = character.Transform;
            _vital = root.GetComponentInChildren<IVitalComponent>();
            _statService = root.GetComponentInChildren<IStatService>();
            _abilityHolder = root.GetComponentInChildren<AbilityHolder>();
            if (_vital == null || _statService == null)
            {
                Debug.LogWarning("[RealPlayerDataProvider] Player thiếu IVitalComponent hoặc IStatService — HUD không có dữ liệu.");
                Unbind();
                return;
            }

            _deathReported = false;
            _vital.CurrentStatsChanged += OnVitalsChanged;
            if (_abilityHolder != null) _abilityHolder.AbilityCooldownStarted += OnCooldownStarted;

            BuildHotbar(root.GetComponentInChildren<CoreBase>());
            HotbarChanged?.Invoke();
            OnVitalsChanged();
        }

        private void Unbind()
        {
            if (_vital != null) _vital.CurrentStatsChanged -= OnVitalsChanged;
            if (_abilityHolder != null) _abilityHolder.AbilityCooldownStarted -= OnCooldownStarted;
            _vital = null;
            _statService = null;
            _abilityHolder = null;
        }

        private void OnVitalsChanged()
        {
            // Hàm này chạy BÊN TRONG VitalStatsBase.Reduction/Recovery của gameplay: lỗi ở UI không được
            // ném ngược vào đó (sẽ làm hỏng luồng nhận damage), nên chặn và log tại đây.
            try
            {
                RefreshStats();
                StatsChanged?.Invoke(_stats);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void RefreshStats()
        {
            // Tên / class lấy từ save đang chơi (game chưa lưu tên trong gameplay).
            SaveSlotData save = FindSelectedSave();
            _stats.characterName = save != null && !string.IsNullOrEmpty(save.characterName) ? save.characterName : DefaultName;
            _stats.className = save != null && !string.IsNullOrEmpty(save.className) ? save.className : DefaultName;

            if (_vital == null || _statService == null) return;

            // Giá trị hiện tại được nạp từ chính GetFullStat() (VitalStatsBase.Reborn), nên cùng bộ khóa:
            // kiểm tra khóa ở đây tránh KeyNotFoundException của GetCurrentStatValue (BUG-066).
            Dictionary<StatType, float> full = _statService.GetFullStat();
            _stats.level = _statService.GetLevel();
            _stats.maxHP = _statService.GetStatValue(StatType.HP);
            _stats.maxMana = _statService.GetStatValue(StatType.Mana);
            _stats.currentHP = full.ContainsKey(StatType.HP) ? _vital.GetCurrentStatValue(StatType.HP) : 0f;
            _stats.currentMana = full.ContainsKey(StatType.Mana) ? _vital.GetCurrentStatValue(StatType.Mana) : 0f;
            // TODO: nối logic thật — gameplay chưa có EXP; expToNextLevel = 0 thì HUD ẩn thanh EXP.
            _stats.exp = 0;
            _stats.expToNextLevel = 0;

            _stats.attributes.Clear();
            foreach (KeyValuePair<StatType, float> pair in full)
            {
                _stats.attributes.Add(new StatLine(pair.Key.ToString(), pair.Value));
            }
        }

        private void BuildHotbar(CoreBase core)
        {
            _hotbar.Clear();
            List<AbilityBinding> bindings = core != null && core.Data != null ? core.Data.AbilityBindings : null;

            for (int i = 0; i < Slots.Length; i++)
            {
                AbilityDefinition ability = FindAbility(bindings, Slots[i]);
                _hotbar.Add(new SkillSlotData
                {
                    keyLabel = i < SlotKeyLabels.Length ? SlotKeyLabels[i] : string.Empty,
                    skillName = ability != null ? ability.DisplayName : string.Empty,
                    icon = ability != null ? ability.Icon : null,
                    iconColor = ability != null ? BoundSlotColor : EmptySlotColor,
                    cooldownSeconds = ability != null ? ability.Cooldown : 0f,
                });
            }
        }

        private static AbilityDefinition FindAbility(List<AbilityBinding> bindings, AbilitySlot slot)
        {
            if (bindings == null) return null;
            foreach (AbilityBinding binding in bindings)
            {
                if (binding != null && binding.Slot == slot && binding.Ability != null) return binding.Ability;
            }
            return null;
        }

        private void OnCooldownStarted(AbilitySlot slot, float duration)
        {
            int index = Array.IndexOf(Slots, slot);
            if (index >= 0 && index < _hotbar.Count) _hotbar[index].cooldownSeconds = duration;
            UIEvents.UseSkill(index);
        }

        private static SaveSlotData FindSelectedSave()
        {
            ISaveProvider saves = UIServices.Save;
            foreach (SaveSlotData slot in saves.GetSlots())
            {
                if (slot.slotIndex == saves.SelectedSlot) return slot;
            }
            return null;
        }

        private void OnPlayerDeath(object _)
        {
            if (_deathReported) return;
            _deathReported = true;
            PlayerDied?.Invoke();
        }
    }
}
