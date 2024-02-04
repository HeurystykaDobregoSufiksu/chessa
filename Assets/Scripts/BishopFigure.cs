using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BishopFigure : chessFigure
{
    public BishopFigure()
    {
        moves = new List<Vector2Int>() {
           new Vector2Int(-1, 1),
           new Vector2Int(1, -1),
            new Vector2Int(-1, -1),  // Top left
            new Vector2Int(1, 1)
        };
        figure = Figures.B;
    }

    public override List<Vector2Int> PossibleMoves() {
        List<Vector2Int> tempList = new List<Vector2Int>();
        moves.ForEach(move => tempList.AddRange(AddMovesFromDirection(move)));
        availableMoves = tempList;
        return tempList;
    }
    

  

}
