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

public class GearView : MonoBehaviour
{
    public Image imgBg;
    public Image imgIcon;
    public Image imgRank;
    public Image imgRankBG;
    public Color[] colorRanks;
    public GameObject[] imgStars;
    public Image imgEquiped;
    public SimpleTMPButton btnChoice;

    public GameObject imgHighlight;

    public GearData gearData;
    private UnityAction<GearView> choiceAction;

    private UnityAction<GearView, bool> addAction;
    private bool add = false;

    private void Start()
    {
        if(btnChoice != null) btnChoice.onClick.AddListener(BtnChoice_Pressed);
    }

    private void Init(GearData _gearData)
    {
        gearData = _gearData;
        
        if (gearData == null)
        {
            imgIcon.SetActive(false);
            imgRank.SetActive(false);
            imgRankBG.SetActive(false);
            
            var count = imgStars.Length;
            for (int i = 0; i < count; i++)
            {
                imgStars[i].SetActive(false);
            }
            
            imgEquiped.SetActive(false);
        }
        else
        {
            imgIcon.SetActive(true);
            imgIcon.sprite = gearData.GetIcon();
            SetImgRank(gearData.Rank);
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

            SetEquipedSize(gearData.IsEquipped());
        }
    }

    public void Init(GearData _gearData, UnityAction<GearView> _choiceAction = null)
    {
        Init(_gearData);
        choiceAction = _choiceAction;
        addAction = null;
    }

    public void Init(GearData _gearData, UnityAction<GearView, bool> _addAction)
    {
        Init(_gearData);
        choiceAction = null;
        addAction = _addAction;

        add = false;
        imgHighlight.SetActive(add);
    }

    public void BtnChoice_Pressed()
    {
        if (choiceAction != null)
        {
            choiceAction(this);
        }
        if (addAction != null)
        {
            add = !add;
            addAction(this, add);

            imgHighlight.SetActive(add);
        }
    }

    public void Choice()
    {
        if (imgHighlight != null) imgHighlight.SetActive(true);
    }

    public void UnChoice()
    {
        if (imgHighlight != null) imgHighlight.SetActive(false);
    }
    
    private void SetImgRank(int rank)
    {
        if (imgRank == null) return;

        imgRank.sprite = AssetsCollection.instance.GetRankIcon(rank);
        imgRank.SetActive(true);

        imgRankBG.color = colorRanks[rank - 1];
    }
    
    private void SetEquipedSize(bool equip)
    {
        float alpha = imgBg.rectTransform.sizeDelta.y / 200f;

        imgEquiped.SetActive(equip);
        imgEquiped.rectTransform.sizeDelta = new Vector2(40f * alpha, 40f * alpha);
    }
}