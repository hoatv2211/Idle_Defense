using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OderLayerZShadown : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 pos = transform.position;
        pos.z = 100f + OderLayerZ.PIVOT_POINT;
        transform.position = pos;
    }
}
