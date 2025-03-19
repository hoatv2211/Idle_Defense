using System;
using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;

public class UnEquipGear : MonoBehaviour
{
    public Image imgRank;
    public Image imgIcon;
    public GameObject imgHighlight;
    public GameObject[] imgStars;
    public SimpleTMPButton btnChoice;
    
    public GearData gearData;

    private UnityAction<UnEquipGear> choiceUnQuipGear;

    private void Start()
    {
        btnChoice.onClick.AddListener(BtnChoice_Pressed);
    }

    public void Init(GearData _gearData, UnityAction<UnEquipGear> _choiceUnQuipGear)
    {
        gearData = _gearData;
        choiceUnQuipGear = _choiceUnQuipGear;

        imgRank.sprite = gearData.GetRankIcon();
        imgIcon.SetActive(true);
        imgIcon.sprite = gearData.GetIcon();
        var star = gearData.star;
        var count = imgStars.Length;
        for (int i = 0; i < star; i++)
        {
            imgStars[i].SetActive(true);
        }

        for (int i = star; i < count; i++)
        {
            imgStars[i].SetActive(false);
        }

        UnChoice();
    }

    public void BtnChoice_Pressed()
    {
        choiceUnQuipGear(this);
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
