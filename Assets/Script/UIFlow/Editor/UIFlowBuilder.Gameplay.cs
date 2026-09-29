using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UIFlow.EditorTools.UIKit;

namespace UIFlow.EditorTools
{
    /// <summary>Phần dựng scene GameplayUI (HUD + cửa sổ in-game), GameplayMock và 2 prefab world-space.</summary>
    public static partial class UIFlowBuilder
    {
        // ═════════════════════════ Prefab world-space ═════════════════════════

        private static WorldHealthBar BuildHealthBarPrefab()
        {
            GameObject root = new("WorldHealthBar", typeof(RectTransform), typeof(Canvas), typeof(CanvasGroup));
            Canvas canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;   // Canvas là vật trong thế giới, đi theo quái
            canvas.sortingOrder = 50;                     // Vẽ trên sprite quái (sortingOrder 0)
            RectTransform rect = Rect(root);
            rect.sizeDelta = new Vector2(120, 16);
            rect.localScale = Vector3.one * 0.01f;         // 120 px × 0.01 = 1.2 unit: vừa bằng thân quái

            Image background = Img(root.transform, "Background", new Color(0f, 0f, 0f, 0.7f), White);
            Stretch(background.gameObject);
            Image trail = Filled(root.transform, "Trail", new Color(1f, 0.85f, 0.6f), Image.FillMethod.Horizontal);
            Stretch(trail.gameObject, 2, 2, 2, 2);
            Image fill = Filled(root.transform, "Fill", new Color(0.9f, 0.2f, 0.2f), Image.FillMethod.Horizontal);
            Stretch(fill.gameObject, 2, 2, 2, 2);

            WorldHealthBar bar = root.AddComponent<WorldHealthBar>();
            Wire(bar, ("fill", fill), ("trail", trail));
            return SavePrefab(root, HealthBarPrefabPath).GetComponent<WorldHealthBar>();
        }

        private static DamageNumber BuildDamageNumberPrefab()
        {
            GameObject root = new("DamageNumber");
            TextMeshPro text = root.AddComponent<TextMeshPro>();
            text.font = TMP_Settings.defaultFontAsset;
            text.fontSize = 6;
            text.fontStyle = FontStyles.Bold;
            text.alignment = TextAlignmentOptions.Center;
            text.enableWordWrapping = false;
            text.text = "99";
            text.rectTransform.sizeDelta = new Vector2(3f, 1f);
            text.GetComponent<MeshRenderer>().sortingOrder = 100;   // Trên thanh máu (50) và sprite
            root.AddComponent<DamageNumber>();
            return SavePrefab(root, DamageNumberPrefabPath).GetComponent<DamageNumber>();
        }

        private static GameObject SavePrefab(GameObject root, string path)
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            return prefab;
        }

        // ═════════════════════════ GameplayUI ═════════════════════════

        private static void BuildGameplayUI(UIDebugConfig config, DamageNumber damageNumberPrefab)
        {
            // Scene này KHÔNG có Camera/EventSystem: nó được load Additive chồng lên scene gameplay (thật hoặc mock),
            // dùng luôn camera và EventSystem của scene đó.
            Scene scene = NewScene();
            Bootstrap(config, applySavedSettings: false);

            Canvas canvas = ScreenCanvas("GameplayCanvas", 10);   // sortingOrder 10: vẽ trên Canvas cũ của LoadRandomMap
            UIManager manager = canvas.gameObject.AddComponent<UIManager>();
            GameplayUIController controller = canvas.gameObject.AddComponent<GameplayUIController>();

            HUDPanel hud = BuildHud(canvas.transform);
            TabWindow tabWindow = BuildTabWindow(canvas.transform);
            DialoguePanel dialogue = BuildDialogue(canvas.transform);
            ShopPanel shop = BuildShop(canvas.transform);
            SettingsPanel settings = BuildSettings(canvas.transform);
            PausePanel pause = BuildPause(canvas.transform, settings);
            GameOverPanel gameOver = BuildGameOver(canvas.transform);
            settings.transform.SetAsLastSibling();

            Wire(manager, ("rootPanel", hud), ("escapeFallbackPanel", pause), ("controlsTimeScale", true));
            Wire(controller, ("uiManager", manager), ("hud", hud), ("tabWindow", tabWindow), ("dialoguePanel", dialogue),
                 ("shopPanel", shop), ("gameOverPanel", gameOver));

            GameObject numbers = new("DamageNumbers");
            DamageNumberSpawner spawner = numbers.AddComponent<DamageNumberSpawner>();
            Wire(spawner, ("prefab", AssetDatabase.LoadAssetAtPath<DamageNumber>(DamageNumberPrefabPath)));

            Save(scene, GameplayUIPath);
        }

        private static HUDPanel BuildHud(Transform canvas)
        {
            HUDPanel hud = Panel<HUDPanel>(canvas, "HUDPanel", new Color(0, 0, 0, 0), closeOnEscape: false, hidePrevious: false);

            // ── Góc trên-trái: level, tên, máu, mana, exp
            GameObject status = Node("StatusTopLeft", hud.transform);
            Place(status, TopLeft, TopLeft, new Vector2(30, -30), new Vector2(600, 170));
            Image levelBadge = Img(status.transform, "LevelBadge", new Color(0.1f, 0.1f, 0.15f, 0.9f), UIResources.knob);
            Place(levelBadge.gameObject, TopLeft, TopLeft, Vector2.zero, new Vector2(120, 120));
            TextMeshProUGUI level = Txt(levelBadge.transform, "Level", "1", 48, TextAlignmentOptions.Center, AccentColor);
            Stretch(level.gameObject);
            TextMeshProUGUI characterName = Txt(status.transform, "Name", "", 30);
            Place(characterName.gameObject, TopLeft, TopLeft, new Vector2(140, 0), new Vector2(440, 42));
            StatBar hp = Bar(status.transform, "HPBar", new Vector2(140, -48), new Vector2(440, 30), new Color(0.85f, 0.2f, 0.22f), true);
            StatBar mana = Bar(status.transform, "ManaBar", new Vector2(140, -86), new Vector2(440, 22), new Color(0.25f, 0.45f, 0.95f), true);
            StatBar exp = Bar(status.transform, "ExpBar", new Vector2(140, -116), new Vector2(440, 10), new Color(0.6f, 0.85f, 0.3f), false);

            // ── Góc trên-phải: minimap (khung giữ chỗ)
            Image minimap = Img(hud.transform, "Minimap", new Color(0f, 0f, 0f, 0.55f), Rounded);
            Place(minimap.gameObject, TopRight, TopRight, new Vector2(-30, -30), new Vector2(280, 280));
            TextMeshProUGUI minimapLabel = Txt(minimap.transform, "Label", "MINIMAP\n<size=60%>TODO: nối logic thật\n(MapGridController)</size>", 26, TextAlignmentOptions.Center, MutedColor);
            Stretch(minimapLabel.gameObject, 10, 10, 10, 10);

            // ── Bên phải: theo dõi nhiệm vụ
            Image questFrame = Img(hud.transform, "QuestTracker", new Color(0f, 0f, 0f, 0.35f), Rounded);
            Place(questFrame.gameObject, TopRight, TopRight, new Vector2(-30, -340), new Vector2(420, 360));
            TextMeshProUGUI questHeader = Txt(questFrame.transform, "Header", "Nhiệm vụ", 28, TextAlignmentOptions.Left, AccentColor);
            Place(questHeader.gameObject, TopLeft, TopLeft, new Vector2(20, -12), new Vector2(380, 40));
            TextMeshProUGUI questBody = Txt(questFrame.transform, "Body", "", 22, TextAlignmentOptions.TopLeft);
            Stretch(questBody.gameObject, 20, 20, 60, 12);
            QuestTracker tracker = questFrame.gameObject.AddComponent<QuestTracker>();
            Wire(tracker, ("bodyText", questBody));

            // ── Dưới-giữa: hotbar kỹ năng
            GameObject hotbarObject = Node("SkillHotbar", hud.transform);
            Place(hotbarObject, BottomCenter, BottomCenter, new Vector2(0, 30), new Vector2(700, 110));
            HLayout(hotbarObject, 12);
            GameObject slot = Node("SlotTemplate", hotbarObject.transform);
            Size(Rect(slot), 100, 100);
            Img(slot.transform, "Frame", new Color(0f, 0f, 0f, 0.6f), Rounded).rectTransform.anchorMin = Vector2.zero;
            Stretch(slot.transform.Find("Frame").gameObject);
            Image icon = Img(slot.transform, "Icon", Color.white, Rounded);
            Stretch(icon.gameObject, 10, 10, 10, 10);
            Image cooldown = Filled(slot.transform, "Cooldown", new Color(0f, 0f, 0f, 0.7f), Image.FillMethod.Radial360);
            Stretch(cooldown.gameObject, 10, 10, 10, 10);
            TextMeshProUGUI key = Txt(slot.transform, "Key", "1", 20, TextAlignmentOptions.BottomLeft);
            Stretch(key.gameObject, 8, 8, 6, 4);
            TextMeshProUGUI timer = Txt(slot.transform, "Timer", "", 34, TextAlignmentOptions.Center);
            Stretch(timer.gameObject);
            SkillHotbar hotbar = hotbarObject.AddComponent<SkillHotbar>();
            Wire(hotbar, ("slotContainer", hotbarObject.transform), ("slotTemplate", slot));

            // ── Trái-giữa: thông báo tự ẩn
            GameObject feed = Node("Notifications", hud.transform);
            Place(feed, MiddleLeft, MiddleLeft, new Vector2(30, 40), new Vector2(560, 300));
            VLayout(feed, 8, null, TextAnchor.LowerLeft);
            Image entry = Img(feed.transform, "EntryTemplate", new Color(0f, 0f, 0f, 0.6f), Rounded);
            CanvasGroup entryGroup = entry.gameObject.AddComponent<CanvasGroup>();
            entryGroup.blocksRaycasts = false;
            Size(entry, preferredHeight: 50);
            TextMeshProUGUI entryText = Txt(entry.transform, "Text", "", 24, TextAlignmentOptions.MidlineLeft);
            Stretch(entryText.gameObject, 16, 16, 4, 4);
            NotificationFeed notifications = feed.AddComponent<NotificationFeed>();
            Wire(notifications, ("container", feed.transform), ("entryTemplate", entryGroup));

            TextMeshProUGUI hints = Txt(hud.transform, "Hints", "I túi đồ · K kỹ năng · J nhiệm vụ · Q/E đổi tab · Esc tạm dừng", 20, TextAlignmentOptions.Left, MutedColor);
            Place(hints.gameObject, BottomLeft, BottomLeft, new Vector2(30, 20), new Vector2(900, 36));

            Wire(hud, ("hpBar", hp), ("manaBar", mana), ("expBar", exp), ("nameText", characterName), ("levelText", level),
                 ("hotbar", hotbar), ("notifications", notifications));
            return hud;
        }

        private static StatBar Bar(Transform parent, string name, Vector2 position, Vector2 size, Color color, bool showText)
        {
            Image background = Img(parent, name, new Color(0f, 0f, 0f, 0.6f), White);
            Place(background.gameObject, TopLeft, TopLeft, position, size);
            Image trail = Filled(background.transform, "Trail", new Color(1f, 0.9f, 0.7f, 0.8f), Image.FillMethod.Horizontal);
            Stretch(trail.gameObject, 2, 2, 2, 2);
            Image fill = Filled(background.transform, "Fill", color, Image.FillMethod.Horizontal);
            Stretch(fill.gameObject, 2, 2, 2, 2);
            TextMeshProUGUI value = null;
            if (showText)
            {
                value = Txt(background.transform, "Value", "", size.y * 0.7f, TextAlignmentOptions.Center);
                Stretch(value.gameObject);
            }
            StatBar bar = background.gameObject.AddComponent<StatBar>();
            Wire(bar, ("fill", fill), ("trail", trail), ("valueText", value));
            return bar;
        }

        private static TabWindow BuildTabWindow(Transform canvas)
        {
            TabWindow panel = Panel<TabWindow>(canvas, "TabWindow", DimColor, pausesGame: true);
            RectTransform window = Window(panel.transform, "Window", new Vector2(1520, 880), null);

            GameObject tabs = Node("Tabs", window);
            Place(tabs, TopLeft, TopLeft, new Vector2(30, -24), new Vector2(1200, 64));
            HLayout(tabs, 12, null, TextAnchor.MiddleLeft);
            string[] titles = { "Túi đồ [I]", "Nhân vật", "Kỹ năng [K]", "Nhiệm vụ [J]" };
            Button[] tabButtons = new Button[titles.Length];
            for (int i = 0; i < titles.Length; i++)
            {
                tabButtons[i] = Btn(tabs.transform, "Tab" + i, titles[i], 26);
                Size(tabButtons[i], 250, 60);
            }
            TextMeshProUGUI tabHint = Txt(window, "TabHint", "Q / E để đổi tab", 20, TextAlignmentOptions.Right, MutedColor);
            Place(tabHint.gameObject, TopRight, TopRight, new Vector2(-110, -40), new Vector2(300, 30));
            Button close = Btn(window, "CloseButton", "X", 30, new Color(0.7f, 0.25f, 0.25f, 0.8f));
            Place(close.gameObject, TopRight, TopRight, new Vector2(-24, -24), new Vector2(64, 60));

            ItemTooltip tooltip = BuildTooltip(panel.transform);
            Image dragIcon = Img(panel.transform, "DragIcon", Color.white, Rounded);
            Place(dragIcon.gameObject, Center, Center, Vector2.zero, new Vector2(90, 90));

            InventoryPanel inventory = BuildInventoryPage(window, tooltip, dragIcon);
            CharacterPanel character = BuildCharacterPage(window);
            SkillTreePanel skills = BuildSkillPage(window);
            QuestPanel quests = BuildQuestPage(window);

            // Tooltip + icon kéo nằm trên cùng để luôn vẽ đè lên các trang.
            dragIcon.transform.SetAsLastSibling();
            tooltip.transform.SetAsLastSibling();

            Wire(panel, ("tabButtons", tabButtons), ("pages", new Object[] { inventory, character, skills, quests }), ("closeButton", close));
            return panel;
        }

        private static GameObject PageRoot(Transform window, string name)
        {
            GameObject page = Node(name, window);
            Stretch(page, 30, 30, 110, 30);
            return page;
        }

        private static ItemTooltip BuildTooltip(Transform parent)
        {
            Image background = Img(parent, "ItemTooltip", new Color(0.05f, 0.05f, 0.08f, 0.97f), Rounded);
            RectTransform rect = Place(background.gameObject, Center, new Vector2(0, 1), Vector2.zero, new Vector2(440, 200));
            VLayout(background.gameObject, 6, new RectOffset(18, 18, 14, 16), TextAnchor.UpperLeft);
            ContentSizeFitter fitter = background.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;   // Cao theo nội dung, rộng cố định 440
            background.gameObject.AddComponent<CanvasGroup>();
            TextMeshProUGUI title = Txt(background.transform, "Title", "", 30);
            TextMeshProUGUI body = Txt(background.transform, "Body", "", 22);
            ItemTooltip tooltip = background.gameObject.AddComponent<ItemTooltip>();
            Wire(tooltip, ("titleText", title), ("bodyText", body));
            rect.SetAsLastSibling();
            return tooltip;
        }

        private static InventoryPanel BuildInventoryPage(Transform window, ItemTooltip tooltip, Image dragIcon)
        {
            GameObject page = PageRoot(window, "InventoryPage");

            GameObject grid = Node("Grid", page.transform);
            Place(grid, TopLeft, TopLeft, Vector2.zero, new Vector2(730, 480));
            GridLayoutGroup layout = grid.AddComponent<GridLayoutGroup>();
            layout.cellSize = new Vector2(110, 110);
            layout.spacing = new Vector2(10, 10);
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layout.constraintCount = 6;   // 24 ô = 6 cột × 4 hàng

            Image frame = Img(grid.transform, "SlotTemplate", new Color(1, 1, 1, 0.1f), Rounded, raycast: true);
            frame.gameObject.AddComponent<CanvasGroup>();
            Image icon = Img(frame.transform, "Icon", Color.white, Rounded);
            Stretch(icon.gameObject, 14, 14, 14, 14);
            TextMeshProUGUI count = Txt(frame.transform, "Count", "", 22, TextAlignmentOptions.BottomRight);
            Stretch(count.gameObject, 6, 10, 6, 6);
            InventorySlot slot = frame.gameObject.AddComponent<InventorySlot>();
            Wire(slot, ("icon", icon), ("rarityFrame", frame), ("countText", count));

            TextMeshProUGUI gold = Txt(page.transform, "Gold", "", 32, TextAlignmentOptions.Left);
            Place(gold.gameObject, TopRight, TopRight, new Vector2(0, 0), new Vector2(600, 50));
            TextMeshProUGUI equipped = Txt(page.transform, "Equipped", "", 26, TextAlignmentOptions.TopLeft);
            Place(equipped.gameObject, TopRight, TopRight, new Vector2(0, -70), new Vector2(600, 260));
            TextMeshProUGUI help = Txt(page.transform, "Help",
                "Kéo thả để sắp xếp.\nChuột phải để mặc.\nRê chuột để xem & so sánh chỉ số.", 22, TextAlignmentOptions.TopLeft, MutedColor);
            Place(help.gameObject, TopRight, TopRight, new Vector2(0, -360), new Vector2(600, 120));

            InventoryPanel inventory = page.AddComponent<InventoryPanel>();
            Wire(inventory, ("gridContainer", grid.transform), ("slotTemplate", slot), ("dragIcon", dragIcon), ("tooltip", tooltip),
                 ("goldText", gold), ("equippedText", equipped));
            return inventory;
        }

        private static CharacterPanel BuildCharacterPage(Transform window)
        {
            GameObject page = PageRoot(window, "CharacterPage");
            TextMeshProUGUI header = Txt(page.transform, "Header", "", 32, TextAlignmentOptions.TopLeft);
            Place(header.gameObject, TopLeft, TopLeft, Vector2.zero, new Vector2(700, 180));
            TextMeshProUGUI stats = Txt(page.transform, "Stats", "", 26, TextAlignmentOptions.TopLeft);
            Place(stats.gameObject, TopLeft, TopLeft, new Vector2(0, -200), new Vector2(700, 520));
            CharacterPanel character = page.AddComponent<CharacterPanel>();
            Wire(character, ("headerText", header), ("statsText", stats));
            return character;
        }

        private static SkillTreePanel BuildSkillPage(Transform window)
        {
            GameObject page = PageRoot(window, "SkillPage");
            TextMeshProUGUI points = Txt(page.transform, "Points", "", 28, TextAlignmentOptions.Left);
            Place(points.gameObject, TopLeft, TopLeft, Vector2.zero, new Vector2(900, 44));

            GameObject tiers = Node("Tiers", page.transform);
            Place(tiers, TopLeft, TopLeft, new Vector2(0, -60), new Vector2(900, 620));
            VLayout(tiers, 40, null, TextAnchor.UpperCenter);
            GameObject row = Node("RowTemplate", tiers.transform);
            HLayout(row, 30);
            Size(Rect(row), preferredHeight: 110);
            Button node = Btn(row.transform, "NodeTemplate", "Kỹ năng", 24);
            Size(node, 250, 104);
            node.transform.SetParent(tiers.transform, false);   // Mẫu nút nằm ngoài hàng mẫu để không bị nhân đôi
            node.transform.SetAsFirstSibling();

            Image detailFrame = Img(page.transform, "Detail", new Color(0, 0, 0, 0.25f), Rounded);
            Place(detailFrame.gameObject, TopRight, TopRight, new Vector2(0, -60), new Vector2(480, 620));
            TextMeshProUGUI detail = Txt(detailFrame.transform, "Text", "", 24, TextAlignmentOptions.TopLeft);
            Stretch(detail.gameObject, 20, 20, 20, 120);
            Button unlock = Btn(detailFrame.transform, "UnlockButton", "Học", 28, AccentColor);
            Place(unlock.gameObject, BottomCenter, BottomCenter, new Vector2(0, 24), new Vector2(260, 70));

            SkillTreePanel skills = page.AddComponent<SkillTreePanel>();
            Wire(skills, ("tierContainer", tiers.transform), ("rowTemplate", row), ("nodeTemplate", node), ("pointsText", points),
                 ("detailText", detail), ("unlockButton", unlock));
            return skills;
        }

        private static QuestPanel BuildQuestPage(Transform window)
        {
            GameObject page = PageRoot(window, "QuestPage");
            Transform list = ScrollList(page.transform, "QuestList");
            Place(list.parent.parent.gameObject, TopLeft, TopLeft, Vector2.zero, new Vector2(520, 720));
            Button template = Btn(list, "QuestButtonTemplate", "Nhiệm vụ", 24);
            template.GetComponentInChildren<TMP_Text>().alignment = TextAlignmentOptions.MidlineLeft;
            Size(template, preferredHeight: 64);

            TextMeshProUGUI detail = Txt(page.transform, "Detail", "", 26, TextAlignmentOptions.TopLeft);
            Place(detail.gameObject, TopRight, TopRight, Vector2.zero, new Vector2(860, 600));
            Toggle track = Tgl(page.transform, "TrackToggle", "Theo dõi trên HUD", 26);
            Place(track.gameObject, new Vector2(1, 0), new Vector2(1, 0), new Vector2(-560, 20), new Vector2(320, 44));

            QuestPanel quests = page.AddComponent<QuestPanel>();
            Wire(quests, ("listContainer", list), ("questButtonTemplate", template), ("detailText", detail), ("trackToggle", track));
            return quests;
        }

        private static DialoguePanel BuildDialogue(Transform canvas)
        {
            // Không có nền phủ màn hình (thấy game phía sau) và không ẩn HUD; vẫn dừng game khi đang nói chuyện.
            DialoguePanel panel = Panel<DialoguePanel>(canvas, "DialoguePanel", new Color(0, 0, 0, 0.001f), hidePrevious: false, pausesGame: true);
            Image box = Img(panel.transform, "Box", new Color(0.05f, 0.05f, 0.09f, 0.95f), Rounded, raycast: true);
            Place(box.gameObject, BottomCenter, BottomCenter, new Vector2(0, 40), new Vector2(1500, 290));
            Button next = box.gameObject.AddComponent<Button>();   // Click bất cứ đâu trên hộp thoại = tiếp / tua nhanh
            next.transition = Selectable.Transition.None;

            TextMeshProUGUI speaker = Txt(box.transform, "Speaker", "", 32, TextAlignmentOptions.Left, AccentColor);
            Place(speaker.gameObject, TopLeft, TopLeft, new Vector2(40, -20), new Vector2(800, 46));
            TextMeshProUGUI body = Txt(box.transform, "Body", "", 30, TextAlignmentOptions.TopLeft);
            Stretch(body.gameObject, 40, 40, 80, 50);
            TextMeshProUGUI hint = Txt(box.transform, "ContinueHint", "Click / Space ›", 22, TextAlignmentOptions.Right, MutedColor);
            Place(hint.gameObject, new Vector2(1, 0), new Vector2(1, 0), new Vector2(-40, 14), new Vector2(400, 36));

            Toggle fastForward = Tgl(box.transform, "FastForwardToggle", "Tua nhanh", 22);
            Place(fastForward.gameObject, TopRight, TopRight, new Vector2(-220, -20), new Vector2(200, 44));
            Button skip = Btn(box.transform, "SkipButton", "Bỏ qua", 24);
            Place(skip.gameObject, TopRight, TopRight, new Vector2(-24, -16), new Vector2(170, 52));

            Wire(panel, ("speakerText", speaker), ("bodyText", body), ("continueHint", hint.gameObject), ("nextButton", next),
                 ("fastForwardToggle", fastForward), ("skipButton", skip));
            return panel;
        }

        private static ShopPanel BuildShop(Transform canvas)
        {
            ShopPanel panel = Panel<ShopPanel>(canvas, "ShopPanel", DimColor, pausesGame: true);
            RectTransform window = Window(panel.transform, "Window", new Vector2(1500, 880), "Cửa hàng — Thợ rèn");
            TextMeshProUGUI gold = Txt(window, "Gold", "", 30, TextAlignmentOptions.Right);
            Place(gold.gameObject, TopRight, TopRight, new Vector2(-120, -30), new Vector2(400, 44));
            Button close = Btn(window, "CloseButton", "X", 30, new Color(0.7f, 0.25f, 0.25f, 0.8f));
            Place(close.gameObject, TopRight, TopRight, new Vector2(-24, -24), new Vector2(64, 60));

            TextMeshProUGUI buyLabel = Txt(window, "BuyLabel", "Mua", 28, TextAlignmentOptions.Left, MutedColor);
            Place(buyLabel.gameObject, TopLeft, TopLeft, new Vector2(40, -100), new Vector2(600, 40));
            Transform buyList = ScrollList(window, "BuyList");
            Place(buyList.parent.parent.gameObject, TopLeft, TopLeft, new Vector2(40, -145), new Vector2(690, 610));
            TextMeshProUGUI sellLabel = Txt(window, "SellLabel", "Bán (túi đồ của bạn)", 28, TextAlignmentOptions.Left, MutedColor);
            Place(sellLabel.gameObject, TopRight, TopRight, new Vector2(-40, -100), new Vector2(690, 40));
            Transform sellList = ScrollList(window, "SellList");
            Place(sellList.parent.parent.gameObject, TopRight, TopRight, new Vector2(-40, -145), new Vector2(690, 610));

            Image row = Img(buyList, "RowTemplate", new Color(1, 1, 1, 0.06f), Rounded, raycast: true);
            Size(row, preferredHeight: 76);
            Image icon = Img(row.transform, "Icon", Color.white, Rounded);
            Place(icon.gameObject, MiddleLeft, MiddleLeft, new Vector2(12, 0), new Vector2(54, 54));
            TextMeshProUGUI itemName = Txt(row.transform, "Name", "", 26, TextAlignmentOptions.MidlineLeft);
            Stretch(itemName.gameObject, 80, 300, 0, 0);
            TextMeshProUGUI price = Txt(row.transform, "Price", "", 26, TextAlignmentOptions.MidlineRight);
            Place(price.gameObject, MiddleRight, MiddleRight, new Vector2(-170, 0), new Vector2(120, 60));
            Button action = Btn(row.transform, "ActionButton", "Mua", 24, AccentColor);
            Place(action.gameObject, MiddleRight, MiddleRight, new Vector2(-12, 0), new Vector2(140, 56));
            ShopItemRow rowView = row.gameObject.AddComponent<ShopItemRow>();
            Wire(rowView, ("icon", icon), ("nameText", itemName), ("priceText", price), ("actionButton", action));

            TextMeshProUGUI message = Txt(window, "Message", "", 24, TextAlignmentOptions.Left, AccentColor);
            Place(message.gameObject, BottomLeft, BottomLeft, new Vector2(40, 40), new Vector2(1200, 50));
            ItemTooltip tooltip = BuildTooltip(panel.transform);

            Wire(panel, ("buyContainer", buyList), ("sellContainer", sellList), ("rowTemplate", rowView), ("goldText", gold),
                 ("messageText", message), ("tooltip", tooltip), ("closeButton", close));
            return panel;
        }

        private static PausePanel BuildPause(Transform canvas, SettingsPanel settings)
        {
            PausePanel panel = Panel<PausePanel>(canvas, "PausePanel", DimColor, hidePrevious: false, pausesGame: true);
            RectTransform window = Window(panel.transform, "Window", new Vector2(560, 620), "Tạm dừng");
            GameObject column = Node("Buttons", window);
            Stretch(column, 50, 50, 110, 40);
            VLayout(column, 18);
            Button resume = Btn(column.transform, "ResumeButton", "Tiếp tục", 30, AccentColor);
            Size(resume, preferredHeight: 80);
            Button save = Btn(column.transform, "SaveButton", "Lưu game", 30);
            Size(save, preferredHeight: 80);
            Button settingsButton = Btn(column.transform, "SettingsButton", "Cài đặt", 30);
            Size(settingsButton, preferredHeight: 80);
            Button menu = Btn(column.transform, "MainMenuButton", "Về menu chính", 30);
            Size(menu, preferredHeight: 80);

            Wire(panel, ("resumeButton", resume), ("saveButton", save), ("settingsButton", settingsButton),
                 ("mainMenuButton", menu), ("settingsPanel", settings));
            return panel;
        }

        private static GameOverPanel BuildGameOver(Transform canvas)
        {
            GameOverPanel panel = Panel<GameOverPanel>(canvas, "GameOverPanel", new Color(0.2f, 0.02f, 0.03f, 0.88f),
                                                       closeOnEscape: false, pausesGame: true);
            TextMeshProUGUI title = Txt(panel.transform, "Title", "BẠN ĐÃ GỤC NGÃ", 96, TextAlignmentOptions.Center, new Color(1f, 0.4f, 0.35f));
            title.fontStyle = FontStyles.Bold;
            Place(title.gameObject, Center, Center, new Vector2(0, 160), new Vector2(1400, 140));
            TextMeshProUGUI subtitle = Txt(panel.transform, "Subtitle", "Hồi sinh ở checkpoint gần nhất hoặc quay về menu.", 30, TextAlignmentOptions.Center, MutedColor);
            Place(subtitle.gameObject, Center, Center, new Vector2(0, 60), new Vector2(1400, 50));

            GameObject row = Node("Buttons", panel.transform);
            Place(row, Center, Center, new Vector2(0, -80), new Vector2(760, 90));
            HLayout(row, 40);
            Button respawn = Btn(row.transform, "RespawnButton", "Hồi sinh", 32, AccentColor);
            Size(respawn, 340, 84);
            Button menu = Btn(row.transform, "MainMenuButton", "Về menu", 32);
            Size(menu, 340, 84);

            Wire(panel, ("respawnButton", respawn), ("mainMenuButton", menu));
            return panel;
        }

        // ═════════════════════════ GameplayMock ═════════════════════════

        private static void BuildGameplayMock(UIDebugConfig config, WorldHealthBar healthBarPrefab)
        {
            Scene scene = NewScene();
            Camera camera = CameraObject(new Color(0.12f, 0.14f, 0.13f), 6f);
            EventSystemObject();
            Bootstrap(config, applySavedSettings: false);

            Block("Floor", new Vector2(0, 0), new Vector2(20, 11), new Color(0.18f, 0.2f, 0.19f), -10);
            Block("Player (giả)", new Vector2(0, -2.5f), new Vector2(0.8f, 1.2f), new Color(0.35f, 0.55f, 1f), 0);
            WorldLabel("Người chơi (giả)", new Vector2(0, -3.6f), 3f);
            Block("NPC Thợ rèn", new Vector2(-6f, 2f), new Vector2(0.9f, 1.3f), new Color(0.95f, 0.8f, 0.3f), 0);
            WorldLabel("Thợ rèn — nhấn T", new Vector2(-6f, 0.9f), 3f);
            WorldLabel("GAMEPLAY MOCK — click quái · 1-5 kỹ năng · H/M/X · T nói chuyện · B shop · U nhiệm vụ · N thông báo",
                       new Vector2(0, 5.2f), 3.2f);

            Vector2[] enemyPositions = { new(3f, 2f), new(6f, -0.5f), new(3.5f, -3f) };
            for (int i = 0; i < enemyPositions.Length; i++)
            {
                GameObject enemy = Block("Slime " + (i + 1), enemyPositions[i], new Vector2(1f, 1f), new Color(0.9f, 0.35f, 0.4f), 0);
                enemy.AddComponent<BoxCollider2D>();   // Collider 1x1 khớp sprite 1x1 → click trúng mới tính

                // Nạp lại prefab từ đĩa: tham chiếu giữ từ lúc tạo có thể bị Unity hủy sau khi mở scene mới.
            GameObject barPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(HealthBarPrefabPath);
            GameObject barObject = (GameObject)PrefabUtility.InstantiatePrefab(barPrefab);
                barObject.name = enemy.name + " HealthBar";
                MockEnemy mockEnemy = enemy.AddComponent<MockEnemy>();
                Wire(mockEnemy, ("healthBar", barObject.GetComponent<WorldHealthBar>()), ("body", enemy.GetComponent<SpriteRenderer>()));
            }

            GameObject controllerObject = new("GameplayMockController");
            GameplayMockController controller = controllerObject.AddComponent<GameplayMockController>();
            Wire(controller, ("worldCamera", camera));

            Save(scene, GameplayMockPath);
        }

        private static GameObject Block(string name, Vector2 position, Vector2 size, Color color, int sortingOrder)
        {
            GameObject go = new(name, typeof(SpriteRenderer));
            go.transform.position = position;
            go.transform.localScale = new Vector3(size.x, size.y, 1f);
            SpriteRenderer renderer = go.GetComponent<SpriteRenderer>();
            renderer.sprite = White;
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;
            return go;
        }

        private static void WorldLabel(string text, Vector2 position, float fontSize)
        {
            GameObject go = new(text.Length > 24 ? text.Substring(0, 24) : text);
            TextMeshPro label = go.AddComponent<TextMeshPro>();
            label.font = TMP_Settings.defaultFontAsset;
            label.text = text;
            label.fontSize = fontSize;
            label.alignment = TextAlignmentOptions.Center;
            label.enableWordWrapping = false;
            label.color = MutedColor;
            label.rectTransform.sizeDelta = new Vector2(20f, 1f);
            label.GetComponent<MeshRenderer>().sortingOrder = 5;
            go.transform.position = position;
        }
    }
}
