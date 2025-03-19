using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using Spine;
using UnityEngine;

public class HeroSkillDamagePerSecond : HeroSkill
{
    public ParticleMan effectInEnemyPrefab;
    public ParticleMan effectCharge;
    public override TrackEntry OnSkill()
    {
        if (effectCharge != null) effectCharge.Play();

        return base.OnSkill() ;
    }


    public override void LineCastSkill(Vector3 _pos)
    {
        isCast = true;
        currentPlaySkillTime = 0.5f;
        heroExControl.isSkill = true;
        heroExControl.LookAt(_pos);
        OnSkill();
        heroExControl.SetCountDown();
    }

    public override void End()
    {
       
        base.End();
    }

    public override void TriggerSkill()
    {
        
        if (effectCharge != null) effectCharge.Stop();
        if (heroExControl.IsDead()) return;

        //if (effectBeam != null) effectBeam.SetActive(true);

        if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);

        var skillValues = heroExControl.SkillValues;
        var skillNerf = new SkillNerf(heroExControl.heroData.baseId, heroExControl,effectInEnemyPrefab, skillValues[0]);
        skillNerf.downMovementPercent = skillValues[3];
        skillNerf.damagePerSec = heroExControl.Damage * skillValues[2] / 100f; //bắn 3 phát hoả cầu về phía đối thủ, gây 150% đồng thời thiêu đốt 30% damage trong 3 giây.
        
        InfoAttacker infoAttacker = new InfoAttacker(true,
                                                        InfoAttacker.TYPE_SKILL, 
                                                        heroExControl, 
                                                        null,
                                                        skillNerf,
                                                        heroExControl.Damage * skillValues[1] / 100f,
                                                        heroExControl.CritRate,
                                                        heroExControl.CritDamage,
                                                        heroExControl.Accuracy,
                                                        100f);


        //nếu enemies ở gần dưới 3 ô
        RaycastHit2D[] hits = Physics2D.CircleCastAll((Vector2) transform.position + heroExControl.Offset, 3f * Config.xRange, Vector2.zero);
        foreach (var item in hits)
        {
            if (item.collider.CompareTag(Config.TAG_ENEMY))
            {
                var enemyControl = item.collider.GetComponent<EnemyControl>();
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

        heroExControl.isSkill = false;
    }
}
