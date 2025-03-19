using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using UnityEngine;
using UnityEngine.UI;

public class EnemyView : MonoBehaviour
{
    public Image imgIcon;
    public Image imgRace;

    public void Init(EnemyData _enemyData)
    {
        imgIcon.sprite = _enemyData.GetIcon();
        imgRace.sprite = _enemyData.GetElementIcon();
    }
}
