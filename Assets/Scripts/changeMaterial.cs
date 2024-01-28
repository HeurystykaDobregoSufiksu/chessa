using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeMaterial : MonoBehaviour
{

    public void change(Material mat) {
        transform.GetComponent<Renderer>().material = mat;
    }
}
