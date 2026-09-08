using UnityEngine;

public class WeaponController : MonoBehaviour, IItem
{
    [Header("Weapon Stats")]
    public WeaponStats stats;
    float maxCooldown;
    float currentCooldown;
    protected PlayerMovement pm;

    public bool MaxLevelReached()
    {
        return stats.Level == stats.upgrades.Count + 1; // this means that you need at least one upgrade to be upgradeable
    }

    public void UpgradeItem()
    {
        return;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        name = stats.name + " Controller";
        maxCooldown = stats.Cooldown;
        currentCooldown = maxCooldown;
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
        Debug.Log($"Using {name}!");
        return;
    }
}
