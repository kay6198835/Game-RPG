namespace UIFlow
{
    /// <summary>
    /// Tên scene dùng trong luồng UI. Đặt ở một chỗ để không gõ sai chuỗi rải rác khắp code.
    /// Scene phải có trong File > Build Settings thì SceneManager mới load được.
    /// </summary>
    public static class SceneNames
    {
        public const string MainGamePlay = "MainGamePlay";   // Scene menu (logo, đăng nhập, menu chính…)
        public const string Loading = "Loading";             // Màn hình loading dùng chung
        public const string GameplayReal = "LoadRandomMap";  // Scene dungeon thật (không sửa)
        public const string GameplayMock = "GameplayMock";   // Scene giả để test HUD
        public const string GameplayUI = "GameplayUI";       // Scene chỉ chứa UI in-game, load kiểu Additive
    }
}
