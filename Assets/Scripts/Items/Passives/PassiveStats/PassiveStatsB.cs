using UnityEngine;

[CreateAssetMenu(fileName = "New Passive Stats", menuName = "Stats/Passive Stats B")]
public class PassiveStatsB : ItemStatsB
{
    public PassiveB.Modifier baseStats;
    public PassiveB.Modifier[] growth;

    public PassiveB.Modifier GetLevelData(int level)
    {
        if (level - 2 < growth.Length)
        {
            return growth[level - 2];
        }

        Debug.LogError($"The passive doesn't have any defined level data for level {level}");
        return new PassiveB.Modifier();
    }
}
