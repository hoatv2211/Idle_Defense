using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using UnityEngine;

public class BarrierControl : HeroControl
{
    public SpriteRenderer imgBody;
    public Sprite[] imgBarriers;
    
    private IEnumerator disapear = null;
    
    private IEnumerator showRedColor = null;

    public override void Init(float _HP, float _HPRegen)
    {
        attackTypes = new[] {IDs.ATTACK_TYPE_TRAP};

        //máu của barrier, cái này kế thừa ra 1 cái riêng sau
        HP = _HP;
        HP_MAX = HP;
        hpRegen = _HPRegen;

        //get hit show red color
        if (showRedColor != null)
        {
            StopCoroutine(showRedColor);
            showRedColor = null;
        }
        
        Refresh();
    }

    public override void Refresh()
    {
        base.Refresh();
        
        if (disapear != null)
        {
            StopCoroutine(disapear);
            disapear = null;
        }
        var color = imgBody.color;
        color.a = 1f;
        imgBody.color = color;
        
        SetImgBody();
    }

    public override void GetHit(InfoAttacker infoAttacker)
    {
        base.GetHit(infoAttacker);

        ShowRedColor();
        SetImgBody();
    }

    public override void OnDead()
    {
        base.OnDead();

        disapear = IEDisapear();
        StartCoroutine(disapear);
    }

    private void SetImgBody()
    {
        var count = imgBarriers.Length;
        if(count <= 0) return;
        
        var indexHP = (int) (HP * count / HP_MAX);
        if (indexHP == count) indexHP = count - 1;
        imgBody.sprite = imgBarriers[indexHP];
    }

    private IEnumerator IEDisapear()
    {
        float timePlay = 1.0f;
        while (timePlay >= 0f)
        {
            yield return null;
            timePlay -= Time.deltaTime;
            var color = imgBody.color;
            color.a = timePlay;
            imgBody.color = color;
        }
        
        gameObject.SetActive(false);
    }
    
    private void ShowRedColor()
    {
        if (showRedColor != null)
        {
            StopCoroutine(showRedColor);
        }

        showRedColor = IEShowRedColor();
        StartCoroutine(showRedColor);
    }

    protected virtual IEnumerator IEShowRedColor()
    {
        float timeMax = 0.5f;
        float timePlay = timeMax;
        float a;
        while (timePlay >= 0f)
        {
            yield return null;
            timePlay -= Time.deltaTime;
            a = imgBody.color.a;
            var c = Color.Lerp(Color.white, Color.red, (timePlay / timeMax));
            c.a = a;
            imgBody.color = c;
        }
    }
}
