using Core.Scripts.Gameplay.Enemy.Boss;
using HedgehogTeam.EasyTouch.Enemy;
using UnityEngine;

namespace Core.Scripts.Gameplay.Enemy.MiniBoss
{
    public class MB3Attack : EnemyAttackDuelist
    {
        public override bool CanAttack()
        {
           
            var mb3Move = enemyExControl.autoMove as MB3Move;
            if (!mb3Move.IsStopMoveStraight()) return false;
            if (enemyExControl.target == null) return false;
            atkSpeed = enemyExControl.AttackTime * xCountAttackInAnim;
            var delta = transform.position- enemyExControl.target.transform.position;
            float rotationZ = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg-(90);
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
            return true;
        }
    }
}