using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using UnityEngine;

public class EnemySkill : MonoBehaviour
{
    protected EnemyExControl enemyExControl;
    protected float skillSpeed = 0.75f;
    protected float currentPlaySkillTime;
    protected float threshold;
    protected string animName;

    public string[] nameOfSkillAnimations;
    public string eventNameSkill;
    public Spine.EventData eventDataSkill;

    public Vector2 offsetSkill;
    public Vector2 sizeSkill;
    
    protected int countGun = 0;
    protected int maxGun;
    public BaseShot[] shots;
    public ParticleMan[] muzzles;

    public string soundStartSkill;
    public string soundTriggerSkill;

    public virtual void Init(EnemyExControl _enemyExControl)
    {
        enemyExControl = _enemyExControl;
        currentPlaySkillTime = 0f;
        threshold = enemyExControl.random * Config.enemyMoveThreshold;

        eventDataSkill = enemyExControl.skeletonAnimation.Skeleton.Data.FindEvent(eventNameSkill);
        enemyExControl.skeletonAnimation.AnimationState.Event += HandleAnimationStateEvent;

        maxGun = shots.Length;
        enabled = true;
    }

    public virtual void End()
    {
        enabled = false;
    }

    public virtual void FixedUpdate()
    {
        currentPlaySkillTime -= Time.fixedDeltaTime;
        if (currentPlaySkillTime < 0f) currentPlaySkillTime = 0f;
    }

    public virtual void Skill()
    {
        //sau này muốn vừa đi vừa bắn thì tắt đi
        enemyExControl.StopMove(skillSpeed);
        enemyExControl.AnimSkill();
    }

    public bool Skilling()
    {
        return currentPlaySkillTime > 0f;
    }

    public virtual TrackEntry OnSkill()
    {
        enemyExControl.SetInvisible(false);

        if (!soundStartSkill.Equals("")) SoundManager.Instance.PlaySFX(soundStartSkill);

        currentPlaySkillTime = skillSpeed;
        
        var trackEntry = enemyExControl.skeletonAnimation.AnimationState.SetAnimation(0, animName, false);
        Debug.Log("animation name = "+animName);
        trackEntry.Complete += AnimSkill_Complete;
        return trackEntry;
    }

    private void AnimSkill_Complete(TrackEntry trackEntry)
    {
        SkillEnd();
        enemyExControl.AnimIdle();
    }

    public virtual void SkillEnd()
    {

    }
    
    public virtual RaycastHit2D[] CheckHits()
    {
        var size = sizeSkill + new Vector2(threshold / 2f, threshold);
        return Physics2D.BoxCastAll((Vector2) transform.position + offsetSkill - new Vector2(0f, size.y / 2f), size, 0f, Vector2.zero);
    }
    
    public virtual bool CanSkill()
    {
        //nếu trong vùng skill mà có hero thì cast thôi
        var hits = CheckHits();

        foreach (var item in hits)
        {
            if (item.collider.CompareTag(Config.TAG_HERO))
            {
                //đoạn này hơi sida, thôi kệ vì code base phải chuẩn
                //đoạn này lấy ra time cần để stop move -> gán vào biến skillSpeed, sau dùng ở hàm OnSkill
                animName = nameOfSkillAnimations[UnityEngine.Random.Range(0, nameOfSkillAnimations.Length)];
                var duration = enemyExControl.skeletonAnimation.skeleton.Data.FindAnimation(animName).Duration;
                skillSpeed = duration + enemyExControl.AttackTime / 6f; //mỗi lần attack cần gán lại vì còn theo boost
                return true;
            }
        }

        return false;
    }
    
    //-------------active event-----
    public virtual void HandleAnimationStateEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if(enemyExControl.lastIndexSkill == -1) return;
        if(enemyExControl.skills[enemyExControl.lastIndexSkill]!=this) return;
        CheckEvent(e);
    }

    public virtual void CheckEvent(Spine.Event e)
    {
        bool eventMatch = (eventDataSkill == e.Data); // Performance recommendation: Match cached reference instead of string.
        if (eventMatch)
        {
            TriggerSkill();
        }
    }

    public virtual void TriggerSkill()
    {
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3) offsetSkill - new Vector3(0f, sizeSkill.y / 2f, 0f), new Vector3(sizeSkill.x, sizeSkill.y, 0f));
    }

}