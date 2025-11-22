using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnFigure : chessFigure
{
   
    public PawnFigure()
    {
        if (isWhite)
        {
            moves = new List<Vector2Int>()
            {
                new Vector2Int(-1, 0),
                new Vector2Int(-1, -1),
                 new Vector2Int(-1, 1),
            };
            if (!hasMoved) moves.Add(new Vector2Int(-2, 0));
        }
        else
        {
            moves = new List<Vector2Int>()
            {
                new Vector2Int(1, 0),
                new Vector2Int(1, -1),
                 new Vector2Int(1, 1),
            };
            if (!hasMoved) moves.Add(new Vector2Int(2, 0));
        }

        figure = Figures.P;
    }

    public override List<Vector2Int> PossibleMoves()
    {
        if (isPinned && forcedMoves != null && forcedMoves.Count > 0)
        {
            availableMoves = forcedMoves;
            return forcedMoves;
        }

        List<Vector2Int> tempList = new List<Vector2Int>();
        foreach (var move in moves)
        {
            Vector2Int temp = currentTile.position + move;
            if (!InBounds(temp)) continue;

            // Forward moves (including double-move)
            if (move.y == 0)
            {
                // Check if destination is empty
                if (!currentTile.boardManager.board[temp.x, temp.y].currentFigure)
                {
                    // For double-move, also check intermediate square
                    if (Math.Abs(move.x) == 2)
                    {
                        Vector2Int intermediate = currentTile.position + new Vector2Int(move.x / 2, 0);
                        if (!currentTile.boardManager.board[intermediate.x, intermediate.y].currentFigure)
                        {
                            tempList.Add(temp);
                        }
                    }
                    else
                    {
                        tempList.Add(temp);
                    }
                }
            }
            // Diagonal captures
            else if (currentTile.boardManager.board[temp.x, temp.y].currentFigure &&
                     currentTile.boardManager.board[temp.x, temp.y].currentFigure.isWhite != isWhite)
            {
                tempList.Add(temp);
            }
        }
        availableMoves = tempList;
        return tempList;
    }


}
