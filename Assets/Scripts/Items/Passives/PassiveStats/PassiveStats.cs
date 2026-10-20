using UnityEngine;

[CreateAssetMenu(fileName = "New Passive Stats", menuName = "Stats/Passive Stats")]
public class PassiveStats : ItemStats
{
    [Header("Initial Stats")]
    public Passive.Modifier baseStats;

    [Header("All Future Stats")]
    [Tooltip("If true, you will always recieve only the modifiers from the base stats when this item levels up.")]
    public bool alwaysRepeatBaseModifier = true;
    public Passive.Modifier[] growth;

    public override Item.LevelData GetLevelData(int level)
    {
        if (level <= 1 || alwaysRepeatBaseModifier) return baseStats;

        if (level - 2 < growth.Length)
        {
            return growth[level - 2];
        }

        Debug.LogError($"The passive doesn't have any defined level data for level {level}");
        return new Passive.Modifier();
    }
}
