using System;
using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using FoodZombie.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;
using Utilities.Inspector;

namespace FoodZombie.UI
{
    public class WheelsPanel : MyGamesBasePanel
    {
        public CurrencyView coinView;
        public CurrencyView gemView;
        public CurrencyView expHeroView;
        public CurrencyView blueChipView;
        public CurrencyView goldenChipView;

        public GameObject groupGeneral;
        public GameObject groupRoyale;

        public Toggle togGeneral;
        public Toggle togRoyale;

        public TextMeshProUGUI txtGeneralTime;
        public TextMeshProUGUI txtRoyaleTime;
        public List<RewardView> rewardViews;
        public List<GameObject> imgLockClaims;
        public List<GameObject> imgHighlights;

        public SimpleTMPButton btnGeneralRefresh;
        public SimpleTMPButton btnFreeGeneralRefresh;
        public SimpleTMPButton btnGeneralDraw;
        public SimpleTMPButton btnGeneralDrawX10;
        public SimpleTMPButton btnGeneralAds;
        public SimpleTMPButton btnRoyaleRefresh;
        public SimpleTMPButton btnFreeRoyaleRefresh;
        public SimpleTMPButton btnRoyaleDraw;
        public SimpleTMPButton btnRoyaleDrawX10;

        public GameObject imgLock;

        private WheelData WheelData => GameData.Instance.WheelData;

        private int lastHighlights;

        void Start()
        {
            togGeneral.onValueChanged.AddListener(TogUpgrade_Changed);
            togRoyale.onValueChanged.AddListener(TogDisassemble_Changed);

            btnGeneralRefresh.onClick.AddListener(BtnGeneralRefresh_Pressed);
            btnFreeGeneralRefresh.onClick.AddListener(BtnFreeGeneralRefresh_Pressed);
            btnGeneralDraw.onClick.AddListener(BtnGeneralDraw_Pressed);
            btnGeneralDrawX10.onClick.AddListener(BtnGeneralDrawX10_Pressed);
            btnGeneralAds.onClick.AddListener(BtnGeneralAds_Pressed);
            btnRoyaleRefresh.onClick.AddListener(BtnRoyaleRefresh_Pressed);
            btnFreeRoyaleRefresh.onClick.AddListener(BtnFreeRoyaleRefresh_Pressed);
            btnRoyaleDraw.onClick.AddListener(BtnRoyaleDraw_Pressed);
            btnRoyaleDrawX10.onClick.AddListener(BtnRoyaleDrawX10_Pressed);

            EventDispatcher.AddListener<HasFreeGeneralRefresh>(OnHasFreeGeneralRefresh);
            EventDispatcher.AddListener<HasFreeRoyaleRefresh>(OnHasFreeRoyaleRefresh);
        }

        private void OnDestroy()
        {
            EventDispatcher.AddListener<HasFreeGeneralRefresh>(OnHasFreeGeneralRefresh);
            EventDispatcher.RemoveListener<HasFreeRoyaleRefresh>(OnHasFreeRoyaleRefresh);
        }

        private void Update()
        {
            txtGeneralTime.text = Localization.Get(Localization.ID.PANEL_TITLE_67)+"\n" + WheelData.GetTimeGeneralRefresh();
            txtRoyaleTime.text = Localization.Get(Localization.ID.PANEL_TITLE_67) + "\n" + WheelData.GetTimeRoyaleRefresh();
        }

        internal override void Init()
        {
            coinView.Init(IDs.CURRENCY_COIN);
            gemView.Init(IDs.CURRENCY_GEM);
            expHeroView.Init(IDs.CURRENCY_EXP_HERO);
            blueChipView.Init(IDs.CURRENCY_BLUE_CHIP);
            goldenChipView.Init(IDs.CURRENCY_GOLDEN_CHIP);

            //check trước rồi mới hiện
            WheelData.CheckRefreshTime();

            if (togGeneral.isOn)
            {
                ShowGeneralRewards();
            }
            else
            {
                ShowRoyaleRewards();
            }
        }
		protected override void AfterHiding()
		{
			base.AfterHiding();
            Config.LogScene(TrackingConstants.SceneName.Screen_Home_MainMenu);
        }
		private void TogUpgrade_Changed(bool value)
        {
            if (value)
            {
                imgHighlights[lastHighlights].SetActive(false);
                ShowGeneralRewards();
            }
        }

        private void TogDisassemble_Changed(bool value)
        {
            if (value)
            {
                imgHighlights[lastHighlights].SetActive(false);
                ShowRoyaleRewards();
            }
        }

        private void ShowGeneralRewards()
        {
            groupGeneral.SetActive(true);
            groupRoyale.SetActive(false);

            var slotRewardDatas = WheelData.GetGeneralSlotRewards();
            var count = slotRewardDatas.Count;
            for (int i = 0; i < count; i++)
            {
                var slotRewardData = slotRewardDatas[i];
                rewardViews[i].Init(slotRewardData.GetReward());
                imgLockClaims[i].SetActive(!slotRewardData.CanClaim);
            }

            if (WheelData.FreeGeneralRefresh)
            {
                btnGeneralRefresh.SetActive(false);
                btnFreeGeneralRefresh.SetActive(true);
                txtGeneralTime.SetActive(false);
            }
            else
            {
                btnGeneralRefresh.SetActive(true);
                btnFreeGeneralRefresh.SetActive(false);
                txtGeneralTime.SetActive(true);
            }

            var spinCountByAdsInDay = WheelData.SpinCountByAdsInDay;
            btnGeneralAds.SetEnable(spinCountByAdsInDay < 5);
            //btnGeneralAds.labelTMP.text = "Free Chip " + spinCountByAdsInDay + "/5";

            btnGeneralAds.labelTMP.text = Localization.Get(Localization.ID.PANEL_WHEEL_BUTTON_37)+ " " + spinCountByAdsInDay + "/5";
        }

        private void ShowRoyaleRewards()
        {
            groupGeneral.SetActive(false);
            groupRoyale.SetActive(true);

            var slotRewardDatas = WheelData.GetRoyaleSlotRewards();
            var royaleSlots = WheelData.RoyaleSlots;
            var count = slotRewardDatas.Count;
            for (int i = 0; i < count; i++)
            {
                var slotRewardData = slotRewardDatas[i];
                rewardViews[i].Init(slotRewardData.GetReward());
                imgLockClaims[i].SetActive(!slotRewardData.CanClaim);
            }

            if (GameData.Instance.LevelUnlockContent < Constants.UNLOCK_WHEELS_ROYALE)
            {
                btnRoyaleRefresh.SetActive(true);
                btnFreeRoyaleRefresh.SetActive(false);
                txtRoyaleTime.SetActive(false);
            }
            else
            {
                if (WheelData.FreeRoyaleRefresh)
                {
                    btnRoyaleRefresh.SetActive(false);
                    btnFreeRoyaleRefresh.SetActive(true);
                    txtRoyaleTime.SetActive(false);
                }
                else
                {
                    btnRoyaleRefresh.SetActive(true);
                    btnFreeRoyaleRefresh.SetActive(false);
                    txtRoyaleTime.SetActive(true);
                }
            }
        }

        private void GeneralDraw()
        {
            int index = 0;
            RewardInfo rewardInfo = null;
            WheelData.GeneralDraw(ref index, ref rewardInfo);

            StartCoroutine(IEDraw(index, rewardInfo));
        }

        private void RoyaleDraw()
        {
            int index = 0;
            RewardInfo rewardInfo = null;
            WheelData.RoyaleDraw(ref index, ref rewardInfo);

            StartCoroutine(IEDraw(index, rewardInfo));
        }

        private IEnumerator IEDraw(int index, RewardInfo rewardInfo)
        {
            imgLock.SetActive(true);
            Lock(true);

            SoundManager.Instance.PlaySFX(IDs.SOUND_SPIN, 1f);

            var count = imgHighlights.Count;
            for (int i = 0; i < count; i++)
            {
                imgHighlights[i].SetActive(false);
            }

            imgHighlights[lastHighlights].SetActive(true);
            for (int i = 0; i < 12; i++)
            {
                yield return new WaitForSeconds(0.2f - 0.009f * i);
                imgHighlights[lastHighlights].SetActive(false);
                lastHighlights++;
                if (lastHighlights >= imgHighlights.Count) lastHighlights = 0;
                imgHighlights[lastHighlights].SetActive(true);
            }
            for (int i = 0; i < 12; i++)
            {
                yield return new WaitForSeconds(0.05f + 0.009f * i);
                imgHighlights[lastHighlights].SetActive(false);
                lastHighlights++;
                if (lastHighlights >= imgHighlights.Count) lastHighlights = 0;
                imgHighlights[lastHighlights].SetActive(true);
            }

            while (lastHighlights != index)
            {
                yield return new WaitForSeconds(0.21f);
                imgHighlights[lastHighlights].SetActive(false);
                lastHighlights++;
                if (lastHighlights >= imgHighlights.Count) lastHighlights = 0;
                imgHighlights[lastHighlights].SetActive(true);
            }

            yield return new WaitForSeconds(0.15f);
            imgHighlights[lastHighlights].SetActive(false);
            yield return new WaitForSeconds(0.15f);
            imgHighlights[lastHighlights].SetActive(true);
            yield return new WaitForSeconds(0.15f);
            imgHighlights[lastHighlights].SetActive(false);
            yield return new WaitForSeconds(0.15f);
            imgHighlights[lastHighlights].SetActive(true);

            imgLock.SetActive(false);
            Lock(false);
            LogicAPI.ClaimReward(rewardInfo, TrackingConstants.VALUE_WHEEL);

            if (togGeneral.isOn)
            {
                ShowGeneralRewards();
            }
            else
            {
                ShowRoyaleRewards();
            }
        }

        private void BtnGeneralRefresh_Pressed()
        {
            var currenciesGroup = GameData.Instance.CurrenciesGroup;
            if (!currenciesGroup.CanPay(IDs.CURRENCY_GEM, 10))
            {
                MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_2));
               // MainPanel.instance.ShowWarningPopup("Not enough gem");
                GameData.Instance.UserGroup.MissGemCount++;
                return;
            }

            currenciesGroup.Pay(IDs.CURRENCY_GEM, 10, TrackingConstants.VALUE_WHEEL_F5_GENERAL);
            WheelData.RefreshGeneralReward();

            ShowGeneralRewards();
        }

        private void BtnFreeGeneralRefresh_Pressed()
        {
            WheelData.RefreshGeneralReward();

            ShowGeneralRewards();
        }

        private void BtnGeneralDraw_Pressed()
        {
            var currenciesGroup = GameData.Instance.CurrenciesGroup;
            if (!currenciesGroup.CanPay(IDs.CURRENCY_BLUE_CHIP, 1))
            {
                MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_37));
                //MainPanel.instance.ShowWarningPopup("Not enough blue chip");
                return;
            }

            currenciesGroup.Pay(IDs.CURRENCY_BLUE_CHIP, 1);
            GeneralDraw();
        }

        private void BtnGeneralDrawX10_Pressed()
        {
            var vipLevel = GameData.Instance.UserGroup.VipLevel;
            if (GameData.Instance.LevelUnlockContent < Constants.UNLOCK_WHEELS_ROYALE && vipLevel < 2)
            {
                MainPanel.instance.ShowWarningPopup(string.Format(Localization.Get(Localization.ID.MESSAGE_38), GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_WHEELS_ROYALE)));
                //MainPanel.instance.ShowWarningPopup( GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_WHEELS_ROYALE) + " or Get VIP level 2 to play this feature");
                return;
            }
            var currenciesGroup = GameData.Instance.CurrenciesGroup;
            if (!currenciesGroup.CanPay(IDs.CURRENCY_BLUE_CHIP, 8))
            {
                MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_37));
               // MainPanel.instance.ShowWarningPopup("Not enough blue chip");
                return;
            }

            currenciesGroup.Pay(IDs.CURRENCY_BLUE_CHIP, 8);

            //loop
            var rewardInfos = new List<RewardInfo>();
            int index = 0;
            RewardInfo rewardInfo = null;
            for (int i = 0; i < 10; i++)
            {
                WheelData.GeneralDraw(ref index, ref rewardInfo);
                rewardInfos.Add(rewardInfo);
            }
            LogicAPI.ClaimRewards(rewardInfos, TrackingConstants.VALUE_WHEEL);
        }

        private void BtnGeneralAds_Pressed()
        {
            if (!AdsHelper.__IsVideoRewardedAdReady())
            {
                MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_1));
                //MainPanel.instance.ShowWarningPopup("Ads not available");
            }
            else
            {
                AdsHelper.__ShowVideoRewardedAd(TrackingConstants.ADS_REWARD_FREE_WHEEL, OnRewardedAdCompleted);
            }
        }

        private void OnRewardedAdCompleted(bool isCompleted)
        {
            if (isCompleted)
            {
                WheelData.AddSpinCountByAdsInDay();
                GeneralDraw();
            }
        }

        private void BtnRoyaleRefresh_Pressed()
        {
            if (GameData.Instance.LevelUnlockContent < Constants.UNLOCK_WHEELS_ROYALE)
            {
                MainPanel.instance.ShowWarningPopup(string.Format(Localization.Get(Localization.ID.MESSAGE_39), GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_WHEELS_ROYALE)));
                //MainPanel.instance.ShowWarningPopup("Get " + GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_WHEELS_ROYALE) + " to play this feature");
                return;
            }

            var currenciesGroup = GameData.Instance.CurrenciesGroup;
            if (!currenciesGroup.CanPay(IDs.CURRENCY_GEM, 100))
            {
                MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_2));
               // MainPanel.instance.ShowWarningPopup("Not enough gem");
                GameData.Instance.UserGroup.MissGemCount++;
                return;
            }

            currenciesGroup.Pay(IDs.CURRENCY_GEM, 100, TrackingConstants.VALUE_WHEEL_F5_ROYALE);
            WheelData.RefreshRoyaleReward();

            ShowRoyaleRewards();
        }

        private void BtnFreeRoyaleRefresh_Pressed()
        {
            WheelData.RefreshRoyaleReward();

            ShowRoyaleRewards();
        }

        private void BtnRoyaleDraw_Pressed()
        {
            if (GameData.Instance.LevelUnlockContent < Constants.UNLOCK_WHEELS_ROYALE)
            {
                MainPanel.instance.ShowWarningPopup(string.Format(Localization.Get(Localization.ID.MESSAGE_39), GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_WHEELS_ROYALE)));
                //MainPanel.instance.ShowWarningPopup("Get " + GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_WHEELS_ROYALE) + " to play this feature");
                return;
            }

            var currenciesGroup = GameData.Instance.CurrenciesGroup;
            if (!currenciesGroup.CanPay(IDs.CURRENCY_GOLDEN_CHIP, 1))
            {
                //MainPanel.instance.ShowWarningPopup("Not enough golden chip");
                MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_40));
                return;
            }

            currenciesGroup.Pay(IDs.CURRENCY_GOLDEN_CHIP, 1);
            RoyaleDraw();
        }

        private void BtnRoyaleDrawX10_Pressed()
        {
            if (GameData.Instance.LevelUnlockContent < Constants.UNLOCK_WHEELS_ROYALE)
            {
                MainPanel.instance.ShowWarningPopup(string.Format(Localization.Get(Localization.ID.MESSAGE_39), GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_WHEELS_ROYALE)));
                //MainPanel.instance.ShowWarningPopup("Get " + GameData.Instance.LevelUnlockContentText( Constants.UNLOCK_WHEELS_ROYALE) + " to play this feature");
                return;
            }

            var currenciesGroup = GameData.Instance.CurrenciesGroup;
            if (!currenciesGroup.CanPay(IDs.CURRENCY_GOLDEN_CHIP, 10))
            {
                MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_40));
                //MainPanel.instance.ShowWarningPopup("Not enough golden chip");
                return;
            }

            currenciesGroup.Pay(IDs.CURRENCY_GOLDEN_CHIP, 10);

            //loop
            var rewardInfos = new List<RewardInfo>();
            int index = 0;
            RewardInfo rewardInfo = null;
            for (int i = 0; i < 10; i++)
            {
                WheelData.RoyaleDraw(ref index, ref rewardInfo);
                rewardInfos.Add(rewardInfo);
            }
            LogicAPI.ClaimRewards(rewardInfos, TrackingConstants.VALUE_WHEEL);
        }

        private void OnHasFreeGeneralRefresh(HasFreeGeneralRefresh e)
        {
            btnGeneralRefresh.SetActive(false);
            btnFreeGeneralRefresh.SetActive(true);
            txtGeneralTime.SetActive(false);
        }

        private void OnHasFreeRoyaleRefresh(HasFreeRoyaleRefresh e)
        {
            if (GameData.Instance.LevelUnlockContent < Constants.UNLOCK_WHEELS_ROYALE)
            {
                btnRoyaleRefresh.SetActive(true);
                btnFreeRoyaleRefresh.SetActive(false);
                txtRoyaleTime.SetActive(false);
            }
            else
            {
                btnRoyaleRefresh.SetActive(false);
                btnFreeRoyaleRefresh.SetActive(true);
                txtRoyaleTime.SetActive(false);
            }
        }
    }
}
