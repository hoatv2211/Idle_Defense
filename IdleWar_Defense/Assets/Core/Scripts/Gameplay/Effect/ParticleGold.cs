using System;
using UnityEngine;
using System.Collections;
using FoodZombie;
using FoodZombie.UI;

public class ParticleGold : MonoBehaviour {
    [SerializeField] private ParticleSystem system;
    private float waitTime = 0.35f; //time chờ để tween các đồng xu vào target

    private static ParticleSystem.Particle[] particles = new ParticleSystem.Particle[1000];
    private int count;
    private Vector2 target;

    private bool playingSound = false;
    
    public void Play(float delay)
    {
        StartCoroutine(IEPlay(delay));
    }

    private IEnumerator IEPlay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Play();
    }

	void FixedUpdate(){
        waitTime -= Time.fixedDeltaTime;

        if (waitTime <= 0)
        {
            target = MainGamePanel.instance.HubPanel.GetCoinIconPos();

            system.Stop();
            system.gravityModifier = 0f;
            system.startSpeed = 0f;
            count = system.GetParticles(particles);

            for (int i = 0; i < count; i++)
            {
                ParticleSystem.Particle particle = particles[i];

                Vector3 v1 = particle.position;
                Vector3 v2 = new Vector3(target.x, target.y, v1.z);

                particle.position = Vector3.MoveTowards(v1, v2, 0.25f);
                particles[i] = particle;
            }

            system.SetParticles(particles, count);

            //if (waitTime <= -0.6f && !playingSound)
            //{
            //    StartCoroutine(IEPlaySound());
            //    playingSound = true;
            //}
        }
	}

    public void Play()
    {
        system.Stop();
        waitTime = 0.35f;
        playingSound = false;
        
        system.startSpeed = Config.EasyRandom(5f, 7f);
        system.Play();
    }

    private IEnumerator IEPlaySound()
    {
        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(Config.EasyRandom(0f, 0.5f));
            SoundManager.Instance.PlaySFX(IDs.SOUND_COIN);
        }
    }
}