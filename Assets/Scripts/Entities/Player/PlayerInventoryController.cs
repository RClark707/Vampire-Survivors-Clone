using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    [Serializable]
    public class Slot
    {
        public Item item;
        //public Image image;

        public void Assign(Item assignedItem)
        {
            item = assignedItem;
            if (item is Weapon)
            {
                Weapon w = item as Weapon;
                //image.enabled = true;
                //image.sprite = w.statsData.icon;
            }
            else
            {
                Passive p = item as Passive;
                //image.enabled = true;
                //image.sprite = p.statsData.icon;
            }
            Debug.Log($"Assigned {item.name} to player's inventory.");
        }

        public void Clear()
        {
            item = null;
            //image.enabled = false;
            //image.sprite = null;
        }

        public bool IsEmpty()
        {
            return item == null;
        }
    }

    [Header("Inventory Slots UI")]
    public UIInventoryIconDisplay weaponsDisplay;
    public UIInventoryIconDisplay passivesDisplay;
    public List<Slot> weaponSlots = new List<Slot>(6);
    public List<Slot> passiveSlots = new List<Slot>(6);



    //[Serializable]
    //public class UpgradeUI
    //{
    //    public TMP_Text upgradeNameDisplay;
    //    public TMP_Text upgradeDescriptionDisplay;
    //    public Image upgradeIcon;
    //    public Button upgradeButton;
    //}

    [Header("All Available Items")]
    public List<WeaponStats> availableWeapons = new List<WeaponStats>();
    public List<PassiveStats> availablePassives = new List<PassiveStats>();

    [Header("Level Up UI")]
    public UIUpgradeWindow upgradeWindow;

    Player player;

    void Start()
    {
        player = GetComponent<Player>();
    }

    public bool Has(ItemStats type) { return Get(type); }

    public Item Get(ItemStats type) // This function sends you to a different function based on type
    {
        if (type is WeaponStats) return Get(type as WeaponStats);
        else if (type is PassiveStats) return Get(type as PassiveStats);
        return null;
    }

    public Passive Get(PassiveStats type)
    {
        foreach (Slot S in passiveSlots)
        {
            Passive p = S.item as Passive;
            if (p && p.statsData == type)
            {
                return p;
            }
        }
        return null;
    }

    public Weapon Get(WeaponStats type)
    {
        foreach (Slot s in weaponSlots)
        {
            Weapon w = s.item as Weapon;
            if (w && w.statsData == type)
            {
                return w;
            }
        }
        return null;
    }

    public bool Remove(ItemStats stats, bool removeUpgradeAvailability = false)
    {
        if (stats is WeaponStats) return Remove(stats as WeaponStats, removeUpgradeAvailability);
        else if (stats is PassiveStats) return Remove(stats as PassiveStats, removeUpgradeAvailability);
        return false;
    }

    public bool Remove(WeaponStats stats, bool removeUpgradeAvailability = false)
    {
        if (removeUpgradeAvailability) availableWeapons.Remove(stats);

        for (int i = 0; i < weaponSlots.Count; i++)
        {
            Weapon w = weaponSlots[i].item as Weapon;
            if (w.statsData == stats)
            {
                weaponSlots[i].Clear();
                weaponsDisplay.Refresh();
                w.OnUnequip();
                Destroy(w.gameObject);
                return true;
            }
        }

        return false;
    }

    public bool Remove(PassiveStats stats, bool removeUpgradeAvailability = false)
    {
        if (removeUpgradeAvailability) availablePassives.Remove(stats);

        for (int i = 0; i < passiveSlots.Count; i++)
        {
            Passive p = passiveSlots[i].item as Passive;
            if (p.statsData == stats)
            {
                passiveSlots[i].Clear();
                weaponsDisplay.Refresh();
                p.OnUnequip();
                Destroy(p.gameObject);
                return true;
            }
        }

        return false;
    }

    public int Add(ItemStats stats)
    {
        if (stats is WeaponStats) return Add(stats as WeaponStats);
        else if (stats is PassiveStats) return Add(stats as PassiveStats);
        return -1;
    }

    public int Add(WeaponStats stats)
    {
        int slotNum = -1;

        for (int i = 0; i < weaponSlots.Capacity; i++)
        {
            if (weaponSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }

        if (slotNum < 0) return slotNum;

        Type weaponType = Type.GetType(stats.behavior);

        if (weaponType != null)
        {
            // instantiate the weapon
            GameObject go = new GameObject(stats.baseStats.name + " Controller");
            Weapon spawnedWeapon = (Weapon)go.AddComponent(weaponType);
            spawnedWeapon.transform.SetParent(transform); // change this to whatever parent you want
            spawnedWeapon.transform.localPosition = Vector2.zero;
            spawnedWeapon.Initialize(stats);
            spawnedWeapon.OnEquip();

            weaponSlots[slotNum].Assign(spawnedWeapon);
            weaponsDisplay.Refresh();

            if (GameController.Instance != null && GameController.Instance.choosingUpgrades)
            {
                GameController.Instance.EndPlayerLevelUp();
            }

            return slotNum;
        }
        else
        {
            Debug.LogError($"Invalid weapon type specified for {stats.name}");
        }

        return -1;
    }

    public int Add(PassiveStats stats)
    {
        int slotNum = -1;

        for (int i = 0; i < passiveSlots.Capacity; i++)
        {
            if (passiveSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }

        if (slotNum < 0) return slotNum;


        GameObject go = new GameObject(stats.baseStats.name + " Passive");
        Passive spawnedPassive = go.AddComponent<Passive>();
        spawnedPassive.Initialize(stats);
        spawnedPassive.transform.SetParent(transform); // change this to whatever parent you want
        spawnedPassive.transform.localPosition = Vector2.zero;
        // spawnedPassive.OnEquip(); // apparently this isn't necessary to call?

        passiveSlots[slotNum].Assign(spawnedPassive);
        passivesDisplay.Refresh();

        if (GameController.Instance != null && GameController.Instance.choosingUpgrades)
        {
            GameController.Instance.EndPlayerLevelUp();
        }
        player.RecalculateStats(); // should we call this for weapons too?

        return slotNum;
    }

    public bool LevelUpItem(ItemStats stats)
    {
        Item item = Get(stats);
        if (item) return LevelUpItem(item);
        return false;
    }

    public bool LevelUpItem(Item item)
    {
        if (!item.LevelUp())
        {
            Debug.LogError($"Failed to level up {item.name}");
            return false;
        }

        weaponsDisplay.Refresh();
        passivesDisplay.Refresh();

        if (GameController.Instance != null && GameController.Instance.choosingUpgrades)
        {
            GameController.Instance.EndPlayerLevelUp();
        }

        if (item is Passive) player.RecalculateStats();

        return true;
    }

    int GetRemainingSlots(List<Slot> slots)
    {
        int count = 0;
        foreach (Slot s in slots)
        {
            if (s.IsEmpty()) count++;
        }

        return count;
    }

    public int GetRemainingSlots<T>() where T : Item { return GetRemainingSlots(new List<Slot>(GetSlots<T>())); }
    public int GetRemainingSlotsFor<T>() where T : ItemStats { return GetRemainingSlots(new List<Slot>(GetSlotsFor<T>())); }

    public T[] GetAllItemsAvailableFor<T>() where T : ItemStats
    {
        switch (typeof(T).ToString())
        {
            case "PassiveStats":
                return availablePassives.ToArray() as T[];
            case "WeaponStats":
                return availableWeapons.ToArray() as T[];
            case "ItemStats":
                List<ItemStats> s = new List<ItemStats>(availablePassives);
                s.AddRange(availableWeapons);
                return s.ToArray() as T[];
            default:
                Debug.LogWarning("Generic type provided to GetAllItemsAvailableFor() has no return behaviour");
                return null;
        }
    }

    public T[] GetAllUnownedItemsFor<T>() where T : ItemStats
    {
        var available = GetAllItemsAvailableFor<T>();

        if (available == null || available.Length == 0) return new T[0];

        List<T> unowned = new List<T>(available);

        var slots = GetSlotsFor<T>();
        if (slots != null)
        {
            foreach (Slot s in slots) // this will check if we own any of the available items, and remove them from the list
            {
                if (s?.item?.statsData != null && unowned.Contains(s.item.statsData as T))
                {
                    unowned.Remove(s.item.statsData as T);
                }
            }
        }

        return unowned.ToArray();
    }

    public T[] GetUpgradables<T>() where T : Item
    {
        List<T> upgradables = new List<T>();
        foreach (Slot s in GetSlots<T>())
        {
            if (s.item is T t && t.CanLevelUp()) upgradables.Add(t);
        }
        return upgradables.ToArray();
    }

    public T[] GetEvolvables<T>() where T : Item
    {
        List<T> evolvables = new List<T>();
        foreach (Slot s in GetSlots<T>())
        {
            if (s.item is T t && t.CanEvolve().Length > 0) evolvables.Add(t);
        }
        return evolvables.ToArray();
    }

    public Slot[] GetSlots<T>() where T : Item
    {
        switch (typeof(T).ToString())
        {
            case "Passive":
                return passiveSlots.ToArray();
            case "Weapon":
                return weaponSlots.ToArray();
            case "Item":
                List<Slot> s = new List<Slot>(passiveSlots);
                s.AddRange(weaponSlots);
                return s.ToArray();
            default:
                Debug.LogWarning("Generic type provided to GetSlots() has no return behaviour");
                return null;
        }
    }

    public Slot[] GetSlotsFor<T>() where T : ItemStats
    {
        switch (typeof(T).ToString())
        {
            case "PassiveStats":
                return passiveSlots.ToArray();
            case "WeaponStats":
                return weaponSlots.ToArray();
            case "ItemStats":
                List<Slot> s = new List<Slot>(passiveSlots);
                s.AddRange(weaponSlots);
                return s.ToArray();
            default:
                Debug.LogWarning("Generic type provided to GetSlotsFor() has no return behaviour");
                return null;
        }
    }

    void SetUpgradeOptions()
    {
        // make a copy of all the items available in the game, then make a list of ones we have access to choose from for upgrades
        List<ItemStats> availableUpgrades = new List<ItemStats>();
        List<ItemStats> allUpgrades = new List<ItemStats>(availableWeapons);
        allUpgrades.AddRange(availablePassives);

        int openWeaponSlots = GetRemainingSlots(weaponSlots);
        int openPassiveSlots = GetRemainingSlots(passiveSlots);

        // populate available upgrades based on items we have equipped and items we have space for in our open slots
        foreach (ItemStats stats in allUpgrades)
        {
            Item item = Get(stats);

            if (item)
            {
                if (item.currentLevel < stats.maxLevel) availableUpgrades.Add(stats);
            }
            else
            {
                if (stats is WeaponStats && openWeaponSlots > 0) availableUpgrades.Add(stats);
                else if (stats is PassiveStats && openPassiveSlots > 0) availableUpgrades.Add(stats);
            }
        }

        int availableUpgradeCount = availableUpgrades.Count;
        if (availableUpgradeCount > 0)
        {
            bool getExtraItem = 1f - 1f / player.ActualStats.luck > UnityEngine.Random.value;
            if (getExtraItem || availableUpgradeCount < 4) upgradeWindow.SetUpgrades(this, availableUpgrades, 4); // why does this check for less than 4?
            else upgradeWindow.SetUpgrades(this, availableUpgrades, 3, "Increase your Luck stat for a chance to get 4 items!");
        }
        else if (GameController.Instance != null && GameController.Instance.choosingUpgrades)
        {
            GameController.Instance.EndPlayerLevelUp();
        }
    }

    public void ClearAndSetUpgrades()
    {
        // ClearUpgradeOptions();
        SetUpgradeOptions();
    }

}
