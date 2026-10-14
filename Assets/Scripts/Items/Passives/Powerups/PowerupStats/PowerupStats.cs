using UnityEngine;

[CreateAssetMenu(fileName = "New Powerup", menuName = "Stats/Powerup Stats")]
public class PowerupStats : PassiveStats
{
    [Header("Powerup Price Settings")]
    [Min(0)] public float baseCost = 200f;
    [Min(0)] public float baseFee = 20f;
    [Min(0)] public float feeFactor = 1.1f;

    public virtual float GetCost(int level)
    {
        return GetBaseCost(level) + GetFeeCost(level);
    }

    public virtual float GetBaseCost(int level)
    {
        if (level < 0) return 0f;
        return Mathf.Floor(baseCost * (1 + level));
    }

    public virtual float GetFeeCost(int level)
    {
        if (level <= 0) return 0f;
        return Mathf.Floor(baseFee * Mathf.Pow(feeFactor, level));
    }

    public override Item.LevelData GetLevelData(int level)
    {
        if (level <= 1) return baseStats;

        if (level - 2 < growth.Length)
        {
            return growth[level - 2];
        }

        Debug.LogWarning($"This powerup doesn't have level up stats configured for level {level}.");
        return new Passive.Modifier();
    }

}
