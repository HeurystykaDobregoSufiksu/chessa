using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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
    public gameManager gm;
    private chessFigure currentFigure;
    public pieceHolder whitePieceHolder;
    public pieceHolder blackPieceHolder;
    public botHandMovement bot;
    GameObject figuresContainer;
    List<string> letterArray = new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H" };
    public (chessTile, chessTile) prevMove = (null, null);
    public bool isCheck = false;
    public List<Vector2Int> tilesToBlockCheck = new();
    // Start is called before the first frame update
    void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<gameManager>();
        boardModelScale = boardModel.transform.localScale;
        boardCenter = boardModel.transform.Find("boardCenter");
        
        tilePreset.transform.localScale = tilePreset.transform.localScale * boardModelScale.x;
        tileSize = tilePreset.transform.localScale.x;
        print(boardModel.transform.localScale.x);
        boardParent = new GameObject("boardParent");
        figuresContainer = new GameObject("figures");

        boardParent.transform.parent = boardModel.transform;
        generateChessTiles();
    }

    void generateChessTiles()
    {
        for (int i = -4; i < 4; i++)
        {
            for (int j = -4; j < 4; j++)
            {
                GameObject temp = GameObject.Instantiate(tilePreset, new Vector3(boardCenter.position.x + i * tileSize + tileSize / 2, boardCenter.position.y + zAxis, boardCenter.position.z + j * tileSize + tileSize / 2), Quaternion.identity, boardParent.transform);
                temp.name = letterArray[i + 4] + (9 - (j + 5)).ToString();
                board[i + 4, j + 4] = temp.GetComponent<chessTile>();
                board[i + 4, j + 4].position = new Vector2Int(i + 4, j + 4);
                chessTile ct = temp.GetComponent<chessTile>();
                ct.boardManager = this;
            }
        }
    }
    public void setChessFigures(string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR")
    {
        //var fenList = fen.Split();
        var fenPositions = fen.Split('/');
        figuresContainer.transform.parent = boardModel.transform;
        print(fen);
        for (int i = 0; i < fenPositions.Length; i++)
        {
            int offset = 0;
            for (int j = 0; j < fenPositions[i].Length; j++)
            {
                if (int.TryParse(fenPositions[i][j].ToString(), out int fenNumber))
                {
                    offset += fenNumber - 1;

                }
                else
                {
                    Transform temp;
                    var curr = board[i, j + offset];
                    switch (fenPositions[i][j].ToString().ToLower())
                    {
                        case "r":
                            temp = Instantiate(RookFigure, curr.transform.position, Quaternion.Euler(-90, 0, 0), figuresContainer.transform);
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
                    curr.currentFigure.SetWhite(pieceWhite);
                    if (pieceWhite == gm.playerWhite)
                    {
                        temp.GetComponent<XRGrabInteractable>().interactionLayers = (1 << InteractionLayerMask.NameToLayer("playersPiece"));
                    }
                    curr.currentFigure.currentTile.moveFigureToTile();
                }

            }
        }
    }

    public void resetBoard()
    {
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
    public void onPieceSelect(chessFigure figure, bool isMouse = false)
    {
        if (isMouse && currentFigure) onPieceDrop(currentFigure);
        gm.toggleInteractors(false);

        /*        if (currentFigure != null) {
                    currentFigure.GetComponent<XRBaseInteractable>().selectingInteractor.enableInteractions = false;
                    currentFigure.currentTile.moveFigureToTile();
                }*/
        currentFigure = figure;
        currentFigure.currentTile.changeOverlay(0);
        if(!isMouse) currentFigure.GetComponent<XRBaseInteractable>().selectingInteractor.enableInteractions = true;
        currentFigure.PossibleMoves();
        foreach (var poss in currentFigure.availableMoves)
        {
            board[poss.x, poss.y].changeOverlay(3);
        }
        hs.setTarget(currentFigure.transform);
        bot.lookAtSetParent(currentFigure.transform);
        print(currentFigure);
    }
    public void onPieceDrop(chessFigure figure, chessTile mouseTile = null)
    {
        bot.lookAtSetParent();

        currentFigure.currentTile.hideOverlay();

        foreach (var poss in currentFigure.availableMoves)
        {
            board[poss.x, poss.y].hideOverlay();
        }
        chessTile newTile;
        gm.toggleInteractors(true);
        if (mouseTile) {
            newTile = mouseTile; //Mouse Controls
        }
        else {
            newTile = currentFigure.CheckPosition(); //VR controls
        }
        

        if (newTile == null || newTile == currentFigure.currentTile)
        {
            print(newTile);
            figure.currentTile.moveFigureToTile();
            return;
        }

        if (gm.testMode)
        {
            movePiece(currentFigure.currentTile, newTile);
        }
        else
        {
            bool valid = gm.checkMove(currentFigure.currentTile.position, newTile.position);
            if (valid)
            {
                //EVENT EMIT ETD
            }
            else
            {
                currentFigure.currentTile.moveFigureToTile();
            }
        }

        currentFigure = null;

        hs.setTarget();

    }
    public void movePiece(chessTile startPos, chessTile endPos, bool playerMove = true)
    {

        if (endPos.currentFigure != null)
        {
            var currHolder = endPos.currentFigure.isWhite ? whitePieceHolder : blackPieceHolder;
            currHolder.placePiece(endPos.currentFigure.transform);
        }

        if (prevMove.Item1 != null && prevMove.Item2 != null)
        {
            prevMove.Item1.hideOverlay();
            prevMove.Item2.hideOverlay();
        }

        if (!startPos.currentFigure.PossibleMoves().Contains(endPos.position)) return;

        prevMove = (startPos, endPos);
        prevMove.Item1.changeOverlay(1);
        prevMove.Item2.changeOverlay(2);

        endPos.currentFigure = startPos.currentFigure;
        endPos.currentFigure.currentTile = endPos;
        bool castleShort = false ;
        bool castleLong = false;
        if (endPos.currentFigure.figure == Figures.K)
        {
            var s = startPos.position - endPos.position;
            if (s.x < -1) castleShort = true;
            else if (s.x > 1) castleLong = true;
        }
        if (endPos.currentFigure.isWhite)
        {
            if(castleShort) movePiece(board[7, 7], board[7, 5], true);
            if(castleLong) movePiece(board[7, 0], board[7, 3], true);
        }
        else
        {
            if (castleShort) movePiece(board[0, 7], board[0, 5], true);
            if (castleLong) movePiece(board[0, 0], board[0, 3], true);
        }
       
        endPos.moveFigureToTile();
        startPos.currentFigure = null;
        if (!playerMove)
        {
            isCheck = IsKingInCheck();
            CheckForPins();
        }
        else
        {
            endPos.currentFigure.hasMoved = true;
        }
    }
    public List<Vector2Int> GetSquaresBetween(int kingX, int kingY, int pieceX, int pieceY)
    {
        List<Vector2Int> lista = new();
        // Determine direction of attack
        int xDirection = Math.Sign(pieceX - kingX);
        int yDirection = Math.Sign(pieceY - kingY);

        // Current position, starting next to the king
        int currentX = kingX + xDirection;
        int currentY = kingY + yDirection;

        // Iterate until we reach the attacking piece
        while (currentX != pieceX || currentY != pieceY)
        {
            if (!InBounds(new Vector2Int(currentX, currentY))) break;
            lista.Add(new Vector2Int(currentX, currentY));
            currentX += xDirection;
            currentY += yDirection;
        }
        return lista;
    }
    private bool IsKingInCheck()
    {
        tilesToBlockCheck = new();
        isCheck = false;
        chessTile KP = FindKingPosition();
        List<Vector2Int> checks = new List<Vector2Int>();

        for (int x = 0; x < 8; x += 1)
        {
            for (int y = 0; y < 8; y += 1)
            {
                var tileFigure = board[x, y].currentFigure;
                if (tileFigure && tileFigure.isWhite != gm.playerWhite)
                {
                    var pm = tileFigure.PossibleMoves();
                    if (pm.Contains(KP.position) )
                    {
                        if(tileFigure.figure != Figures.N) tilesToBlockCheck = GetSquaresBetween(KP.position.x, KP.position.y, board[x, y].position.x, board[x, y].position.y);

                        checks.Add(tileFigure.currentTile.position);
                        isCheck = true;
                    }
                }
            }
        }
        if (checks.Count>1)
        {
            tilesToBlockCheck = new List<Vector2Int>();
        }

        return isCheck;
    }
  

    private void CheckForPins()
    {
        // Reset all pin flags for player's pieces
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                var tileFigure = board[x, y].currentFigure;
                if (tileFigure && tileFigure.isWhite == gm.playerWhite)
                {
                    tileFigure.isPinned = false;
                    tileFigure.forcedMoves = new List<Vector2Int>();
                }
            }
        }

        chessTile KP = FindKingPosition();
        var possibleDirections = new List<Vector2Int>() {
           new Vector2Int(0, 1),
           new Vector2Int(1, 0),
           new Vector2Int(-1, 0),
           new Vector2Int(0, -1),
           new Vector2Int(-1, 1),
           new Vector2Int(1, -1),
           new Vector2Int(-1, -1),
           new Vector2Int(1, 1)
        };

        possibleDirections.ForEach(x => FindPinsFromDirection(KP.position, x));

    }
    private bool InBounds(Vector2Int temp)
    {
        return !(temp.x < 0 || temp.y < 0 || temp.y >= 8 || temp.x >= 8);
    }

    public void FindPinsFromDirection(Vector2Int kingPos, Vector2Int direction)
    {
        chessFigure playerFigure = null ;
        var tempPos = kingPos + direction;
        List<Vector2Int> mvs = new List<Vector2Int>();

        while (InBounds(tempPos))
        {
            var tileFigure = board[tempPos.x, tempPos.y].currentFigure;
            if (tileFigure && tileFigure.isWhite == gm.playerWhite)
            {
                if (playerFigure is not null) break;
                playerFigure = tileFigure;
            }

            if ( tileFigure && tileFigure.isWhite != gm.playerWhite)
            {
                List<Vector2Int> forcedMoves = new();
                if (playerFigure is null) break;
                if(tileFigure.figure==Figures.B && direction.x!=0 && direction.y != 0) forcedMoves = GetSquaresBetween(playerFigure.currentTile.position.x, playerFigure.currentTile.position.y, tileFigure.currentTile.position.x, tileFigure.currentTile.position.y);
                if (tileFigure.figure == Figures.R && (direction.x == 0 || direction.y == 0)) forcedMoves = GetSquaresBetween(playerFigure.currentTile.position.x, playerFigure.currentTile.position.y, tileFigure.currentTile.position.x, tileFigure.currentTile.position.y);
                if (tileFigure.figure == Figures.Q && ((direction.x == 0 || direction.y == 0) || (direction.x != 0 && direction.y != 0))) forcedMoves = GetSquaresBetween(playerFigure.currentTile.position.x, playerFigure.currentTile.position.y, tileFigure.currentTile.position.x, tileFigure.currentTile.position.y);

                playerFigure.isPinned = true;

                // Set availableMoves to only the moves that keep the piece on the pin line
                var p = playerFigure.PossibleMoves();
                var f = forcedMoves.Where(x => p.Contains(x)).ToList();

                // Also allow capturing the attacking piece
                f.Add(tileFigure.currentTile.position);

                playerFigure.forcedMoves = f;
                playerFigure.availableMoves = f;
                break;
            }

            tempPos = tempPos + direction;

        }
    }

    private chessTile FindKingPosition()
    {

        for (int x = 0; x < 8; x += 1)
        {
            for (int y = 0; y < 8; y += 1)
            {
                if (board[x, y].currentFigure && board[x, y].currentFigure.figure == Figures.K && board[x, y].currentFigure.isWhite == gm.playerWhite) return board[x, y];
            }
        }
        return null;
    }

    void Update()
    {
        if (currentFigure != null)
        {
            currentFigure.CheckPosition();
        }
        // Do something with the object that was hit by the raycast.
    }
}
