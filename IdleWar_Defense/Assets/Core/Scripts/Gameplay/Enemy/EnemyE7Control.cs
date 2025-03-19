using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;
using Spine.Unity;
using Spine;
using System.Collections.Generic;
using System;
using FoodZombie;
using Utilities.Common;
using Debug = UnityEngine.Debug;

public class EnemyE7Control : EnemyExControl
{
    public override int STAGE
    {
        get
        {
            return stage;
        }
        set
        {
            TrackEntry trackEntry;
            if (stage != ANIM_DEAD_STAGE)
            {
                switch (value)
                {
                    case ANIM_DEAD_STAGE:
                        if (stage != value && !attack.Attacking())
                        {
                            trackEntry = attack.OnAttack();
                            trackEntry.Complete += AnimDead_Complete;
                            stage = ANIM_DEAD_STAGE;
                        }
                        break;
                    case ANIM_IDLE_STAGE:
                        if (stage != value && !attack.Attacking())
                        {
                            if (stage == ANIM_GET_HIT_STAGE)
                            {
                                skeletonAnimation.AnimationState.AddAnimation(0, nameOfIdleAnimations[UnityEngine.Random.Range(0, nameOfIdleAnimations.Length)], true, 0f);
                            }
                            else
                            {
                                trackEntry = skeletonAnimation.AnimationState.SetAnimation(0, nameOfIdleAnimations[UnityEngine.Random.Range(0, nameOfIdleAnimations.Length)], true);
                                if(stage != ANIM_ATTACK_STAGE) skeletonAnimation.Update(Mathf.Abs(random) / 1.8f);
                                trackEntry.TimeScale = 1f + random / 4f;
                            }
                            stage = value;
                        }
                        break;
                    case ANIM_RUN_STAGE:
                        if (stage != value && !attack.Attacking())
                        {
                            OnRun();
                            if (stage == ANIM_GET_HIT_STAGE)
                            {
                                skeletonAnimation.AnimationState.AddAnimation(0, nameOfRunAnimations[UnityEngine.Random.Range(0, nameOfRunAnimations.Length)], true, 0f);
                            }
                            else
                            {
                                trackEntry = skeletonAnimation.AnimationState.SetAnimation(0, nameOfRunAnimations[UnityEngine.Random.Range(0, nameOfRunAnimations.Length)], true);
                                if (stage != ANIM_ATTACK_STAGE) skeletonAnimation.Update(Mathf.Abs(random) / 1.8f);
                                trackEntry.TimeScale = 1f + random / 4f;
                                
                                // trackEntry.Complete += AnimRun_Complete; //hot fix cho GD
                            }
                            stage = value;
                        }
                        break;
                    case ANIM_ATTACK_STAGE:
                        if (!attack.Attacking())
                        {
                            OnDead();
                            trackEntry = attack.OnAttack();
                            trackEntry.Complete += AnimDead_Complete;
                            stage = ANIM_DEAD_STAGE;
                        }
                        break;
                    case ANIM_GET_HIT_STAGE:
                        if (stage != value /*&& !attack.Attacking()*/)
                        {
                            skeletonAnimation.AnimationState.SetAnimation(0, nameOfGetHitAnimations[UnityEngine.Random.Range(0, nameOfGetHitAnimations.Length)], false);
                            stage = value;
                        }
                        break;
                }
            }
        }
    }
}