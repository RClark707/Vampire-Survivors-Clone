using UnityEngine;

public abstract class ItemB : MonoBehaviour // this class must be subclassed in order to be used on a GO
{
    public int currentLevel = 1, maxLevel = 1;

    protected Player owner;

    public virtual void Initialize(ItemStatsB stats)
    {
        maxLevel = stats.maxLevel;
        owner = FindAnyObjectByType<Player>();
    }

    public virtual bool CanLevelUp()
    {
        return currentLevel < maxLevel; // double check if this should be <= for evolutions
    }

    // this method is meant to be overridden and will provide logic for evolutions
    public virtual bool LevelUp() { return true; }

    public virtual void OnEquip() { }

    public virtual void OnUnequip() { }


}
