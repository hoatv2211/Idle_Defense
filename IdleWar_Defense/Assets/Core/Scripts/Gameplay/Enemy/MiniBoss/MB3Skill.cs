using System.Collections;
using Core.Scripts.Gameplay.Enemy.Boss;
using FoodZombie;
using Spine;
using UnityEngine;

namespace Core.Scripts.Gameplay.Enemy.MiniBoss
{
    public class MB3Skill : EnemySkill
    {
        public float xDamagePercent;
        public float freezeDuration;
        public ParticleMan effectInHeroPrefab;
        public override TrackEntry OnSkill()
        {
            enemyExControl.autoTarget.End();
            var heroList = GameplayController.Instance.GetHeroExs();
            if (heroList?.Count != 0)
            {
                enemyExControl.target = heroList[Random.Range(0, heroList.Count)];
                var delta = transform.position- enemyExControl.target.transform.position;
                float rotationZ = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg-(90);
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
            }

            return base.OnSkill();
        }

        public override bool CanSkill()
        {
            var mb3Move = enemyExControl.autoMove as MB3Move;
            if (!mb3Move.IsStopMoveStraight()) return false;
            animName = nameOfSkillAnimations[UnityEngine.Random.Range(0, nameOfSkillAnimations.Length)];
            var duration = enemyExControl.skeletonAnimation.skeleton.Data.FindAnimation(animName).Duration;
            skillSpeed = duration + enemyExControl.AttackTime / 6f; //mỗi lần attack cần gán lại vì còn theo boost
            return mb3Move.IsStopMoveStraight();
        }

        public override void TriggerSkill()
        {
          
            if (enemyExControl.IsDead()) return;
            
            if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);
            var skillNerf = new SkillNerf(enemyExControl.enemyData.Id, null,effectInHeroPrefab, freezeDuration);
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
                0f);
            foreach (var shot in shots)
            {
                shot.Shot(infoAttacker);
            }
        }

        public override void SkillEnd()
        {
            enemyExControl.autoTarget.Init(enemyExControl);
            //enemyExControl.transform.LookAt( Vector3.down);
            transform.localEulerAngles = Vector3.zero;
            base.SkillEnd();
        }
    }
}