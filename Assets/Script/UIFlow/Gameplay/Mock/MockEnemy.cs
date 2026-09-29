using System.Collections;
using UnityEngine;

namespace UIFlow
{
    /// <summary>
    /// Quái giả trong scene GameplayMock, chỉ để thử thanh máu world-space và số damage.
    /// Không dùng Entity / EntityVitalStats của gameplay. Chết thì 2 giây sau tự hồi đầy máu.
    /// TODO: nối logic thật — quái thật đã có EntityUIController; thanh máu này chỉ dùng cho mock.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class MockEnemy : MonoBehaviour
    {
        [SerializeField] private float maxHP = 120f;
        [SerializeField] private WorldHealthBar healthBar;
        [SerializeField] private SpriteRenderer body;

        private float _hp;
        private bool _dead;

        private void Start()
        {
            _hp = maxHP;
            healthBar.Follow(transform);
            healthBar.SetRatio(1f);
        }

        public void Hit(float amount, bool isCritical)
        {
            if (_dead) return;
            _hp = Mathf.Max(0f, _hp - amount);
            healthBar.SetRatio(_hp / maxHP);
            UIEvents.ShowDamage(transform.position, amount, isCritical);

            if (_hp > 0f) return;
            _dead = true;
            UIEvents.Notify($"{name} bị hạ! +120 EXP");
            if (UIServices.Player is MockPlayerDataProvider player) player.DebugAddExp(120);
            StartCoroutine(Revive());
        }

        private IEnumerator Revive()
        {
            body.color = new Color(1f, 1f, 1f, 0.25f);
            yield return new WaitForSeconds(2f);
            _hp = maxHP;
            _dead = false;
            body.color = Color.white;
            healthBar.SetRatio(1f);
        }
    }
}
