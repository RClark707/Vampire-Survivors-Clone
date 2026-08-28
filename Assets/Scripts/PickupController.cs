using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{
    [System.Serializable]
    public class Pickup
    {
        public string name;
        public GameObject itemPrefab;
        public float dropChance;
    }

    public List<Pickup> pickups;

    private void OnDestroy()
    {
        float rand = UnityEngine.Random.Range(0f, 100f);
        // TODO: Improve the drop chance algo
        foreach (Pickup pickup in pickups)
        {
            if (rand < pickup.dropChance)
            {
                // drop that item
                Debug.Log($"The {name} dropped a {pickup.name}!");
                Instantiate(pickup.itemPrefab, transform.position, Quaternion.identity);
                break;
            }
        }
    }
}
