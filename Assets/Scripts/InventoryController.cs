using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public List<WeaponController> weaponSlots = new List<WeaponController>(6);
    public List<Passive> passiveSlots = new List<Passive>(6);

    public List<Image> weaponUISlots = new List<Image>(6);
    public List<Image> passiveUISlots = new List<Image>(6);

    [HideInInspector]
    public Transform weaponsParent;
    [HideInInspector]
    public Transform passivesParent;

    Player player;

    public List<WeaponController> GetActiveWeapons()
    {
        List<WeaponController> active = new List<WeaponController>();

        foreach (WeaponController wc in weaponSlots)
        {
            if (wc != null) active.Add(wc);
        }

        return active;
    }

    //public List<Passive> GetActivePassives()
    //{
    //    List<Passive> active = new List<Passive>();

    //    foreach (Passive p in passiveSlots)
    //    {
    //        if (p != null) active.Add(p);
    //    }

    //    return active;
    //}

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

        WeaponController item = weaponSlots[slotIndex];

        if (item.IsUpgradeable())
        {
            Debug.Log($"The {item.name} is about to level up.");
            item.UpgradeItem();
        }
        else
        {
            Debug.Log($"The {item.name} item is already at its maximum level of {item.Level}!");

            if (GetPossibleEvolutions().Contains(item)) // is it eligible for evolution?
            {
                Debug.Log($"Instead, the {item.name} will be evolved!");
                GameObject evolvedController = Instantiate(item.stats.EvolvedWeaponController, player.transform.position, Quaternion.identity); // adds the controller to the game
                evolvedController.transform.SetParent(weaponsParent); // places the controller beneath the player, with all other controllers
                AddWeapon(slotIndex, evolvedController.GetComponent<WeaponController>()); // adds the weapon to the UI
                Destroy(item.gameObject); // removes the old weapon controller from the game scene
            }
        }

        if (GameController.Instance != null && GameController.Instance.choosingUpgrades)
        {
            GameController.Instance.EndPlayerLevelUp();
        }
    }

    public void LevelUpPassive(int slotIndex)
    {
        Passive item = passiveSlots[slotIndex];

        if (item.IsUpgradeable())
        {
            Debug.Log($"The {item.name} is about to level up.");
            item.UpgradeItem();
        }
        else
        {
            Debug.Log($"The {item.name} item is already at its maximum level of {item.Level}!");
            // this is where you can add functionality for evolving passive items
        }

        if (GameController.Instance != null && GameController.Instance.choosingUpgrades)
        {
            GameController.Instance.EndPlayerLevelUp();
        }
    }

    public List<WeaponController> GetPossibleEvolutions()
    {
        // find all possible evolutions based on the current inventory
        List<WeaponController> possibleEvolutions = new List<WeaponController>();

        foreach (WeaponController wc in weaponSlots) // for each slot we have (weapons)
        {
            if (wc != null) // is the slot filled with a weapon?
            {
                if (wc.HasEvolution() && !wc.IsUpgradeable()) // does the weapon have an evolution and is it max level?
                {
                    Debug.Log($"We found a {wc.name} that can evolve, but do you have the right passive?");
                    if (wc.stats.CatalystPassive != null) // does it require a catalyst?
                    {
                        foreach (Passive p in passiveSlots) // for each slot we have (passives)
                        {
                            if (p != null) // is the slot filled with a passive?
                            {
                                if (wc.stats.CatalystPassive == p.stats) // do we have the correct item to evolve with?
                                {
                                    // this is where we can implement checking for the correct level of the passive
                                    // we can evolve!
                                    Debug.Log($"You have the correct passive, {p.name}, to evolve your {wc.name}!");
                                    possibleEvolutions.Add(wc);
                                }
                                else
                                {
                                    Debug.Log($"We checked your {p.name}, but that doesn't match {wc.stats.CatalystPassive.name}");
                                }
                            }
                        }
                    }
                    else // then the weapon doesn't require a catalyst
                    {
                        Debug.Log($"Your {wc.name} can evolve all on its own!");
                        possibleEvolutions.Add(wc);
                    }
                }
            }
        }

        return possibleEvolutions;
    }

    // private void Update()
    // {
    //     if (GetPossibleEvolutions().Count > 0)
    //     {
    //         Debug.Log("There are possible evolutions available!");
    //     }
    // }

    private void Start()
    {
        player = FindAnyObjectByType<Player>();
    }

    //IEnumerator LevelUpItems()
    //{
    //    yield return new WaitForSeconds(2);

    //    LevelUpPassive(0);
    //    LevelUpPassive(1);
    //    LevelUpWeapon(0);
    //    LevelUpWeapon(1);
    //}
}
