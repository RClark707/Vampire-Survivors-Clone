using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public CharacterStats characterStats;

    // These are the Player's Current Stat Values, NOT the base stats
    GameObject weapon;
    float maxHealth;
    float curHealth;
    float movementSpeed;
    float recovery;
    float armor;
    float might;
    float projectileSpeed;
    float area;
    float magnet;
    float growth;
    float luck;

    [Header("Experience & Leveling")]
    float xp = 0;
    float totalXP = 0;
    float nextLevelXPRequirement;
    int level = 1;

    [System.Serializable]
    public class XPRequirement
    {
        public int minLevel;
        public float nextLevelXPRequirement;
    }

    public List<XPRequirement> xpRequirements;

    private void Awake()
    {
        name = characterStats.name;
        weapon = characterStats.StartingWeapon;
        maxHealth = characterStats.MaxHealth;
        movementSpeed = characterStats.MovementSpeed;
        recovery = characterStats.Recovery;
        armor = characterStats.Armor;
        might = characterStats.Might;
        projectileSpeed = characterStats.ProjectileSpeed;
        area = characterStats.Area;
        magnet = characterStats.Magnet;
        growth = characterStats.Growth;
        luck = characterStats.Luck;
    }

    private void Start()
    {
        nextLevelXPRequirement = xpRequirements[0].nextLevelXPRequirement;
    }

    public void GainXP(float amount)
    {
        xp += amount * growth;
        totalXP += amount * growth;

        CheckXP();
    }

    void CheckXP()
    {
        if (xp >= nextLevelXPRequirement)
        {
            xp -= nextLevelXPRequirement;
            level++;
            foreach (XPRequirement xpr in xpRequirements)
            {
                if (level < xpr.minLevel)
                {
                    break;
                }
                else
                {
                    nextLevelXPRequirement = xpr.nextLevelXPRequirement;
                }
            }
        }
    }
}
