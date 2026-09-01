using UnityEngine;

public class HealthPotion : MonoBehaviour, IPickup
{
    public PickupStats pickupStats;
    float healingAmount;

    private void Awake()
    {
        name = pickupStats.name;
        healingAmount = pickupStats.PickupStrength;
    }

    public void Pickup()
    {
        Player player = FindAnyObjectByType<Player>();
        player.RestoreHealth(healingAmount);
        Destroy(gameObject);
    }
}
