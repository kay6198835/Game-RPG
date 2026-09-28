using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIFlow
{
    /// <summary>
    /// Tạo nhân vật: chọn class, ngoại hình (tóc / da / màu áo), đặt tên, preview xoay 8 hướng, nút Ngẫu nhiên.
    /// Game là 2D góc nhìn từ trên → "xoay" = đổi sprite theo 8 hướng (quy ước DirectionResolver:
    /// 0 = dưới-trái, đi theo chiều kim đồng hồ). Chưa có sprite thì preview hiện khối màu + mũi tên hướng.
    /// </summary>
    public class CharacterCreationPanel : UIPanel
    {
        [Header("Class")]
        [SerializeField] private Transform classListContainer;
        [SerializeField] private Button classButtonTemplate;
        [SerializeField] private TMP_Text classDescriptionText;

        [Header("Ngoại hình (mỗi mục có nút < và >)")]
        [SerializeField] private Button hairPrevButton;
        [SerializeField] private Button hairNextButton;
        [SerializeField] private TMP_Text hairLabel;
        [SerializeField] private Button skinPrevButton;
        [SerializeField] private Button skinNextButton;
        [SerializeField] private TMP_Text skinLabel;
        [SerializeField] private Button colorPrevButton;
        [SerializeField] private Button colorNextButton;
        [SerializeField] private TMP_Text colorLabel;

        [Header("Preview")]
        [SerializeField] private Image previewBody;
        [SerializeField] private Image previewHair;
        [SerializeField] private RectTransform previewDirectionArrow;
        [SerializeField] private TMP_Text previewDirectionText;
        [SerializeField] private Button rotateLeftButton;
        [SerializeField] private Button rotateRightButton;
        [SerializeField] private Toggle autoRotateToggle;
        [SerializeField] private float autoRotateInterval = 0.6f;

        [Header("Tên + xác nhận")]
        [SerializeField] private TMP_InputField nameInput;
        [SerializeField] private TMP_Text errorText;
        [SerializeField] private Button randomButton;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button backButton;

        // Dữ liệu ngoại hình mẫu. TODO: nối logic thật — chuyển sang ScriptableObject khi có asset ngoại hình.
        private static readonly string[] HairStyles = { "Ngắn", "Dài", "Buộc", "Trọc", "Xù" };
        private static readonly Color[] HairColors = { new(0.25f, 0.15f, 0.1f), new(0.9f, 0.8f, 0.4f), new(0.1f, 0.1f, 0.1f), new(0.6f, 0.2f, 0.1f), new(0.85f, 0.85f, 0.9f) };
        private static readonly Color[] SkinTones = { new(1f, 0.87f, 0.75f), new(0.9f, 0.72f, 0.55f), new(0.7f, 0.5f, 0.35f), new(0.45f, 0.3f, 0.2f) };
        private static readonly Color[] OutfitColors = { Color.white, new(0.8f, 0.3f, 0.3f), new(0.3f, 0.5f, 0.9f), new(0.3f, 0.75f, 0.4f), new(0.6f, 0.4f, 0.8f) };
        private static readonly string[] RandomNames = { "Aria", "Bram", "Cyra", "Doran", "Elin", "Fenn", "Gwyn", "Hale", "Isla", "Joren" };
        private static readonly string[] DirectionNames = { "Dưới-trái", "Trái", "Trên-trái", "Lên", "Trên-phải", "Phải", "Dưới-phải", "Xuống" };

        private const int MinNameLength = 2;
        private const int MaxNameLength = 16;

        private readonly List<Button> _classButtons = new();
        private IReadOnlyList<CharacterClassInfo> _classes;
        private int _classIndex;
        private int _hairIndex;
        private int _skinIndex;
        private int _colorIndex;
        private int _direction = 7;   // Bắt đầu nhìn xuống (về phía người chơi)
        private float _rotateTimer;

        public event Action<SaveSlotData> CharacterCreated;
        public event Action BackClicked;

        protected override void Awake()
        {
            base.Awake();
            classButtonTemplate.gameObject.SetActive(false);
            nameInput.characterLimit = MaxNameLength;

            hairPrevButton.onClick.AddListener(() => { _hairIndex = Wrap(_hairIndex - 1, HairStyles.Length); RefreshPreview(); });
            hairNextButton.onClick.AddListener(() => { _hairIndex = Wrap(_hairIndex + 1, HairStyles.Length); RefreshPreview(); });
            skinPrevButton.onClick.AddListener(() => { _skinIndex = Wrap(_skinIndex - 1, SkinTones.Length); RefreshPreview(); });
            skinNextButton.onClick.AddListener(() => { _skinIndex = Wrap(_skinIndex + 1, SkinTones.Length); RefreshPreview(); });
            colorPrevButton.onClick.AddListener(() => { _colorIndex = Wrap(_colorIndex - 1, OutfitColors.Length); RefreshPreview(); });
            colorNextButton.onClick.AddListener(() => { _colorIndex = Wrap(_colorIndex + 1, OutfitColors.Length); RefreshPreview(); });
            rotateLeftButton.onClick.AddListener(() => Rotate(-1));
            rotateRightButton.onClick.AddListener(() => Rotate(1));
            randomButton.onClick.AddListener(Randomize);
            confirmButton.onClick.AddListener(Confirm);
            backButton.onClick.AddListener(() => BackClicked?.Invoke());
            nameInput.onValueChanged.AddListener(_ => errorText.text = string.Empty);
        }

        protected override void OnShown()
        {
            BuildClassList();
            errorText.text = string.Empty;
            if (string.IsNullOrEmpty(nameInput.text)) nameInput.text = RandomNames[UnityEngine.Random.Range(0, RandomNames.Length)];
            RefreshPreview();
        }

        private void Update()
        {
            if (!IsVisible || autoRotateToggle == null || !autoRotateToggle.isOn) return;
            _rotateTimer += Time.unscaledDeltaTime;
            if (_rotateTimer < autoRotateInterval) return;
            _rotateTimer = 0f;
            Rotate(1);
        }

        private void BuildClassList()
        {
            foreach (Button button in _classButtons) Destroy(button.gameObject);
            _classButtons.Clear();

            _classes = UIServices.Player.GetAvailableClasses();
            if (_classes.Count == 0)
            {
                // Bản Real chưa có dữ liệu class → báo rõ thay vì để màn hình trống không hiểu vì sao.
                classDescriptionText.text = "Chưa có class nào (bật useMockData để dùng class mẫu).";
                confirmButton.interactable = false;
                return;
            }

            confirmButton.interactable = true;
            for (int i = 0; i < _classes.Count; i++)
            {
                Button button = Instantiate(classButtonTemplate, classListContainer);
                button.gameObject.SetActive(true);
                button.GetComponentInChildren<TMP_Text>().text = _classes[i].displayName;
                int index = i;
                button.onClick.AddListener(() => SelectClass(index));
                _classButtons.Add(button);
            }
            SelectClass(Mathf.Clamp(_classIndex, 0, _classes.Count - 1));
        }

        private void SelectClass(int index)
        {
            _classIndex = index;
            for (int i = 0; i < _classButtons.Count; i++)
            {
                _classButtons[i].targetGraphic.color = i == index ? _classes[i].tint : new Color(1f, 1f, 1f, 0.15f);
            }
            classDescriptionText.text = _classes[index].description;
            RefreshPreview();
        }

        private void Rotate(int step)
        {
            _direction = Wrap(_direction + step, 8);
            RefreshPreview();
        }

        private void Randomize()
        {
            if (_classes != null && _classes.Count > 0) SelectClass(UnityEngine.Random.Range(0, _classes.Count));
            _hairIndex = UnityEngine.Random.Range(0, HairStyles.Length);
            _skinIndex = UnityEngine.Random.Range(0, SkinTones.Length);
            _colorIndex = UnityEngine.Random.Range(0, OutfitColors.Length);
            _direction = UnityEngine.Random.Range(0, 8);
            nameInput.text = RandomNames[UnityEngine.Random.Range(0, RandomNames.Length)];
            RefreshPreview();
        }

        private void RefreshPreview()
        {
            hairLabel.text = HairStyles[_hairIndex];
            skinLabel.text = $"Tông {_skinIndex + 1}";
            colorLabel.text = $"Màu {_colorIndex + 1}";

            CharacterClassInfo info = _classes != null && _classes.Count > 0 ? _classes[_classIndex] : null;
            Sprite sprite = info != null && info.directionSprites != null && info.directionSprites.Length == 8
                ? info.directionSprites[_direction]
                : null;

            previewBody.sprite = sprite;
            // Có sprite: tô màu da. Không có sprite: trộn màu class với màu áo để thấy rõ khác biệt.
            previewBody.color = sprite != null
                ? SkinTones[_skinIndex]
                : (info != null ? info.tint : Color.gray) * OutfitColors[_colorIndex];
            previewHair.color = HairColors[_hairIndex];

            // Mũi tên chỉ hướng nhìn: hướng 0 (dưới-trái) = 225°, mỗi bước quay 45° theo chiều kim đồng hồ.
            float angle = 225f - _direction * 45f;
            if (previewDirectionArrow != null) previewDirectionArrow.localEulerAngles = new Vector3(0f, 0f, angle - 90f);
            previewDirectionText.text = $"Hướng {_direction + 1}/8 · {DirectionNames[_direction]}";
        }

        private void Confirm()
        {
            string characterName = nameInput.text.Trim();
            if (characterName.Length < MinNameLength)
            {
                errorText.text = $"Tên cần ít nhất {MinNameLength} ký tự.";
                return;
            }
            if (_classes == null || _classes.Count == 0) return;

            NewCharacterRequest request = new()
            {
                characterName = characterName,
                classId = _classes[_classIndex].displayName,
                hairIndex = _hairIndex,
                skinIndex = _skinIndex,
                colorIndex = _colorIndex,
            };
            SaveSlotData save = UIServices.Save.CreateNewSave(request);
            UIServices.OnSaveSelected();
            CharacterCreated?.Invoke(save);
        }

        private static int Wrap(int value, int count) => (value % count + count) % count;
    }
}
