using Terresquall;
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
        if (collision.TryGetComponent(out Pickup pickup))
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
        onCoinCollected?.Invoke();
        return coins;
    }

    public void SaveCoinsToStash(bool async = false)
    {
        // SaveController.Instance.AddCoins(coins);
        SaveController.Instance.SavedData.totalCoins += coins;
        if (async) Bench.SaveGameAsync();
        else Bench.SaveGame();
    }

    public void ResetRunCoins()
    {
        coins = 0;
        onCoinCollected?.Invoke();
    }
}
