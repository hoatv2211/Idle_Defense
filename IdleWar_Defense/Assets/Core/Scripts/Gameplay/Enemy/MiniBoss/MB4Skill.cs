using System.Collections;
using DG.Tweening;
using FoodZombie;
using UnityEngine;

namespace Core.Scripts.Gameplay.Enemy.MiniBoss
{
    public class MB4Skill : EnemySkill
    {
        public Transform spawn;
        public int spawnEnemyId;
        public int number;

        public override bool CanSkill()
        {
            animName = nameOfSkillAnimations[UnityEngine.Random.Range(0, nameOfSkillAnimations.Length)];
            var duration = enemyExControl.skeletonAnimation.skeleton.Data.FindAnimation(animName).Duration;
            skillSpeed = duration + enemyExControl.AttackTime / 6f; //mỗi lần attack cần gán lại vì còn theo boost
            return true;
        }

        public override void TriggerSkill()
        {
            if (enemyExControl.IsDead()) return;
            if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);

            int level = enemyExControl.level;
            StartCoroutine(spawnEnemy(level));
        }

        private IEnumerator spawnEnemy(int level)
        {
            yield return null;
            for (var i = 0; i < number; i++)
            {
                //var enemy = GameplayController.Instance.SpawnEnemyWithId(spawnEnemyId,level,Random.Range(-3f,3f),Random.Range(transform.position.y,6f));
                var enemy = GameplayController.Instance.SpawnEnemyWithId(spawnEnemyId,level,spawn.position.x,spawn.position.y);
                var tween = enemy.transform.DOMove(
                    new Vector3(Random.Range(-3f, 3f), Random.Range(transform.position.y, 6f), 1000), 0.2f);
                tween.SetEase(Ease.Linear);
                tween.OnStart(() => {enemy.GetComponent<OderLayerZUpdate>().enabled = false; });
                tween.OnComplete(() => { enemy.GetComponent<OderLayerZUpdate>().enabled = true; });
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}