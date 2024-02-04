using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnFigure : chessFigure
{
   
    public PawnFigure()
    {
        moves = new List<Vector2Int>() { new Vector2Int(-2, 1), new Vector2Int(-2, -1), new Vector2Int(2, 1), new Vector2Int(2, -1), new Vector2Int(-1, 2), new Vector2Int(-1, -2), new Vector2Int(1, 2), new Vector2Int(1, -2) };

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
        List<Vector2Int> tempList = new List<Vector2Int>();
        foreach (var move in moves)
        {
            var temp = currentTile.position + move;
            if (!InBounds(temp)) continue;
            if (move.y==0 && !currentTile.boardManager.board[temp.x, temp.y].currentFigure) tempList.Add(temp);
            else if (currentTile.boardManager.board[temp.x, temp.y].currentFigure && currentTile.boardManager.board[temp.x, temp.y].currentFigure.isWhite != isWhite) tempList.Add(temp);
        }
        availableMoves = tempList;
        return tempList;
    }


}
