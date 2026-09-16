using System.Collections.Generic;
using UnityEngine;

public class GarlicBehavior : MeleeWeaponBehavior
{
    List<GameObject> markedEnemies;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        markedEnemies = new List<GameObject>();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Entity e))
        {
            Debug.Log($"A {name} just hit an {e.name}");
            e.TakeDamage(GetCurrentDamage());
            if (e) // is it still alive?
            {
                if (collision.CompareTag("Enemy") && collision.TryGetComponent(out Enemy enemy))
                {
                    // Debug.Log("Applying Knockback after a Projectile collided with an Enemy");
                    enemy.ApplyKnockback(player.transform.position, knockbackAmount);
                }
                markedEnemies.Add(e.gameObject);
            }
        }
    }
}
