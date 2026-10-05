using UnityEngine;
using UnityEngine.SceneManagement;

namespace UIFlow
{
    /// <summary>
    /// Truyền "scene đích" sang màn Loading và điều khiển luồng chuyển scene.
    ///
    /// Vì sao dùng static class (không dùng ScriptableObject hay DontDestroyOnLoad)?
    /// - Biến static sống qua mọi lần đổi scene mà không cần GameObject nào tồn tại → không có
    ///   singleton DontDestroyOnLoad chen vào scene gameplay cũ.
    /// - ScriptableObject cũng giữ được giá trị, nhưng trong Editor giá trị runtime có thể bị ghi
    ///   ngược vào file .asset (project đã từng dính lỗi này: BUG-063) → tránh.
    /// - Dữ liệu cần truyền chỉ là một chuỗi tên scene, không cần chỉnh trong Inspector.
    /// </summary>
    public static class SceneFlow
    {
        /// <summary>Scene mà màn Loading cần load. Loading đọc giá trị này trong Start().</summary>
        public static string TargetScene { get; private set; } = SceneNames.MainGamePlay;

        /// <summary>Sau khi scene đích load xong, có load thêm scene GameplayUI (Additive) không.</summary>
        public static bool AttachGameplayUI { get; private set; }

        /// <summary>Đi tới một scene bất kỳ, luôn qua màn Loading.</summary>
        public static void GoTo(string targetScene, bool attachGameplayUI = false)
        {
            TargetScene = targetScene;
            AttachGameplayUI = attachGameplayUI;

            // Menu tạm dừng đặt timeScale = 0. Nếu quên trả về 1, scene sau sẽ đứng hình.
            Time.timeScale = 1f;

            if (attachGameplayUI)
            {
                // Đăng ký một lần: khi scene đích load xong thì gắn thêm GameplayUI.
                SceneManager.sceneLoaded -= OnSceneLoaded;
                SceneManager.sceneLoaded += OnSceneLoaded;
            }

            SceneManager.LoadScene(SceneNames.Loading);
        }

        /// <summary>Vào gameplay: load LoadRandomMap rồi gắn GameplayUI (HUD) lên trên.</summary>
        public static void EnterGameplay()
        {
            // TODO: nối logic thật — khi có hệ thống save, truyền slot đang chọn vào gameplay ở đây.
            GoTo(SceneNames.GameplayReal, attachGameplayUI: true);
        }

        /// <summary>Thoát gameplay về menu chính.</summary>
        public static void ReturnToMainMenu()
        {
            GoTo(SceneNames.MainGamePlay);
        }

        // Nếu bật "Enter Play Mode Options" (tắt Domain Reload), biến static không tự reset
        // giữa các lần bấm Play → reset thủ công ở đây.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            TargetScene = SceneNames.MainGamePlay;
            AttachGameplayUI = false;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Chỉ phản ứng với đúng scene đích, bỏ qua chính scene Loading và GameplayUI.
            if (scene.name != TargetScene) return;

            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (!AttachGameplayUI) return;

            // Load Additive: scene đích vẫn chạy nguyên vẹn, UI chỉ được "đặt chồng" lên trên.
            // Dùng LoadScene (đồng bộ, hoàn tất ở frame kế) thay vì LoadSceneAsync: bản async bị Unity chia nhỏ
            // theo backgroundLoadingPriority nên scene nhiều UI này mất 16–20 giây mới hiện HUD (đã đo).
            SceneManager.LoadScene(SceneNames.GameplayUI, LoadSceneMode.Additive);
        }
    }
}
