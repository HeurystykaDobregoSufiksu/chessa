using System;
using System.Collections.Generic;

namespace ChessGame.Models
{
    /// <summary>
    /// Settings for Woodpecker training mode
    /// </summary>
    [Serializable]
    public class WoodpeckerSettings
    {
        /// <summary>
        /// Number of puzzles in the woodpecker set
        /// </summary>
        public int PuzzleCount { get; set; } = 100;

        /// <summary>
        /// Number of cycles to complete
        /// </summary>
        public int TotalCycles { get; set; } = 5;

        /// <summary>
        /// Minimum puzzle rating
        /// </summary>
        public int MinRating { get; set; } = 1000;

        /// <summary>
        /// Maximum puzzle rating
        /// </summary>
        public int MaxRating { get; set; } = 1800;

        /// <summary>
        /// Selected themes for puzzle filtering (null = all themes)
        /// </summary>
        public List<string> SelectedThemes { get; set; }

        /// <summary>
        /// Selected opening tags for puzzle filtering (null = all openings)
        /// </summary>
        public List<string> SelectedOpenings { get; set; }

        public WoodpeckerSettings()
        {
            SelectedThemes = new List<string>();
            SelectedOpenings = new List<string>();
        }
    }

    /// <summary>
    /// Statistics for a single woodpecker cycle
    /// </summary>
    [Serializable]
    public class WoodpeckerCycleStats
    {
        /// <summary>
        /// Cycle number (1-based)
        /// </summary>
        public int CycleNumber { get; set; }

        /// <summary>
        /// Number of puzzles completed in this cycle
        /// </summary>
        public int PuzzlesCompleted { get; set; }

        /// <summary>
        /// Number of correct moves on first attempt
        /// </summary>
        public int CorrectMoves { get; set; }

        /// <summary>
        /// Number of incorrect moves
        /// </summary>
        public int IncorrectMoves { get; set; }

        /// <summary>
        /// Time spent on this cycle in seconds
        /// </summary>
        public float TimeSpentSeconds { get; set; }

        /// <summary>
        /// Timestamp when cycle started
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Timestamp when cycle completed (null if in progress)
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Accuracy percentage (correct moves / total moves * 100)
        /// </summary>
        public float Accuracy
        {
            get
            {
                int totalMoves = CorrectMoves + IncorrectMoves;
                return totalMoves > 0 ? (float)CorrectMoves / totalMoves * 100f : 0f;
            }
        }

        /// <summary>
        /// Average time per puzzle in seconds
        /// </summary>
        public float AverageTimePerPuzzle
        {
            get
            {
                return PuzzlesCompleted > 0 ? TimeSpentSeconds / PuzzlesCompleted : 0f;
            }
        }
    }

    /// <summary>
    /// Complete woodpecker session data
    /// </summary>
    [Serializable]
    public class WoodpeckerSession
    {
        /// <summary>
        /// Unique session ID
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Settings used for this session
        /// </summary>
        public WoodpeckerSettings Settings { get; set; }

        /// <summary>
        /// Current cycle number (1-based)
        /// </summary>
        public int CurrentCycle { get; set; }

        /// <summary>
        /// Current puzzle index within the cycle
        /// </summary>
        public int CurrentPuzzleIndex { get; set; }

        /// <summary>
        /// List of puzzle IDs in this woodpecker set (fixed for all cycles)
        /// </summary>
        public List<string> PuzzleIds { get; set; }

        /// <summary>
        /// Statistics for each completed or in-progress cycle
        /// </summary>
        public List<WoodpeckerCycleStats> CycleStats { get; set; }

        /// <summary>
        /// Session creation timestamp
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Is the session completed?
        /// </summary>
        public bool IsCompleted { get; set; }

        public WoodpeckerSession()
        {
            SessionId = Guid.NewGuid().ToString();
            PuzzleIds = new List<string>();
            CycleStats = new List<WoodpeckerCycleStats>();
            CurrentCycle = 1;
            CurrentPuzzleIndex = 0;
            CreatedAt = DateTime.Now;
            IsCompleted = false;
        }
    }
}
