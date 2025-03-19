using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FoodZombie;
using Spine;
using UnityEngine;
using Event = Spine.Event;

public class B1Skill_1 : EnemySkill
{
    public float jumpRange;
    public float xDamagePercentSkill1;


    public string skillJumpEventName;
    public EventData eventDataJumpSkill;

    #region skill 1


    public override void Init(EnemyExControl _enemyExControl)
    {
        base.Init(_enemyExControl);
        eventDataJumpSkill = enemyExControl.skeletonAnimation.Skeleton.Data.FindEvent(skillJumpEventName);

    }

    public override void Skill()
    {
        //sau này muốn vừa đi vừa bắn thì tắt đi
        enemyExControl.StopMove(skillSpeed);
        var control = enemyExControl as B1Control;
        control.STAGE = B1Control.ANIM_JUMP_1_STAGE;
        //enemyExControl.AnimSkill();
    }
    public override void HandleAnimationStateEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (enemyExControl.lastIndexSkill == -1) return;
        if (enemyExControl.skills[enemyExControl.lastIndexSkill] != this) return;
        if (nameOfSkillAnimations.Contains(trackEntry.Animation.Name))
            CheckEvent(e);
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
                                                        100,
                                                        xDamagePercentSkill1,
                                                        enemyExControl.Accuracy,
                                                        100f);
        //nếu enemies ở gần dưới 3 ô
        RaycastHit2D[] hits = Physics2D.CircleCastAll((Vector2)transform.position + enemyExControl.Offset, 3f * Config.xRange, Vector2.zero);
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
    public void JumpToTarget()
    {
        if (enemyExControl == null) return;
        enemyExControl.StopMove(1f);
        var control = enemyExControl as B1Control;
        if (control != null)
            control.lastPost = transform.position;
        if (enemyExControl.target != null)
            transform.position = enemyExControl.target.transform.position + new Vector3(0, Config.xRange, 0f);
    }
    public override void SkillEnd()
    {
        enemyExControl.autoMove.StopMove(enemyExControl.enemyData.Cooldown);
        JumpBack();
    }

    private void JumpBack()
    {
        var control = enemyExControl as B1Control;
        control.STAGE = B1Control.ANIM_JUMP_2_STAGE;
        enemyExControl.autoMove.StopMove(enemyExControl.enemyData.Cooldown);
    }


    #endregion
}
