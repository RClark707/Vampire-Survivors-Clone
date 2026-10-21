using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Stats B", menuName = "Stats/Weapon Stats")]
public class WeaponStats : ItemStats
{
    [HideInInspector]
    public string behavior;
    public Weapon.Stats baseStats = new Weapon.Stats
    {

        duration = 0.5f,
        damage = 10f,
        damageVariance = 0f,
        area = 1f,
        projectileSpeed = 1f,
        cooldown = 1f,
        amount = 1,
        piercing = 1,
        projectileInterval = 0.1f,
        knockback = 1f,
    };

    public Weapon.Stats[] linearGrowth; // for level ups
    public Weapon.Stats[] randomGrowth; // for endless play

    public override Item.LevelData GetLevelData(int level)
    {
        if (level <= 1) return baseStats;

        // do we have a level defined here?
        if (level - 2 < linearGrowth.Length)
        {
            return linearGrowth[level - 2];
        }

        if (randomGrowth.Length > 0)
        {
            return randomGrowth[UnityEngine.Random.Range(0, randomGrowth.Length)];
        }

        Debug.LogError($"The weapon doesn't have any defined level data for level {level}");
        return new Weapon.Stats();
    }
}
