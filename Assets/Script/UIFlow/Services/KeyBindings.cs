using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace UIFlow
{
    /// <summary>
    /// Bảng gán phím, lưu bằng PlayerPrefs.
    /// - Phím của UI (tạm dừng, túi đồ, chuyển tab…) được UI đọc trực tiếp qua UIInput → đổi là có tác dụng ngay.
    /// - Phím gameplay (di chuyển, tấn công…) chỉ được lưu lại; gameplay cũ tự đọc PlayerInput.inputactions.
    ///   TODO: nối logic thật — áp phím gameplay đã lưu bằng InputAction.ApplyBindingOverride() lên PlayerInput.
    /// Project dùng Input System mới (activeInputHandler = Both) nên dùng enum Key thay cho KeyCode.
    /// </summary>
    public static class KeyBindings
    {
        public class Binding
        {
            public string id;
            public string displayName;
            public Key defaultKey;
            public bool isUiKey;

            public Key CurrentKey
            {
                get
                {
                    string saved = PlayerPrefs.GetString(PrefKey(id), string.Empty);
                    return Enum.TryParse(saved, out Key key) ? key : defaultKey;
                }
            }
        }

        public const string Pause = "pause";
        public const string Inventory = "inventory";
        public const string SkillTree = "skilltree";
        public const string QuestLog = "questlog";
        public const string TabLeft = "tab_left";
        public const string TabRight = "tab_right";
        public const string Interact = "interact";

        public static readonly List<Binding> All = new()
        {
            new() { id = Pause, displayName = "Tạm dừng / Quay lại", defaultKey = Key.Escape, isUiKey = true },
            new() { id = Inventory, displayName = "Túi đồ", defaultKey = Key.I, isUiKey = true },
            new() { id = SkillTree, displayName = "Cây kỹ năng", defaultKey = Key.K, isUiKey = true },
            new() { id = QuestLog, displayName = "Nhiệm vụ", defaultKey = Key.J, isUiKey = true },
            new() { id = TabLeft, displayName = "Tab trước", defaultKey = Key.Q, isUiKey = true },
            new() { id = TabRight, displayName = "Tab sau", defaultKey = Key.E, isUiKey = true },
            new() { id = Interact, displayName = "Tương tác", defaultKey = Key.G, isUiKey = false },
            new() { id = "move_up", displayName = "Đi lên", defaultKey = Key.W, isUiKey = false },
            new() { id = "move_down", displayName = "Đi xuống", defaultKey = Key.S, isUiKey = false },
            new() { id = "move_left", displayName = "Sang trái", defaultKey = Key.A, isUiKey = false },
            new() { id = "move_right", displayName = "Sang phải", defaultKey = Key.D, isUiKey = false },
            new() { id = "dash", displayName = "Lướt", defaultKey = Key.Space, isUiKey = false },
            new() { id = "equip", displayName = "Trang bị / Tháo", defaultKey = Key.F, isUiKey = false },
        };

        public static Binding Find(string id) => All.Find(b => b.id == id);

        public static Key Get(string id)
        {
            Binding binding = Find(id);
            return binding != null ? binding.CurrentKey : Key.None;
        }

        public static void Set(string id, Key key)
        {
            PlayerPrefs.SetString(PrefKey(id), key.ToString());
            PlayerPrefs.Save();
        }

        public static void ResetAll()
        {
            foreach (Binding binding in All) PlayerPrefs.DeleteKey(PrefKey(binding.id));
            PlayerPrefs.Save();
        }

        /// <summary>Tìm binding khác đang dùng cùng phím (để cảnh báo trùng). Null nếu không trùng.</summary>
        public static Binding FindConflict(string id, Key key)
        {
            foreach (Binding binding in All)
            {
                if (binding.id != id && binding.CurrentKey == key) return binding;
            }
            return null;
        }

        /// <summary>Tên phím dễ đọc cho người chơi, theo layout bàn phím hiện tại.</summary>
        public static string DisplayName(Key key)
        {
            if (key == Key.None) return "-";
            KeyControl control = Keyboard.current != null ? Keyboard.current[key] : null;
            return control != null ? control.displayName : key.ToString();
        }

        private static string PrefKey(string id) => "ui.key." + id;
    }
}
