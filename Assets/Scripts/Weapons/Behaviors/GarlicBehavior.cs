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
        // base.OnTriggerEnter2D(collision);

        // Is there a way to check if we are a prop/enemy at the same time? using the same base script?
        if (collision.CompareTag("Enemy") && !markedEnemies.Contains(collision.gameObject))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            Debug.Log($"A {weaponStats.name} just hit a {enemy.name}");
            enemy.TakeDamage(damage);
            if (enemy) // is it still alive?
            {
                markedEnemies.Add(enemy.gameObject);
            }
        }
        else if (collision.CompareTag("Prop") && !markedEnemies.Contains(collision.gameObject))
        {
            if (collision.TryGetComponent(out BreakableProps prop))
            {
                prop.TakeDamage(damage);
                if (prop)
                {
                    markedEnemies.Add(prop.gameObject);
                }
            }
        }
    }
}
