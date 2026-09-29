using System;
using UnityEngine;

public class PassiveB : ItemB
{
    public PassiveStatsB statsData;
    [SerializeField] CharacterStatsB.Stats currentBoosts;

    [Serializable]
    public class Modifier : LevelData
    {
        public CharacterStatsB.Stats boosts;
    }

    public virtual void Initialize(PassiveStatsB stats)
    {
        base.Initialize(stats);
        this.statsData = stats;
        currentBoosts = stats.baseStats.boosts;
    }

    public virtual CharacterStatsB.Stats GetBoosts()
    {
        return currentBoosts;
    }

    public override bool LevelUp()
    {
        // base.LevelUp();

        if (!CanLevelUp())
        {
            Debug.LogWarning($"Cannot level up your {name} to level {currentLevel + 1}. It has already reached the max level of {maxLevel}");
            return false;
        }

        currentBoosts += ((Modifier)statsData.GetLevelData(++currentLevel)).boosts;
        return true;
    }
}
