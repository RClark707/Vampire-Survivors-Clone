using UnityEngine;

public class KnifeBehavior : ProjectileWeaponBehavior
{
    KnifeController kc;

    protected override void Start()
    {
        base.Start();
        kc = FindAnyObjectByType<KnifeController>();
    }

    void Update()
    {
        // fling the knife along a direction
        transform.position += projectileDirection * kc.baseSpeed * Time.deltaTime;
    }
}
