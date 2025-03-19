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

public class HeroGearSlot : MonoBehaviour
{
    public Image imgRank;
    public Image imgRankBG;
    public Color[] colorRanks;
    public Image imgIcon;
    public GameObject[] imgStars;
    public GameObject imgEquiped;
    public SimpleTMPButton btnChoice;

    public GameObject imgHighlight;

    public int slotIndex;
    public GearData gearData;
    private UnityAction<HeroGearSlot> choiceGearSlot;

    private void Start()
    {
        btnChoice.onClick.AddListener(BtnChoice_Pressed);
    }

    public void Init(GearData _gearData, UnityAction<HeroGearSlot> _choiceGearSlot, int _slotIndex = -1)
    {
        slotIndex = _slotIndex;
        gearData = _gearData;
        choiceGearSlot = _choiceGearSlot;
        if (gearData == null)
        {
            //rank
            imgRank.SetActive(false);
            imgRankBG.SetActive(false);
            //icon
            imgIcon.SetActive(false);
            //star
            var count = imgStars.Length;
            for (int i = 0; i < count; i++)
            {
                imgStars[i].SetActive(false);
            }
            
            imgEquiped.SetActive(false);
        }
        else
        {
            //rank
            imgRank.SetActive(true);
            imgRank.sprite = gearData.GetRankIcon();
            imgRankBG.SetActive(true);
            imgRankBG.color = colorRanks[gearData.Rank - 1];
            //icon
            imgIcon.SetActive(true);
            imgIcon.sprite = gearData.GetIcon();
            //star
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
            
            imgEquiped.SetActive(gearData.IsEquipped());
        }
    }

    public void BtnChoice_Pressed()
    {
        choiceGearSlot(this);
    }
    
    public void Choice()
    {
        if (imgHighlight != null) imgHighlight.SetActive(true);
    }

    public void UnChoice()
    {
        if (imgHighlight != null) imgHighlight.SetActive(false);
    }
}
