using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingFigure : chessFigure
{
    public bool isChecked = false;
    public KingFigure()
    {
        moves = new List<Vector2Int>() {
            new Vector2Int(-1, -1), // Bottom left
            new Vector2Int(-1, 0),  // Left
            new Vector2Int(-1, 1),  // Top left
            new Vector2Int(0, -1),  // Bottom
            new Vector2Int(0, 1),   // Top
            new Vector2Int(1, -1),  // Bottom right
            new Vector2Int(1, 0),   // Right
            new Vector2Int(1, 1),    // Top right
            
        };
        if (!hasMoved) 
        {
            moves.Add(new Vector2Int(0, 0));
            moves.Add(new Vector2Int(9, 9)); 
        }
        figure = Figures.K;
    }
    private bool IsTileUnderAttack(Vector2Int tile)
    {
        chessTile[,] board = currentTile.boardManager.board;
        gameManager gm = currentTile.boardManager.gm;

        for (int x = 0; x < 8; x += 1)
        {
            for (int y = 0; y < 8; y += 1)
            {
                if (currentTile.position.x == x && currentTile.position.y == y) continue;
                chessFigure tileFigure = board[x, y].currentFigure;
                if (tileFigure && tileFigure.isWhite != gm.playerWhite)
                {
                    if (tileFigure.PossibleMoves().Contains(tile)) return true;
                }
            }
        }

        return false;
    }
    public override List<Vector2Int> PossibleMoves() {
        chessTile[,] board = currentTile.boardManager.board;
        gameManager gm= currentTile.boardManager.gm;
        List<Vector2Int> tempList = new List<Vector2Int>();
        Vector2Int left = new Vector2Int(-1, 0);
        Vector2Int right = new Vector2Int(1, 0);
        foreach (var move in moves) {

            
            Vector2Int temp = currentTile.position + move;
            if(!InBounds(temp)) continue;
            if (board[temp.x, temp.y].currentFigure && board[temp.x, temp.y].currentFigure.isWhite == isWhite) continue;
            if (IsTileUnderAttack(temp)) continue;
            if (move.x == 0 && move.y == 0)
            {
                // Short castle (kingside)
                if (gm.playerWhite)
                {
                    var casteShort = currentTile.position + 2 * right;
                    var pos1 = currentTile.position + right;
                    var pos2 = currentTile.position + 2 * right;
                    var rookPos = currentTile.position + 3 * right;

                    if (InBounds(pos1) && InBounds(pos2) && InBounds(rookPos) &&
                        !board[pos1.x, pos1.y].currentFigure &&
                        !board[pos2.x, pos2.y].currentFigure &&
                        board[rookPos.x, rookPos.y].currentFigure &&
                        board[rookPos.x, rookPos.y].currentFigure.figure == Figures.R &&
                        !board[rookPos.x, rookPos.y].currentFigure.hasMoved &&
                        !IsTileUnderAttack(currentTile.position) &&
                        !IsTileUnderAttack(pos1) &&
                        !IsTileUnderAttack(pos2))
                    {
                        tempList.Add(casteShort);
                    }
                }
                else
                {
                    var casteShort = currentTile.position + 2 * left;
                    var pos1 = currentTile.position + left;
                    var pos2 = currentTile.position + 2 * left;
                    var rookPos = currentTile.position + 3 * left;

                    if (InBounds(pos1) && InBounds(pos2) && InBounds(rookPos) &&
                        !board[pos1.x, pos1.y].currentFigure &&
                        !board[pos2.x, pos2.y].currentFigure &&
                        board[rookPos.x, rookPos.y].currentFigure &&
                        board[rookPos.x, rookPos.y].currentFigure.figure == Figures.R &&
                        !board[rookPos.x, rookPos.y].currentFigure.hasMoved &&
                        !IsTileUnderAttack(currentTile.position) &&
                        !IsTileUnderAttack(pos1) &&
                        !IsTileUnderAttack(pos2))
                    {
                        tempList.Add(casteShort);
                    }
                }
            }
            else if (move.x == 9 && move.y == 9)
            {
                // Long castle (queenside)
                if (gm.playerWhite)
                {
                    var casteLong = currentTile.position + 2 * left;
                    var pos1 = currentTile.position + left;
                    var pos2 = currentTile.position + 2 * left;
                    var pos3 = currentTile.position + 3 * left;
                    var rookPos = currentTile.position + 4 * left;

                    if (InBounds(pos1) && InBounds(pos2) && InBounds(pos3) && InBounds(rookPos) &&
                        !board[pos1.x, pos1.y].currentFigure &&
                        !board[pos2.x, pos2.y].currentFigure &&
                        !board[pos3.x, pos3.y].currentFigure &&
                        board[rookPos.x, rookPos.y].currentFigure &&
                        board[rookPos.x, rookPos.y].currentFigure.figure == Figures.R &&
                        !board[rookPos.x, rookPos.y].currentFigure.hasMoved &&
                        !IsTileUnderAttack(currentTile.position) &&
                        !IsTileUnderAttack(pos1) &&
                        !IsTileUnderAttack(pos2))
                    {
                        tempList.Add(casteLong);
                    }
                }
                else
                {
                    var casteLong = currentTile.position + 2 * right;
                    var pos1 = currentTile.position + right;
                    var pos2 = currentTile.position + 2 * right;
                    var pos3 = currentTile.position + 3 * right;
                    var rookPos = currentTile.position + 4 * right;

                    if (InBounds(pos1) && InBounds(pos2) && InBounds(pos3) && InBounds(rookPos) &&
                        !board[pos1.x, pos1.y].currentFigure &&
                        !board[pos2.x, pos2.y].currentFigure &&
                        !board[pos3.x, pos3.y].currentFigure &&
                        board[rookPos.x, rookPos.y].currentFigure &&
                        board[rookPos.x, rookPos.y].currentFigure.figure == Figures.R &&
                        !board[rookPos.x, rookPos.y].currentFigure.hasMoved &&
                        !IsTileUnderAttack(currentTile.position) &&
                        !IsTileUnderAttack(pos1) &&
                        !IsTileUnderAttack(pos2))
                    {
                        tempList.Add(casteLong);
                    }
                }
            }
            else
            {
                tempList.Add(temp);
            }

        }
        availableMoves = tempList;
        return tempList;
    }

    // Start is called before the first frame update
    
}
