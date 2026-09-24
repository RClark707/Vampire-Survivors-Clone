using UnityEngine;

public class WhipWeapon : ProjectileWeaponB
{
    int currentSpawnCount;
    float currentSpawnYOffset;

    protected override bool Attack(int attackAmount = 1)
    {
        if (!currentStats.projectilePrefab)
        {
            Debug.LogError($"Projectile prefab is missing for {name}");
            currentCooldown = statsData.baseStats.cooldown;
            return false;
        }

        if (!CanAttack()) return false;

        if (currentCooldown <= 0)
        {
            currentSpawnCount = 0;
            currentSpawnYOffset = 0f;
        }

        float spawnDir = Mathf.Sign(pm.lastMoveDirection.x) * (currentSpawnCount % 2 != 0 ? -1 : 1); // face left or right?
        Vector2 spawnOffset = new Vector2(
            spawnDir * Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            currentSpawnYOffset
            );

        Projectile prefab = Instantiate(
            currentStats.projectilePrefab,
            owner.transform.position + (Vector3)spawnOffset,
            Quaternion.identity
            );
        prefab.owner = owner;

        if (spawnDir < 0)
        {
            prefab.transform.localScale = new Vector3(
                -Mathf.Abs(prefab.transform.localScale.x), // flip on the x-axis
                prefab.transform.localScale.y,
                prefab.transform.localScale.z
                );

            // Debug.Log(spawnDir + " | " + prefab.transform.localScale);
        }

        prefab.weapon = this;
        currentCooldown = statsData.baseStats.cooldown;
        attackAmount--;

        // determine where to spawn the projectile
        currentSpawnCount++;
        if (currentSpawnCount > 1 && currentSpawnCount % 2 == 0)
        {
            currentSpawnYOffset += 1;
        }

        if (attackAmount > 0)
        {
            currentAttackCount = attackAmount; // this determines if CanAttack() evals to true
            currentAttackInterval = statsData.baseStats.projectileInterval;
        }

        return true;
    }
}
