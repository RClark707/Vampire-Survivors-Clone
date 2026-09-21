using UnityEngine;

public class Chest : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerInventoryController p = collision.GetComponent<PlayerInventoryController>();
            if (p)
            {
                Open(p);
                Destroy(gameObject);
            }
        }
    }

    public void Open(PlayerInventoryController inventory, bool isHigherTier = false)
    {
        Debug.Log("Chest opened!");

        // roll a random amount of rewards to give, based on player luck and tier

        // check to see if the player has possible evolutions, if so, fill one of the rewards with that
        foreach (PlayerInventoryController.Slot s in inventory.weaponSlots)
        {
            WeaponB w = s.item as WeaponB;
            if (w.statsData.evolutionData == null) continue;

            foreach (ItemStatsB.Evolution e in w.statsData.evolutionData)
            {
                if (e.evoReq == ItemStatsB.Evolution.EvolutionRequirements.TreasureChest)
                {
                    bool attempt = w.AttemptEvolution(e, 0);
                    if (attempt) return;
                }
            }
        }

        // populate all remaining rewards with items the player already has
        // level up all selected items

        // if any rewards couldn't be filled because all player weapons are fully upgraded, populate with gold
    }
}
