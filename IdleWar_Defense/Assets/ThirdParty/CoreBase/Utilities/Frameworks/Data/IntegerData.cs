using System;
using UnityEngine;
using Utilities.AntiCheat;

namespace Utilities.Pattern.Data
{
    public class IntegerData : FunData
    {
        private ObscuredInt mValue;
        private int mDefaultValue;
        private bool mChanged;

        public int Value
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

        public IntegerData(int pId, int pDefaultValue = 0) : base(pId)
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

        private int GetSavedValue()
        {
            string val = GetStringValue();
            if (string.IsNullOrEmpty(val))
                return mDefaultValue;

            int output = 0;
            if (int.TryParse(val, out output))
            {
                return output;
            }
            else
            {
                Debug.LogError("can not parse key " + mKey + " with value " + val + " to int");
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