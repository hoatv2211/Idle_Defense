using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleMan : MonoBehaviour
{
    public ParticleSystem particleSystem;

    void OnValidate()
    {
        particleSystem = GetComponent<ParticleSystem>();
        if (particleSystem == null)
        {
            particleSystem = GetComponentInChildren<ParticleSystem>();
        }
    }

    public void Play(float delay, bool killAfterPlay = false)
    {
        StartCoroutine(IEPlay(delay, killAfterPlay));
    }

    private IEnumerator IEPlay(float delay, bool killAfterPlay)
    {
        yield return new WaitForSeconds(delay);
        Play();
        if (killAfterPlay)
        {
            yield return new WaitForSeconds(particleSystem.time + 1);
            SimplePool.Despawn(gameObject);
        }
    }

    public void Play()
    {
        /*if(!particleSystem.isPlaying)*/
        particleSystem.Play();
    }

    public void Stop()
    {
        if (particleSystem.isPlaying) particleSystem.Stop();
    }
}
