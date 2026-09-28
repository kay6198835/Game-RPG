using System;
using System.Collections.Generic;

namespace UIFlow
{
    /// <summary>Danh sách nhiệm vụ cho bảng Nhiệm vụ và khung theo dõi trên HUD.</summary>
    public interface IQuestProvider
    {
        IReadOnlyList<QuestData> GetQuests();

        void SetTracked(string questId, bool tracked);

        event Action Changed;
    }
}
