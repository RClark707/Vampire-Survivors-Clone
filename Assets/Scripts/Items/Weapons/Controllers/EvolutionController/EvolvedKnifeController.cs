using UnityEngine;

public class EvolvedKnifeController : WeaponController
{
    protected override void Use()
    {
        base.Use();
        GameObject go = Instantiate(stats.WeaponPrefab, transform.position, Quaternion.identity);

        // SET WEAPON STATS
        EvolvedKnifeBehavior behavior = go.GetComponent<EvolvedKnifeBehavior>();
        behavior.gameObject.name = stats.name;
        behavior.damage = damage;
        behavior.projectileSpeed = projectileSpeed;
        behavior.cooldown = maxCooldown;
        behavior.pierceCount = pierceCount;

        go.GetComponent<EvolvedKnifeBehavior>().SetProjectileDirectionAndRotation(pm.lastMoveDirection);
    }
}