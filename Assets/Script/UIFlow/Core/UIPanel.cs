using System.Collections;
using UnityEngine;

namespace UIFlow
{
    /// <summary>
    /// Lớp cha cho mọi panel. Ẩn/hiện bằng CanvasGroup (alpha + chặn click) thay vì SetActive:
    /// - GameObject luôn active nên Awake/OnEnable không chạy lại mỗi lần mở → không mất trạng thái.
    /// - Có thể làm hiệu ứng mờ dần (fade).
    /// UIManager là nơi duy nhất gọi Show/Hide; code khác gọi UIManager.Open / Close.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class UIPanel : MonoBehaviour
    {
        [Header("Hành vi panel")]
        [Tooltip("Nhấn Esc thì panel này đóng lại. Tắt cho logo, loading, Game Over.")]
        [SerializeField] private bool closeOnEscape = true;

        [Tooltip("Ẩn panel bên dưới khi mở panel này (màn hình toàn phần). Tắt cho popup/tooltip.")]
        [SerializeField] private bool hidePrevious = true;

        [Tooltip("Dừng game (Time.timeScale = 0) khi panel này đang mở. Chỉ có tác dụng ở scene gameplay.")]
        [SerializeField] private bool pausesGame;

        [SerializeField, Range(0f, 1f)] private float fadeDuration = 0.12f;

        private CanvasGroup _canvasGroup;
        private Coroutine _fade;

        public bool CloseOnEscape => closeOnEscape;
        public bool HidePrevious => hidePrevious;
        public bool PausesGame => pausesGame;
        public bool IsVisible { get; private set; }

        protected UIManager Manager { get; private set; }

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            SetVisibleImmediate(false);
        }

        /// <summary>UIManager gọi khi đăng ký panel.</summary>
        public void Bind(UIManager manager) => Manager = manager;

        public void Show()
        {
            if (IsVisible) return;
            IsVisible = true;
            transform.SetAsLastSibling();   // Vẽ đè lên các panel khác trong cùng Canvas
            StartFade(1f);
            OnShown();
        }

        public void Hide()
        {
            if (!IsVisible) return;
            IsVisible = false;
            StartFade(0f);
            OnHidden();
        }

        /// <summary>Panel tự đóng chính nó (ví dụ nút "Quay lại").</summary>
        public void CloseSelf()
        {
            if (Manager != null) Manager.Close(this);
            else Hide();
        }

        /// <summary>Ghi đè để làm mới nội dung mỗi lần panel hiện lên.</summary>
        protected virtual void OnShown() { }

        protected virtual void OnHidden() { }

        private void SetVisibleImmediate(bool visible)
        {
            IsVisible = visible;
            _canvasGroup.alpha = visible ? 1f : 0f;
            _canvasGroup.interactable = visible;
            _canvasGroup.blocksRaycasts = visible;
        }

        private void StartFade(float target)
        {
            // Chặn click ngay lập tức, không đợi fade xong → tránh bấm nhầm vào panel đang ẩn dần.
            _canvasGroup.interactable = target > 0f;
            _canvasGroup.blocksRaycasts = target > 0f;

            if (_fade != null) StopCoroutine(_fade);
            if (fadeDuration <= 0f || !isActiveAndEnabled)
            {
                _canvasGroup.alpha = target;
                return;
            }
            _fade = StartCoroutine(Fade(target));
        }

        private IEnumerator Fade(float target)
        {
            float start = _canvasGroup.alpha;
            float time = 0f;
            while (time < fadeDuration)
            {
                // unscaledDeltaTime: vẫn chạy khi game đang tạm dừng (timeScale = 0).
                time += Time.unscaledDeltaTime;
                _canvasGroup.alpha = Mathf.Lerp(start, target, time / fadeDuration);
                yield return null;
            }
            _canvasGroup.alpha = target;
            _fade = null;
        }
    }
}
