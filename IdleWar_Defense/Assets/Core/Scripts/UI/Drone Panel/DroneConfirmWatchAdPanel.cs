using System;
using System.Collections.Generic;
using FoodZombie;
using FoodZombie.UI;
using Utilities.Components;
using Random = UnityEngine.Random;

namespace Core.Scripts.UI.Drone_Panel
{
    public class DroneConfirmWatchAdPanel : MyGamesBasePanel
    {

        public RewardView[] items;
        public SimpleTMPButton btnWatch;
        private List<int> _itemIds;
        private static List<RewardInfo> _rewardInfos;
        private Action callBackAction;
        private void Start()
        {
            btnWatch.onClick.AddListener(BtnWatch_Pressed);
        }
        private void OnEnable()
        {
            GameplayController.Instance.PauseGame();
        }

        internal override void Back()
        {
            GameplayController.Instance.ResumeGame();
            callBackAction?.Invoke();
            base.Back();
        }

        internal void Init(Action callback)
        {
            callBackAction = callback;
            SelectRandomTrapId();
            CreateRandomReward();
            
            
            for (var i = 0; i < items.Length; i++)
            {
                var itemView = items[i];
                itemView.Init(_rewardInfos[i]);
            }
        }

        private void BtnWatch_Pressed()
        {
            //TODO: log event cho tracking click drone
            //Config.LogEvent(TrackingConstants.EVENT_WIN_X2_REQUEST);
            if (!AdsHelper.__IsVideoRewardedAdReady())
            {
                MainGamePanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_1));
                //MainGamePanel.instance.ShowWarningPopup("Ads not available");
            }
            else
            {
                Back();
                AdsHelper.__ShowVideoRewardedAd(TrackingConstants.ADS_REWARD_DRONE_IN_GAME, OnRewardedAdCompleted);
            }
        }
        private void CreateRandomReward()
        {
            _rewardInfos = new List<RewardInfo>();
            var freeDroneItems = GameData.Instance.StoreGroup.FreeDroneItems;
            var count = _itemIds.Count;
            for (var i = 0; i < count; i++)
            {
                _rewardInfos.Add( freeDroneItems[_itemIds[i]].GetReward());
            }
        }

        private void SelectRandomTrapId()
        {
            var freeDroneItems = GameData.Instance.StoreGroup.FreeDroneItems;
            var count = freeDroneItems.Count;
            _itemIds = new List<int>();
            for (var i = 0; i < 1; i++)
            {
                int rd;
                do
                {
                    rd = Random.Range(0, count);
                } while (_itemIds.Contains(rd) && _itemIds.Count <2);
                _itemIds.Add(rd);
            }
        }

        private void OnRewardedAdCompleted(bool success)
        {
            if (!success) return;
            
            LogicAPI.ClaimRewards(_rewardInfos);
            GameplayController.Instance.ShowTrapDatas();
        }
    }
}
