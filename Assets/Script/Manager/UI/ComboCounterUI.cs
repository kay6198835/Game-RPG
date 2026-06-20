using TMPro;
using UnityEngine;

public class ComboCounterUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private string format = "{0} HIT";
    [SerializeField] private float hideDelay = 1.5f;

    private void OnEnable()
    {
        EventManager.Resgister(EventID.ON_COMBO_HIT, OnComboHit);
        SetVisible(false);
    }

    private void OnDisable()
    {
        EventManager.UnResgister(EventID.ON_COMBO_HIT, OnComboHit);
        CancelInvoke(nameof(Hide));
    }

    private void OnComboHit(object payload)
    {
        int count = (int)payload;
        if (count < 2)
        {
            return;
        }
        comboText.text = string.Format(format, count);
        SetVisible(true);
        CancelInvoke(nameof(Hide));
        Invoke(nameof(Hide), hideDelay);
    }

    private void Hide()
    {
        SetVisible(false);
    }

    private void SetVisible(bool visible)
    {
        if (canvasGroup == null)
        {
            return;
        }
        canvasGroup.alpha = visible ? 1f : 0f;
    }
}
