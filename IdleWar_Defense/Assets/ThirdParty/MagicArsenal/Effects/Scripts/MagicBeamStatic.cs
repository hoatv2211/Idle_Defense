﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MagicArsenal
{

public class MagicBeamStatic : MonoBehaviour
{

    [Header("Prefabs")]
    public GameObject beamLineRendererPrefab; //Put a prefab with a line renderer onto here.
    public GameObject beamStartPrefab; //This is a prefab that is put at the start of the beam.
    public GameObject beamEndPrefab; //Prefab put at end of beam.

    private GameObject beamStart;
    private GameObject beamEnd;
    private GameObject beam;
    private LineRenderer line;

    [Header("Beam Options")]
    public bool alwaysOn = true; //Enable this to spawn the beam when script is loaded.
    public bool beamCollides = true; //Beam stops at colliders
    public float beamLength = 100; //Ingame beam length
    public float beamEndOffset = 0f; //How far from the raycast hit point the end effect is positioned
    public float textureScrollSpeed = 0f; //How fast the texture scrolls along the beam, can be negative or positive.
    public float textureLengthScale = 1f;   //Set this to the horizontal length of your texture relative to the vertical. 
                                            //Example: if texture is 200 pixels in height and 600 in length, set this to 3
    
    public float timeSpawn = 0.25f;
    public UnityEvent OnBeamEnd;
    private float timeSpawnCounter;
    void Start()
    {
        
    }

    private void OnEnable()
    {
        if (alwaysOn) //When the object this script is attached to is enabled, spawn the beam.
            SpawnBeam();
    }

    private void OnDisable() //If the object this script is attached to is disabled, remove the beam.
    {
        RemoveBeam();
    }

    private void OnDestroy()
    {
        RemoveBeam();
    }

    void FixedUpdate()
    {
        if (beam) //Updates the beam
        {
            line.SetPosition(0, transform.position);

            Vector3 end;
            RaycastHit hit;
            // if (beamCollides && Physics.Raycast(transform.position, transform.forward, out hit)) //Checks for collision
            //     end = hit.point - (transform.forward * beamEndOffset);
            // else
            timeSpawnCounter -= Time.fixedDeltaTime;
            end = Vector3.Lerp(transform.position, transform.position + (transform.forward * beamLength), 1f - timeSpawnCounter / 0.25f);

            line.SetPosition(1, end);

            if (beamStart)
            {
                beamStart.transform.position = transform.position;
                beamStart.transform.LookAt(end);
            }
            if (beamEnd)
            {
                beamEnd.transform.position = end;
                beamEnd.transform.LookAt(beamStart.transform.position);
            }

            float distance = Vector3.Distance(transform.position, end);
            line.material.mainTextureScale = new Vector2(distance / textureLengthScale, 1); //This sets the scale of the texture so it doesn't look stretched
            line.material.mainTextureOffset -= new Vector2(Time.fixedDeltaTime * textureScrollSpeed, 0); //This scrolls the texture along the beam if not set to 0
        }
    }

    public void SpawnBeam() //This function spawns the prefab with linerenderer
    {
        //timeSpawn = 0.25f;
        
        CancelInvoke();
        Invoke(nameof(BeamEnd),timeSpawn);
        if (beamLineRendererPrefab)
        {
            if (beamStartPrefab)
                beamStart = Instantiate(beamStartPrefab, transform.position, Quaternion.identity);
            if (beamEndPrefab)
                beamEnd = Instantiate(beamEndPrefab, transform.position, Quaternion.identity);
            beam = Instantiate(beamLineRendererPrefab);
            beam.transform.position = transform.position;
            beam.transform.parent = transform;
            beam.transform.rotation = transform.rotation;
            line = beam.GetComponent<LineRenderer>();
            line.useWorldSpace = true;
            timeSpawnCounter = timeSpawn;
            #if UNITY_5_5_OR_NEWER
			line.positionCount = 2;
			#else
			line.SetVertexCount(2); 
			#endif
        }
        else
            print("Add a prefab with a line renderer to the MagicBeamStatic script on " + gameObject.name + "!");
    }

    private void BeamEnd()
    {
        OnBeamEnd.Invoke();
    }
    public void RemoveBeam() //This function removes the prefab with linerenderer
    {
        if (beam)
            Destroy(beam);
        if (beamStart)
            Destroy(beamStart);
        if (beamEnd)
            Destroy(beamEnd);
    }
}
}