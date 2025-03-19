using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Utilities.Common;
using Debug = UnityEngine.Debug;

namespace Utilities.Pattern.Data
{
    public class TimerTaskManager : IUpdate
    {
        #region Constants

        public const int MONTHS_PER_YEAR = 12;
        public const int DAYS_PER_WEEK = 7;
        public const int DAYS_PER_MONTH = 30;
        public const int HOURS_PER_DAY = 24;
        public const int MINUTES_PER_HOUR = 60;

        public const int MILLISECONDS_PER_SECOND = 1000;
        public const int MICROSECONDS_PER_SECOND = 1000 * 1000;
        public const long NANOSECONDS_PER_SECOND = 1000 * 1000 * 1000;

        public const long MICROSECONDS_PER_MILLISECOND = MICROSECONDS_PER_SECOND / MILLISECONDS_PER_SECOND;

        public const long NANOSECONDS_PER_MICROSECOND = NANOSECONDS_PER_SECOND / MICROSECONDS_PER_SECOND;
        public const long NANOSECONDS_PER_MILLISECOND = NANOSECONDS_PER_SECOND / MILLISECONDS_PER_SECOND;

        public const float SECONDS_PER_NANOSECOND = 1f / NANOSECONDS_PER_SECOND;
        public const float MICROSECONDS_PER_NANOSECOND = 1f / NANOSECONDS_PER_MICROSECOND;
        public const float MILLISECONDS_PER_NANOSECOND = 1f / NANOSECONDS_PER_MILLISECOND;

        public const float SECONDS_PER_MICROSECOND = 1f / MICROSECONDS_PER_SECOND;
        public const float MILLISECONDS_PER_MICROSECOND = 1f / MICROSECONDS_PER_MILLISECOND;

        public const float SECONDS_PER_MILLISECOND = 1f / MILLISECONDS_PER_SECOND;

        public const int SECONDS_PER_MINUTE = 60;
        public const int SECONDS_PER_HOUR = SECONDS_PER_MINUTE * MINUTES_PER_HOUR;
        public const int SECONDS_PER_DAY = SECONDS_PER_HOUR * HOURS_PER_DAY;
        public const int SECONDS_PER_WEEK = SECONDS_PER_DAY * DAYS_PER_WEEK;
        public const int SECONDS_PER_MONTH = SECONDS_PER_DAY * DAYS_PER_MONTH;
        public const int SECONDS_PER_YEAR = SECONDS_PER_MONTH * MONTHS_PER_YEAR;

        #endregion

        //=============================================================

        #region Members

        private static TimerTaskManager mInstance;
        public static TimerTaskManager instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = new TimerTaskManager();
                return mInstance;
            }
        }

        private DateTime mDayZero;
        private bool mLocalTimeSynced;
        private long mLocalTimeOffset;

        private long mServerTimeOffset;
        private bool mTimeServerFetched;
        private bool mFetchingTimeServer;
        private float mSecondsElapsed;

        private List<TimerTask> mTimerTasks;

        public int id { get; set; }

        #endregion

        //=============================================================

        #region Public

        public TimerTaskManager()
        {
            mTimerTasks = new List<TimerTask>();
            mLocalTimeSynced = false;
            mTimeServerFetched = false;
            mDayZero = new DateTime(2017, 1, 1);

            SyncTimers();

            WaitUtil.AddUpdate(this);
        }

        public long GetSecondsSinceBoot()
        {
            return GetMillisSinceBoot() / MILLISECONDS_PER_SECOND;
        }

        public long GetCurrentServerSeconds()
        {
            if (mTimeServerFetched == false)
            {
                SyncTimeServer();
                return 0;
            }
            else
            {
                return (GetLocalMillisSeconds() + mServerTimeOffset) / MILLISECONDS_PER_SECOND;
            }
        }

        public long GetMillisSinceBoot()
        {
            SyncTimeLocal();
            return GetLocalMillisSeconds() + mLocalTimeOffset;
        }

        public void OnApplicationFocus(bool pFocus)
        {
            if (pFocus)
            {
                SyncTimers();
            }
            else
            {
                mLocalTimeSynced = false;
                mTimeServerFetched = false;
            }
        }

        public DateTime GetNow()
        {
            return UnbiasedTime.Instance.Now().AddMilliseconds(mServerTimeOffset);
            //return DateTime.Now.AddMilliseconds(mServerTimeOffset);
        }

        public void SyncTimers()
        {
            SyncTimeLocal();
            SyncTimeServer();
        }

        public void AddTimerTask(TimerTask pTimer)
        {
            mTimerTasks.Add(pTimer);
        }

        public void Update(float pUnscaledDetalTime)
        {
            int count = mTimerTasks.Count;
            if (count > 0)
            {
                mSecondsElapsed += pUnscaledDetalTime;
                if (mSecondsElapsed >= 1.0f)
                {
                    mSecondsElapsed -= 1.0f;
                    long currentServerSeconds = GetCurrentServerSeconds();
                    long currentLocalSeconds = GetSecondsSinceBoot();
                    
                    for (int i = count - 1; i >= 0; i--)
                    {
                        var task = mTimerTasks[i];
                        if (task != null)
                        {
                            if (task.IsRunning)
                                task.Update(currentServerSeconds, currentLocalSeconds, 1);
                            else
                                mTimerTasks.RemoveAt(i);
                        }
                    }
                }
            }
        }

        #endregion

        //================================================================

        #region Private

        private long GetLocalMillisSeconds()
        {
            return (long)(UnbiasedTime.Instance.Now() - mDayZero).TotalMilliseconds;
            //return (long)(DateTime.Now - mDayZero).TotalMilliseconds;
        }

        private void SyncTimeLocal()
        {
            if (mLocalTimeSynced == false)
            {
                mLocalTimeSynced = true;
                mLocalTimeOffset = RNative.getMillisSinceBoot() - GetLocalMillisSeconds();
            }
        }
        
        // // get millis since boot
        // private long GetMillisSinceBootBySystem()
        // {
        //     var ticks = System.Diagnostics.Stopwatch.GetTimestamp();
        //     var uptime = ((double)ticks) / System.Diagnostics.Stopwatch.Frequency;
        //     var uptimeSpan = System.TimeSpan.FromSeconds(uptime);
        //     //
        //     double miliSeconds = uptimeSpan.TotalMilliseconds;
        //     //
        //     
        //     UnityEngine.Debug.Log("GetMillisSinceBootBySystem() " + miliSeconds);
        //
        //     return (long)miliSeconds;
        // }

        private void SyncTimeServer()
        {
            if (!mTimeServerFetched && !mFetchingTimeServer)
            {
                string url = "https://showcase.api.linx.twenty57.net/UnixTime/tounix?date=now";

                //var form = new WWWForm();
                UnityWebRequest.ClearCookieCache();
                var request = UnityWebRequest.Get(url);
                request.SendWebRequest();

                mFetchingTimeServer = true;
                WaitUtil.Start(() => request.isDone,
                    () =>
                    {
                        mFetchingTimeServer = false;
                        if (request.isNetworkError)
                        {
                            //Error
                            mTimeServerFetched = false;
                            mServerTimeOffset = 0;
                        }
                        else
                        {
                            if (request.responseCode == 200) //OK
                            {
                                mTimeServerFetched = true;
                                var text = request.downloadHandler.text;
                                long unixTime = 0;
                                if (long.TryParse(text, out unixTime))
                                {
                                    DateTime dateTime = Config.UnixTimeStampToDateTime(unixTime);
                                    
                                    mServerTimeOffset =
                                        (long) (dateTime - mDayZero).TotalMilliseconds - GetLocalMillisSeconds();
                                }
                            }
                            else
                            {
                                //Error
                                mTimeServerFetched = false;
                                mServerTimeOffset = 0;
                            }
                        }
                    });
            }
        }
        
        #endregion
    }
}