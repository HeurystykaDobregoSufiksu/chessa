using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ChessGame;
using ChessGame.Models;

/// <summary>
/// Displays progress during an active Woodpecker session
/// </summary>
public class WoodpeckerProgressUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject progressPanel;
    [SerializeField] private TMP_Text cycleText;
    [SerializeField] private TMP_Text puzzleProgressText;
    [SerializeField] private TMP_Text accuracyText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private Slider progressBar;
    [SerializeField] private Button quitButton;

    [Header("Cycle Complete Panel")]
    [SerializeField] private GameObject cycleCompletePanel;
    [SerializeField] private TMP_Text cycleCompleteTitle;
    [SerializeField] private TMP_Text cycleStatsText;
    [SerializeField] private TMP_Text comparisonText;
    [SerializeField] private Button nextCycleButton;
    [SerializeField] private Button quitSessionButton;

    private WoodpeckerMode woodpeckerMode;
    private gameManager gameManagerRef;
    private float sessionStartTime;

    private void Start()
    {
        woodpeckerMode = FindObjectOfType<WoodpeckerMode>();
        gameManagerRef = FindObjectOfType<gameManager>();

        if (woodpeckerMode != null)
        {
            woodpeckerMode.OnPuzzleChanged += UpdateProgress;
            woodpeckerMode.OnCycleChanged += OnCycleChanged;
            woodpeckerMode.OnCycleCompleted += OnCycleCompleted;
            woodpeckerMode.OnSessionCompleted += OnSessionCompleted;
            woodpeckerMode.OnSessionStarted += OnSessionStarted;
        }

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuit);

        if (nextCycleButton != null)
            nextCycleButton.onClick.AddListener(OnNextCycle);

        if (quitSessionButton != null)
            quitSessionButton.onClick.AddListener(OnQuitSession);

        // Hide panels initially
        if (progressPanel != null)
            progressPanel.SetActive(false);

        if (cycleCompletePanel != null)
            cycleCompletePanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (woodpeckerMode != null)
        {
            woodpeckerMode.OnPuzzleChanged -= UpdateProgress;
            woodpeckerMode.OnCycleChanged -= OnCycleChanged;
            woodpeckerMode.OnCycleCompleted -= OnCycleCompleted;
            woodpeckerMode.OnSessionCompleted -= OnSessionCompleted;
            woodpeckerMode.OnSessionStarted -= OnSessionStarted;
        }
    }

    private void Update()
    {
        // Update time display
        if (woodpeckerMode != null && woodpeckerMode.GetCurrentSession() != null && timeText != null)
        {
            var stats = woodpeckerMode.GetAllCycleStats();
            if (stats.Count > 0)
            {
                var currentStats = stats[stats.Count - 1];
                if (!currentStats.EndTime.HasValue)
                {
                    float currentTime = Time.time - sessionStartTime + currentStats.TimeSpentSeconds;
                    timeText.text = FormatTime(currentTime);
                }
            }
        }
    }

    private void OnSessionStarted(WoodpeckerSession session)
    {
        sessionStartTime = Time.time;
        ShowProgressPanel();
        UpdateProgress(0, session.PuzzleIds.Count);
    }

    private void OnCycleChanged(int currentCycle, int totalCycles)
    {
        sessionStartTime = Time.time;
        if (cycleText != null)
        {
            cycleText.text = $"Cycle {currentCycle}/{totalCycles}";
        }
    }

    private void UpdateProgress(int currentPuzzle, int totalPuzzles)
    {
        if (woodpeckerMode == null) return;

        var progress = woodpeckerMode.GetProgress();

        // Update cycle info
        if (cycleText != null)
        {
            cycleText.text = $"Cycle {progress.currentCycle}/{progress.totalCycles}";
        }

        // Update puzzle progress
        if (puzzleProgressText != null)
        {
            puzzleProgressText.text = $"Puzzle {currentPuzzle}/{totalPuzzles}";
        }

        // Update progress bar
        if (progressBar != null)
        {
            progressBar.value = totalPuzzles > 0 ? (float)currentPuzzle / totalPuzzles : 0f;
        }

        // Update accuracy
        if (accuracyText != null)
        {
            var stats = woodpeckerMode.GetAllCycleStats();
            if (stats.Count > 0)
            {
                var currentStats = stats[stats.Count - 1];
                accuracyText.text = $"Accuracy: {currentStats.Accuracy:F1}%";
            }
        }
    }

    private void OnCycleCompleted(WoodpeckerCycleStats stats)
    {
        ShowCycleCompletePanel(stats);
    }

    private void OnSessionCompleted(WoodpeckerSession session)
    {
        ShowSessionCompletePanel(session);
    }

    private void ShowProgressPanel()
    {
        if (progressPanel != null)
            progressPanel.SetActive(true);

        if (cycleCompletePanel != null)
            cycleCompletePanel.SetActive(false);
    }

    private void ShowCycleCompletePanel(WoodpeckerCycleStats stats)
    {
        if (progressPanel != null)
            progressPanel.SetActive(false);

        if (cycleCompletePanel != null)
            cycleCompletePanel.SetActive(true);

        if (cycleCompleteTitle != null)
        {
            cycleCompleteTitle.text = $"Cycle {stats.CycleNumber} Complete!";
        }

        if (cycleStatsText != null)
        {
            cycleStatsText.text = $"Time: {FormatTime(stats.TimeSpentSeconds)}\n" +
                                  $"Accuracy: {stats.Accuracy:F1}%\n" +
                                  $"Correct Moves: {stats.CorrectMoves}\n" +
                                  $"Incorrect Moves: {stats.IncorrectMoves}\n" +
                                  $"Avg Time/Puzzle: {FormatTime(stats.AverageTimePerPuzzle)}";
        }

        // Show comparison with previous cycle
        if (comparisonText != null && stats.CycleNumber > 1 && woodpeckerMode != null)
        {
            var previousStats = woodpeckerMode.GetCycleStats(stats.CycleNumber - 1);
            if (previousStats != null)
            {
                float timeImprovement = ((previousStats.TimeSpentSeconds - stats.TimeSpentSeconds) / previousStats.TimeSpentSeconds) * 100f;
                float accuracyChange = stats.Accuracy - previousStats.Accuracy;

                string timeColor = timeImprovement > 0 ? "green" : "red";
                string accuracyColor = accuracyChange > 0 ? "green" : "red";

                comparisonText.text = $"<color={timeColor}>Time: {(timeImprovement > 0 ? "-" : "+")}{Mathf.Abs(timeImprovement):F1}%</color>\n" +
                                     $"<color={accuracyColor}>Accuracy: {(accuracyChange > 0 ? "+" : "")}{accuracyChange:F1}%</color>";
            }
            else
            {
                comparisonText.text = "First cycle complete!";
            }
        }

        // Check if session is complete
        if (woodpeckerMode != null)
        {
            var session = woodpeckerMode.GetCurrentSession();
            if (session != null && session.CurrentCycle >= session.Settings.TotalCycles)
            {
                if (nextCycleButton != null)
                    nextCycleButton.gameObject.SetActive(false);
            }
            else
            {
                if (nextCycleButton != null)
                    nextCycleButton.gameObject.SetActive(true);
            }
        }
    }

    private void ShowSessionCompletePanel(WoodpeckerSession session)
    {
        if (progressPanel != null)
            progressPanel.SetActive(false);

        if (cycleCompletePanel != null)
            cycleCompletePanel.SetActive(true);

        if (cycleCompleteTitle != null)
        {
            cycleCompleteTitle.text = "Session Complete!";
        }

        if (cycleStatsText != null)
        {
            string summary = "Overall Statistics:\n\n";

            foreach (var cycleStats in session.CycleStats)
            {
                summary += $"Cycle {cycleStats.CycleNumber}: {FormatTime(cycleStats.TimeSpentSeconds)} - {cycleStats.Accuracy:F1}%\n";
            }

            // Calculate improvement
            if (session.CycleStats.Count >= 2)
            {
                var firstCycle = session.CycleStats[0];
                var lastCycle = session.CycleStats[session.CycleStats.Count - 1];
                float timeImprovement = ((firstCycle.TimeSpentSeconds - lastCycle.TimeSpentSeconds) / firstCycle.TimeSpentSeconds) * 100f;
                float accuracyImprovement = lastCycle.Accuracy - firstCycle.Accuracy;

                summary += $"\nTotal Improvement:\n";
                summary += $"Time: {(timeImprovement > 0 ? "-" : "+")}{Mathf.Abs(timeImprovement):F1}%\n";
                summary += $"Accuracy: {(accuracyImprovement > 0 ? "+" : "")}{accuracyImprovement:F1}%";
            }

            cycleStatsText.text = summary;
        }

        if (comparisonText != null)
        {
            comparisonText.text = "Congratulations!";
        }

        if (nextCycleButton != null)
            nextCycleButton.gameObject.SetActive(false);
    }

    private void OnNextCycle()
    {
        if (cycleCompletePanel != null)
            cycleCompletePanel.SetActive(false);

        ShowProgressPanel();

        // The next cycle will be started automatically by WoodpeckerMode
        if (gameManagerRef != null)
        {
            gameManagerRef.nextPuzzle();
        }
    }

    private void OnQuit()
    {
        // Save progress and return to mode selection
        if (progressPanel != null)
            progressPanel.SetActive(false);

        // Could show mode selection UI here
        Debug.Log("Woodpecker session paused. Progress saved.");
    }

    private void OnQuitSession()
    {
        if (woodpeckerMode != null)
        {
            woodpeckerMode.ClearSession();
        }

        if (cycleCompletePanel != null)
            cycleCompletePanel.SetActive(false);

        if (gameManagerRef != null)
        {
            gameManagerRef.SetGameMode(GameMode.PuzzlePractice);
        }

        Debug.Log("Woodpecker session ended.");
    }

    private string FormatTime(float seconds)
    {
        int hours = Mathf.FloorToInt(seconds / 3600);
        int minutes = Mathf.FloorToInt((seconds % 3600) / 60);
        int secs = Mathf.FloorToInt(seconds % 60);

        if (hours > 0)
        {
            return $"{hours}h {minutes}m {secs}s";
        }
        else if (minutes > 0)
        {
            return $"{minutes}m {secs}s";
        }
        else
        {
            return $"{secs}s";
        }
    }
}
