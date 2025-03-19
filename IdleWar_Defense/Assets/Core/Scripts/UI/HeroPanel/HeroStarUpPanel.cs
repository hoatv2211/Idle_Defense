using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;

namespace FoodZombie.UI
{
    public class HeroStarUpPanel : MyGamesBasePanel
    {
        public TextMeshProUGUI txtElementCost;
        public Image imgElementCost;
        public Image[] imgStars;
        public Sprite imgStarOn, imgStarOff;
        public TextMeshProUGUI txtInfo;

        public SimpleTMPButton btnStarUp;

        public Transform imgFocus;
        public GameObject imgLock;

        private HeroView heroView;
        private int elementCost;
        private int currencyId;

        private UnityAction<HeroView> showHeroInHeroPanel;
        private void Start()
        {
            btnStarUp.onClick.AddListener(BtnStarUp_Pressed);
        }

        public void Init(HeroView _heroView, UnityAction<HeroView> _showHeroInHeroPanel)
        {
            heroView = _heroView;
            showHeroInHeroPanel = _showHeroInHeroPanel;

            ShowInfo();
            CoroutineUtil.StartCoroutine(IEFocus());
        }

        private void ShowInfo()
        {
            var heroData = heroView.heroData;
            var maxStars = heroData.GetMaxStars();
            var star = heroData.star;
            var count = imgStars.Length;
            for (int i = 0; i < star; i++)
            {
                imgStars[i].SetActive(true);
                imgStars[i].sprite = imgStarOn;
            }
            for (int i = star; i < maxStars; i++)
            {
                imgStars[i].SetActive(true);
                imgStars[i].sprite = imgStarOff;
            }
            for (int i = maxStars; i < count; i++)
            {
                imgStars[i].SetActive(false);
            }
            
            if (heroData.IsMaxStarUp())
            {
                txtInfo.text = "Hero is max of stars";
                elementCost = 0;
                
                txtElementCost.transform.parent.SetActive(false);
                
                btnStarUp.SetEnable(false);
                imgFocus.SetActive(false);
            }
            else {
                txtInfo.text = "Upgrade Hero to " + (heroData.star + 1) + " stars";
                elementCost = heroData.GetHeroStarUpCost();
                currencyId = CurrenciesGroup.GetCurrencyIDFromElementID(heroData.Element);
                var currency = GameData.Instance.CurrenciesGroup.GetValue(currencyId);
                
                txtElementCost.transform.parent.SetActive(true);
                txtElementCost.text = Config.CurrencyAndCostToKKKString(currency, elementCost);
                imgElementCost.sprite = AssetsCollection.instance.GetElementIcon(heroData.Element);
                
                btnStarUp.SetEnable(true);
                imgFocus.SetActive(true);
            }
        }

        //vì xử dụng layout nên phải đợi 1 frame mới set positon imgFocus đc
        private IEnumerator IEFocus()
        {
            yield return null;
            var heroData = heroView.heroData;
            if (!heroData.IsMaxStarUp())
            {
                imgFocus.position = new Vector3(imgStars[heroData.star].transform.position.x,
                    imgFocus.position.y,
                    imgFocus.position.z);
            }
        }

        private void BtnStarUp_Pressed()
        {
            var heroData = heroView.heroData;
            var currenciesGroup = GameData.Instance.CurrenciesGroup;
            if (!currenciesGroup.CanPay(currencyId, elementCost))
            {
                //MainPanel.instance.ShowWarningPopup("Not enough element dust");
                MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_17));
                return;
            }

            currenciesGroup.Pay(currencyId, elementCost);
            heroData.StarUp();
            ShowInfo();

            if (heroData.IsMaxStarUp())
            {
                imgLock.SetActive(true);
                Lock(true);

                StartCoroutine(IEBack());
            }
            else
            {
                imgLock.SetActive(true);
                Lock(true);
                
                imgFocus.DOMoveX(imgStars[heroData.star].transform.position.x, 0.5f).OnComplete(() =>
                {
                    imgLock.SetActive(false);
                    Lock(false);
                    
                    Back();
                    showHeroInHeroPanel(heroView);
                    heroView.Init(heroView.heroData, this);//fix tạm
                });
            }
        }

        private IEnumerator IEBack()
        {
            yield return new WaitForSeconds(0.5f);
            
            imgLock.SetActive(false);
            Lock(false);
            
            Back();
            showHeroInHeroPanel(heroView);
            heroView.Init(heroView.heroData, this);
        }
    }
}
