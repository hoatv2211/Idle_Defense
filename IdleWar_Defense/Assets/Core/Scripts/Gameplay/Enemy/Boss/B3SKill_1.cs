using FoodZombie;
using Spine;
using UnityEngine;

namespace Core.Scripts.Gameplay.Enemy.Boss
{
    public class B3SKill_1 : EnemySkill
    {
        public float xDamagePercent;
        public float stunDuration;
        public ParticleMan effectInHeroPrefab;

        private int currentAttack;
        public override TrackEntry OnSkill()
        {
            enemyExControl.autoTarget.End();
            var heroList = GameplayController.Instance.GetHeroExs();
            if (heroList.Count > 0)
            {
                var heroEx = heroList[Random.Range(0, heroList.Count)];
                if(heroEx != null) enemyExControl.target = heroEx;
            }
            return base.OnSkill();
        }

        public override void TriggerSkill()
        {
            if (enemyExControl.IsDead()) return;

            if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);
            var skillNerf = new SkillNerf(enemyExControl.enemyData.Id, null,effectInHeroPrefab, stunDuration);
            skillNerf.stun = true;
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

            shots[currentAttack].Shot(infoAttacker);
            currentAttack++;
            currentAttack %= 3;
           
        }

        public override void SkillEnd()
        {
            base.SkillEnd();
            enemyExControl.autoTarget.Init(enemyExControl);
        }
    }
}