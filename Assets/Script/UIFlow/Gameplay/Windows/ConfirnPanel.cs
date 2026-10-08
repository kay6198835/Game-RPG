using System.Text;
using TMPro;
using UnityEngine;

namespace UIFlow
{
    public class ConfirnPanel : UIPanel
    {
        [SerializeField] private TMP_Text tileText;
        [SerializeField] private TMP_Text messText;

        private readonly StringBuilder _builder = new();

        public void AccessConfirm()
        {
            UIEvents.AccessAction();
        }

    }
}