using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UIFlow
{
    /// <summary>
    /// Một dòng gán phím: tên hành động + nút hiện phím hiện tại. Bấm nút → chờ phím mới.
    /// Đang chờ mà nhấn Esc = hủy. Trùng phím với hành động khác = hoán đổi 2 phím.
    /// </summary>
    public class KeyBindingRow : MonoBehaviour
    {
        [SerializeField] private TMP_Text actionLabel;
        [SerializeField] private Button keyButton;
        [SerializeField] private TMP_Text keyLabel;

        private KeyBindings.Binding _binding;
        private bool _listening;
        private int _listenStartFrame;
        private System.Action<string> _onMessage;

        public void Bind(KeyBindings.Binding binding, System.Action<string> onMessage)
        {
            _binding = binding;
            _onMessage = onMessage;
            actionLabel.text = binding.isUiKey ? binding.displayName : binding.displayName + " <size=70%><color=#999>(gameplay)</color></size>";
            keyButton.onClick.RemoveAllListeners();
            keyButton.onClick.AddListener(StartListening);
            Refresh();
        }

        public void Refresh()
        {
            if (_binding != null) keyLabel.text = KeyBindings.DisplayName(_binding.CurrentKey);
        }

        private void StartListening()
        {
            _listening = true;
            _listenStartFrame = Time.frameCount;
            UIManager.BlockEscape = true;   // Để Esc không đóng màn Cài đặt trong lúc đang chờ phím
            keyLabel.text = "Nhấn phím…";
        }

        private void Update()
        {
            // Bỏ qua frame bắt đầu: bấm nút bằng Enter/Space thì chính phím đó không bị gán nhầm.
            if (!_listening || Time.frameCount == _listenStartFrame) return;

            Key pressed = UIInput.AnyKeyPressedThisFrame();
            if (pressed == Key.None) return;

            _listening = false;
            // Esc hủy, trừ khi đang gán cho chính hành động "Tạm dừng".
            if (pressed == Key.Escape && _binding.id != KeyBindings.Pause)
            {
                Refresh();
                StartCoroutine(ReleaseEscapeNextFrame());
                return;
            }

            Key oldKey = _binding.CurrentKey;
            KeyBindings.Binding conflict = KeyBindings.FindConflict(_binding.id, pressed);
            if (conflict != null)
            {
                KeyBindings.Set(conflict.id, oldKey);
                _onMessage?.Invoke($"\"{conflict.displayName}\" đổi sang {KeyBindings.DisplayName(oldKey)} vì trùng phím.");
            }
            KeyBindings.Set(_binding.id, pressed);
            Refresh();
            StartCoroutine(ReleaseEscapeNextFrame());
        }

        private IEnumerator ReleaseEscapeNextFrame()
        {
            // Mở khóa Esc ở frame SAU: nếu mở ngay, UIManager.Update cùng frame vẫn thấy Esc và đóng panel.
            yield return null;
            UIManager.BlockEscape = false;
        }

        private void OnDisable()
        {
            if (_listening) UIManager.BlockEscape = false;
            _listening = false;
        }
    }
}
