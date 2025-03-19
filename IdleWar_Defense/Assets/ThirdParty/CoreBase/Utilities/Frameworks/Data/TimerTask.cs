using System;
using Utilities.Common;

namespace Utilities.Pattern.Data
{
    public class TimerTask : DataGroup
    {
        private Action<TimerTask, long> mOnCompleted;

        private LongData mRemainSeconds;
        private LongData mLocalSeconds;
        private LongData mServerRemainSeconds;
        private LongData mServerSeconds;
        private LongData mTaskDurationSeconds;
        private bool mOnlineOnly;

        public long RemainSeconds { get { return mRemainSeconds.Value; } }
        public long Seconds { get { return mLocalSeconds.Value; } }
        public long TaskDurationSeconds { get { return mTaskDurationSeconds.Value; } }
        public bool IsRunning { get { return mRemainSeconds.Value > 0; } }

        public TimerTask(int pId, bool pOnlineOnly) : base(pId)
        {
            mRemainSeconds = AddData(new LongData(0));
            mLocalSeconds = AddData(new LongData(1));
            mServerRemainSeconds = AddData(new LongData(2));
            mServerSeconds = AddData(new LongData(3));
            mTaskDurationSeconds = AddData(new LongData(4));
            mOnlineOnly = pOnlineOnly;
        }

        public override void Load(string pBaseKey, string pSaverIdString)
        {
            base.Load(pBaseKey, pSaverIdString);

            if (IsRunning)
                TimerTaskManager.instance.AddTimerTask(this);
        }

        public void SetOnComplete(Action<TimerTask, long> pAction)
        {
            mOnCompleted = pAction;
        }

        private void Finished()
        {
            var remainSeconds = mRemainSeconds.Value;

            Stop();

            if (mOnCompleted != null)
                mOnCompleted(this, remainSeconds);
        }

        public void Stop()
        {
            mRemainSeconds.Value = 0;
            mLocalSeconds.Value = 0;
            mServerRemainSeconds.Value = 0;
            mServerSeconds.Value = 0;
        }

        public void PassSeconds(long pPassSeconds)
        {
            if (!IsRunning)
                return;

            mRemainSeconds.Value -= pPassSeconds;
            mServerRemainSeconds.Value -= pPassSeconds;

            if (!IsRunning)
                Finished();
        }

        public void AddSeconds(long pSeconds)
        {
            if (IsRunning)
            {
                mRemainSeconds.Value += pSeconds;
                mServerRemainSeconds.Value += pSeconds;
            }
        }

        public void Start(long pSecondsDurations)
        {
            var timeManager = TimerTaskManager.instance;
            long curServerSeconds = timeManager.GetCurrentServerSeconds();
            long curLocalSeconds = timeManager.GetSecondsSinceBoot();
            long seconds = pSecondsDurations;
            mTaskDurationSeconds.Value = seconds;
            mRemainSeconds.Value = seconds;

            if (mOnlineOnly)
            {
                mServerRemainSeconds.Value = 0;
                mServerSeconds.Value = 0;
                mLocalSeconds.Value = 0;
            }
            else
            {
                mServerRemainSeconds.Value = seconds;
                mServerSeconds.Value = curServerSeconds;
                mLocalSeconds.Value = curLocalSeconds;
            }

            TimerTaskManager.instance.AddTimerTask(this);
        }

        public void Update(long pCurServerSeconds, long pCurLocalSeconds, long pDeltaOnlineSeconds)
        {
            if (IsRunning)
            {
                if (mOnlineOnly)
                    mRemainSeconds.Value -= pDeltaOnlineSeconds;
                else
                {
                    if (pCurServerSeconds > 0)
                    {
                        //If server time was saved, count directly, otherwise record it
                        long dServerSeconds = pCurServerSeconds - mServerSeconds.Value;// this value should never be negative
                        if (mServerSeconds.Value > 0 && dServerSeconds > 0)
                        {
                            mServerRemainSeconds.Value -= dServerSeconds;
                            mServerSeconds.Value = pCurServerSeconds;

                            mLocalSeconds.Value = pCurLocalSeconds;
                            mRemainSeconds.Value = mServerRemainSeconds.Value;
                        }
                        else
                        {
                            mServerSeconds.Value = pCurServerSeconds;
                            mServerRemainSeconds.Value = mRemainSeconds.Value;
                        }
                    }
                    //
                    if (mLocalSeconds.Value <= 0)
                    {
                        mLocalSeconds.Value = pCurLocalSeconds;
                    }
                    //
                    long dt = pCurLocalSeconds - mLocalSeconds.Value;
                    //
                    if (dt < 0)
                    {
                        // means user turn off device then switch on
                        dt = pCurLocalSeconds;
                    }

                    if (dt > 0)
                    {
                        mLocalSeconds.Value = pCurLocalSeconds;
                        mRemainSeconds.Value -= dt;
                    }
                }
                // check if finished
                if (!IsRunning)
                    Finished();
            }
        }
    }
}