using UnityEngine;

public class Coin : Pickup
{
    public int Quantity
    {
        get { return Mathf.RoundToInt(strength); }
        set { strength = value; }
    }

    public override void Collect(Player player)
    {
        PlayerCollector collector = player.GetComponentInChildren<PlayerCollector>();
        if (collector)
        {
            collector.AddCoins(Quantity);
        }
        base.Collect(player);
    }
}
