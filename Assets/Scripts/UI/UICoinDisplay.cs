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
    }

    public void UpdateDisplay()
    {
        if (collector != null)
        {
            displayTarget.text = Mathf.RoundToInt(collector.GetCoins()).ToString();
        }
        else
        {
            float coins = SaveController.LastLoadedGameData.coins;
            displayTarget.text = Mathf.RoundToInt(coins).ToString();
        }
    }
}
