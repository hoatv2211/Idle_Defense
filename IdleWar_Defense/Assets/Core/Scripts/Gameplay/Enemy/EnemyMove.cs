using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    protected float threshold; //sai số , tránh 2 con trùng hình vào nhau
    protected float startFollowY; // tọa độ y bắt đầu đi về phía hero target
    protected EnemyExControl enemyExControl;
    protected float length = 0f; // time chờ để tấn công

    public bool forceMoveStraight;
    public virtual void Init(EnemyExControl _enemyExControl)
    {
        enemyExControl = _enemyExControl;
        threshold = enemyExControl.random * Config.enemyMoveThreshold;
        if (threshold <= 0.15f) threshold = 0.15f; //bé gần = 0 thì liên tục di chuyển đó
        startFollowY = enemyExControl.Range - 2.2f + enemyExControl.random * 2f; //2.2f là y thấp nhất rồi + thêm Range Enemy
        length = 0f;
        
        enabled = true;
    }
    
    public virtual void End()
    {
        enabled = false;
    }
    
    // Update is called once per frame
    public virtual void FixedUpdate()
    {
        if (length > 0f)
        {
            length -= Time.fixedDeltaTime;
        }
        else
        {
            if (enemyExControl.target != null) Move();
            length = 0f;
        }
    }

    public void StopMove(float _length)
    {
        if(length > _length) return;
        
        length = _length;
        // LookAtTarget();
    }

    //move to the target
    public virtual void Move()
    {
        var position = (Vector2) enemyExControl.transform.position;
        var target = enemyExControl.target;
        var targetPosition = (Vector2) target.transform.position;
        if (enemyExControl.attack != null)
        {
            var attack = enemyExControl.attack;
            position += attack.offsetAttack;
        }

        if (position.y > startFollowY || forceMoveStraight)
        {
            transform.Translate(Vector3.down * enemyExControl.Movement * Time.fixedDeltaTime / 10f);

            if (position.y < targetPosition.y)
            {
                transform.position = new Vector3(transform.position.x, targetPosition.y, transform.position.z);
            }
        }
        else
        {
            //IDLE: nếu khoảng cách 2 thằng bằng tổng size / 2f
            float totalY = (enemyExControl.Size.y + target.Size.y) / 2f;
            if (Mathf.Abs(position.x - targetPosition.x) < threshold
                && Mathf.Abs(position.y - targetPosition.y) < totalY)
            {
                return;
            }
            
            //RUN TO TARGET: chạy đến khu vực tầm đánh
            Vector2 delta = (targetPosition + new Vector2(0f, enemyExControl.Range)) - position;
            transform.Translate(delta.normalized * enemyExControl.Movement * Time.fixedDeltaTime / 10f);
        }
        
        enemyExControl.AnimRun();
    }

    ////When the Primitive collides with the walls, it will reverse direction
    //void OnTriggerEnter2D(Collider2D col)
    //{
    //    if(col.CompareTag(Config.TAG_ENEMY))
    //    {
    //        if (transform.position.x >= col.transform.position.x && length <= 0f)
    //        {
    //            StopMove(0.5f);
    //        }
    //    }
    //}

    // private void LookAtTarget()
    // {
    //     if (enemyExControl.target != null)
    //     {
    //         float x = enemyExControl.target.transform.position.x;
    //
    //         if (transform.position.x >= x)
    //         {
    //             Left();
    //         }
    //         else
    //         {
    //             Right();
    //         }
    //     }
    // }

    // private void Left()
    // {
    //     if (enemyExControl.GetDirect() == DIRECTION.Right)
    //     {
    //         var scale = transform.localScale;
    //         transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
    //         scale = enemyExControl.hpBar.transform.localScale;
    //         enemyExControl.hpBar.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
    //     }
    // }
    //
    // private void Right()
    // {
    //     if (enemyExControl.GetDirect() == DIRECTION.Left)
    //     {
    //         var scale = transform.localScale;
    //         transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
    //         scale = enemyExControl.hpBar.transform.localScale;
    //         enemyExControl.hpBar.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
    //     }
    // }
}
