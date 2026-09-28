using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIFlow
{
    /// <summary>
    /// Thanh máu / mana / exp. Hai lớp:
    /// - fill: nhảy ngay tới giá trị mới.
    /// - trail (vệt trễ): tụt từ từ phía sau, để người chơi thấy mình vừa mất bao nhiêu.
    /// Dùng Image kiểu Filled nên không phải đổi kích thước RectTransform.
    /// </summary>
    public class StatBar : MonoBehaviour
    {
        [SerializeField] private Image fill;
        [SerializeField] private Image trail;
        [SerializeField] private TMP_Text valueText;
        [SerializeField] private float trailSpeed = 0.6f;

        private float _target = 1f;

        public void SetValue(float current, float max)
        {
            _target = max > 0f ? Mathf.Clamp01(current / max) : 0f;
            fill.fillAmount = _target;
            if (trail != null && trail.fillAmount < _target) trail.fillAmount = _target;   // Hồi máu: vệt đi cùng luôn
            if (valueText != null) valueText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
        }

        private void Update()
        {
            if (trail == null || trail.fillAmount <= _target) return;
            trail.fillAmount = Mathf.MoveTowards(trail.fillAmount, _target, trailSpeed * Time.unscaledDeltaTime);
        }
    }
}
