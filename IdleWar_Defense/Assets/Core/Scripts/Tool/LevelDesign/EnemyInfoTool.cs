using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities.Components;

public class EnemyInfoTool : MonoBehaviour
{
    public Image icon;
    public GameObject imgHighlight;
    public SimpleTMPButton btnChoice;
    public TMP_InputField inputLevel;
    private int index;
    private UnityAction<int> choiceAction;
    private UnityAction<int, int> changeAction;

    public void Init(int _index, EnemyInfo _enemyInfo, UnityAction<int> _choiceAction, UnityAction<int, int> _changeAction)
    {
        btnChoice.onClick.AddListener(BtnChoice_Pressed);
        inputLevel.onValueChanged.AddListener(InputLevel_Changed);
        
        index = _index;
        choiceAction = _choiceAction;
        changeAction = _changeAction;
        
        if (_enemyInfo.id != 0)
        {
            icon.sprite = AssetsCollection.instance.enemyIcon.GetAsset(_enemyInfo.id - 1);
            inputLevel.text = _enemyInfo.level + "";
        }
    }

    private void BtnChoice_Pressed()
    {
        choiceAction(index);
        imgHighlight.SetActive(true);
    }

    private void InputLevel_Changed(string s)
    {
        changeAction(index, int.Parse(s));
    }

    public void Choice()
    {
        imgHighlight.SetActive(true);
    }
    public void UnChoice()
    {
        imgHighlight.SetActive(false);
    }
}
