using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Sirenix.Serialization;
using UnityEngine;
using Utilities.Common;
using Utilities.Pattern.Data;
using Debug = UnityEngine.Debug;

namespace FoodZombie
{
    [System.Serializable]
    public class LevelUser : IComparable<LevelUser>
    {
        public int level;
        public int exp;
        public string unlock;
        public int[] rewardTypes;
        public int[] rewardIds;
        public int[] rewardValues;

        public List<RewardInfo> GetRewards()
        {
            var rewards = new List<RewardInfo>();
            if (rewardTypes != null)
            {
                var count = rewardTypes.Length;
                for (int i = 0; i < count; i++)
                {
                    rewards.Add(new RewardInfo(rewardTypes[i], rewardIds[i], rewardValues[i]));
                }
            }

            return rewards;
        }

        public int CompareTo(LevelUser other)
        {
            return level.CompareTo(other.level);
        }


    }

    [System.Serializable]
    public class VipUser : IComparable<VipUser>
    {
        public int vip;
        public int exp;
        public int heroSlot;
        public int buyCoinLimit;
        public int buyFastLoot;
        public int discoveryTime;
        public int discoveryBuyTime;
        public int afkBonusCoin;
        public int afkHeroExpBonus;
        public string unlockFeature;
        public int[] rewardTypes;
        public int[] rewardIds;
        public int[] rewardValues;

        public List<RewardInfo> GetRewards()
        {
            var rewards = new List<RewardInfo>();
            if (rewardTypes != null)
            {
                var count = rewardTypes.Length;
                for (int i = 0; i < count; i++)
                {
                    rewards.Add(new RewardInfo(rewardTypes[i], rewardIds[i], rewardValues[i]));
                }
            }

            return rewards;
        }

        public int CompareTo(VipUser other)
        {
            return vip.CompareTo(other.vip);
        }
    }

    public class UserGroup : DataGroup
    {

        public enum UserGameStep
        {
            OPEN_GAME = 0,
            CLICK_BATTLE = 100,
            CLICK_MISSION = 200,
            CLICK_MISSION_PLAY = 300,
            START_MISSION = 400,
            START_MISSION_CLICK_HEROFORMATION = 410,
            START_MISSION_CLICK_STARTGAME = 420,
            START_MISSION_CLICK_STARTGAME_SUCCESS = 430,
            END_MISSION = 500
        }


        #region Members

        private List<LevelUser> levelUsers;
        private List<VipUser> vipUsers;

        private IntegerData exp;
        private IntegerData level;
        private IntegerData vipExp;
        private IntegerData vipLevel;
        private IntegerData lastClaimLevel;
        private BoolData claimedVipToDay;
        private IntegerData gameStep;
        private BoolData openGameBefore;

        public IntegerData buyCoinCount;
        public IntegerData buyFastLootCount;
        public IntegerData rateShowCount;
        public BoolData rateCanOpen;
        private IntegerData missGemCount;
        private IntegerData levelShowup;
        public IntegerData inappBuyGemCount;
        public IntegerData playDayCount;
        public IntegerData missionLastLog;
        public IntegerData inappTime;
        public BoolData skipTut;

        static UserModel _UserData;
        public static UserModel UserData
        {
            get
            {
                if (_UserData == null) _UserData = GameSave.GetUserModel();
                return _UserData;
            }
            set
            {
                _UserData = value;
                GameSave.SetUserModel(_UserData);
                EventDispatcher.Raise(new UserModelChange());
            }
        }
        public string GameStepString
        {
            get
            {
                return ((UserGameStep)gameStep.Value).ToString();
            }
        }
        public int Exp => exp.Value;
        public int Level => level.Value;
        public int VipExp => vipExp.Value;
        public int VipLevel => vipLevel.Value;
        public int LastClaimLevel => lastClaimLevel.Value;
        public bool ClaimedVipToDay => vipLevel.Value <= 0 || claimedVipToDay.Value; //nếu quá 7 ngày và đã claim

        public bool OpenGameBefore
        {
            get
            {
                return openGameBefore.Value;
            }
            set
            {
                openGameBefore.Value = value;
            }
        }
        public bool RateCanOpen
        {
            get
            {
                return rateCanOpen.Value;
            }
            set
            {
                rateCanOpen.Value = value;
            }
        }

        public int MissGemCount
        {
            get { return missGemCount.Value; }
            set
            {
                missGemCount.Value = value;
            }
        }
        public int LevelShowup
        {
            get { return levelShowup.Value; }
            set
            {
                levelShowup.Value = value;
            }
        }
        public List<VipUser> VipUsers => vipUsers;

        #endregion

        //=============================================

        #region Public

        public void SetLevel(int _lv)
        {
            level.Value = _lv;
        }

        public override void PostLoad()
        {
            base.PostLoad();

            CheckLevelUnlocked();
            CheckVipLevelUnlocked();
        }

        public UserGroup(int _Id) : base(_Id)
        {
            //Declare sub groups which contain units data
            exp = AddData(new IntegerData(0, 0));
            level = AddData(new IntegerData(1, 1));
            vipExp = AddData(new IntegerData(2, 0));
            vipLevel = AddData(new IntegerData(3, 0));
            lastClaimLevel = AddData(new IntegerData(4, 1));
            claimedVipToDay = AddData(new BoolData(5, false));
            buyCoinCount = AddData(new IntegerData(6, 0));
            buyFastLootCount = AddData(new IntegerData(7, 0));
            gameStep = AddData(new IntegerData(8, 0));
            openGameBefore = AddData(new BoolData(9, false));
            rateShowCount = AddData(new IntegerData(10, 0));
            rateCanOpen = AddData(new BoolData(11, false));
            missGemCount = AddData(new IntegerData(12, 0));
            levelShowup = AddData(new IntegerData(13, 0));
            inappBuyGemCount = AddData(new IntegerData(14, 0));
            playDayCount = AddData(new IntegerData(15, 0));
            missionLastLog = AddData(new IntegerData(16, 0));
            inappTime = AddData(new IntegerData(17, 0));
            skipTut = AddData(new BoolData(18, false));
            InitLevelUsers();
            InitVipUsers();
        }
        public void SetGameStep(UserGameStep input)
        {
            int lastStep = Enum.GetValues(typeof(UserGameStep)).Cast<int>().Max();
            int value = (int)input;
            if (gameStep.Value < value && gameStep.Value <= lastStep)
            {
                gameStep.Value = value;
                GameData.Instance.SaveGame();
            }
            Debug.Log("set  gameStep " + (UserGameStep)gameStep.Value);
        }
        public void NewDay()
        {
            claimedVipToDay.Value = false;
            buyCoinCount.Value = 0;
            buyFastLootCount.Value = 0;

            List<TrapData> trapDatas = GameData.Instance.BaseGroup.GetAllTrapDatas();
            foreach (var item in trapDatas)
            {
                GameSave.Trap_TryTime_Set(item, 0);
            }

            playDayCount.Value++;
            //New day count Log:
            int _currentMission = GameData.Instance.MissionsGroup.CurrentMissionId;
            if (_currentMission != missionLastLog.Value)
            {
                missionLastLog.Value = _currentMission;
                //Config.LogPurchaseEvent
                Config.LogEvent(TrackingConstants.MISSION_MAX_DAY_COUNT, TrackingConstants.PARAM_DAY, playDayCount.Value.ToString(), TrackingConstants.PARAM_MISSION, _currentMission);
            }

        }

        //exp
        public void AddExp(int _exp)
        {
            exp.Value += _exp;
            EventDispatcher.Raise(new UserExpChangeEvent());

            CheckLevelUnlocked();
        }

        public LevelUser GetLevelInfo(int level)
        {
            return levelUsers[level - 1];
        }

        public List<RewardInfo> ClaimLevel()
        {
            var oldLevel = lastClaimLevel.Value;
            lastClaimLevel.Value++;
            return levelUsers[oldLevel - 1].GetRewards();
        }

        public bool CanShowLevelUp()
        {
            return lastClaimLevel.Value < level.Value;
        }

        //vip
        public void AddVipExp(int _exp)
        {
            vipExp.Value += _exp;

            CheckVipLevelUnlocked();
        }

        public VipUser GetCurrentVipInfo()
        {
            if (vipLevel.Value > 0) return vipUsers[vipLevel.Value - 1];

            return null;
        }

        public VipUser GetVipInfo(int vip)
        {
            return vipUsers[vip - 1];
        }

        public List<RewardInfo> ClaimVip()
        {
            if (!ClaimedVipToDay)
            {
                var vipUser = GetCurrentVipInfo();
                claimedVipToDay.Value = true;

                //Daily Quest và Achievement
                GameData.Instance.DailyQuestsGroup.TakeVipBonus();

                return vipUser.GetRewards();
            }

            return null;
        }

        //vip count
        public void BuyCoin()
        {
            buyCoinCount.Value++;
        }
        public void TryTrap(TrapData trapData)
        {
            int current = GameSave.Trap_TryTime_Get(trapData);
            GameSave.Trap_TryTime_Set(trapData, current + 1);
        }
        public bool CanTryTrap(TrapData trapData)
        {
            return GameSave.Trap_TryTime_Get(trapData) <= 0;
        }
        public void InappBuyGem()
        {
            inappBuyGemCount.Value++;
        }
        public void InappUpdate()
        {
            inappTime.Value++;
        }
        public bool isIAP()
        {
            return inappTime.Value > 0 || inappBuyGemCount.Value > 0;
        }
        public void BuyFastLoot()
        {
            buyFastLootCount.Value++;
        }

        //
        public bool CanBuyCoin()
        {
            var vipInfo = GetCurrentVipInfo();
            if (vipInfo == null) return buyCoinCount.Value < Constants.BUY_COIN_LIMIT;
            return buyCoinCount.Value < Constants.BUY_COIN_LIMIT + vipInfo.buyCoinLimit;
        }

        public bool CanBuyFastLoot()
        {
            var vipInfo = GetCurrentVipInfo();
            if (vipInfo == null) return buyFastLootCount.Value < Constants.BUY_FAST_LOOT_LIMIT;
            return buyFastLootCount.Value < Constants.BUY_FAST_LOOT_LIMIT + vipInfo.buyFastLoot;
        }

        public bool SkipTut
        {
            get
            {
                return skipTut.Value;
            }
            set
            {
                skipTut.Value = value;
            }

        }

        #endregion

        //==============================================

        #region Private

        private void CheckLevelUnlocked()
        {
            var oldLevel = level.Value;

            var count = levelUsers.Count;
            var expValue = exp.Value;
            int _level = 1;
            for (int i = 1; i < count; i++)
            {
                if (expValue > levelUsers[i - 1].exp && expValue <= levelUsers[i].exp)
                {
                    _level = levelUsers[i].level;
                }
            }

            var max = levelUsers.Count - 1;
            if (expValue > levelUsers[max].exp)
            {
                _level = levelUsers[max].level;
            }

            level.Value = _level;
            if (oldLevel != _level) EventDispatcher.Raise(new UserLevelUpEvent());
        }

        private void InitLevelUsers()
        {
            var dataContent = GameData.GetTextContent("Data/LevelUser");
            levelUsers = JsonHelper.GetJsonList<LevelUser>(dataContent);
            levelUsers.Sort();

            //đoạn này hotfix a Quang làm data là số exp cần để nâng level, chứ ko phải mức min max
            int total = 0;
            var count = levelUsers.Count;
            for (int i = 0; i < count; i++)
            {
                total += levelUsers[i].exp;
                levelUsers[i].exp = total;
            }
        }

        public float GetPercentExp()
        {
            float minExp = 0f;
            float maxExp = levelUsers[0].exp;
            int levelValue = level.Value;
            if (levelValue > 1)
            {
                minExp = levelUsers[levelValue - 2].exp;
                maxExp = levelUsers[levelValue - 1].exp;
            }

            return (exp.Value - minExp) / (maxExp - minExp);
        }

        //Vip
        private void CheckVipLevelUnlocked()
        {
            var oldLevel = vipLevel.Value;

            var count = vipUsers.Count;
            var expValue = vipExp.Value;
            int _level = 0;
            if (expValue >= vipUsers[0].exp) _level = 1; //vì ban đầu là ko có vip, muốn là vip 1 phải lớn hơn = 25
            for (int i = 1; i < count; i++)
            {
                if (expValue >= vipUsers[i - 1].exp && expValue < vipUsers[i].exp)
                {
                    _level = vipUsers[i - 1].vip;
                }
            }

            var max = vipUsers.Count - 1;
            if (expValue >= vipUsers[max].exp)
            {
                _level = vipUsers[max].vip;
            }

            vipLevel.Value = _level;
            if (oldLevel != _level)
            {
                EventDispatcher.Raise(new VipLevelUpEvent());
            }
        }

        private void InitVipUsers()
        {
            var dataContent = GameData.GetTextContent("Data/VipUser");
            vipUsers = JsonHelper.GetJsonList<VipUser>(dataContent);
            vipUsers.Sort();

            //đoạn này hotfix a Quang làm data là số exp cần để nâng level, chứ ko phải mức min max
            int total = 0;
            var count = vipUsers.Count;
            for (int i = 0; i < count; i++)
            {
                total += vipUsers[i].exp;
                vipUsers[i].exp = total;
            }
        }

        public float GetPercentVipExp()
        {
            float minExp = 0f;
            float maxExp = vipUsers[0].exp;
            int levelValue = vipLevel.Value;
            if (levelValue > 0 && levelValue < vipUsers.Count)
            {
                minExp = vipUsers[levelValue - 1].exp;
                maxExp = vipUsers[levelValue].exp;
            }

            var percent = (vipExp.Value - minExp) / (maxExp - minExp);
            if (percent > 1f) return 1f;
            else return percent;
        }

        #endregion
    }
}
