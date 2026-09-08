using System.Collections.Generic;
using UnityEngine;

public class ItemStats : ScriptableObject
{
    public new string name;
    public Sprite icon;
    [SerializeField]
    int level = 1; // apply certain modifiers when the Level changes?
    public int Level { get => level; set => level = value; }
    public List<Upgrade> upgrades;

    public enum PlayerStats { Health, Speed, Might, Armor, Recovery, Area, Magnet, Growth, Luck };
    public enum WeaponStats { Damage, Cooldown, Projectile_Speed, Pierce_Count };
    public class Upgrade
    {
        public string description;
        public PlayerStats playerUpgrade; // we *should* be able to apply increases to multiple player stats with a single upgrade
        public WeaponStats weaponUpgrade; // we *should* be able to apply increases to multiple weapon stats with a single upgrade
        public float multiplier;
    }
}
