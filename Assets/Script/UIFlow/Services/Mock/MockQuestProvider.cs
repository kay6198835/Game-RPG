using System;
using System.Collections.Generic;

namespace UIFlow
{
    /// <summary>
    /// Nhiệm vụ giả. TODO: nối logic thật — project chưa có hệ thống nhiệm vụ.
    /// </summary>
    public class MockQuestProvider : IQuestProvider
    {
        private readonly List<QuestData> _quests = MockCatalog.CreateQuests();

        public event Action Changed;

        public IReadOnlyList<QuestData> GetQuests() => _quests;

        public void SetTracked(string questId, bool tracked)
        {
            QuestData quest = _quests.Find(q => q.id == questId);
            if (quest == null || quest.isTracked == tracked) return;
            quest.isTracked = tracked;
            Changed?.Invoke();
        }

        /// <summary>Chỉ dùng để test: tăng tiến độ mục tiêu đầu tiên chưa xong. Trả về nhiệm vụ vừa đổi.</summary>
        public QuestData DebugAdvance()
        {
            foreach (QuestData quest in _quests)
            {
                if (quest.isCompleted) continue;
                foreach (QuestObjective objective in quest.objectives)
                {
                    if (objective.IsDone) continue;
                    objective.current++;
                    quest.isCompleted = quest.objectives.TrueForAll(o => o.IsDone);
                    Changed?.Invoke();
                    return quest;
                }
            }
            return null;
        }
    }
}
