using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Scripts.Gameplay.Enemy.Boss;
using DG.Tweening;
using FoodZombie;
using UnityEngine;

public class B2Skill_3 : EnemySkill
{
	public override void Skill()
	{
		//sau này muốn vừa đi vừa bắn thì tắt đi
		var b2Control = enemyExControl as B2Control;
		b2Control.activeSkill = B2Control.SKILL.SKILL_3;
		enemyExControl.StopMove(skillSpeed);
		var control = enemyExControl as B2Control;
		control.STAGE = B2Control.ANIM_SKILL_3_STAGE;

	}
	public void JumpToTarget()
	{
		enemyExControl.autoTarget.End();
		enemyExControl.StopMove(1f);
		var heroList = GameplayController.Instance.GetHeroes();
		var aliveHero = heroList.FindAll(a => !a.IsDead());
		if (aliveHero == null || aliveHero.Count == 0) return;
		var highestHero = aliveHero[Random.Range(0, aliveHero.Count)];
		enemyExControl.target = highestHero;
		var control = enemyExControl as B2Control;
		control.lastPost = transform.position;
		transform.DOMove(enemyExControl.target.transform.position + new Vector3(0, Config.xRange, 0f), 0.5f);
	}

	public override void TriggerSkill()
	{
		if (enemyExControl == null || enemyExControl.IsDead()) return;
		if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);
		InfoAttacker infoAttacker = new InfoAttacker(false,
			InfoAttacker.TYPE_SKILL,
			null,
			enemyExControl,
			null,
			enemyExControl.Damage,
			100,
			100,
			enemyExControl.Accuracy,
			0);
		if (enemyExControl != null && !enemyExControl.IsDead() && enemyExControl.target != null)
			enemyExControl.target.GetHit(infoAttacker);

	}

	public override void SkillEnd()
	{
		base.SkillEnd();
		var b2Control = enemyExControl as B2Control;
		b2Control.activeSkill = B2Control.SKILL.SKILL_END;

		var control = enemyExControl as B2Control;
		control.STAGE = B2Control.ANIM_JUMP_1_STAGE;
	}
}
