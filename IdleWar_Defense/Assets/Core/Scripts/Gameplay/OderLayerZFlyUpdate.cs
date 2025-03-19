using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OderLayerZFlyUpdate : MonoBehaviour {
	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        Vector3 pos = transform.position;
        pos.z = -100f + pos.y * 10f + OderLayerZ.PIVOT_POINT;
        transform.position = pos;
    }
}
