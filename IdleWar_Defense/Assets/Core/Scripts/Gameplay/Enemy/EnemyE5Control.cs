using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;
using Utilities.Common;

public class EnemyE5Control : EnemyExControl
{
    public SkeletonAnimation[] skeletonAnimationClones;
    public ParticleMan[] effectDieClones;

    private int countClones;
    private int maxClones;
    private float distanceHP; //khoảng cách HP để tắt 1 clone
    private List<SkeletonAnimation> listCloneDies;
    private int indexCloneDie = 0;
    
    public override int STAGE
    {
        get
        {
            return stage;
        }
        set
        {
            TrackEntry trackEntry;
            string animName;
            if (stage != ANIM_DEAD_STAGE)
            {
                switch (value)
                {
                    case ANIM_DEAD_STAGE:
                        if (stage != value)
                        {
                            animName = nameOfDeadAnimations[UnityEngine.Random.Range(0, nameOfDeadAnimations.Length)];
                            trackEntry = skeletonAnimation.AnimationState.SetAnimation(0, animName, false);
                            trackEntry.Complete += AnimDead_Complete;

                            for (int i = 0; i < countClones; i++)
                            {
                                skeletonAnimationClones[i].AnimationState.SetAnimation(0, animName, false);
                            }
                            
                            stage = value;
                        }
                        break;
                    case ANIM_IDLE_STAGE:
                        if (stage != value && !attack.Attacking())
                        {
                            animName = nameOfIdleAnimations[UnityEngine.Random.Range(0, nameOfIdleAnimations.Length)];
                            if (stage == ANIM_GET_HIT_STAGE)
                            {
                                skeletonAnimation.AnimationState.AddAnimation(0, animName, true, 0f);
                                
                                for (int i = 0; i < countClones; i++)
                                {
                                    skeletonAnimationClones[i].AnimationState.AddAnimation(0, animName, true, 0f);
                                }
                            }
                            else
                            {
                                trackEntry = skeletonAnimation.AnimationState.SetAnimation(0, animName, true);
                                if(stage != ANIM_ATTACK_STAGE) skeletonAnimation.Update(Mathf.Abs(random) / 1.8f);
                                trackEntry.TimeScale = 1f + random / 4f;
                                
                                for (int i = 0; i < countClones; i++)
                                {
                                    skeletonAnimationClones[i].AnimationState.SetAnimation(0, animName, true);
                                    if(stage != ANIM_ATTACK_STAGE) skeletonAnimationClones[i].Update(Mathf.Abs(random) / 1.8f);
                                }
                            }
                            stage = value;
                        }
                        break;
                    case ANIM_RUN_STAGE:
                        if (stage != value && !attack.Attacking())
                        {
                            OnRun();
                            animName = nameOfRunAnimations[UnityEngine.Random.Range(0, nameOfRunAnimations.Length)];
                            if (stage == ANIM_GET_HIT_STAGE)
                            {
                                skeletonAnimation.AnimationState.AddAnimation(0, animName, true, 0f);
                                
                                for (int i = 0; i < countClones; i++)
                                {
                                    skeletonAnimationClones[i].AnimationState.AddAnimation(0, animName, true, 0f);
                                }
                            }
                            else
                            {
                                trackEntry = skeletonAnimation.AnimationState.SetAnimation(0, animName, true);
                                if (stage != ANIM_ATTACK_STAGE) skeletonAnimation.Update(Mathf.Abs(random) / 1.8f);
                                trackEntry.TimeScale = 1f + random / 4f;
                                
                                for (int i = 0; i < countClones; i++)
                                {
                                    skeletonAnimationClones[i].AnimationState.SetAnimation(0, animName, true);
                                    if (stage != ANIM_ATTACK_STAGE) skeletonAnimationClones[i].Update(Mathf.Abs(random) / 1.8f);
                                }
                                
                                // trackEntry.Complete += AnimRun_Complete; //hot fix cho GD
                            }
                            stage = value;
                        }
                        break;
                    case ANIM_ATTACK_STAGE:
                        if (!attack.Attacking())
                        {
                            attack.OnAttack();
                            animName = nameOfIdleAnimations[UnityEngine.Random.Range(0, nameOfIdleAnimations.Length)];
                            skeletonAnimation.AnimationState.AddAnimation(0, animName, true, 0f);
                            
                            for (int i = 0; i < countClones; i++)
                            {
                                skeletonAnimationClones[i].AnimationState.AddAnimation(0, animName, true, 0f);
                            }
                            stage = value;
                        }
                        break;
                    case ANIM_GET_HIT_STAGE:
                        if (stage != value /*&& !attack.Attacking()*/)
                        {
                            animName = nameOfGetHitAnimations[UnityEngine.Random.Range(0, nameOfGetHitAnimations.Length)];
                            skeletonAnimation.AnimationState.SetAnimation(0, animName, false);
                            
                            for (int i = 0; i < countClones; i++)
                            {
                                skeletonAnimationClones[i].AnimationState.SetAnimation(0, animName, false);
                            }
                            stage = value;
                        }
                        break;
                }
            }
        }
    }
    
    public override void Refresh()
    {
        if(listCloneDies != null) listCloneDies.Clear();
        else listCloneDies = new List<SkeletonAnimation>();
        indexCloneDie = 0;
        
        maxClones = skeletonAnimationClones.Length;
        countClones = maxClones;
        distanceHP = HP_MAX / (maxClones + 1); //tính cả con origin nguyên bản
        for (int i = 0; i < countClones; i++)
        {
            var skeletonAnimationClone = skeletonAnimationClones[i];
            skeletonAnimationClone.SetActive(true);
            skeletonAnimationClone.Initialize(false);
            skeletonAnimationClone.skeleton.A = 1f;
            skeletonAnimationClone.skeleton.SetColor(Color.white);
        }
        
        base.Refresh();
    }
    
    public override void OnDead()
    {
        base.OnDead();
        
        for (int i = 0; i < countClones; i++)
        {
            var effectDieClone = effectDieClones[i];
            effectDieClone.Play(delayTimeEffectDie);
        }
    }
    
    protected override IEnumerator IEDisapear()
    {
        float timePlay = 1.0f;
        while (timePlay >= 0f)
        {
            yield return null;
            timePlay -= Time.deltaTime;
            skeletonAnimation.skeleton.A = timePlay;
            
            for (int i = 0; i < countClones; i++)
            {
                var skeletonAnimationClone = skeletonAnimationClones[i];
                skeletonAnimationClone.skeleton.A = timePlay;
            }
        }
        
        gameObject.SetActive(false);
    }
    
    protected override IEnumerator IEStartInvisible()
    {
        float timeMax = 1f;
        float timePlay = timeMax;
        while (timePlay >= 0f)
        {
            yield return null;
            timePlay -= Time.deltaTime;
            var a = 0.4f + (timePlay / timeMax) * 0.6f;
            if (a < 0.4f) a = 0.4f;
            skeletonAnimation.skeleton.A = a; // chỉ fade một nửa
            
            for (int i = 0; i < countClones; i++)
            {
                var skeletonAnimationClone = skeletonAnimationClones[i];
                skeletonAnimationClone.skeleton.A = a;
            }
        }
    }
    
    protected override IEnumerator IEEndInvisible()
    {
        float timeMax = 0.45f;
        float timePlay = timeMax;
        while (timePlay >= 0f)
        {
            yield return null;
            timePlay -= Time.deltaTime;
            var a = 0.4f + (1f - (timePlay / timeMax)) * 0.6f;
            if (a > 1f) a = 1f;
            skeletonAnimation.skeleton.A = a; // chỉ fade một nửa
            
            for (int i = 0; i < countClones; i++)
            {
                var skeletonAnimationClone = skeletonAnimationClones[i];
                skeletonAnimationClone.skeleton.A = a;
            }
        }
    }
    
    protected override IEnumerator IEShowRedColor()
    {
        float timeMax = 0.5f;
        float timePlay = timeMax;
        while (timePlay >= 0f)
        {
            yield return null;
            timePlay -= Time.deltaTime;
            var c = Color.Lerp(Color.white, Color.red, (timePlay / timeMax));
            skeletonAnimation.skeleton.SetColor(c);
            
            for (int i = 0; i < countClones; i++)
            {
                var skeletonAnimationClone = skeletonAnimationClones[i];
                skeletonAnimationClone.skeleton.SetColor(c);
            }
        }
    }
    
    public override void GetHit(InfoAttacker infoAttacker)
    {
        base.GetHit(infoAttacker);

        countClones = (int) (HP / distanceHP) - 1;
        if (countClones < 0) countClones = 0;
        for (int i = countClones; i < maxClones; i++)
        {
            var skeletonAnimationClone = skeletonAnimationClones[i];
            //nếu clone chưa có trong list die thì cho die và add vào list
            if (!listCloneDies.Contains(skeletonAnimationClone))
            {
                listCloneDies.Add(skeletonAnimationClone);
                
                var animName = nameOfDeadAnimations[UnityEngine.Random.Range(0, nameOfDeadAnimations.Length)];
                var trackEntry = skeletonAnimationClone.AnimationState.SetAnimation(0, animName, false);
                trackEntry.Complete += AnimDeadClone_Complete;
                
                var effectDieClone = effectDieClones[i];
                effectDieClone.Play(delayTimeEffectDie);
            }
        }
    }
    
    protected void AnimDeadClone_Complete(TrackEntry trackEntry)
    {
        if(listCloneDies.Count > 0) StartCoroutine(IECloneDisapear());
    }
    
    //lấy clone đầu tiên trong list die ra cho fade và disable
    protected IEnumerator IECloneDisapear()
    {
        var skeletonAnimationClone = listCloneDies[indexCloneDie];
        indexCloneDie++;
        float timePlay = 1.0f;
        while (timePlay >= 0f)
        {
            yield return null;
            timePlay -= Time.deltaTime;
            skeletonAnimationClone.skeleton.A = timePlay;
        }
        
        skeletonAnimationClone.SetActive(false);
    }
}
