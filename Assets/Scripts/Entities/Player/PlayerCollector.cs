using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
    Player player;
    CircleCollider2D collector;

    public delegate void OnCoinCollected();
    public OnCoinCollected onCoinCollected;

    float coins;

    public float GetCoins() { return coins; }

    private void Start()
    {
        player = FindAnyObjectByType<Player>();
        collector = GetComponent<CircleCollider2D>();
        coins = 0;
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

    public float AddCoins(float amount)
    {
        coins += amount;
        onCoinCollected();
        return coins;
    }

    public void SaveCoinsToStash()
    {
        SaveController.LastLoadedGameData.coins += coins;
        coins = 0;
        SaveController.Save();
    }
}
