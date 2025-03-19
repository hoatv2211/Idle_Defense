using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using UnityEngine;
using Utilities.Common;
using Utilities.Pattern.Data;
using Utilities.Service.RFirebase;

namespace FoodZombie
{
    public class CurrencyData : DataGroup
    {
        private IntegerData mValue;
        private bool mOnlineOnly;
        private DateTime? mStartDate;
        private DateTime? mExpiredDate;
        private int mMax = -1;

        public CurrencyData(int pId, int pDefault, int pMax = -1, bool pOnlineOnly = false, DateTime? pStartDate = null, DateTime? pEndDate = null) : base(pId)
        {
            mValue = AddData(new IntegerData(0, pDefault));
            mMax = pMax;
            mOnlineOnly = pOnlineOnly;
            mStartDate = pStartDate;
            mExpiredDate = pEndDate;
        }

        public CurrencyData(int pId, int pDefault, int pMax = -1) : base(pId)
        {
            mValue = AddData(new IntegerData(0, pDefault));
            mMax = pMax;
            mOnlineOnly = false;
            mStartDate = null;
            mExpiredDate = null;
        }

        public void Add(int pValue, bool limit)
        {
            if (pValue == 0)
                return;

            if (limit && mMax > 0 && mValue.Value + pValue >= mMax)
                mValue.Value = mMax;
            else
                mValue.Value += pValue;

            EventDispatcher.Raise(new CurrencyChangedEvent(Id, pValue));
        }

        public bool Pay(int pValue)
        {
            if (mValue.Value < pValue)
                return false;

            Add(-pValue, false);
            return true;
        }

        public bool CanPay(int pValue)
        {
            return mValue.Value >= pValue;
        }

        public int GetValue()
        {
            if (mOnlineOnly)
            {
                var now = GetNow();
                if (now != null && ((mStartDate == null && mExpiredDate == null) || (now.Value > mStartDate.Value && now < mExpiredDate.Value)))
                    return mValue.Value;
                else
                    return 0;
            }
            else
            {
                return mValue.Value;
            }
        }

        public DateTime? GetNow()
        {
            //TODO: Call get server time here
            var utcNow = DateTime.UtcNow;
            var vnNow = utcNow.AddHours(7);
            return vnNow;
        }

        /// <summary>
        /// Get Config from server and set to this
        /// </summary>
        public void SetDate(DateTime pStartDate, DateTime pEndDate)
        {
            mStartDate = pStartDate;
            mExpiredDate = pEndDate;
        }
    }

    public class CurrenciesGroup : DataGroup
    {
        // private int total_collect_coin = 0;
        // private int total_collect_gem = 0;
        // private int total_use_coin = 0;
        // private int total_use_gem = 0;

        public Dictionary<int, CurrencyData> currencies;

        public CurrenciesGroup(int pId) : base(pId)
        {
            currencies = new Dictionary<int, CurrencyData>();
            currencies.Add(IDs.CURRENCY_COIN, AddData(new CurrencyData(IDs.CURRENCY_COIN, 0, -1))); //-1 là không có giới hạn
            currencies.Add(IDs.CURRENCY_GEM, AddData(new CurrencyData(IDs.CURRENCY_GEM, 0, -1)));
            currencies.Add(IDs.CURRENCY_EXP_HERO, AddData(new CurrencyData(IDs.CURRENCY_EXP_HERO, 0, -1)));
            currencies.Add(IDs.CURRENCY_DUST_ELECTRIC, AddData(new CurrencyData(IDs.CURRENCY_DUST_ELECTRIC, 0, -1)));
            currencies.Add(IDs.CURRENCY_DUST_METALIC, AddData(new CurrencyData(IDs.CURRENCY_DUST_METALIC, 0, -1)));
            currencies.Add(IDs.CURRENCY_DUST_NITROGEN, AddData(new CurrencyData(IDs.CURRENCY_DUST_NITROGEN, 0, -1)));
            currencies.Add(IDs.CURRENCY_DUST_LAVA, AddData(new CurrencyData(IDs.CURRENCY_DUST_LAVA, 0, -1)));
            currencies.Add(IDs.CURRENCY_MATERIAL, AddData(new CurrencyData(IDs.CURRENCY_MATERIAL, 0, -1)));
            currencies.Add(IDs.CURRENCY_TICKET, AddData(new CurrencyData(IDs.CURRENCY_TICKET, 10, -1)));
            currencies.Add(IDs.CURRENCY_TICKET_PVP, AddData(new CurrencyData(IDs.CURRENCY_TICKET_PVP, 0, -1)));
            currencies.Add(IDs.CURRENCY_HONOR, AddData(new CurrencyData(IDs.CURRENCY_HONOR, 0, -1)));

            currencies.Add(IDs.CURRENCY_POWER_FRAGMENT, AddData(new CurrencyData(IDs.CURRENCY_POWER_FRAGMENT, 0, -1)));
            currencies.Add(IDs.CURRENCY_POWER_CRYSTAL, AddData(new CurrencyData(IDs.CURRENCY_POWER_CRYSTAL, 0, -1)));
            currencies.Add(IDs.CURRENCY_DEVINE_CRYSTAL, AddData(new CurrencyData(IDs.CURRENCY_DEVINE_CRYSTAL, 0, -1)));
            currencies.Add(IDs.CURRENCY_BLUE_CHIP, AddData(new CurrencyData(IDs.CURRENCY_BLUE_CHIP, 2, -1)));
            currencies.Add(IDs.CURRENCY_GOLDEN_CHIP, AddData(new CurrencyData(IDs.CURRENCY_GOLDEN_CHIP, 0, -1)));
            currencies.Add(IDs.CURRENCY_BLUE_HERO_FRAGMENT, AddData(new CurrencyData(IDs.CURRENCY_BLUE_HERO_FRAGMENT, 10, -1)));
            currencies.Add(IDs.CURRENCY_EPIC_HERO_FRAGMENT, AddData(new CurrencyData(IDs.CURRENCY_EPIC_HERO_FRAGMENT, 10, -1)));
        }

        public override void PostLoad()
        {
            base.PostLoad();
        }

        public bool CanPay(int pType, int pValue)
        {
            if (currencies.ContainsKey(pType))
                return currencies[pType].CanPay(pValue);
            else
                return false;
        }

        public bool Pay(int pType, int pValue, string source = TrackingConstants.VALUE_EMPTY)
        {
            if (currencies.ContainsKey(pType))
            {
                bool paid = currencies[pType].Pay(pValue);
                //tracking
                if (paid)
                {
                    if (pType == IDs.CURRENCY_COIN)
                    {
                        // total_use_coin += pValue;
                        // UnityEngine.Debug.Log("total_collect_coin " + total_collect_coin + "--------------" + "total_use_coin " + total_use_coin);
                        if (pValue >= 0)
                            Config.LogEvent(TrackingConstants.USE_COIN, TrackingConstants.PARAM_SOURCE, source, TrackingConstants.PARAM_AMOUNT, pValue);
                    }
                    else if (pType == IDs.CURRENCY_GEM)
                    {
                        // total_use_gem += pValue;
                        // UnityEngine.Debug.Log("total_collect_gem " + total_collect_gem + "--------------" + "total_use_gem " + total_use_gem);
                        if (pValue >= 0)
                            Config.LogEvent(TrackingConstants.USE_GEM, TrackingConstants.PARAM_SOURCE, source, TrackingConstants.PARAM_AMOUNT, pValue);
                    }
                }
                //  if (currencies[pType].GetValue()<0) currencies[pType].va
                return paid;
            }
            else
                return false;
        }

        public void Add(int pType, int pValue, string source, bool limit = false)
        {
            if (currencies.ContainsKey(pType))
                currencies[pType].Add(pValue, limit);

            //tracking
            if (pValue > 0)
            {
                if (pType == IDs.CURRENCY_COIN)
                {
                    // total_collect_coin += pValue;
                    // UnityEngine.Debug.Log("total_collect_coin " + total_collect_coin + "--------------" + "total_use_coin " + total_use_coin);
                    Config.LogEvent(TrackingConstants.COLLECT_COIN, TrackingConstants.PARAM_SOURCE, source, TrackingConstants.PARAM_AMOUNT, pValue);
                }
                else if (pType == IDs.CURRENCY_GEM)
                {
                    // total_collect_gem += pValue;
                    // UnityEngine.Debug.Log("total_collect_gem " + total_collect_gem + "--------------" + "total_use_gem " + total_use_gem);
                    Config.LogEvent(TrackingConstants.COLLECT_GEM, TrackingConstants.PARAM_SOURCE, source, TrackingConstants.PARAM_AMOUNT, pValue);
                }
            }
        }

        public void SetDate(int pType, DateTime pStartDate, DateTime pEndDate)
        {
            if (currencies.ContainsKey(pType))
                currencies[pType].SetDate(pStartDate, pEndDate);
        }

        public int GetValue(int pType)
        {
            if (currencies.ContainsKey(pType))
                return currencies[pType].GetValue();
            else
                return 0;
        }

        public int GetCoin()
        {
            return currencies[IDs.CURRENCY_COIN].GetValue();
        }

        public int GetGem()
        {
            return currencies[IDs.CURRENCY_GEM].GetValue();
        }

        public int GetExpHero()
        {
            return currencies[IDs.CURRENCY_EXP_HERO].GetValue();
        }

        public static int GetCurrencyIDFromElementID(int elementId)
        {
            switch (elementId)
            {
                case IDs.ELEMENT_ELECTRIC:
                    return IDs.CURRENCY_DUST_ELECTRIC;
                case IDs.ELEMENT_METALIC:
                    return IDs.CURRENCY_DUST_METALIC;
                case IDs.ELEMENT_NITROGEN:
                    return IDs.CURRENCY_DUST_NITROGEN;
                case IDs.ELEMENT_LAVA:
                    return IDs.CURRENCY_DUST_LAVA;
            }
            return IDs.CURRENCY_DUST_ELECTRIC;
        }
    }
}