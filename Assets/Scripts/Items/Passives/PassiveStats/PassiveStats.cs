using UnityEngine;

[CreateAssetMenu(fileName = "New Passive Stats", menuName = "Stats/Passive Stats B")]
public class PassiveStats : ItemStats
{
    public Passive.Modifier baseStats;
    public Passive.Modifier[] growth;

    public override Item.LevelData GetLevelData(int level)
    {
        if (level <= 1) return baseStats;

        if (level - 2 < growth.Length)
        {
            return growth[level - 2];
        }

        Debug.LogError($"The passive doesn't have any defined level data for level {level}");
        return new Passive.Modifier();
    }
}
