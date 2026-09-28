using System.Text;
using TMPro;
using UnityEngine;

namespace UIFlow
{
    /// <summary>Khung theo dõi nhiệm vụ bên phải HUD: chỉ hiện nhiệm vụ đang được theo dõi.</summary>
    public class QuestTracker : MonoBehaviour
    {
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private int maxQuests = 3;

        private readonly StringBuilder _builder = new();

        private void OnEnable()
        {
            UIServices.Quests.Changed += Refresh;
            Refresh();
        }

        private void OnDisable()
        {
            UIServices.Quests.Changed -= Refresh;
        }

        public void Refresh()
        {
            _builder.Clear();
            int shown = 0;
            foreach (QuestData quest in UIServices.Quests.GetQuests())
            {
                if (!quest.isTracked || quest.isCompleted || shown >= maxQuests) continue;
                shown++;
                _builder.Append("<b>").Append(quest.title).Append("</b>\n");
                foreach (QuestObjective objective in quest.objectives)
                {
                    string color = objective.IsDone ? "#7CFC7C" : "#DDDDDD";
                    _builder.Append($"<color={color}>• {objective.description} {objective.current}/{objective.required}</color>\n");
                }
                _builder.Append('\n');
            }
            bodyText.text = shown == 0 ? "<color=#888>Không theo dõi nhiệm vụ nào</color>" : _builder.ToString();
        }
    }
}
