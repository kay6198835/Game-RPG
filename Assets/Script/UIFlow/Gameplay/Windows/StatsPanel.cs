using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIFlow
{
    /// <summary>
    /// Cửa sổ chỉ số + cộng điểm level-up (phím C / nút "Chỉ số" trên HUD). Bố cục theo UI stats cũ của LoadRandomMap
    /// (StatsUIController): cột chỉ số chính có −/+, cột chỉ số phụ, footer Hoàn tác / Reset điểm / Xác nhận.
    /// Mỗi lần mở là một phiên cộng điểm; đóng mà chưa Xác nhận → tự Hoàn tác (giống nút Close của UI cũ).
    /// Prefab: Assets/Prefab/UIFlow/StatsPanel.prefab (UIFlowBuilder tạo một lần, chỉnh tay được).
    /// </summary>
    public class StatsPanel : UIPanel
    {
        [SerializeField] private TMP_Text pointsText;
        [SerializeField] private Transform primaryContainer;
        [SerializeField] private Transform derivedContainer;
        [SerializeField] private StatRowView primaryRowTemplate;
        [SerializeField] private StatRowView derivedRowTemplate;
        [SerializeField] private Button acceptButton;
        [SerializeField] private Button revertButton;
        [SerializeField] private Button respecButton;
        [SerializeField] private Button closeButton;

        private readonly Dictionary<StatType, StatRowView> _rows = new();

        protected override void Awake()
        {
            base.Awake();
            primaryRowTemplate.gameObject.SetActive(false);
            derivedRowTemplate.gameObject.SetActive(false);
            acceptButton.onClick.AddListener(() => UIServices.Player.AcceptStats());
            revertButton.onClick.AddListener(() => UIServices.Player.RevertStats());
            respecButton.onClick.AddListener(() => UIServices.Player.RespecStats());
            closeButton.onClick.AddListener(CloseSelf);
        }

        private void OnEnable() => UIServices.Player.StatsChanged += OnStatsChanged;

        private void OnDisable() => UIServices.Player.StatsChanged -= OnStatsChanged;

        protected override void OnShown()
        {
            UIServices.Player.BeginStatSession();
            Refresh();
        }

        protected override void OnHidden() => UIServices.Player.RevertStats();

        private void OnStatsChanged(PlayerStatsData _)
        {
            if (IsVisible) Refresh();
        }

        private void OnStatChange(StatType statType, int delta) => UIServices.Player.TryChangeStat(statType, delta);

        private void Refresh()
        {
            IPlayerDataProvider player = UIServices.Player;

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
