using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace UIFlow
{
    /// <summary>
    /// Bàn điều khiển test của scene GameplayMock. Thay cho gameplay thật để thử toàn bộ UI in-game:
    ///   Click quái = gây damage (Shift+click = chí mạng) · 1..5 = dùng kỹ năng trên hotbar
    ///   H = mất 25 máu · M = tốn 15 mana · X = +300 EXP · T = nói chuyện NPC (xong thì mở shop)
    ///   B = mở shop · U = tiến độ nhiệm vụ · N = thông báo thử
    /// TODO: nối logic thật — scene này chỉ tồn tại khi skipGameplayInit bật.
    /// </summary>
    public class GameplayMockController : MonoBehaviour
    {
        [SerializeField] private Camera worldCamera;

        private static readonly Key[] SkillKeys = { Key.Digit1, Key.Digit2, Key.Digit3, Key.Digit4, Key.Digit5 };
        private readonly Collider2D[] _hits = new Collider2D[4];   // Cấp phát 1 lần, dùng lại mỗi click

        private void Start()
        {
            // Bấm Play thẳng ở GameplayMock (không đi qua menu) thì SceneFlow chưa gắn GameplayUI → tự gắn.
            // IsValid() (không phải isLoaded): scene đang load dở cũng tính, tránh load 2 lần.
            if (!SceneManager.GetSceneByName(SceneNames.GameplayUI).IsValid())
            {
                SceneManager.LoadScene(SceneNames.GameplayUI, LoadSceneMode.Additive);
            }
            StartCoroutine(ShowHintWhenUIReady());
        }

        private IEnumerator ShowHintWhenUIReady()
        {
            // Thông báo gửi trước khi GameplayUI load xong sẽ không ai nhận → đợi scene UI sẵn sàng.
            while (!SceneManager.GetSceneByName(SceneNames.GameplayUI).isLoaded) yield return null;
            yield return null;   // Thêm 1 frame để GameplayUIController chạy OnEnable đăng ký sự kiện
            UIEvents.Notify("GameplayMock: click quái, 1-5, H, M, X, T, B, U, N để thử UI.");
        }

        private void Update()
        {
            // Khi game dừng (menu tạm dừng / túi đồ) thì không nhận phím test.
            if (Time.timeScale == 0f || Keyboard.current == null) return;

            HandleClick();

            for (int i = 0; i < SkillKeys.Length; i++)
            {
                if (Keyboard.current[SkillKeys[i]].wasPressedThisFrame) UIEvents.UseSkill(i);
            }

            MockPlayerDataProvider player = UIServices.Player as MockPlayerDataProvider;
            if (Keyboard.current.hKey.wasPressedThisFrame) player?.DebugChangeHP(-25f);
            if (Keyboard.current.mKey.wasPressedThisFrame) player?.DebugChangeMana(-15f);
            if (Keyboard.current.xKey.wasPressedThisFrame) player?.DebugAddExp(300);
            if (Keyboard.current.tKey.wasPressedThisFrame) UIEvents.RequestDialogue(MockCatalog.CreateNpcDialogue(), UIEvents.RequestShop);
            if (Keyboard.current.bKey.wasPressedThisFrame) UIEvents.RequestShop();
            if (Keyboard.current.nKey.wasPressedThisFrame) UIEvents.Notify("Nhặt được Quặng bạch kim x1");
            if (Keyboard.current.uKey.wasPressedThisFrame && UIServices.Quests is MockQuestProvider quests)
            {
                QuestData changed = quests.DebugAdvance();
                UIEvents.Notify(changed == null ? "Mọi nhiệm vụ đã xong." : (changed.isCompleted ? "Hoàn thành: " : "Tiến độ: ") + changed.title);
            }
        }

        private void HandleClick()
        {
            if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame) return;
            // Click trúng UI (nút, cửa sổ) thì không tính là đánh quái.
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            Vector2 world = worldCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            int count = Physics2D.OverlapPointNonAlloc(world, _hits);
            for (int i = 0; i < count; i++)
            {
                if (!_hits[i].TryGetComponent(out MockEnemy enemy)) continue;
                bool critical = Keyboard.current.shiftKey.isPressed;
                enemy.Hit(critical ? Random.Range(35f, 50f) : Random.Range(12f, 22f), critical);
                return;
            }
        }
    }
}
