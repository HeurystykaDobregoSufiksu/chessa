using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class boardManager : MonoBehaviour
{
    public chessTile[,] board = new chessTile[8, 8];
    public GameObject boardModel;
    private Vector3 boardModelScale;
    private Transform boardCenter;
    public GameObject tilePreset;
    private GameObject boardParent;
    public float zAxis;
    private float tileSize;
    public Material whiteMat, blackMat;
    public Transform PawnFigure, KnightFigure, RookFigure, KingFigure, QueenFigure, BishopFigure;
    public hoverScript hs;
    public LayerMask lm;
    public Camera cam;
    private chessTile lastHighlighted;
    private gameManager gm;
    private chessFigure currentFigure;
    public pieceHolder whitePieceHolder;
    public pieceHolder blackPieceHolder;
    public botHandMovement bot;
    GameObject figuresContainer;
    List<string> letterArray = new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H" };
    public (chessTile, chessTile) prevMove = (null,null);
    // Start is called before the first frame update
    void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<gameManager>();
        boardModelScale = boardModel.transform.localScale;
        boardCenter = boardModel.transform.Find("boardCenter");
        print(boardCenter.position);
        tilePreset.transform.localScale = tilePreset.transform.localScale * boardModelScale.x;
        tileSize = tilePreset.transform.localScale.x;
        print(boardModel.transform.localScale.x);
        boardParent = new GameObject("boardParent");
        figuresContainer = new GameObject("figures");

        boardParent.transform.parent = boardModel.transform;
        generateChessTiles();
    }

    void generateChessTiles() {
        for (int i = -4; i < 4; i++) {
            for (int j = -4; j < 4; j++) {
                GameObject temp = GameObject.Instantiate(tilePreset, new Vector3(boardCenter.position.x + i * tileSize + tileSize / 2, boardCenter.position.y + zAxis, boardCenter.position.z + j * tileSize + tileSize / 2), Quaternion.identity, boardParent.transform);
                temp.name = letterArray[i + 4] + ( 9 - (j+5)).ToString();
                board[i + 4, j + 4] = temp.GetComponent<chessTile>();
                board[i + 4, j + 4].position = new Vector2Int(i + 4, j + 4);
                chessTile ct = temp.GetComponent<chessTile>();
                ct.boardManager = this;
            }
        }
    }
    public void setChessFigures(string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR") {
        //var fenList = fen.Split();
        var fenPositions = fen.Split('/');
        figuresContainer.transform.parent = boardModel.transform;
        print(fen);
        for(int i = 0; i < fenPositions.Length; i++) {
            int offset = 0;
            for(int j = 0; j < fenPositions[i].Length; j++) {
                if(int.TryParse(fenPositions[i][j].ToString(), out int fenNumber)) {
                    offset += fenNumber -1;

                }
                else {
                    Transform temp;
                    var curr = board[i, j + offset];
                    switch (fenPositions[i][j].ToString().ToLower()) {
                        case "r":
                            temp = Instantiate(RookFigure, curr.transform.position, Quaternion.Euler(-90,0,0), figuresContainer.transform);
                            break;
                        case "n":
                            temp = Instantiate(KnightFigure, curr.transform.position, Quaternion.Euler(-90, 0, 0), figuresContainer.transform);
                            break;
                        case "b":
                            temp = Instantiate(BishopFigure, curr.transform.position, Quaternion.Euler(-90, 0, 0), figuresContainer.transform);
                            break;
                        case "q":
                            temp = Instantiate(QueenFigure, curr.transform.position, Quaternion.Euler(-90, 0, 0), figuresContainer.transform);
                            break;
                        case "k":
                            temp = Instantiate(KingFigure, curr.transform.position, Quaternion.Euler(-90, 0, 0), figuresContainer.transform);
                            break;
                        case "p":
                            temp = Instantiate(PawnFigure, curr.transform.position, Quaternion.Euler(-90, 0, 0), figuresContainer.transform);
                            break;
                        default:
                            temp = null;
                            break;
                    }
                    if (temp is null) continue;
                    temp.transform.localScale = temp.transform.localScale * boardModelScale.x;
                    curr.currentFigure = temp.GetComponent<chessFigure>();
                    curr.currentFigure.currentTile = curr;
                    bool pieceWhite = fenPositions[i][j].ToString().ToLower() != fenPositions[i][j].ToString();
                    curr.currentFigure.setWhite(pieceWhite);
                    if(pieceWhite == gm.playerWhite) {
                        temp.GetComponent<XRGrabInteractable>().interactionLayers = (1 << InteractionLayerMask.NameToLayer("playersPiece"));
                    }
                    curr.currentFigure.currentTile.moveFigureToTile();
                }

            }
        }
    }

    public void resetBoard() {
        boardModel.transform.rotation = Quaternion.Euler(0, 90, 0);
        Destroy(boardParent.gameObject);
        boardParent = new GameObject("boardParent");
        boardParent.transform.parent = boardModel.transform;
        Destroy(figuresContainer.gameObject);
        whitePieceHolder.resetHolder();
        blackPieceHolder.resetHolder();
        figuresContainer = new GameObject("figures");
        generateChessTiles();

    }
    public void onPieceSelect(chessFigure figure) {
        gm.toggleInteractors(false);

/*        if (currentFigure != null) {
            currentFigure.GetComponent<XRBaseInteractable>().selectingInteractor.enableInteractions = false;
            currentFigure.currentTile.moveFigureToTile();
        }*/
        currentFigure = figure;
        currentFigure.currentTile.changeOverlay(0);
        currentFigure.GetComponent<XRBaseInteractable>().selectingInteractor.enableInteractions = true;
        currentFigure.possibleMoves();
        foreach (var poss in currentFigure.availableMoves) {
            board[poss.x, poss.y].changeOverlay(3);
        }
        hs.setTarget(currentFigure.transform);
        bot.lookAtSetParent(currentFigure.transform);
        print(currentFigure);
    }
    public void onPieceDrop(chessFigure figure) {
        bot.lookAtSetParent();

        currentFigure.currentTile.hideOverlay();

        foreach (var poss in currentFigure.availableMoves) {
            board[poss.x, poss.y].hideOverlay();
        }

        gm.toggleInteractors(true);
        chessTile newTile = currentFigure.checkPosition();

        if(newTile == null || newTile == currentFigure.currentTile) {
            figure.currentTile.moveFigureToTile();
            return;
        }



        if (gm.testMode) {
            movePiece(currentFigure.currentTile, newTile);
        }
        else {
            bool valid = gm.checkMove(currentFigure.currentTile.position, newTile.position);
            if (valid) {
                //EVENT EMIT ETD
            }
            else {
                currentFigure.currentTile.moveFigureToTile();
            }
        }
        
        currentFigure = null;

        hs.setTarget();

    }
    public void movePiece(chessTile startPos, chessTile endPos) {

        if (endPos.currentFigure != null) {
            var currHolder = gm.whiteTurn ? whitePieceHolder : blackPieceHolder;
            currHolder.placePiece(endPos.currentFigure.transform);
        }

        if (prevMove.Item1 != null && prevMove.Item2 != null) {
            prevMove.Item1.hideOverlay();
            prevMove.Item2.hideOverlay();
        }
        prevMove = (startPos, endPos);
        prevMove.Item1.changeOverlay(1);
        prevMove.Item2.changeOverlay(2);

        endPos.currentFigure = startPos.currentFigure;
        endPos.currentFigure.currentTile = endPos;
        endPos.moveFigureToTile();
        startPos.currentFigure = null;
    }
    void Update()
    {
        if(currentFigure != null) {
            currentFigure.checkPosition();
        }
            // Do something with the object that was hit by the raycast.
    }
}
