using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class gameManager : MonoBehaviour
{
    boardManager bm;
    public botHandMovement bot;
    public string gameCSV;
    public bool whiteTurn;
    public bool playerWhite;
    private List<string> correctMoves;
    private string FEN;
    int correctMoveCount = 0;
    public UnityEvent correctMove;
    public UnityEvent wrongMove;
    public UnityEvent resetMats;
    public List<XRBaseInteractor> interactorList;

    public bool testMode;
    public Transform nextUI;
    List<string> puzzleList = new List<string>() { "00sHx,q3k1nr/1pp1nQpp/3p4/1P2p3/4P3/B1PP1b2/B5PP/5K2 b k - 0 17,e8d7 a2e6 d7d8 f7f8,1760,80,83,72,mate mateIn2 middlegame short,https://lichess.org/yyznGmXs/black#34,Italian_Game Italian_Game_Classical_Variation",
                                                    "00sJ9,r3r1k1/p4ppp/2p2n2/1p6/3P1qb1/2NQR3/PPB2PP1/R1B3K1 w - - 5 18,e3g3 e8e1 g1h2 e1c1 a1c1 f4h6 h2g1 h6c1,2671,105,87,325,advantage attraction fork middlegame sacrifice veryLong,https://lichess.org/gyFeQsOE#35,French_Defense French_Defense_Exchange_Variation",
                                                    "00sJb,Q1b2r1k/p2np2p/5bp1/q7/5P2/4B3/PPP3PP/2KR1B1R w - - 1 17,d1d7 a5e1 d7d1 e1e3 c1b1 e3b6,2235,76,97,64,advantage fork long,https://lichess.org/kiuvTFoE#33,Sicilian_Defense Sicilian_Defense_Dragon_Variation",
                                                    "00sO1,1k1r4/pp3pp1/2p1p3/4b3/P3n1P1/8/KPP2PN1/3rBR1R b - - 2 31,b8c7 e1a5 b7b6 f1d1,998,85,94,293,advantage discoveredAttack master middlegame short,https://lichess.org/vsfFkG0s/black#62,"};

// Start is called before the first frame update
    void Start()
    {
        bm = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<boardManager>();
        startGame(gameCSV);
    }
    public void startGame(string csv) {
        print(csv);
        string[] gameInfo = csv.Split(',');
        string[] temp = gameInfo[2].Split(' ');
        correctMoves = new List<string>(temp);
        correctMoveCount = 0;
        print(correctMoves);
        string[] init = gameInfo[1].Split(' ');

        //bm.setChessFigures("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR");
        whiteTurn = init[1] == "w" ? true : false;
        playerWhite = !whiteTurn;
        print(playerWhite + " WOAH");
        if (!testMode) {
            bm.setChessFigures(init[0]);
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
        bm.movePiece(start, stop);
        correctMoveCount += 1;
        whiteTurn = !whiteTurn;
        toggleInteractors(true);
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
                correctMove.Invoke();
                nextUI.gameObject.active = true;
            }
            else {
                makeMove(correctMoves[correctMoveCount]);
            }

            return true;
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
