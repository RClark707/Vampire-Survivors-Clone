using UnityEngine;

public class Passive : MonoBehaviour, IItem
{
    [Header("Passive Stats")]
    public PassiveStats stats;
    [SerializeField]
    protected int level;
    protected Player player;

    public bool MaxLevelReached()
    {
        return stats.upgrades.Count != 0 && (level == stats.upgrades.Count + 1);
    }

    public bool IsUpgradeable()
    {
        // Debug.Log($"So, can we upgrade? {level - 1 < stats.upgrades.Count}");
        return level - 1 < stats.upgrades.Count;
    }

    public void UpgradeItem()
    {
        Debug.Log($"The {name} is currently level {level}.");

        if (!IsUpgradeable())
        {
            Debug.Log($"Your {name} item is already at its maximum level of {level}!");
            // Debug.Log($"The {name} has {stats.upgrades.Count} total upgrades available.");
            return;
        }

        if (player == null)
        {
            Debug.Log("No player reference!");
            return;
        }

        ItemStats.LevelUpUpgrades levelUp = stats.upgrades[level - 1]; // if item is level 1 (all items start at level 1), we get the first upgrade in the list

        foreach (ItemStats.LevelUpUpgrades.Upgrade upgrade in levelUp.statUpgrades)
        {
            // the functionality of this code allows you to apply the same modifier to both a player & weapon stat at a single time!
            ApplyPlayerStatModifier(upgrade.playerStatUpgrade, upgrade.multiplier);
            // ApplyWeaponStatModifier(upgrade.weaponStatUpgrade, upgrade.multiplier);
        }

        level++;
        Debug.Log($"The {name} is now level {level}.");
    }

    private void Awake()
    {
        name = stats.name;
        level = stats.Level;
        player = FindAnyObjectByType<Player>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ApplyPlayerStatModifier(stats.Stat, stats.Multiplier);
    }

    /// <summary>
    /// This function applies a percentage multiplier to a single specific stat. 
    /// The multiplier is given as a percentage increase and then converted to an actual multiplier. 
    /// Example: entering 50 as the multiplier, it is covnerted to 1 + 50/100 = 1.5 for the actual multiplier used in calculations.
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="multiplier"></param>
    protected void ApplyPlayerStatModifier(ItemStats.PlayerStats stat, float multiplier)
    {
        Debug.Log($"Applying a {multiplier}% modifier to your {stat}");

        switch (stat)
        {
            case ItemStats.PlayerStats.Armor:
                player.armor *= 1 + multiplier / 100f;
                break;
            case ItemStats.PlayerStats.Area:
                player.area *= 1 + multiplier / 100f;
                break;
            case ItemStats.PlayerStats.Growth:
                player.growth *= 1 + multiplier / 100f;
                break;
            case ItemStats.PlayerStats.Health:
                player.maxHealth *= 1 + multiplier / 100f;
                break;
            case ItemStats.PlayerStats.Luck:
                player.luck *= 1 + multiplier / 100f;
                break;
            case ItemStats.PlayerStats.Magnet:
                player.magnet *= 1 + multiplier / 100f;
                break;
            case ItemStats.PlayerStats.Might:
                player.might *= 1 + multiplier / 100f;
                break;
            case ItemStats.PlayerStats.Speed:
                player.movementSpeed *= 1 + multiplier / 100f;
                break;
            default:
                Debug.Log($"No modifier to apply to player stat type of {stat}");
                break;
        }
    }
}
