using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour // this is explicitly NOT an Entity
{
    [Header("Character Stats")]
    [SerializeField] CharacterStatsB characterData;
    [HideInInspector] public CharacterStatsB.Stats baseStats;
    CharacterStatsB.Stats _actualStats;
    public CharacterStatsB.Stats ActualStats // there is a consideration to avoid making this setter public, but it already is, so cry about it
    {
        get { return _actualStats; }
        set
        {
            _actualStats = value;
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
                _health = Mathf.Clamp(value, 0f, _actualStats.maxHealth);
                // Update the Health Bar
                GameController.Instance.AssignHealthBarUI(_health / _actualStats.maxHealth);
                if (_health <= 0f)
                {
                    Kill();
                }
            }
        }
    }

    [Header("Inventory")]
    PlayerInventoryController inv;

    [Header("Experience & Leveling")]
    PlayerCollector collector;
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
    public int Level
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
    public ParticleSystem blockedEffect;

    private void Awake()
    {
        if (CharacterSelector.Instance)
        {
            characterData = CharacterSelector.GetCharacterStats();
            CharacterSelector.Instance.DestroySingleton();
        }

        inv = GetComponent<PlayerInventoryController>();
        collector = GetComponentInChildren<PlayerCollector>();

        // Assign variables

        baseStats = _actualStats = characterData.stats;
        _health = _actualStats.maxHealth;
    }

    private void Start()
    {
        inv.Add(characterData.StartingWeapon);

        nextLevelXPRequirement = xpRequirements[0].nextLevelXPRequirement;

        #region Assign UI with GameController
        GameController.Instance.AssignCharacterUI(characterData.Icon, characterData.name);
        GameController.Instance.AssignLevelUI(Level);
        // GameController.Instance.AssignItemsUI(inv.weaponSlots, inv.passiveSlots);
        GameController.Instance.AssignExperienceBarUI(XP / nextLevelXPRequirement);
        GameController.Instance.AssignHealthBarUI(Health / _actualStats.maxHealth);
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
        Health += _actualStats.recovery * Time.deltaTime;
    }

    public void TakeDamage(float amount)
    {
        if (isInvincible) return;

        amount -= _actualStats.armor;

        if (Mathf.Approximately(amount, 0f))
        {
            if (blockedEffect) Destroy(Instantiate(blockedEffect, transform.position, Quaternion.identity), 5f);
        }
        else
        {
            Health -= amount;
            // Debug.Log($"{name} has {Health} health left after taking {amount} damage!");
            if (damageEffect) Destroy(Instantiate(damageEffect, transform.position, Quaternion.identity), 5f);
        }

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
        XP += amount * _actualStats.growth;
        totalXP += amount * _actualStats.growth;

        // Debug.Log($"You gained {amount * growth} XP.");

        CheckXP(); // this only gets checked once per instance of xp gain
    }

    void CheckXP()
    {
        while (XP >= nextLevelXPRequirement) // just change this to a while loop?
        {
            XP -= nextLevelXPRequirement;
            Level++;
            // Debug.Log($"You are now level {Level}.");
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

    public void RecalculateStats()
    {
        _actualStats = baseStats;
        foreach (PlayerInventoryController.Slot s in inv.passiveSlots)
        {
            PassiveB p = s.item as PassiveB;
            if (p)
            {
                _actualStats += p.GetBoosts();
            }
        }

        collector.SetRadius(_actualStats.magnet);
    }
}
