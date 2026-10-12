public class ExperienceGem : Pickup
{
    public override void Collect(Player player)
    {
        player.GainXP(strength);
        base.Collect(player);
    }
}
