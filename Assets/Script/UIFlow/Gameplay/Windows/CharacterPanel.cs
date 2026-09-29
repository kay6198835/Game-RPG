using System.Text;
using TMPro;
using UnityEngine;

namespace UIFlow
{
    /// <summary>Trang Nhân vật: tên, class, level, exp và toàn bộ chỉ số.</summary>
    public class CharacterPanel : TabPage
    {
        [SerializeField] private TMP_Text headerText;
        [SerializeField] private TMP_Text statsText;

        private readonly StringBuilder _builder = new();

        public override string Title => "Nhân vật";

        private void OnEnable() => UIServices.Player.StatsChanged += Refresh;

        private void OnDisable() => UIServices.Player.StatsChanged -= Refresh;

        public override void OnTabShown() => Refresh(UIServices.Player.GetStats());

        private void Refresh(PlayerStatsData stats)
        {
            headerText.text = $"<size=130%><b>{stats.characterName}</b></size>\n{stats.className} · Cấp {stats.level}\n" +
                              $"<color=#AAA>EXP {stats.exp} / {stats.expToNextLevel}</color>";

            _builder.Clear();
            _builder.Append($"HP: {Mathf.CeilToInt(stats.currentHP)} / {Mathf.CeilToInt(stats.maxHP)}\n");
            _builder.Append($"Mana: {Mathf.CeilToInt(stats.currentMana)} / {Mathf.CeilToInt(stats.maxMana)}\n\n");
            foreach (StatLine line in stats.attributes)
            {
                _builder.Append($"{line.statName}<pos=60%>{line.value:0.#}\n");   // <pos> căn cột giá trị thẳng hàng
            }
            statsText.text = _builder.ToString();
        }
    }
}
