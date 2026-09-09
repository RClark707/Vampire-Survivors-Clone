using System.Collections;
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
        //if (weaponSlots[slotIndex].MaxLevelReached() && weaponSlots[slotIndex].stats.EvolutionPrefab != null)
        //{
        //    Debug.Log($"The {weaponSlots[slotIndex].name} is at maximum level and can be evolved.");
        //    // evolve the weapon
        //    WeaponController wc = weaponSlots[slotIndex];
        //    GameObject evolution = Instantiate(wc.stats.EvolutionPrefab, transform.position, Quaternion.identity);
        //    evolution.transform.SetParent(weaponsParent);
        //    AddWeapon(slotIndex, evolution.GetComponent<WeaponController>());
        //    Destroy(wc.gameObject);
        //}
        //else if (weaponSlots[slotIndex].MaxLevelReached())
        //{
        //    Debug.Log($"The {weaponSlots[slotIndex].name} is at maximum level and cannot be evolved.");
        //}
        //else
        //{
        //    Debug.Log($"The {weaponSlots[slotIndex].name} is about to level up.");
        //    weaponSlots[slotIndex].UpgradeItem();
        //}

        Debug.Log($"The {weaponSlots[slotIndex].name} is about to level up.");
        weaponSlots[slotIndex].UpgradeItem();
    }

    public void LevelUpPassive(int slotIndex)
    {
        //if (passiveSlots[slotIndex].MaxLevelReached())
        //{
        //    Debug.Log($"The {passiveSlots[slotIndex].name} is at maximum level and cannot be evolved.");
        //    // add functionality for evolving passive items here
        //    return;
        //}
        //else
        //{
        //    Debug.Log($"The {passiveSlots[slotIndex].name} is about to level up.");
        //    passiveSlots[slotIndex].UpgradeItem();
        //}

        Debug.Log($"The {passiveSlots[slotIndex].name} is about to level up.");
        passiveSlots[slotIndex].UpgradeItem();
    }

    private void Start()
    {
        StartCoroutine(LevelUpItems());
    }

    IEnumerator LevelUpItems()
    {
        yield return new WaitForSeconds(2);

        LevelUpPassive(0);
        LevelUpPassive(1);
        LevelUpWeapon(0);
        LevelUpWeapon(1);
    }
}
