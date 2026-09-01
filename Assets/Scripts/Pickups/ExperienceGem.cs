using UnityEngine;

public class ExperienceGem : MonoBehaviour, IPickup
{
    public PickupStats pickupStats;
    int xpQuantity;

    private void Awake()
    {
        name = pickupStats.name;
        xpQuantity = (int)pickupStats.PickupStrength;
    }

    public void Pickup()
    {
        Player player = FindAnyObjectByType<Player>();
        player.GainXP(xpQuantity);
        Destroy(gameObject);
    }
}
