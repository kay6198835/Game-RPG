#if UNITY_EDITOR
using UnityEngine;

namespace QA.Automation
{
    // The runtime player and the components the suites read. The player is spawned by PlayerManager
    // (resolver.Instantiate) on first access to PlayerManager.Player, exactly as in the game.
    public sealed class QAPlayerRig
    {
        public Player Player { get; private set; }
        public PlayerInputHandler Input { get; private set; }
        public WeaponHolder WeaponHolder { get; private set; }
        public AbilityHolder AbilityHolder { get; private set; }
        public StatHandler Stats { get; private set; }
        public VitalStatsComponent Vital { get; private set; }
        public INegativeReceiver Hurtbox { get; private set; }

        public bool Ready => Player != null && Stats != null && Vital != null;

        public void Bind(Player player)
        {
            Player = player;
            Input = player.GetComponentInChildren<PlayerInputHandler>(true);
            WeaponHolder = player.GetComponentInChildren<WeaponHolder>(true);
            AbilityHolder = player.GetComponentInChildren<AbilityHolder>(true);
            Stats = player.GetComponentInChildren<StatHandler>(true);
            Vital = player.GetComponentInChildren<VitalStatsComponent>(true);
            Hurtbox = player.GetComponentInChildren<NegativeReciver>(true);
        }

        public float Hp => Vital.GetCurrentStatValue(VitalStatType.HP);
        public float Mana => Vital.GetCurrentStatValue(VitalStatType.Mana);
        public float MaxHp => Stats.GetStatValue(StatType.HP);
        public float MaxMana => Stats.GetStatValue(StatType.Mana);

        public string MissingComponents() =>
            $"input={Input != null} weapon={WeaponHolder != null} ability={AbilityHolder != null} stats={Stats != null} vital={Vital != null} hurtbox={Hurtbox != null}";
    }
}
#endif
