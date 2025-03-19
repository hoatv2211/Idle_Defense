  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    public bool autoHide = true;
    
    public SpriteRenderer imgHpBarBG;
    public SpriteRenderer imgHpBar;
    public SpriteRenderer imgShieldBar;

    private float length;
    private float sizeY;

    private float timeToHidden = 2f;
    private float lastHp;
    private void OnDrawGizmosSelected()
    {
        length = imgHpBar.sprite.rect.width / 100f;
        //imgHpBar.sprite.textureRect.width

        imgHpBar.transform.localPosition = new Vector3(-length / 2f, 0f, 0.02f);
        imgShieldBar.transform.localPosition = new Vector3(-length / 2f, 0f, 0.01f);
    }

    public void Init()
    {
        length = imgHpBar.sprite.rect.width / 100f;
        sizeY = imgHpBar.size.y;
        
        timeToHidden = 2f;
    }

    // Update is called once per frame
    private void Update()
    {
        if (enabled)
        {
            if (autoHide)
            {
                timeToHidden -= Time.deltaTime;

                if (timeToHidden <= 0f)
                {
                    imgHpBar.color = new Color(1f, 1f, 1f, 0f);
                    imgShieldBar.color = new Color(1f, 1f, 1f, 0f);
                    imgHpBarBG.color = new Color(1f, 1f, 1f, 0f);
                }
            }
            
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
    }

    public Vector3 ShowHP(float hp, float hpMax, float shield = 0f, float shieldMax = 0f)
    {
        if (hp <= 0f)
        {
            imgHpBar.size = new Vector2(0f, sizeY);
            imgShieldBar.size = new Vector2(0f, sizeY);

            if (autoHide)
            {
                timeToHidden = 0f;

                imgHpBar.color = new Color(1f, 1f, 1f, 0f);
                imgShieldBar.color = new Color(1f, 1f, 1f, 0f);
                imgHpBarBG.color = new Color(1f, 1f, 1f, 0f);
            }

            lastHp = hp;
            var pos = imgHpBar.transform.position;
            pos.z = OderLayerZ.Z_HP;
            return pos;
        }
        else
        {
            imgHpBar.size = new Vector2(length * (hp / hpMax), sizeY);
            if (shield <= 0f || shieldMax <= 0f)
            {
                imgShieldBar.size = new Vector2(0f, sizeY);
            }
            else
            {
                imgShieldBar.size = new Vector2(length * (shield / shieldMax), sizeY);
            }

            if (autoHide && lastHp > hp)
            {
                timeToHidden = 2f;
                imgHpBar.color = new Color(1f, 1f, 1f, 1f);
                imgShieldBar.color = new Color(1f, 1f, 1f, 1f);
                imgHpBarBG.color = new Color(1f, 1f, 1f, 1f);
            }

            lastHp = hp;
            var pos = imgHpBar.transform.position + new Vector3(length * (hp / hpMax), 0f, 0f);
            pos.z = OderLayerZ.Z_HP;
            return pos;
        }
    }
}
