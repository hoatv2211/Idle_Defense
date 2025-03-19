using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using UnityEngine;

public class MB1Skill : EnemySkill
{
    public float damageReductionPercent;
    public float duration;
    public float hpLostPercentTrigger;

    public ParticleMan buffEffect;
    private float lastHPPercent;
    public override void Init(EnemyExControl _enemyExControl)
    {
        base.Init(_enemyExControl);
        lastHPPercent = 100;
        InvokeRepeating(nameof(CheckCanSkill),0f,0.5f);
    }
    
    public void CheckCanSkill()
    {
        if (enemyExControl.IsDead()) return ;
        var currentPercent = enemyExControl.GetHPPercent();
        if (lastHPPercent - currentPercent < 10) return ;
        lastHPPercent = currentPercent;
        ActiveSkill();
    }
   
    public void ActiveSkill()
    {
        if (enemyExControl.IsDead()) return;

        if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);

        var skillBuff = new SkillBuff(enemyExControl.enemyData.Id,buffEffect,duration);
        skillBuff.damageReduction = damageReductionPercent;
        enemyExControl.AddBuff(skillBuff);
        // TODO: thêm add skill buff ở đây
    }
}
