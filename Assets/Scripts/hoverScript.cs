using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hoverScript : MonoBehaviour
{
    private Transform target;
    private LineRenderer lr;
    private Transform hitPoint;
    private int layerMask;
    // Start is called before the first frame update
    void Start()
    {
        target = null;
        lr = transform.GetComponent<LineRenderer>();
        hitPoint = transform.Find("hitPoint");
        int tileLayerIndex = LayerMask.NameToLayer("tile");
        layerMask = (1 << tileLayerIndex);
    }
    public void setTarget(Transform target = null) {
        this.target = target;
        if(target == null) {
            lr.enabled = false;
            hitPoint.gameObject.active = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(target != null) {
            RaycastHit hit;
            Ray ray = new Ray(target.position, Vector3.down);
            if (Physics.Raycast(ray, out hit, 0.2f, layerMask)) {
                lr.enabled = true;
                hitPoint.gameObject.active = true;
                Transform objectHit = hit.transform;
                lr.SetPosition(0, target.position);
                lr.SetPosition(1, hit.point);
                hitPoint.position = objectHit.position;
                // Do something with the object that was hit by the raycast.
                chessTile temp = objectHit.GetComponent<chessTile>();
            }
            else {
                lr.enabled = false;
                hitPoint.gameObject.active = false;
            }

        }
    }
}
