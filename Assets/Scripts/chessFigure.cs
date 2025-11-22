using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Figures { K,N,B,Q,P,R}
public abstract class chessFigure : MonoBehaviour
{
    public bool hasMoved = false;
    public bool isWhite;
    public chessTile currentTile;
    public List<Vector2Int> moves;
    public List<Vector2Int> availableMoves;
    public List<Vector2Int> forcedMoves;
    private int layerMask;
    public Figures figure;
    public bool isPinned = false;
    // Start is called before the first frame update
    void Start()
    {

    }
    private void Awake() {
        int tileLayerIndex = LayerMask.NameToLayer("tile");
        layerMask = (1 << tileLayerIndex);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public chessTile CheckPosition() {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out hit, 0.2f, layerMask)) {    
            Transform objectHit = hit.transform;
            chessTile temp = objectHit.GetComponent<chessTile>();
            return temp;
        }
        return null;
    }
    public void SetWhite(bool white) {
        isWhite = white;
        if (isWhite) {
            transform.GetComponent<Renderer>().material = currentTile.boardManager.whiteMat;
        }
        else {
            transform.GetComponent<Renderer>().material = currentTile.boardManager.blackMat;
        }
    }
    public abstract List<Vector2Int> PossibleMoves();
    public List<Vector2Int> AddMovesFromDirection(Vector2Int move)
    {
        Vector2Int tempPos = currentTile.position + move;
        List<Vector2Int> mvs = new List<Vector2Int>();
        while (InBounds(tempPos))
        {

            if (currentTile.boardManager.board[tempPos.x, tempPos.y].currentFigure && currentTile.boardManager.board[tempPos.x, tempPos.y].currentFigure.isWhite == isWhite) break;
            if (currentTile.boardManager.board[tempPos.x, tempPos.y].currentFigure && currentTile.boardManager.board[tempPos.x, tempPos.y].currentFigure.isWhite != isWhite) { mvs.Add(tempPos); break; }
            mvs.Add(tempPos);
            tempPos = tempPos + move;
        }
        return mvs;
    }
    public bool InBounds(Vector2Int temp) {
        return !(temp.x < 0 || temp.y < 0 || temp.y >= 8 || temp.x >= 8) ;
       
    }
    public bool CanMove() {
        // Knights cannot move at all when pinned
        // Other pieces have their moves restricted by CheckForPins in boardManager
        if (isPinned && figure == Figures.N) {
            return false;
        }
        return true;
    }
}
