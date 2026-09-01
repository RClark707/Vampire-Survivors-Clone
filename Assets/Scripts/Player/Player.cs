using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // These are the Player's Current Stat Values, NOT the base stats
    [Header("Character Stats")]
    public CharacterStats characterStats;
    GameObject weapon;
    [HideInInspector]
    public float maxHealth { get; private set; }
    [HideInInspector]
    public float health { get; private set; }
    [HideInInspector]
    public float movementSpeed { get; private set; }// this needs to be passed to the movement script
    [HideInInspector]
    public float recovery { get; private set; }
    [HideInInspector]
    public float armor { get; private set; }
    [HideInInspector]
    public float might { get; private set; }
    [HideInInspector]
    public float projectileSpeed { get; private set; }
    [HideInInspector]
    public float area { get; private set; }
    [HideInInspector]
    public float magnet { get; private set; }
    [HideInInspector]
    public float growth { get; private set; }
    [HideInInspector]
    public float luck { get; private set; } // adjust all stats like this?

    [Header("Experience & Leveling")]
    public List<XPRequirement> xpRequirements;

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

    // Invincibility Frames
    [Header("I-Frames")]
    public float invincibilityDuration;
    float invincibilityTimer;
    bool isInvincible;

    private void Awake()
    {
        name = characterStats.name;
        weapon = characterStats.StartingWeapon;
        maxHealth = characterStats.MaxHealth;
        health = maxHealth;
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

    private void Update()
    {
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
        else if (isInvincible)
        {
            isInvincible = false;
        }
    }

    public void RestoreHealth(float healing)
    {
        health = Mathf.Min(health + healing, maxHealth);

        Debug.Log($"After healing, you have {health} health left!");
    }

    public void TakeDamage(float dmg)
    {
        if (isInvincible) return;

        health -= dmg;

        invincibilityTimer = invincibilityDuration;
        isInvincible = true;

        Debug.Log($"You have {health} health left!");

        if (health <= 0)
        {
            // End the game
            Kill();
        }
    }

    public void Kill()
    {
        Debug.Log("You died!");
    }

    public void GainXP(float amount)
    {
        xp += amount * growth;
        totalXP += amount * growth;

        Debug.Log($"You gained {amount * growth} XP.");

        CheckXP();
    }

    void CheckXP()
    {
        if (xp >= nextLevelXPRequirement)
        {
            xp -= nextLevelXPRequirement;
            level++;
            Debug.Log($"You are now level {level}.");
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
