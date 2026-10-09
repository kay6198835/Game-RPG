using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIFlow
{
    public class ConfirnPanel : UIPanel
    {
        [SerializeField] private TMP_Text tileText;
        [SerializeField] private TMP_Text messText;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        private readonly StringBuilder _builder = new();

        public void SetConfirm(ConfirnData data)
        {
            tileText.text = data.confirmText;
            messText.text = data.cancelText;
        }

        public void AccessConfirm()
        {
            CloseSelf();
            UIEvents.AccessAction();
        }

        protected override void Awake()
        {
            base.Awake();
            confirmButton.onClick.AddListener(AccessConfirm);
            cancelButton.onClick.AddListener(CloseSelf);
        }

    }
}

public struct ConfirnData
{
    public string confirmText;
    public string cancelText;
}