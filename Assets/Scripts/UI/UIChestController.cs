using System.Collections;
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
    public Sprite closedChestSprite;
    public Sprite openedChestSprite;
    public Image chestImage;
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
            chestImage.sprite = closedChestSprite;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void Activate(PlayerCollector collector, Chest chest)
    {
        if (!Instance) { Debug.LogWarning("There is no UIChestController _instance!"); return; }

        Instance.collector = collector;
        Instance.currentChest = chest;
        Instance.currentChestStats = chest.GetCurrentChestStats();

        // Debug.Log(Instance.currentChestStats);

        GameController.Instance.StartChestState();
        Instance.gameObject.SetActive(true);
    }

    public static void NotifyItemRecieved(Sprite icon) // change this name
    {
        if (!Instance) { Debug.LogWarning("There is no UIChestController _instance!"); return; }

        Instance.icons.Add(icon);
    }

    private IEnumerator FlashWhite(Image image, int times = 3, float flashDuration = 0.2f)
    {
        originalColor = image.color;

        for (int i = 0; i < times; i++)
        {
            image.color = Color.white;
            yield return new WaitForSecondsRealtime(flashDuration);

            image.color = originalColor;
            yield return new WaitForSecondsRealtime(0.2f);
        }
    }

    private IEnumerator ActivateCurvedBeams(float spawnTime)
    {
        yield return new WaitForSecondsRealtime(spawnTime);
        curvedBeams.SetActive(true);
    }

    private IEnumerator HandleCoinDisplay(float totalCoinReward)
    {
        coinsText.gameObject.SetActive(true);
        float elapsedTime = 0f;
        coins = totalCoinReward;

        while (elapsedTime < totalCoinReward)
        {
            elapsedTime += Time.unscaledDeltaTime * 40f;
            coinsText.text = string.Format("{0:F2}", elapsedTime);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(2f);
        doneButton.SetActive(true);
    }

    private void SetupBeam(int index)
    {
        items[index].weaponBeam.SetActive(true);
        items[index].beam.SetActive(true);
        items[index].spriteImage.sprite = icons[index];
        items[index].beam.GetComponent<Image>().color = currentChestStats.beamColors[index];
    }

    private IEnumerator ShowDelayedBeams(int startIndex, int endIndex)
    {
        yield return new WaitForSecondsRealtime(currentChestStats.beamDelayTime);

        for (int i = startIndex; i < endIndex; i++)
        {
            SetupBeam(i);
        }
    }

    public void DisplayBeam(float numberOfBeams)
    {
        int delayedStartIndex = Mathf.Max(0, (int)numberOfBeams - currentChestStats.amountOfDelayedBeams);

        for (int i = 0; i < delayedStartIndex; i++)
        {
            SetupBeam(i);
        }

        if (currentChestStats.amountOfDelayedBeams > 0)
        {
            StartCoroutine(ShowDelayedBeams(delayedStartIndex, (int)numberOfBeams));
        }

        StartCoroutine(DisplayItems(numberOfBeams));
    }

    private IEnumerator DisplayItems(float numberOfBeams)
    {
        yield return new WaitForSecondsRealtime(currentChestStats.animationDuration);

        if (numberOfBeams == 5)
        {
            // show first item
            items[0].weaponBeam.SetActive(false);
            items[0].sprite.SetActive(true);
            yield return new WaitForSecondsRealtime(0.3f);

            // show second and third items
            for (int i = 1; i <= 2; i++)
            {
                items[i].weaponBeam.SetActive(false);
                items[i].sprite.SetActive(true);
            }
            yield return new WaitForSecondsRealtime(0.3f);

            // show fourh and fifth items
            for (int i = 3; i <= 4; i++)
            {
                items[i].weaponBeam.SetActive(false);
                items[i].sprite.SetActive(true);
            }
            yield return new WaitForSecondsRealtime(0.3f);
        }
        else
        {
            for (int i = 0; i < numberOfBeams; i++)
            {
                items[i].weaponBeam.SetActive(false);
                items[i].sprite.SetActive(true);
                yield return new WaitForSecondsRealtime(0.3f);
            }
        }
    }

    public IEnumerator Open()
    {
        if (currentChestStats.hasFireworks)
        {
            isAnimating = false; // ensure this can't be skipped?
            StartCoroutine(FlashWhite(chestPanel, 5));
            fireworks.SetActive(true);
            yield return new WaitForSecondsRealtime(currentChestStats.fireworksDelay);
        }

        isAnimating = true; // now allow for skipping
        chestImage.sprite = openedChestSprite;

        if (currentChestStats.hasCurvedBeams)
        {
            StartCoroutine(ActivateCurvedBeams(currentChestStats.curveBeamsSpawnTime));
        }

        StartCoroutine(HandleCoinDisplay(Random.Range(currentChestStats.minCoins, currentChestStats.maxCoins)));

        DisplayBeam(currentChestStats.numberOfItems);
        openingVFX.SetActive(true);
        beamVFX.SetActive(true);

        yield return new WaitForSecondsRealtime(currentChestStats.animationDuration);
        openingVFX.SetActive(false);
    }

    public void Begin()
    {
        chestImage.sprite = closedChestSprite; // replace this with an animation or IEnumerator
        openChestButton.SetActive(false);

        chestSequenceCoroutine = StartCoroutine(Open());
        audioSource.clip = currentChestStats.openingSound;
        audioSource.Play();
    }

    private void SkipToRewards()
    {
        if (chestSequenceCoroutine != null)
        {
            StopCoroutine(chestSequenceCoroutine);
        }

        StopAllCoroutines();

        for (int i = 0; i < icons.Count; i++)
        {
            SetupBeam(i);
            items[i].weaponBeam.SetActive(false);
            items[i].sprite.SetActive(true);
        }

        coinsText.gameObject.SetActive(true);
        coinsText.text = coins.ToString("F2");
        chestImage.sprite = openedChestSprite;
        doneButton.SetActive(true);
        openingVFX.SetActive(false);
        isAnimating = false;
        chestPanel.color = originalColor;

        if (audioSource != null && currentChestStats.openingSound != null)
        {
            audioSource.clip = currentChestStats.openingSound;

            float skipToTime = Mathf.Max(0, audioSource.clip.length - (audioSource.clip.length - 0.5f));
            audioSource.time = skipToTime;
            audioSource.Play();
        }
    }

    private void Update()
    {
        if (isAnimating && Input.GetButtonDown("Cancel"))
        {
            SkipToRewards();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            TryPressButton(openChestButton);
            TryPressButton(doneButton);
        }
    }

    private void TryPressButton(GameObject buttonObj)
    {
        if (buttonObj.activeInHierarchy)
        {
            Button btn = buttonObj.GetComponent<Button>();
            if (btn != null && btn.interactable)
            {
                btn.onClick.Invoke();
            }
        }
    }

    public void CloseUI()
    {
        collector.AddCoins(coins);

        chestImage.sprite = closedChestSprite;
        openChestButton.SetActive(true);
        beamVFX.SetActive(false);
        coinsText.gameObject.SetActive(false);
        doneButton.SetActive(false);
        fireworks.SetActive(false);
        curvedBeams.SetActive(false);

        gameObject.SetActive(false);
        ResetDisplay();

        if (pickupSound)
        {
            audioSource.clip = pickupSound;
            audioSource.time = 0f;
            audioSource.Play();
        }

        isAnimating = false;

        GameController.Instance.EndChestState();
        currentChest.RefreshItemDisplays();
    }

    private void ResetDisplay()
    {
        foreach (var item in items)
        {
            item.beam.SetActive(false);
            item.sprite.SetActive(false);
            item.spriteImage.sprite = null;
        }

        currentChestStats = null;
        icons.Clear();
    }
}
