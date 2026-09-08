using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour // this is explicitly NOT an Entity
{
    // These are the Player's Current Stat Values, NOT the base stats
    [Header("Character Stats")]
    public CharacterStats stats;
    GameObject weapon;
    [HideInInspector]
    public float maxHealth { get; set; }
    [HideInInspector]
    public float health { get; set; }
    [HideInInspector]
    public float movementSpeed { get; set; }
    [HideInInspector]
    public float recovery { get; set; }
    [HideInInspector]
    public float armor { get; set; }
    [HideInInspector]
    public float might { get; set; }
    // [HideInInspector]
    // public float projectileSpeed { get; set; }
    [HideInInspector]
    public float area { get; set; }
    [HideInInspector]
    public float magnet { get; set; }
    [HideInInspector]
    public float growth { get; set; }
    [HideInInspector]
    public float luck { get; set; } // adjust all stats like this?

    [Header("Inventory")]
    InventoryController inv;
    public int openWeaponIndex;
    public int openPassiveIndex;
    public Transform weaponsParent;
    public Transform passivesParent;

    public GameObject secondWeapon;
    public GameObject firstPassive, secondPassive;

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
        if (CharacterSelector.Instance)
        {
            stats = CharacterSelector.GetCharacterStats();
            CharacterSelector.Instance.DestroySingleton();
        }

        inv = FindAnyObjectByType<InventoryController>();
        inv.weaponsParent = weaponsParent;
        inv.passivesParent = passivesParent;

        name = stats.name;
        weapon = stats.StartingWeapon;
        maxHealth = stats.MaxHealth;
        movementSpeed = stats.MovementSpeed;
        recovery = stats.Recovery;
        armor = stats.Armor;
        might = stats.Might;
        // projectileSpeed = characterStats.ProjectileSpeed;
        area = stats.Area;
        magnet = stats.Magnet;
        growth = stats.Growth;
        luck = stats.Luck;

        health = maxHealth;
        AddItem(weapon);
        AddItem(secondWeapon);
        AddItem(firstPassive);
        AddItem(secondPassive);
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

        Recover();
    }

    public void RestoreHealth(float amount)
    {
        health = Mathf.Min(health + amount, maxHealth);

        Debug.Log($"After healing, you have {health} health left!");
    }

    public void Recover()
    {
        health = Mathf.Min(health + recovery * Time.deltaTime, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (isInvincible) return;

        health = Mathf.Max(health - amount, 0f);
        Debug.Log($"{name} has {health} health left after taking {amount} damage!");
        if (health <= 0f)
        {
            Kill();
        }

        invincibilityTimer = invincibilityDuration;
        isInvincible = true;
    }

    public void Kill()
    {
        Debug.Log($"{name} has died! Oh no!");
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

    public void AddItem(GameObject item)
    {
        if (item.TryGetComponent(out WeaponController wc))
        {
            if (openWeaponIndex >= inv.weaponSlots.Count - 1)
            {
                Debug.Log("Weapon slots are already full!");
                return;
            }

            GameObject go = Instantiate(item, transform.position, Quaternion.identity);
            go.transform.SetParent(weaponsParent);
            inv.AddWeapon(openWeaponIndex, wc);
            openWeaponIndex++;

        }
        else if (item.TryGetComponent(out Passive p))
        {
            if (openPassiveIndex >= inv.passiveSlots.Count - 1)
            {
                Debug.Log("Passive slots are already full!");
                return;
            }

            GameObject go = Instantiate(item, transform.position, Quaternion.identity);
            go.transform.SetParent(passivesParent);
            inv.AddPassive(openPassiveIndex, p);
            openPassiveIndex++;
        }
    }
}
