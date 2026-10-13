using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour // this class must be subclassed in order to be used on a GO
{
    public int currentLevel = 1, maxLevel = 1;
    [HideInInspector]
    public ItemStats statsData;
    protected ItemStats.Evolution[] evolutions;
    protected PlayerInventoryController inventory;
    protected Player owner;
    public Player Owner { get { return owner; } }

    [System.Serializable]
    public class LevelData
    {
        public string name, description;
    }

    public virtual void Initialize(ItemStats stats)
    {
        maxLevel = stats.maxLevel;

        evolutions = stats.evolutionData;

        inventory = FindAnyObjectByType<PlayerInventoryController>();
        owner = FindAnyObjectByType<Player>();
    }

    public virtual bool CanLevelUp()
    {
        return currentLevel < maxLevel; // double check if this should be <= for evolutions
    }

    // this method is meant to be overridden and will provide logic for evolutions
    public virtual bool LevelUp()
    {
        if (evolutions == null) return true;

        foreach (ItemStats.Evolution e in evolutions)
        {
            if (e.evoReq == ItemStats.Evolution.EvolutionRequirements.Automatic) AttemptEvolution(e);
        }

        return true;
    }

    public virtual ItemStats.Evolution[] CanEvolve()
    {
        List<ItemStats.Evolution> possibleEvolutions = new List<ItemStats.Evolution>();

        foreach (ItemStats.Evolution e in evolutions)
        {
            if (CanEvolve(e)) possibleEvolutions.Add(e);
        }

        return possibleEvolutions.ToArray();
    }

    public virtual bool CanEvolve(ItemStats.Evolution evolution, int levelUpAmount = 1)
    {
        if (evolution.evolutionLevel > currentLevel + levelUpAmount)
        {
            Debug.Log($"Evolution is not possible with the current level of {currentLevel} and evolution level of {evolution.evolutionLevel}");
            return false;
        }

        foreach (ItemStats.Evolution.Config c in evolution.catalysts)
        {
            Item item = inventory.Get(c.itemType);
            if (!item || item.currentLevel < c.level)
            {
                Debug.Log($"Cannot evolve, missing {c.itemType.name}");
                return false;
            }
        }

        return true;
    }

    public virtual bool AttemptEvolution(ItemStats.Evolution evolution, int levelUpAmount = 1)
    {
        if (!CanEvolve(evolution, levelUpAmount)) return false;

        bool consumePassives = (evolution.consumes & ItemStats.Evolution.Consumption.Passives) > 0;
        bool consumeWeapons = (evolution.consumes & ItemStats.Evolution.Consumption.Weapons) > 0;

        foreach (ItemStats.Evolution.Config c in evolution.catalysts)
        {
            if (c.itemType is PassiveStats && consumePassives) inventory.Remove(c.itemType, true);
            if (c.itemType is WeaponStats && consumeWeapons) inventory.Remove(c.itemType, true);
        }

        if (this is Passive && consumePassives) inventory.Remove((this as Passive).statsData, true);
        else if (this is Weapon && consumeWeapons) inventory.Remove((this as Weapon).statsData, true);

        inventory.Add(evolution.outcome.itemType);
        return true;
    }

    public virtual void OnEquip() { }

    public virtual void OnUnequip() { }


}
