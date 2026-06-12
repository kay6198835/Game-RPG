using UnityEngine;

namespace Game.Stats
{
    /// <summary>
    /// Demo cách dùng: gắn vào GameObject có CharacterStats rồi xem Console.
    /// Trong game thật, logic này nằm ở EquipmentManager / BuffSystem.
    /// </summary>
    public class StatSystemDemo : MonoBehaviour
    {
        [SerializeField] private CharacterStats stats;

        // Thực tế Source sẽ là instance Item/Buff — ở đây dùng object trống cho gọn.
        private readonly object ironSword = new object();

        private void Start()
        {
            if (stats == null) stats = GetComponent<CharacterStats>();

            stats.OnStatChanged += type =>
                Debug.Log($"[Stat đổi] {type} = {stats.Get(type):0.##}");

            Debug.Log($"PhysDamage ban đầu: {stats.Get(StatType.PhysicalDamage):0.##}");

            // ----- Trang bị "Kiếm sắt": +5 STR, +12 PhysDamage, +10% AttackSpeed -----
            stats.AddModifier(StatType.STR,            new StatModifier(5f,    ModifierType.Flat,       ironSword));
            stats.AddModifier(StatType.PhysicalDamage, new StatModifier(12f,   ModifierType.Flat,       ironSword));
            stats.AddModifier(StatType.AttackSpeed,    new StatModifier(0.10f, ModifierType.PercentAdd, ironSword));

            Debug.Log($"PhysDamage sau khi cầm kiếm: {stats.Get(StatType.PhysicalDamage):0.##}");

            // ----- Lên level + cộng điểm: derived stats tự cập nhật -----
            stats.Level += 1;
            stats.AddPrimaryPoint(StatType.STR, 2f);

            // ----- Tháo kiếm: mọi modifier từ nguồn này tự gỡ sạch trên mọi stat -----
            stats.RemoveModifiersFromSource(ironSword);
            Debug.Log($"PhysDamage sau khi tháo kiếm: {stats.Get(StatType.PhysicalDamage):0.##}");
        }
    }
}
