using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities.Components;

public class EnemyStatisticTool : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI txtLevel;
    public TextMeshProUGUI txtCount;
    public TextMeshProUGUI txtPower;
    public TextMeshProUGUI txtCoinDrop;

    public void Init(EnemyInfo _enemyInfo, int count, int power, int coinDrop)
    {
        icon.sprite = AssetsCollection.instance.enemyIcon.GetAsset(_enemyInfo.id - 1);

        txtLevel.text = _enemyInfo.level + "";
        txtCount.text = "x" + count + "";
        txtPower.text = "P " + power + "";
        txtCoinDrop.text = "C " + coinDrop + "";
    }
}
