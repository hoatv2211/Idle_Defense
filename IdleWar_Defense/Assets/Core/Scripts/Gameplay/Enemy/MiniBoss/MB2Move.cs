using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MB2Move : EnemyMove
{
    public float yCoordinateStopMove;
    public float distanceMove;
    private Action callBack;
    public override void Move()
    {
        if (transform.position.y <= yCoordinateStopMove ) return;
        base.Move();
    }

    public bool IsStopMoveStraight()
    {
        return transform.position.y <= yCoordinateStopMove;
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
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalRotate(new Vector3(0, 0, -90), 0.2f));
        var tween = transform.DOMoveX(transform.position.x - distanceMove, 1f);
        tween.OnStart(() => { enemyExControl.AnimRun(); });
        sequence.Append(tween);
        var tween2 = transform.DOLocalRotate(new Vector3(0, 0, 0), 0.2f);
        tween2.OnComplete(() =>
        {
            Debug.Log("move left finish");
            callBack?.Invoke();
            isMoving = false;
        });
        sequence.Append(tween2);
        sequence.Play();
    }
    
    private void Right()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalRotate(new Vector3(0, 0, 90), 0.2f));
        var tween = transform.DOMoveX(transform.position.x + distanceMove, 1f);
        tween.OnStart(() => { enemyExControl.AnimRun(); });
        sequence.Append(tween);
        var tween2 = transform.DOLocalRotate(new Vector3(0, 0, 0), 0.2f);
        tween2.OnComplete(() =>
        {
            Debug.Log("move right finish");
            callBack?.Invoke();
            isMoving = false;
        });
        sequence.Append(tween2);
        sequence.Play();
    }
}
