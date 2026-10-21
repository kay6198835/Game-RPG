using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIFlow
{
    /// <summary>
    /// Trang Nhân vật: tên, class, cấp, máu/mana, bảng chỉ số và cộng điểm level-up (thay UI stats cũ
    /// StatsUIController của LoadRandomMap).
    /// Mỗi lần trang hiện lên là một phiên cộng điểm: −/+ từng điểm, Hoàn tác trả hết, Xác nhận chốt lại,
    /// Reset trả mọi điểm đã chốt. Rời trang mà chưa Xác nhận → tự Hoàn tác (giống UI cũ khi đóng bảng).
    /// </summary>
    public class CharacterPanel : TabPage
    {
        [SerializeField] private TMP_Text headerText;
        [SerializeField] private TMP_Text vitalsText;
        [SerializeField] private TMP_Text pointsText;
        [SerializeField] private Transform primaryContainer;
        [SerializeField] private Transform derivedContainer;
        [SerializeField] private StatRowView primaryRowTemplate;
        [SerializeField] private StatRowView derivedRowTemplate;
        [SerializeField] private Button acceptButton;
        [SerializeField] private Button revertButton;
        [SerializeField] private Button respecButton;

        private readonly Dictionary<StatType, StatRowView> _rows = new();

        public override string Title => "Nhân vật";

        private void Awake()
        {
            primaryRowTemplate.gameObject.SetActive(false);
            derivedRowTemplate.gameObject.SetActive(false);
            acceptButton.onClick.AddListener(Accept);
            revertButton.onClick.AddListener(Revert);
            respecButton.onClick.AddListener(Respec);
        }

        private void OnEnable() => UIServices.Player.StatsChanged += Refresh;

        private void OnDisable() => UIServices.Player.StatsChanged -= Refresh;

        public override void OnTabShown()
        {
            UIServices.Player.BeginStatSession();
            Refresh(UIServices.Player.GetStats());
        }

        public override void OnTabHidden() => UIServices.Player.RevertStats();

        private void OnStatChange(StatType statType, int delta) => UIServices.Player.TryChangeStat(statType, delta);

        private void Accept() => UIServices.Player.AcceptStats();

        private void Revert() => UIServices.Player.RevertStats();

        private void Respec() => UIServices.Player.RespecStats();

        private void Refresh(PlayerStatsData stats)
        {
            IPlayerDataProvider player = UIServices.Player;

            headerText.text = $"<size=130%><b>{stats.characterName}</b></size>\n{stats.className} · Cấp {stats.level}";
            vitalsText.text = $"HP: {Mathf.CeilToInt(stats.currentHP)} / {Mathf.CeilToInt(stats.maxHP)}\n" +
                              $"Mana: {Mathf.CeilToInt(stats.currentMana)} / {Mathf.CeilToInt(stats.maxMana)}";

            int remaining = player.UnspentStatPoints;
            bool showSteps = player.StatPointsAtOpen > 0;
            pointsText.text = showSteps ? $"Điểm còn lại: <color=#F5B333><b>{remaining}</b></color>" : "Không có điểm để cộng";

            foreach (StatRowData row in player.GetStatRows())
            {
                GetOrCreateRow(row).Bind(row, showSteps, remaining > 0);
            }

            bool changed = remaining != player.StatPointsAtOpen;
            acceptButton.gameObject.SetActive(changed);
            revertButton.gameObject.SetActive(changed);
            respecButton.gameObject.SetActive(player.CanRespec);
        }

        private StatRowView GetOrCreateRow(StatRowData row)
        {
            if (_rows.TryGetValue(row.statType, out StatRowView view)) return view;

            StatRowView template = row.isPrimary ? primaryRowTemplate : derivedRowTemplate;
            view = Instantiate(template, row.isPrimary ? primaryContainer : derivedContainer);
            view.gameObject.SetActive(true);
            view.Init(row.statType, OnStatChange);
            _rows[row.statType] = view;
            return view;
        }
    }
}
