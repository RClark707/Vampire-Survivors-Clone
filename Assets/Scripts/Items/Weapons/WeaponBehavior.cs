using UnityEngine;

public class WeaponBehavior : MonoBehaviour
{
    [Header("Weapon Stats")] // these stats are modified and set by the Weapon Controller when the weapon projectile* is instanced
    public float weaponDuration;
    [HideInInspector]
    public float damage;
    [HideInInspector]
    public float projectileSpeed;
    [HideInInspector]
    public float cooldown; // this value is never used
    [HideInInspector]
    public int pierceCount;

    [Header("Player Reference")]
    Player player;

    protected virtual void Awake()
    {
        player = FindAnyObjectByType<Player>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        // Set the projectile's lifespan
        Destroy(gameObject, weaponDuration);
    }

    public float GetCurrentDamage()
    {
        return damage * player.Might;
    }

    public float GetCurrentProjectileSpeed()
    {
        return projectileSpeed * player.ProjectileSpeed;
    }

    // We make these virtual in case we need to override them later
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Entity e))
        {
            // Debug.Log($"A {name} just hit an {e.name}");
            e.TakeDamage(GetCurrentDamage());
        }
    }
}
