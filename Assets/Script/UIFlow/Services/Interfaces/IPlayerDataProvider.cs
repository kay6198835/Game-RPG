using System;
using System.Collections.Generic;

namespace UIFlow
{
    /// <summary>
    /// Dữ liệu nhân vật cho HUD, bảng Nhân vật, cây kỹ năng, hotbar và màn Game Over.
    /// UI chỉ ĐỌC; thay đổi duy nhất UI được yêu cầu là mở khóa kỹ năng và hồi sinh.
    /// </summary>
    public interface IPlayerDataProvider
    {
        PlayerStatsData GetStats();

        /// <summary>Bắn ra mỗi khi máu/mana/level đổi → HUD cập nhật, không cần poll trong Update.</summary>
        event Action<PlayerStatsData> StatsChanged;

        /// <summary>Bắn ra đúng MỘT lần khi nhân vật chết → mở màn Game Over.</summary>
        event Action PlayerDied;

        /// <summary>Danh sách ô hotbar đổi (ví dụ Player thật vừa xuất hiện) → HUD dựng lại hotbar.</summary>
        event Action HotbarChanged;

        void Respawn();

        IReadOnlyList<SkillSlotData> GetHotbar();

        IReadOnlyList<SkillNodeData> GetSkillTree();

        int SkillPoints { get; }

        bool TryUnlockSkill(string skillId);

        // ── Cộng điểm chỉ số (tab Nhân vật). Một "phiên" mở khi tab hiện lên: điểm cộng trong phiên được trừ lại
        //    từng điểm, Hoàn tác trả hết, Xác nhận chốt lại. Logic nằm ở gameplay (StatPointSession), UI chỉ gọi.

        IReadOnlyList<StatRowData> GetStatRows();

        /// <summary>Điểm level-up còn lại trong phiên đang mở.</summary>
        int UnspentStatPoints { get; }

        /// <summary>Số điểm có lúc mở phiên. 0 → ẩn nút cộng/trừ.</summary>
        int StatPointsAtOpen { get; }

        /// <summary>Đã từng chốt điểm level-up nào chưa → hiện nút Reset điểm.</summary>
        bool CanRespec { get; }

        void BeginStatSession();

        /// <summary>delta = +1 cộng một điểm, -1 trừ lại một điểm đã cộng trong phiên.</summary>
        bool TryChangeStat(StatType statType, int delta);

        void AcceptStats();

        void RevertStats();

        void RespecStats();
    }
}
