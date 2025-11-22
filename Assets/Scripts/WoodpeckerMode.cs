using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ChessGame.Models;

namespace ChessGame
{
    /// <summary>
    /// Manages woodpecker training mode logic
    /// </summary>
    public class WoodpeckerMode : MonoBehaviour
    {
        public static WoodpeckerMode Instance { get; private set; }

        [SerializeField] private DBService dbService;

        private WoodpeckerSession currentSession;
        private WoodpeckerCycleStats currentCycleStats;
        private float cycleStartTime;
        private List<PuzzleModel> allPuzzles;
        private int currentPuzzleIndexInCycle;

        // Events
        public event Action<WoodpeckerSession> OnSessionStarted;
        public event Action<WoodpeckerCycleStats> OnCycleCompleted;
        public event Action<WoodpeckerSession> OnSessionCompleted;
        public event Action<int, int> OnPuzzleChanged; // currentIndex, totalCount
        public event Action<int, int> OnCycleChanged; // currentCycle, totalCycles

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
        }

        private void Start()
        {
            if (dbService == null)
            {
                dbService = FindObjectOfType<DBService>();
            }
        }

        /// <summary>
        /// Starts a new woodpecker session with the given settings
        /// </summary>
        public void StartNewSession(WoodpeckerSettings settings)
        {
            Debug.Log($"Starting new Woodpecker session: {settings.PuzzleCount} puzzles, {settings.TotalCycles} cycles");

            // Create new session
            currentSession = new WoodpeckerSession
            {
                Settings = settings,
                CurrentCycle = 1,
                CurrentPuzzleIndex = 0,
                CreatedAt = DateTime.Now,
                IsCompleted = false
            };

            // Load puzzles for this session
            allPuzzles = dbService.GetPuzzles(
                settings.SelectedThemes?.Count > 0 ? settings.SelectedThemes : null,
                settings.MinRating,
                settings.MaxRating,
                settings.PuzzleCount
            );

            if (allPuzzles == null || allPuzzles.Count == 0)
            {
                Debug.LogError("Failed to load puzzles for Woodpecker mode!");
                return;
            }

            // Store puzzle IDs in session
            currentSession.PuzzleIds = allPuzzles.Select(p => p.PuzzleId).ToList();

            Debug.Log($"Loaded {allPuzzles.Count} puzzles for Woodpecker session");

            // Start first cycle
            StartCycle(1);

            // Save session
            SaveSession();

            OnSessionStarted?.Invoke(currentSession);
        }

        /// <summary>
        /// Resumes an existing woodpecker session
        /// </summary>
        public void ResumeSession(WoodpeckerSession session)
        {
            currentSession = session;

            // Load the puzzles based on stored IDs
            allPuzzles = dbService.GetPuzzlesByIds(currentSession.PuzzleIds);

            if (allPuzzles == null || allPuzzles.Count == 0)
            {
                Debug.LogError("Failed to load puzzles for resumed session!");
                return;
            }

            // Resume current cycle
            currentCycleStats = currentSession.CycleStats.LastOrDefault();
            if (currentCycleStats == null || currentCycleStats.EndTime.HasValue)
            {
                // Start a new cycle if the last one was completed
                StartCycle(currentSession.CurrentCycle);
            }
            else
            {
                cycleStartTime = Time.time - currentCycleStats.TimeSpentSeconds;
            }

            currentPuzzleIndexInCycle = currentSession.CurrentPuzzleIndex;

            Debug.Log($"Resumed Woodpecker session: Cycle {currentSession.CurrentCycle}/{currentSession.Settings.TotalCycles}, Puzzle {currentPuzzleIndexInCycle + 1}/{allPuzzles.Count}");
        }

        /// <summary>
        /// Starts a new cycle
        /// </summary>
        private void StartCycle(int cycleNumber)
        {
            Debug.Log($"Starting cycle {cycleNumber}/{currentSession.Settings.TotalCycles}");

            currentCycleStats = new WoodpeckerCycleStats
            {
                CycleNumber = cycleNumber,
                PuzzlesCompleted = 0,
                CorrectMoves = 0,
                IncorrectMoves = 0,
                TimeSpentSeconds = 0,
                StartTime = DateTime.Now,
                EndTime = null
            };

            currentSession.CycleStats.Add(currentCycleStats);
            currentSession.CurrentCycle = cycleNumber;
            currentSession.CurrentPuzzleIndex = 0;
            currentPuzzleIndexInCycle = 0;
            cycleStartTime = Time.time;

            OnCycleChanged?.Invoke(currentSession.CurrentCycle, currentSession.Settings.TotalCycles);
        }

        /// <summary>
        /// Gets the current puzzle
        /// </summary>
        public PuzzleModel GetCurrentPuzzle()
        {
            if (allPuzzles == null || currentPuzzleIndexInCycle >= allPuzzles.Count)
            {
                return null;
            }

            return allPuzzles[currentPuzzleIndexInCycle];
        }

        /// <summary>
        /// Gets the next puzzle in the current cycle
        /// </summary>
        public PuzzleModel GetNextPuzzle()
        {
            if (currentSession == null || allPuzzles == null)
            {
                Debug.LogWarning("No active Woodpecker session");
                return null;
            }

            currentPuzzleIndexInCycle++;
            currentSession.CurrentPuzzleIndex = currentPuzzleIndexInCycle;

            // Update cycle stats
            currentCycleStats.PuzzlesCompleted++;
            currentCycleStats.TimeSpentSeconds = Time.time - cycleStartTime;

            OnPuzzleChanged?.Invoke(currentPuzzleIndexInCycle, allPuzzles.Count);

            // Check if cycle is complete
            if (currentPuzzleIndexInCycle >= allPuzzles.Count)
            {
                CompleteCycle();
                return null;
            }

            SaveSession();
            return allPuzzles[currentPuzzleIndexInCycle];
        }

        /// <summary>
        /// Records a correct move
        /// </summary>
        public void RecordCorrectMove()
        {
            if (currentCycleStats != null)
            {
                currentCycleStats.CorrectMoves++;
                currentCycleStats.TimeSpentSeconds = Time.time - cycleStartTime;
                SaveSession();
            }
        }

        /// <summary>
        /// Records an incorrect move
        /// </summary>
        public void RecordIncorrectMove()
        {
            if (currentCycleStats != null)
            {
                currentCycleStats.IncorrectMoves++;
                currentCycleStats.TimeSpentSeconds = Time.time - cycleStartTime;
                SaveSession();
            }
        }

        /// <summary>
        /// Completes the current cycle
        /// </summary>
        private void CompleteCycle()
        {
            if (currentCycleStats == null) return;

            currentCycleStats.EndTime = DateTime.Now;
            currentCycleStats.TimeSpentSeconds = Time.time - cycleStartTime;

            Debug.Log($"Cycle {currentSession.CurrentCycle} completed!");
            Debug.Log($"  Puzzles: {currentCycleStats.PuzzlesCompleted}");
            Debug.Log($"  Accuracy: {currentCycleStats.Accuracy:F1}%");
            Debug.Log($"  Time: {FormatTime(currentCycleStats.TimeSpentSeconds)}");

            OnCycleCompleted?.Invoke(currentCycleStats);

            // Check if all cycles are complete
            if (currentSession.CurrentCycle >= currentSession.Settings.TotalCycles)
            {
                CompleteSession();
            }
            else
            {
                // Automatically start next cycle
                StartCycle(currentSession.CurrentCycle + 1);
            }

            SaveSession();
        }

        /// <summary>
        /// Completes the entire woodpecker session
        /// </summary>
        private void CompleteSession()
        {
            currentSession.IsCompleted = true;
            Debug.Log("Woodpecker session completed!");

            // Show summary statistics
            ShowSessionSummary();

            OnSessionCompleted?.Invoke(currentSession);
            SaveSession();
        }

        /// <summary>
        /// Shows summary statistics for the completed session
        /// </summary>
        private void ShowSessionSummary()
        {
            Debug.Log("=== Woodpecker Session Summary ===");
            foreach (var stats in currentSession.CycleStats)
            {
                Debug.Log($"Cycle {stats.CycleNumber}: {FormatTime(stats.TimeSpentSeconds)} - {stats.Accuracy:F1}% accuracy");
            }

            // Calculate improvement
            if (currentSession.CycleStats.Count >= 2)
            {
                var firstCycle = currentSession.CycleStats[0];
                var lastCycle = currentSession.CycleStats[currentSession.CycleStats.Count - 1];
                float timeImprovement = ((firstCycle.TimeSpentSeconds - lastCycle.TimeSpentSeconds) / firstCycle.TimeSpentSeconds) * 100f;
                float accuracyImprovement = lastCycle.Accuracy - firstCycle.Accuracy;

                Debug.Log($"Time Improvement: {timeImprovement:F1}%");
                Debug.Log($"Accuracy Change: {accuracyImprovement:+0.0;-0.0}%");
            }
        }

        /// <summary>
        /// Saves the current session to PlayerPrefs
        /// </summary>
        private void SaveSession()
        {
            if (currentSession == null) return;

            string json = JsonUtility.ToJson(currentSession);
            PlayerPrefs.SetString("WoodpeckerSession", json);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Loads the saved session from PlayerPrefs
        /// </summary>
        public WoodpeckerSession LoadSession()
        {
            if (PlayerPrefs.HasKey("WoodpeckerSession"))
            {
                string json = PlayerPrefs.GetString("WoodpeckerSession");
                return JsonUtility.FromJson<WoodpeckerSession>(json);
            }
            return null;
        }

        /// <summary>
        /// Clears the saved session
        /// </summary>
        public void ClearSession()
        {
            PlayerPrefs.DeleteKey("WoodpeckerSession");
            PlayerPrefs.Save();
            currentSession = null;
            currentCycleStats = null;
            allPuzzles = null;
        }

        /// <summary>
        /// Gets the current session
        /// </summary>
        public WoodpeckerSession GetCurrentSession()
        {
            return currentSession;
        }

        /// <summary>
        /// Formats time in seconds to a readable string
        /// </summary>
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

        /// <summary>
        /// Gets progress information for UI
        /// </summary>
        public (int currentCycle, int totalCycles, int currentPuzzle, int totalPuzzles) GetProgress()
        {
            if (currentSession == null)
            {
                return (0, 0, 0, 0);
            }

            return (
                currentSession.CurrentCycle,
                currentSession.Settings.TotalCycles,
                currentPuzzleIndexInCycle + 1,
                allPuzzles?.Count ?? 0
            );
        }

        /// <summary>
        /// Gets statistics for a specific cycle
        /// </summary>
        public WoodpeckerCycleStats GetCycleStats(int cycleNumber)
        {
            if (currentSession == null || cycleNumber < 1)
            {
                return null;
            }

            return currentSession.CycleStats.FirstOrDefault(s => s.CycleNumber == cycleNumber);
        }

        /// <summary>
        /// Gets all cycle statistics
        /// </summary>
        public List<WoodpeckerCycleStats> GetAllCycleStats()
        {
            return currentSession?.CycleStats ?? new List<WoodpeckerCycleStats>();
        }
    }
}
