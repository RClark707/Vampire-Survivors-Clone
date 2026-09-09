using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemStats : ScriptableObject
{
    public new string name;
    public Sprite icon;
    [SerializeField]
    int level = 1; // apply certain modifiers when the Level changes?
    public int Level { get => level; set => level = value; }
    public List<LevelUpUpgrades> upgrades;

    public enum PlayerStats { None, Health, Speed, Might, Armor, Recovery, Area, Magnet, Growth, Luck };
    public enum WeaponStats { None, Damage, Cooldown, Projectile_Speed, Pierce_Count };

    [Serializable]
    public class LevelUpUpgrades
    {
        public string description;
        public List<Upgrade> statUpgrades;
        [Serializable]
        public class Upgrade
        {
            public PlayerStats playerStatUpgrade;
            public WeaponStats weaponStatUpgrade;
            public float multiplier;
        }
    }
}
