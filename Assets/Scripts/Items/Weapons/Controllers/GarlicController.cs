using UnityEngine;

public class GarlicController : WeaponController
{
    protected override void Use()
    {
        base.Use();
        GameObject go = Instantiate(stats.WeaponPrefab, transform.position, Quaternion.identity);
        go.transform.SetParent(transform); // the weapon is parented to the controller

        // SET WEAPON STATS
        GarlicBehavior behavior = go.GetComponent<GarlicBehavior>();
        behavior.gameObject.name = stats.name;
        behavior.damage = damage;
        behavior.projectileSpeed = projectileSpeed;
        behavior.cooldown = maxCooldown;
        behavior.pierceCount = pierceCount;
    }
}
