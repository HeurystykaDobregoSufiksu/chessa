using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenFigure : chessFigure
{
    public QueenFigure()
    {
        moves = new List<Vector2Int>() {
           new Vector2Int(0, 1),
           new Vector2Int(1, 0),
            new Vector2Int(-1, 0), 
            new Vector2Int(0, -1),
            new Vector2Int(-1, 1),
           new Vector2Int(1, -1),
            new Vector2Int(-1, -1),  
            new Vector2Int(1, 1)
        };
        figure = Figures.Q;
    }

    public override List<Vector2Int> PossibleMoves() {
        List<Vector2Int> tempList = new List<Vector2Int>();
        moves.ForEach(move => tempList.AddRange(AddMovesFromDirection(move)));
        availableMoves = tempList;
        return tempList;
    }
    

  

}
