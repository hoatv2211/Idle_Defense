using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownBar : MonoBehaviour
{
    public SpriteRenderer imgCooldownBarBG;
    public SpriteRenderer imgCooldownBar;
    public GameObject fullFX;
    
    private float length;
    private float sizeY;

    // float timeToHidden = 2f;
    private float timeToHiddenFX = 0f;
    
    private void OnDrawGizmosSelected()
    {
        length = imgCooldownBar.sprite.rect.width / 100f;
        //imgHpBar.sprite.textureRect.width

        imgCooldownBar.transform.localPosition = new Vector3(-length / 2f, 0f, 0.01f);
    }

    public void Init()
    {
        length = imgCooldownBar.sprite.rect.width / 100f;
        sizeY = imgCooldownBar.size.y;
    }

    // Update is called once per frame
    // void FixedUpdate()
    // {
    //     timeToHidden -= Time.fixedDeltaTime;
    //     
    //     if(timeToHidden <= 0f)
    //     {
    //         imgHpBar.color = new Color(1f, 1f, 1f, 0f);
    //         imgHpBarBG.color = new Color(1f, 1f, 1f, 0f);
    //     }
    // }

    private void FixedUpdate()
    {
        timeToHiddenFX -= Time.fixedDeltaTime;
        
        if (timeToHiddenFX <= 0f)
        {
            if (fullFX.activeSelf) fullFX.SetActive(false);
        }
        else
        {
            if (!fullFX.activeSelf) fullFX.SetActive(true);
        }
    }

    public Vector3 ShowCooldown(float cooldown, float cooldown_MAX)
    {
        if (cooldown <= 0f) //full
        {
            timeToHiddenFX = 1f;
            imgCooldownBar.size = new Vector2(length, sizeY);

            // timeToHidden = 0f;
            //
            // imgHpBar.color = new Color(1f, 1f, 1f, 0f);
            // imgHpBarBG.color = new Color(1f, 1f, 1f, 0f);

            return imgCooldownBar.transform.position;
        }
        else
        {
            imgCooldownBar.size = new Vector2(length * (1f - cooldown / cooldown_MAX), sizeY);

            // timeToHidden = 2f;
            // imgHpBar.color = new Color(1f, 1f, 1f, 1f);
            // imgHpBarBG.color = new Color(1f, 1f, 1f, 1f);

            return imgCooldownBar.transform.position + new Vector3(length * (1 - cooldown / cooldown_MAX), 0f, 0f);
        }
    }
}
