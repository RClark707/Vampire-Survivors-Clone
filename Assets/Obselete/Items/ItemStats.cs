using System;
using System.Collections.Generic;
using UnityEngine;

[Obsolete("This is replaced by ItemStatsB")]
public class ItemStats : ScriptableObject
{
    public new string name;
    public Sprite icon;

    [SerializeField]
    string summary; // this is the item's description of what it does, like "High Damage, High Area Scaling"
    public string Summary { get => summary; set => summary = value; }

    [SerializeField]
    int level = 1; // this doesn't actually get changed
    public int Level { get => level; set => level = value; }
    public int maxLevel = 1;

    public List<LevelUpUpgrades> upgrades;

    public enum PlayerUpgradeStats { None, Health, Speed, Might, Armor, Recovery, Area, Magnet, Growth, Luck, ProjectileSpeed };
    public enum WeaponUpgradeStats { None, Damage, Cooldown, Projectile_Speed, Pierce_Count };

    [Serializable]
    public class LevelUpUpgrades
    {
        public string description;
        public List<Upgrade> statUpgrades;

        [Serializable]
        public class Upgrade
        {
            public PlayerUpgradeStats playerStatUpgrade;
            public WeaponUpgradeStats weaponStatUpgrade;
            public float multiplier;
        }
    }
}
