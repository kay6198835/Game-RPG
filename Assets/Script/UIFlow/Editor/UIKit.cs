using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace UIFlow.EditorTools
{
    /// <summary>
    /// Bộ hàm nhỏ để dựng uGUI bằng code trong Editor. Chỉ chạy trong Editor (thư mục Editor/),
    /// không vào bản build. Mỗi hàm tương đương một thao tác chuột trong Hierarchy / Inspector.
    /// </summary>
    public static class UIKit
    {
        public const string WhiteSpritePath = "Assets/Sprite/UIFlow/UIWhite.png";

        public static readonly Color PanelColor = new(0.08f, 0.09f, 0.13f, 0.96f);
        public static readonly Color WindowColor = new(0.13f, 0.14f, 0.2f, 0.98f);
        public static readonly Color DimColor = new(0f, 0f, 0f, 0.6f);
        public static readonly Color ButtonColor = new(1f, 1f, 1f, 0.15f);
        public static readonly Color AccentColor = new(0.95f, 0.7f, 0.2f);
        public static readonly Color TextColor = new(0.93f, 0.93f, 0.95f);
        public static readonly Color MutedColor = new(0.65f, 0.67f, 0.72f);

        private static Sprite _white;
        private static DefaultControls.Resources _resources;

        /// <summary>Sprite trắng 4x4. Image kiểu Filled (thanh máu, cooldown) BẮT BUỘC có sprite, để null sẽ không fill.</summary>
        public static Sprite White
        {
            get
            {
                if (_white != null) return _white;
                _white = AssetDatabase.LoadAssetAtPath<Sprite>(WhiteSpritePath);
                if (_white != null) return _white;

                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(WhiteSpritePath));
                Texture2D texture = new(4, 4);
                Color[] pixels = new Color[16];
                for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
                texture.SetPixels(pixels);
                File.WriteAllBytes(WhiteSpritePath, texture.EncodeToPNG());
                Object.DestroyImmediate(texture);
                AssetDatabase.ImportAsset(WhiteSpritePath);

                TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(WhiteSpritePath);
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = 4;   // 4 px = 1 unit → sprite 1x1 unit, tiện làm khối vuông trong scene mock
                importer.filterMode = FilterMode.Point;
                importer.SaveAndReimport();
                _white = AssetDatabase.LoadAssetAtPath<Sprite>(WhiteSpritePath);
                return _white;
            }
        }

        public static Sprite Rounded => AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

        public static DefaultControls.Resources UIResources
        {
            get
            {
                if (_resources.standard != null) return _resources;
                _resources = new DefaultControls.Resources
                {
                    standard = Rounded,
                    background = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd"),
                    inputField = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/InputFieldBackground.psd"),
                    knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd"),
                    checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Checkmark.psd"),
                    dropdown = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/DropdownArrow.psd"),
                    mask = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UIMask.psd"),
                };
                return _resources;
            }
        }

        private static TMP_DefaultControls.Resources TmpResources => new()
        {
            standard = UIResources.standard,
            background = UIResources.background,
            inputField = UIResources.inputField,
            knob = UIResources.knob,
            checkmark = UIResources.checkmark,
            dropdown = UIResources.dropdown,
            mask = UIResources.mask,
        };

        // ───────────────────────── Scene-level ─────────────────────────

        /// <summary>
        /// Canvas Screen Space - Overlay + Canvas Scaler "Scale With Screen Size" 1920x1080.
        /// Match = 0.5: màn hình rộng hơn hay cao hơn 16:9 đều co giãn cân bằng, UI không bị kéo méo.
        /// </summary>
        public static Canvas ScreenCanvas(string name, int sortingOrder)
        {
            GameObject go = new(name, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            go.layer = LayerMask.NameToLayer("UI");
            Canvas canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;

            CanvasScaler scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            return canvas;
        }

        /// <summary>EventSystem dùng InputSystemUIInputModule vì project chạy Input System mới.</summary>
        public static void EventSystemObject()
        {
            GameObject go = new("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            // Không gán action ở đây: khi trống, InputSystemUIInputModule.OnEnable() tự dùng DefaultInputActions lúc chạy.
        }

        public static Camera CameraObject(Color background, float orthographicSize)
        {
            GameObject go = new("Main Camera", typeof(Camera), typeof(AudioListener));
            go.tag = "MainCamera";
            go.transform.position = new Vector3(0f, 0f, -10f);
            Camera camera = go.GetComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = orthographicSize;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = background;
            return camera;
        }

        // ───────────────────────── RectTransform ─────────────────────────

        public static GameObject Node(string name, Transform parent)
        {
            GameObject go = new(name, typeof(RectTransform));
            go.layer = LayerMask.NameToLayer("UI");
            go.transform.SetParent(parent, false);
            return go;
        }

        public static RectTransform Rect(Component component) => (RectTransform)component.transform;

        public static RectTransform Rect(GameObject go) => (RectTransform)go.transform;

        /// <summary>Đặt anchor + pivot + vị trí + kích thước một lần. Anchor quyết định UI "bám" vào góc/cạnh nào khi đổi độ phân giải.</summary>
        public static RectTransform Place(GameObject go, Vector2 anchor, Vector2 pivot, Vector2 position, Vector2 size)
        {
            RectTransform rt = Rect(go);
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.pivot = pivot;
            rt.anchoredPosition = position;
            rt.sizeDelta = size;
            return rt;
        }

        /// <summary>Kéo giãn phủ kín cha, chừa lề (trái, phải, trên, dưới).</summary>
        public static RectTransform Stretch(GameObject go, float left = 0, float right = 0, float top = 0, float bottom = 0)
        {
            RectTransform rt = Rect(go);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.offsetMin = new Vector2(left, bottom);
            rt.offsetMax = new Vector2(-right, -top);
            return rt;
        }

        public static readonly Vector2 Center = new(0.5f, 0.5f);
        public static readonly Vector2 TopLeft = new(0f, 1f);
        public static readonly Vector2 TopRight = new(1f, 1f);
        public static readonly Vector2 TopCenter = new(0.5f, 1f);
        public static readonly Vector2 BottomCenter = new(0.5f, 0f);
        public static readonly Vector2 BottomLeft = new(0f, 0f);
        public static readonly Vector2 MiddleLeft = new(0f, 0.5f);
        public static readonly Vector2 MiddleRight = new(1f, 0.5f);

        // ───────────────────────── Widgets ─────────────────────────

        public static Image Img(Transform parent, string name, Color color, Sprite sprite = null, bool raycast = false)
        {
            GameObject go = Node(name, parent);
            Image image = go.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.raycastTarget = raycast;
            if (sprite == Rounded) image.type = Image.Type.Sliced;
            return image;
        }

        /// <summary>Image kiểu Filled — dùng cho thanh máu (Horizontal) và cooldown (Radial360).</summary>
        public static Image Filled(Transform parent, string name, Color color, Image.FillMethod method)
        {
            Image image = Img(parent, name, color, White);
            image.type = Image.Type.Filled;
            image.fillMethod = method;
            image.fillOrigin = method == Image.FillMethod.Radial360 ? (int)Image.Origin360.Top : (int)Image.OriginHorizontal.Left;
            image.fillClockwise = false;
            image.fillAmount = 1f;
            return image;
        }

        public static TextMeshProUGUI Txt(Transform parent, string name, string text, float size,
                                          TextAlignmentOptions align = TextAlignmentOptions.Left, Color? color = null)
        {
            GameObject go = Node(name, parent);
            TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = size;
            tmp.alignment = align;
            tmp.color = color ?? TextColor;
            tmp.raycastTarget = false;   // Chữ không cần bắt click → đỡ tốn raycast
            tmp.enableWordWrapping = true;
            return tmp;
        }

        public static Button Btn(Transform parent, string name, string label, float fontSize = 28, Color? color = null)
        {
            Image image = Img(parent, name, color ?? ButtonColor, Rounded, raycast: true);
            Button button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            ColorBlock colors = button.colors;
            colors.highlightedColor = new Color(1.25f, 1.25f, 1.25f);
            colors.pressedColor = new Color(0.8f, 0.8f, 0.8f);
            colors.selectedColor = new Color(1.15f, 1.15f, 1.15f);
            colors.colorMultiplier = 1.5f;
            button.colors = colors;

            TextMeshProUGUI text = Txt(image.transform, "Label", label, fontSize, TextAlignmentOptions.Center);
            Stretch(text.gameObject, 8, 8, 4, 4);
            return button;
        }

        public static Toggle Tgl(Transform parent, string name, string label, float fontSize = 26)
        {
            GameObject go = DefaultControls.CreateToggle(UIResources);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.layer = LayerMask.NameToLayer("UI");

            // Toggle mặc định dùng Text cũ → xóa, thay bằng TextMeshPro theo quy ước project.
            Object.DestroyImmediate(go.transform.Find("Label").gameObject);
            RectTransform background = (RectTransform)go.transform.Find("Background");
            background.anchorMin = background.anchorMax = new Vector2(0f, 0.5f);
            background.pivot = new Vector2(0f, 0.5f);
            background.anchoredPosition = Vector2.zero;
            background.sizeDelta = new Vector2(34, 34);
            RectTransform check = (RectTransform)background.Find("Checkmark");
            check.sizeDelta = new Vector2(30, 30);

            TextMeshProUGUI text = Txt(go.transform, "Label", label, fontSize);
            Stretch(text.gameObject, 46, 0, 0, 0);
            text.alignment = TextAlignmentOptions.MidlineLeft;
            Rect(go).sizeDelta = new Vector2(300, 40);
            return go.GetComponent<Toggle>();
        }

        public static Slider Sld(Transform parent, string name)
        {
            GameObject go = DefaultControls.CreateSlider(UIResources);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.layer = LayerMask.NameToLayer("UI");
            Rect(go).sizeDelta = new Vector2(420, 28);
            return go.GetComponent<Slider>();
        }

        public static TMP_InputField Input(Transform parent, string name, string placeholder, float fontSize = 26)
        {
            GameObject go = TMP_DefaultControls.CreateInputField(TmpResources);
            go.name = name;
            go.transform.SetParent(parent, false);
            SetLayerRecursive(go);
            TMP_InputField input = go.GetComponent<TMP_InputField>();
            input.pointSize = fontSize;
            ((TMP_Text)input.placeholder).text = placeholder;
            Rect(go).sizeDelta = new Vector2(520, 56);
            return input;
        }

        public static TMP_Dropdown Drop(Transform parent, string name, float fontSize = 24)
        {
            GameObject go = TMP_DefaultControls.CreateDropdown(TmpResources);
            go.name = name;
            go.transform.SetParent(parent, false);
            SetLayerRecursive(go);
            TMP_Dropdown dropdown = go.GetComponent<TMP_Dropdown>();
            dropdown.captionText.fontSize = fontSize;
            dropdown.itemText.fontSize = fontSize;
            Rect(go).sizeDelta = new Vector2(420, 48);

            // Mặc định mỗi dòng cao 20 px — quá nhỏ ở 1080p → nới ra.
            RectTransform template = (RectTransform)go.transform.Find("Template");
            template.sizeDelta = new Vector2(0, 300);
            RectTransform content = (RectTransform)template.Find("Viewport/Content");
            content.sizeDelta = new Vector2(0, 44);
            RectTransform item = (RectTransform)content.Find("Item");
            item.sizeDelta = new Vector2(0, 44);
            return dropdown;
        }

        /// <summary>Danh sách cuộn dọc. Trả về "Content": thêm con vào đây, VerticalLayoutGroup tự xếp.</summary>
        public static Transform ScrollList(Transform parent, string name, float spacing = 8, float scrollSensitivity = 30)
        {
            GameObject root = Node(name, parent);
            Image background = root.AddComponent<Image>();
            background.color = new Color(0f, 0f, 0f, 0.2f);
            ScrollRect scroll = root.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.scrollSensitivity = scrollSensitivity;
            scroll.movementType = ScrollRect.MovementType.Clamped;

            GameObject viewport = Node("Viewport", root.transform);
            Stretch(viewport, 4, 4, 4, 4);
            viewport.AddComponent<RectMask2D>();   // RectMask2D: cắt phần thừa, rẻ hơn Mask vì không cần stencil

            GameObject content = Node("Content", viewport.transform);
            RectTransform contentRect = Rect(content);
            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.sizeDelta = Vector2.zero;
            VLayout(content, spacing, new RectOffset(4, 4, 4, 4));
            ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;   // Content cao theo số dòng → thanh cuộn đúng

            scroll.viewport = Rect(viewport);
            scroll.content = contentRect;
            return content.transform;
        }

        // ───────────────────────── Layout ─────────────────────────

        public static VerticalLayoutGroup VLayout(GameObject go, float spacing, RectOffset padding = null,
                                                  TextAnchor alignment = TextAnchor.UpperCenter, bool expandWidth = true)
        {
            VerticalLayoutGroup layout = go.AddComponent<VerticalLayoutGroup>();
            layout.spacing = spacing;
            layout.padding = padding ?? new RectOffset();
            layout.childAlignment = alignment;
            layout.childControlWidth = expandWidth;
            layout.childForceExpandWidth = expandWidth;
            layout.childControlHeight = true;
            layout.childForceExpandHeight = false;
            return layout;
        }

        public static HorizontalLayoutGroup HLayout(GameObject go, float spacing, RectOffset padding = null,
                                                    TextAnchor alignment = TextAnchor.MiddleCenter, bool controlSize = true)
        {
            HorizontalLayoutGroup layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = spacing;
            layout.padding = padding ?? new RectOffset();
            layout.childAlignment = alignment;
            layout.childControlWidth = controlSize;
            layout.childControlHeight = controlSize;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            return layout;
        }

        public static LayoutElement Size(Component component, float preferredWidth = -1, float preferredHeight = -1, float flexibleWidth = -1)
        {
            // Không dùng "??" với UnityEngine.Object: trong Editor, GetComponent trả về object "giả null" nên ?? không bắt được.
            if (!component.TryGetComponent(out LayoutElement element)) element = component.gameObject.AddComponent<LayoutElement>();
            element.preferredWidth = preferredWidth;
            element.preferredHeight = preferredHeight;
            element.flexibleWidth = flexibleWidth;
            if (preferredHeight >= 0) element.minHeight = preferredHeight;
            return element;
        }

        // ───────────────────────── Panels ─────────────────────────

        /// <summary>
        /// Tạo panel phủ toàn màn hình: CanvasGroup (để UIPanel ẩn/hiện) + component panel + nền.
        /// Cài luôn các cờ closeOnEscape / hidePrevious / pausesGame của UIPanel.
        /// </summary>
        public static T Panel<T>(Transform canvas, string name, Color background, bool closeOnEscape = true,
                                 bool hidePrevious = true, bool pausesGame = false) where T : UIPanel
        {
            GameObject go = Node(name, canvas);
            Stretch(go);
            go.AddComponent<CanvasGroup>();
            if (background.a > 0f)
            {
                Image image = go.AddComponent<Image>();
                image.color = background;
                image.raycastTarget = true;   // Nền chặn click xuyên xuống panel bên dưới
            }
            T panel = go.AddComponent<T>();
            Wire(panel, ("closeOnEscape", closeOnEscape), ("hidePrevious", hidePrevious), ("pausesGame", pausesGame));
            return panel;
        }

        /// <summary>Khung cửa sổ ở giữa màn hình, có tiêu đề.</summary>
        public static RectTransform Window(Transform parent, string name, Vector2 size, string title)
        {
            Image frame = Img(parent, name, WindowColor, Rounded, raycast: true);
            RectTransform rt = Place(frame.gameObject, Center, Center, Vector2.zero, size);
            if (!string.IsNullOrEmpty(title))
            {
                TextMeshProUGUI titleText = Txt(frame.transform, "Title", title, 40, TextAlignmentOptions.Center, AccentColor);
                Place(titleText.gameObject, TopCenter, TopCenter, new Vector2(0, -20), new Vector2(size.x - 40, 56));
            }
            return rt;
        }

        // ───────────────────────── Serialized fields ─────────────────────────

        /// <summary>
        /// Gán field [SerializeField] private bằng SerializedObject — giống kéo thả trong Inspector.
        /// Sai tên field → ném lỗi ngay để phát hiện sớm, thay vì âm thầm để trống.
        /// </summary>
        public static void Wire(Object target, params (string field, object value)[] values)
        {
            SerializedObject serialized = new(target);
            foreach ((string field, object value) in values)
            {
                SerializedProperty property = serialized.FindProperty(field);
                if (property == null) throw new System.ArgumentException($"{target.GetType().Name} không có field \"{field}\"");

                switch (value)
                {
                    case bool b: property.boolValue = b; break;
                    case float f: property.floatValue = f; break;
                    case int i: property.intValue = i; break;
                    case string s: property.stringValue = s; break;
                    case Object[] array:
                        property.arraySize = array.Length;
                        for (int i = 0; i < array.Length; i++) property.GetArrayElementAtIndex(i).objectReferenceValue = array[i];
                        break;
                    case Object reference: property.objectReferenceValue = reference; break;
                    case null: property.objectReferenceValue = null; break;
                    default: throw new System.ArgumentException($"Kiểu {value.GetType().Name} chưa được hỗ trợ ({field})");
                }
            }
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void SetLayerRecursive(GameObject go)
        {
            int layer = LayerMask.NameToLayer("UI");
            foreach (Transform child in go.GetComponentsInChildren<Transform>(true)) child.gameObject.layer = layer;
        }
    }
}
