using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public enum GameState { Play, LevelUp, Pause, GameOver };
    public GameState currentState;
    // [HideInInspector]
    public GameState previousState;
    public bool isGameOver = false;
    public bool choosingUpgrades = false;
    public GameObject playerInventoryController;

    [Header("General UI")]
    public GameObject displayScreen;
    public GameObject levelUpScreen;

    [Header("Health Bar UI")]
    public Image healthBar;

    [Header("Experience Bar UI")]
    public Image experienceBarHolder;
    public Image experienceBar;
    public TextMeshProUGUI inGameLevelDisplay;

    [Header("Run Timer UI")]
    public TextMeshProUGUI timeDisplay;
    public float timeLimit;
    float timer;
    public TextMeshProUGUI inGameTimeDisplay;

    [Header("Rotating UI")]
    public Color gameOverColor;
    public Color pauseColor;
    public TextMeshProUGUI titleDisplay;
    public GameObject giveUpButton;
    public GameObject resumeButton;
    public GameObject mainMenuButton;

    [Header("Stats UI")]
    public TextMeshProUGUI levelDisplay;
    public TextMeshProUGUI maxHealthDisplay;
    public TextMeshProUGUI curHealthDisplay;
    public TextMeshProUGUI recoveryDisplay;
    public TextMeshProUGUI armorDisplay;
    public TextMeshProUGUI speedDisplay;
    public TextMeshProUGUI projSpeedDisplay;
    public TextMeshProUGUI mightDisplay;
    public TextMeshProUGUI areaDisplay;
    public TextMeshProUGUI magnetDisplay;
    public TextMeshProUGUI growthDisplay;
    public TextMeshProUGUI luckDisplay;

    [Header("Character UI")]
    public TextMeshProUGUI characterName;
    public Image characterImage;
    public List<Image> weaponsUI = new List<Image>(6);
    public List<Image> passivesUI = new List<Image>(6);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        ToggleDisplays();
    }

    private void Update()
    {
        // Finite State Machine!
        switch (currentState)
        {
            case GameState.Play:
                CheckForPauseAndResume();
                UpdateTimer();
                break;
            case GameState.Pause:
                CheckForPauseAndResume();
                break;
            case GameState.LevelUp:
                if (!choosingUpgrades)
                {
                    choosingUpgrades = true;
                    Time.timeScale = 0f;
                    ToggleDisplays();
                    Debug.Log("Level Up Screen Entered");
                }
                break;
            case GameState.GameOver:
                if (!isGameOver)
                {
                    isGameOver = true;
                    Time.timeScale = 0f;
                    Debug.Log("Game Over");
                    ToggleDisplays();
                }
                break;
            default:
                Debug.Log($"No case for the given Game State {currentState}");
                // when to log a warning vs. an error?
                break;
        }
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
    }

    public void PauseGame()
    {
        if (currentState != GameState.Pause)
        {
            previousState = currentState;
            ChangeState(GameState.Pause);
            Time.timeScale = 0f; // this stops the game
            ToggleDisplays();
            Debug.Log("The game has been paused.");
        }
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Pause)
        {
            ChangeState(previousState);
            Time.timeScale = 1f; // this starts the game again
            ToggleDisplays();
            Debug.Log("The game has been resumed.");
        }
    }

    void CheckForPauseAndResume()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Pause)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void GameOver()
    {
        ChangeState(GameState.GameOver);
        Debug.Log("The game is now over.");
    }

    void UpdateTimer()
    {
        timer += Time.deltaTime;
        AssignTimerUI();

        if (timer >= timeLimit)
        {
            GameOver();
        }
    }

    public void StartPlayerLevelUp()
    {
        ChangeState(GameState.LevelUp);
        playerInventoryController.SendMessage("ClearAndSetUpgradeOptions");
    }

    public void EndPlayerLevelUp()
    {
        choosingUpgrades = false;
        Time.timeScale = 1f;
        ChangeState(GameState.Play);
        ToggleDisplays();
    }

    #region Assign UI Elements
    public void ToggleDisplays()
    {
        bool toggleOn = false;
        bool isPlay = false;

        switch (currentState)
        {
            case GameState.Play:
                isPlay = true;
                break;
            case GameState.Pause:
                toggleOn = true;
                displayScreen.GetComponent<Image>().color = pauseColor;
                titleDisplay.text = "Game Paused";
                break;
            case GameState.LevelUp:
                break;
            case GameState.GameOver:
                toggleOn = true;
                displayScreen.GetComponent<Image>().color = gameOverColor;
                titleDisplay.text = "Final Results";
                break;
            default:
                Debug.Log($"No case for the given Game State {currentState}");
                break;
        }

        resumeButton.SetActive(!isGameOver); // show only in pause menu
        giveUpButton.SetActive(!isGameOver); // show only in pause menu
        mainMenuButton.SetActive(isGameOver); // show only in game over menu

        inGameTimeDisplay.gameObject.SetActive(isPlay);
        experienceBarHolder.gameObject.SetActive(isPlay);
        displayScreen.SetActive(toggleOn);
        levelUpScreen.SetActive(choosingUpgrades); // this is only ever true when Level Up is the Game State
    }

    void AssignTimerUI()
    {
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.RoundToInt(timer % 60);

        timeDisplay.text = string.Format("Run Duration {0:00}:{1:00}", minutes, seconds);
        inGameTimeDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void AssignCharacterUI(Sprite icon, string charName)
    {
        characterImage.sprite = icon;
        characterName.text = charName;
    }

    public void AssignHealthBarUI(float barFillAmount)
    {
        healthBar.fillAmount = barFillAmount;
    }

    public void AssignLevelUI(int level)
    {
        levelDisplay.text = "Level: " + level;
        inGameLevelDisplay.text = "Level: " + level;
    }

    public void AssignExperienceBarUI(float barFillAmount)
    {
        experienceBar.fillAmount = barFillAmount;
    }

    public void AssignTimeUI(int minutes, int seconds)
    {
        timeDisplay.text = "Run Duration: " + Mathf.RoundToInt(minutes) + ":" + Mathf.RoundToInt(seconds);
    }

    //public void AssignItemsUI(List<Image> weapons, List<Image> passives)
    //{
    //    if (weapons.Count != weaponsUI.Count || passives.Count != passivesUI.Count)
    //    {
    //        Debug.LogError("Item arrays have different length!");
    //        return;
    //    }

    //    // assign weapons UI
    //    for (int i = 0; i < weapons.Count; i++)
    //    {
    //        if (weapons[i].sprite)
    //        {
    //            weaponsUI[i].enabled = true;
    //            weaponsUI[i].sprite = weapons[i].sprite;
    //        }
    //        else
    //        {
    //            weaponsUI[i].enabled = false;
    //        }
    //    }

    //    // assign passives UI
    //    for (int i = 0; i < passives.Count; i++)
    //    {
    //        if (passives[i].sprite)
    //        {
    //            passivesUI[i].enabled = true;
    //            passivesUI[i].sprite = passives[i].sprite;
    //        }
    //        else
    //        {
    //            passivesUI[i].enabled = false;
    //        }
    //    }
    //}
    #endregion
}
