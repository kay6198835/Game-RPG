using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIFlow
{
    /// <summary>
    /// Hộp thoại NPC với hiệu ứng chữ chạy (typewriter).
    /// - Click / Space / Enter khi chữ đang chạy → hiện hết câu (tua nhanh).
    /// - Click khi câu đã hiện hết → sang câu tiếp.
    /// - Giữ nút "Tua nhanh" (hoặc bật) → chữ chạy nhanh gấp fastForwardMultiplier.
    /// - Nút "Bỏ qua" / Esc → kết thúc hộp thoại ngay.
    /// Dùng TMP maxVisibleCharacters thay vì cắt chuỗi: không tạo chuỗi mới mỗi frame và thẻ &lt;color&gt; không bị vỡ.
    /// </summary>
    public class DialoguePanel : UIPanel
    {
        [SerializeField] private TMP_Text speakerText;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private GameObject continueHint;
        [SerializeField] private Button nextButton;          // Nút phủ cả hộp thoại, bắt click
        [SerializeField] private Toggle fastForwardToggle;
        [SerializeField] private Button skipButton;
        [SerializeField] private float charactersPerSecond = 40f;
        [SerializeField] private float fastForwardMultiplier = 4f;

        private IReadOnlyList<DialogueLine> _lines;
        private Action _onFinished;
        private int _index;
        private float _visible;
        private int _totalCharacters;

        private bool LineComplete => _visible >= _totalCharacters;

        protected override void Awake()
        {
            base.Awake();
            nextButton.onClick.AddListener(Advance);
            skipButton.onClick.AddListener(CloseSelf);
        }

        public void Play(IReadOnlyList<DialogueLine> lines, Action onFinished)
        {
            _lines = lines;
            _onFinished = onFinished;
            _index = 0;
            if (_lines == null || _lines.Count == 0)
            {
                CloseSelf();
                return;
            }
            ShowLine();
        }

        protected override void OnHidden()
        {
            // Gọi callback ĐÚNG MỘT LẦN dù đóng bằng cách nào (hết câu, Bỏ qua, Esc).
            Action callback = _onFinished;
            _onFinished = null;
            callback?.Invoke();
        }

        private void Update()
        {
            if (!IsVisible || _lines == null) return;

            // Chuột đã do nextButton xử lý; ở đây chỉ bắt Space/Enter để không tua 2 lần một cú click.
            if (UIInput.KeyboardConfirmPressed()) Advance();

            if (LineComplete) return;
            float speed = charactersPerSecond * (fastForwardToggle != null && fastForwardToggle.isOn ? fastForwardMultiplier : 1f);
            // unscaledDeltaTime: hộp thoại dừng game nhưng chữ vẫn phải chạy.
            _visible = Mathf.Min(_totalCharacters, _visible + speed * Time.unscaledDeltaTime);
            bodyText.maxVisibleCharacters = Mathf.FloorToInt(_visible);
            continueHint.SetActive(LineComplete);
        }

        private void ShowLine()
        {
            DialogueLine line = _lines[_index];
            speakerText.text = line.speaker;
            bodyText.text = line.text;
            bodyText.ForceMeshUpdate();   // Cập nhật textInfo để biết số ký tự hiển thị (không tính thẻ rich text)
            _totalCharacters = bodyText.textInfo.characterCount;
            _visible = 0f;
            bodyText.maxVisibleCharacters = 0;
            continueHint.SetActive(false);
        }

        private void Advance()
        {
            if (!LineComplete)
            {
                _visible = _totalCharacters;          // Tua nhanh: hiện hết câu hiện tại
                bodyText.maxVisibleCharacters = _totalCharacters;
                continueHint.SetActive(true);
                return;
            }

            _index++;
            if (_index >= _lines.Count) CloseSelf();
            else ShowLine();
        }
    }
}
