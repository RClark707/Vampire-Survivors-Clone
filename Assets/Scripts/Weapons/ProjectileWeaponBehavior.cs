using UnityEngine;

public class ProjectileWeaponBehavior : WeaponBehavior
{
    protected Vector3 projectileDirection;

    protected override void Awake()
    {
        base.Awake();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
    }

    public void SetProjectileDirectionAndRotation(Vector3 dir)
    {
        projectileDirection = dir;

        float angle = Mathf.Atan2(projectileDirection.y, projectileDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90); // this hard codes a value, and assumes all weapons face UP! We can use variables instead
    }

    // We override to update the projectile piercing
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        // base.OnTriggerEnter2D(collision);
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            Debug.Log($"A {name} just hit a {enemy.name}");
            enemy.TakeDamage(damage);
            UpdatePierceCount();
        }
        else if (collision.CompareTag("Prop"))
        {
            if (collision.TryGetComponent(out BreakableProps prop))
            {
                prop.TakeDamage(damage);
                UpdatePierceCount();
            }
        }
    }

    void UpdatePierceCount()
    {
        pierceCount--;

        if (pierceCount <= 0)
        {
            Destroy(gameObject);
        }
    }
}
