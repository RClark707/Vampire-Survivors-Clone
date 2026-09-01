using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
    Player player;
    CircleCollider2D collector;

    private void Start()
    {
        player = FindAnyObjectByType<Player>();
        collector = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        collector.radius = player.magnet;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IPickup pickup))
        {
            pickup.Follow(player.transform);
        }
    }
}
