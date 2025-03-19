using System.Collections;
using System.Collections.Generic;
using Core.Scripts.Gameplay.Enemy.Boss;
using FoodZombie;
using Spine;
using UnityEngine;

public class B2Skill_1 : EnemySkill
{
    public float xDamagePercent;
    public float freezeDuration;
    public ParticleMan effectInHero;

    public override void TriggerSkill()
    {
        if (enemyExControl.IsDead()) return;
        var heroList = GameplayController.Instance.GetHeroes();
        var aliveHero = heroList.FindAll(a => !a.IsDead());
        if(aliveHero==null || aliveHero.Count==0) return;
        var randomHero = aliveHero[Random.Range(0, aliveHero.Count)];
        if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);
        var skillNerf = new SkillNerf(enemyExControl.enemyData.Id, null,effectInHero, freezeDuration);
        skillNerf.stun = true;
        skillNerf.color = Color.blue;
        InfoAttacker infoAttacker = new InfoAttacker(false,
                                                        InfoAttacker.TYPE_SKILL,                                             
                                                        null,
                                                        enemyExControl,
                                                        skillNerf,
                                                        enemyExControl.Damage,
                                                        100,
                                                        xDamagePercent,
                                                        enemyExControl.Accuracy,
                                                        0);
        
        randomHero.GetHit(infoAttacker);

    }

    public override void SkillEnd()
    {
        base.SkillEnd();
        var b2Control = enemyExControl as B2Control;
        b2Control.activeSkill = B2Control.SKILL.NONE;
    }
}
