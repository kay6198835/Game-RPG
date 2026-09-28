using System;
using System.Collections.Generic;

namespace UIFlow
{
    /// <summary>
    /// Nhiệm vụ thật: để trống vì gameplay chưa có hệ thống nhiệm vụ.
    /// TODO: nối logic thật — bọc hệ thống nhiệm vụ khi có.
    /// </summary>
    public class RealQuestProvider : IQuestProvider
    {
        private readonly List<QuestData> _quests = new();

        public event Action Changed { add { } remove { } }

        public IReadOnlyList<QuestData> GetQuests() => _quests;

        public void SetTracked(string questId, bool tracked) { }
    }
}
