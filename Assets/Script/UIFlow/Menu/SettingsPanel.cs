using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIFlow
{
    /// <summary>
    /// Cài đặt gồm 3 trang: Âm thanh, Đồ họa, Gán phím. Dùng chung cho menu chính và menu tạm dừng
    /// (mỗi scene có một bản). Giá trị được áp ngay khi kéo/bấm và lưu khi đóng panel.
    /// </summary>
    public class SettingsPanel : UIPanel
    {
        [Header("Tab")]
        [SerializeField] private Button[] tabButtons;
        [SerializeField] private GameObject[] pages;

        [Header("Âm thanh")]
        [SerializeField] private Slider masterSlider;
        [SerializeField] private TMP_Text masterValue;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private TMP_Text musicValue;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private TMP_Text sfxValue;

        [Header("Đồ họa")]
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private TMP_Dropdown qualityDropdown;
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private Toggle vsyncToggle;

        [Header("Gán phím")]
        [SerializeField] private Transform keyListContainer;
        [SerializeField] private KeyBindingRow keyRowTemplate;
        [SerializeField] private Button resetKeysButton;
        [SerializeField] private TMP_Text keyMessageText;

        [SerializeField] private Button backButton;

        private readonly List<KeyBindingRow> _keyRows = new();
        private readonly List<Vector2Int> _resolutions = new();
        private bool _refreshing;   // Đang nạp giá trị vào control → bỏ qua sự kiện onValueChanged

        protected override void Awake()
        {
            base.Awake();
            keyRowTemplate.gameObject.SetActive(false);

            for (int i = 0; i < tabButtons.Length; i++)
            {
                int index = i;
                tabButtons[i].onClick.AddListener(() => ShowPage(index));
            }

            masterSlider.onValueChanged.AddListener(v => { if (!_refreshing) SettingsStore.MasterVolume = v; masterValue.text = Percent(v); });
            musicSlider.onValueChanged.AddListener(v => { if (!_refreshing) SettingsStore.MusicVolume = v; musicValue.text = Percent(v); });
            sfxSlider.onValueChanged.AddListener(v => { if (!_refreshing) SettingsStore.SfxVolume = v; sfxValue.text = Percent(v); });

            resolutionDropdown.onValueChanged.AddListener(i => { if (!_refreshing) SettingsStore.Resolution = _resolutions[i]; });
            qualityDropdown.onValueChanged.AddListener(i => { if (!_refreshing) SettingsStore.QualityLevel = i; });
            fullscreenToggle.onValueChanged.AddListener(v => { if (!_refreshing) SettingsStore.Fullscreen = v; });
            vsyncToggle.onValueChanged.AddListener(v => { if (!_refreshing) SettingsStore.VSync = v; });

            resetKeysButton.onClick.AddListener(() =>
            {
                KeyBindings.ResetAll();
                RefreshKeyRows();
                keyMessageText.text = "Đã khôi phục phím mặc định.";
            });
            backButton.onClick.AddListener(CloseSelf);

            BuildKeyRows();
        }

        protected override void OnShown()
        {
            _refreshing = true;

            masterSlider.value = SettingsStore.MasterVolume;
            musicSlider.value = SettingsStore.MusicVolume;
            sfxSlider.value = SettingsStore.SfxVolume;
            masterValue.text = Percent(masterSlider.value);
            musicValue.text = Percent(musicSlider.value);
            sfxValue.text = Percent(sfxSlider.value);

            BuildResolutionOptions();
            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
            qualityDropdown.SetValueWithoutNotify(Mathf.Clamp(SettingsStore.QualityLevel, 0, QualitySettings.names.Length - 1));
            fullscreenToggle.SetIsOnWithoutNotify(SettingsStore.Fullscreen);
            vsyncToggle.SetIsOnWithoutNotify(SettingsStore.VSync);

            _refreshing = false;

            keyMessageText.text = string.Empty;
            RefreshKeyRows();
            ShowPage(0);
        }

        protected override void OnHidden()
        {
            SettingsStore.Save();
        }

        private void ShowPage(int index)
        {
            for (int i = 0; i < pages.Length; i++) pages[i].SetActive(i == index);
            for (int i = 0; i < tabButtons.Length; i++)
            {
                tabButtons[i].targetGraphic.color = i == index ? new Color(0.95f, 0.7f, 0.2f) : new Color(1f, 1f, 1f, 0.15f);
            }
        }

        private void BuildResolutionOptions()
        {
            _resolutions.Clear();
            List<string> labels = new();
            foreach (Resolution resolution in Screen.resolutions)
            {
                Vector2Int size = new(resolution.width, resolution.height);
                if (_resolutions.Contains(size)) continue;   // Cùng kích thước khác tần số quét → gộp làm một
                _resolutions.Add(size);
                labels.Add($"{size.x} x {size.y}");
            }
            if (_resolutions.Count == 0)
            {
                _resolutions.Add(new Vector2Int(1920, 1080));
                labels.Add("1920 x 1080");
            }

            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(labels);
            int current = _resolutions.IndexOf(SettingsStore.Resolution);
            resolutionDropdown.SetValueWithoutNotify(current >= 0 ? current : _resolutions.Count - 1);
        }

        private void BuildKeyRows()
        {
            foreach (KeyBindings.Binding binding in KeyBindings.All)
            {
                KeyBindingRow row = Instantiate(keyRowTemplate, keyListContainer);
                row.gameObject.SetActive(true);
                row.Bind(binding, message =>
                {
                    keyMessageText.text = message;
                    RefreshKeyRows();
                });
                _keyRows.Add(row);
            }
        }

        private void RefreshKeyRows()
        {
            foreach (KeyBindingRow row in _keyRows) row.Refresh();
        }

        private static string Percent(float value) => Mathf.RoundToInt(value * 100f) + "%";
    }
}
