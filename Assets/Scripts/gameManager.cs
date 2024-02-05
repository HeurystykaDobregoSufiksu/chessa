using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class gameManager : MonoBehaviour
{
    boardManager bm;
    public botHandMovement bot;
   
    public bool whiteTurn;
    public bool playerWhite;
    private List<string> correctMoves;
    private string FEN;
    int correctMoveCount = 0;
    int wrongMoveCount = 0;
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
        puzzleList = _DBService.GetPuzzles(null);
        bm = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<boardManager>();
        //nextPuzzle();
        PuzzleModel pm;
        pm = new PuzzleModel(); pm.FEN = "8/5k2/2b5/8/8/8/2Q5/1K6 w - - 0 1"; pm.Moves = "c2c3 c6d5 c3b3"; // Pin bishop (NIE DZIALA)
        pm = new PuzzleModel(); pm.FEN = "8/5k2/2b5/8/8/2Q5/8/1K6 w - - 0 1"; pm.Moves = "c3b3 c6d5 b3b4"; // check (bishop moze wszedzie, król może się cofac po diagonalu checka)
        pm = new PuzzleModel(); pm.FEN = "r3k2r/2q5/8/8/8/8/P6P/R1Q1K2R w - - 0 1"; pm.Moves = "c1c2 e8g8"; // black castle  (NIE DZIALA)
        pm = new PuzzleModel(); pm.FEN = "r3k2r/2q5/8/8/8/8/P1Q4P/R3K2R b - - 0 1"; pm.Moves = "c7c8 e0a0"; // white castle  (NIE DZIALA)
     /*        pm = new PuzzleModel(); pm.FEN = "r3k2r/3q4/8/8/8/8/P6P/R1Q1K2R w - - 0 1"; pm.Moves = "c1c2 e8c8"; // black castle long blocked
                pm = new PuzzleModel(); pm.FEN = "r3k2r/3q4/8/8/8/8/8/R1Q1K2R w - - 0 1"; pm.Moves = "c1c4 e8g8"; // black castle short blocked*/


        startGame(pm);
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
       
        int id = Random.Range(0, puzzleList.Count);
        bm.resetBoard();
        resetMats.Invoke();

        startGame(puzzleList[id]);
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

            if (correctMoveCount >= correctMoves.Count) {

                if (wrongMoveCount == 0)
                {
                    //++elo
                }
                
                correctMove.Invoke();
                nextUI.gameObject.active = true;
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
