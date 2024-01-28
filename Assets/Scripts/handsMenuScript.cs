using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handsMenuScript : MonoBehaviour
{
    public Transform UI;
    public Transform leftContr, rightContr;
    private Vector3 uiStartPos;
    private Quaternion uiStartRot;
    // Start is called before the first frame update
    void Awake()
    {
        uiStartPos = UI.position;
        uiStartRot = UI.rotation;
    }
    void OnDisable() {
        print("disabled");
        UI.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        UI.parent = null;
        UI.position = uiStartPos;
        UI.rotation = uiStartRot;
        
    }

    void OnEnable() {

        if(GameObject.Find("L_Palm")) UI.transform.parent = GameObject.Find("L_Palm").transform;
        UI.localPosition = Vector3.zero;
        UI.localScale = new Vector3(0.05f, 0.05f, 0.05f);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
