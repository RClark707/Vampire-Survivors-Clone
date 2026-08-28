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
    }
}
