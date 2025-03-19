using System;
using System.Collections.Generic;
using UnityEngine;

namespace FoodZombie
{
    [System.Serializable]
    public class RewardInfo
    {
        private int mType;
        private int mId;
        private int mValue;
        private int mRatio;

        public int Type
        {
            get
            {
                return mType;
            }
        }
        public int Id { get { return mId; } }
        public int Value
        {
            get
            {
                return mValue;
            }
        }
        public int Ratio { get { return mRatio; } }

        public RewardInfo(int pType, int pId, int pValue, int pRatio = 100)
        {
            mType = pType;
            mId = pId;
            mValue = pValue;
            mRatio = pRatio;
        }

        public Sprite GetIcon()
        {
            return LogicAPI.GetRewardIcon(mType, mId, mValue);
        }

        public string GetName()
        {
            return LogicAPI.GetRewardName(mType, mId);
        }

        public string GetShortDescription()
        {
            return LogicAPI.GetShortDescription(mType, mId, mValue);
        }

        public string GetDescription()
        {
            return LogicAPI.GetDescription(mType, mId, mValue);
        }

        public void SetId(int pId)
        {
            mId = pId;
        }
        
        public void SetValue(int pValue)
        {
            mValue = pValue;
        }

        /// <summary>
        /// Incase id = -1 (mean random) but we want to know what it is in firstplace
        /// We separate this function because some panel/popup need to show clearly beforehand some don't
        /// </summary>
        public RewardInfo PrePickReward()
        {
            if (mId == -1)
                mId = LogicAPI.GetRandomRewardId(mType);
            return this;
        }
    }
}
