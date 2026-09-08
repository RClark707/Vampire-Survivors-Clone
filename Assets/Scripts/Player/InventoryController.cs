using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    // can also opt to do a nested class
    public List<WeaponController> weaponSlots = new List<WeaponController>(6);
    // public int[] weaponLevels = new int[6]; // we're opting to use levels ON the item itself
    public List<Passive> passiveSlots = new List<Passive>(6);
    // public int[] passiveLevels = new int[6];

    public Transform weaponsParent;
    public Transform passivesParent;

    public void AddWeapon(int slotIndex, WeaponController weapon)
    {
        weaponSlots[slotIndex] = weapon;
    }

    public void AddPassive(int slotIndex, Passive passive)
    {
        passiveSlots[slotIndex] = passive;
    }

    public void LevelUpWeapon(int slotIndex)
    {
        if (weaponSlots[slotIndex].MaxLevelReached())
        {
            // evolve the weapon
            WeaponController wc = weaponSlots[slotIndex];
            GameObject evolution = Instantiate(wc.stats.EvolutionPrefab, transform.position, Quaternion.identity);
            evolution.transform.SetParent(weaponsParent);
            AddWeapon(slotIndex, evolution.GetComponent<WeaponController>());
            Destroy(wc.gameObject);
        }
        else
        {
            weaponSlots[slotIndex].stats.Level++;
        }
    }

    public void LevelUpPassive(int slotIndex)
    {
        if (passiveSlots[slotIndex].MaxLevelReached())
        {
            Debug.Log("This item can't be upgraded any more!");
            // add functionality for evolving passive items here
            return;
        }
        else
        {
            passiveSlots[slotIndex].stats.Level++;
        }

    }
}
