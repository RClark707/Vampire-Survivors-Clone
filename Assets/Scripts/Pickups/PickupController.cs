using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{
    [System.Serializable]
    public class Pickup
    {
        public GameObject itemPrefab;
        public float dropChance; // these need not add to 100
    }

    public List<Pickup> pickups;

    private void OnDestroy()
    {
        DropItem();
    }

    void DropItem()
    {
        Pickup pickup = null;

        float chance = UnityEngine.Random.Range(0f, 1f);

        for (int i = 0; i < pickups.Count; i++)
        {
            float weightedChance = GetWeightedChance(i);
            // Debug.Log($"Comparing {chance} with {weightedChance}");

            if (chance <= weightedChance)
            {
                pickup = pickups[i];
                break;
            }
        }

        if (pickup != null)
        {
            Debug.Log($"The {name} dropped a {pickup.itemPrefab.name}!");
            Instantiate(pickup.itemPrefab, transform.position, Quaternion.identity);
        }
    }

    float GetWeightedChance(int index)
    {
        Player player = FindAnyObjectByType<Player>();
        float sum = 0f;

        foreach (Pickup p in pickups)
        {
            sum += p.dropChance * player.luck;
        }

        float cumulative = 0f;

        if (index == pickups.Count - 1)
        {
            return 1f;
        }

        for (int i = 0; i <= index + 1; i++)
        {
            cumulative += pickups[i].dropChance * player.luck;
        }

        return cumulative / sum;
    }
}
