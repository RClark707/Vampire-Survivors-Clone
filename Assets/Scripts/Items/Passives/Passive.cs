using UnityEngine;

public class Passive : MonoBehaviour, IItem
{
    [Header("Passive Stats")]
    public PassiveStats stats;

    protected Player player;

    public bool MaxLevelReached()
    {
        return stats.Level == stats.upgrades.Count + 1;
    }

    public void UpgradeItem()
    {
        return;
    }

    protected void ApplyModifier()
    {
        switch (stats.Stat)
        {
            case ItemStats.PlayerStats.Armor:
                player.armor *= 1 + stats.Multiplier / 100f;
                break;
            case ItemStats.PlayerStats.Area:
                player.area *= 1 + stats.Multiplier / 100f;
                break;
            case ItemStats.PlayerStats.Growth:
                player.growth *= 1 + stats.Multiplier / 100f;
                break;
            case ItemStats.PlayerStats.Health:
                player.maxHealth *= 1 + stats.Multiplier / 100f;
                break;
            case ItemStats.PlayerStats.Luck:
                player.luck *= 1 + stats.Multiplier / 100f;
                break;
            case ItemStats.PlayerStats.Magnet:
                player.magnet *= 1 + stats.Multiplier / 100f;
                break;
            case ItemStats.PlayerStats.Might:
                player.might *= 1 + stats.Multiplier / 100f;
                break;
            case ItemStats.PlayerStats.Speed:
                player.movementSpeed *= 1 + stats.Multiplier / 100f;
                break;
            default:
                Debug.Log("No matching stat type for this passive");
                break;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<Player>();
        ApplyModifier();
    }
}
