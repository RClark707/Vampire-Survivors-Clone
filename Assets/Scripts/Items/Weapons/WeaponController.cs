using UnityEngine;

public class WeaponController : MonoBehaviour, IItem
{
    [Header("Weapon Stats")]
    public WeaponStats stats;
    [SerializeField]
    protected int level;
    protected float damage;
    protected float projectileSpeed;
    protected float maxCooldown;
    float currentCooldown;
    protected int pierceCount;

    // protected Player player;
    protected PlayerMovement pm;

    public bool MaxLevelReached()
    {
        // this means that you need at least one upgrade to be upgradeable
        return stats.upgrades.Count != 0 && (level == stats.upgrades.Count + 1);
    }

    public bool IsUpgradeable()
    {
        // TODO: When this function runs, it evaluates level as 0, even when level is 1. Why?
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

        //if (player == null)
        //{
        //    Debug.Log("No player reference!");
        //    return;
        //}

        ItemStats.LevelUpUpgrades levelUp = stats.upgrades[level - 1]; // if item is level 1 (all items start at level 1), we get the first upgrade in the list

        foreach (ItemStats.LevelUpUpgrades.Upgrade upgrade in levelUp.statUpgrades)
        {
            // the functionality of this code allows you to apply the same modifier to both a player & weapon stat at a single time!
            // ApplyPlayerStatModifier(upgrade.playerStatUpgrade, upgrade.multiplier);
            ApplyWeaponStatModifier(upgrade.weaponStatUpgrade, upgrade.multiplier);
        }

        level++;
        Debug.Log($"The {name} is now level {level}.");
    }

    private void Awake()
    {
        name = stats.name + " Controller";
        maxCooldown = stats.Cooldown;
        currentCooldown = maxCooldown;
        damage = stats.Damage;
        projectileSpeed = stats.ProjectileSpeed;
        level = stats.Level;
        // player = FindAnyObjectByType<Player>();
        pm = FindAnyObjectByType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        currentCooldown -= Time.deltaTime;

        if (currentCooldown > 0f)
        {
            return;
        }

        currentCooldown = maxCooldown;

        // Then Use the Weapon
        Use();
    }

    // This method is meant to be overridden
    // and we can return a bool if we want to check that it runs successfully
    protected virtual void Use()
    {
        // Debug.Log($"Using {name}!");

        // SET WEAPON STATS
        //WeaponBehavior behavior = go.GetComponent<WeaponBehavior>();
        //behavior.gameObject.name = stats.name;
        //behavior.damage = damage;
        //behavior.projectileSpeed = projectileSpeed;
        //behavior.cooldown = maxCooldown;
        //behavior.pierceCount = pierceCount;

        return;
    }

    protected void ApplyWeaponStatModifier(ItemStats.WeaponStats stat, float multiplier)
    {
        Debug.Log($"Applying a {multiplier}% multiplier to the {name}'s {stat}");

        switch (stat)
        {
            case ItemStats.WeaponStats.Damage:
                damage *= 1 + multiplier / 100f;
                break;
            case ItemStats.WeaponStats.Projectile_Speed:
                projectileSpeed *= 1 + multiplier / 100f;
                break;
            case ItemStats.WeaponStats.Cooldown:
                maxCooldown *= 1 + multiplier / 100f;
                break;
            case ItemStats.WeaponStats.Pierce_Count:
                pierceCount *= Mathf.RoundToInt(1 + multiplier / 100f); // THIS IS IMPORTANT FUNCTIONALITY
                break;
            default:
                Debug.Log($"No modifier to apply to weapon stat type of {stat}");
                break;
        }
    }

    /// <summary>
    /// This function applies a percentage multiplier to a single specific stat. 
    /// The multiplier is given as a percentage increase and then converted to an actual multiplier. 
    /// Example: entering 50 as the multiplier, it is covnerted to 1 + 50/100 = 1.5 for the actual multiplier used in calculations.
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="multiplier"></param>
    //protected void ApplyPlayerStatModifier(ItemStats.PlayerStats stat, float multiplier)
    //{
    //    switch (stat)
    //    {
    //        case ItemStats.PlayerStats.Armor:
    //            player.Armor *= 1 + multiplier / 100f;
    //            break;
    //        case ItemStats.PlayerStats.Area:
    //            player.Area *= 1 + multiplier / 100f;
    //            break;
    //        case ItemStats.PlayerStats.Growth:
    //            player.Growth *= 1 + multiplier / 100f;
    //            break;
    //        case ItemStats.PlayerStats.Health:
    //            player.MaxHealth *= 1 + multiplier / 100f;
    //            break;
    //        case ItemStats.PlayerStats.Luck:
    //            player.Luck *= 1 + multiplier / 100f;
    //            break;
    //        case ItemStats.PlayerStats.Magnet:
    //            player.Magnet *= 1 + multiplier / 100f;
    //            break;
    //        case ItemStats.PlayerStats.Might:
    //            player.Might *= 1 + multiplier / 100f;
    //            break;
    //        case ItemStats.PlayerStats.Speed:
    //            player.MovementSpeed *= 1 + multiplier / 100f;
    //            break;
    //        default:
    //            Debug.Log($"No modifier to apply to player stat type of {stat}");
    //            break;
    //    }
    //}
}
