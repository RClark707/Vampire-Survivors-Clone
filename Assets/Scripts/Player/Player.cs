using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour // this is explicitly NOT an Entity
{
    [Header("Character Stats")]
    [SerializeField] CharacterStatsB characterData;
    [HideInInspector] public CharacterStatsB.Stats baseStats;
    CharacterStatsB.Stats actualStats;

    GameObject weapon;

    #region Current Stats
    public float MaxHealth
    {
        get { return actualStats.maxHealth; }
        set
        {
            if (actualStats.maxHealth != value)
            {
                actualStats.maxHealth = value;
                // put additional logic each time the value changes here
                if (GameController.Instance != null) GameController.Instance.maxHealthDisplay.text = "Maximum Health: " + Mathf.RoundToInt(actualStats.maxHealth);
            }
        }
    }

    private float _health;
    public float Health
    {
        get { return _health; }
        set
        {
            if (_health != value)
            {
                _health = Mathf.Clamp(value, 0f, MaxHealth);
                // put additional logic each time the value changes here
                if (GameController.Instance != null)
                {
                    GameController.Instance.curHealthDisplay.text = "Health: " + Mathf.RoundToInt(_health);
                    GameController.Instance.AssignHealthBarUI(_health / MaxHealth);
                }

                if (_health <= 0f)
                {
                    Kill();
                }
            }
        }
    }

    public float CurrentMoveSpeed
    {
        get { return MoveSpeed; }
        set { MoveSpeed = value; }
    }
    public float MoveSpeed
    {
        get { return actualStats.moveSpeed; }
        set
        {
            if (actualStats.moveSpeed != value)
            {
                actualStats.moveSpeed = value;
                // put additional logic each time the value changes here
                if (GameController.Instance != null) GameController.Instance.speedDisplay.text = "Move Speed: " + actualStats.moveSpeed;
            }
        }
    }

    public float CurrentRecovery
    {
        get { return Recovery; }
        set { Recovery = value; }
    }
    public float Recovery
    {
        get { return actualStats.recovery; }
        set
        {
            if (actualStats.recovery != value)
            {
                actualStats.recovery = value;
                // put additional logic each time the value changes here
                if (GameController.Instance != null) GameController.Instance.recoveryDisplay.text = "Recovery: " + actualStats.recovery;
            }
        }
    }

    public float CurrentArmor
    {
        get { return Armor; }
        set { Armor = value; }

    }
    public float Armor
    {
        get { return actualStats.armor; }
        set
        {
            if (actualStats.armor != value)
            {
                actualStats.armor = value;
                // put additional logic each time the value changes here
                if (GameController.Instance != null) GameController.Instance.armorDisplay.text = "Armor: " + actualStats.armor;
            }
        }
    }

    public float CurrentMight
    {
        get { return Might; }
        set { Might = value; }
    }
    public float Might
    {
        get { return actualStats.might; }
        set
        {
            if (actualStats.might != value)
            {
                actualStats.might = value;
                // put additional logic each time the value changes here
                if (GameController.Instance != null) GameController.Instance.mightDisplay.text = "Might: " + actualStats.might;
            }
        }
    }

    public float CurrentProjectileSpeed
    {
        get { return ProjectileSpeed; }
        set { ProjectileSpeed = value; }
    }
    public float ProjectileSpeed
    {
        get { return actualStats.projectileSpeed; }
        set
        {
            if (actualStats.projectileSpeed != value)
            {
                actualStats.projectileSpeed = value;
                // put additional logic each time the value changes here
                if (GameController.Instance != null) GameController.Instance.projSpeedDisplay.text = "Projectile Speed: " + actualStats.projectileSpeed;
            }
        }
    }

    public float CurrentArea
    {
        get { return Area; }
        set { Area = value; }

    }
    public float Area
    {
        get { return actualStats.area; }
        set
        {
            if (actualStats.area != value)
            {
                actualStats.area = value;
                // put additional logic each time the value changes here
                if (GameController.Instance != null) GameController.Instance.areaDisplay.text = "Area: " + actualStats.area;
            }
        }
    }

    public float CurrentMagnet
    {
        get { return Magnet; }
        set { Magnet = value; }
    }
    public float Magnet
    {
        get { return actualStats.magnet; }
        set
        {
            if (actualStats.magnet != value)
            {
                actualStats.magnet = value;
                // put additional logic each time the value changes here
                if (GameController.Instance != null) GameController.Instance.magnetDisplay.text = "Magnet: " + actualStats.magnet;
            }
        }
    }

    public float CurrentGrowth
    {
        get { return Growth; }
        set { Growth = value; }
    }
    public float Growth
    {
        get { return actualStats.growth; }
        set
        {
            if (actualStats.growth != value)
            {
                actualStats.growth = value;
                // put additional logic each time the value changes here
                if (GameController.Instance != null) GameController.Instance.growthDisplay.text = "Growth: " + actualStats.growth;
            }
        }
    }

    private float CurrentLuck
    {
        get { return Luck; }
        set { Luck = value; }
    }
    public float Luck
    {
        get { return actualStats.luck; }
        set
        {
            if (actualStats.luck != value)
            {
                actualStats.luck = value;
                // put additional logic each time the value changes here
                if (GameController.Instance != null) GameController.Instance.luckDisplay.text = "Luck: " + actualStats.luck;
            }
        }
    }
    #endregion

    [Header("Inventory")]
    PlayerInventoryController inv;
    public int openWeaponIndex;
    public int openPassiveIndex;

    [Header("Experience & Leveling")]
    public List<XPRequirement> xpRequirements;

    float _xp = 0;
    float XP
    {
        get { return _xp; }
        set
        {
            if (_xp != value)
            {
                _xp = value;
                if (GameController.Instance != null)
                {
                    GameController.Instance.AssignExperienceBarUI(XP / nextLevelXPRequirement);
                }
            }
        }
    }
    float totalXP = 0;
    float nextLevelXPRequirement;
    int _level = 1;
    int Level
    {
        get { return _level; }
        set
        {
            if (_level != value)
            {
                _level = value;
                if (GameController.Instance != null)
                {
                    GameController.Instance.AssignLevelUI(Level);
                }
            }
        }
    }

    [System.Serializable]
    public class XPRequirement
    {
        public int minLevel;
        public float nextLevelXPRequirement;
    }

    [Header("I-Frames")]
    public float invincibilityDuration;
    float invincibilityTimer;
    bool isInvincible;

    [Header("Particles")]
    public ParticleSystem damageEffect;

    private void Awake()
    {
        if (CharacterSelector.Instance)
        {
            characterData = CharacterSelector.GetCharacterStats();
            CharacterSelector.Instance.DestroySingleton();
        }

        inv = GetComponent<PlayerInventoryController>();

        // Assign variables

        baseStats = actualStats = characterData.stats;
        _health = actualStats.maxHealth;
    }

    private void Start()
    {
        inv.Add(characterData.StartingWeapon);

        nextLevelXPRequirement = xpRequirements[0].nextLevelXPRequirement;

        #region Assign UI with GameController
        GameController.Instance.maxHealthDisplay.text = "Maximum Health: " + Mathf.RoundToInt(MaxHealth);
        GameController.Instance.curHealthDisplay.text = "Health: " + Mathf.RoundToInt(Health);
        GameController.Instance.speedDisplay.text = "Move Speed: " + MoveSpeed;
        GameController.Instance.projSpeedDisplay.text = "Proj. Speed: " + ProjectileSpeed;
        GameController.Instance.recoveryDisplay.text = "Recovery: " + Recovery;
        GameController.Instance.armorDisplay.text = "Armor: " + Armor;
        GameController.Instance.mightDisplay.text = "Might: " + Might;
        GameController.Instance.areaDisplay.text = "Area: " + Area;
        GameController.Instance.magnetDisplay.text = "Magnet: " + Magnet;
        GameController.Instance.growthDisplay.text = "Growth: " + Growth;
        GameController.Instance.luckDisplay.text = "Luck: " + Luck;
        GameController.Instance.AssignCharacterUI(characterData.Icon, characterData.name);
        GameController.Instance.AssignLevelUI(Level);
        GameController.Instance.AssignItemsUI(inv.weaponSlots, inv.passiveSlots);
        GameController.Instance.AssignExperienceBarUI(XP / nextLevelXPRequirement);
        GameController.Instance.AssignHealthBarUI(Health / MaxHealth);
        #endregion
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
        Health += amount;

        // Debug.Log($"After healing, you have {Health} health left!");
    }

    public void Recover()
    {
        Health += Recovery * Time.deltaTime;
    }

    public void TakeDamage(float amount)
    {
        if (isInvincible) return;

        Health -= amount;
        // Debug.Log($"{name} has {Health} health left after taking {amount} damage!");

        if (damageEffect) Destroy(Instantiate(damageEffect, transform.position, Quaternion.identity), 5f);

        invincibilityTimer = invincibilityDuration;
        isInvincible = true;
    }

    public void Kill()
    {
        if (!GameController.Instance.isGameOver) // we only want to call this method once!
        {
            GameController.Instance.GameOver();
            // GameController.Instance.AssignItemsUI(inv.weaponSlots, inv.passiveSlots);
        }
    }

    public void GainXP(float amount)
    {
        XP += amount * Growth;
        totalXP += amount * Growth;

        // Debug.Log($"You gained {amount * growth} XP.");

        CheckXP();
    }

    void CheckXP()
    {
        if (XP >= nextLevelXPRequirement)
        {
            XP -= nextLevelXPRequirement;
            Level++;
            Debug.Log($"You are now level {Level}.");
            GameController.Instance.StartPlayerLevelUp();

            foreach (XPRequirement xpr in xpRequirements) // reassign nextLevelXPRequirement
            {
                if (Level < xpr.minLevel)
                {
                    break;
                }
                else
                {
                    nextLevelXPRequirement = xpr.nextLevelXPRequirement;
                    GameController.Instance.AssignExperienceBarUI(XP / nextLevelXPRequirement);
                }
            }
        }
    }

    //[Obsolete("Old function we used to add weapons, no longer used")]
    //public void AddItem(GameObject item)
    //{
    //    if (item.TryGetComponent(out WeaponController wc))
    //    {
    //        if (openWeaponIndex >= inv.weaponSlots.Count - 1)
    //        {
    //            Debug.Log("Weapon slots are already full!");
    //            return;
    //        }

    //        GameObject go = Instantiate(item, transform.position, Quaternion.identity);
    //        go.transform.SetParent(weaponsParent);
    //        inv.AddWeapon(openWeaponIndex, go.GetComponent<WeaponController>()); // this is a different controller than wc!
    //        if (GameController.Instance != null)
    //        {
    //            GameController.Instance.AssignItemsUI(inv.weaponUISlots, inv.passiveUISlots);
    //        }
    //        openWeaponIndex++;

    //    }
    //    else if (item.TryGetComponent(out Passive p))
    //    {
    //        if (openPassiveIndex >= inv.passiveSlots.Count - 1)
    //        {
    //            Debug.Log("Passive slots are already full!");
    //            return;
    //        }

    //        GameObject go = Instantiate(item, transform.position, Quaternion.identity);
    //        go.transform.SetParent(passivesParent);
    //        inv.AddPassive(openPassiveIndex, go.GetComponent<Passive>());
    //        openPassiveIndex++;
    //    }

    //    if (GameController.Instance != null && GameController.Instance.choosingUpgrades)
    //    {
    //        GameController.Instance.EndPlayerLevelUp();
    //    }
    //}

    public void RecalculateStats()
    {
        actualStats = baseStats;
        foreach (PlayerInventoryController.Slot s in inv.passiveSlots)
        {
            PassiveB p = s.item as PassiveB;
            if (p)
            {
                actualStats += p.GetBoosts();
            }
        }
    }
}
