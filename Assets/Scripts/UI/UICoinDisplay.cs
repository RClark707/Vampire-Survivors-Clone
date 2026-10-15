using TMPro;
using UnityEngine;

public class UICoinDisplay : MonoBehaviour
{
    TextMeshProUGUI displayTarget;
    public PlayerCollector collector;

    void Start()
    {
        displayTarget = GetComponentInChildren<TextMeshProUGUI>();
        UpdateDisplay();
        if (collector != null) collector.onCoinCollected += UpdateDisplay;
        if (SaveController.Instance != null) SaveController.Instance.OnCoinsChanged += UpdateDisplay;
    }

    public void UpdateDisplay()
    {
        if (collector != null)
        {
            displayTarget.text = Mathf.RoundToInt(collector.GetCoins()).ToString();
        }
        else if (SaveController.Instance != null)
        {
            float coins = SaveController.Instance.SavedData.totalCoins;
            displayTarget.text = Mathf.RoundToInt(coins).ToString();
        }
        else
        {
            Debug.LogWarning("You have neither a Save Controller nor a Player Collector to update the display!");
        }
    }
}
