using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HubInfoHero : MonoBehaviour
{
    public Image imgAvatar;
    public Image imgHpBar;
    
    public void Init(Sprite imgAvatarSprite)
    {
        imgAvatar.sprite = imgAvatarSprite;
    }

    public void ShowHP(float hp, float hpMax)
    {
        imgHpBar.fillAmount = hp / hpMax;
    }
}
