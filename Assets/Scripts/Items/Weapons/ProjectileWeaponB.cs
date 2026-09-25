using UnityEngine;

public class ProjectileWeaponB : WeaponB
{
    protected float currentAttackInterval;
    protected int currentAttackCount;

    protected override void Update()
    {
        base.Update();

        // if the weapon is actively firing multiple projectiles, we are allowed to keep attacking
        currentAttackInterval -= Time.deltaTime;
        if (currentAttackInterval <= 0)
        {
            Attack(currentAttackCount); // writing it this way ONLY functions if we add code to the Attack() function to reset the currentAttackInterval
        }
    }

    public override bool CanAttack()
    {
        if (currentAttackCount > 0) return true;
        return base.CanAttack();
    }

    protected override bool Attack(int attackAmount = 1)
    {
        if (!currentStats.projectilePrefab)
        {
            Debug.LogError($"Projectile prefab has not yet been set for the {name}.");
            currentCooldown = statsData.baseStats.cooldown;
            return false;
        }

        if (!CanAttack()) return false;

        // if there is a proc effect, play it on the player
        if (currentStats.procEffect)
        {
            Destroy(Instantiate(currentStats.procEffect, owner.transform), 5f);
        }

        float spawnAngle = GetSpawnAngle();

        Projectile prefab = Instantiate(
            currentStats.projectilePrefab,
                owner.transform.position + (Vector3)GetSpawnOffset(spawnAngle),
                Quaternion.Euler(0, 0, spawnAngle)
        );

        prefab.weapon = this;
        prefab.owner = owner;

        if (currentCooldown <= 0) // did our cooldown run out?
        {
            currentCooldown += currentStats.cooldown;
        }

        if (attackAmount > 0) // should we make another attack?
        {
            currentAttackCount = attackAmount;
            currentAttackInterval = statsData.baseStats.projectileInterval;
        }

        return true;
    }

    protected virtual float GetSpawnAngle()
    {
        return Mathf.Atan2(pm.lastMoveDirection.y, pm.lastMoveDirection.x) * Mathf.Rad2Deg;
    }

    protected virtual Vector2 GetSpawnOffset(float spawnAngle = 0f)
    {
        return Quaternion.Euler(0f, 0f, spawnAngle) * new Vector2(
            Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax)
            );
    }
}
