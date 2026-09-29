using System.Collections.Generic;
using UnityEngine;

namespace UIFlow
{
    /// <summary>
    /// Tạo số damage khi có UIEvents.ShowDamage. Dùng pool (Queue) để không Instantiate/Destroy
    /// mỗi lần trúng đòn — trận đánh đông quái có thể tạo hàng chục số mỗi giây.
    /// Pool riêng của UI, không dùng ObjectPoolManager của gameplay để hai bên độc lập.
    /// </summary>
    public class DamageNumberSpawner : MonoBehaviour
    {
        [SerializeField] private DamageNumber prefab;
        [SerializeField] private int prewarm = 10;

        private readonly Queue<DamageNumber> _pool = new();

        private void Awake()
        {
            for (int i = 0; i < prewarm; i++) Release(Create());
        }

        private void OnEnable() => UIEvents.DamageNumberRequested += Spawn;

        private void OnDisable() => UIEvents.DamageNumberRequested -= Spawn;

        private void Spawn(Vector3 position, float amount, bool isCritical)
        {
            DamageNumber number = _pool.Count > 0 ? _pool.Dequeue() : Create();
            number.gameObject.SetActive(true);
            number.Play(position, amount, isCritical, Release);
        }

        private DamageNumber Create() => Instantiate(prefab, transform);

        private void Release(DamageNumber number)
        {
            number.gameObject.SetActive(false);
            _pool.Enqueue(number);
        }
    }
}
