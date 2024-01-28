using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class chessFigure : MonoBehaviour
{
    public bool isWhite;
    public chessTile currentTile;
    public List<Vector2Int> moves;
    public List<Vector2Int> availableMoves;
    private int layerMask;

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
    public chessTile checkPosition() {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out hit, 0.2f, layerMask)) {    
            Transform objectHit = hit.transform;
            chessTile temp = objectHit.GetComponent<chessTile>();
            return temp;
        }
        return null;
    }
    public void setWhite(bool white) {
        isWhite = white;
        if (isWhite) {
            transform.GetComponent<Renderer>().material = currentTile.boardManager.whiteMat;
        }
        else {
            transform.GetComponent<Renderer>().material = currentTile.boardManager.blackMat;
        }
    }
    public abstract List<Vector2Int> possibleMoves();

    public bool inBounds(Vector2Int temp) {
        return !(temp.x < 0 || temp.y < 0 || temp.y >= 8 || temp.x >= 8) ;
       
    }
    public  bool canMove() {
        //TODO pinned 
        return true;
    }
}
