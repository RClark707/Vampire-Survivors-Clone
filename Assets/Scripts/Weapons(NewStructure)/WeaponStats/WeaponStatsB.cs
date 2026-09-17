using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Stats B", menuName = "Stats/Weapon Stats B")]
public class WeaponStatsB : ScriptableObject
{
    public Sprite icon;
    public int maxLevel;

    //public enum Behavior { None, AuraWeapon, AxeWeapon, LightningWeapon, ProjectileWeapon, WhipWeapon }
    // public Behavior behavior;
    [HideInInspector]
    public string behavior;
    public WeaponB.Stats baseStats;
    public WeaponB.Stats[] linearGrowth; // for level ups
    public WeaponB.Stats[] randomGrowth; // for endless play

    public WeaponB.Stats GetLevelData(int level)
    {
        // do we have a level defined here?
        if (level - 2 < linearGrowth.Length)
        {
            return linearGrowth[level - 2];
        }

        if (randomGrowth.Length > 0)
        {
            return randomGrowth[Random.Range(0, randomGrowth.Length)];
        }

        Debug.LogError($"The weapon doesn't have any defined level data for level {level}");
        return new WeaponB.Stats();
    }
}
