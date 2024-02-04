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
        var board = currentTile.boardManager.board;
        var gm = currentTile.boardManager.gm;
       
        for (int x = 0; x < 8; x += 1)
        {
            for (int y = 0; y < 8; y += 1)
            {
                var tileFigure = board[x, y].currentFigure;
                if (tileFigure && tileFigure.isWhite != gm.playerWhite)
                {
                    if (tileFigure.PossibleMoves().Contains(tile)) return true;
                }
            }
        }

        return false;
    }
    public override List<Vector2Int> PossibleMoves() {
        var board = currentTile.boardManager.board;
        var gm= currentTile.boardManager.gm;
        List<Vector2Int> tempList = new List<Vector2Int>();
        var left = new Vector2Int(-1, 0);
        var right = new Vector2Int(1, 0);
        foreach (var move in moves) {

            
            var temp = currentTile.position + move;
            if(!InBounds(temp)) continue;
            if (board[temp.x, temp.y].currentFigure && board[temp.x, temp.y].currentFigure.isWhite == isWhite) continue;
            if (IsTileUnderAttack(temp)) continue;
            if (move.x == 0 && move.y == 0)
            {
                if (gm.playerWhite)
                {
                    for(int i = 0; i < 3; i += 1)
                    {
                        var casteShort= currentTile.position + 2*right;
                        var pos = currentTile.position + right;
                        if (!board[pos.x, pos.y].currentFigure && i < 2) break;

                        if(i==2 && board[pos.x, pos.y].currentFigure && board[pos.x, pos.y].currentFigure.figure == Figures.R && !board[pos.x, pos.y].currentFigure.hasMoved) tempList.Add(casteShort);
                    }
                }
                else
                {
                    for (int i = 0; i < 3; i += 1)
                    {
                        var casteShort = currentTile.position + 2 * left;
                        var pos = currentTile.position + left;
                        if (!board[pos.x, pos.y].currentFigure && i < 2) break;
                        if (i == 2 && board[pos.x, pos.y].currentFigure && board[pos.x, pos.y].currentFigure.figure == Figures.R && !board[pos.x, pos.y].currentFigure.hasMoved) tempList.Add(casteShort);
                    }
                }
            }
            else if (move.x == 9 && move.y == 9)
            {
                //castle long
                if (gm.playerWhite)
                {
                    for (int i = 0; i < 4; i += 1)
                    {
                        var casteLong= currentTile.position + 2 * left;

                        var pos = currentTile.position + left;
                        if (!board[pos.x, pos.y].currentFigure && i < 3) break;
                        if (i == 3 && board[pos.x, pos.y].currentFigure && board[pos.x, pos.y].currentFigure.figure == Figures.R && !board[pos.x, pos.y].currentFigure.hasMoved) tempList.Add(casteLong);
                    }
                }
                else
                {
                    for (int i = 0; i < 4; i += 1)
                    {
                        var casteLong = currentTile.position + 2 * right;
                        var pos = currentTile.position + right;
                        if (!board[pos.x, pos.y].currentFigure && i < 3) break;
                        if (i == 3 && board[pos.x, pos.y].currentFigure && board[pos.x, pos.y].currentFigure.figure == Figures.R && !board[pos.x, pos.y].currentFigure.hasMoved) tempList.Add(casteLong);
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
