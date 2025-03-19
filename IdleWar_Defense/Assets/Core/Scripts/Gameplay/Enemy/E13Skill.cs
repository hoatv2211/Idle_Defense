using System;
using System.Linq;
using FoodZombie;
using Spine;
using UnityEngine;
using Event = Spine.Event;

public class E13Skill : EnemySkill
{
    public string skillJumpEventName;
    public EventData eventDataJumpSkill;

    public override void Init(EnemyExControl _enemyExControl)
    {
        base.Init(_enemyExControl);
        eventDataJumpSkill = enemyExControl.skeletonAnimation.Skeleton.Data.FindEvent(skillJumpEventName);
        
    }
    
    public override bool CanSkill()
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
                skillSpeed = duration; //mỗi lần attack cần gán lại vì còn theo boost
                return true;
            }
        }

        return false;
    }
    public override void HandleAnimationStateEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if(enemyExControl.lastIndexSkill == -1) return;
        if(enemyExControl.skills[enemyExControl.lastIndexSkill]!=this) return;
        if(nameOfSkillAnimations.Contains(trackEntry.Animation.Name))
            CheckEvent(e);
    }
    public override void CheckEvent(Event e)
    {

        base.CheckEvent(e);
        var b = (eventDataJumpSkill == e.Data);
        if (b)
        {
            JumpToTarget();
        }
    }
    
    public override void TriggerSkill()
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
        
        //đánh melee thì check ray cast
        RaycastHit2D[] hits = CheckHits();
        foreach (var item in hits)
        {
            if (item.collider.CompareTag(Config.TAG_HERO))
            {
                var heroControl = item.collider.GetComponent<HeroControl>();
                // if (heroControl == enemyExControl.target)
                // {
                heroControl.GetHit(infoAttacker);
                return;
                // }
            }
        }
    }

    private void JumpToTarget()
    {
        if(enemyExControl.target==null) return;
        if(enemyExControl.target.IsDead()) return;
        
        enemyExControl.StopMove(1f);
        var control = enemyExControl as Enemy13Control;
        control.lastPost = transform.position;
        transform.position = enemyExControl.target.transform.position + new Vector3(0, Config.xRange, 0f);
    }
    public override void SkillEnd()
    {
        enemyExControl.autoMove.StopMove(enemyExControl.enemyData.Cooldown);
        Invoke(nameof(JumpBack),1f);
    }

    private void JumpBack()
    {
        var control = enemyExControl as Enemy13Control;
        control.STAGE = Enemy13Control.ANIM_JUMP_1_STAGE;
        enemyExControl.autoMove.StopMove(enemyExControl.enemyData.Cooldown);
    }

    
}
