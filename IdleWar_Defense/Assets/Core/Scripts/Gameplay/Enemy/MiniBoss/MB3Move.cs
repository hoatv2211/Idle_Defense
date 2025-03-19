using UnityEngine;

namespace Core.Scripts.Gameplay.Enemy.Boss
{
    public class MB3Move : EnemyMove
    {
        public float yCoordinateStopMove;
        public override void Move()
        {
            Debug.Log("go hể");
            if (transform.position.y <= yCoordinateStopMove)
            {
                var delta = transform.position- enemyExControl.target.transform.position;
                float rotationZ = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg-(90);
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
                return;
            }
            base.Move();
        }
        public bool IsStopMoveStraight()
        {
            return transform.position.y <= yCoordinateStopMove;
        }
    }
}