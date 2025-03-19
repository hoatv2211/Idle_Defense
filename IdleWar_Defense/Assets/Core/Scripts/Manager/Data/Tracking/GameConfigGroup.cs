
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Common;
using Utilities.Pattern.Data;

namespace FoodZombie
{
    public class GameConfigGroup : DataGroup
    {
        private const int SECONDS_PER_DAY = 24 * 60 * 60;

        #region Members

        public Action<bool> onEnableMuisic;
        public Action<bool> onEnableSFX;
        public Action<bool> onEnableVibration;

        private BoolData mNoAds;
        private StringData mDisplayName;
        private BoolData mEnableSFX;
        private BoolData mEnableMusic;
        private BoolData mEnableVibration;
        private BoolData mEnableNotification;
        private BoolData mEnableHint;
        private ListData<string> mSentEvents;
        /// <summary>
        /// This is GPGS user id
        /// </summary>
        private StringData mStorageUserId;
        private StringData mStorageUserName;
        private BoolData mRated;
        private StringData mLastTimeRestore;
        private IntegerData mCountShowRate;
        private IntegerData mLastDay;//old
        private BoolData mShowedDailyRate;

        private LongData mLastUnitTime;//new
        private IntegerData mLastSecondInDay;//new

        private BoolData mEnableAutoTarget;
        private BoolData mEnableX2Speed;
        private BoolData mEnableX3Speed;

        public bool EnableSFX => mEnableSFX.Value;
        public bool EnableMusic => mEnableMusic.Value;
        public bool EnableVibration => mEnableVibration.Value;
        public bool EnableNotification => mEnableNotification.Value;
        public bool EnableHint => mEnableHint.Value;
        public string DisplayName => mDisplayName.Value;
        public List<string> SentEvents => mSentEvents.GetValues();
        public string StorageUserId => mStorageUserId.Value;
        public string StorageUserName => mStorageUserName.Value;
        public bool Rated => mRated.Value;
        public int CountShowRate => mCountShowRate.Value;
        public bool ShowedDailyRate => mShowedDailyRate.Value;
        public bool NoAds => mNoAds.Value;
        public bool EnableAutoTarget => mEnableAutoTarget.Value;
        public bool EnableX2Speed => mEnableX2Speed.Value;
        public bool EnableX3Speed => mEnableX3Speed.Value;

        public DateTime LastTimeRestore
        {
            get
            {
                if (string.IsNullOrEmpty(mLastTimeRestore.Value))
                    return DateTime.MinValue;
                else
                {
                    //DateTime time = DateTime.Now;
                    DateTime time = UnbiasedTime.Instance.Now(); //Fix Cheat Time
                    if (DateTime.TryParse(mLastTimeRestore.Value, out time))
                        return time;
                    else
                        return DateTime.MinValue;
                }
            }
            set
            {
                mLastTimeRestore.Value = (value.ToString());
            }
        }

        #endregion

        //==============================================

        #region Public

        public GameConfigGroup(int pId) : base(pId)
        {
            mEnableSFX = AddData(new BoolData(1, true));
            mEnableMusic = AddData(new BoolData(2, true));
            mEnableNotification = AddData(new BoolData(3, true));
            mEnableHint = AddData(new BoolData(4, true));
            mDisplayName = AddData(new StringData(5));
            mSentEvents = AddData(new ListData<string>(6, new List<string>()));
            mStorageUserId = AddData(new StringData(7));
            mStorageUserName = AddData(new StringData(8));
            mRated = AddData(new BoolData(9));
            mLastTimeRestore = AddData(new StringData(10));
            mCountShowRate = AddData(new IntegerData(11));
            mLastDay = AddData(new IntegerData(12));
            mShowedDailyRate = AddData(new BoolData(13));
            mEnableVibration = AddData(new BoolData(14, true));
            mNoAds = AddData(new BoolData(15, false));
            mEnableAutoTarget = AddData(new BoolData(16, false));
            mEnableX2Speed = AddData(new BoolData(17, false));
            mEnableX3Speed = AddData(new BoolData(18, false));
            //new
            mLastUnitTime = AddData(new LongData(19, 0));
            mLastSecondInDay = AddData(new IntegerData(20, 0));
        }

        public override void PostLoad()
        {
            base.PostLoad();

            //from ver 0.2.5
            if (mEnableX2Speed.Value && mEnableX3Speed.Value)
            {
                mEnableX2Speed.Value = true;
                mEnableX3Speed.Value = false;
            }

            if (mLastUnitTime.Value == 0)
            {
                long timeStamp = (long)(UnbiasedTime.Instance.UtcNow().Subtract(new DateTime(2019, 1, 1))).TotalSeconds;
                mLastUnitTime.Value = timeStamp;

                //7h sáng mới reset day, thì một ngày có 24h, 24 - 7 = 17
                //int currentSecondInDay = (int)(DateTime.Now - DateTime.Today.Subtract(new TimeSpan(0, 17, 0, 0))).TotalSeconds;
                //Fix Cheat Time
                int currentSecondInDay = (int)(UnbiasedTime.Instance.Now() - UnbiasedTime.Instance.Today().Subtract(new TimeSpan(0, 17, 0, 0))).TotalSeconds;
                currentSecondInDay = currentSecondInDay % SECONDS_PER_DAY;
                mLastSecondInDay.Value = currentSecondInDay;

                //send new day
                GameData.Instance.MissionsGroup.NewDay();
                GameData.Instance.DailyQuestsGroup.NewDay();
                GameData.Instance.HeroesGroup.NewDay();
                GameData.Instance.WheelData.NewDay();
                GameData.Instance.DiscoveriesGroup.NewDay();
                GameData.Instance.UserGroup.NewDay();
                GameData.Instance.StoreGroup.NewDay();
                // mLastDay.Value = day;
                mShowedDailyRate.Value = false;
            }

            CheckNewDay();
        }

        public void SetEnableHint(bool pValue)
        {
            mEnableHint.Value = (pValue);
        }

        public void SetEnableMusic(bool pValue)
        {
            mEnableMusic.Value = (pValue);
            onEnableMuisic.Raise(pValue);
        }

        public void SetEnableNotification(bool pValue)
        {
            mEnableNotification.Value = (pValue);
        }

        public void SetEnableSFX(bool pValue)
        {
            mEnableSFX.Value = (pValue);
            onEnableSFX.Raise(pValue);
        }

        public void SetEnableVibration(bool pValue)
        {
            mEnableVibration.Value = pValue;
            onEnableVibration.Raise(pValue);
        }

        public void SetDisplayName(string pValue)
        {
            mDisplayName.Value = (pValue);
        }

        public void AddSentEvent(string pEventName)
        {
            SentEvents.Add(pEventName);
        }

        public void SetNoAds(bool pValue)
        {
            mNoAds.Value = pValue;
        }

        public void SetAutoTarget(bool pValue)
        {
            mEnableAutoTarget.Value = pValue;
        }

        public void SetX2Speed(bool pValue)
        {
            mEnableX2Speed.Value = pValue;
        }

        public void SetX3Speed(bool pValue)
        {
            mEnableX3Speed.Value = pValue;
        }

        /// <summary>
        /// NOTE: only call this method when
        /// Upload or backup storage
        /// DO NOT CALL IT WHEN LOGIN GPGS
        /// </summary>
        public void SetStorageAccount(string puserId, string pUserName)
        {
            mStorageUserId.Value = (puserId);
            mStorageUserName.Value = (pUserName);
        }

        public void Rate()
        {
            mRated.Value = (true);
        }

        public void AddCountShowRate()
        {
            mShowedDailyRate.Value = (true);
            mCountShowRate.Value += (1);
        }

        bool isPause = false;
        bool isMusicOnWhenPause = false;
        float timeScaleWhenPause = 1;
        public void PauseGame()
        {
#if UNITY_ANDROID
            return;
#endif

            isPause = true;
            isMusicOnWhenPause = EnableMusic;
            timeScaleWhenPause = Time.timeScale;

            Time.timeScale = 0;
            SetEnableMusic(false);
        }
        public void UnPauseGame()
        {
#if UNITY_ANDROID
            return;
#endif
            isPause = false;
            Time.timeScale = timeScaleWhenPause;
            SetEnableMusic(isMusicOnWhenPause);
        }

        #endregion

        //=============================================

        #region Private

        public void CheckNewDay()
        {
            // int day = DateTime.Now.DayOfYear;

            long timeStamp = (long)(UnbiasedTime.Instance.UtcNow().Subtract(new DateTime(2019, 1, 1))).TotalSeconds;

            //7h sáng mới reset day, thì một ngày có 24h, 24 - 7 = 17
            //int currentSecondInDay = (int)(DateTime.Now - DateTime.Today.Subtract(new TimeSpan(0, 17, 0, 0))).TotalSeconds;
            //Fix Cheat Time
            int currentSecondInDay = (int)(UnbiasedTime.Instance.Now() - UnbiasedTime.Instance.Today().Subtract(new TimeSpan(0, 17, 0, 0))).TotalSeconds;

            currentSecondInDay = currentSecondInDay % SECONDS_PER_DAY;
            if (currentSecondInDay < mLastSecondInDay.Value
               || timeStamp >= mLastUnitTime.Value + SECONDS_PER_DAY)
            {
                mLastUnitTime.Value = timeStamp;
                mLastSecondInDay.Value = currentSecondInDay;

                GameData.Instance.MissionsGroup.NewDay();
                GameData.Instance.DailyQuestsGroup.NewDay();
                GameData.Instance.HeroesGroup.NewDay();
                GameData.Instance.WheelData.NewDay();
                GameData.Instance.DiscoveriesGroup.NewDay();
                GameData.Instance.UserGroup.NewDay();
                GameData.Instance.StoreGroup.NewDay();
                // mLastDay.Value = day;
                mShowedDailyRate.Value = false;
            }
        }

        public int GetDayOfWeek()
        {
            //return 2 tức là thứ 2
            //var dayOfWeek = (int)DateTime.Now.DayOfWeek;
            //Fix Cheat Time
            var dayOfWeek = (int)UnbiasedTime.Instance.Now().DayOfWeek;
            if (dayOfWeek != 0) dayOfWeek++;

            int currentSecondInDay = (int)(UnbiasedTime.Instance.Now() - UnbiasedTime.Instance.Today()).TotalSeconds;
            if (currentSecondInDay < 7 * 60 * 60)
            {
                dayOfWeek--;
                if (dayOfWeek == 1) dayOfWeek = 0;
                else if (dayOfWeek < 0) dayOfWeek = 7;
            }

            return dayOfWeek;
        }

        #endregion
    }
}