using System;
using DG.Tweening;
using Spine;
using UnityEngine;

namespace Core.Scripts.Gameplay.Enemy.Boss
{
    public class B2Control : EnemyExControl
    {
        public string jump1Animation, jump2Animation;
        public Vector3 lastPost;
        protected const int ANIM_SKILL_2_STAGE = 6;
        public const int ANIM_SKILL_3_STAGE = 7;
        public const int ANIM_JUMP_1_STAGE = 8;
        public const int ANIM_JUMP_2_STAGE = 9;

        public enum SKILL
        {
            SKILL_1,
            SKILL_2,
            SKILL_END,
            SKILL_3,
            NONE
        }

        public SKILL activeSkill;

        public override int STAGE
        {
            get { return stage; }
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
                                trackEntry = skeletonAnimation.AnimationState.SetAnimation(0,
                                    nameOfDeadAnimations[UnityEngine.Random.Range(0, nameOfDeadAnimations.Length)],
                                    false);
                                trackEntry.Complete += AnimDead_Complete;
                                stage = value;
                            }

                            break;
                        case ANIM_IDLE_STAGE:
                            if (stage != value && !attack.Attacking() && !IsSkilling())
                            {
                                if (stage == ANIM_GET_HIT_STAGE)
                                {
                                    skeletonAnimation.AnimationState.AddAnimation(0,
                                        nameOfIdleAnimations[UnityEngine.Random.Range(0, nameOfIdleAnimations.Length)],
                                        true, 0f);
                                }
                                else
                                {
                                    trackEntry = skeletonAnimation.AnimationState.SetAnimation(0,
                                        nameOfIdleAnimations[UnityEngine.Random.Range(0, nameOfIdleAnimations.Length)],
                                        true);
                                    if (stage != ANIM_ATTACK_STAGE) skeletonAnimation.Update(Mathf.Abs(random) / 1.8f);
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
                                    skeletonAnimation.AnimationState.AddAnimation(0,
                                        nameOfRunAnimations[UnityEngine.Random.Range(0, nameOfRunAnimations.Length)],
                                        true,
                                        0f);
                                }
                                else
                                {
                                    trackEntry = skeletonAnimation.AnimationState.SetAnimation(0,
                                        nameOfRunAnimations[UnityEngine.Random.Range(0, nameOfRunAnimations.Length)],
                                        true);
                                    if (stage != ANIM_ATTACK_STAGE) skeletonAnimation.Update(Mathf.Abs(random) / 1.8f);
                                    trackEntry.TimeScale = 1f + random / 4f;
                                    
                                    trackEntry.Complete += AnimRun_Complete; //hot fix cho GD
                                }

                                stage = value;
                            }

                            break;
                        case ANIM_ATTACK_STAGE:
                            if (!attack.Attacking() && !IsSkilling() &&
                                stage != ANIM_JUMP_1_STAGE && stage != ANIM_JUMP_2_STAGE)
                            {
                                attack.OnAttack();
                                skeletonAnimation.AnimationState.AddAnimation(0,
                                    nameOfIdleAnimations[UnityEngine.Random.Range(0, nameOfIdleAnimations.Length)],
                                    true,
                                    0f);
                                stage = value;
                            }

                            break;
                        case ANIM_GET_HIT_STAGE:
                            if (stage != value /*&& !attack.Attacking()*/)
                            {
                                skeletonAnimation.AnimationState.SetAnimation(0,
                                    nameOfGetHitAnimations[UnityEngine.Random.Range(0, nameOfGetHitAnimations.Length)],
                                    false);
                                stage = value;
                            }

                            break;
                        case ANIM_SKILL_STAGE:
                            if (!IsSkilling())
                            {
                                skills[lastIndexSkill].OnSkill();
                                // skeletonAnimation.AnimationState.AddAnimation(0,
                                //     nameOfIdleAnimations[UnityEngine.Random.Range(0, nameOfIdleAnimations.Length)], true,
                                //     0f);
                                stage = value;
                            }

                            break;
                        case ANIM_SKILL_2_STAGE:
                            if (!IsSkilling())
                            {
                                skills[lastIndexSkill].OnSkill();
                                stage = value;
                            }
                            break;
                        case ANIM_SKILL_3_STAGE:
                            if (!IsSkilling())
                            {
                                skills[lastIndexSkill].OnSkill();
                                stage = value;
                                Skill3Start();
                            }
                            break;
                        case ANIM_JUMP_1_STAGE:
                            if (stage != value)
                            {
                                stage = value;
                                trackEntry = skeletonAnimation.AnimationState.SetAnimation(0, jump1Animation, false);
                                trackEntry.Complete += Anim_Jump1_Complete;
                                Anim_Jump1_Start();
                            }

                            break;
                        case ANIM_JUMP_2_STAGE:
                            if (stage != value)
                            {
                                stage = value;
                                trackEntry = skeletonAnimation.AnimationState.SetAnimation(0, jump2Animation, false);
                                trackEntry.Complete += Anim_Jump2_Complete;
                                //Debug.Log("jump2");
                               // Invoke(nameof(BackToLastPosition), 0.5f);
                            }

                            break;
                    }
                }
            }
        }

        protected void Anim_Jump1_Start()
        {
            if (activeSkill == SKILL.SKILL_2)
            {
                Invoke(nameof(JumpToTarget),0.25f);
            }
            else if (activeSkill == SKILL.SKILL_END)
            {
                Invoke(nameof(BackToLastPosition),0.25f);
            }
        }

        void Skill3Start(){
            Invoke(nameof(JumpToTargetSkill3),0.25f);
        }
        void JumpToTargetSkill3()
        {
            var skill2 = skills[2] as B2Skill_3;
            skill2.JumpToTarget();
        }
        
        void JumpToTarget()
        {
            var skill2 = skills[1] as B2Skill_2;
            skill2.JumpToTarget();
        }
        protected void Anim_Jump1_Complete(TrackEntry trackEntry)
        {
            if (activeSkill == SKILL.SKILL_2||activeSkill == SKILL.SKILL_END)
            {
                STAGE = ANIM_JUMP_2_STAGE;
            }
            else
                STAGE = ANIM_IDLE_STAGE;
        }

        protected void Anim_Jump2_Complete(TrackEntry trackEntry)
        {
            if (activeSkill == SKILL.SKILL_2)
            {
                STAGE = ANIM_SKILL_2_STAGE;
            }
            else STAGE = ANIM_IDLE_STAGE;
        }

        
        private void BackToLastPosition()
        {
            autoMove.StopMove(0.25f);
            transform.DOMove(lastPost, 0.25f).SetEase(Ease.InSine);
        }
    }
}