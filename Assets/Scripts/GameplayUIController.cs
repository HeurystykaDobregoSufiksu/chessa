using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles UI controls during gameplay
/// Provides functionality to return to main menu and display game mode info
/// </summary>
public class GameplayUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private TMP_Text gameModeText;

    [Header("Pause Menu Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button returnToMenuButton;

    private bool isPaused = false;
    private gameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<gameManager>();

        // Set up button listeners
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);

        if (pauseButton != null)
            pauseButton.onClick.AddListener(OnPauseButtonClicked);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(OnResumeButtonClicked);

        if (returnToMenuButton != null)
            returnToMenuButton.onClick.AddListener(OnReturnToMenuButtonClicked);

        // Hide pause menu initially
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        // Display current game mode
        UpdateGameModeDisplay();
    }

    private void Update()
    {
        // Allow ESC key to toggle pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                OnResumeButtonClicked();
            else
                OnPauseButtonClicked();
        }
    }

    private void UpdateGameModeDisplay()
    {
        if (gameModeText == null || gameManager == null) return;

        string modeText = gameManager.currentGameMode switch
        {
            GameMode.PuzzlePractice => "Puzzle Practice",
            GameMode.Woodpecker => "Woodpecker Method",
            _ => "Unknown Mode"
        };

        gameModeText.text = $"Mode: {modeText}";
    }

    private void OnMainMenuButtonClicked()
    {
        ReturnToMainMenu();
    }

    private void OnPauseButtonClicked()
    {
        isPaused = true;
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        Time.timeScale = 0f; // Pause the game
    }

    private void OnResumeButtonClicked()
    {
        isPaused = false;
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        Time.timeScale = 1f; // Resume the game
    }

    private void OnReturnToMenuButtonClicked()
    {
        Time.timeScale = 1f; // Ensure time scale is reset
        ReturnToMainMenu();
    }

    private void ReturnToMainMenu()
    {
        // Save any necessary data before returning to menu
        if (gameManager != null && gameManager.currentGameMode == GameMode.Woodpecker)
        {
            WoodpeckerMode woodpeckerMode = FindObjectOfType<WoodpeckerMode>();
            if (woodpeckerMode != null)
            {
                woodpeckerMode.SaveSession();
                Debug.Log("Woodpecker session saved before returning to menu");
            }
        }

        // Load the menu scene
        MenuSceneController.Instance.LoadMenuScene();
    }
}
