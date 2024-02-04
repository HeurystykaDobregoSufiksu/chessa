using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class knightFigure : chessFigure
{
   
    public override List<Vector2Int> PossibleMoves() {
        if(!CanMove()) return new List<Vector2Int>(); // parent

        List<Vector2Int> tempList = new List<Vector2Int>();
        foreach (var move in moves) {
            var temp = currentTile.position + move;
            if(!InBounds(temp)) continue;
            if (currentTile.boardManager.board[temp.x, temp.y].currentFigure && currentTile.boardManager.board[temp.x, temp.y].currentFigure.isWhite == isWhite) continue;
            tempList.Add(temp);
        }
        availableMoves = tempList;
        return tempList;
    }

    // Start is called before the first frame update
    void Start()
    {
        figure = Figures.N;
        moves = new List<Vector2Int>() { new Vector2Int(-2, 1), new Vector2Int(-2, -1), new Vector2Int(2, 1), new Vector2Int(2, -1), new Vector2Int(-1, 2), new Vector2Int(-1, -2), new Vector2Int(1, 2), new Vector2Int(1, -2) };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
