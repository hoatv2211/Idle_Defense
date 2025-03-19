using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using UnityEngine;
using Utilities.Common;
using FoodZombie.UI;

public class HeroSkillChargeDamageSlowMovement : HeroSkill
{
    public ParticleMan effectSkill;
    public ParticleMan effectCharge;
    public GameObject[] effectBeams;
    public ParticleMan effectInEnemyPrefab;

    public bool isStun = false;
    public float effectTime = 2f;
    public float freezePercentByRus = 40f; //hot fix Những enemy dính đạn sẽ có 40% khả năng bị đóng băng vĩnh viễn ( không tác dụng với boss và mini boss )

    public override TrackEntry OnSkill()
    {
        if (effectCharge != null) effectCharge.Play();

        return base.OnSkill();
    }


    public override void LineCastSkill(Vector3 _pos)
    {
        heroExControl.isSkill = true;
        isCast = true;
        heroExControl.LookAt(_pos);
        OnSkill();
        heroExControl.SetCountDown();
        currentPlaySkillTime += effectTime;
    }


    public override void End()
    {  
        base.End();
        if (effectCharge != null) effectCharge.Stop();
        if (effectBeams != null)
        {
            var count = effectBeams.Length;
            for (int i = 0; i < count; i++)
            {
                effectBeams[i].SetActive(false);
            }
        }
    }


    public override void TriggerSkill()
    {
        if (effectCharge != null) effectCharge.Stop();
        if (heroExControl.IsDead()) return;
        

        if (effectBeams != null)
        {
            var count = effectBeams.Length;
            for (int i = 0; i < count; i++)
            {
                effectBeams[i].SetActive(true);
            }
        }
        if (effectSkill != null) effectSkill.Play();

        if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);
        var skillValues = heroExControl.SkillValues;
        
        var skillNerf = new SkillNerf(heroExControl.heroData.baseId, heroExControl, effectInEnemyPrefab, skillValues[0]);
        skillNerf.downMovementPercent = skillValues[3];
        skillNerf.stun = isStun;
        skillNerf.stunPercent = freezePercentByRus;
        InfoAttacker infoAttacker = new InfoAttacker(true,
                                                        InfoAttacker.TYPE_SKILL,
                                                        heroExControl,
                                                        null,
                                                        skillNerf,
                                                        heroExControl.Damage,
                                                        skillValues[1],
                                                        skillValues[2],
                                                        heroExControl.Accuracy);
            
        //nếu enemies ở gần dưới 3 ô
        RaycastHit2D[] hits = Physics2D.CircleCastAll((Vector2) transform.position + heroExControl.Offset, 3f * Config.xRange, Vector2.zero);
        foreach (var item in hits)
        {
            if (item.collider.CompareTag(Config.TAG_ENEMY))
            {
                var enemyControl = item.collider.GetComponent<EnemyExControl>();
                enemyControl.GetHit(infoAttacker);
            }
        }

        //
        if (shots != null && shots.Length > 0)
        {
            //đánh range thì spawn bullet
            shots[countGun].Shot(infoAttacker);
        }
        else
        {
            //đánh melee thì check ray cast
            hits = Physics2D.BoxCastAll((Vector2) transform.position + offsetSkill + new Vector2(0f, sizeSkill.y / 2f),
                sizeSkill, 0f, Vector2.zero);

            foreach (var item in hits)
            {
                if (item.collider.CompareTag(Config.TAG_ENEMY))
                {
                    var enemyControl = item.collider.GetComponent<EnemyExControl>();
                    enemyControl.GetHit(infoAttacker);
                }
            }
        }

        if (shots != null && shots.Length > 0)
        {
            //đánh range thì spawn bullet
            if (muzzles != null && muzzles.Length > 0)
            {
                muzzles[countGun].Play();
            }

            countGun++;
            if (countGun >= shots.Length) countGun = 0;
        }

        StartCoroutine((IEEndEffect()));
        
    }

    private IEnumerator IEEndEffect()
    {
        yield return new WaitForSeconds(effectTime);
        
        if (effectBeams != null)
        {
            var count = effectBeams.Length;
            for (int i = 0; i < count; i++)
            {
                effectBeams[i].SetActive(false);
            }
        }
        if (effectSkill != null) effectSkill.Stop();
    }
}
