using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using ChessGame;

public class gameManager : MonoBehaviour
{
    boardManager bm;
    public botHandMovement bot;

    public bool whiteTurn;
    public bool playerWhite;
    [Header("Scenario Section")]
    public bool isScenario;
    public string scenFEN, scenMoves;

    [Header("Game Mode Section")]
    public GameMode currentGameMode = GameMode.PuzzlePractice;
    private WoodpeckerMode woodpeckerMode;

    private List<string> correctMoves;
    private string FEN;
    int correctMoveCount = 0;
    int wrongMoveCount = 0;

    [Header("Events Section")]

    public UnityEvent correctMove;
    public UnityEvent wrongMove;
    public UnityEvent resetMats;
    public List<XRBaseInteractor> interactorList;
    private DBService _DBService;
    public bool testMode;
    public Transform nextUI;
    List<PuzzleModel> puzzleList = new List<PuzzleModel>();

// Start is called before the first frame update
    void Start()
    {
        _DBService = new();
        bm = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<boardManager>();

        // Initialize woodpecker mode
        woodpeckerMode = FindObjectOfType<WoodpeckerMode>();
        if (woodpeckerMode == null)
        {
            GameObject wpGO = new GameObject("WoodpeckerMode");
            woodpeckerMode = wpGO.AddComponent<WoodpeckerMode>();
        }

        // Check if game mode was set from the menu
        if (PlayerPrefs.HasKey("SelectedGameMode"))
        {
            int gameModeValue = PlayerPrefs.GetInt("SelectedGameMode");
            currentGameMode = (GameMode)gameModeValue;
            PlayerPrefs.DeleteKey("SelectedGameMode"); // Clear after reading
            Debug.Log($"Game mode loaded from menu: {currentGameMode}");
        }

        if (isScenario) {
            PuzzleModel pm;
            pm = new PuzzleModel(); pm.FEN = scenFEN; pm.Moves = scenMoves;
            startGame(pm);
        }
        else {
            // Check if there's an active woodpecker session
            if (currentGameMode == GameMode.Woodpecker)
            {
                LoadWoodpeckerSession();
            }
            else
            {
                LoadPuzzlesWithDefaultSettings();
                nextPuzzle();
            }
        }
    }

    private void LoadPuzzlesWithDefaultSettings()
    {
        // Load default puzzles or from saved settings
        PuzzleSettingsManager settingsManager = FindObjectOfType<PuzzleSettingsManager>();
        if (settingsManager != null)
        {
            PuzzleSettings settings = settingsManager.GetSettings();
            ReloadPuzzles(settings);
        }
        else
        {
            puzzleList = _DBService.GetPuzzles(null);
        }
    }

    public void ReloadPuzzles(PuzzleSettings settings)
    {
        List<string> themes = null;

        // Combine themes and openings for filtering
        if (settings.SelectedThemes.Count > 0 || settings.SelectedOpenings.Count > 0)
        {
            themes = new List<string>();
            themes.AddRange(settings.SelectedThemes);
            themes.AddRange(settings.SelectedOpenings);
        }

        puzzleList = _DBService.GetPuzzles(themes, settings.MinRating, settings.MaxRating);

        if (puzzleList.Count == 0)
        {
            Debug.LogWarning("No puzzles found matching the current filters. Loading all puzzles.");
            puzzleList = _DBService.GetPuzzles(null);
        }

        Debug.Log($"Loaded {puzzleList.Count} puzzles with filters: Rating {settings.MinRating}-{settings.MaxRating}, Themes: {(themes != null ? string.Join(", ", themes) : "All")}");
    }

    private void LoadWoodpeckerSession()
    {
        var session = woodpeckerMode.LoadSession();
        if (session != null && !session.IsCompleted)
        {
            Debug.Log("Resuming existing Woodpecker session");
            woodpeckerMode.ResumeSession(session);
            nextPuzzle();
        }
        else
        {
            Debug.Log("No active Woodpecker session found");
            // Could show UI to start a new session or fall back to puzzle practice
            currentGameMode = GameMode.PuzzlePractice;
            LoadPuzzlesWithDefaultSettings();
            nextPuzzle();
        }
    }

    public void StartWoodpeckerMode(ChessGame.Models.WoodpeckerSettings settings)
    {
        currentGameMode = GameMode.Woodpecker;
        woodpeckerMode.StartNewSession(settings);
        nextPuzzle();
    }

    public void SetGameMode(GameMode mode)
    {
        currentGameMode = mode;
        if (mode == GameMode.PuzzlePractice)
        {
            LoadPuzzlesWithDefaultSettings();
        }
    }
    public void startGame(PuzzleModel puzzle) {
        // print(puzzle.PuzzleId);
        print("xzxzxzxz");
        correctMoves = new List<string>(puzzle.Moves.Split(' '));
        correctMoveCount = wrongMoveCount= 0;
        print(correctMoves);
        string[] FEN = puzzle.FEN.Split(' ');

        //bm.setChessFigures("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR");
        whiteTurn = FEN[1] == "w" ? true : false;
        playerWhite = !whiteTurn;
        print(playerWhite + " WOAH");
        print(puzzle.Moves);
        print(puzzle.FEN);
        if (!testMode) {
            bm.setChessFigures(FEN[0]);
        }
        else {
            bm.setChessFigures();
        }
        bm.boardModel.transform.rotation = playerWhite ? Quaternion.Euler(0, -90, 0) : Quaternion.Euler(0, 90, 0);

        if (!testMode) makeMove(correctMoves[correctMoveCount]);
    }
    public void nextPuzzle() {
        bm.resetBoard();
        resetMats.Invoke();

        PuzzleModel puzzle = null;

        if (currentGameMode == GameMode.Woodpecker)
        {
            puzzle = woodpeckerMode.GetCurrentPuzzle();
            if (puzzle == null)
            {
                Debug.Log("Woodpecker cycle/session complete!");
                // Could show completion UI here
                return;
            }
            Debug.Log($"Woodpecker: Cycle {woodpeckerMode.GetProgress().currentCycle}/{woodpeckerMode.GetProgress().totalCycles}, Puzzle {woodpeckerMode.GetProgress().currentPuzzle}/{woodpeckerMode.GetProgress().totalPuzzles}");
        }
        else
        {
            // Standard puzzle practice mode
            if (puzzleList.Count == 0)
            {
                Debug.LogError("No puzzles available!");
                return;
            }
            int id = Random.Range(0, puzzleList.Count);
            puzzle = puzzleList[id];
        }

        startGame(puzzle);
        bm.hs.setTarget();
        nextUI.gameObject.active = false;
    }
    public void makeMove(string move) {
        if (correctMoveCount >= correctMoves.Count) return; //ENDGAME
        toggleInteractors(false);
        Vector2Int correctStart = convertToPosition(correctMoves[correctMoveCount].Substring(0, 2));
        Vector2Int correctStop = convertToPosition(correctMoves[correctMoveCount].Substring(2, 2));
        bot.makePlay(bm.board[correctStart.x, correctStart.y], bm.board[correctStop.x, correctStop.y]);
    }
    public void botMoveCompleted(chessTile start, chessTile stop) {
        bm.movePiece(start, stop,false);
        correctMoveCount += 1;
        whiteTurn = !whiteTurn;
        toggleInteractors(true);

       // makeMove(correctMoves[correctMoveCount]); //UWAGA TYLKO DO TESTOW BOT GRA CALY CZAS
    }
    public bool checkMove(Vector2Int start, Vector2Int stop) {
        if (correctMoveCount >= correctMoves.Count) return false; //WIN CONDITION
        Vector2Int correctStart = convertToPosition(correctMoves[correctMoveCount].Substring(0,2));
        Vector2Int correctStop = convertToPosition(correctMoves[correctMoveCount].Substring(2, 2));
        print(correctStart);
        print(correctStop);
        if(start == correctStart && stop == correctStop) {
            bm.movePiece(bm.board[correctStart.x, correctStart.y], bm.board[correctStop.x, correctStop.y]);
            correctMoveCount += 1;
            whiteTurn = !whiteTurn;

            // Record correct move for woodpecker mode
            if (currentGameMode == GameMode.Woodpecker)
            {
                woodpeckerMode.RecordCorrectMove();
            }

            if (correctMoveCount >= correctMoves.Count) {

                if (wrongMoveCount == 0)
                {
                    //++elo
                }

                correctMove.Invoke();

                // Handle puzzle completion based on game mode
                if (currentGameMode == GameMode.Woodpecker)
                {
                    // Automatically advance to next puzzle in woodpecker mode
                    PuzzleModel nextWoodpeckerPuzzle = woodpeckerMode.GetNextPuzzle();
                    if (nextWoodpeckerPuzzle != null)
                    {
                        this.Invoke(() => nextPuzzle(), 1.0f);
                    }
                    else
                    {
                        // Cycle or session complete
                        nextUI.gameObject.active = true;
                    }
                }
                else
                {
                    nextUI.gameObject.active = true;
                }
            }
            else {
                makeMove(correctMoves[correctMoveCount]);
            }

            return true;
        }
        else
        {
            //--elo
            wrongMoveCount += 1;

            // Record incorrect move for woodpecker mode
            if (currentGameMode == GameMode.Woodpecker)
            {
                woodpeckerMode.RecordIncorrectMove();
            }
        }

        wrongMove.Invoke();
        this.Invoke(() => resetMats.Invoke(), 0.5f);
        return false;
    }
    public void toggleInteractors(bool val) {
        foreach (var interactor in interactorList) {
            interactor.enableInteractions = val;
        }
    }
    private Vector2Int convertToPosition(string chessNotation) {
        List<string> letterArray = new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H" };
        return new Vector2Int(8 - int.Parse(chessNotation[1].ToString()), letterArray.IndexOf(chessNotation[0].ToString().ToUpper()));
    }

    // Update is called once per frame
    void Update(){
        
    }
}
