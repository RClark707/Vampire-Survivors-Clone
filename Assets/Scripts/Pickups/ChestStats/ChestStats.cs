using UnityEngine;

[CreateAssetMenu(fileName = "New Chest Stats", menuName = "Stats/Chest Stats")]
public class ChestStats : ScriptableObject
{
    // these stats are used to develop a profile for rendering chest animations and determining awards

    [Header("General Settings")]
    public string profileName = "Basic Chest Profile";
    [Range(0f, 1f)] public float luckScaling = 1f;
    [Range(0f, 100f)] public float baseDropChance;
    public float animationDuration;

    [Header("Fireworks")]
    public bool hasFireworks = false;
    [Range(0f, 100f)] public float fireworksDelay = 1f;

    [Header("Item Display Settings")]
    public int numberOfItems = 1;
    [Tooltip("You need at least as many beam colors as number of items to be rewarded.")]
    public Color[] beamColors = new Color[] { new Color(0, 0, 1, 0.6f) };

    [Range(0f, 100f)] public float itemDelay = 0f;
    public int delayedBeams = 0;

    public bool hasCurvedBeams;
    public float curveBeamsSpawnTime;

    [Header("Coin Settings")]
    public float minCoins = 100f;
    public float maxCoins = 500f;

    [Header("Sound Effects")]
    public AudioClip openingSound;

}
