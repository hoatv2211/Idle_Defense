using Spine;
using UnityEngine;

namespace Core.Scripts.Gameplay.Enemy.MiniBoss
{
    public class MB1Control : EnemyExControl
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
                        if (stage != value)
                        {
                            trackEntry = skeletonAnimation.AnimationState.SetAnimation(0, nameOfDeadAnimations[UnityEngine.Random.Range(0, nameOfDeadAnimations.Length)], false);
                            trackEntry.Complete += AnimDead_Complete;
                            stage = value;
                        }
                        break;
                    case ANIM_IDLE_STAGE:
                        if (stage != value && !attack.Attacking() && !IsSkilling())
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
                        if (stage != value && !attack.Attacking() && !IsSkilling())
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
                                
                                trackEntry.Complete += AnimRun_Complete; //hot fix cho GD
                            }
                            stage = value;
                        }
                        break;
                    case ANIM_ATTACK_STAGE:
                        if (!attack.Attacking() && !IsSkilling())
                        {
                            attack.OnAttack();
                            skeletonAnimation.AnimationState.AddAnimation(0, nameOfIdleAnimations[UnityEngine.Random.Range(0, nameOfIdleAnimations.Length)], true, 0f);
                            stage = value;
                        }
                        break;
                    case ANIM_GET_HIT_STAGE:
                        if (stage != value /*&& !attack.Attacking()*/)
                        {
                            skeletonAnimation.AnimationState.SetAnimation(0, nameOfGetHitAnimations[UnityEngine.Random.Range(0, nameOfGetHitAnimations.Length)], false);
                            stage = value;
                        }
                        break;
                    case ANIM_SKILL_STAGE:
                        if (!IsSkilling())
                        {
                            skills[lastIndexSkill].OnSkill();
                            skeletonAnimation.AnimationState.AddAnimation(0, nameOfIdleAnimations[UnityEngine.Random.Range(0, nameOfIdleAnimations.Length)], true, 0f);
                            stage = value;
                        }
                        break;
                }
            }
        }
    }

        public override void CheckSkills()
        {
            var mb1Move = autoMove as MB1Move;
            if (/*!mb1Move.IsStopMoveStraight()||*/mb1Move.isMoving) return;
            if(attack.Attacking()) return;
            mb1Move.MoveLeftOrRight(transform.position.x>0, ()=> { STAGE = ANIM_ATTACK_STAGE; });
        }
    }
}