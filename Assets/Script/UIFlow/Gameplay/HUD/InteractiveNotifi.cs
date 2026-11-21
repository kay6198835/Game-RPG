using TMPro;
using UnityEngine;

namespace UIFlow
{
    [System.Serializable]
    public class InteractData
    {
        public string textMessage;
        public KeyCode keyCode;
    }
    public class InteractiveNotifi : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMessage;
        [SerializeField] private TextMeshProUGUI textButton;
        void Start()
        {
            gameObject.SetActive(false);
        }
        public void Switch(InteractData data)
        {
            if (data != null)
            {
                gameObject.SetActive(true);
                textMessage.text = data.textMessage;
                textButton.text = data.keyCode.ToString();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}