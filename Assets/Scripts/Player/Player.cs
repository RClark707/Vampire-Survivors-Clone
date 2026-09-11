using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour // this is explicitly NOT an Entity
{
    [Header("Character Stats")]
    public CharacterStats stats;
    GameObject weapon;

    #region Current Stats
    private float _maxHealth { get; set; }
    public float MaxHealth
    {
        get { return _maxHealth; }
        set
        {
            if (_maxHealth != value)
            {
                _maxHealth = value;
                // put additional logic each time the value changes here
                if (GameController.Instance != null) GameController.Instance.maxHealthDisplay.text = "Maximum Health: " + Mathf.RoundToInt(_maxHealth);
            }
        }
    }

    private float _health { get; set; }
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

    private float _movementSpeed { get; set; }
    public float MovementSpeed
    {
        get { return _movementSpeed; }
        set
        {
            if (_movementSpeed != value)
            {
                _movementSpeed = value;
                // put additional logic each time the value changes here
                if (GameController.Instance != null) GameController.Instance.speedDisplay.text = "Speed: " + _movementSpeed;
            }
        }
    }

    private float _recovery { get; set; }
    public float Recovery
    {
        get { return _recovery; }
        set
        {
            if (_recovery != value)
            {
                _recovery = value;
                // put additional logic each time the value changes here
                if (GameController.Instance != null) GameController.Instance.recoveryDisplay.text = "Recovery: " + _recovery;
            }
        }
    }

    private float _armor { get; set; }
    public float Armor
    {
        get { return _armor; }
        set
        {
            if (_armor != value)
            {
                _armor = value;
                // put additional logic each time the value changes here
                if (GameController.Instance != null) GameController.Instance.armorDisplay.text = "Armor: " + _armor;
            }
        }
    }

    private float _might { get; set; }
    public float Might
    {
        get { return _might; }
        set
        {
            if (_might != value)
            {
                _might = value;
                // put additional logic each time the value changes here
                if (GameController.Instance != null) GameController.Instance.mightDisplay.text = "Might: " + _might;
            }
        }
    }

    // [HideInInspector]
    // public float projectileSpeed { get; set; }

    private float _area { get; set; }
    public float Area
    {
        get { return _area; }
        set
        {
            if (_area != value)
            {
                _area = value;
                // put additional logic each time the value changes here
                if (GameController.Instance != null) GameController.Instance.areaDisplay.text = "Area: " + _area;
            }
        }
    }

    private float _magnet { get; set; }
    public float Magnet
    {
        get { return _magnet; }
        set
        {
            if (_magnet != value)
            {
                _magnet = value;
                // put additional logic each time the value changes here
                if (GameController.Instance != null) GameController.Instance.magnetDisplay.text = "Magnet: " + _magnet;
            }
        }
    }

    private float _growth { get; set; }
    public float Growth
    {
        get { return _growth; }
        set
        {
            if (_growth != value)
            {
                _growth = value;
                // put additional logic each time the value changes here
                if (GameController.Instance != null) GameController.Instance.growthDisplay.text = "Growth: " + _growth;
            }
        }
    }

    private float _luck { get; set; }
    public float Luck
    {
        get { return _luck; }
        set
        {
            if (_luck != value)
            {
                _luck = value;
                // put additional logic each time the value changes here
                if (GameController.Instance != null) GameController.Instance.luckDisplay.text = "Luck: " + _luck;
            }
        }
    }
    #endregion

    [Header("Inventory")]
    InventoryController inv;
    public int openWeaponIndex;
    public int openPassiveIndex;
    public Transform weaponsParent;
    public Transform passivesParent;
    // For testing purposes only
    // public GameObject secondWeapon;
    // public GameObject firstPassive, secondPassive;

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

        // SET CHARACTER STATS
        name = stats.name;
        weapon = stats.StartingWeapon;
        MaxHealth = stats.MaxHealth;
        MovementSpeed = stats.MovementSpeed;
        Recovery = stats.Recovery;
        Armor = stats.Armor;
        Might = stats.Might;
        // projectileSpeed = characterStats.ProjectileSpeed;
        Area = stats.Area;
        Magnet = stats.Magnet;
        Growth = stats.Growth;
        Luck = stats.Luck;

        Health = MaxHealth;

        AddItem(weapon);
        // AddItem(secondWeapon);
        // AddItem(firstPassive);
        // AddItem(secondPassive);
    }

    private void Start()
    {
        nextLevelXPRequirement = xpRequirements[0].nextLevelXPRequirement;

        #region Assign UI with GameController
        GameController.Instance.maxHealthDisplay.text = "Maximum Health: " + Mathf.RoundToInt(_maxHealth);
        GameController.Instance.curHealthDisplay.text = "Health: " + Mathf.RoundToInt(_health);
        GameController.Instance.speedDisplay.text = "Speed: " + _movementSpeed;
        GameController.Instance.recoveryDisplay.text = "Recovery: " + _recovery;
        GameController.Instance.armorDisplay.text = "Armor: " + _armor;
        GameController.Instance.mightDisplay.text = "Might: " + _might;
        GameController.Instance.areaDisplay.text = "Area: " + _area;
        GameController.Instance.magnetDisplay.text = "Magnet: " + _magnet;
        GameController.Instance.growthDisplay.text = "Growth: " + _growth;
        GameController.Instance.luckDisplay.text = "Luck: " + _luck;
        GameController.Instance.AssignCharacterUI(stats.Icon, stats.name);
        GameController.Instance.AssignLevelUI(Level);
        GameController.Instance.AssignItemsUI(inv.weaponUISlots, inv.passiveUISlots);
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

        Debug.Log($"After healing, you have {Health} health left!");
    }

    public void Recover()
    {
        Health += Recovery * Time.deltaTime;
    }

    public void TakeDamage(float amount)
    {
        if (isInvincible) return;

        Health -= amount;
        Debug.Log($"{name} has {Health} health left after taking {amount} damage!");

        invincibilityTimer = invincibilityDuration;
        isInvincible = true;
    }

    public void Kill()
    {
        if (!GameController.Instance.isGameOver) // we only want to call this method once!
        {
            GameController.Instance.GameOver();
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
            foreach (XPRequirement xpr in xpRequirements)
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
            GameController.Instance.StartPlayerLevelUp();
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
            inv.AddWeapon(openWeaponIndex, go.GetComponent<WeaponController>()); // this is a different controller than wc!
            if (GameController.Instance != null)
            {
                GameController.Instance.AssignItemsUI(inv.weaponUISlots, inv.passiveUISlots);
            }
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
            inv.AddPassive(openPassiveIndex, go.GetComponent<Passive>());
            openPassiveIndex++;
        }

        if (GameController.Instance != null && GameController.Instance.choosingUpgrades)
        {
            GameController.Instance.EndPlayerLevelUp();
        }
    }
}
