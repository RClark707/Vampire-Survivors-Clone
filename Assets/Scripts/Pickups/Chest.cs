using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    InventoryController inv;
    RewardsController rew;

    void Start()
    {
        inv = FindAnyObjectByType<InventoryController>();
        rew = FindAnyObjectByType<RewardsController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Open();
            Destroy(gameObject);
        }
    }

    public void Open()
    {
        Debug.Log("Chest Opened!");

        // roll a random amount of rewards to give, based on player luck

        // check to see if the player has possible evolutions, if so, fill one of the rewards with that
        List<WeaponController> possibleEvolutions = inv.GetPossibleEvolutions();
        if (possibleEvolutions.Count > 0)
        {
            // select a random evolution to evolve
            WeaponController weaponToEvolve = possibleEvolutions[Random.Range(0, possibleEvolutions.Count)];
            int slotIndex = inv.weaponSlots.IndexOf(weaponToEvolve);
            inv.LevelUpWeapon(slotIndex);
        }

        // populate all remaining rewards with items the player already has
        // level up all selected items

        // if any rewards couldn't be filled because all player weapons are fully upgraded, populate with gold
    }
}
