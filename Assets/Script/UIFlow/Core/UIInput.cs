using UnityEngine.InputSystem;

namespace UIFlow
{
    /// <summary>
    /// Đọc phím cho UI bằng Input System mới (project đang dùng nó, không dùng Input.GetKeyDown).
    /// Mọi chỗ đều kiểm tra Keyboard.current != null vì máy không có bàn phím sẽ trả về null.
    /// </summary>
    public static class UIInput
    {
        /// <summary>Phím của hành động actionId (xem KeyBindings) vừa được nhấn ở frame này.</summary>
        public static bool WasPressed(string actionId)
        {
            return WasPressed(KeyBindings.Get(actionId));
        }

        public static bool WasPressed(Key key)
        {
            if (key == Key.None || Keyboard.current == null) return false;
            return Keyboard.current[key].wasPressedThisFrame;
        }

        /// <summary>Phím bất kỳ vừa được nhấn — dùng cho màn gán phím. Trả về Key.None nếu không có.</summary>
        public static Key AnyKeyPressedThisFrame()
        {
            if (Keyboard.current == null || !Keyboard.current.anyKey.wasPressedThisFrame) return Key.None;

            foreach (var control in Keyboard.current.allKeys)
            {
                if (control != null && control.wasPressedThisFrame) return control.keyCode;
            }
            return Key.None;
        }

        /// <summary>Chuột trái / Space / Enter — dùng để tua nhanh hộp thoại, bỏ qua logo.</summary>
        public static bool ConfirmPressed()
        {
            bool mouse = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
            bool keys = Keyboard.current != null &&
                        (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame);
            return mouse || keys;
        }
    }
}
