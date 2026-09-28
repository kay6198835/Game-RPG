using UnityEngine;

namespace UIFlow
{
    /// <summary>
    /// Chỗ cấu hình DUY NHẤT cho các cờ mock/bypass của luồng UI.
    /// Tạo bằng: Create > UIFlow > UI Debug Config (hoặc chạy Tools > UI Flow > Build All).
    /// Tắt hết cờ = luồng chạy với logic thật, không cần sửa code UI.
    /// </summary>
    [CreateAssetMenu(fileName = "UIDebugConfig", menuName = "UIFlow/UI Debug Config")]
    public class UIDebugConfig : ScriptableObject
    {
        [Header("Cờ mock / bypass (mặc định BẬT khi đang làm UI)")]
        [Tooltip("UI dùng dữ liệu giả: chỉ số nhân vật, túi đồ, nhiệm vụ, cây kỹ năng.")]
        public bool useMockData = true;

        [Tooltip("Bỏ qua màn đăng nhập / chọn server, vào thẳng menu chính.")]
        public bool skipLogin = true;

        [Tooltip("Khi vào game: load scene GameplayMock thay vì scene gameplay thật (LoadRandomMap).")]
        public bool skipGameplayInit = true;

        [Tooltip("Dùng 3 file save mẫu thay vì đọc/ghi save thật.")]
        public bool skipSaveLoad = true;

        [Tooltip("Khi vào LoadRandomMap thật: KHÔNG gắn thêm scene GameplayUI (HUD/pause mới), để scene cũ chạy y như hiện tại.")]
        public bool bypassLoadRandomLogic = true;

        [Header("Màn hình khởi động / loading")]
        [Tooltip("Logo hiện bao lâu (giây) trước khi vào menu.")]
        [Range(0f, 10f)] public float splashDuration = 2f;

        [Tooltip("Màn loading hiện tối thiểu bao lâu (giây) để người chơi kịp đọc tip.")]
        [Range(0f, 10f)] public float minLoadingTime = 1.5f;
    }
}
