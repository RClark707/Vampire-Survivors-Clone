using System.Collections.Generic;
using Terresquall;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ToggleGroup))]
public class UIPowerupSelector : MonoBehaviour
{
    public PowerupStats defaultPowerup;
    public static PowerupStats selectedPowerup;

    [Header("Template")]
    public Toggle toggleTemplate;
    public string powerupNamePath = "PowerupName";
    public string powerupIconPath = "PowerupIcon";

    [Tooltip("Name of the tickbox parent container, which holds all the tickboxes.")]
    public string tickboxContainerPath = "Tickboxes";
    [Tooltip("Name of a single tick, not the tickbox")]
    public string tickPath = "Tick";

    Dictionary<PowerupStats, List<Image>> cachedTicks = new Dictionary<PowerupStats, List<Image>>();
    Dictionary<PowerupStats, Toggle> cachedToggleButtons = new Dictionary<PowerupStats, Toggle>();

    private int purchasedLevel = 0;
    private int previewLevel = 1;

    [Header("Description Box")]
    public TextMeshProUGUI powerupName;
    public TextMeshProUGUI powerupDescription;
    public Image powerupIcon;
    public TextMeshProUGUI powerupPrice;
    public Button purchaseButton;

    void Start()
    {
        if (SaveController.Instance != null)
        {
            SaveController.Instance.OnPowerupsChanged += SyncTicksForAll;
            SaveController.Instance.OnPowerupsChanged += RefreshUIAfterLoad;
        }
        else
        {
            Debug.LogWarning("Your Save Controller is unavailable!");
        }

        SyncTicksForAll();

        if (defaultPowerup)
        {
            Select(defaultPowerup);
        }
    }

    void OnDestroy()
    {
        if (SaveController.Instance != null)
        {
            SaveController.Instance.OnPowerupsChanged -= SyncTicksForAll;
            SaveController.Instance.OnPowerupsChanged -= RefreshUIAfterLoad;
        }
    }

    void RefreshUIAfterLoad()
    {
        if (selectedPowerup != null)
        {
            purchasedLevel = SaveController.Instance.GetLevel(selectedPowerup);
            previewLevel = Mathf.Clamp(purchasedLevel + 1, 1, selectedPowerup.maxLevel);
            RefreshUI();
        }
    }

    Toggle FindToggle(PowerupStats powerup)
    {
        if (cachedToggleButtons.ContainsKey(powerup))
        {
            return cachedToggleButtons[powerup];
        }

        // otherwise

        foreach (Transform child in toggleTemplate.transform.parent)
        {
            if (child.name == powerup.name)
            {
                if (child.TryGetComponent(out Toggle t))
                {
                    cachedToggleButtons[powerup] = t;
                    return t;
                }

                return null;
            }
        }

        Debug.LogWarning($"Could not find the toggle button for the powerup: {powerup.name}");
        return null;
    }

    List<Image> FindTicks(PowerupStats powerup)
    {
        if (cachedTicks.ContainsKey(powerup))
        {
            return cachedTicks[powerup];
        }

        // otherwise

        List<Image> ticks = new List<Image>();

        Toggle toggleButton = FindToggle(powerup);
        if (toggleButton == null) return ticks;

        Transform tickBoxesContainer = toggleButton.transform.Find(tickboxContainerPath);
        foreach (Transform tickbox in tickBoxesContainer)
        {
            foreach (Transform child in tickbox)
            {
                if (child.name.ToLower().Contains(tickPath.ToLower()))
                {
                    Image img = child.GetComponent<Image>();
                    if (img != null)
                    {
                        ticks.Add(img);
                    }
                }
            }
        }

        cachedTicks[powerup] = ticks;

        // Debug.Log($"Found {ticks.Count} ticks for {powerup.name}.");
        return ticks;
    }

    public void SyncTicksForAll()
    {
        if (SaveController.Instance == null)
        {
            Debug.LogWarning("SaveController is unavailable to save powerups.");
            return;
        }

#if UNITY_EDITOR
        PowerupStats[] allPowerups = GetAllPowerups();
        foreach (PowerupStats powerup in allPowerups)
        {
            SyncTicks(powerup);
        }
#else
if (selected != null) 
{
SyncTicksForPowerUp(selectedPowerup);
}
#endif
    }

    void SyncTicks(PowerupStats powerup)
    {
        if (powerup == null) return;

        int currentLevel = SaveController.Instance.GetLevel(powerup);
        List<Image> ticks = FindTicks(powerup);

        for (int i = 0; i < ticks.Count; i++)
        {
            if (ticks[i] != null)
            {
                ticks[i].gameObject.SetActive(i < currentLevel); // 0 < 1 is false, so no ticks means its level 1. 1 < 1 is also false.
            }
        }
    }

    public void Select(PowerupStats powerup)
    {
        if (powerup == null) return;

        if (SaveController.Instance == null)
        {
            Debug.LogWarning("Your Save Controller is unavailable!");
            return;
        }

        selectedPowerup = powerup;

        purchasedLevel = SaveController.Instance.GetLevel(powerup);
        previewLevel = Mathf.Clamp(purchasedLevel + 1, 1, selectedPowerup.maxLevel);

        RefreshUI();
    }

    void RefreshUI()
    {
        if (selectedPowerup == null) return;

        Item.LevelData levelData;

        if (purchasedLevel >= selectedPowerup.maxLevel)
        {
            levelData = selectedPowerup.GetLevelData(selectedPowerup.maxLevel);
            powerupPrice.text = "MAX";

            if (purchaseButton != null)
            {
                purchaseButton.gameObject.SetActive(false);
            }
        }
        else
        {
            levelData = selectedPowerup.GetLevelData(previewLevel);

            float cost = selectedPowerup.GetCost(previewLevel);
            powerupPrice.text = cost.ToString();

            if (purchaseButton != null)
            {
                purchaseButton.gameObject.SetActive(true);
            }
        }

        powerupName.text = selectedPowerup.name;
        powerupDescription.text = levelData.description;
        powerupIcon.sprite = selectedPowerup.icon;

        UpdateLevelTicks(); // why call this instead of
                            // SyncTicks(selectedPowerup);
                            // Because the previous function compares to current level, not purchased level (and uses SaveController)
                            // We could just add those as parameters?
    }

    void UpdateLevelTicks()
    {
        if (selectedPowerup == null) return;

        List<Image> ticks = FindTicks(selectedPowerup);

        for (int i = 0; i < ticks.Count; i++)
        {
            if (ticks[i] != null)
            {
                ticks[i].gameObject.SetActive(i < purchasedLevel);
            }
        }
    }

    public void Buy()
    {
        if (selectedPowerup == null) return;

        if (purchasedLevel >= selectedPowerup.maxLevel) // why run this instead of RefreshUI????
        {
            powerupPrice.text = "MAX";
            if (purchaseButton != null) purchaseButton.gameObject.SetActive(false);
            return;
        }

        float cost = selectedPowerup.GetCost(previewLevel);
        if (!SaveController.Instance.TrySpendCoins(cost))
        {
            Debug.LogWarning("Not enough coins to buy this power up!");
            return;
        }

        SaveController.Instance.LevelUp(selectedPowerup);

        purchasedLevel = SaveController.Instance.GetLevel(selectedPowerup);
        previewLevel = Mathf.Clamp(purchasedLevel + 1, 1, selectedPowerup.maxLevel);

        RefreshUI();

        Bench.SaveGame(Bench.currentSlot, true);
    }

    public static PowerupStats GetSelected()
    {
        return selectedPowerup;
    }

#if UNITY_EDITOR
    public static PowerupStats[] GetAllPowerups()
    {
        List<PowerupStats> powerups = new List<PowerupStats>();
        string[] guids = AssetDatabase.FindAssets("t:PowerupStats");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            PowerupStats powerup = AssetDatabase.LoadAssetAtPath<PowerupStats>(path);
            if (powerup != null)
            {
                powerups.Add(powerup);
            }
        }

        return powerups.ToArray();
    }
#endif
}
