using System;
using System.Collections.Generic;
using UnityEngine;

namespace UIFlow
{
    /// <summary>
    /// Quản lý mở/đóng panel trong MỘT scene bằng một ngăn xếp (stack).
    /// - Open(panel): đẩy panel lên đỉnh.
    /// - Esc: đóng panel trên đỉnh → luôn quay về màn trước; hết panel thì về HUD / menu gốc.
    /// - Ở scene gameplay: Esc khi không có panel nào → mở escapeFallbackPanel (menu tạm dừng).
    ///
    /// Lưu ý: project còn một lớp UIManager rỗng cũ ở Assets/Script/Manager/UI/. Lớp này nằm trong
    /// namespace UIFlow nên không đụng tên; trong code UIFlow, "UIManager" luôn là lớp này.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Tooltip("Panel gốc: luôn nằm dưới cùng, Esc không đóng được (menu chính hoặc HUD).")]
        [SerializeField] private UIPanel rootPanel;

        [Tooltip("Mở panel này khi nhấn Esc mà không có panel nào đang mở (menu tạm dừng). Để trống ở scene menu.")]
        [SerializeField] private UIPanel escapeFallbackPanel;

        [Tooltip("Cho phép UIManager đặt Time.timeScale theo cờ PausesGame của panel. Chỉ bật ở scene gameplay.")]
        [SerializeField] private bool controlsTimeScale;

        private readonly List<UIPanel> _stack = new();

        /// <summary>Đặt true khi đang chờ người chơi bấm phím mới (màn gán phím) để Esc không đóng panel.</summary>
        public static bool BlockEscape { get; set; }

        public event Action StackChanged;

        public UIPanel Top => _stack.Count > 0 ? _stack[_stack.Count - 1] : rootPanel;

        /// <summary>Không có panel nào ngoài root → người chơi đang ở HUD / menu gốc.</summary>
        public bool IsAtRoot => _stack.Count == 0;

        private void Awake()
        {
            // Cho mọi panel con biết UIManager của nó, để panel gọi được CloseSelf().
            foreach (UIPanel panel in GetComponentsInChildren<UIPanel>(true)) panel.Bind(this);
        }

        private void Start()
        {
            if (rootPanel != null) rootPanel.Show();
        }

        private void OnDestroy()
        {
            // Rời scene khi đang tạm dừng thì phải trả thời gian về bình thường.
            if (controlsTimeScale) Time.timeScale = 1f;
            BlockEscape = false;
        }

        private void Update()
        {
            if (BlockEscape || !UIInput.WasPressed(KeyBindings.Pause)) return;
            HandleEscape();
        }

        public void HandleEscape()
        {
            if (_stack.Count > 0)
            {
                if (Top.CloseOnEscape) Back();
                return;
            }
            if (escapeFallbackPanel != null) Open(escapeFallbackPanel);
        }

        /// <summary>Thay panel gốc (logo → đăng nhập → menu chính). Đóng hết panel đang mở.</summary>
        public void SetRoot(UIPanel panel)
        {
            CloseAll();
            if (rootPanel != null && rootPanel != panel) rootPanel.Hide();
            rootPanel = panel;
            if (rootPanel != null) rootPanel.Show();
            Refresh();
        }

        public void Open(UIPanel panel)
        {
            if (panel == null) return;
            if (_stack.Contains(panel))
            {
                // Đã mở rồi → chỉ đưa lên đỉnh.
                _stack.Remove(panel);
            }
            else if (panel.HidePrevious && Top != null)
            {
                Top.Hide();
            }
            _stack.Add(panel);
            panel.Show();
            Refresh();
        }

        public void Close(UIPanel panel)
        {
            if (panel == null || !_stack.Contains(panel)) return;

            bool wasTop = panel == Top;
            _stack.Remove(panel);
            panel.Hide();

            // Hiện lại panel ngay bên dưới (nó đã bị ẩn lúc panel kia mở).
            if (wasTop && Top != null && !Top.IsVisible) Top.Show();
            Refresh();
        }

        /// <summary>Quay về màn trước.</summary>
        public void Back()
        {
            if (_stack.Count > 0) Close(_stack[_stack.Count - 1]);
        }

        /// <summary>Đóng mọi panel, về HUD / menu gốc.</summary>
        public void CloseAll()
        {
            for (int i = _stack.Count - 1; i >= 0; i--) _stack[i].Hide();
            _stack.Clear();
            if (rootPanel != null) rootPanel.Show();
            Refresh();
        }

        public bool IsOpen(UIPanel panel) => _stack.Contains(panel);

        private void Refresh()
        {
            if (controlsTimeScale)
            {
                bool pause = _stack.Exists(panel => panel.PausesGame);
                Time.timeScale = pause ? 0f : 1f;
            }
            StackChanged?.Invoke();
        }
    }
}
