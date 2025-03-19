using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDamageDelay : Bullet
{
    // bullet này đứng yên, phát nổ sau 1 khoảng thời gian
    public float delayDuration = 1.5f;
    public bool startCount;
    private float countTime = 0f;
    public ParticleMan explode;
    public bool DestroyAfterDelay = true;
    public float delayDestroy = 0f;
    public void Init(InfoAttacker _infoAttacker)
    {
        base.Init(_infoAttacker);
        countTime = 0;
        startCount = false;
    }
    public override void UpdateMove()
    {
        if (shooting == false)
        {
            return;
        }
        if(startCount)
            countTime += Time.deltaTime;
        if(countTime>delayDuration)
            CheckHits();
    }

    private ParticleMan _particleMan;
    private Vector3 pos;
    protected override void CheckHits()
    {
        base.CheckHits();
       
        if (DestroyAfterDelay)
        {
            if (explode != null)
            {
                if(_particleMan==null)
                    _particleMan = Instantiate(explode,transform.parent);
                _particleMan.transform.localPosition = pos;
                _particleMan.Play();
            }

            Invoke(nameof(DestroyBullet),delayDestroy);
        }
    }
    public void StartCountDown()
    {
        startCount = true;
        pos = transform.localPosition;
        
    }

    private void DestroyBullet()
    {
        ReleaseBullet();
    }
}
