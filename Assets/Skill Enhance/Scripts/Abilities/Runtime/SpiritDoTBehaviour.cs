using System.Collections;
using UnityEngine;

public class SpiritDoTBehaviour : MonoBehaviour
{
    private Health _health;
    private float _damagePerTick;
    private float _duration;
    private GameObject _summonPrefab;

    public void Initialize(float damagePerTick, float duration, GameObject summonPrefab)
    {
        _damagePerTick = damagePerTick;
        _duration = duration;
        _summonPrefab = summonPrefab;

        _health = GetComponent<Health>();
        if (_health == null)
        {
            Destroy(this);
            return;
        }

        StartCoroutine(DoTRoutine());
    }

    private IEnumerator DoTRoutine()
    {
        float elapsed = 0f;

        while (elapsed < _duration)
        {
            yield return new WaitForSeconds(1f);
            elapsed += 1f;

            if (_health.IsDead)
            {
                // mục tiêu chết từ nguồn khác trong khoảng tick
                TrySummon();
                break;
            }

            _health.TakeDamage(_damagePerTick);

            if (_health.IsDead)
            {
                // mục tiêu chết do tick này
                TrySummon();
                break;
            }
        }

        Destroy(this);
    }

    private void TrySummon()
    {
        if (_summonPrefab == null)
            return;

        Instantiate(_summonPrefab, transform.position, Quaternion.identity);
    }
}
