using UIFlow;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Brings the in-game UI into the CombatTest scene and offers a hotkey class switch that bypasses the
// Champion confirm panel. GameplayUI is required, not optional: UIEvents.OnOpenConfirmPanel invokes
// its event unguarded (BUG-102), so interacting with a monument without it throws.
public class CombatTestBootstrap : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private bool loadGameplayUI = true;
    [Tooltip("F1, F2, F3 ... pick the class at the same index.")]
    [SerializeField] private PlayerData[] classes;

    private static readonly Key[] ClassKeys = { Key.F1, Key.F2, Key.F3, Key.F4, Key.F5 };

    private void Start()
    {
        if (loadGameplayUI && !SceneManager.GetSceneByName(SceneNames.GameplayUI).isLoaded)
            SceneManager.LoadScene(SceneNames.GameplayUI, LoadSceneMode.Additive);
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null || playerManager == null || classes == null) return;

        for (int i = 0; i < classes.Length && i < ClassKeys.Length; i++)
        {
            if (classes[i] == null || !keyboard[ClassKeys[i]].wasPressedThisFrame) continue;
            playerManager.Player.SetCharacterData(classes[i]);
            Debug.Log($"[CombatTest] Class -> {classes[i].name}");
        }
    }
}
