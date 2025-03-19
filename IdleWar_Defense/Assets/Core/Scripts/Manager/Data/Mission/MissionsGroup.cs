using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FoodZombie.UI;
using UnityEngine;
using Utilities.Common;
using Utilities.Pattern.Data;
using Random = System.Random;

namespace FoodZombie
{
    public class MissionData : DataGroup
    {
        public MissionInfo baseData { get; private set; }
        private IntegerData stars;
        private BoolData claimedBossReward;
        public int mapId;
        private int typeMode;

        //afk
        public int CoinAFK => baseData.coinAFK;
        public int GemAFK => baseData.gemAFK;
        public int UserEXPAFK => baseData.userEXPAFK;
        public int HeroEXPAFK => baseData.heroEXPAFK;

        public bool IsWin => stars.Value > 0;
        public bool ClaimedBossReward => claimedBossReward.Value;

        public MissionData(int _id, MissionInfo _baseData, int _mapId, int _typeMode = Config.TYPE_MODE_NORMAL) : base(_id)
        {
            baseData = _baseData;
            mapId = _mapId;
            typeMode = _typeMode;
            stars = AddData(new IntegerData(0, 0));
            claimedBossReward = AddData(new BoolData(1, false));
        }

        public void Win()
        {
            stars.Value = 1;
        }

        public string GetName()
        {
            if (baseData.id == 0) return "Sanctuary";

            if (typeMode == Config.TYPE_MODE_NORMAL)
            {
                if (mapId == 1)
                {
                    int missionID = baseData.id % 1000;
                    if (missionID == 1)
                        return "Tut 1";
                    if (missionID == 2)
                        return "Tut 2";

                    return mapId + "-" + (missionID - 2) + "";
                }
                else
                    return mapId + "-" + baseData.id % 1000 + "";
            }
            else
            {
                var lastDiscoveryData = DiscoveriesGroup.lastDiscoveryData;
                return lastDiscoveryData.NameLocal + " Lv." + baseData.id;
            }
        }

        public List<RewardInfo> GetMainRewardInfos()
        {
            if (typeMode == Config.TYPE_MODE_NORMAL)
            {
                var coin = baseData.coinBonus;
                var gem = baseData.gemBonus;
                var expUser = baseData.userEXPBonus;
                var expHero = baseData.heroEXPBonus;

                var rewardInfos = new List<RewardInfo>();
                //coin
                if (coin > 0) rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_COIN, coin));
                //gem
                if (gem > 0) rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_GEM, gem));
                //Claim EXP user
                if (expUser > 0) rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_EXP_USER, 0, expUser));
                //EXP hero
                if (expHero > 0) rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_EXP_HERO, expHero));

                return rewardInfos;
            }
            else
            {
                return new List<RewardInfo>();
            }
        }

        public List<RewardInfo> GetBonusRewardInfos()
        {
            if (typeMode == Config.TYPE_MODE_NORMAL)
            {
                var rewardInfos = new List<RewardInfo>();
                if (baseData.gearBonus != 0)
                    rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_GEAR, baseData.gearBonus, 1));

                return rewardInfos;
            }
            else
            {
                return new List<RewardInfo>();
            }
        }

        public List<RewardInfo> GetAFKRewardInfos(int seconds)
        {
            //  Debug.LogError("GetAFKRewardInfos " + seconds);
            // #if DEVELOPMENT
            //             var minutes = seconds * 100 / 60;
            //             var hours = seconds * 100 / 3600;
            // #else
            var minutes = seconds / 60;
            var hours = seconds / 3600;
            // #endif
            var coin = (int)(baseData.coinAFK * minutes * Constants.MISSION_COIN_X);
            var gem = baseData.gemAFK * hours;
            var expUser = (int)(baseData.userEXPAFK * minutes * Constants.MISSION_USER_EXP_X);
            var expHero = (int)(baseData.heroEXPAFK * minutes * Constants.MISSION_HERO_EXP_X);

            var vipInfo = GameData.Instance.UserGroup.GetCurrentVipInfo();
            if (vipInfo != null)
            {
                coin += (vipInfo.afkBonusCoin * coin) / 100;
                expHero += (vipInfo.afkHeroExpBonus * expHero) / 100;
            }

            var rewardInfos = new List<RewardInfo>();
            //coin
            if (coin > 0) rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_COIN, coin));
            //gem
            if (gem > 0) rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_GEM, gem));
            //Claim EXP user
            if (expUser > 0) rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_EXP_USER, 0, expUser));
            //EXP hero
            if (expHero > 0) rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_EXP_HERO, expHero));

            return rewardInfos;
        }

        public int GenerateGearRewardId()
        {
            float r = UnityEngine.Random.Range(0f, 1f);
            if (r <= (baseData.rateGearAFK / 100f))
            {
                return baseData.gearAFK;
            }

            return -1;
        }

        //boss reward
        public List<RewardInfo> GetBossRewardInfos()
        {
            var coin = baseData.coinBoss;
            var gem = baseData.gemBoss;
            var expUser = baseData.userEXPBoss;
            var expHero = baseData.heroEXPBoss;

            var rewardInfos = new List<RewardInfo>();
            //gear
            if (baseData.gearBoss != 0) rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_GEAR, baseData.gearBonus, 1));
            //coin
            if (coin > 0) rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_COIN, coin));
            //gem
            if (gem > 0) rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_GEM, gem));
            //Claim EXP user
            if (expUser > 0) rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_EXP_USER, 0, expUser));
            //EXP hero
            if (expHero > 0) rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_EXP_HERO, expHero));

            if (baseData.powerFragmentBoss != 0)
                rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_POWER_FRAGMENT, baseData.powerFragmentBoss));
            if (baseData.powerCrystalBoss != 0)
                rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_POWER_CRYSTAL, baseData.powerCrystalBoss));
            if (baseData.powerDevineBoss != 0)
                rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_DEVINE_CRYSTAL, baseData.powerDevineBoss));

            return rewardInfos;
        }

        public List<RewardInfo> ClaimBossReward()
        {
            claimedBossReward.Value = true;
            return GetBossRewardInfos();
        }
    }

    public class MissionsGroup : DataGroup
    {
        private const int INTRO_MISSION_ID = 6;

        // #if DEVELOPMENT
        // private const int TIME_AFK = 8 * 5;
        // private const int TIME_AFK_GEAR = 4 * 5;
        // private const int TIME_BUFF = 2 * 5;
        // #else
        private const int TIME_AFK = 8 * 60 * 60; //8 hours
        private const int TIME_AFK_GEAR = 15 * 60; //15 minutes
        private const int TIME_BUFF = 2 * 60 * 60;
        // #endif

        #region Members

        private List<MapInfo> listMaps;

        private DataGroup missionsGroup;
        private IntegerData lastWinMissionId;
        private IntegerData currentMissionId;
        private TimerTask timerAFK;
        private TimerTask timerAFKGear;
        private ListData<int> rewardAFKGearIds;
        private MissionData introMissionData;
        private BoolData freeTicket;

        public List<MapInfo> ListMaps => listMaps;
        public int LastWinMissionId => lastWinMissionId.Value;
        public int CurrentMissionId => currentMissionId.Value;
        public bool isTimeAFKRunning => timerAFK.IsRunning;
        public bool FreeTicket => freeTicket.Value;

        #endregion

        //=============================================

        #region Public

        public MissionsGroup(int pId) : base(pId)
        {
            missionsGroup = AddData(new DataGroup(0));
            lastWinMissionId = AddData(new IntegerData(1, -1));
            currentMissionId = AddData(new IntegerData(2, 0));
            timerAFK = AddData(new TimerTask(3, false));
            timerAFKGear = AddData(new TimerTask(4, false));
            rewardAFKGearIds = AddData(new ListData<int>(5, new List<int>()));
            //INTRO_MISSION_ID = 6
            freeTicket = AddData(new BoolData(7, true));

            //trong InitMapsMissionsGroup có set:   introMissionData = AddData(new MissionData());
            InitMapsMissionsGroup();
            timerAFK.SetOnComplete(AFKTimeIsMax);
            timerAFKGear.SetOnComplete(AFKTimeGearDrop);
        }

        public void NewDay()
        {
            freeTicket.Value = true;
        }

        public override void PostLoad()
        {
            base.PostLoad();

            //lần chơi đầu tiên, chỉ định mission đầu tiên trong list làm mặc định
            if (currentMissionId.Value == 0)
            {
                currentMissionId.Value = missionsGroup.GetDataByIndex<MissionData>(0).baseData.id;
                lastWinMissionId.Value = -1;
            }

            //đề phòng sau này xóa mission, thêm mission
            CheckCurrentMission();

            //default time task
            CheckAFKTime();
        }

        public bool IsWinIntroMission()
        {
            // return !IsFirstMission();
            // if (TutorialsManager.SKIP_MISSION_INTRO)
            introMissionData.Win();
            return true;

            return introMissionData.IsWin;
        }

        public bool IsFirstMission()
        {
            return (currentMissionId.Value == 0 || currentMissionId.Value == missionsGroup.GetDataByIndex<MissionData>(0).baseData.id);
        }

        private void CheckCurrentMission()
        {
            var missionDatas = GetListMissionData();
            var count = missionDatas.Count;
            var currentMissionIdValue = currentMissionId.Value;
            //đoạn này cho trường hợp mission cuối
            //nếu chơi đến mission cuối rồi và lastmission = currentMissionId
            if (currentMissionIdValue == lastWinMissionId.Value
                && currentMissionIdValue == missionDatas[count - 1].baseData.id)
            {
                return;
            }

            //ko thì check cho trường hợp GD thêm map
            var indexMissionId = 0;
            //kiểm tra nếu currentMissionId có tồn tại không,
            //nếu không tồn tại thì tìm cái missionId lớn nhất mà bé hơn cái missionId đang save
            for (int i = 0; i < count; i++)
            {
                var item = missionDatas[i];
                if (item.baseData.id <= currentMissionIdValue)
                {
                    indexMissionId = i;
                }
                else
                {
                    break;
                }
            }

            //lấy ra được index mission hiện tại và last win mission đứng trước.
            currentMissionId.Value = missionDatas[indexMissionId].baseData.id;
            if (indexMissionId > 0) lastWinMissionId.Value = missionDatas[indexMissionId - 1].baseData.id;

            //nếu currentMission hiện tại đã win thì next mission
            var currentMission = GetCurrentMissionData();
            if (currentMission.IsWin) SetNextMission();
        }

        //map và mission id bắt đầu từ 1
        public MissionData GetMissionData(int pId)
        {
            return missionsGroup.GetData<MissionData>(pId);
        }

        public MapInfo GetCurrentMapInfo()
        {
            var currentMission = GetCurrentMissionData();

            //hot fix for dev, tester pass intro mission
            //nếu gọi vào hàm này thì pass luôn mission intro
            if (!introMissionData.IsWin)
            {
                currentMission.Win();
                //re-get
                currentMission = GetCurrentMissionData();
            }

            return listMaps[currentMission.mapId - 1];
        }

        public MissionData GetLastWinMissionData()
        {
            return GetMissionData(lastWinMissionId.Value);
        }

        public int IntToMissionID(int index)
        {
            int _curremtIndex = 0;
            for (int i = 0; i < listMaps.Count; i++)
            {
                MapInfo map = listMaps[i];
                for (int j = 0; j < map.missionNumber; j++)
                {
                    _curremtIndex++;
                    if (_curremtIndex == index)
                    {
                        int mapIndex = i + 1;
                        int missionIndex = j + 1;
                        return mapIndex * 1000 + missionIndex;
                    }
                }
            }
            return -1;
        }
        public MissionData GetCurrentMissionData()
        {
            //	Debug.LogError(" currentMissionId " + currentMissionId.Value);
            return GetMissionData(currentMissionId.Value);
            //if (introMissionData.IsWin) return GetMissionData(currentMissionId.Value);
            //return introMissionData;
        }

        public List<MissionData> GetListMissionData()
        {
            var list = new List<MissionData>();
            foreach (MissionData item in missionsGroup.Children)
            {
                list.Add(item);
            }
            return list;
        }

        public List<MissionData> GetListMissionDataInMap(int mapId)
        {
            var list = new List<MissionData>();
            foreach (MissionData item in missionsGroup.Children)
            {
                if (item.mapId == mapId) list.Add(item);
            }
            return list;
        }

        public void SetCurrentMission(int _currentMissionId)
        {

            lastWinMissionId.Value = _currentMissionId - 1;
            if (lastWinMissionId.Value <= 0) lastWinMissionId.Value = 0;
            currentMissionId.Value = _currentMissionId;
        }

        public void Cleared(ref List<RewardInfo> mainRewardInfos, ref List<RewardInfo> bonusRewardInfos, Dictionary<int, int> totalKills, int lastEnemiesCoin)
        {
            //nếu đã qua intro mission
            var winIntroMissionData = introMissionData.IsWin;
            var currentMissionIdValue = currentMissionId.Value;

            // if (winIntroMissionData)
            // {
            //     //nếu là mission đầu tiên thì chạy cái afk time reward
            //     if (currentMissionIdValue == 1004)
            //     {
            //         ResetAFKTime();
            //     }
            // }

            //count enemies
            int enemyId;
            int killsCount = 0;
            foreach (var kill in totalKills)
            {
                enemyId = kill.Key;
                killsCount = kill.Value;
                var enemyData = GameData.Instance.EnemiesGroup.GetEnemyData(enemyId);
                enemyData.AddKillsCount(killsCount);

                if (winIntroMissionData && (
                    enemyId == IDs.MB1
                    || enemyId == IDs.MB2
                    || enemyId == IDs.MB3
                    || enemyId == IDs.MB4
                    || enemyId == IDs.B1
                    || enemyId == IDs.B2
                    || enemyId == IDs.B3
                    || enemyId == IDs.B4))
                {
                    //Daily Quest và Achievement
                    GameData.Instance.AchievementsGroup.KillBoss();
                }
            }

            var mission = GetCurrentMissionData();
            //reward
            mainRewardInfos = mission.GetMainRewardInfos();
            bonusRewardInfos = mission.GetBonusRewardInfos();

            //Tách riêng coin drop từ Enemy ra. Lượng coin này chỉ có thể nhận khi user chiến thắng màn chơi. Ở màn Win sẽ hiển thị 2 loại coin : coin drop từ quái và coin bonus.
            if (lastEnemiesCoin > 0)
            {
                //insert vào vị trí thứ 2, còn x2 coin stage ở vị trí 1
                mainRewardInfos.Insert(1, new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_COIN, lastEnemiesCoin));
            }

            //hotfix: Clear 1-5 reward : x10 Power crystal
            if (mission.baseData.id == 1001)
            {
                bonusRewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_POWER_CRYSTAL, 10));
            }
            List<RewardInfo> bossGift = mission.GetBossRewardInfos();
            for (int i = 0; i < bossGift.Count; i++)
            {
                if (bossGift[i] != null) bonusRewardInfos.Add(bossGift[i]);
            }

            //save
            mission.Win();

            //nếu là mission cuối map
            var missionNumber = GetCurrentMapInfo().missionNumber;
            if (IsWinIntroMission() && missionNumber == mission.baseData.id % 1000)
            {
                //Daily Quest và Achievement
                GameData.Instance.AchievementsGroup.CompleteMap();
            }

            if (winIntroMissionData)
            {
                //chỉ tracking từ first mission 1001
                //  if (currentMissionIdValue <= 4000 || currentMissionIdValue % 5 == 0)
                {
                    Config.LogEvent(TrackingConstants.MISSION_WIN_COUNT, TrackingConstants.PARAM_MISSION, currentMissionIdValue);
                    //	AppsFlyerObjectScript.LogLevelClear(currentMissionIdValue);
                    //AdjustManager.LogLevelClear(currentMissionIdValue);
                }

                SetNextMission();
            }
            else
            {
                //set lastWinMission về 0 != 1
                lastWinMissionId.Value = introMissionData.baseData.id;
            }
        }

        private void SetNextMission()
        {
            //    Debug.LogError("Set Next Mision");
            var currentMissionIdValue = currentMissionId.Value;

            var missionDatas = GetListMissionData();
            var count = missionDatas.Count;
            lastWinMissionId.Value = currentMissionIdValue;
            //tìm trong list có missionId lớn hơn tiếp theo là được
            for (int i = 0; i < count; i++)
            {
                var item = missionDatas[i];
                var id = item.baseData.id;
                if (id > currentMissionIdValue)
                {
                    currentMissionId.Value = id;
                    break;
                }
            }
        }

        public RewardInfo Lose()
        {
            var currentMissionIdValue = currentMissionId.Value;
            //  if (currentMissionIdValue <= 4000 || currentMissionIdValue % 5 == 0)
            {
                Config.LogEvent(TrackingConstants.MISSION_LOSE_COUNT, TrackingConstants.PARAM_MISSION, currentMissionIdValue);
            }

            //gold reward
            var coin = 100;

            return new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_COIN, coin);
        }

        //AFK time reward
        private void CheckAFKTime()
        {
            if (!timerAFK.IsRunning)
            {
                timerAFK.Start(TIME_AFK);
            }

            if (!timerAFKGear.IsRunning)
            {
                timerAFKGear.Start(TIME_AFK_GEAR);
            }
        }

        private void ResetAFKTime()
        {
            var seconds = TIME_AFK - timerAFK.RemainSeconds;
            if (seconds > 0) timerAFK.AddSeconds(seconds);
            seconds = TIME_AFK_GEAR - timerAFKGear.RemainSeconds;
            if (seconds > 0) timerAFKGear.AddSeconds(seconds);

            rewardAFKGearIds.Reset();
        }

        private void AFKTimeIsMax(TimerTask timer, long remain)
        {
            // if (remain <= 0)
            // {
            //     UnityEngine.Debug.Log("AFK " + remain);
            // }
        }

        private void AFKTimeGearDrop(TimerTask timer, long remain)
        {
            if (remain <= 0)
            {
                var lastMissionWinData = GetLastWinMissionData();
                if (lastMissionWinData != null)
                {
                    var gearRewardId = lastMissionWinData.GenerateGearRewardId();
                    if (rewardAFKGearIds.Count < 32 && gearRewardId > 0) rewardAFKGearIds.Add(gearRewardId);
                    // EventDispatcher.Raise(new SafeChangeValueEvent(0, true));
                }

                timerAFKGear.Start(TIME_AFK_GEAR);
            }
        }

        public long GetRemainTimeAFK()
        {
            return timerAFK.RemainSeconds;
        }

        public long GetNextTimeAFK()
        {
            return TIME_AFK - timerAFK.RemainSeconds;
        }
        public string GetTimeAFK()
        {
            var s = TIME_AFK - timerAFK.RemainSeconds;
            return TimeHelper.FormatHHMMss(s, true);
        }

        public List<RewardInfo> GetAFKRewards(int minTime = 0)
        {
            if (IsFirstMission()) return new List<RewardInfo>();

            int seconds = TIME_AFK - (int)timerAFK.RemainSeconds;
            if (seconds > TIME_AFK)
            {
                seconds = TIME_AFK;
            }
            if (minTime > 0)
                if (seconds < minTime) seconds = minTime;

            var lastMissionWinData = GetLastWinMissionData();
            var rewardInfos = lastMissionWinData.GetAFKRewardInfos(seconds);
            //gear
            var count = rewardAFKGearIds.Count;
            RewardInfo lastRewardInfo = null;
            for (int i = 0; i < count; i++)
            {
                var rewardAFKGearId = rewardAFKGearIds[i];
                if (lastRewardInfo != null && lastRewardInfo.Id == rewardAFKGearId)
                {
                    lastRewardInfo.SetValue(lastRewardInfo.Value + 1);
                }
                else
                {
                    lastRewardInfo = new RewardInfo(IDs.REWARD_TYPE_GEAR, rewardAFKGearId, 1);
                    rewardInfos.Add(lastRewardInfo);
                }
            }

            return rewardInfos;
        }

        public void ClaimAFKRewards()
        {
            rewardAFKGearIds.Values = new List<int>();

            if (timerAFK.IsRunning)
            {
                timerAFK.Stop();
            }

            timerAFK.Start(TIME_AFK);
        }

        public List<RewardInfo> GetBuffTimeRewards()
        {
            if (IsFirstMission()) return new List<RewardInfo>();

            int seconds = TIME_BUFF;

            var lastMissionWinData = GetLastWinMissionData();
            var rewardInfos = lastMissionWinData.GetAFKRewardInfos(seconds);
            //gear
            var count = TIME_BUFF / TIME_AFK_GEAR;
            RewardInfo lastRewardInfo = null;
            for (int i = 0; i < count; i++)
            {
                var rewardAFKGearId = lastMissionWinData.GenerateGearRewardId();
                if (rewardAFKGearId > 0)
                {
                    if (lastRewardInfo != null && lastRewardInfo.Id == rewardAFKGearId)
                    {
                        lastRewardInfo.SetValue(lastRewardInfo.Value + 1);
                    }
                    else
                    {
                        lastRewardInfo = new RewardInfo(IDs.REWARD_TYPE_GEAR, rewardAFKGearId, 1);
                        rewardInfos.Add(lastRewardInfo);
                    }
                }
            }

            //Daily Quest và Achievement
            GameData.Instance.DailyQuestsGroup.FastTravel();

            return rewardInfos;
        }

        // public void BuffTime()
        // {
        //     if (timerAFK.IsRunning)
        //     {
        //         timerAFK.PassSeconds(TIME_BUFF);
        //     }
        // }

        public void SetFreeTicket()
        {
            freeTicket.Value = true;
        }

        public void ClaimFreeTicket()
        {
            freeTicket.Value = false;
        }

        #endregion

        //==============================================

        #region Private

        private void InitMapsMissionsGroup()
        {
            //init intro mission
            string dataContent = GameData.GetTextContent("LevelDesign/mission_Intro");
            if (dataContent != null && !dataContent.Equals(""))
            {
                var missionInfo = JsonUtility.FromJson<MissionInfo>(dataContent);
                introMissionData = AddData(new MissionData(INTRO_MISSION_ID, missionInfo, 1));
                //mission intro vẫn ở map 1
            }

            //init map
            dataContent = GameData.GetTextContent("LevelDesign/totalInfo");
            if (dataContent != null && !dataContent.Equals(""))
            {
                var totalInfo = JsonUtility.FromJson<TotalInfo>(dataContent);
                var countMap = totalInfo.countMap;
                //get tổng map

                listMaps = new List<MapInfo>();
                for (int i = 0; i < countMap; i++)
                {
                    //đọc thông tin mapInfo trong mỗi folder map
                    if (GameUnityData.instance.GameRemoteConfig.active_ABLevel)
                    {
                        dataContent = GameData.GetTextContent("LevelDesign/ABTest/Map" + (i + 1) + "/mapInfo");
                        if (dataContent.Trim().Length <= 0)
                        {
                            Debug.LogError("Not found AB Test level");
                            dataContent = GameData.GetTextContent("LevelDesign/Map" + (i + 1) + "/mapInfo");
                        }
                    }
                    else
                        dataContent = GameData.GetTextContent("LevelDesign/Map" + (i + 1) + "/mapInfo");
                    if (dataContent != null && !dataContent.Equals(""))
                    {
                        var mapInfo = JsonUtility.FromJson<MapInfo>(dataContent);
                        listMaps.Add(mapInfo);
                        //vì map id từ 1 đến 5 nên dùng list và get theo id - 1 là được

                        var countMission = mapInfo.missionNumber;
                        for (int j = 0; j < countMission; j++)
                        {
                            if (GameUnityData.instance.GameRemoteConfig.active_ABLevel)
                            {
                                dataContent = GameData.GetTextContent("LevelDesign/ABTest/Map" + (i + 1) + "/mission_" + (j + 1));
                                if (dataContent.Trim().Length <= 0)
                                {
                                    Debug.LogError("Not found AB Test level");
                                    dataContent = GameData.GetTextContent("LevelDesign/Map" + (i + 1) + "/mission_" + (j + 1));
                                }
                            }
                            else
                                dataContent = GameData.GetTextContent("LevelDesign/Map" + (i + 1) + "/mission_" + (j + 1));
                            if (dataContent != null && !dataContent.Equals(""))
                            {
                                //đọc thông tin MissionInfo
                                //vì mission id theo kiểu 1001->1010: tức là đang ở map 1 và mission thứ 1 -> mission 10
                                //phải chơi kiểu 1000 này rồi map 2 là 2000 vì sau có thể tăng giảm map mà nó ko ảnh hưởng order
                                //nên phải get theo id
                                var missionInfo = JsonUtility.FromJson<MissionInfo>(dataContent);
                                var data = new MissionData(missionInfo.id, missionInfo, mapInfo.id);
                                missionsGroup.AddData(data);
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
