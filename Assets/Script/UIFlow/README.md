# UIFlow — Luồng UI độc lập (uGUI + TextMeshPro)

Tài liệu cho người mới học Unity UI. Toàn bộ luồng UI nằm trong `Assets/Script/UIFlow/`, namespace `UIFlow`,
**không sửa một dòng code gameplay nào**. Mọi chỗ dùng dữ liệu giả / bỏ qua logic cũ đều có comment `// TODO: nối logic thật`.

---

## 0. Chạy thử trong 30 giây

1. Mở Unity → `Assets/Scenes/Main/MainGamePlay.unity` → bấm **Play**.
2. Logo → menu chính → **Tiếp tục** → màn Loading → scene `GameplayMock` có HUD.
3. Trong GameplayMock: click vào quái đỏ (Shift+click = chí mạng), `1`–`5` kỹ năng, `H` mất máu, `M` tốn mana,
   `X` +EXP, `T` nói chuyện NPC (xong tự mở shop), `B` shop, `U` tiến độ nhiệm vụ, `N` thông báo,
   `I` túi đồ, `K` kỹ năng, `J` nhiệm vụ, `Q`/`E` đổi tab, `Esc` quay lại / tạm dừng.

Kiểm tra tự động cả luồng: **Tools > UI Flow > Run Smoke Test** (37 bước, xem Console: `RESULT: PASS`).
Dựng lại scene sau khi sửa bố cục trong code: **Tools > UI Flow > Build All** (⚠️ ghi đè 4 scene UIFlow).

---

## 1. Luồng scene

```
Khởi động
  └─ MainGamePlay (menu) ──Tiếp tục / Chơi mới / Chọn save──► Loading ──► GameplayMock  (skipGameplayInit = BẬT)
                                                                     └─► LoadRandomMap (skipGameplayInit = TẮT)
       ▲                                                                   │  + GameplayUI load Additive chồng lên
       └──────────── Loading ◄──── Tạm dừng > Về menu / Game Over > Về menu ┘
```

Build Settings (script tự đặt):

| # | Scene | Vai trò |
|---|---|---|
| 0 | `Main/MainGamePlay` | Logo, đăng nhập, menu chính, tạo nhân vật, chọn save, cài đặt |
| 1 | `Main/Loading` | Màn loading dùng chung, thanh tiến trình thật |
| 2 | `Main/Test/LoadRandomMap` | Dungeon thật — **không sửa** |
| 3 | `Main/GameplayUI` | Chỉ có UI in-game, luôn load kiểu **Additive** |
| 4 | `Test/GameplayMock` | Gameplay giả để test HUD |
| 5+ | `StartScene`, `SetLevel` | Giữ nguyên như cũ |

**Vì sao LoadRandomMap không phải màn loading?** Scene tên `LoadRandomMap` thực ra là scene *gameplay*
(sinh mê cung, Player, quái, `GameLifetimeScope`). Nó không có `LoadSceneAsync` hay thanh tiến trình nào.
Nên màn loading là scene mới `Loading`, còn `LoadRandomMap` đóng vai "Gameplay".

**Vì sao HUD nằm ở scene riêng `GameplayUI`?** Để gắn HUD vào scene thật mà **không sửa file scene đó**:
`SceneFlow` nghe `SceneManager.sceneLoaded`, khi scene đích xong thì `LoadScene(GameplayUI, Additive)`.

---

## 2. Cờ mock / bypass — `Assets/SO/UIFlow/UIDebugConfig.asset`

| Cờ | BẬT (mặc định khi làm UI) | TẮT |
|---|---|---|
| `useMockData` | Chỉ số, túi đồ, nhiệm vụ, cây kỹ năng giả | Bản Real (hiện là stub, chờ nối) |
| `skipLogin` | Logo → menu chính luôn | Logo → màn đăng nhập (Real = server "Offline") |
| `skipGameplayInit` | Vào game = `GameplayMock` | Vào game = `LoadRandomMap` thật |
| `skipSaveLoad` | 3 save mẫu trong RAM | Save JSON thật ở `Application.persistentDataPath/ui_saves.json` |
| `bypassLoadRandomLogic` | Vào `LoadRandomMap` **không** gắn UI mới — scene cũ chạy y hệt hiện nay | Gắn `GameplayUI` (HUD, tạm dừng, túi đồ) lên map thật |

Chỗ duy nhất đọc cờ: `Core/UIServices.cs`. UI chỉ gọi `UIServices.Save / Login / Player / Inventory / Quests`
(các interface) nên **tắt hết cờ là chạy bản Real mà không sửa code UI** (smoke test bước 31–37 kiểm tra đúng điều này).

---

## 3. Cấu trúc code

```
UIFlow/
  Core/        UIDebugConfig, SceneNames, SceneFlow (truyền scene đích), UIServices (chọn Mock/Real),
               UIBootstrap, UIPanel (ẩn/hiện bằng CanvasGroup), UIManager (stack panel + Esc), UIInput, UIEvents
  Data/        UIDataModels — lớp dữ liệu thuần UI hiển thị (không dính Player/ItemSO)
  Services/    Interfaces/ (ISaveProvider, ILoginService, IPlayerDataProvider, IInventoryProvider, IQuestProvider)
               Mock/ (dữ liệu mẫu) · Real/ (bọc logic thật / stub) · SettingsStore · KeyBindings
  Menu/        Splash, Login, MainMenu, CharacterCreation, SaveSelect(+SaveSlotView), Settings(+KeyBindingRow), MainMenuFlow
  Loading/     LoadingScreen
  Gameplay/    HUD/ (HUDPanel, StatBar, SkillHotbar, QuestTracker, NotificationFeed)
               Windows/ (TabWindow, Inventory, InventorySlot, ItemTooltip, Character, SkillTree, Quest)
               Dialogue/, Shop/, World/ (WorldHealthBar, DamageNumber, DamageNumberSpawner)
               PausePanel, GameOverPanel, GameplayUIController, Mock/ (GameplayMockController, MockEnemy)
  Editor/      UIKit (hàm dựng uGUI), UIFlowBuilder (dựng scene), UIFlowSmokeTest
```

**Truyền "scene đích" sang Loading — vì sao `static class SceneFlow`?**
Biến static sống qua mọi lần đổi scene mà không cần GameObject `DontDestroyOnLoad` nào chen vào scene gameplay cũ.
ScriptableObject cũng giữ được giá trị nhưng trong Editor giá trị runtime có thể bị ghi ngược vào `.asset`
(project từng dính lỗi này — BUG-063). Dữ liệu cần truyền chỉ là một chuỗi tên scene.

**Vì sao `UIServices` (static) thay vì VContainer như gameplay?** `GameLifetimeScope` chỉ có trong scene gameplay và
ném lỗi ngay khi thiếu component. Scene menu / loading không có Player nên UI cần một chỗ chọn provider độc lập.

---

## 4. Hierarchy từng scene

### MainGamePlay
```
Main Camera            (Orthographic, nền tối)
EventSystem            (InputSystemUIInputModule — project dùng Input System mới)
UIBootstrap            (config = UIDebugConfig, applySavedSettings = true)
MenuCanvas             Screen Space - Overlay · Canvas Scaler 1920x1080 · UIManager
  Background
  SplashPanel          Logo (Title, Subtitle), Hint
  LoginPanel           Window (UserNameInput, PasswordInput, ServerList/…/ServerRowTemplate, StatusText, LoginButton)
  MainMenuPanel        Title, Buttons (ContinueButton+Detail, NewGame, LoadGame, Settings, Quit), Version
  CharacterCreationPanel  ClassColumn · Preview (Body, Hair, Compass/Arrow, DirectionText, RotateLeft/Right, AutoRotate)
                          · AppearanceColumn (Hair/Skin/Outfit rows, NameInput, ErrorText) · Footer (Back, Random, Confirm)
  SaveSelectPanel      Window (SaveList/…/SaveSlotTemplate, EmptyText, MessageText, BackButton)
  SettingsPanel        Window (Tabs, AudioPage, GraphicsPage, KeysPage/KeyList/…/KeyRowTemplate, BackButton)
MainMenuFlow           (nối các panel với nhau)
```

### Loading
```
UIBootstrap
LoadingCanvas          Screen Space - Overlay · sortingOrder 100 · LoadingScreen
  Background, BottomShade, TargetText, TipText, ProgressBar/Fill (Image Filled), PercentText
  ErrorGroup (tắt sẵn)  ErrorText, BackToMenuButton, EventSystem (chỉ bật khi lỗi)
```
Không có Camera/EventSystem riêng: Canvas Overlay không cần camera, và scene đích mang camera của nó vào
→ không bị cảnh báo 2 AudioListener / 2 EventSystem.

### GameplayUI (load Additive)
```
UIBootstrap
GameplayCanvas         Screen Space - Overlay · sortingOrder 10 · UIManager (root = HUD, Esc rỗng = Pause, controlsTimeScale)
                       · GameplayUIController
  HUDPanel             StatusTopLeft (LevelBadge, Name, HPBar, ManaBar, ExpBar) · Minimap (trên-phải)
                       · QuestTracker (phải) · SkillHotbar (dưới-giữa) · Notifications (trái-giữa) · Hints
  TabWindow            Window (Tabs, CloseButton, InventoryPage, CharacterPage, SkillPage, QuestPage) · DragIcon · ItemTooltip
  DialoguePanel        Box (Speaker, Body, ContinueHint, FastForwardToggle, SkipButton)
  ShopPanel            Window (BuyList, SellList, RowTemplate, Gold, Message) · ItemTooltip
  PausePanel           Window (Resume, Save, Settings, MainMenu)
  GameOverPanel        Title, Subtitle, Buttons (Respawn, MainMenu)
  SettingsPanel
DamageNumbers          DamageNumberSpawner (pool số damage)
```

### GameplayMock
```
Main Camera, EventSystem, UIBootstrap
Floor, Player (giả), NPC Thợ rèn, nhãn chữ (TextMeshPro 3D)
Slime 1..3             SpriteRenderer + BoxCollider2D + MockEnemy
Slime N HealthBar      prefab WorldHealthBar — Canvas **World Space**
GameplayMockController
```

**Screen Space hay World Space?** Mọi menu/HUD/cửa sổ = *Screen Space - Overlay* (luôn phủ màn hình, không cần camera).
Thanh máu trên đầu quái = *World Space* (đi theo quái, scale 0.01 để 100 px = 1 unit).
Số damage = `TextMeshPro` 3D (không cần Canvas, rẻ hơn một Canvas cho mỗi con số).

---

## 5. Anchor + Canvas Scaler (để UI không vỡ ở mọi độ phân giải)

- Canvas Scaler: **Scale With Screen Size**, Reference **1920x1080**, Match **0.5** → màn rộng hơn hay cao hơn 16:9
  đều co giãn cân bằng.
- Neo mỗi khối vào góc/cạnh nó thuộc về: máu/mana = **top-left**, minimap = **top-right**, hotbar = **bottom-center**,
  nhiệm vụ = **right**, cửa sổ = **center**, thanh loading = kéo giãn **trái↔phải** với lề 160.
- Thử: Game view → đổi sang 1280x720, 2560x1080, 1920x1200 — không khối nào bị lệch ra ngoài.

---

## 6. Lỗi thường gặp và cách tránh

| Triệu chứng | Nguyên nhân | Cách tránh |
|---|---|---|
| `Scene 'X' couldn't be loaded because it has not been added to the build settings` | Quên thêm scene vào Build Settings | Chạy **Tools > UI Flow > Update Build Settings Only**. Loading cũng tự báo lỗi thay vì treo |
| Thanh máu / cooldown không chạy dù `fillAmount` đổi | `Image` kiểu Filled mà không có sprite | Luôn gán sprite (dự án dùng `Sprite/UIFlow/UIWhite.png`) |
| Kéo đồ thả vào ô khác không ăn | Icon đang kéo chặn raycast nên ô đích không nhận `OnDrop` | `dragIcon.raycastTarget = false` |
| Tooltip nhấp nháy khi rê chuột | Tooltip đè lên ô đồ và "ăn" chuột | `CanvasGroup.blocksRaycasts = false` trên tooltip |
| Về menu xong mọi thứ đứng hình | Menu tạm dừng để `Time.timeScale = 0` | `SceneFlow.GoTo` và `UIManager.OnDestroy` luôn trả về 1 |
| Fade / chữ chạy không chạy khi tạm dừng | Dùng `Time.deltaTime` (bằng 0 khi timeScale = 0) | UI dùng `Time.unscaledDeltaTime` / `WaitForSecondsRealtime` |
| Nhấn Esc lúc gán phím thì đóng luôn màn Cài đặt | Cùng frame UIManager cũng đọc Esc | `UIManager.BlockEscape`, mở khóa ở frame sau |
| Cảnh báo "There are 2 event systems" | Scene additive mang EventSystem riêng | GameplayUI không có EventSystem; chỉ tạo khi thiếu |
| `Ambiguity between 'Path'…` / `'Resources'…` | Project có class `Path` (A*) toàn cục | Viết đầy đủ `System.IO.Path` |
| Chữ tiếng Việt thành ô vuông | Font không có glyph | Font mặc định có fallback động; font riêng phải bật Dynamic hoặc thêm ký tự tiếng Việt vào atlas |
| HUD hiện chậm vài giây khi vào game | `LoadSceneAsync` Additive bị chia nhỏ theo `backgroundLoadingPriority` | Dùng `LoadScene(…, Additive)` (đo: 16–20 s → 0.3 s) |

---

## 7. Cách ly khỏi gameplay cũ — những gì đã kiểm tra

| Chỗ dính gameplay | Ảnh hưởng tới UI | Cách xử lý (không sửa file gốc) |
|---|---|---|
| `EventManager` (bus static) | `ON_PLAYER_DEATH` bắn **mỗi frame** (BUG-086) | `RealPlayerDataProvider` chỉ *nghe*, có cờ chỉ phản ứng 1 lần, hủy đăng ký khi reset |
| Singleton `MazeController`, `EnemyManager`, `LevelManager` | Chỉ tồn tại trong `LoadRandomMap` | Scene menu / loading không chứa chúng; không có `DontDestroyOnLoad` nào |
| `GameLifetimeScope` (VContainer) | Ném lỗi nếu thiếu component | UI không dùng container; `UIServices` độc lập |
| Phím Q/E là phím gameplay | Mở túi đồ bấm Q/E có thể ra chiêu | Cửa sổ tab đặt `pausesGame` → game dừng khi đang mở |
| Chết không có đường hồi sinh (BUG-087) | Game Over trên map thật | Real `Respawn()` = load lại scene gameplay qua Loading (TODO) |
| Không có save / login / túi đồ / nhiệm vụ | Menu cần dữ liệu | Mock đầy đủ; Real là stub có `// TODO: nối logic thật` |

---

## 8. Git

Làm trên branch UI flow riêng (hiện là `origin/feature/ui-flow-maingameplay`), không làm trên `main`. Luôn commit file `.meta` đi kèm.
Sau khi chạy Play, TextMeshPro có thể tự sửa `LiberationSans SDF - Fallback.asset` (thêm glyph vào atlas động) —
đó là dữ liệu sinh tự động, **không cần commit**: `git checkout -- "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF - Fallback.asset"`.
