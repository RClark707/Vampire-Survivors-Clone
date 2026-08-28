using UnityEngine;

public class KnifeController : WeaponController
{
    protected override void Use()
    {
        base.Use();
        GameObject go = Instantiate(weaponStats.WeaponPrefab, transform.position, Quaternion.identity);
        go.GetComponent<KnifeBehavior>().SetProjectileDirectionAndRotation(pm.lastMoveDirection);
    }
}
