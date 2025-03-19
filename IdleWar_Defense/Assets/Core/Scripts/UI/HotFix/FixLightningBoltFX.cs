using System.Collections;
using System.Collections.Generic;
using DigitalRuby.ThunderAndLightning;
using UnityEngine;

public class FixLightningBoltFX : MonoBehaviour
{
    public LightningBoltShapeSphereScript lightningBolt;
    
    // Start is called before the first frame update
    void Start()
    {
        lightningBolt.Radius *= transform.lossyScale.x * 100f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
