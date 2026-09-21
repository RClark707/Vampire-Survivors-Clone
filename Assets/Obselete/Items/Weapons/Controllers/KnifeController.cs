using UnityEngine;

public class KnifeController : WeaponController
{
    protected override void Use()
    {
        base.Use();
        GameObject go = Instantiate(stats.WeaponBehavior, transform.position, Quaternion.identity);

        // SET WEAPON STATS
        KnifeBehavior behavior = go.GetComponent<KnifeBehavior>();
        behavior.gameObject.name = stats.name;
        behavior.damage = damage;
        behavior.projectileSpeed = projectileSpeed;
        behavior.cooldown = maxCooldown;
        behavior.pierceCount = pierceCount;

        go.GetComponent<KnifeBehavior>().SetProjectileDirectionAndRotation(pm.lastMoveDirection);
    }
}