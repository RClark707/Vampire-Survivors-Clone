using System.Collections.Generic;
using UnityEngine;

public class LightningRingWeapon : ProjectileWeaponB
{
    List<Enemy> allSelectedEnemies = new List<Enemy>();

    protected override bool Attack(int attackAmount = 1)
    {
        if (!currentStats.hitEffect)
        {
            Debug.LogError($"Hit effect prefab is missing from the {name}.");
            currentCooldown = currentStats.cooldown;
            return false;
        }

        if (!CanAttack()) return false;

        if (currentCooldown <= 0)
        {
            allSelectedEnemies = new List<Enemy>(FindObjectsByType<Enemy>(FindObjectsSortMode.None));
            currentCooldown += currentStats.cooldown;
            currentAttackCount = attackAmount;
        }

        Enemy target = PickEnemy();
        if (target)
        {
            DamageArea(target.transform.position, currentStats.area, GetDamage());

            Instantiate(currentStats.hitEffect, target.transform.position, Quaternion.identity);
        }

        // if there is a proc effect, play it on the player
        if (currentStats.procEffect)
        {
            Destroy(Instantiate(currentStats.procEffect, owner.transform), 5f);
        }

        if (attackAmount > 0)
        {
            currentAttackCount = attackAmount - 1;
            currentAttackInterval = currentStats.projectileInterval;
        }

        return true;
    }

    Enemy PickEnemy()
    {
        Enemy target = null;

        while (!target && allSelectedEnemies.Count > 0)
        {
            int idx = Random.Range(0, allSelectedEnemies.Count);
            target = allSelectedEnemies[idx];

            if (!target)
            {
                allSelectedEnemies.RemoveAt(idx);
                continue; // try again
            }

            // check if the enemy is on screen
            Renderer r = target.GetComponent<Renderer>();
            if (!r || !r.isVisible)
            {
                allSelectedEnemies.Remove(target); // why call this instead of "Remove At index"?
                target = null;
                continue; // try again
            }
        }

        allSelectedEnemies.Remove(target);
        return target;
    }

    void DamageArea(Vector3 position, float radius, float damage)
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(position, radius);
        foreach (Collider2D c in targets)
        {
            Enemy e = c.GetComponent<Enemy>();
            if (e) { e.TakeDamage(damage, transform.position); continue; }
            // if we didn't see an enemy, is there a prop we can damage?
            DestructibleProp dp = c.GetComponent<DestructibleProp>();
            if (dp) { dp.TakeDamage(damage, transform.position); continue; }
        }
    }
}
