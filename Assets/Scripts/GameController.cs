using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public enum GameState { Play, Pause, GameOver };
    public GameState currentState;
    // [HideInInspector]
    public GameState previousState;

    [Header("UI")]
    public GameObject pauseScreen;
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

        DisableScreens();
        Time.timeScale = 1f; // this overrides if we were paused before switching scenes
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
            pauseScreen.SetActive(true);
            Debug.Log("The game has been paused.");
        }
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Pause)
        {
            ChangeState(previousState);
            Time.timeScale = 1f; // this starts the game again
            pauseScreen.SetActive(false);
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
    }

    void DisableScreens()
    {
        pauseScreen.SetActive(false);
    }
}
