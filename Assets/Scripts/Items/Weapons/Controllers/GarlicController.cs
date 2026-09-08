using UnityEngine;

public class GarlicController : WeaponController
{
    protected override void Use()
    {
        base.Use();
        GameObject go = Instantiate(stats.WeaponPrefab, transform.position, Quaternion.identity);
        go.transform.SetParent(transform); // the weapon is parented to the controller
    }
}
