
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Common;
using Utilities.Pattern.Data;
using Utilities.Service;
using Utilities.Service.RFirebase;
using Zirpl.CalcEngine;
using Random = System.Random;

namespace FoodZombie
{
    [Serializable]
    public class PackageDefinition
    {
        public int id;
        public int type;
        public string sku;
        public string name;
        public int imgBgIndex; //hot fix cho artist vẽ store
        public int imgGemIndex; //hot fix cho artist vẽ store
        public string logName;
        public int level;
        public int[] rewardTypes;
        public int[] rewardIds;
        public int[] rewardValues;
        public string usd;
        public string sale;

        public List<RewardInfo> GetRewards()
        {
            var rewards = new List<RewardInfo>();
            var count = rewardTypes.Length;
            for (int i = 0; i < count; i++)
            {
                rewards.Add(new RewardInfo(rewardTypes[i], rewardIds[i], rewardValues[i]));
            }

            return rewards;
        }
    }

    public class PackageData : DataGroup
    {
        public PackageDefinition baseData { get; private set; }
        private IntegerData buyCount; //đã mua bao nhiêu lần, type == SPECIAL_PACK thì mua một lần thôi
        private IntegerData dayOfWeek;
        private IntegerData dayOfYear;
        private IntegerData monthOfYear;

        public int BuyCount => buyCount.Value;
        public bool CanBuy
        {
            get
            {
                //Check Special Pack buy?
                if (baseData.type == IDs.SPECIAL_PACK)
                {
                    //   Debug.Log("get pack "+baseData.sku);

                    if (BuyCount <= 0)
                    {
                        if (PaymentHelper.IsProductOwned(this))
                        {
                            buyCount.Value++;
                        }
                    }
                }
                if (baseData.type == IDs.SPECIAL_PACK
                    || baseData.type == IDs.DAILY_PACK
                    || baseData.type == IDs.WEEKLY_PACK
                    || baseData.type == IDs.MONTHLY_PACK)
                {
                    return BuyCount <= 0;
                }
                return true;
            }
        }

        /// <summary>
        /// Check Level User and show
        /// </summary>
        public bool CanShow => GameData.Instance.UserGroup.Level >= baseData.level;
        public int Type => baseData.type;
        public string Sku => baseData.sku;
        public string Name => baseData.name;
        public int ImgBgIndex => baseData.imgBgIndex;
        public int ImgGemIndex => baseData.imgGemIndex;
        public string LogName => baseData.logName;
        public string Usd => baseData.usd;

        public PackageData(int _id, PackageDefinition _baseData) : base(_id)
        {
            baseData = _baseData;
            buyCount = AddData(new IntegerData(0, 0));
            dayOfWeek = AddData(new IntegerData(1, 0));
            dayOfYear = AddData(new IntegerData(2, 0));
            monthOfYear = AddData(new IntegerData(3, 0));
        }



        public List<RewardInfo> GetRewards()
        {
            return baseData.GetRewards();
        }

        public List<RewardInfo> Buy()
        {
            buyCount.Value++;

            var now = UnbiasedTime.Instance.Now();
            dayOfWeek.Value = (int)now.DayOfWeek;
            dayOfYear.Value = (int)now.DayOfYear;
            monthOfYear.Value = (int)now.Month;

            GameData.Instance.StoreGroup.AddBuyCount();

            //Daily Quest và Achievement
            GameData.Instance.DailyQuestsGroup.BuyAnythingInStore();

            EventDispatcher.Raise(new SpecialPackBoughtEvent());
            return GetRewards();
        }

        public int GetVipExp()
        {
            var rewards = GetRewards();
            var count = rewards.Count;
            for (int i = 0; i < count; i++)
            {
                var reward = rewards[i];
                if (reward.Type == IDs.REWARD_TYPE_VIP) return reward.Value;
            }

            return 0;
        }

        public void Clear()
        {
            if (buyCount.Value <= 0) return;

            if (Type == IDs.DAILY_PACK)
            {
                buyCount.Value = 0;
                dayOfWeek.Value = 0;
                dayOfYear.Value = 0;
                monthOfYear.Value = 0;
            }
            else if (Type == IDs.WEEKLY_PACK)
            {
                var now = UnbiasedTime.Instance.Now();
                if ((int)now.DayOfWeek < dayOfWeek.Value
                    || (int)now.DayOfYear >= dayOfYear.Value + 7)
                {
                    buyCount.Value = 0;
                    dayOfWeek.Value = 0;
                    dayOfYear.Value = 0;
                    monthOfYear.Value = 0;
                }
            }
            else if (Type == IDs.MONTHLY_PACK)
            {
                var now = UnbiasedTime.Instance.Now();
                if ((int)now.Month != monthOfYear.Value)
                {
                    buyCount.Value = 0;
                    dayOfWeek.Value = 0;
                    dayOfYear.Value = 0;
                    monthOfYear.Value = 0;
                }
            }
        }
    }

    [Serializable]
    public class PremiumPackage
    {
        public int id;
        public int type;
        public string sku;
        public string name;
        public string logName;
        public int[] rewardTypes;
        public int[] rewardIds;
        public int[] rewardValues;
        public int[] dailyRewardTypes;
        public int[] dailyRewardIds;
        public int[] dailyRewardValues;
        public string usd;

        public List<RewardInfo> GetRewards()
        {
            var rewards = new List<RewardInfo>();
            var count = rewardTypes.Length;
            for (int i = 0; i < count; i++)
            {
                rewards.Add(new RewardInfo(rewardTypes[i], rewardIds[i], rewardValues[i]));
            }

            return rewards;
        }

        public List<RewardInfo> GetDailyRewards()
        {
            var rewards = new List<RewardInfo>();
            var count = dailyRewardTypes.Length;
            for (int i = 0; i < count; i++)
            {
                rewards.Add(new RewardInfo(dailyRewardTypes[i], dailyRewardIds[i], dailyRewardValues[i]));
            }

            return rewards;
        }
    }

    public class PremiumPackageData : DataGroup
    {
        public PremiumPackage baseData { get; private set; }
        private BoolData claimedToday;

        public bool ClaimedToday => claimedToday.Value;

        public int Type => baseData.type;
        public string Sku => baseData.sku;
        public string Name => baseData.name;
        public string LogName => baseData.logName;
        public string Usd => baseData.usd;

        public PremiumPackageData(int _id, PremiumPackage _baseData) : base(_id)
        {
            baseData = _baseData;
            claimedToday = AddData(new BoolData(0, false));
        }

        public List<RewardInfo> GetRewards()
        {
            return baseData.GetRewards();
        }

        public List<RewardInfo> GetDailyRewards()
        {
            return baseData.GetDailyRewards();
        }

        public List<RewardInfo> Claim()
        {
            claimedToday.Value = true;
            return GetDailyRewards();
        }

        public void Clear()
        {
            claimedToday.Value = false;
        }

        public List<RewardInfo> Buy()
        {
            GameData.Instance.StoreGroup.AddBuyCount();

            //Daily Quest và Achievement
            GameData.Instance.DailyQuestsGroup.BuyAnythingInStore();

            EventDispatcher.Raise(new SpecialPackBoughtEvent());
            return GetRewards();
        }
    }

    [Serializable]
    public class MarketItem
    {
        public int id;
        public string name;
        public int rewardType;
        public int rewardId;
        public int rewardValue;
        public int buyLimit;
        public int coinCost;
        public int gemCost;

        public RewardInfo GetReward()
        {
            return new RewardInfo(rewardType, rewardId, rewardValue);
        }
    }

    public class MarketItemData : DataGroup
    {
        public MarketItem baseData { get; private set; }
        private IntegerData buyCount;

        public bool CanBuy => buyCount.Value < baseData.buyLimit;
        public int BuyCount => buyCount.Value;
        public int CoinCost => baseData.coinCost;
        public int GemCost => baseData.gemCost;
        public int BuyLimit => baseData.buyLimit;

        public bool ByCoin => baseData.coinCost > 0;


        public MarketItemData(int _id, MarketItem _baseData) : base(_id)
        {
            baseData = _baseData;
            buyCount = AddData(new IntegerData(0, 0));
        }

        public void Clear()
        {
            buyCount.Value = 0;
        }

        public RewardInfo GetReward()
        {
            return baseData.GetReward();
        }

        public RewardInfo Buy()
        {
            buyCount.Value++;

            //Daily Quest và Achievement
            GameData.Instance.DailyQuestsGroup.BuyAnythingInStore();

            return GetReward();
        }
    }

    [Serializable]
    public class FlashSaleDefinition
    {
        public int id;
        public string name;
        public int rewardType;
        public int rewardId;
        public int[] rewardValues;
        public int unit;
        public int coinCost;
        public int gemCost;
        public int coinCostSale;
        public int gemCostSale;
        public int rate;

        //Random 6/64 món hàng hiện có để bán trong flash sale.Random loại tiền của món hàng nếu món hàng có giá trị của 2 loại tiền.
        //Random số lượng của món hàng đó (đã định sẵn ở cột số lượng) rồi nhân với giá hàng sau khi sale
        //sau 12h sẽ rerandom đổi 6 món hàng mới

        public RewardInfo GetReward(int valueIndex)
        {
            return new RewardInfo(rewardType, rewardId, rewardValues[valueIndex]);
        }
    }

    public class FlashSaleData : DataGroup
    {
        public FlashSaleDefinition baseData { get; private set; }
        private IntegerData valueIndex; //đã mua bao nhiêu lần, type == SPECIAL_PACK thì mua một lần thôi
        private BoolData bought;
        private BoolData byCoin;
        public int ValueIndex => valueIndex.Value;
        public bool Bought => bought.Value;
        public bool ByCoin => byCoin.Value;
        public string Name => baseData.name;
        public string NameLocal
        {
            get
            {
                switch (baseData.rewardType)
                {
                    case IDs.REWARD_TYPE_CURRENCY:
                        return Localization.Get("CURRENCY_NAME_" +baseData.rewardId);
                    case IDs.REWARD_TYPE_TRAP:
                        return Localization.Get("ITEM_TRAP_" + baseData.rewardId);
                    case IDs.REWARD_TYPE_GEAR:
                        return Localization.Get("ITEM_NAME_"+ baseData.rewardId);
                    default:
                        return "";
                }
            }
        }

        public int CoinCost => baseData.coinCost * baseData.rewardValues[ValueIndex] / baseData.unit;
        public int GemCost => baseData.gemCost * baseData.rewardValues[ValueIndex] / baseData.unit;
        public int CoinCostSale => baseData.coinCostSale * baseData.rewardValues[ValueIndex] / baseData.unit;
        public int GemCostSale => baseData.gemCostSale * baseData.rewardValues[ValueIndex] / baseData.unit;
        public int Rate => baseData.rate;
        public int rewardValuesLength => baseData.rewardValues.Length;
        public int Unit => baseData.unit;

        public FlashSaleData(int _id, FlashSaleDefinition _baseData) : base(_id)
        {
            baseData = _baseData;
            valueIndex = AddData(new IntegerData(0, 0));
            bought = AddData(new BoolData(1, false));
            byCoin = AddData(new BoolData(2, false));
        }

        public void Init(bool _byCoin, int _valueIndex)
        {
            bought.Value = false;

            if (_byCoin)
            {
                //nếu trả bằng coin thì check có giá coin không, nếu không trả bằng gem
                if (CoinCost > 0) byCoin.Value = true;
                else byCoin.Value = false;
            }
            else
            {
                //nếu trả bằng gem thì check có giá gem không, nếu không trả bằng coin
                if (GemCost > 0) byCoin.Value = false;
                else byCoin.Value = true;
            }

            valueIndex.Value = _valueIndex;
        }

        public RewardInfo GetReward()
        {
            return baseData.GetReward(valueIndex.Value);
        }

        public RewardInfo Buy()
        {
            bought.Value = true;

            //Daily Quest và Achievement
            GameData.Instance.DailyQuestsGroup.BuyAnythingInStore();

            return GetReward();
        }
    }

    [Serializable]
    public class FreeDroneItem
    {
        public int id;
        public string name;
        public int rewardType;
        public int rewardId;
        public int[] rewardValues;

        //Random mỗi lần 2 loại trap. Mỗi loại random 1~3. 
        public RewardInfo GetReward()
        {
            return new RewardInfo(rewardType, rewardId, rewardValues[Config.EasyRandom(0, rewardValues.Length)]);
        }
    }

    [Serializable]
    public class PremiumDroneItem
    {
        public int id;
        public string name;
        public int rewardType;
        public int rewardId;
        public int rewardValue;

        public RewardInfo GetReward()
        {
            return new RewardInfo(rewardType, rewardId, rewardValue);
        }
    }

    //==================================

    public class StoreGroup : DataGroup
    {
        private const int TIME_REFRESH = 8 * 60 * 60;
        private const int PREMIUM_PASS_ID = 16;
        #region Members

        private DataGroup specialPacksGroup;
        private DataGroup marketItemsGroup;
        private DataGroup flashSalesGroup;
        private IntegerData buyCount;
        private ListData<int> idFlashSaleChoices;
        private TimerTask timerRefresh;
        private PackageData premiumPass;
        private DataGroup gemPacksGroup;
        private DataGroup battlePacksGroup;
        private DataGroup dealPacksGroup;
        private DataGroup premiumPacksGroup;
        private IntegerData inappPopupTarget;
        private ListData<int> inappPopupWasShowed;
        private ListData<int> inappPopupToShow;
        private IntegerData inappPopupSubcriptionDayShow;

        private List<FreeDroneItem> freeDroneItems;
        private List<PremiumDroneItem> premiumDroneItems;

        public int InAppPopupTarget => inappPopupTarget.Value;

        // public ListData<int> InAppPopupWasShowedt => inappPopupWasShowed.Values;
        public int BuyCount => buyCount.Value;
        public PackageData PremiumPass => premiumPass;

        public List<FreeDroneItem> FreeDroneItems => freeDroneItems;
        public List<PremiumDroneItem> PremiumDroneItems => premiumDroneItems;

        public int InappPopupSubcriptionDayShow
        {
            get
            {
                return inappPopupSubcriptionDayShow.Value;
            }
            set
            {
                inappPopupSubcriptionDayShow.Value = value;
            }
        }

        #endregion

        //=============================================

        #region Public

        public StoreGroup(int pId) : base(pId)
        {
            specialPacksGroup = AddData(new DataGroup(0));
            flashSalesGroup = AddData(new DataGroup(1));
            buyCount = AddData(new IntegerData(2, 0));
            idFlashSaleChoices = AddData(new ListData<int>(3, new List<int>()));
            timerRefresh = AddData(new TimerTask(4, false));
            gemPacksGroup = AddData(new DataGroup(5));
            battlePacksGroup = AddData(new DataGroup(6));
            marketItemsGroup = AddData(new DataGroup(7));
            dealPacksGroup = AddData(new DataGroup(8));
            premiumPacksGroup = AddData(new DataGroup(9));
            inappPopupTarget = AddData(new IntegerData(10, -1));
            inappPopupWasShowed = AddData(new ListData<int>(11, new List<int>()));
            inappPopupToShow = AddData(new ListData<int>(12, new List<int>()));
            inappPopupSubcriptionDayShow = AddData(new IntegerData(13, 0));
            //Không dùng 16:16 cho Premium PASS

            InitSpecialPackages();
            InitPremiumPackages();
            InitGemPackages();
            InitBattlePackages();
            InitDealPackages();
            InitFlashSales();
            InitFreeDroneItems();
            InitPremiumDroneItems();
            InitPremiumPass();
            InitMarketItems();

            timerRefresh.SetOnComplete(TimeRefreshEnd);
        }

        public void NewDay()
        {
            F5Market();

            var premiumPackageDatas = GetAllPremiumPackageDatas();
            var count = premiumPackageDatas.Count;
            for (int i = 0; i < count; i++)
            {
                var premiumPackageData = premiumPackageDatas[i];
                premiumPackageData.Clear();
            }

            var packageDatas = GetAllDealPackageDatas();
            count = packageDatas.Count;
            for (int i = 0; i < count; i++)
            {
                var packageData = packageDatas[i];
                packageData.Clear();
            }

            //Popup inapp:
            InAppPopupUpdateShow(InappPopup.ID_1stTimeBuy, false);
            if (this.InappPopupSubcriptionDayShow > 0) this.InappPopupSubcriptionDayShow++;
            if (this.InappPopupSubcriptionDayShow >= 7)
            {
                GameData.Instance.UserGroup.MissGemCount = 0;
                this.InappPopupSubcriptionDayShow = 0;
                InAppPopupUpdateShow(InappPopup.ID_subcription, false);
            }
            Config.isLoseToday = false;

        }

        public override void PostLoad()
        {
            base.PostLoad();

            if (idFlashSaleChoices.Count <= 0) GenRandomFlashSaleChoices();
        }

        public List<string> GetAllConsumableSkus()
        {
            List<string> skus = new List<string>();

            var packageDatas = GetAllSpecialPackageDatas();
            var count = packageDatas.Count;
            for (int i = 0; i < count; i++)
            {
                var packageData = packageDatas[i];
                if (packageData.Usd != null
                    && !packageData.Usd.Equals("")
                    && !packageData.Usd.Equals("0")
                    && packageData.Sku != null
                    && !packageData.Sku.Equals(""))
                {
                    skus.Add(packageData.Sku);
                }
            }

            packageDatas = GetAllDealPackageDatas();
            count = packageDatas.Count;
            for (int i = 0; i < count; i++)
            {
                var packageData = packageDatas[i];
                if (packageData.Usd != null
                    && !packageData.Usd.Equals("")
                    && !packageData.Usd.Equals("0")
                    && packageData.Sku != null
                    && !packageData.Sku.Equals(""))
                {
                    skus.Add(packageData.Sku);
                }
            }

            packageDatas = GetAllBattlePackageDatas();
            count = packageDatas.Count;
            for (int i = 0; i < count; i++)
            {
                var packageData = packageDatas[i];
                if (packageData.Usd != null
                    && !packageData.Usd.Equals("")
                    && !packageData.Usd.Equals("0")
                    && packageData.Sku != null
                    && !packageData.Sku.Equals(""))
                {
                    skus.Add(packageData.Sku);
                }
            }

            packageDatas = GetAllGemPackageDatas();
            count = packageDatas.Count;
            for (int i = 0; i < count; i++)
            {
                var packageData = packageDatas[i];
                if (packageData.Usd != null
                    && !packageData.Usd.Equals("")
                    && !packageData.Usd.Equals("0")
                    && packageData.Sku != null
                    && !packageData.Sku.Equals(""))
                {
                    skus.Add(packageData.Sku);
                }
            }

            skus.Add(premiumPass.Sku);
            skus.Add(Constants.PREMIUM_DRONE_GEM_SKU);

            return skus;
        }

        public List<string> GetAllSubscriptionSkus()
        {
            List<string> skus = new List<string>();

            var premiumPackageData = GetAllPremiumPackageDatas();
            var count = premiumPackageData.Count;
            for (int i = 0; i < count; i++)
            {
                var packageData = premiumPackageData[i];
                if (packageData.Usd != null
                    && !packageData.Usd.Equals("")
                    && !packageData.Usd.Equals("0")
                    && packageData.Sku != null
                    && !packageData.Sku.Equals(""))
                {
                    skus.Add(packageData.Sku);
                }
            }

            return skus;
        }

        //Special
        public void AddBuyCount()
        {
            buyCount.Value++;
            EventDispatcher.Raise(new BuyCountChangeEvent());
        }

        public List<PackageData> GetAllSpecialPackageDatas()
        {
            var list = new List<PackageData>();
            foreach (PackageData item in specialPacksGroup.Children)
                list.Add(item);
            return list;
        }

        public PackageData GetSpecialPackageData(int pId)
        {
            return specialPacksGroup.GetData<PackageData>(pId);
        }

        //Premium
        public List<PremiumPackageData> GetAllPremiumPackageDatas()
        {
            var list = new List<PremiumPackageData>();
            foreach (PremiumPackageData item in premiumPacksGroup.Children)
                list.Add(item);
            return list;
        }

        public PremiumPackageData GetPremiumPackageData(int pId)
        {
            return premiumPacksGroup.GetData<PremiumPackageData>(pId);
        }

        //Gem
        public List<PackageData> GetAllGemPackageDatas()
        {
            var list = new List<PackageData>();
            foreach (PackageData item in gemPacksGroup.Children)
                list.Add(item);
            return list;
        }

        public PackageData GetGemPackageData(int pId)
        {
            return gemPacksGroup.GetData<PackageData>(pId);
        }

        //Battle
        public List<PackageData> GetAllBattlePackageDatas()
        {
            var list = new List<PackageData>();
            foreach (PackageData item in battlePacksGroup.Children)
                list.Add(item);
            return list;
        }

        public PackageData GetBattlePackageData(int pId)
        {
            return gemPacksGroup.GetData<PackageData>(pId);
        }

        //deal
        public List<PackageData> GetAllDealPackageDatas()
        {
            var list = new List<PackageData>();
            foreach (PackageData item in dealPacksGroup.Children)
                list.Add(item);
            return list;
        }

        public PackageData GetDealPackageData(int pId)
        {
            return dealPacksGroup.GetData<PackageData>(pId);
        }

        //Market
        public List<MarketItemData> GetAllMarketItemDatas()
        {
            var list = new List<MarketItemData>();
            foreach (MarketItemData item in marketItemsGroup.Children)
                list.Add(item);
            return list;
        }

        public MarketItemData GetMarketItemData(int pId)
        {
            return gemPacksGroup.GetData<MarketItemData>(pId);
        }

        //Flash Sale
        public List<FlashSaleData> GetAllFlashSaleDatas()
        {
            var list = new List<FlashSaleData>();
            foreach (FlashSaleData item in flashSalesGroup.Children)
                list.Add(item);
            return list;
        }

        public List<FlashSaleData> GetFlashSaleDataChoices()
        {
            var flashSaleChoices = new List<FlashSaleData>();
            var flashSales = GetAllFlashSaleDatas();
            var count = flashSales.Count;
            var countChoice = idFlashSaleChoices.Count;
            for (int i = 0; i < count; i++)
            {
                var flashSale = flashSales[i];
                for (int j = 0; j < countChoice; j++)
                {
                    if (flashSale.Id == idFlashSaleChoices[j])
                    {
                        flashSaleChoices.Add(flashSale);
                        break;
                    }
                }
            }

            return flashSaleChoices;
        }

        public void GenRandomFlashSaleChoices()
        {
            //clear list data
            idFlashSaleChoices.Values = new List<int>();

            var flashSaleDatas = GetAllFlashSaleDatas();
            for (int i = 0; i < 6; i++)
            {
                var count = flashSaleDatas.Count;
                var ids = new int[count];
                var chances = new float[count];
                for (int j = 0; j < count; j++)
                {
                    var flashSaleData = flashSaleDatas[j];
                    ids[j] = flashSaleData.Id;
                    chances[j] = flashSaleData.Rate;
                }

                var choice = LogicAPI.CalcRandomWithChances(chances);
                var id = ids[choice];

                //add to list
                idFlashSaleChoices.Add(id);
                //set value and cost
                var flashSaleDataChoice = flashSaleDatas[choice];
                flashSaleDataChoice.Init(Config.EasyRandom(0, 2) == 0, Config.EasyRandom(0, flashSaleDataChoice.rewardValuesLength));
                flashSaleDatas.RemoveAt(choice);
            }
        }

        public string GetTimeRefresh()
        {
            var s = timerRefresh.RemainSeconds;
            return TimeHelper.FormatHHMMss(s, true);
        }

        private void TimeRefreshEnd(TimerTask timer, long remain)
        {
            if (remain <= 0)
            {
                RefreshFlashSale();

                EventDispatcher.Raise(new RefreshFlashSaleEvent());
            }
        }

        public void CheckRefreshTime()
        {
            if (!timerRefresh.IsRunning)
            {
                RefreshFlashSale();

                EventDispatcher.Raise(new RefreshFlashSaleEvent());
            }
        }

        private void RefreshFlashSale()
        {
            GenRandomFlashSaleChoices();
            //trường hợp trả coin refresh nên phải kiểm tra
            if (!timerRefresh.IsRunning) timerRefresh.Start(TIME_REFRESH);
            EventDispatcher.Raise(new FlashSaleRefreshEvent());
        }

        public void F5FlashSale()
        {
            if (timerRefresh.IsRunning)
            {
                timerRefresh.Stop();
            }

            RefreshFlashSale();
        }

        public void F5Market()
        {
            var marketItemDatas = GetAllMarketItemDatas();
            var count = marketItemDatas.Count;
            for (int i = 0; i < count; i++)
            {
                marketItemDatas[i].Clear();
            }
        }

        public bool InAppPopupIsShow(int id)
        {
            if (inappPopupWasShowed.Contain(id)) return true; return false;
        }
        public void InAppPopupUpdateShow(int id, bool isShow)
        {
            if (isShow)
            {
                if (!inappPopupWasShowed.Contain(id)) inappPopupWasShowed.Add(id);
            }
            else
            {
                if (inappPopupWasShowed.Contain(id)) inappPopupWasShowed.Remove(id);
            }
        }
        public void InAppPopupRequestShow(int id)
        {
            Debug.LogError("not support");
            inappPopupToShow.Add(id);
        }
        public int InAppPopupGetCurrentShow()
        {
            Debug.LogError("not support");
            if (inappPopupToShow == null || inappPopupToShow.Count <= 0) return -1;
            return inappPopupToShow[inappPopupToShow.Count - 1];
        }
        #endregion

        //==============================================

        #region Private

        private void InitSpecialPackages()
        {
            //Read json data from resouce file
            var dataContent = GameData.GetTextContent("Data/Special");
            //Parse json data to list objects
            var collection = JsonHelper.GetJsonList<PackageDefinition>(dataContent);
            if (collection != null)
            {
                // collection.Sort();
                foreach (var item in collection)
                {
                    //Declare unit data, then push it into a data group
                    var data = new PackageData(item.id, item);
                    specialPacksGroup.AddData(data);
                }
            }

            //Debug.Log(collection);
        }

        private void InitPremiumPackages()
        {
            //Read json data from resouce file
            var dataContent = GameData.GetTextContent("Data/Premium");
            //Parse json data to list objects
            var collection = JsonHelper.GetJsonList<PremiumPackage>(dataContent);
            if (collection != null)
            {
                // collection.Sort();
                foreach (var item in collection)
                {
                    //Declare unit data, then push it into a data group
                    var data = new PremiumPackageData(item.id, item);
                    premiumPacksGroup.AddData(data);
                }
            }

            //Debug.Log(collection);
        }

        private void InitGemPackages()
        {
            //Read json data from resouce file
            var dataContent = GameData.GetTextContent("Data/Gem");
            //Parse json data to list objects
            var collection = JsonHelper.GetJsonList<PackageDefinition>(dataContent);
            if (collection != null)
            {
                // collection.Sort();
                foreach (var item in collection)
                {
                    //Declare unit data, then push it into a data group
                    var data = new PackageData(item.id, item);
                    gemPacksGroup.AddData(data);
                }
            }

            //Debug.Log(collection);
        }

        private void InitBattlePackages()
        {
            //Read json data from resouce file
            var dataContent = GameData.GetTextContent("Data/Battle");
            //Parse json data to list objects
            var collection = JsonHelper.GetJsonList<PackageDefinition>(dataContent);
            if (collection != null)
            {
                // collection.Sort();
                foreach (var item in collection)
                {
                    //Declare unit data, then push it into a data group
                    var data = new PackageData(item.id, item);
                    battlePacksGroup.AddData(data);
                }
            }

            //Debug.Log(collection);
        }

        private void InitDealPackages()
        {
            //Read json data from resouce file
            var dataContent = GameData.GetTextContent("Data/Deal");
            //Parse json data to list objects
            var collection = JsonHelper.GetJsonList<PackageDefinition>(dataContent);
            if (collection != null)
            {
                // collection.Sort();
                foreach (var item in collection)
                {
                    //Declare unit data, then push it into a data group
                    var data = new PackageData(item.id, item);
                    dealPacksGroup.AddData(data);
                }
            }

            //Debug.Log(collection);
        }

        private void InitFlashSales()
        {
            //Read json data from resouce file
            var dataContent = GameData.GetTextContent("Data/FlashSale");
            //Parse json data to list objects
            var collection = JsonHelper.GetJsonList<FlashSaleDefinition>(dataContent);
            if (collection != null)
            {
                // collection.Sort();
                foreach (var item in collection)
                {
                    //Declare unit data, then push it into a data group
                    var data = new FlashSaleData(item.id, item);
                    flashSalesGroup.AddData(data);
                }
            }

            //Debug.Log(collection);
        }

        private void InitMarketItems()
        {
            //Read json data from resouce file
            var dataContent = GameData.GetTextContent("Data/MarketPlace");
            //Parse json data to list objects
            var collection = JsonHelper.GetJsonList<MarketItem>(dataContent);
            if (collection != null)
            {
                // collection.Sort();
                foreach (var item in collection)
                {
                    //Declare unit data, then push it into a data group
                    //if (item.id >= 5) break;
                    var data = new MarketItemData(item.id, item);
                    marketItemsGroup.AddData(data);
                }
            }

            //Debug.Log(collection);
        }

        private void InitPremiumPass()
        {
            //Read json data from resouce file
            var dataContent = GameData.GetTextContent("Data/PremiumPass");
            //Parse json data to list objects
            var collection = JsonHelper.GetJsonList<PackageDefinition>(dataContent);
            if (collection != null)
            {
                //chỉ có 1 premium pass
                var packageDefinition = collection[0];
                premiumPass = AddData(new PackageData(PREMIUM_PASS_ID, packageDefinition));
            }

            //Debug.Log(collection);
        }

        private void InitFreeDroneItems()
        {
            //Read json data from resouce file
            var dataContent = GameData.GetTextContent("Data/FreeDrone");
            //Parse json data to list objects
            freeDroneItems = JsonHelper.GetJsonList<FreeDroneItem>(dataContent);
        }

        private void InitPremiumDroneItems()
        {
            //Read json data from resouce file
            var dataContent = GameData.GetTextContent("Data/PremiumDrone");
            //Parse json data to list objects
            premiumDroneItems = JsonHelper.GetJsonList<PremiumDroneItem>(dataContent);
        }

        #endregion
    }
}