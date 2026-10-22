using UnityEngine;

/// <summary>
/// This script was written by kyle on the Terresquall blog: https://blog.terresquall.com/community/topic/general-creating-a-king-bible-weapon/
/// The only changes were to sync their script with my own.
/// </summary>
public class RingWeapon : ProjectileWeapon
{
    public void SpawnRing(Player player = null)
    {
        // Only activate this if the player is present.
        if (player)
        {
            float angleOffset = 2 * Mathf.PI / Mathf.Max(1, GetAmount()); // 2 * Mathf.PI = 360. We are just calculating how far of an angle to space out the prefabs from each other on a circle's circumference.
            float currentAngle = 0;
            for (int i = 0; i < GetAmount(); i++)
            {
                // Convert the spawn angle onto a point on the circle's circumference and space it away relative to the player.
                Vector3 spawnPosition = player.transform.position + new Vector3(
                    GetAreaPercentage() * Mathf.Cos(currentAngle),
                    GetAreaPercentage() * Mathf.Sin(currentAngle)
                );

                // Spawn the book at the calculated position, and parents it to the owner so it follows them around.
                Projectile prefab = Instantiate(currentStats.projectilePrefab, spawnPosition, Quaternion.identity, owner.transform);

                prefab.owner = owner;
                currentAngle += angleOffset;
                prefab.weapon = this;
            }
        }
    }

    protected override bool Attack(int attackCount = 1)
    {
        // If no projectile prefab is assigned, leave a warning message.
        if (!currentStats.projectilePrefab)
        {
            Debug.LogError($"Projectile prefab is missing for {name}.");
            ResetWeaponCooldown(true);
            return false;
        }

        // Can we attack?
        if (!CanAttack()) return false;

        SpawnRing(owner);

        ResetWeaponCooldown(true);

        attackCount--;

        // Do we perform another attack?
        if (attackCount > 0)
        {
            currentAttackCount = attackCount - 1;
            currentAttackInterval = ((WeaponStats)statsData).baseStats.projectileInterval;
        }

        return true;
    }

    public override bool ResetWeaponCooldown(bool strict = false)
    {
        // When 'strict' is enabled and the cooldown is not yet finished,
        // do not refresh the cooldown.
        if (strict && currentCooldown > 0) return false;

        // Calculate what the cooldown is going to be, factoring in the cooldown
        // reduction stat in the player character.
        // Cooldown is dependent on lifespan, so we add it.
        float actualCooldown = (currentStats.duration + currentStats.cooldown) * Owner.ActualStats.cooldown;
        // Debug.Log($"The {name} has a cooldown of {currentStats.duration + currentStats.cooldown} modified by our cooldown of {Owner.ActualStats.cooldown}.");

        // Limit the maximum cooldown to the actual cooldown, so we cannot increase
        // the cooldown above the cooldown stat if we accidentally call this function
        // multiple times.
        currentCooldown = Mathf.Min(actualCooldown, currentCooldown + actualCooldown);
        return true;
    }

    public override bool LevelUp(bool updateUI = true)
    {
        base.LevelUp(updateUI);
        // Spawn a ring after every upgrade option, just like in the original game.
        SpawnRing(owner);
        ResetWeaponCooldown(false);
        return true;
    }
}
