using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutGroup))]
public class UIInventoryIconDisplay : MonoBehaviour
{
    public GameObject slotTemplate;
    public uint maxSlots = 6;
    public bool showLevels = true;
    public PlayerInventoryController inventory;

    public GameObject[] slots;

    [Header("Paths")]
    public string iconPath = "Icon";
    public string levelTextPath = "Level";
    [HideInInspector] public string targetedItemList;

    void Reset()
    {
        slotTemplate = transform.GetChild(0).gameObject;
        inventory = FindAnyObjectByType<PlayerInventoryController>();
    }

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (!inventory) Debug.LogWarning("No inventory attached to this UI icon display");

        Type t = typeof(PlayerInventoryController);
        FieldInfo field = t.GetField(targetedItemList, BindingFlags.Public | BindingFlags.Instance);

        if (field == null)
        {
            Debug.LogWarning("The specified list is not found in the inventory.");
            return;
        }

        List<PlayerInventoryController.Slot> items = (List<PlayerInventoryController.Slot>)field.GetValue(inventory);

        for (int i = 0; i < items.Count; i++)
        {
            if (i >= slots.Length) // if we have mismatching items.Count != slots.Length values (we should be updating this at run-time I think)
            {
                Debug.LogWarning($"You have {items.Count} slots, but only {slots.Length} slots in the UI.");
                break;
            }

            ItemB item = items[i].item;

            Transform iconObj = slots[i].transform.Find(iconPath);
            if (iconObj)
            {
                Image icon = iconObj.GetComponent<Image>(); // might need to be get comp in child

                if (!item)
                {
                    icon.color = new Color(1, 1, 1, 0); // opacity to 0
                    iconObj.parent.GetComponent<Image>().color = new Color(1, 1, 1, 0); // set background opacity to 0 too
                }
                else
                {
                    icon.color = new Color(1, 1, 1, 1); // opacity to 1
                    iconObj.parent.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    if (icon) icon.sprite = item.statsData.icon; // why can we access an icon's color but not it's sprite if it's null?
                }
            }

            Transform levelObj = slots[i].transform.Find(levelTextPath);
            if (levelObj)
            {
                TextMeshProUGUI levelTxt = levelObj.GetComponent<TextMeshProUGUI>();
                if (levelTxt)
                {
                    if (!item || !showLevels) levelTxt.text = "";
                    else levelTxt.text = item.currentLevel.ToString();
                }
            }
        }
    }
}
