using UnityEngine;

public class WeaponBehavior : MonoBehaviour
{
    public WeaponStats weaponStats;
    public float weaponDuration;

    // Current Stats
    protected float damage;
    protected float speed;
    protected float cooldown;
    protected int pierceCount;

    protected virtual void Awake()
    {
        name = weaponStats.name;
        damage = weaponStats.Damage;
        speed = weaponStats.Speed;
        cooldown = weaponStats.Cooldown;
        pierceCount = weaponStats.PierceCount;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        // Set the projectile's lifespan
        Destroy(gameObject, weaponDuration);
    }

    // We make these virtual in case we need to override them later
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            Debug.Log($"A {weaponStats.name} just hit a {enemy.name}");
            enemy.TakeDamage(damage);
        }
        else if (collision.CompareTag("Prop"))
        {
            if (collision.TryGetComponent(out BreakableProps prop))
            {
                prop.TakeDamage(damage);
            }
        }
    }
}
