using System;
using Utilities.Common;

namespace Utilities.Pattern.Data
{
    /// <summary>
    /// Note: this can not not prevent time cheating
    /// </summary>
    public class TimeCounterData : DataGroup
    {
        private Action<double> mOnFinished;
        private DateTimeData mLocalStart;
        private DateTimeData mServerStart;
        private LongData mDuration;
        private WaitUtil.CountdownEvent mCounter;

        public TimeCounterData(int pId) : base(pId)
        {
            mLocalStart = AddData(new DateTimeData(0, null));
            mServerStart = AddData(new DateTimeData(1, null));
            mDuration = AddData(new LongData(2));

            TimeHelper.CheckServerTime(null);
        }

        public override void PostLoad()
        {
            base.PostLoad();

            if (IsRunning())
                Register();
        }

        public void SetListener(Action<double> pOnFinished)
        {
            mOnFinished = pOnFinished;
        }

        public double GetRemainSeconds()
        {
            var now = TimeHelper.GetServerTime();
            if (now != null && mServerStart.Value == null)
            {
                double offset = 0;
                if (mLocalStart.Value != null)
                    offset = (DateTime.Now - mLocalStart.Value.Value).TotalSeconds;
                mServerStart.Value = now.Value.AddSeconds(-offset);
                var endTime = mServerStart.Value.Value.AddSeconds(mDuration.Value);

                return (endTime - now.Value).TotalSeconds;
            }
            else if (now != null && mServerStart.Value != null)
            {
                var startTime = mServerStart.Value;
                var endTime = startTime.Value.AddSeconds(mDuration.Value);
                return (endTime - now.Value).TotalSeconds;
            }
            else
            {
                var startTime = mLocalStart.Value;
                var endTime = startTime.Value.AddSeconds(mDuration.Value);
                return (endTime - DateTime.Now).TotalSeconds;
            }
        }

        public void Start(int pSeconds)
        {
            mDuration.Value = pSeconds;
            mLocalStart.Value = DateTime.Now;

            var now = TimeHelper.GetServerTime();
            if (now != null)
                mServerStart.Value = now;

            Register();
        }

        public void Stop()
        {
            if (mCounter != null)
                WaitUtil.RemoveCountdownEvent(mCounter);
        }

        public void Finish()
        {
            if (mCounter != null)
                WaitUtil.RemoveCountdownEvent(mCounter);

            if (mOnFinished != null)
                mOnFinished(GetRemainSeconds());
        }

        private void Register()
        {
            if (mCounter != null)
                WaitUtil.RemoveCountdownEvent(mCounter);

            mCounter = WaitUtil.Start(new WaitUtil.CountdownEvent()
            {
                waitTime = (float)GetRemainSeconds(),
                unscaledTime = true,
                doSomething = (pass) =>
                {
                    if (mOnFinished != null)
                        mOnFinished(GetRemainSeconds());
                }
            });
        }

        public bool IsRunning()
        {
            if (mLocalStart.Value == null)
                return false;
            else
                return GetRemainSeconds() > 0;
        }

        public override void OnApplicationPaused(bool pPaused)
        {
            base.OnApplicationPaused(pPaused);

            TimeHelper.CheckServerTime(null);
        }
    }
}