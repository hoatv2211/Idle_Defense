using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Inspector;
using Utilities.Components;
using System;
using Spine.Unity;
using Utilities.Common;
using UnityEngine.SceneManagement;
using DG.Tweening;
using HedgehogTeam.EasyTouch.UI;

namespace FoodZombie.UI
{
    public class LosePanel : MyGamesBasePanel
    {
        public SimpleTMPButton btnHome, btnLevelUpHero, btnUpgradeEquipment, btnRestart, btnSummonHero;
        public SimpleTMPButton btnStatistic;
        public RewardView rewardView;

        private void Start()
        {
            btnHome.onClick.AddListener(BtnHome_Pressed);
            btnLevelUpHero.onClick.AddListener(BtnLevelUpHero_Pressed);
            btnUpgradeEquipment.onClick.AddListener(BtnUpgradeEquipment_Pressed);
            btnRestart.onClick.AddListener(BtnRestart_Pressed);
            btnSummonHero.onClick.AddListener(BtnSummonHero_Pressed);
            btnStatistic.onClick.AddListener(BtnStatistic_Pressed);
        }

        public void Init(RewardInfo rewardInfo)
        {
            GameplayController.Instance.PauseGame();
            SoundManager.Instance.PlaySFX(IDs.SOUND_DEFEAT, 1f);
            Lock(true);

            LogicAPI.ClaimReward(rewardInfo, TrackingConstants.VALUE_LOSE, false);

            rewardView.Init(rewardInfo);
            Config.LastGameResult = -1;
            Config.isLoseToday = true;
        }

        private void BtnHome_Pressed()
        {
            AdsHelper.Instance.CheckAndShowAdsEndGame(TrackingConstants.ADS_INTER_END_GAME, () => { Time.timeScale = 0; }, (show) =>
            {
                Time.timeScale = 1;
                Lock(false);
                GameplayController.Instance.BackToHome();
            });
        }

        private void BtnLevelUpHero_Pressed()
        {
            AdsHelper.Instance.CheckAndShowAdsEndGame(TrackingConstants.ADS_INTER_END_GAME, () => { Time.timeScale = 0; }, (show) =>
            {
                Time.timeScale = 1;
                Lock(false);
                Config.backToHomePanel = SceneName.HeroPanel;
                GameplayController.Instance.BackToHome();
            });
        }

        private void BtnUpgradeEquipment_Pressed()
        {
            AdsHelper.Instance.CheckAndShowAdsEndGame(TrackingConstants.ADS_INTER_END_GAME, () => { Time.timeScale = 0; }, (show) =>
            {
                Time.timeScale = 1;
                Lock(false);
                Config.backToHomePanel = SceneName.FactoryPanel;
                GameplayController.Instance.BackToHome();
            });
        }

        private void BtnRestart_Pressed()
        {
            AdsHelper.Instance.CheckAndShowAdsEndGame(TrackingConstants.ADS_INTER_END_GAME, () => { Time.timeScale = 0; }, (show) =>
            {
                Time.timeScale = 1;
                Lock(false);
                // AdsHelper.__ShowInterstitialAd(TrackingConstants.ADS_INTER_END_GAME);
                GameplayController.Instance.RestartGame();
            });
        }

        private void BtnSummonHero_Pressed()
        {
            AdsHelper.Instance.CheckAndShowAdsEndGame(TrackingConstants.ADS_INTER_END_GAME, () => { Time.timeScale = 0; }, (show) =>
            {
                Time.timeScale = 1;
                Lock(false);
                Config.backToHomePanel = SceneName.SummonGatePanel;
                GameplayController.Instance.BackToHome();
            });
        }
        private void BtnStatistic_Pressed()
        {
            MainGamePanel.instance.ShowHeroStatisticsPanel(DamageTracker.instance.heroDamageInBattle);
        }
    }
}
