using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIFlow
{
    /// <summary>
    /// Trang Cây kỹ năng: mỗi hàng là một tầng (tier). Màu nút cho biết trạng thái:
    /// xanh = đã học, trắng = học được ngay, xám = thiếu điểm hoặc thiếu kỹ năng trước.
    /// </summary>
    public class SkillTreePanel : TabPage
    {
        [SerializeField] private Transform tierContainer;
        [SerializeField] private GameObject rowTemplate;     // Có HorizontalLayoutGroup
        [SerializeField] private Button nodeTemplate;
        [SerializeField] private TMP_Text pointsText;
        [SerializeField] private TMP_Text detailText;
        [SerializeField] private Button unlockButton;

        private static readonly Color Unlocked = new(0.35f, 0.8f, 0.45f);
        private static readonly Color Available = new(1f, 1f, 1f, 0.9f);
        private static readonly Color Locked = new(0.4f, 0.4f, 0.4f, 0.6f);

        private readonly List<GameObject> _rows = new();
        private readonly Dictionary<string, Button> _nodes = new();
        private string _selectedId;

        public override string Title => "Kỹ năng";

        private void Awake()
        {
            rowTemplate.SetActive(false);
            nodeTemplate.gameObject.SetActive(false);
            unlockButton.onClick.AddListener(UnlockSelected);
        }

        public override void OnTabShown() => Rebuild();

        private void Rebuild()
        {
            foreach (GameObject row in _rows) Destroy(row);
            _rows.Clear();
            _nodes.Clear();

            IReadOnlyList<SkillNodeData> tree = UIServices.Player.GetSkillTree();
            int maxTier = -1;
            foreach (SkillNodeData node in tree) maxTier = Mathf.Max(maxTier, node.tier);

            for (int tier = 0; tier <= maxTier; tier++)
            {
                GameObject row = Instantiate(rowTemplate, tierContainer);
                row.SetActive(true);
                _rows.Add(row);
                foreach (SkillNodeData node in tree)
                {
                    if (node.tier != tier) continue;
                    Button button = Instantiate(nodeTemplate, row.transform);
                    button.gameObject.SetActive(true);
                    button.GetComponentInChildren<TMP_Text>().text = $"{node.displayName}\n<size=75%>{node.cost} điểm</size>";
                    string id = node.id;
                    button.onClick.AddListener(() => Select(id));
                    _nodes[node.id] = button;
                }
            }

            if (tree.Count == 0) detailText.text = "Chưa có cây kỹ năng.";
            RefreshStates();
            if (_selectedId != null) Select(_selectedId);
            else detailText.text = tree.Count == 0 ? "Chưa có cây kỹ năng." : "Chọn một kỹ năng để xem chi tiết.";
        }

        private void RefreshStates()
        {
            pointsText.text = $"Điểm kỹ năng: <color=#FFD54A>{UIServices.Player.SkillPoints}</color>";
            foreach (SkillNodeData node in UIServices.Player.GetSkillTree())
            {
                if (_nodes.TryGetValue(node.id, out Button button)) button.targetGraphic.color = StateColor(node);
            }
        }

        private void Select(string id)
        {
            _selectedId = id;
            SkillNodeData node = Find(id);
            if (node == null) return;

            string requirement = string.IsNullOrEmpty(node.prerequisiteId) ? "Không" : Find(node.prerequisiteId)?.displayName ?? node.prerequisiteId;
            detailText.text = $"<b>{node.displayName}</b>\n{node.description}\n\n<color=#AAA>Cần: {requirement} · Giá: {node.cost} điểm</color>";
            unlockButton.interactable = CanUnlock(node);
            unlockButton.GetComponentInChildren<TMP_Text>().text = node.unlocked ? "Đã học" : "Học";
        }

        private void UnlockSelected()
        {
            if (_selectedId == null || !UIServices.Player.TryUnlockSkill(_selectedId)) return;
            UIEvents.Notify("Đã học " + Find(_selectedId).displayName);
            RefreshStates();
            Select(_selectedId);
        }

        private Color StateColor(SkillNodeData node)
        {
            if (node.unlocked) return Unlocked;
            return CanUnlock(node) ? Available : Locked;
        }

        private bool CanUnlock(SkillNodeData node)
        {
            if (node.unlocked || UIServices.Player.SkillPoints < node.cost) return false;
            if (string.IsNullOrEmpty(node.prerequisiteId)) return true;
            SkillNodeData required = Find(node.prerequisiteId);
            return required == null || required.unlocked;
        }

        private static SkillNodeData Find(string id)
        {
            foreach (SkillNodeData node in UIServices.Player.GetSkillTree())
            {
                if (node.id == id) return node;
            }
            return null;
        }
    }
}
