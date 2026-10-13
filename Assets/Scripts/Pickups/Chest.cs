using System.Collections.Generic;
using UnityEngine;

public class Chest : Sortable // we aren't subclassing Pickup because Chests behave differently than most pickups
{
    [System.Flags]
    public enum RewardType
    {
        NewPassive = 1, NewWeapon = 2, UpgradePassive = 4, UpgradeWeapon = 8, Evolution = 16
    }
    [Header("Chest Settings")]
    public RewardType possibleReward = (RewardType)~0;

    public enum RewardCountType { Sequential, Random }; // do we randomly pick number of rewards, or get it in a sequence of values?
    public RewardCountType rewardCountType = RewardCountType.Sequential;

    public ChestStats[] chestStats;
    int currentStatIndex = 0;

    public ChestStats GetCurrentChestStats()
    {
        return chestStats[currentStatIndex];
    }

    public static int totalRewards = 0;

    PlayerInventoryController recipient;

    public ChestStats GetNextChestStats()
    {
        if (chestStats == null || chestStats.Length == 0)
        {
            Debug.LogWarning("Chest Stats were left unset.");
            return null;
        }

        switch (rewardCountType)
        {
            case RewardCountType.Sequential:
                currentStatIndex = Mathf.Clamp( // confine the next index to whatever the next reward is (assuming this will be incremented)
                    totalRewards,
                    0,
                    chestStats.Length - 1
                    );
                break;
            case RewardCountType.Random:
                // use luck and weighted chance to determine the amount of rewards
                float playerLuck = recipient.GetComponent<Player>().ActualStats.luck;

                List<(int index, ChestStats stats, float weight)> weightedStats = new List<(int index, ChestStats stats, float weight)>();
                float totalWeight = 0f;
                for (int i = 0; i < chestStats.Length; i++)
                {
                    float weight = chestStats[i].baseDropChance * (1 + chestStats[i].luckScaling * (playerLuck - 1));
                    totalWeight += weight;
                    weightedStats.Add((i, chestStats[i], weight));
                }

                weightedStats.Sort((a, b) => a.weight.CompareTo(b.weight));

                float r = Random.Range(0, totalWeight);
                float cumulative = 0f;

                foreach (var entry in weightedStats)
                {
                    cumulative += entry.weight;
                    if (r <= cumulative)
                    {
                        currentStatIndex = entry.index;
                        return entry.stats;
                    }
                }
                break;
            default:
                break;
        }

        return GetCurrentChestStats(); // if nothing else, just get the current stats
    }

    private int GetRewardCount()
    {
        ChestStats cs = GetNextChestStats();
        if (cs) return cs.numberOfItems;

        return 1; // otherwise
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerInventoryController inventory))
        {
            recipient = inventory;

            int rewardCount = GetRewardCount();

            for (int i = 0; i < rewardCount; i++)
            {
                Open(inventory);
            }

            gameObject.SetActive(false);

            UIChestController.Activate(inventory.GetComponentInChildren<PlayerCollector>(), this);

            totalRewards = (totalRewards + 1) % (chestStats.Length + 1); // set up wrap around incrementing for sequential rewards
        }
    }

    void Open(PlayerInventoryController inventory)
    {
        if (inventory == null) return;

        // go down this list until it works, the order matters!
        if (possibleReward.HasFlag(RewardType.Evolution) && TryEvolve<Weapon>(inventory)) return;
        if (possibleReward.HasFlag(RewardType.UpgradeWeapon) && TryEvolve<Weapon>(inventory)) return;
        if (possibleReward.HasFlag(RewardType.UpgradePassive) && TryEvolve<Passive>(inventory)) return;
        if (possibleReward.HasFlag(RewardType.NewWeapon) && TryGive<WeaponStats>(inventory)) return;
        if (possibleReward.HasFlag(RewardType.NewPassive) && TryGive<PassiveStats>(inventory)) return;
    }

    T TryGive<T>(PlayerInventoryController inventory) where T : ItemStats
    {
        T[] possibilities = inventory.GetAllUnownedItemsFor<T>();
        if (possibilities.Length == 0) return null;
        T t = possibilities[Random.Range(0, possibilities.Length)]; // grab and add a random item
        inventory.Add(t);
        UIChestController.NotifyItemRecieved(t.icon);
        return t;
    }

    T TryUpgrade<T>(PlayerInventoryController inventory) where T : Item
    {
        T[] upgradables = inventory.GetUpgradables<T>();
        if (upgradables.Length == 0) return null;
        T t = upgradables[Random.Range(0, upgradables.Length)]; // grab and upgrade a random item
        inventory.LevelUpItem(t);
        UIChestController.NotifyItemRecieved(t.statsData.icon);
        return t;
    }


    T TryEvolve<T>(PlayerInventoryController inventory) where T : Item
    {
        T[] evolvables = inventory.GetEvolvables<T>();
        foreach (Item i in evolvables)
        {
            ItemStats.Evolution[] possibleEvolutions = i.CanEvolve();
            foreach (ItemStats.Evolution e in possibleEvolutions)
            {
                if (i.AttemptEvolution(e, 0))
                {
                    UIChestController.NotifyItemRecieved(e.outcome.itemType.icon);
                    return i as T; // we only care about the first evolution we see
                }
            }
        }

        return null;
    }


    //public ChestStats stats;

    //public ChestStats GetStats() { return stats; }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        PlayerInventoryController p = collision.GetComponent<PlayerInventoryController>();
    //        if (p)
    //        {
    //            Open(p);
    //            Destroy(gameObject);
    //        }
    //    }
    //}

    //public void Open(PlayerInventoryController inventory, bool isHigherTier = false)
    //{
    //    Debug.Log("Chest opened!");

    //    // roll a random amount of rewards to give, based on player luck and tier

    //    // check to see if the player has possible evolutions, if so, fill one of the rewards with that
    //    foreach (PlayerInventoryController.Slot s in inventory.weaponSlots)
    //    {
    //        WeaponB w = s.item as WeaponB;
    //        if (!w || w.statsData.evolutionData == null) continue;

    //        foreach (ItemStatsB.Evolution e in w.statsData.evolutionData)
    //        {
    //            if (e.evoReq == ItemStatsB.Evolution.EvolutionRequirements.TreasureChest)
    //            {
    //                bool attempt = w.AttemptEvolution(e, 0);
    //                if (attempt) return;
    //            }
    //        }
    //    }

    //    // populate all remaining rewards with items the player already has
    //    // level up all selected items

    //    // if any rewards couldn't be filled because all player weapons are fully upgraded, populate with gold
    //}
}
