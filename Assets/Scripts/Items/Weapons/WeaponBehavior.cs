using UnityEngine;

public class WeaponBehavior : MonoBehaviour
{
    [Header("Weapon Stats")]
    public WeaponStats weaponStats;
    public float weaponDuration;

    // Current Stats
    protected float damage;
    protected float speed;
    protected float cooldown;
    protected int pierceCount;

    [Header("Player Reference")]
    Player player;

    protected virtual void Awake()
    {
        name = weaponStats.name;
        damage = weaponStats.Damage;
        speed = weaponStats.Speed;
        cooldown = weaponStats.Cooldown;
        pierceCount = weaponStats.PierceCount;

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
        return damage *= player.might;
    }

    // We make these virtual in case we need to override them later
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Entity e))
        {
            Debug.Log($"A {name} just hit an {e.name}");
            e.TakeDamage(GetCurrentDamage());
        }
    }
}
