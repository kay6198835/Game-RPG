using System;
using System.Collections.Generic;
using UnityEngine;

namespace UIFlow
{
    /// <summary>
    /// Kênh sự kiện riêng của UI: bên ngoài (gameplay) YÊU CẦU UI
    /// làm gì đó mà không cần giữ tham chiếu tới panel nào.
    ///
    /// Vì sao không dùng EventManager của gameplay? EventManager nhận object không kiểu và thêm giá trị
    /// vào enum EventID là sửa code gameplay. Kênh này nằm hoàn toàn trong UIFlow.
    /// Người đăng ký (GameplayUIController) phải hủy đăng ký trong OnDisable.
    /// </summary>
    public static class UIEvents
    {
        /// <summary>Mở hộp thoại NPC. Callback onFinished chạy khi hộp thoại đóng.</summary>
        public static event Action<IReadOnlyList<DialogueLine>, Action> DialogueRequested;
        public static event Action<ConfirnData> OpenConfirnPanel;
        public static event Action ShopRequested;
        public static event Action StatRequested;

        /// <summary>Thông báo tự ẩn góc màn hình.</summary>
        public static event Action<string> NotificationRequested;
        public static event Action<InteractData> InteractiveNotificationRequested;

        /// <summary>Hiện số damage world-space tại vị trí (tọa độ thế giới).</summary>
        public static event Action<Vector3, float, bool> DamageNumberRequested;

        /// <summary>Kỹ năng ở ô index trên hotbar vừa được dùng → hiện cooldown.</summary>
        public static event Action<int> SkillUsed;

        /// <summary>Gameplay xin rời màn chơi về menu chính (ví dụ: tương tác sau khi boss chết).</summary>
        public static event Action ReturnToMainMenuRequested;

        public static void RequestDialogue(IReadOnlyList<DialogueLine> lines, Action onFinished = null)
            => DialogueRequested?.Invoke(lines, onFinished);

        public static void RequestShop() => ShopRequested?.Invoke();

        public static void RequestStats() => StatRequested?.Invoke();
        public static void Notify(string message) => NotificationRequested?.Invoke(message);

        public static void ShowDamage(Vector3 worldPosition, float amount, bool isCritical = false)
            => DamageNumberRequested?.Invoke(worldPosition, amount, isCritical);

        public static void UseSkill(int hotbarIndex) => SkillUsed?.Invoke(hotbarIndex);

        /// <summary>
        /// Về menu chính. GameplayUIController đóng hết panel rồi gọi SceneFlow.ReturnToMainMenu().
        /// Không có GameplayUI (bấm Play thẳng ở LoadRandomMap) thì chuyển scene luôn.
        /// </summary>
        public static void RequestReturnToMainMenu()
        {
            if (ReturnToMainMenuRequested != null) ReturnToMainMenuRequested.Invoke();
            else SceneFlow.ReturnToMainMenu();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            DialogueRequested = null;
            ShopRequested = null;
            StatRequested = null;
            NotificationRequested = null;
            DamageNumberRequested = null;
            SkillUsed = null;
            ReturnToMainMenuRequested = null;
        }

        public static void OnOpenConfirmPanel(ConfirnData data)
        {
            OpenConfirnPanel.Invoke(data);
        }

        public static void SwitchInteractiveNotification(InteractData data = null) => InteractiveNotificationRequested?.Invoke(data);
    }
}
