using UnityEngine;

namespace Core.Scripts.Gameplay.Enemy.MiniBoss
{
    public class MB2Attack : EnemyAttack
    {
        public override bool CanAttack()
        {
            var mb3Move = enemyExControl.autoMove as MB2Move;
            if (!mb3Move.IsStopMoveStraight()||mb3Move.isMoving) return false;
            if (enemyExControl.target == null) return false;
            var mb2Control = enemyExControl as MB2Control;
            if (mb2Control.moveBeforeSkill) return false;
            
            atkSpeed = enemyExControl.AttackTime * xCountAttackInAnim;
            return true;
        }
    }
}