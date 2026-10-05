using UnityEngine;

namespace UIFlow
{
    /// <summary>
    /// Cấu hình của luồng UI: bỏ qua màn đăng nhập, thời gian logo và màn loading.
    /// Tạo bằng: Create > UIFlow > UI Debug Config (hoặc chạy Tools > UI Flow > Build All).
    /// UI luôn chạy với dữ liệu thật (LoadRandomMap + Player thật), không còn chế độ mock.
    /// </summary>
    [CreateAssetMenu(fileName = "UIDebugConfig", menuName = "UIFlow/UI Debug Config")]
    public class UIDebugConfig : ScriptableObject
    {
        [Header("Luồng menu")]
        [Tooltip("Bỏ qua màn đăng nhập / chọn server, vào thẳng menu chính.")]
        public bool skipLogin = true;

        [Header("Màn hình khởi động / loading")]
        [Tooltip("Logo hiện bao lâu (giây) trước khi vào menu.")]
        [Range(0f, 10f)] public float splashDuration = 2f;

        [Tooltip("Màn loading hiện tối thiểu bao lâu (giây) để người chơi kịp đọc tip.")]
        [Range(0f, 10f)] public float minLoadingTime = 1.5f;
    }
}
