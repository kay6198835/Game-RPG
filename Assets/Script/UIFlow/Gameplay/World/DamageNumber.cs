using System;
using TMPro;
using UnityEngine;

namespace UIFlow
{
    /// <summary>
    /// Số damage bay lên rồi mờ dần. Dùng TextMeshPro (bản 3D, KHÔNG phải TextMeshProUGUI) nên không cần Canvas:
    /// chữ là một vật trong thế giới giống sprite. Rẻ hơn tạo một Canvas World Space cho mỗi con số.
    /// </summary>
    [RequireComponent(typeof(TextMeshPro))]
    public class DamageNumber : MonoBehaviour
    {
        [SerializeField] private float lifetime = 0.9f;
        [SerializeField] private float riseSpeed = 1.2f;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color criticalColor = new(1f, 0.8f, 0.2f);

        private TextMeshPro _text;
        private float _age;
        private Vector3 _drift;
        private Action<DamageNumber> _onFinished;

        private void Awake()
        {
            _text = GetComponent<TextMeshPro>();
        }

        public void Play(Vector3 position, float amount, bool isCritical, Action<DamageNumber> onFinished)
        {
            _onFinished = onFinished;
            _age = 0f;
            // Lệch ngang ngẫu nhiên để nhiều số liền nhau không chồng lên nhau.
            _drift = new Vector3(UnityEngine.Random.Range(-0.4f, 0.4f), riseSpeed, 0f);
            transform.position = position + new Vector3(0f, 0.5f, 0f);
            transform.localScale = Vector3.one * (isCritical ? 1.4f : 1f);
            _text.text = isCritical ? Mathf.RoundToInt(amount) + "!" : Mathf.RoundToInt(amount).ToString();
            _text.color = isCritical ? criticalColor : normalColor;
        }

        private void Update()
        {
            _age += Time.deltaTime;
            transform.position += _drift * Time.deltaTime;
            _drift.y = Mathf.Max(0.2f, _drift.y - riseSpeed * Time.deltaTime);   // Chậm dần

            Color color = _text.color;
            color.a = 1f - Mathf.Clamp01((_age - lifetime * 0.5f) / (lifetime * 0.5f));
            _text.color = color;

            if (_age >= lifetime) _onFinished?.Invoke(this);
        }
    }
}
