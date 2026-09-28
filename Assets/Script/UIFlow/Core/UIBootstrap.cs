using UnityEngine;

namespace UIFlow
{
    /// <summary>
    /// Đặt một cái ở mỗi scene UI. Chạy TRƯỚC mọi script UI khác (DefaultExecutionOrder âm)
    /// để UIServices đã có config khi các panel gọi tới trong Awake/Start.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public class UIBootstrap : MonoBehaviour
    {
        [SerializeField] private UIDebugConfig config;
        [Tooltip("Áp cài đặt đã lưu (âm lượng, đồ họa). Chỉ cần bật ở scene đầu tiên.")]
        [SerializeField] private bool applySavedSettings;

        private void Awake()
        {
            if (config == null)
            {
                Debug.LogWarning("[UIBootstrap] Chưa gán UIDebugConfig, dùng cờ mặc định (tất cả BẬT).", this);
            }
            UIServices.Init(config);

            if (applySavedSettings) SettingsStore.ApplySaved();
        }
    }
}
