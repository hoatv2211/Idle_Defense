using System;
using UnityEngine;
using Utilities.AntiCheat;

namespace Utilities.Pattern.Data
{
    public class LongData : FunData
    {
        //private ObscuredLong mValue;
        private long mValue;
        private long mDefaultValue;
        private bool mChanged;

        public long Value
        {
            get { return mValue; }
            set
            {
                if (value != mValue)
                {
                    mValue = value;
                    mChanged = true;
                }
            }
        }

        public LongData(int pId, long pDefaultValue = 0) : base(pId)
        {
            mDefaultValue = pDefaultValue;
        }

        public override void Load(string pBaseKey, string pSaverIdString)
        {
            base.Load(pBaseKey, pSaverIdString);
            mValue = GetSavedValue();
        }

        public override bool Stage()
        {
            if (mChanged)
            {
                SetStringValue(mValue.ToString());
                mChanged = false;
                return true;
            }
            return false;
        }

        private long GetSavedValue()
        {
            string val = GetStringValue();
            if (string.IsNullOrEmpty(val))
                return mDefaultValue;

            long output = 0;
            if (long.TryParse(val, out output))
            {
                return output;
            }
            else
            {
                Debug.LogError("can not parse key " + mKey + " with value " + val + " to long");
                return mDefaultValue;
            }
        }

        public override void Reload(bool pClearIndex)
        {
            base.Reload(pClearIndex);
            mValue = GetSavedValue();
            mChanged = false;
        }

        public override void Reset()
        {
            Value = mDefaultValue;
        }

        public override bool Cleanable()
        {
            if (mIndex != -1 && Value == mDefaultValue)
            {
                return true;
            }
            return false;
        }
    }
}