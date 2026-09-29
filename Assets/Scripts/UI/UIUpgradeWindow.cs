using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(VerticalLayoutGroup))]
public class UIUpgradeWindow : MonoBehaviour
{
    VerticalLayoutGroup verticalLayout;

    public RectTransform upgradeOptionTemplate;
    public TextMeshProUGUI tooltipTemplate;

    [Header("Settings")]
    public int maxOptions = 4;
    public string newText = "New!";

    public Color newTextColor = Color.yellow, levelTextColor = Color.white;

    [Header("Paths")]
    public string iconPath = "Icon/ItemIcon";
    public string namePath = "Name", descriptionPath = "Description", buttonPath = "Button", levelPath = "Level";

    // we'll track these in our functions
    RectTransform rectTransform;
    float optionHeight; // default height of the upgrade option tempalte
    int activeOptions;

    // all update buttons on the window
    List<RectTransform> upgradeOptions = new List<RectTransform>();

    Vector2 lastScreen; // track the width and height of the last frame so we know when to recalculate sizes

    public void SetUpgrades(PlayerInventoryController inventory, List<ItemStatsB> possibleUpgrades, int pick = 3, string tooltip = "")
    {
        pick = Mathf.Min(maxOptions, pick); // how many options to choose

        if (maxOptions > upgradeOptions.Count)
        {
            for (int i = upgradeOptions.Count; i < pick; i++)
            {
                GameObject go = Instantiate(upgradeOptionTemplate.gameObject, transform);
                upgradeOptions.Add((RectTransform)go.transform);
            }
        }

        tooltipTemplate.text = tooltip;
        tooltipTemplate.gameObject.SetActive(tooltip.Trim() != ""); // if the tooltip isn't empty, show it

        // activate only the buttons we need, and disable the rest
        activeOptions = 0;
        int totalPossibleUpgrades = possibleUpgrades.Count;
        foreach (RectTransform rt in upgradeOptions)
        {
            if (activeOptions < pick && activeOptions < totalPossibleUpgrades)
            {
                rt.gameObject.SetActive(true);

                ItemStatsB selectedItem = possibleUpgrades[Random.Range(0, possibleUpgrades.Count)];
                possibleUpgrades.Remove(selectedItem);
                ItemB item = inventory.Get(selectedItem);

                // insert  the name of the item
                TextMeshProUGUI name = rt.Find(namePath).GetComponent<TextMeshProUGUI>();
                if (name)
                {
                    name.text = selectedItem.name; // TODO: this grabs the file name
                }

                TextMeshProUGUI level = rt.Find(levelPath).GetComponent<TextMeshProUGUI>();
                if (level)
                {
                    if (item)
                    {
                        if (item.currentLevel >= item.maxLevel)
                        {
                            level.text = "Max!";
                        }
                        else
                        {
                            level.text = selectedItem.GetLevelData(item.currentLevel + 1).name; // if we name our levels "Level: 2" this works, otherwise we can cut name and just access current Level
                            level.color = levelTextColor;
                        }
                    }
                    else
                    {
                        level.text = newText;
                        level.color = newTextColor;
                    }
                }

                TextMeshProUGUI desc = rt.Find(descriptionPath).GetComponent<TextMeshProUGUI>();
                if (desc)
                {
                    if (item)
                    {
                        desc.text = selectedItem.GetLevelData(item.currentLevel + 1).description;
                    }
                    else
                    {
                        desc.text = selectedItem.GetLevelData(1).description;
                    }
                }

                Image icon = rt.Find(iconPath).GetComponent<Image>();
                if (icon)
                {
                    icon.sprite = selectedItem.icon;
                }

                Button b = rt.Find(buttonPath).GetComponent<Button>();
                if (b)
                {
                    b.onClick.RemoveAllListeners();
                    if (item)
                    {
                        b.onClick.AddListener(() => inventory.LevelUpItem(item));
                    }
                    else
                    {
                        b.onClick.AddListener(() => inventory.Add(selectedItem));
                    }
                }

                activeOptions++;
            }
            else // if the number of options we picked have already been filled, disable the rest
            {
                rt.gameObject.SetActive(false);
            }

            RecalculateLayout();
        }
    }

    void RecalculateLayout()
    {
        // calculates total available height for all options and divides it by number of options
        optionHeight = (rectTransform.rect.height - verticalLayout.padding.top - verticalLayout.padding.bottom - (maxOptions - 1) * verticalLayout.spacing);
        if (activeOptions == maxOptions && tooltipTemplate.gameObject.activeSelf)
        {
            optionHeight /= maxOptions + 1;
        }
        else
        {
            optionHeight /= maxOptions;
        }

        // recalculates the height of the tooltip
        if (tooltipTemplate.gameObject.activeSelf)
        {
            RectTransform tooltipRect = (RectTransform)tooltipTemplate.transform;
            tooltipTemplate.gameObject.SetActive(true);
            tooltipRect.sizeDelta = new Vector2(tooltipRect.sizeDelta.x, optionHeight);
            tooltipTemplate.transform.SetAsLastSibling();
        }

        foreach (RectTransform rt in upgradeOptions)
        {
            if (!rt.gameObject.activeSelf) continue;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, optionHeight);
        }
    }

    void Update()
    {
        if (lastScreen.x != Screen.width || lastScreen.y != Screen.height)
        {
            RecalculateLayout();
            lastScreen = new Vector2(Screen.width, Screen.height);
        }
    }

    void Awake()
    {
        verticalLayout = GetComponentInChildren<VerticalLayoutGroup>();
        if (tooltipTemplate) tooltipTemplate.gameObject.SetActive(false);
        if (upgradeOptionTemplate) upgradeOptions.Add(upgradeOptionTemplate);

        rectTransform = (RectTransform)transform;
    }

    void Reset()
    {
        upgradeOptionTemplate = (RectTransform)transform.Find("UpgradeOption");
        tooltipTemplate = transform.Find("Tooltip").GetComponentInChildren<TextMeshProUGUI>();
    }
}
