using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using ChessGame;

/// <summary>
/// Main menu manager that handles the menu UI and navigation
/// Provides a modern, centralized menu system for game mode selection
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameModeSelectionPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("Main Menu Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button exitButton;

    [Header("Game Mode Cards")]
    [SerializeField] private GameModeCard puzzlePracticeCard;
    [SerializeField] private GameModeCard woodpeckerCard;
    [SerializeField] private Button backToMainMenuButton;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Animation Settings")]
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField] private float panelTransitionSpeed = 0.3f;

    private GameObject currentPanel;

    private void Awake()
    {
        // Initialize canvas group if not assigned
        if (canvasGroup == null)
        {
            canvasGroup = GetComponentInChildren<CanvasGroup>();
        }
    }

    private void Start()
    {
        // Set up button listeners
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonClicked);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);

        if (creditsButton != null)
            creditsButton.onClick.AddListener(OnCreditsButtonClicked);

        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitButtonClicked);

        if (backToMainMenuButton != null)
            backToMainMenuButton.onClick.AddListener(OnBackToMainMenu);

        // Initialize game mode cards
        if (puzzlePracticeCard != null)
        {
            puzzlePracticeCard.Initialize(
                "Puzzle Practice",
                "Solve random chess puzzles at your own pace. Improve your tactical vision and pattern recognition.",
                GameMode.PuzzlePractice
            );
            puzzlePracticeCard.OnCardSelected += OnGameModeSelected;
        }

        if (woodpeckerCard != null)
        {
            woodpeckerCard.Initialize(
                "Woodpecker Method",
                "Train pattern recognition by solving the same puzzle set multiple times across different cycles.",
                GameMode.Woodpecker
            );
            woodpeckerCard.OnCardSelected += OnGameModeSelected;
        }

        // Show main menu panel initially
        ShowMainMenu();

        // Fade in
        StartCoroutine(FadeIn());
    }

    private void OnDestroy()
    {
        // Clean up listeners
        if (puzzlePracticeCard != null)
            puzzlePracticeCard.OnCardSelected -= OnGameModeSelected;

        if (woodpeckerCard != null)
            woodpeckerCard.OnCardSelected -= OnGameModeSelected;
    }

    private void OnPlayButtonClicked()
    {
        ShowPanel(gameModeSelectionPanel);
    }

    private void OnSettingsButtonClicked()
    {
        ShowPanel(settingsPanel);
    }

    private void OnCreditsButtonClicked()
    {
        ShowPanel(creditsPanel);
    }

    private void OnExitButtonClicked()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void OnBackToMainMenu()
    {
        ShowMainMenu();
    }

    private void OnGameModeSelected(GameMode mode)
    {
        StartCoroutine(LoadGameModeScene(mode));
    }

    private void ShowMainMenu()
    {
        ShowPanel(mainMenuPanel);
    }

    private void ShowPanel(GameObject panelToShow)
    {
        if (panelToShow == null) return;

        // Hide all panels
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (gameModeSelectionPanel != null) gameModeSelectionPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);

        // Show the requested panel
        panelToShow.SetActive(true);
        currentPanel = panelToShow;

        // Animate panel entrance
        StartCoroutine(AnimatePanelEntrance(panelToShow));
    }

    private IEnumerator AnimatePanelEntrance(GameObject panel)
    {
        CanvasGroup panelGroup = panel.GetComponent<CanvasGroup>();
        if (panelGroup == null)
        {
            panelGroup = panel.AddComponent<CanvasGroup>();
        }

        panelGroup.alpha = 0f;
        float elapsed = 0f;

        while (elapsed < panelTransitionSpeed)
        {
            elapsed += Time.deltaTime;
            panelGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / panelTransitionSpeed);
            yield return null;
        }

        panelGroup.alpha = 1f;
    }

    private IEnumerator FadeIn()
    {
        if (canvasGroup == null) yield break;

        canvasGroup.alpha = 0f;
        float elapsed = 0f;
        float duration = 1f / fadeSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOut()
    {
        if (canvasGroup == null) yield break;

        canvasGroup.alpha = 1f;
        float elapsed = 0f;
        float duration = 1f / fadeSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }

    private IEnumerator LoadGameModeScene(GameMode mode)
    {
        // Fade out
        yield return StartCoroutine(FadeOut());

        // Load the gameplay scene
        MenuSceneController.Instance.LoadGameplayScene(mode);
    }
}
