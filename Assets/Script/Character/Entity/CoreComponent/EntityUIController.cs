using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class EntityUIController : EntityCoreComponent<EntityCore>
{
    [SerializeField] private Slider slider;
    private EntityVitalStats entityVitalStats;
    private EntityStatsHandler entityStatsHandler;

    protected override void Start()
    {
        base.Start();
        Core.GetCoreComponent(out entityVitalStats);
        Core.GetCoreComponent(out entityStatsHandler);
    }

    public void UpdateUIHealth(float value)
    {
        slider.value = value;
    }
}