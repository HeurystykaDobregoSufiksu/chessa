using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kmbInputs : MonoBehaviour
{
    public Camera camera;
    public boardManager bm;
    private chessFigure currFig;
    void Start() {

    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit)) {
                Transform objectHit = hit.transform;
                chessFigure figure = hit.transform.GetComponent<chessFigure>();
                if (figure) {
                    print("WOAH KLIKNALES W FIGURE");
                    bm.onPieceSelect(figure,true);
                    currFig = figure;
                }
                else {
                    chessTile tile = hit.transform.GetComponent<chessTile>();
                    if(tile && currFig) {
                        print("WOAH KLIKNALES W POLE PO KLIKNIECIU W FIGURE");

                        bm.onPieceDrop(currFig,tile);
                        currFig = null;
                    }
                }

                // Do something with the object that was hit by the raycast.
            }
        }
    }
}
