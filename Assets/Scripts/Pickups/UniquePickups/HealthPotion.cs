public class HealthPotion : Pickup
{
    public override void Collect(Player player)
    {
        player.RestoreHealth(strength);
        base.Collect(player);
    }
}
