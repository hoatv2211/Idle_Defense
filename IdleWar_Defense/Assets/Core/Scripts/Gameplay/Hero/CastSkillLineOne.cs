using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastSkillLineOne : MonoBehaviour
{
    HeroSkill heroSkill;
    public void SetInit(HeroSkill _heroSkill)
    {
        gameObject.SetActive(true);
        heroSkill = _heroSkill;

        heroSkill.heroExControl.stage = 5;

        transform.SetParent(_heroSkill.transform);
        transform.localPosition = Vector3.zero;
        heroSkill = _heroSkill;

        Vector2 pos = heroSkill.heroExControl.target?heroSkill.heroExControl.target.transform.position:Vector3.up;
        Vector2 direction = (Vector2)pos - (Vector2)transform.position;
        transform.up = direction;
    }

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
    }

    public void HandleTouchBegan()
    {
        HandleTouchMoved();

    }

    public void HandleTouchMoved()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePosition.y < -4.6f) return;

        Vector2 direction = mousePosition - (Vector2)transform.position;
        transform.up = direction;
        heroSkill.LookAt(mousePosition);
    }

    public void HandleTouchCanceled()
    {

    }

    public void HandleTouchEnded()
    {
        //Action Skill
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        heroSkill.LineCastSkill(mousePosition);
        gameObject.SetActive(false);
    }

   
}
