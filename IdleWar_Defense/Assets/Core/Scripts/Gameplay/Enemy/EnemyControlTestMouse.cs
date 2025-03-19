using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControlTestMouse : EnemyControl
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        transform.position = Camera.main.ScreenToWorldPoint(screenPosition);
    }
}
