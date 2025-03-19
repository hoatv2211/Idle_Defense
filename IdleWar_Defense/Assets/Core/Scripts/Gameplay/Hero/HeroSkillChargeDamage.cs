using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using UnityEngine;
using FoodZombie.UI;

public class HeroSkillChargeDamage : HeroSkill
{
    public ParticleMan effectCharge;
    public GameObject effectBeam;
    public float effectTime = 2f;

    //public override void Skill()
    //{
    //    if (!GameplayController.Instance.autoPlay)
    //    {
    //        switch (heroExControl.typeSkill)
    //        {
    //            case TypeSkill.CastLineHold:
    //                GameplayController.Instance.castSkillLineHold.SetInit(this);
    //                break;
    //            case TypeSkill.CastLineOne:
    //                GameplayController.Instance.castSkillLineOne.SetInit(this);
    //                currentPlaySkillTime = 5;
    //                break;
    //            default:

    //                break;
    //        }
    //    }
        
       
    //    base.Skill();
    //}

    public override void LineCastSkill(Vector3 _pos)
    {

        isCast = true;
        heroExControl.isSkill = true;
        heroExControl.LookAt(_pos);
        OnSkill();
        heroExControl.SetCountDown();
        //currentPlaySkillTime = effectTime;
    }

    public override void End()
    {
        base.End();
        heroExControl.isSkill = false;
        if (effectCharge != null) effectCharge.Stop();
        if (effectBeam != null) effectBeam.SetActive(false);
    }

    public override TrackEntry OnSkill()
    {
        if (effectCharge != null) effectCharge.Play();
        return base.OnSkill();
    }
    IEnumerator IEDOSkill;
    public override void TriggerSkill()
    {
        if (effectCharge != null) effectCharge.Stop();
        if (heroExControl.IsDead()) return;
        if (effectBeam != null) effectBeam.SetActive(true);
        
        if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);
        if (IEDOSkill != null)
            StopCoroutine(IEDOSkill);
        IEDOSkill = DoSkillDamage();
        StartCoroutine(IEDOSkill);

        StartCoroutine((IEEndEffect()));
    }
    IEnumerator DoSkillDamage()
    {
        if (heroExControl.IsDead())
        {
            yield break;
        }
        var skillValues = heroExControl.SkillValues;
        InfoAttacker infoAttacker = new InfoAttacker(true,
                                                        InfoAttacker.TYPE_SKILL,
                                                        heroExControl,
                                                        null,
                                                        null,
                                                        heroExControl.Damage * skillValues[0] / 100f,
                                                        heroExControl.CritRate,
                                                        heroExControl.CritDamage,
                                                        heroExControl.Accuracy,
                                                        skillValues[1]);
        //nếu enemies ở gần dưới 3 ô
        RaycastHit2D[] hits = Physics2D.CircleCastAll((Vector2)transform.position + heroExControl.Offset, 3f * Config.xRange, Vector2.zero);
        foreach (var item in hits)
        {
            if (item.collider.CompareTag(Config.TAG_ENEMY))
            {
                var enemyControl = item.collider.GetComponent<EnemyControl>();
                enemyControl.GetHit(infoAttacker);
                //Debug.LogError("hit " + enemyControl.gameObject.name);
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
            hits = Physics2D.BoxCastAll((Vector2)transform.position + offsetSkill + new Vector2(0f, sizeSkill.y / 2f), sizeSkill, 0f, Vector2.zero);

            foreach (var item in hits)
            {
                if (item.collider.CompareTag(Config.TAG_ENEMY))
                {
                    var enemyControl = item.collider.GetComponent<EnemyControl>();
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
        yield return new WaitForSeconds(0.5f);
        //   Debug.Log("shot new Skill");
        IEDOSkill = DoSkillDamage();
        StartCoroutine(IEDOSkill);

        
    }
    private IEnumerator IEEndEffect()
    {
        yield return new WaitForSeconds(effectTime);
        if (IEDOSkill != null)
        {
            StopCoroutine(IEDOSkill); IEDOSkill = null;
        }
        if (effectBeam != null) effectBeam.SetActive(false);
    }
}
