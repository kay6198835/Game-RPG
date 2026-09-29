using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIFlow
{
    /// <summary>Trang Nhiệm vụ: danh sách bên trái, chi tiết bên phải, nút bật/tắt theo dõi trên HUD.</summary>
    public class QuestPanel : TabPage
    {
        [SerializeField] private Transform listContainer;
        [SerializeField] private Button questButtonTemplate;
        [SerializeField] private TMP_Text detailText;
        [SerializeField] private Toggle trackToggle;

        private readonly List<Button> _buttons = new();
        private readonly StringBuilder _builder = new();
        private string _selectedId;

        public override string Title => "Nhiệm vụ";

        private void Awake()
        {
            questButtonTemplate.gameObject.SetActive(false);
            trackToggle.onValueChanged.AddListener(OnTrackChanged);
        }

        private void OnEnable() => UIServices.Quests.Changed += Rebuild;

        private void OnDisable() => UIServices.Quests.Changed -= Rebuild;

        public override void OnTabShown() => Rebuild();

        private void Rebuild()
        {
            foreach (Button button in _buttons) Destroy(button.gameObject);
            _buttons.Clear();

            IReadOnlyList<QuestData> quests = UIServices.Quests.GetQuests();
            foreach (QuestData quest in quests)
            {
                Button button = Instantiate(questButtonTemplate, listContainer);
                button.gameObject.SetActive(true);
                string state = quest.isCompleted ? "<color=#7CFC7C>[Xong]</color> " : (quest.isTracked ? "<color=#FFD54A>●</color> " : "   ");
                button.GetComponentInChildren<TMP_Text>().text = state + quest.title;
                string id = quest.id;
                button.onClick.AddListener(() => Select(id));
                _buttons.Add(button);
            }

            if (quests.Count == 0)
            {
                detailText.text = "Chưa có nhiệm vụ nào.";
                trackToggle.gameObject.SetActive(false);
                return;
            }
            Select(_selectedId ?? quests[0].id);
        }

        private void Select(string id)
        {
            QuestData quest = Find(id);
            if (quest == null) return;
            _selectedId = id;

            _builder.Clear();
            _builder.Append($"<size=120%><b>{quest.title}</b></size>\n{quest.description}\n\n<b>Mục tiêu</b>\n");
            foreach (QuestObjective objective in quest.objectives)
            {
                string mark = objective.IsDone ? "<color=#7CFC7C>[x]</color>" : "[ ]";
                _builder.Append($"{mark} {objective.description}  {objective.current}/{objective.required}\n");
            }
            if (quest.isCompleted) _builder.Append("\n<color=#7CFC7C>Đã hoàn thành</color>");
            detailText.text = _builder.ToString();

            trackToggle.gameObject.SetActive(!quest.isCompleted);
            trackToggle.SetIsOnWithoutNotify(quest.isTracked);   // Không bắn OnTrackChanged khi chỉ hiển thị
        }

        private void OnTrackChanged(bool tracked)
        {
            if (_selectedId != null) UIServices.Quests.SetTracked(_selectedId, tracked);
        }

        private static QuestData Find(string id)
        {
            foreach (QuestData quest in UIServices.Quests.GetQuests())
            {
                if (quest.id == id) return quest;
            }
            return null;
        }
    }
}
