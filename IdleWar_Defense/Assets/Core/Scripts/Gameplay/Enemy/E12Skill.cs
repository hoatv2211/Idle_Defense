using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using Spine;
using UnityEngine;
using Event = Spine.Event;

public class E12Skill : EnemySkill
{
    public float moveSpeedMultiple;
    public float yCoordinate;
    private bool _active;
    public string endSkillEventName;
    public EventData eventDataEndSkill;
    public override void Init(EnemyExControl _enemyExControl)
    {
        base.Init(_enemyExControl);
        eventDataEndSkill = enemyExControl.skeletonAnimation.Skeleton.Data.FindEvent(endSkillEventName);
        
    }

    public override void Skill()
    {
        enemyExControl.AnimSkill();
    }

    public override bool CanSkill()
    {
        if (transform.position.y <= yCoordinate && !_active)
        {
            animName = nameOfSkillAnimations[UnityEngine.Random.Range(0, nameOfSkillAnimations.Length)];
            var duration = enemyExControl.skeletonAnimation.skeleton.Data.FindAnimation(animName).Duration;
            skillSpeed = duration + enemyExControl.AttackTime / 6f; //mỗi lần attack cần gán lại vì còn theo boost
            return true;
        }

        return false;
    }

    public override TrackEntry OnSkill()
    {
        enemyExControl.StopMove(1f);
        enemyExControl.autoMove.forceMoveStraight = true;
        return base.OnSkill();
    }

    public override void TriggerSkill()
    {
        _active = true;
        enemyExControl.moveSpeedMultiple = moveSpeedMultiple*10/100f;
    }

    public override void SkillEnd()
    {
        enemyExControl.autoMove.forceMoveStraight = false;
        enemyExControl.moveSpeedMultiple = 1;
        if (enemyExControl.IsDead()) return;

        if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);

        InfoAttacker infoAttacker = new InfoAttacker(false,
                                                        InfoAttacker.TYPE_SKILL,                                             
                                                        null,
                                                        enemyExControl,
                                                        null,
                                                        enemyExControl.Damage,
                                                        enemyExControl.CritRate,
                                                        enemyExControl.CritDamage,
                                                        enemyExControl.Accuracy,
                                                        100f);
        //nếu enemies ở gần dưới 3 ô
        RaycastHit2D[] hits = Physics2D.CircleCastAll((Vector2) transform.position + enemyExControl.Offset, 3f * Config.xRange, Vector2.zero);
        foreach (var item in hits)
        {
            if (item.collider.CompareTag(Config.TAG_HERO))
            {
                var heroControl = item.collider.GetComponent<HeroControl>();
                heroControl.GetHit(infoAttacker);
                
                if (maxGun > 0)
                {
                    //đánh range thì chỉ gọi muzzles không cần gọi bullet
                    if (muzzles != null && muzzles.Length > 0)
                    {
                        muzzles[countGun].Play();
                    }

                    countGun++;
                    if (countGun >= maxGun) countGun = 0;
                }
            }
        }
        //
        if (maxGun > 0)
        {
            //đánh range thì spawn bullet
            if (muzzles != null && muzzles.Length > 0)
            {
                muzzles[countGun].Play();
            }
            
            shots[countGun].Shot(infoAttacker);

            countGun++;
            if (countGun >= maxGun) countGun = 0;
        }
        else
        {
            //đánh melee thì check ray cast
            hits = CheckHits();

            foreach (var item in hits)
            {
                if (item.collider.CompareTag(Config.TAG_HERO))
                {
                    var heroControl = item.collider.GetComponent<HeroControl>();
                    heroControl.GetHit(infoAttacker);
                }
            }
        }
    }

    public override void CheckEvent(Event e)
    {
        base.CheckEvent(e);
        var b = (eventDataEndSkill == e.Data);
        if (b)
        {
            SkillEnd();
        }
    }
}
