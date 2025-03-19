using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CastSkillLineHold : MonoBehaviour
{

    private HeroSkill heroSkill;
    public void SetInit(HeroSkill _heroSkill)
    {
        gameObject.SetActive(true);
        transform.SetParent(_heroSkill.transform);
        transform.localPosition = Vector3.zero;
        heroSkill = _heroSkill;

        Vector2 pos = heroSkill.heroExControl.target.transform.position;
        Vector2 direction = (Vector2)pos - (Vector2)transform.position;
        transform.up = direction;
    }

    public bool isActive()
    {
        return gameObject.activeInHierarchy;
    }

    private bool isHold = false;

    //public bool useAtan2;
    //private void Update()
    //{
    //    if (!useAtan2)
    //    {
    //        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        Vector2 direction = mousePosition - (Vector2)transform.position;
    //        transform.up = direction;
    //        //you can take the right axis
    //        //transform.right = direction;
    //    }
    //    else
    //    {
    //        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        Vector2 direction = mousePosition - (Vector2)transform.position;
    //        float angle = Mathf.Atan2(mousePosition.y, mousePosition.x) * Mathf.Rad2Deg;
    //        transform.eulerAngles = new Vector3(0, 0, angle - 90f);
    //        //then here is no need to add extra rotation
    //        //transform.eulerAngles = new Vector3(0, 0, angle);
    //    }
    //}

    void Update()
    {
        // mimic the touch event on Desktop platform
        if (Input.GetMouseButtonDown(0))
        {
            HandleTouchBegan();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            HandleTouchEnded();
        }
        else if (Input.GetMouseButton(0))
        {  
            HandleTouchMoved();
        }

        if (isHold)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (mousePosition.y < -4.6f) return;

            Vector2 direction = mousePosition - (Vector2)transform.position;
            transform.up = direction;
            //you can take the right axis
            //transform.right = direction;

            //var pos = GameplayController.Instance.camera.ScreenToWorldPoint(Input.mousePosition);
            heroSkill.LookAt(mousePosition);
        }
        else
        {
            //Vector2 pos = heroSkill.heroExControl.target.transform.position;
            //Vector2 direction = (Vector2)pos - (Vector2)transform.position;
            //transform.up = direction;
        }

        if (!heroSkill.Skilling())
        {
            gameObject.SetActive(false);
        }
    }

    public void HandleTouchBegan()
    {
        isHold = true;
        HandleTouchMoved();

    }

    public void HandleTouchMoved()
    {
        
    }

    public void HandleTouchEnded()
    {
        isHold = false;
       
    }

    private EnemyControl GetNearestEnemy()
    {
        var list = GameplayController.Instance.GetEnemies();
        EnemyControl target = null;
        float range = 1000000;
        foreach (var enemyControl in list)
        {
            if (Vector2.Distance(new Vector2(heroSkill.transform.position.x, heroSkill.transform.position.y),
                new Vector2(enemyControl.transform.position.x, enemyControl.transform.position.y)) < range)
            {
                target = enemyControl;
                range = Vector2.Distance(
                    new Vector2(heroSkill.transform.position.x, heroSkill.transform.position.y),
                    new Vector2(enemyControl.transform.position.x, enemyControl.transform.position.y));
            }
        }
        return target;
    }
}
