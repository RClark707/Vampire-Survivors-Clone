using UnityEngine;

public class EvolvedKnifeBehavior : ProjectileWeaponBehavior
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        // fling the knife along a direction
        transform.position += projectileDirection * projectileSpeed * Time.deltaTime;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
}
