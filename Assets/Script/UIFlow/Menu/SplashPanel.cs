using System;
using UnityEngine;

namespace UIFlow
{
    /// <summary>
    /// Màn logo: hiện trong splashDuration giây (UIDebugConfig) rồi báo Finished.
    /// Click / Space / Enter để bỏ qua ngay.
    /// </summary>
    public class SplashPanel : UIPanel
    {
        [SerializeField] private RectTransform logo;
        [Tooltip("Logo phóng to nhẹ trong lúc hiện, cho đỡ đơ.")]
        [SerializeField] private float logoGrow = 0.08f;

        private float _timer;
        private bool _finished;

        public event Action Finished;

        protected override void OnShown()
        {
            _timer = 0f;
            _finished = false;
        }

        private void Update()
        {
            if (!IsVisible || _finished) return;

            _timer += Time.unscaledDeltaTime;
            float duration = Mathf.Max(0.01f, UIServices.Config.splashDuration);
            if (logo != null) logo.localScale = Vector3.one * (1f + logoGrow * Mathf.Clamp01(_timer / duration));

            if (_timer >= duration || UIInput.ConfirmPressed())
            {
                _finished = true;
                Finished?.Invoke();
            }
        }
    }
}
