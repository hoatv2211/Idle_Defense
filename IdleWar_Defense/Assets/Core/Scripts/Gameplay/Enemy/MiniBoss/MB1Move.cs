using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MB1Move : EnemyMove
{
    public float distanceMove;
    public float yCoordinateStopMove;
    private Action callBack;
    private Sequence sequence;
    
    public override void End()
    {
        if (sequence != null) sequence.Kill();
        
        base.End();
    }

    public override void Move()
    {
        if (transform.position.y <= yCoordinateStopMove)
        {
            var delta = transform.position- enemyExControl.target.transform.position;
            float rotationZ = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg-(90);
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
            return;
        }
        base.Move();
    }

    
    public bool isMoving;
    public void MoveLeftOrRight(bool moveLeft, Action actionAfterMove)
    {
        isMoving = true;
        callBack = actionAfterMove;
        if (moveLeft)
            Left();
        else 
            Right();
    }
   
    public void Left()
    {
        sequence = DOTween.Sequence();
        //sequence.Append(transform.DOLocalRotate(new Vector3(0, 0, -90), 0.2f));
        var tween = transform.DOMoveX(Config.EasyRandom(-0.75f, -3.2f), 1f);
        tween.OnStart(() => { enemyExControl.AnimRun(); });
        sequence.Append(tween);
        //var tween2 = transform.DOLocalRotate(new Vector3(0, 0, 0), 0.2f);
        tween.OnComplete(() =>
        {
            callBack?.Invoke();
            isMoving = false;
        });
        // sequence.Append(tween2);
        sequence.Play();
    }
    
    private void Right()
    {
        var sequence = DOTween.Sequence();
       // sequence.Append(transform.DOLocalRotate(new Vector3(0, 0, 90), 0.2f));
        var tween = transform.DOMoveX(Config.EasyRandom(0.75f, 3.2f), 1f);
        tween.OnStart(() => { enemyExControl.AnimRun(); });
        sequence.Append(tween);
        //var tween2 = transform.DOLocalRotate(new Vector3(0, 0, 0), 0.2f);
        tween.OnComplete(() =>
        {
            callBack?.Invoke();
            isMoving = false;
        });
        //sequence.Append(tween2);
        sequence.Play();
    }
}
