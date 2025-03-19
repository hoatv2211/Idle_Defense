using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impact : MonoBehaviour
{
    public float liveTime = 2f;
    public ParticleMan particleMan;

    private float delay = 0f;

    public void Init(float _delay)
    {
        delay = _delay;
    }
    
    private void OnEnable()
    {
        particleMan.Play(delay);
        StartCoroutine(IERelease());
    }

    private IEnumerator IERelease()
    {
        yield return new WaitForSeconds(liveTime);
        GameplayController.Instance.ReleaseImpact(this);
    }
}
