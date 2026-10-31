#if UNITY_EDITOR
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace QA.Automation
{
    // Copied from Assets/Script/FastTest/QAProbe/QAInputDriver.cs. Drives the player through the real
    // Input System bindings with two virtual devices, so every case goes through PlayerInputHandler
    // exactly like a human press. Aim is the exception: PlayerInputHandler reads the legacy
    // Input.mousePosition, which a virtual device cannot move, so the aim fields are written directly
    // every frame while a case holds an aim point.
    public sealed class QAInputDriver
    {
        private const BindingFlags Instance = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        private readonly Keyboard _keyboard;
        private readonly Mouse _mouse;
        private readonly InputSettings _originalSettings;
        private readonly FieldInfo _mouseVector;
        private readonly FieldInfo _directionMouseVector;
        private readonly FieldInfo _angleRotationPlayer;

        private PlayerInputHandler _handler;
        private Vector2? _aimPoint;
        private Key _heldKey = Key.None;

        public QAInputDriver()
        {
            // In Play Mode the Editor only routes keyboard input to a focused Game view; a CLI run has no focus.
            _originalSettings = InputSystem.settings;
            InputSettings settings = Object.Instantiate(_originalSettings);
            settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            InputSystem.settings = settings;

            _keyboard = InputSystem.AddDevice<Keyboard>("QAKeyboard");
            _mouse = InputSystem.AddDevice<Mouse>("QAMouse");

            System.Type type = typeof(PlayerInputHandler);
            _mouseVector = type.GetField("mouseVector", Instance);
            _directionMouseVector = type.GetField("directionMouseVector", Instance);
            _angleRotationPlayer = type.GetField("angleRotationPlayer", Instance);
        }

        public void Bind(PlayerInputHandler handler) => _handler = handler;

        public void AimAt(Vector2 worldPoint) => _aimPoint = worldPoint;

        public void ClearAim() => _aimPoint = null;

        // Called every frame by the suite, after input events are processed and before the player's states tick.
        public void ApplyAim()
        {
            if (_handler == null || _aimPoint == null) return;
            Vector2 point = _aimPoint.Value;
            Vector2 direction = (point - (Vector2)_handler.transform.position).normalized;
            _mouseVector?.SetValue(_handler, point);
            _directionMouseVector?.SetValue(_handler, direction);
            _handler.AngleCalculateMouse(direction);
            float rotation = Vector2.SignedAngle(_handler.transform.right, direction);
            _angleRotationPlayer?.SetValue(_handler, (rotation + 360f) % 360f);
        }

        public void KeyDown(Key key)
        {
            _heldKey = key;
            InputSystem.QueueStateEvent(_keyboard, new KeyboardState(key));
        }

        public void KeyUp()
        {
            _heldKey = Key.None;
            InputSystem.QueueStateEvent(_keyboard, new KeyboardState());
        }

        public void MouseDown() => InputSystem.QueueStateEvent(_mouse, new MouseState { buttons = 1 });

        public void MouseUp() => InputSystem.QueueStateEvent(_mouse, new MouseState());

        public bool IsHolding => _heldKey != Key.None;

        public void Dispose()
        {
            if (_keyboard != null) InputSystem.RemoveDevice(_keyboard);
            if (_mouse != null) InputSystem.RemoveDevice(_mouse);
            InputSystem.settings = _originalSettings;
        }
    }
}
#endif
