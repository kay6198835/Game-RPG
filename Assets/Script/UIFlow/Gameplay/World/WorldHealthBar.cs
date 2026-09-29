using UnityEngine;
using UnityEngine.UI;

namespace UIFlow
{
    /// <summary>
    /// Thanh máu trên đầu quái, dùng Canvas kiểu WORLD SPACE: canvas là một vật trong thế giới,
    /// di chuyển cùng quái và bị camera nhìn như mọi sprite khác.
    /// Canvas World Space tính kích thước theo đơn vị thế giới → scale canvas rất nhỏ (0.01) để
    /// 100 px UI = 1 unit, nếu không thanh máu sẽ to bằng cả phòng.
    /// </summary>
    public class WorldHealthBar : MonoBehaviour
    {
        [SerializeField] private Image fill;
        [SerializeField] private Image trail;
        [SerializeField] private Vector2 offset = new(0f, 0.9f);
        [SerializeField] private bool hideWhenFull = true;
        [SerializeField] private float trailSpeed = 0.8f;

        private Transform _target;
        private CanvasGroup _group;
        private float _ratio = 1f;

        private void Awake()
        {
            _group = GetComponent<CanvasGroup>();
        }

        /// <summary>Gắn thanh máu vào một vật để đi theo. Không làm con của vật để không bị lật khi quái xoay/lật sprite.</summary>
        public void Follow(Transform target) => _target = target;

        public void SetRatio(float ratio)
        {
            _ratio = Mathf.Clamp01(ratio);
            fill.fillAmount = _ratio;
            if (trail != null && trail.fillAmount < _ratio) trail.fillAmount = _ratio;
            if (_group != null) _group.alpha = hideWhenFull && _ratio >= 1f ? 0f : 1f;
        }

        private void LateUpdate()
        {
            // LateUpdate: chạy sau khi quái đã di chuyển trong Update → thanh máu không bị trễ 1 frame.
            if (_target != null) transform.position = (Vector2)_target.position + offset;
            if (trail != null && trail.fillAmount > _ratio)
                trail.fillAmount = Mathf.MoveTowards(trail.fillAmount, _ratio, trailSpeed * Time.deltaTime);
        }
    }
}
