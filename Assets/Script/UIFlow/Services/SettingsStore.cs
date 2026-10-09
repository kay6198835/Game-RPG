using UnityEngine;

namespace UIFlow
{
    /// <summary>
    /// Cài đặt âm thanh + đồ họa, lưu bằng PlayerPrefs.
    /// Cài đặt không dính gameplay, PlayerPrefs chạy được ở mọi scene.
    /// </summary>
    public static class SettingsStore
    {
        private const string KeyMaster = "ui.volume.master";
        private const string KeyMusic = "ui.volume.music";
        private const string KeySfx = "ui.volume.sfx";
        private const string KeyQuality = "ui.gfx.quality";
        private const string KeyFullscreen = "ui.gfx.fullscreen";
        private const string KeyVSync = "ui.gfx.vsync";
        private const string KeyResWidth = "ui.gfx.resWidth";
        private const string KeyResHeight = "ui.gfx.resHeight";

        public static float MasterVolume
        {
            get => PlayerPrefs.GetFloat(KeyMaster, 1f);
            set { PlayerPrefs.SetFloat(KeyMaster, value); AudioListener.volume = value; }
        }

        // TODO: nối logic thật — project chưa có AudioMixer; khi có, gán 2 giá trị này vào mixer group.
        public static float MusicVolume
        {
            get => PlayerPrefs.GetFloat(KeyMusic, 0.8f);
            set => PlayerPrefs.SetFloat(KeyMusic, value);
        }

        public static float SfxVolume
        {
            get => PlayerPrefs.GetFloat(KeySfx, 0.8f);
            set => PlayerPrefs.SetFloat(KeySfx, value);
        }

        public static int QualityLevel
        {
            get => PlayerPrefs.GetInt(KeyQuality, QualitySettings.GetQualityLevel());
            set { PlayerPrefs.SetInt(KeyQuality, value); QualitySettings.SetQualityLevel(value, true); }
        }

        public static bool Fullscreen
        {
            get => PlayerPrefs.GetInt(KeyFullscreen, Screen.fullScreen ? 1 : 0) == 1;
            set { PlayerPrefs.SetInt(KeyFullscreen, value ? 1 : 0); Screen.fullScreen = value; }
        }

        public static bool VSync
        {
            get => PlayerPrefs.GetInt(KeyVSync, QualitySettings.vSyncCount > 0 ? 1 : 0) == 1;
            set { PlayerPrefs.SetInt(KeyVSync, value ? 1 : 0); QualitySettings.vSyncCount = value ? 1 : 0; }
        }

        public static Vector2Int Resolution
        {
            get => new(PlayerPrefs.GetInt(KeyResWidth, Screen.width), PlayerPrefs.GetInt(KeyResHeight, Screen.height));
            set
            {
                PlayerPrefs.SetInt(KeyResWidth, value.x);
                PlayerPrefs.SetInt(KeyResHeight, value.y);
                Screen.SetResolution(value.x, value.y, Fullscreen);
            }
        }

        /// <summary>Gọi khi game khởi động để áp cài đặt đã lưu.</summary>
        public static void ApplySaved()
        {
            AudioListener.volume = MasterVolume;
            QualitySettings.SetQualityLevel(Mathf.Clamp(QualityLevel, 0, QualitySettings.names.Length - 1), true);
            QualitySettings.vSyncCount = VSync ? 1 : 0;
            // Trong Editor, Screen.SetResolution không có tác dụng → chỉ áp ở bản build.
            if (!Application.isEditor) Screen.SetResolution(Resolution.x, Resolution.y, Fullscreen);
        }

        public static void Save() => PlayerPrefs.Save();
    }
}
