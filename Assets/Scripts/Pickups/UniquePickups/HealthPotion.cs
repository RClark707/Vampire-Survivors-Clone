using UnityEngine;

public class HealthPotion : Pickup
{
    public override void Awake()
    {
        base.Awake();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    public override void Follow(Transform target)
    {
        base.Follow(target);
    }

    public override void Collect(Player player)
    {
        player.RestoreHealth(strength);
        base.Collect(player);
    }
}
