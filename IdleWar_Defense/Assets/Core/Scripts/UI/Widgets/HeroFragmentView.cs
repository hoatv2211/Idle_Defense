using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;

public class HeroFragmentView : MonoBehaviour
{
    public Image imgBg;
    public Image imgIcon;
    public Image imgElement;
    public Image imgRank;
    public Image imgRankBG;
    public Color[] colorRanks;
    public Image imgRankElement;
    public Image imgStockNumber;
    public TextMeshProUGUI txtStockNumber;
    public SimpleTMPButton btnChoice;
    public GameObject imgNoti;

    public HeroFragmentItemData heroFragmentItemData;
    private UnityAction<HeroFragmentView> choiceAction;

    private void Start()
    {
        if(btnChoice != null) btnChoice.onClick.AddListener(BtnChoice_Pressed);
    }
    
    public void Init(HeroFragmentItemData _heroFragmentItemData, UnityAction<HeroFragmentView> _choiceAction = null)
    {
        heroFragmentItemData = _heroFragmentItemData;
        choiceAction = _choiceAction;

        if (heroFragmentItemData == null)
        {
            imgIcon.SetActive(false);
            imgElement.SetActive(false);
            imgRank.SetActive(false);
            imgRankElement.SetActive(false);

            if(txtStockNumber != null) txtStockNumber.text = "";
        }
        else
        {
            imgIcon.SetActive(true);
            imgIcon.sprite = heroFragmentItemData.GetIcon();
            imgElement.sprite = heroFragmentItemData.GetElementIcon();
            var heroBase = heroFragmentItemData.heroBase;
            SetImgRank(heroBase.rank);
            SetRankElement(heroBase.rank);

            if (txtStockNumber != null)
            {
                txtStockNumber.text = Config.CurrencyAndCostToString(heroFragmentItemData.StockNumber, heroFragmentItemData.baseData.requireFragment);
                if (imgNoti != null) imgNoti.SetActive(heroFragmentItemData.StockNumber >= heroFragmentItemData.baseData.requireFragment);
            }
            SetStockNumberSize();
        }
    }
    
    public void BtnChoice_Pressed()
    {
        if(choiceAction != null) choiceAction(this);
    }
    
    private void SetImgRank(int rank)
    {
        if (imgRank == null) return;

        imgRank.sprite = AssetsCollection.instance.GetRankIcon(rank);
        imgRank.SetActive(true);

        imgRankBG.color = colorRanks[rank - 1];
    }
        
    private void SetRankElement(int rank)
    {
        //những chỗ như smart dissamble, thì ko cần tính toán
        if(imgBg == null || imgRankElement == null) return;
            
        float alpha = imgBg.rectTransform.sizeDelta.y / 200f;

        imgRankElement.SetActive(true);
        imgRankElement.sprite = AssetsCollection.instance.GetRankElementIcon(rank);
        imgRankElement.rectTransform.sizeDelta = new Vector2(69f * alpha, 69f * alpha);
        imgRankElement.rectTransform.localPosition = new Vector3(-76f * alpha, 76f * alpha, 0f);
    }
    
    private void SetStockNumberSize()
    {
        float alpha = imgBg.rectTransform.sizeDelta.y / 200f;
        imgStockNumber.rectTransform.sizeDelta = new Vector2(114f * alpha, 40f * alpha);
    }
}
