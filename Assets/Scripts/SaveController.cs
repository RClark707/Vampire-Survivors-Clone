using System;
using System.Collections.Generic;
using System.Linq;
using Terresquall;
using UnityEngine;

public class SaveController : PersistentObject
{
    static SaveController _instance;

    public static SaveController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<SaveController>();
            }
            return _instance;
        }
    }

    public event Action OnCoinsChanged;
    public event Action OnPowerupsChanged;

    protected SaveData savedData = new SaveData();
    public SaveData SavedData
    {
        get { return savedData; }
    }

    [System.Serializable]
    public new class SaveData : PersistentObject.SaveData
    {
        public float totalCoins;
        public List<Powerup.Stats> powerups = new List<Powerup.Stats>();
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            Time.timeScale = 1f;
            if (Bench.SlotHasSave(Bench.currentSlot))
            {
                Bench.LoadGame(Bench.currentSlot);
            }
            else
            {
                Bench.SaveGame(true);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }

    public float GetTotalCoins() { return savedData.totalCoins; } // why do we have this function if we never use it

    public void AddCoins(float amount)
    {
        savedData.totalCoins += amount;
        OnCoinsChanged?.Invoke();
    }

    public bool TrySpendCoins(float amount) // why is this in the Save Controller script?
    {
        if (amount <= 0) return true;
        if (savedData.totalCoins >= amount)
        {
            savedData.totalCoins -= amount;
            OnCoinsChanged?.Invoke();
            Bench.SaveGame(Bench.currentSlot, true);
            return true;
        }
        return false;
    }

    public int GetLevel(PowerupStats stats)
    {
        Powerup.Stats powerup = Find(stats);
        if (powerup == null) return 0;
        return powerup.level;
    }

    public void SetLevel(PowerupStats stats, int level)
    {
        if (stats == null) return;

        int clampedLevel = Mathf.Clamp(level, 0, stats.maxLevel);
        Powerup.Stats existing = Find(stats);

        if (clampedLevel > 0)
        {
            if (existing != null)
            {
                existing.level = clampedLevel;
            }
            else
            {
                savedData.powerups.Add(new Powerup.Stats(stats.name, clampedLevel));
            }
        }
        else if (existing != null)
        {
            savedData.powerups.Remove(existing);
        }

        OnPowerupsChanged?.Invoke();
    }

    public Powerup.Stats Find(PowerupStats stats)
    {
        if (stats == null) return null;
        return savedData.powerups.FirstOrDefault(e => e.name == stats.name && e.level > 0);
    }

    public bool LevelUp(PowerupStats stats, int amount = 1)
    {
        if (stats == null) return false;

        int currentLevel = GetLevel(stats);
        if (currentLevel >= stats.maxLevel) return false;

        SetLevel(stats, currentLevel + amount);
        return true;
    }

    public List<Powerup.Stats> GetAllPowerups()
    {
        return new List<Powerup.Stats>(savedData.powerups);
    }

    public void ClearAllPowerUps()
    {
        savedData.powerups.Clear();
        OnPowerupsChanged?.Invoke();
    }

    #region Override Functions from PersistentObject

    public override bool CanSave()
    {
        return base.CanSave();
    }

    public override PersistentObject.SaveData Save()
    {
        if (!CanSave())
        {
            Debug.LogWarning("SaveController cannot save!");
            return null;
        }

        return savedData;
    }

    public override bool Load(PersistentObject.SaveData data)
    {
        if (data == null)
        {
            savedData = new SaveData
            {
                totalCoins = 0,
                powerups = new List<Powerup.Stats>()
            };
            Debug.LogWarning("No save data found!");
            return false;
        }

        savedData = data as SaveData;

        if (savedData == null)
        {
            Debug.LogError("Failed to cast SaveData.");
            return false;
        }

        OnCoinsChanged?.Invoke();
        OnPowerupsChanged?.Invoke();

        return true;
    }

    #endregion
}
