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

    public void SetRadius(float area)
    {
        collector.radius = area;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IPickup pickup))
        {
            if (collision.TryGetComponent(out BobbingAnimation bobbingAnimation))
            {
                bobbingAnimation.bobbing = false;
            }
            pickup.Follow(player.transform);
        }
    }
}
