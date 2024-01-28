using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pieceHolder : MonoBehaviour
{
    int currCount = 0;
    public List<Transform> pieceSpot;
    // Start is called before the first frame update
    public void resetHolder() {
        currCount = 0;
    }
    public void placePiece(Transform piece) {
        piece.transform.position = pieceSpot[currCount].position;
        currCount += 1;
        piece.GetComponent<Collider>().enabled = false;
    }
}
