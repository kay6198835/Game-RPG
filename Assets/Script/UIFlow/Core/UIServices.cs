using System;
using UnityEngine;

namespace UIFlow
{
    /// <summary>
    /// Nơi DUY NHẤT chọn bản Mock hay bản Real cho từng interface, dựa vào UIDebugConfig.
    /// UI chỉ gọi UIServices.Save / .Player / … và không bao giờ biết mình đang dùng bản nào.
    ///
    /// Vì sao không dùng VContainer như gameplay?
    /// GameLifetimeScope chỉ có trong scene gameplay và ném lỗi ngay nếu thiếu component cần tìm.
    /// Scene menu / loading không có Player, nên UI dùng lớp static nhỏ này để hoàn toàn độc lập.
    /// Provider được tạo MỘT lần và sống qua các scene, nên dữ liệu mock (nhân vật vừa tạo, đồ vừa mua)
    /// vẫn còn khi đi từ menu vào game.
    /// </summary>
    public static class UIServices
    {
        private static UIDebugConfig _config;
        private static ISaveProvider _save;
        private static ILoginService _login;
        private static IPlayerDataProvider _player;
        private static IInventoryProvider _inventory;
        private static IQuestProvider _quests;

        /// <summary>Config hiện tại. Nếu chưa scene nào gọi Init (ví dụ bấm Play thẳng ở LoadRandomMap) thì dùng giá trị mặc định.</summary>
        public static UIDebugConfig Config
        {
            get
            {
                if (_config == null) _config = ScriptableObject.CreateInstance<UIDebugConfig>();
                return _config;
            }
        }

        public static ISaveProvider Save => _save ??= Config.skipSaveLoad ? new MockSaveProvider() : new RealSaveProvider();

        public static ILoginService Login => _login ??= Config.skipLogin ? new MockLoginService() : new RealLoginService();

        public static IPlayerDataProvider Player => _player ??= Config.useMockData ? new MockPlayerDataProvider() : new RealPlayerDataProvider();

        public static IInventoryProvider Inventory => _inventory ??= Config.useMockData ? new MockInventoryProvider() : new RealInventoryProvider();

        public static IQuestProvider Quests => _quests ??= Config.useMockData ? new MockQuestProvider() : new RealQuestProvider();

        /// <summary>
        /// Gọi từ UIBootstrap ở đầu mỗi scene. Nếu là cùng một asset config thì giữ nguyên provider cũ;
        /// nếu đổi sang asset khác thì tạo lại toàn bộ.
        /// </summary>
        public static void Init(UIDebugConfig config)
        {
            if (config == null || config == _config) return;
            _config = config;
            ResetProviders();
        }

        /// <summary>Tạo lại mọi provider theo cờ hiện tại. Dùng khi đổi cờ lúc đang chạy (smoke test, debug).</summary>
        public static void RebuildProviders() => ResetProviders();

        /// <summary>Gọi khi đổi save để provider nhân vật đọc lại tên / class / level.</summary>
        public static void OnSaveSelected()
        {
            if (_player is MockPlayerDataProvider mock) mock.ResetForNewSave();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            _config = null;
            ResetProviders();
        }

        private static void ResetProviders()
        {
            // RealPlayerDataProvider có đăng ký EventManager → phải hủy đăng ký, không thì rò callback.
            if (_player is IDisposable disposable) disposable.Dispose();
            _save = null;
            _login = null;
            _player = null;
            _inventory = null;
            _quests = null;
        }
    }
}
