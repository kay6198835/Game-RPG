using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UIFlow
{
    /// <summary>
    /// Tooltip món đồ: tên theo màu độ hiếm, loại, chỉ số và SO SÁNH với món đang mặc cùng vị trí
    /// (xanh = tốt hơn, đỏ = kém hơn). Đi theo chuột và tự lật sang bên kia khi sát mép màn hình.
    /// CanvasGroup.blocksRaycasts = false để tooltip không "ăn" chuột của ô đồ bên dưới (gây nhấp nháy).
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class ItemTooltip : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private Vector2 cursorOffset = new(18f, -18f);

        private RectTransform _rect;
        private CanvasGroup _group;
        private readonly StringBuilder _builder = new();

        private void Awake()
        {
            _rect = (RectTransform)transform;
            _group = GetComponent<CanvasGroup>();
            _group.blocksRaycasts = false;
            _group.interactable = false;
            Hide();
        }

        public void Show(ItemData item, ItemData equipped, bool showSellPrice)
        {
            if (item == null) { Hide(); return; }

            titleText.text = $"<color=#{ColorUtility.ToHtmlStringRGB(RarityColor(item.rarity))}>{item.displayName}</color>";
            _builder.Clear();
            _builder.Append($"<color=#AAA>{SlotName(item.slotType)} · {RarityName(item.rarity)}</color>\n\n");

            bool compare = item.IsEquipment && equipped != null && equipped != item;
            foreach (StatLine line in item.stats)
            {
                _builder.Append($"{line.statName}: {Format(line.value)}");
                if (compare) AppendDiff(line.value - equipped.GetStat(line.statName));
                _builder.Append('\n');
            }

            if (compare)
            {
                // Chỉ số món đang mặc có mà món này không có → món này mất chỉ số đó.
                foreach (StatLine line in equipped.stats)
                {
                    if (!HasStat(item.stats, line.statName)) _builder.Append($"<color=#888>{line.statName}: 0</color>").Append(DiffText(-line.value)).Append('\n');
                }
                _builder.Append($"\n<color=#888>So với đang mặc: {equipped.displayName}</color>\n");
            }
            else if (item.IsEquipment && equipped == null)
            {
                _builder.Append("\n<color=#888>Chưa mặc gì ở vị trí này</color>\n");
            }

            if (!string.IsNullOrEmpty(item.description)) _builder.Append($"\n<i>{item.description}</i>\n");
            _builder.Append(showSellPrice ? $"\n<color=#FFD54A>Bán: {item.sellPrice} vàng</color>" : $"\n<color=#FFD54A>Giá: {item.buyPrice} vàng</color>");
            if (item.IsEquipment) _builder.Append("\n<color=#888>Chuột phải: mặc vào</color>");

            bodyText.text = _builder.ToString();
            _group.alpha = 1f;
            FollowMouse();
        }

        public void Hide()
        {
            if (_group != null) _group.alpha = 0f;
        }

        private void Update()
        {
            if (_group.alpha > 0f) FollowMouse();
        }

        private void FollowMouse()
        {
            if (Mouse.current == null) return;
            Vector2 mouse = Mouse.current.position.ReadValue();

            // Lật pivot khi gần mép phải / mép dưới để tooltip luôn nằm trong màn hình.
            bool flipX = mouse.x > Screen.width * 0.65f;
            bool flipY = mouse.y < Screen.height * 0.35f;
            _rect.pivot = new Vector2(flipX ? 1f : 0f, flipY ? 0f : 1f);
            Vector2 offset = new(flipX ? -cursorOffset.x : cursorOffset.x, flipY ? -cursorOffset.y : cursorOffset.y);

            // Canvas kiểu Screen Space - Overlay: tọa độ màn hình = tọa độ thế giới của UI.
            _rect.position = mouse + offset;
        }

        private void AppendDiff(float diff) => _builder.Append(DiffText(diff));

        private static string DiffText(float diff)
        {
            if (Mathf.Approximately(diff, 0f)) return "  <color=#888>(=)</color>";
            return diff > 0f ? $"  <color=#7CFC7C>(+{Format(diff)})</color>" : $"  <color=#FF6B6B>({Format(diff)})</color>";
        }

        private static bool HasStat(List<StatLine> stats, string statName) => stats.Exists(s => s.statName == statName);

        private static string Format(float value) => Mathf.Approximately(value, Mathf.Round(value)) ? value.ToString("0") : value.ToString("0.0#");

        public static Color RarityColor(ItemRarity rarity) => rarity switch
        {
            ItemRarity.Uncommon => new Color(0.4f, 0.9f, 0.4f),
            ItemRarity.Rare => new Color(0.35f, 0.6f, 1f),
            ItemRarity.Epic => new Color(0.75f, 0.4f, 1f),
            ItemRarity.Legendary => new Color(1f, 0.6f, 0.15f),
            _ => new Color(0.9f, 0.9f, 0.9f),
        };

        private static string RarityName(ItemRarity rarity) => rarity switch
        {
            ItemRarity.Uncommon => "Khá",
            ItemRarity.Rare => "Hiếm",
            ItemRarity.Epic => "Sử thi",
            ItemRarity.Legendary => "Huyền thoại",
            _ => "Thường",
        };

        public static string SlotName(ItemSlotType slot) => slot switch
        {
            ItemSlotType.Weapon => "Vũ khí",
            ItemSlotType.Armor => "Giáp",
            ItemSlotType.Helmet => "Mũ",
            ItemSlotType.Accessory => "Phụ kiện",
            ItemSlotType.Consumable => "Tiêu hao",
            _ => "Nguyên liệu",
        };
    }
}
