using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class botHandMovement : MonoBehaviour
{
    public Transform handTarget,holdParent,lookAtTarget;
    private Vector3 handTargetStartPosition;
    public ChainIKConstraint chainIK;

    public MultiAimConstraint lookConstraint;
    public float AnimSpeed;
    public gameManager gm;
    public bool continueLooking = false;
    // Start is called before the first frame update
    void Start()
    {
        handTargetStartPosition = handTarget.position;
 /*       chainIK = handTarget.parent.GetComponent<ChainIKConstraint>();*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void lookAtSetParent(Transform parent = null) {
       // StopCoroutine("looking");
        if(parent == null) {
            continueLooking = false;
            return;
        }
        continueLooking = true;
        StartCoroutine(looking(parent));
        //lookAtTarget.localPosition = Vector3.zero;
        //bool val = parent == null ? false : true;
        //print(val);
        //StartCoroutine(looking(val));
    }

    public void makePlay(chessTile startTile, chessTile endTile) {
        StartCoroutine(handMovement(startTile, endTile));
        
    }
    IEnumerator looking(Transform target) {
        float timeElapsed = 0;
        while (timeElapsed < AnimSpeed) {
            lookConstraint.weight = Mathf.Lerp(0, 1, timeElapsed / AnimSpeed);
            timeElapsed += Time.deltaTime * 2;
            if(continueLooking) lookAtTarget.position = target.position;

            yield return null;
        }
        while (continueLooking) {
            lookAtTarget.position = target.position;
            yield return null;
        }
        timeElapsed = 0;
        while (timeElapsed < AnimSpeed) {
            lookConstraint.weight = Mathf.Lerp(1, 0, timeElapsed / AnimSpeed);
            timeElapsed += Time.deltaTime * 2;
            yield return null;
        }
    }

    IEnumerator handMovement(chessTile startTile, chessTile endTile) {
    handTargetStartPosition = handTarget.position;
    startTile.currentFigure.transform.GetComponent<Collider>().enabled = false;
    Transform prevParent = startTile.currentFigure.transform.parent;
    this.Invoke(() => lookAtSetParent(startTile.currentFigure.transform),AnimSpeed);
    float timeElapsed = 0;
    while (timeElapsed < AnimSpeed) {
        chainIK.weight = Mathf.Lerp(0, 1, timeElapsed / AnimSpeed);
        timeElapsed += Time.deltaTime;
        yield return null;
    }
    Vector3 nextHandPosition = handTargetStartPosition;
        timeElapsed = 0;
        nextHandPosition = new Vector3(startTile.transform.position.x, startTile.transform.position.y + 0.1f, startTile.transform.position.z) ;
        while (timeElapsed < AnimSpeed) {

            handTarget.position = Vector3.Lerp(handTargetStartPosition, nextHandPosition, timeElapsed / AnimSpeed);
            timeElapsed += Time.deltaTime * 2;
            yield return null;
        }
        handTarget.position = nextHandPosition;

        yield return null;
        timeElapsed = 0;
    while (timeElapsed < AnimSpeed) {
        handTarget.position = Vector3.Lerp(nextHandPosition, startTile.transform.position + new Vector3(0,0.1f,0) - (holdParent.position - handTarget.position), timeElapsed / AnimSpeed);
        timeElapsed += Time.deltaTime;
        yield return null;
    }
    nextHandPosition = startTile.transform.position + new Vector3(0, 0.1f, 0) - (holdParent.position - handTarget.position);
    handTarget.position = nextHandPosition;
    yield return null;
    startTile.currentFigure.transform.parent = holdParent;
    timeElapsed = 0;
    while (timeElapsed < AnimSpeed) {
        handTarget.position = Vector3.Lerp(nextHandPosition, endTile.transform.position + new Vector3(0, 0.1f, 0) - (holdParent.position - handTarget.position), timeElapsed / AnimSpeed);
        timeElapsed += Time.deltaTime;
        yield return null;
    }
    nextHandPosition = endTile.transform.position + new Vector3(0, 0.1f, 0) - (holdParent.position - handTarget.position);
    handTarget.position = nextHandPosition;
    yield return null;
    lookAtSetParent();
    startTile.currentFigure.transform.parent = prevParent;
    startTile.currentFigure.transform.GetComponent<Collider>().enabled = true;

    gm.botMoveCompleted(startTile,endTile);
    timeElapsed = 0;
    while (timeElapsed < AnimSpeed) {
        handTarget.position = Vector3.Lerp(nextHandPosition, handTargetStartPosition, timeElapsed / AnimSpeed);
        timeElapsed += Time.deltaTime;
        yield return null;
    }

    timeElapsed = 0;
    while (timeElapsed < AnimSpeed) {
        chainIK.weight = Mathf.Lerp(1, 0, timeElapsed / AnimSpeed);
        timeElapsed += Time.deltaTime;
        yield return null;
    }
}
}
