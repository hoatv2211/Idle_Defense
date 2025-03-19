using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OderLayerZ : MonoBehaviour {
    public const float Z_HP = 200f;
    public const float Z_BULLET = 300f;
    public const float PIVOT_POINT = 600f;

    // Use this for initialization
    private bool mInitialized;
    //do object pool từ cái object[0] đang đc active nên hàm OnEnable luôn đc gọi trước Init
    void Start () {
        Vector3 pos = transform.position;
        pos.z = pos.y * 10f + PIVOT_POINT;
        transform.position = pos;

        mInitialized = true;
    }

    private void OnEnable()
    {
        if (mInitialized)
        {
            Vector3 pos = transform.position;
            pos.z = pos.y * 10f + PIVOT_POINT;
            transform.position = pos;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
