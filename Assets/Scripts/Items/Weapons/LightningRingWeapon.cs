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

        if (attackAmount > 0)
        {
            currentAttackCount = attackAmount - 1;
            currentAttackInterval = currentStats.projectileInterval;
        }

        return true;
    }

    Enemy PickTarget()
    {
        Enemy target = null;

        while (!target && allSelectedEnemies.Count > 0)
        {

        }

        return target;
    }

    void DamageArea(Vector3 target, float area, float damage)
    {

    }
}
