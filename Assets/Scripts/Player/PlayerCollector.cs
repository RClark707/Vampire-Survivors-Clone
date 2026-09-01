using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IPickup pickup))
        {
            Debug.Log($"You picked up a {collision.name}.");
            pickup.Pickup();
        }
    }
}
