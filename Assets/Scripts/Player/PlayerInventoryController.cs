using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryController : MonoBehaviour
{
    [Serializable]
    public class Slot
    {
        [HideInInspector] public ItemB item;
        public Image image;

        public void Assign(ItemB assignedItem)
        {
            item = assignedItem;
            if (item is WeaponB)
            {
                WeaponB w = item as WeaponB;
                image.enabled = true;
                image.sprite = w.statsData.icon;
            }
            else
            {
                PassiveB p = item as PassiveB;
                image.enabled = true;
                image.sprite = p.statsData.icon;
            }
            Debug.Log($"Assigned {item.name} to player's inventory.");
        }

        public void Clear()
        {
            item = null;
            image.enabled = false;
            image.sprite = null;
        }

        public bool IsEmpty()
        {
            return item == null;
        }
    }

    [Header("Inventory Slots UI")]
    public List<Slot> weaponSlots = new List<Slot>(6);
    public List<Slot> passiveSlots = new List<Slot>(6);

    [Serializable]
    public class UpgradeUI
    {
        public TMP_Text upgradeNameDisplay;
        public TMP_Text upgradeDescriptionDisplay;
        public Image upgradeIcon;
        public Button upgradeButton;
    }

    [Header("All Available Items")]
    public List<WeaponStatsB> availableWeapons = new List<WeaponStatsB>();
    public List<PassiveStatsB> availablePassives = new List<PassiveStatsB>();

    [Header("Level Up UI")]
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>();

    Player player;

    void Start()
    {
        player = GetComponent<Player>();
    }

    public bool Has(ItemStatsB type) { return Get(type); }

    public ItemB Get(ItemStatsB type) // This function sends you to a different function based on type
    {
        if (type is WeaponStatsB) return Get(type as WeaponStatsB);
        else if (type is PassiveStatsB) return Get(type as PassiveStatsB);
        return null;
    }

    public PassiveB Get(PassiveStatsB type)
    {
        foreach (Slot S in passiveSlots)
        {
            PassiveB p = S.item as PassiveB;
            if (p.statsData == type)
            {
                return p;
            }
        }
        return null;
    }

    public WeaponB Get(WeaponStatsB type)
    {
        foreach (Slot s in weaponSlots)
        {
            WeaponB w = s.item as WeaponB;
            if (w.statsData == type)
            {
                return w;
            }
        }
        return null;
    }

    public bool Remove(ItemStatsB stats, bool removeUpgradeAvailability = false)
    {
        if (stats is WeaponStatsB) return Remove(stats as WeaponStatsB, removeUpgradeAvailability);
        else if (stats is PassiveStatsB) return Remove(stats as PassiveStatsB, removeUpgradeAvailability);
        return false;
    }

    public bool Remove(WeaponStatsB stats, bool removeUpgradeAvailability = false)
    {
        if (removeUpgradeAvailability) availableWeapons.Remove(stats);

        for (int i = 0; i < weaponSlots.Count; i++)
        {
            WeaponB w = weaponSlots[i].item as WeaponB;
            if (w.statsData == stats)
            {
                weaponSlots[i].Clear();
                w.OnUnequip();
                Destroy(w.gameObject);
                return true;
            }
        }

        return false;
    }

    public bool Remove(PassiveStatsB stats, bool removeUpgradeAvailability = false)
    {
        if (removeUpgradeAvailability) availablePassives.Remove(stats);

        for (int i = 0; i < passiveSlots.Count; i++)
        {
            PassiveB p = passiveSlots[i].item as PassiveB;
            if (p.statsData == stats)
            {
                passiveSlots[i].Clear();
                p.OnUnequip();
                Destroy(p.gameObject);
                return true;
            }
        }

        return false;
    }

    public int Add(ItemStatsB stats)
    {
        if (stats is WeaponStatsB) return Add(stats as WeaponStatsB);
        else if (stats is PassiveStatsB) return Add(stats as PassiveStatsB);
        return -1;
    }

    public int Add(WeaponStatsB stats)
    {
        int slotNum = -1;

        for (int i = 0; i < weaponSlots.Capacity; i++)
        {
            if (weaponSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }

        if (slotNum < 0) return slotNum;

        Type weaponType = Type.GetType(stats.behavior);

        if (weaponType != null)
        {
            // instantiate the weapon
            GameObject go = new GameObject(stats.baseStats.name + " Controller");
            WeaponB spawnedWeapon = (WeaponB)go.AddComponent(weaponType);
            spawnedWeapon.Initialize(stats);
            spawnedWeapon.transform.SetParent(transform); // change this to whatever parent you want
            spawnedWeapon.transform.localPosition = Vector2.zero;
            spawnedWeapon.OnEquip();

            weaponSlots[slotNum].Assign(spawnedWeapon);

            if (GameController.Instance != null && GameController.Instance.choosingUpgrades)
            {
                GameController.Instance.EndPlayerLevelUp();
            }

            return slotNum;
        }
        else
        {
            Debug.LogError($"Invalid weapon type specified for {stats.name}");
        }

        return -1;
    }

    public int Add(PassiveStatsB stats)
    {
        int slotNum = -1;

        for (int i = 0; i < passiveSlots.Capacity; i++)
        {
            if (passiveSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }

        if (slotNum < 0) return slotNum;


        GameObject go = new GameObject(stats.baseStats.name + " Passive");
        PassiveB spawnedPassive = go.AddComponent<PassiveB>();
        spawnedPassive.Initialize(stats);
        spawnedPassive.transform.SetParent(transform); // change this to whatever parent you want
        spawnedPassive.transform.localPosition = Vector2.zero;
        // spawnedPassive.OnEquip(); // apparently this isn't necessary to call?

        passiveSlots[slotNum].Assign(spawnedPassive);

        if (GameController.Instance != null && GameController.Instance.choosingUpgrades)
        {
            GameController.Instance.EndPlayerLevelUp();
        }
        player.RecalculateStats(); // should we call this for weapons too?

        return slotNum;
    }

    public void LevelUpWeapon(int slotIndex, int upgradeIndex)
    {
        if (weaponSlots.Count > slotIndex)
        {
            WeaponB w = weaponSlots[slotIndex].item as WeaponB;

            if (!w.LevelUp())
            {
                Debug.LogWarning($"Failed to level up {w.name}");
                return;
            }

            if (GameController.Instance != null && GameController.Instance.choosingUpgrades)
            {
                GameController.Instance.EndPlayerLevelUp();
            }
        }
    }

    public void LevelUpPassive(int slotIndex, int upgradeIndex)
    {
        if (passiveSlots.Count > slotIndex)
        {
            PassiveB p = passiveSlots[slotIndex].item as PassiveB;

            if (!p.LevelUp())
            {
                Debug.LogWarning($"Failed to level up {p.name}");
                return;
            }

            if (GameController.Instance != null && GameController.Instance.choosingUpgrades)
            {
                GameController.Instance.EndPlayerLevelUp();
            }
            player.RecalculateStats(); // we aren't calling this for weapons?
        }
    }

    void SetUpgradeOptions()
    {
        List<WeaponStatsB> availabeWeaponUpgrades = new List<WeaponStatsB>(availableWeapons);
        List<PassiveStatsB> availabePassiveUpgrades = new List<PassiveStatsB>(availablePassives);

        foreach (UpgradeUI upgradeOption in upgradeUIOptions)
        {
            if (availabeWeaponUpgrades.Count == 0 && availabePassiveUpgrades.Count == 0)
            {
                return;
            }

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
                WeaponStatsB chosenWeaponUpgrade = availabeWeaponUpgrades[UnityEngine.Random.Range(0, availabeWeaponUpgrades.Count)]; // grab a random option from out list
                availabeWeaponUpgrades.Remove(chosenWeaponUpgrade); // we remove this before the null check because why would we want to keep a null thing?

                if (chosenWeaponUpgrade != null)
                {
                    EnableUpgradeUI(upgradeOption);

                    bool isLevelUp = false;
                    for (int i = 0; i < weaponSlots.Count; i++)
                    {
                        WeaponB w = weaponSlots[i].item as WeaponB;

                        if (w != null && w.statsData == chosenWeaponUpgrade)
                        {
                            if (w.currentLevel < chosenWeaponUpgrade.maxLevel) // are we below max level?
                            {
                                Debug.Log($"Your {w.name} is level {w.currentLevel}, the max level is {w.maxLevel}");
                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i, i));
                                WeaponB.Stats nextLevel = chosenWeaponUpgrade.GetLevelData(w.currentLevel + 1);
                                upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                                upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                                upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.icon;
                            }

                            isLevelUp = true;
                            break;
                        }
                    }
                    if (!isLevelUp)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => Add(chosenWeaponUpgrade));
                        upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.baseStats.name;
                        upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.baseStats.description;
                        upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.icon;
                    }
                }
            }
            else if (upgradeType == 1)
            {
                PassiveStatsB chosenPassiveUpgrade = availabePassiveUpgrades[UnityEngine.Random.Range(0, availabePassiveUpgrades.Count)]; // grab a random option from out list
                availabePassiveUpgrades.Remove(chosenPassiveUpgrade); // we remove this before the null check because why would we want to keep a null thing?

                if (chosenPassiveUpgrade != null)
                {
                    EnableUpgradeUI(upgradeOption);

                    bool isLevelUp = false;
                    for (int i = 0; i < passiveSlots.Count; i++)
                    {
                        PassiveB p = passiveSlots[i].item as PassiveB;

                        if (p != null && p.statsData == chosenPassiveUpgrade)
                        {
                            if (p.currentLevel < chosenPassiveUpgrade.maxLevel) // are we below max level?
                            {
                                Debug.Log($"Your {p.name} is level {p.currentLevel}, the max level is {p.maxLevel}");
                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i, i));
                                PassiveB.Modifier nextLevel = chosenPassiveUpgrade.GetLevelData(p.currentLevel + 1);
                                upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                                upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                                upgradeOption.upgradeIcon.sprite = chosenPassiveUpgrade.icon;
                            } // if we are at max level, we don't want to show anything!

                            isLevelUp = true;
                            break;
                        }
                    }
                    if (!isLevelUp)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => Add(chosenPassiveUpgrade));
                        // may need to grab the modifier here?
                        upgradeOption.upgradeNameDisplay.text = chosenPassiveUpgrade.baseStats.name;
                        upgradeOption.upgradeDescriptionDisplay.text = chosenPassiveUpgrade.baseStats.description;
                        upgradeOption.upgradeIcon.sprite = chosenPassiveUpgrade.icon;
                    }
                }
            }
        }
    }

    void ClearUpgradeOptions()
    {
        foreach (UpgradeUI upgradeOption in upgradeUIOptions)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners();
            DisableUpgradeUI(upgradeOption);
        }
    }

    public void ClearAndSetUpgrades()
    {
        ClearUpgradeOptions();
        SetUpgradeOptions();
    }

    void DisableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(false);
    }

    void EnableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(true);
    }
}
