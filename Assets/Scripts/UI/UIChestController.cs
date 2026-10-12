using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIChestController : MonoBehaviour
{
    public static UIChestController Instance;
    PlayerCollector collector;
    Chest currentChest;
    ChestStats currentChestStats;

    [Header("Visual Elements")]
    public GameObject openingVFX;
    public GameObject beamVFX;
    public GameObject fireworks;
    public GameObject curvedBeams;
    public List<ItemDisplay> items;
    Color originalColor = new Color32(0x42, 0x41, 0x87, 255);

    [System.Serializable]
    public struct ItemDisplay
    {
        public GameObject beam;
        public Image spriteImage;
        public GameObject sprite;
        public GameObject weaponBeam;
    }

    [Header("UI Elements")]
    public GameObject openChestButton;
    public GameObject doneButton;
    public Image chestPanel;
    public TextMeshProUGUI coinsText;
    private float coins;

    private List<Sprite> icons = new List<Sprite>();
    private bool isAnimating = false;
    private Coroutine chestSequenceCoroutine;

    [Header("Audio Elements")]
    private AudioSource audioSource;
    public AudioClip pickupSound;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
