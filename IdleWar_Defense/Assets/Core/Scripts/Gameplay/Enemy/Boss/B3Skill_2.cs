using FoodZombie;
using UnityEngine;

namespace HedgehogTeam.EasyTouch.Enemy
{
    public class B3Skill_2 : EnemySkill
    {
        public float xDamagePercent;
        public ParticleMan effectInHeroPrefab;
        
        public override void TriggerSkill()
        {
            if (enemyExControl.IsDead()) return;

            if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);
            var skillNerf = new SkillNerf(enemyExControl.enemyData.Id, null,effectInHeroPrefab, 0.1f);
            InfoAttacker infoAttacker = new InfoAttacker(false,
                InfoAttacker.TYPE_SKILL,
                null,
                enemyExControl,
                skillNerf,
                enemyExControl.Damage,
                100,
                xDamagePercent,
                enemyExControl.Accuracy,
                0f);

            var totalHero = GameplayController.Instance.GetHeroes();
            for (var index = 0; index < totalHero.Count; index++)
            {
                var item = totalHero[index];
                var heroControl = item;
                heroControl.GetHit(infoAttacker);
            }
        }
    }
}