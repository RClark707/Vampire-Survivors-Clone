using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public List<WeaponController> weaponSlots = new List<WeaponController>(6);
    public List<Passive> passiveSlots = new List<Passive>(6);

    // TODO: Instead of what is implemented, use:
    // get the horizontal box of weapon slots
    // get the horizontal box of passive slots
    public List<Image> weaponUISlots = new List<Image>(6);
    public List<Image> passiveUISlots = new List<Image>(6);

    [HideInInspector]
    public Transform weaponsParent;
    [HideInInspector]
    public Transform passivesParent;

    public void AddWeapon(int slotIndex, WeaponController weapon)
    {
        weaponSlots[slotIndex] = weapon;
        weaponUISlots[slotIndex].sprite = weapon.stats.icon;
        weaponUISlots[slotIndex].enabled = true;
    }

    public void AddPassive(int slotIndex, Passive passive)
    {
        passiveSlots[slotIndex] = passive;
        passiveUISlots[slotIndex].sprite = passive.stats.icon;
        passiveUISlots[slotIndex].enabled = true;
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
