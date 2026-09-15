using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardsController : MonoBehaviour
{
    InventoryController inventoryController;
    Player player;

    // These are ALL weapon items in the game
    [Serializable]
    public class WeaponUpgrade
    {
        public GameObject initialWeapon; // this is the weapon controller we need
        public WeaponStats weaponStats;
    }

    // These are ALL passive items in the game
    [Serializable]
    public class PassiveUpgrade
    {
        public GameObject initialPassive;
        public PassiveStats passiveStats;
    }


    // TODO: This can be made into a single GameObject upgradeOption prefab! Then instanced at runtime if we want to
    [Serializable]
    public class UpgradeUI
    {
        public TextMeshProUGUI upgradeNameDisplay;
        public TextMeshProUGUI upgradeDescriptionDisplay;
        public Image upgradeIcon; // combine this with the button as a new prefab!
        public Button upgradeButton;
    }

    public List<WeaponUpgrade> weaponUpgradeOptions = new List<WeaponUpgrade>(); // a list of every weapon in the game
    public List<PassiveUpgrade> passiveUpgradeOptions = new List<PassiveUpgrade>(); // a list of every passive in the game
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>(); // a list of displayed upgrade choices

    void Start()
    {
        inventoryController = FindAnyObjectByType<InventoryController>();
        player = FindAnyObjectByType<Player>();
    }

    void SetUpgradeOptions()
    {
        List<WeaponUpgrade> availabeWeaponUpgrades = new List<WeaponUpgrade>(weaponUpgradeOptions);
        List<PassiveUpgrade> availabePassiveUpgrades = new List<PassiveUpgrade>(passiveUpgradeOptions);

        foreach (var upgradeOption in upgradeUIOptions)
        {
            string nameDisplayText = "";
            string descriptionDisplayText = "";
            Sprite itemIcon = null;

            if (availabeWeaponUpgrades.Count == 0 && availabePassiveUpgrades.Count == 0)
            {
                return;
            }

            // also consider functionality for having all max level items!

            int upgradeType; // randomly pick weapon or passive (0 or 1)

            if (availabeWeaponUpgrades.Count == 0) // there are only passives remaining
            {
                upgradeType = 1;
            }
            else if (availabePassiveUpgrades.Count == 0) // there are only weapons remaining
            {
                upgradeType = 0;
            }
            else
            {
                upgradeType = UnityEngine.Random.Range(0, 2); // this should be a weighted average based on the count of each list
            }

            // PRESENT AN OPTION FOR A WEAPON UPGRADE
            if (upgradeType == 0)
            {
                WeaponUpgrade chosenWeaponUpgrade = availabeWeaponUpgrades[UnityEngine.Random.Range(0, availabeWeaponUpgrades.Count)]; // grab a random option from out list

                availabeWeaponUpgrades.Remove(chosenWeaponUpgrade); // we remove this before the null check because why would we want to keep a null thing?

                if (chosenWeaponUpgrade != null)
                {
                    EnableUpgradeUI(upgradeOption);

                    bool newWeapon = true; // is this a new weapon or an existing one?
                    List<WeaponController> weapons = inventoryController.weaponSlots;

                    for (int i = 0; i < weapons.Count; i++)
                    {
                        if (weapons[i] != null && weapons[i].stats == chosenWeaponUpgrade.weaponStats) // did we randomly pick a weapon we already have
                        {
                            newWeapon = false;

                            if (!weapons[i].IsUpgradeable() && weapons[i].HasEvolution())
                            {
                                // do something here
                                DisableUpgradeUI(upgradeOption);
                                break;
                            }
                            else if (!weapons[i].IsUpgradeable()) // if we can't upgrade any further, don't assign anything
                            {
                                DisableUpgradeUI(upgradeOption);
                                break;
                            }

                            upgradeOption.upgradeButton.onClick.AddListener(() => inventoryController.LevelUpWeapon(i));
                            descriptionDisplayText = weapons[i].stats.upgrades[weapons[i].Level - 1].description; // we want to see the 0th array element for a Level 1 item!
                            nameDisplayText = chosenWeaponUpgrade.weaponStats.name + " Level: " + (weapons[i].Level + 1).ToString();
                            break;
                        }
                    }

                    if (newWeapon) // is this a new weapon to work with? TODO: We also haven't checked for full inventory slots yet!
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => player.AddItem(chosenWeaponUpgrade.initialWeapon));
                        descriptionDisplayText = chosenWeaponUpgrade.weaponStats.Summary;
                        nameDisplayText = chosenWeaponUpgrade.weaponStats.name + " (New)";
                    }

                    upgradeOption.upgradeNameDisplay.text = nameDisplayText;
                    upgradeOption.upgradeDescriptionDisplay.text = descriptionDisplayText;
                    upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.weaponStats.icon;
                }
            }
            // PRESENT AN OPTION FOR A PASSIVE ITEM UPGRADE
            else if (upgradeType == 1)
            {
                PassiveUpgrade chosenPassiveUpgrade = availabePassiveUpgrades[UnityEngine.Random.Range(0, availabePassiveUpgrades.Count)];

                availabePassiveUpgrades.Remove(chosenPassiveUpgrade);

                if (chosenPassiveUpgrade != null)
                {
                    EnableUpgradeUI(upgradeOption);

                    bool newPassive = true;
                    List<Passive> passives = inventoryController.passiveSlots;

                    for (int i = 0; i < passives.Count; i++)
                    {
                        if (passives[i] != null && passives[i].stats == chosenPassiveUpgrade.passiveStats)
                        {
                            newPassive = false;

                            if (!passives[i].IsUpgradeable())
                            {
                                DisableUpgradeUI(upgradeOption);
                                break;
                            }

                            upgradeOption.upgradeButton.onClick.AddListener(() => inventoryController.LevelUpPassive(i));
                            nameDisplayText = chosenPassiveUpgrade.passiveStats.name + " Level: " + (passives[i].Level + 1).ToString();
                            descriptionDisplayText = passives[i].stats.upgrades[passives[i].Level - 1].description;
                            break;
                        }
                    }

                    if (newPassive)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => player.AddItem(chosenPassiveUpgrade.initialPassive));
                        nameDisplayText = chosenPassiveUpgrade.passiveStats.name + " (New)";
                        descriptionDisplayText = chosenPassiveUpgrade.passiveStats.Summary;

                    }

                    upgradeOption.upgradeNameDisplay.text = nameDisplayText;
                    upgradeOption.upgradeDescriptionDisplay.text = descriptionDisplayText;
                    upgradeOption.upgradeIcon.sprite = chosenPassiveUpgrade.passiveStats.icon;
                }
            }


        }
    }

    void ClearUpgradeOptions()
    {
        foreach (var upgradeOption in upgradeUIOptions)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners();
            DisableUpgradeUI(upgradeOption);
        }
    }

    public void ClearAndSetUpgradeOptions()
    {
        ClearUpgradeOptions();
        SetUpgradeOptions();
    }

    void EnableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(true); // this is atrocious
    }

    void DisableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(false);
    }
}
