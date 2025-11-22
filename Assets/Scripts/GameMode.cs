using System;

namespace ChessGame
{
    /// <summary>
    /// Defines the different game modes available in the chess training application
    /// </summary>
    public enum GameMode
    {
        /// <summary>
        /// Standard puzzle practice - solve random puzzles at your own pace
        /// </summary>
        PuzzlePractice,

        /// <summary>
        /// Woodpecker method - repeatedly solve the same set of puzzles across multiple cycles
        /// to build pattern recognition and improve speed
        /// </summary>
        Woodpecker
    }
}
