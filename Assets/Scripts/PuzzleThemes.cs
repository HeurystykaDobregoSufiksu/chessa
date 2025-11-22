using System.Collections.Generic;

public static class PuzzleThemes
{
    // Common chess puzzle themes from Lichess
    public static readonly List<string> CommonThemes = new List<string>
    {
        "advancedPawn",
        "advantage",
        "attackingF2F7",
        "attraction",
        "backRankMate",
        "bishopEndgame",
        "capturingDefender",
        "clearance",
        "defensiveMove",
        "deflection",
        "discoveredAttack",
        "doubleBishopMate",
        "doubleCheck",
        "endgame",
        "enPassant",
        "exposedKing",
        "fork",
        "hangingPiece",
        "hookMate",
        "interference",
        "intermezzo",
        "kingsideAttack",
        "knightEndgame",
        "long",
        "mate",
        "mateIn1",
        "mateIn2",
        "mateIn3",
        "mateIn4",
        "mateIn5",
        "middlegame",
        "opening",
        "pawnEndgame",
        "pin",
        "promotion",
        "queenEndgame",
        "queenRookEndgame",
        "queensideAttack",
        "quietMove",
        "rookEndgame",
        "sacrifice",
        "short",
        "skewer",
        "smotheredMate",
        "trappedPiece",
        "underPromotion",
        "veryLong",
        "xRayAttack",
        "zugzwang"
    };

    // Common opening tags
    public static readonly List<string> CommonOpenings = new List<string>
    {
        "Sicilian",
        "French",
        "Italian",
        "Spanish",
        "Queen's Gambit",
        "King's Indian",
        "Caro-Kann",
        "English",
        "Ruy Lopez",
        "Scandinavian",
        "Alekhine",
        "Pirc",
        "Modern",
        "Benoni",
        "Nimzo-Indian",
        "Grunfeld",
        "Dutch",
        "Catalan",
        "London System",
        "King's Gambit"
    };

    // Most popular/essential themes for UI (to avoid cluttering)
    public static readonly List<string> PopularThemes = new List<string>
    {
        "mate",
        "mateIn1",
        "mateIn2",
        "mateIn3",
        "fork",
        "pin",
        "skewer",
        "discoveredAttack",
        "sacrifice",
        "endgame",
        "middlegame",
        "opening",
        "backRankMate",
        "promotion",
        "deflection"
    };
}
