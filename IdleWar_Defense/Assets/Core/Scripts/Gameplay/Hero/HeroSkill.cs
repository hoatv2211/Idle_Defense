using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using UnityEngine;

public class HeroSkill : MonoBehaviour
{
    [HideInInspector]
    public HeroExControl heroExControl;
    protected float currentPlaySkillTime;

    protected bool isCast = false;

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

    public bool multiSkillTime = false;
    public float multiSkillTimeValue = 1;

    public virtual void Init(HeroExControl _heroExControl)
    {
        heroExControl = _heroExControl;
        currentPlaySkillTime = 0f;

        eventDataSkill = heroExControl.skeletonAnimation.Skeleton.Data.FindEvent(eventNameSkill);
        heroExControl.skeletonAnimation.AnimationState.Event += HandleAnimationStateEvent;

        maxGun = shots.Length;
        enabled = true;
    }

    public virtual void End()
    {
        enabled = false;
    }

    private void FixedUpdate()
    {
        currentPlaySkillTime -= Time.fixedDeltaTime;
        if (currentPlaySkillTime < 0f)
        {
            currentPlaySkillTime = 0f;
            if (heroExControl&&isCast&&!GameplayController.Instance.autoPlay)
            {
                heroExControl.isSkill = false;
                isCast = false;              
            }  
        }
    }

    public virtual void Skill()
    {
        heroExControl.AnimSkill();
    }

    public bool Skilling()
    {
        return currentPlaySkillTime > 0f;
    }

    public virtual TrackEntry OnSkill()
    {
        if (!soundStartSkill.Equals("")) SoundManager.Instance.PlaySFX(soundStartSkill);
        var animName = nameOfSkillAnimations[UnityEngine.Random.Range(0, nameOfSkillAnimations.Length)];
        var trackEntry = heroExControl.skeletonAnimation.AnimationState.SetAnimation(0, animName, false);
        trackEntry.Complete += AnimSkill_Complete;
        if (!multiSkillTime)
            currentPlaySkillTime = trackEntry.Animation.Duration + heroExControl.AttackTime / 6f; //thêm ít time nếu ko nó đánh luôn
        else
            currentPlaySkillTime = trackEntry.Animation.Duration * multiSkillTimeValue + heroExControl.AttackTime / 6f;
        return trackEntry;
    }

    private void AnimSkill_Complete(TrackEntry trackEntry)
    {
        SkillEnd();
        heroExControl.AnimIdle();
    }

    public virtual void SkillEnd()
    {

    }


    //-------------active event-----
    private void HandleAnimationStateEvent(TrackEntry trackEntry, Spine.Event e)
    {
        CheckEvent(e);
    }

    public void CheckEvent(Spine.Event e)
    {
        bool eventMatch =
            (eventDataSkill == e.Data); // Performance recommendation: Match cached reference instead of string.
        if (eventMatch)
        {
            TriggerSkill();
        }
    }

    public virtual void TriggerSkill()
    {
        if (heroExControl.IsDead()) return;

        if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);

        InfoAttacker infoAttacker = new InfoAttacker(true,
                                                        InfoAttacker.TYPE_SKILL,
                                                        heroExControl,
                                                        null,
                                                        null,
                                                        heroExControl.Damage,
                                                        heroExControl.CritRate,
                                                        heroExControl.CritDamage,
                                                        heroExControl.Accuracy,
                                                        100f);
        //nếu enemies ở gần dưới 3 ô
        RaycastHit2D[] hits = Physics2D.CircleCastAll((Vector2)transform.position + heroExControl.Offset, 3f * Config.xRange, Vector2.zero);
        foreach (var item in hits)
        {
            if (item.collider.CompareTag(Config.TAG_ENEMY))
            {
                var enemyControl = item.collider.GetComponent<EnemyControl>();
                enemyControl.GetHit(infoAttacker);

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
            hits = Physics2D.BoxCastAll((Vector2)transform.position + offsetSkill + new Vector2(0f, sizeSkill.y / 2f),
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
    }

    public virtual void AoeCastSkill(Vector3 _pos)
    {

    }

    public virtual void LineCastSkill(Vector3 _pos)
    {

    }

    public virtual void AimCastSkill(EnemyControl _enemy)
    {

    }

    public virtual void LookAt(Vector3 _pos)
    {
        heroExControl.LookAt(_pos);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3)offsetSkill + new Vector3(0f, sizeSkill.y / 2f, 0f),
            new Vector3(sizeSkill.x, sizeSkill.y, 0f));
    }

}