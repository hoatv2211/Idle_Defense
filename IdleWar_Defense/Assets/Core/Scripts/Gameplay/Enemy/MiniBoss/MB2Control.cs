using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Spine;
using UnityEngine;

public class MB2Control : EnemyExControl
{
    public bool moveBeforeSkill = true;
    public override void CheckSkills()
    {
        var count = skills.Count;
        for (int i = 0; i < count; i++)
        {
            //check lần lượt skill, cái nào có thể chạy thì break vòng lặp ko thì next
            lastIndexSkill++;
            if (lastIndexSkill >= count) lastIndexSkill = 0;
            
            var mb2Skill = skills[lastIndexSkill] as MB2Skill;
            if (mb2Skill.CanSkill())
            {
                // move left/right xong rồi mới skill
                Debug.Log("vao can skill");
                moveBeforeSkill = true;
                var mb2Move = autoMove as MB2Move;
                if (!mb2Move.isMoving)
                {
                    mb2Move.MoveLeftOrRight(transform.position.x>0, () =>
                    {
                        Debug.Log("vao skill");
                        mb2Skill.Skill();
                    
                        cooldown_MAX = enemyData.Cooldown;
                        cooldown = cooldown_MAX;
                    });
                }
                
                return;
            }
        }
    }

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
                        Debug.Log("moveBeforeSkill= "+(moveBeforeSkill));
                        if (!attack.Attacking() && !IsSkilling() || !moveBeforeSkill)
                        {
                            attack.OnAttack();
                            //skeletonAnimation.AnimationState.AddAnimation(0, nameOfIdleAnimations[UnityEngine.Random.Range(0, nameOfIdleAnimations.Length)], true, 0f);
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
                            Debug.Log("co vao Skilling");
                            skills[lastIndexSkill].OnSkill();
                            //skeletonAnimation.AnimationState.AddAnimation(0, nameOfIdleAnimations[UnityEngine.Random.Range(0, nameOfIdleAnimations.Length)], true, 0f);
                            stage = value;
                        }
                        break;
                }
            }
        }
    }
}
