using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public enum GameState { Play, Pause, GameOver };
    public GameState currentState;
    // [HideInInspector]
    public GameState previousState;
    public bool isGameOver = false;

    [Header("General UI")]
    public GameObject displayScreen;
    public TextMeshProUGUI timeDisplay;

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

        ToggleDisplay(false);
    }

    private void Update()
    {
        // Finite State Machine!
        switch (currentState)
        {
            case GameState.Play:
                CheckForPauseAndResume();
                break;
            case GameState.Pause:
                CheckForPauseAndResume();
                break;
            case GameState.GameOver:
                if (!isGameOver)
                {
                    isGameOver = true;
                    Time.timeScale = 0f;
                    Debug.Log("Game Over");
                    ToggleDisplay(true);
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
            ToggleDisplay(true);
            Debug.Log("The game has been paused.");
        }
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Pause)
        {
            ChangeState(previousState);
            Time.timeScale = 1f; // this starts the game again
            ToggleDisplay(false);
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

    public void ToggleDisplay(bool toggleOn)
    {
        if (toggleOn) // modify the options on the screen
        {
            if (isGameOver)
            {
                displayScreen.GetComponent<Image>().color = gameOverColor;
                titleDisplay.text = "Final Results";
            }
            else
            {
                displayScreen.GetComponent<Image>().color = pauseColor;
                titleDisplay.text = "Game Paused";
            }

            resumeButton.SetActive(!isGameOver); // show only in pause menu
            giveUpButton.SetActive(!isGameOver); // show only in pause menu
            mainMenuButton.SetActive(isGameOver); // show only in game over menu
        }

        displayScreen.SetActive(toggleOn);
    }

    public void GameOver()
    {
        ChangeState(GameState.GameOver);
        Debug.Log("The game is now over.");
    }

    public void AssignCharacterUI(Sprite icon, string charName)
    {
        characterImage.sprite = icon;
        characterName.text = charName;
    }

    public void AssignLevelUI(int level)
    {
        levelDisplay.text = "Level: " + level;
    }

    public void AssignTimeUI(int minutes, int seconds)
    {
        timeDisplay.text = "Run Duration: " + Mathf.RoundToInt(minutes) + ":" + Mathf.RoundToInt(seconds);
    }

    public void AssignItemsUI(List<Image> weapons, List<Image> passives)
    {
        if (weapons.Count != weaponsUI.Count || passives.Count != passivesUI.Count)
        {
            Debug.LogError("Item arrays have different length!");
            return;
        }

        // assign weapons UI
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i].sprite)
            {
                weaponsUI[i].enabled = true;
                weaponsUI[i].sprite = weapons[i].sprite;
            }
            else
            {
                weaponsUI[i].enabled = false;
            }
        }

        // assign passives UI
        for (int i = 0; i < passives.Count; i++)
        {
            if (passives[i].sprite)
            {
                passivesUI[i].enabled = true;
                passivesUI[i].sprite = passives[i].sprite;
            }
            else
            {
                passivesUI[i].enabled = false;
            }
        }
    }
}
