using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
[CreateAssetMenu(menuName = "Ability SO/Skill Talent SO")]
public class InternalSkillSO : AbstractSkillSO
{
    [SerializeField] private List<UppgradeData> uppgradeData = new List<UppgradeData>();
    [TextArea(1, 4)][SerializeField] private string skillDescription;
    [SerializeField] private Sprite skillIcon;
    [SerializeField] private List<InternalSkillSO> skillPrerequisites = new List<InternalSkillSO>();
    [SerializeField] private int skillTier;
    [SerializeField] private int cost;

    public List<UppgradeData> UppgradeData { get => uppgradeData; }
    public Sprite SkillIcon { get => skillIcon; }
    public List<InternalSkillSO> SkillPrerequisites { get => skillPrerequisites; }
    public int SkillTier { get => skillTier; }
    public int Cost { get => cost; }

    protected override void OnValidate()
    {
        skillName = name;
        if (uppgradeData.Count == 0 || overWriteDescription) return;
        GenerateDescription();
    }
    protected override void GenerateDescription()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append($"{skillName} increases ");
        for (int i = 0; i < uppgradeData.Count; i++)
        {
            stringBuilder.Append(uppgradeData[i].statsTypes.ToString());
            stringBuilder.Append(" by ");
            stringBuilder.Append(uppgradeData[i].skillIncreaseAmount.ToString());
            stringBuilder.Append(uppgradeData[i].isPercentage ? "%" : " points(s)");
            if (i == uppgradeData.Count - 2) stringBuilder.Append(" and ");
            else stringBuilder.Append(i < uppgradeData.Count - 1 ? ", " : ". ");
        }
    }
}
public class UppgradeData
{
    public StatsTypes statsTypes;
    public int skillIncreaseAmount;
    public bool isPercentage;
}
public enum StatsTypes
{
    Strength,
    Dexterity,
    Intelligence,
    Charisma,
}