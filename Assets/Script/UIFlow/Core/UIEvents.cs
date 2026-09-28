using System;
using System.Collections.Generic;
using UnityEngine;

namespace UIFlow
{
    /// <summary>
    /// Kênh sự kiện riêng của UI: bên ngoài (scene GameplayMock, sau này là gameplay thật) YÊU CẦU UI
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

        public static event Action ShopRequested;

        /// <summary>Thông báo tự ẩn góc màn hình.</summary>
        public static event Action<string> NotificationRequested;

        /// <summary>Hiện số damage world-space tại vị trí (tọa độ thế giới).</summary>
        public static event Action<Vector3, float, bool> DamageNumberRequested;

        /// <summary>Kỹ năng ở ô index trên hotbar vừa được dùng → hiện cooldown.</summary>
        public static event Action<int> SkillUsed;

        public static void RequestDialogue(IReadOnlyList<DialogueLine> lines, Action onFinished = null)
            => DialogueRequested?.Invoke(lines, onFinished);

        public static void RequestShop() => ShopRequested?.Invoke();

        public static void Notify(string message) => NotificationRequested?.Invoke(message);

        public static void ShowDamage(Vector3 worldPosition, float amount, bool isCritical = false)
            => DamageNumberRequested?.Invoke(worldPosition, amount, isCritical);

        public static void UseSkill(int hotbarIndex) => SkillUsed?.Invoke(hotbarIndex);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            DialogueRequested = null;
            ShopRequested = null;
            NotificationRequested = null;
            DamageNumberRequested = null;
            SkillUsed = null;
        }
    }
}
