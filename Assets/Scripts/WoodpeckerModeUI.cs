using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ChessGame;
using ChessGame.Models;

/// <summary>
/// UI controller for starting and configuring Woodpecker mode
/// </summary>
public class WoodpeckerModeUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject modeSelectionPanel;
    [SerializeField] private GameObject woodpeckerSetupPanel;
    [SerializeField] private Button startWoodpeckerButton;
    [SerializeField] private Button resumeWoodpeckerButton;
    [SerializeField] private Button backButton;

    [Header("Settings Controls")]
    [SerializeField] private TMP_InputField puzzleCountInput;
    [SerializeField] private TMP_InputField cyclesInput;
    [SerializeField] private Slider minRatingSlider;
    [SerializeField] private Slider maxRatingSlider;
    [SerializeField] private TMP_Text minRatingText;
    [SerializeField] private TMP_Text maxRatingText;

    [Header("References")]
    [SerializeField] private gameManager gameManagerRef;

    private WoodpeckerMode woodpeckerMode;

    private void Start()
    {
        woodpeckerMode = FindObjectOfType<WoodpeckerMode>();
        if (woodpeckerMode == null)
        {
            GameObject wpGO = new GameObject("WoodpeckerMode");
            woodpeckerMode = wpGO.AddComponent<WoodpeckerMode>();
        }

        if (gameManagerRef == null)
        {
            gameManagerRef = FindObjectOfType<gameManager>();
        }

        // Setup button listeners
        if (startWoodpeckerButton != null)
            startWoodpeckerButton.onClick.AddListener(OnStartWoodpecker);

        if (resumeWoodpeckerButton != null)
            resumeWoodpeckerButton.onClick.AddListener(OnResumeWoodpecker);

        if (backButton != null)
            backButton.onClick.AddListener(OnBack);

        // Setup slider listeners
        if (minRatingSlider != null)
        {
            minRatingSlider.onValueChanged.AddListener(OnMinRatingChanged);
            OnMinRatingChanged(minRatingSlider.value);
        }

        if (maxRatingSlider != null)
        {
            maxRatingSlider.onValueChanged.AddListener(OnMaxRatingChanged);
            OnMaxRatingChanged(maxRatingSlider.value);
        }

        // Set default values
        if (puzzleCountInput != null)
            puzzleCountInput.text = "100";

        if (cyclesInput != null)
            cyclesInput.text = "5";

        // Check if there's an existing session
        UpdateResumeButton();
    }

    private void UpdateResumeButton()
    {
        if (resumeWoodpeckerButton != null && woodpeckerMode != null)
        {
            var session = woodpeckerMode.LoadSession();
            bool hasActiveSession = session != null && !session.IsCompleted;
            resumeWoodpeckerButton.gameObject.SetActive(hasActiveSession);
        }
    }

    public void ShowModeSelection()
    {
        if (modeSelectionPanel != null)
            modeSelectionPanel.SetActive(true);

        if (woodpeckerSetupPanel != null)
            woodpeckerSetupPanel.SetActive(false);
    }

    public void ShowWoodpeckerSetup()
    {
        if (modeSelectionPanel != null)
            modeSelectionPanel.SetActive(false);

        if (woodpeckerSetupPanel != null)
            woodpeckerSetupPanel.SetActive(true);

        UpdateResumeButton();
    }

    private void OnStartWoodpecker()
    {
        WoodpeckerSettings settings = new WoodpeckerSettings();

        // Parse settings from UI
        if (puzzleCountInput != null && int.TryParse(puzzleCountInput.text, out int puzzleCount))
        {
            settings.PuzzleCount = Mathf.Clamp(puzzleCount, 10, 500);
        }

        if (cyclesInput != null && int.TryParse(cyclesInput.text, out int cycles))
        {
            settings.TotalCycles = Mathf.Clamp(cycles, 1, 10);
        }

        if (minRatingSlider != null)
        {
            settings.MinRating = Mathf.RoundToInt(minRatingSlider.value);
        }

        if (maxRatingSlider != null)
        {
            settings.MaxRating = Mathf.RoundToInt(maxRatingSlider.value);
        }

        Debug.Log($"Starting Woodpecker mode: {settings.PuzzleCount} puzzles, {settings.TotalCycles} cycles, Rating {settings.MinRating}-{settings.MaxRating}");

        // Start the woodpecker mode
        if (gameManagerRef != null)
        {
            gameManagerRef.StartWoodpeckerMode(settings);
        }

        // Hide setup panel
        if (woodpeckerSetupPanel != null)
            woodpeckerSetupPanel.SetActive(false);
    }

    private void OnResumeWoodpecker()
    {
        var session = woodpeckerMode.LoadSession();
        if (session != null && !session.IsCompleted)
        {
            Debug.Log("Resuming Woodpecker session");

            if (gameManagerRef != null)
            {
                gameManagerRef.SetGameMode(GameMode.Woodpecker);
                woodpeckerMode.ResumeSession(session);
            }

            // Hide setup panel
            if (woodpeckerSetupPanel != null)
                woodpeckerSetupPanel.SetActive(false);
        }
    }

    private void OnBack()
    {
        ShowModeSelection();
    }

    private void OnMinRatingChanged(float value)
    {
        if (minRatingText != null)
        {
            minRatingText.text = Mathf.RoundToInt(value).ToString();
        }

        // Ensure max is always greater than min
        if (maxRatingSlider != null && value > maxRatingSlider.value)
        {
            maxRatingSlider.value = value;
        }
    }

    private void OnMaxRatingChanged(float value)
    {
        if (maxRatingText != null)
        {
            maxRatingText.text = Mathf.RoundToInt(value).ToString();
        }

        // Ensure min is always less than max
        if (minRatingSlider != null && value < minRatingSlider.value)
        {
            minRatingSlider.value = value;
        }
    }

    public void OnPuzzlePracticeModeSelected()
    {
        if (gameManagerRef != null)
        {
            gameManagerRef.SetGameMode(GameMode.PuzzlePractice);
        }

        if (modeSelectionPanel != null)
            modeSelectionPanel.SetActive(false);
    }
}
