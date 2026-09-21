using System.Collections.Generic;
using UnityEngine;

public abstract class ItemB : MonoBehaviour // this class must be subclassed in order to be used on a GO
{
    public int currentLevel = 1, maxLevel = 1;

    protected ItemStatsB.Evolution[] evolutions;
    protected PlayerInventoryController inventory;

    protected Player owner;

    public virtual void Initialize(ItemStatsB stats)
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

        foreach (ItemStatsB.Evolution e in evolutions)
        {
            if (e.evoReq == ItemStatsB.Evolution.EvolutionRequirements.Automatic) AttemptEvolution(e);
        }

        return true;
    }

    public virtual ItemStatsB.Evolution[] CanEvolve()
    {
        List<ItemStatsB.Evolution> possibleEvolutions = new List<ItemStatsB.Evolution>();

        foreach (ItemStatsB.Evolution e in evolutions)
        {
            if (CanEvolve(e)) possibleEvolutions.Add(e);
        }

        return possibleEvolutions.ToArray();
    }

    public virtual bool CanEvolve(ItemStatsB.Evolution evolution, int levelUpAmount = 1)
    {
        if (evolution.evolutionLevel > currentLevel + levelUpAmount)
        {
            Debug.Log($"Evolution is not possible with the current level of {currentLevel} and evolution level of {evolution.evolutionLevel}");
            return false;
        }

        foreach (ItemStatsB.Evolution.Config c in evolution.catalysts)
        {
            ItemB item = inventory.Get(c.itemType);
            if (!item || item.currentLevel < c.level)
            {
                Debug.Log($"Cannot evolve, missing {c.itemType.name}");
                return false;
            }
        }

        return true;
    }

    public virtual bool AttemptEvolution(ItemStatsB.Evolution evolution, int levelUpAmount = 1)
    {
        if (!CanEvolve(evolution, levelUpAmount)) return false;

        bool consumePassives = (evolution.consumes & ItemStatsB.Evolution.Consumption.Passives) > 0;
        bool consumeWeapons = (evolution.consumes & ItemStatsB.Evolution.Consumption.Weapons) > 0;

        foreach (ItemStatsB.Evolution.Config c in evolution.catalysts)
        {
            if (c.itemType is PassiveStatsB && consumePassives) inventory.Remove(c.itemType, true);
            if (c.itemType is WeaponStatsB && consumeWeapons) inventory.Remove(c.itemType, true);
        }

        if (this is PassiveB && consumePassives) inventory.Remove((this as PassiveB).statsData, true);
        else if (this is WeaponB && consumeWeapons) inventory.Remove((this as WeaponB).statsData, true);

        inventory.Add(evolution.outcome.itemType);
        return true;
    }

    public virtual void OnEquip() { }

    public virtual void OnUnequip() { }


}
