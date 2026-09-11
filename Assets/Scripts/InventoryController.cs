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

    public List<WeaponController> GetActiveWeapons()
    {
        List<WeaponController> active = new List<WeaponController>();

        foreach (WeaponController wc in weaponSlots)
        {
            if (wc != null) active.Add(wc);
        }

        return active;
    }

    public List<Passive> GetActivePassives()
    {
        List<Passive> active = new List<Passive>();

        foreach (Passive p in passiveSlots)
        {
            if (p != null) active.Add(p);
        }

        return active;
    }

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

        if (GameController.Instance != null && GameController.Instance.choosingUpgrades)
        {
            GameController.Instance.EndPlayerLevelUp();
        }
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
        if (GameController.Instance != null && GameController.Instance.choosingUpgrades)
        {
            GameController.Instance.EndPlayerLevelUp();
        }
    }

    private void Start()
    {
        // StartCoroutine(LevelUpItems());
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
