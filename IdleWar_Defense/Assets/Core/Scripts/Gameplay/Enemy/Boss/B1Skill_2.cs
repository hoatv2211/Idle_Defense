using FoodZombie;
using UnityEngine;

namespace HedgehogTeam.EasyTouch.Enemy
{
    public class B1Skill_2 : EnemySkill
    {
        public float xDamagePercentSkill2;
        public float duration;
        public ParticleMan effectInHeroPrefab;

        public override void TriggerSkill()
        {
            if (enemyExControl.IsDead()) return;

            if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);
            var skillNerf = new SkillNerf(enemyExControl.enemyData.Id,null, effectInHeroPrefab, duration);
            skillNerf.stun = true;
            InfoAttacker infoAttacker = new InfoAttacker(false,
                InfoAttacker.TYPE_SKILL,
                null,
                enemyExControl,
                skillNerf,
                enemyExControl.Damage,
                100,
                xDamagePercentSkill2,
                enemyExControl.Accuracy,
                100f);

            var totalHero = GameplayController.Instance.GetHeroExs();
            if(totalHero==null) return;
            for (var index = 0; index < totalHero.Count; index++)
            {
                var item = totalHero[index];
                var heroControl = item;
                if (!heroControl.IsDead())
                    heroControl.GetHit(infoAttacker);
            }
        }

        public override void SkillEnd()
        {
            base.SkillEnd();
            currentPlaySkillTime = 0;
            enemyExControl.AnimIdle();
        }
    }
}