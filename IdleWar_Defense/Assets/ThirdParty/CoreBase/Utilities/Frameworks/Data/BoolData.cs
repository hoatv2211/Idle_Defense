using System;
using UnityEngine;
using Utilities.AntiCheat;

namespace Utilities.Pattern.Data
{
    public class BoolData : FunData
    {
        private ObscuredBool mValue;
        private bool mDefaultValue;
        private bool mChanged;

        public bool Value
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

        public BoolData(int pId, bool pDefaultValue = false) : base(pId)
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

        private bool GetSavedValue()
        {
            string val = GetStringValue();
            if (string.IsNullOrEmpty(val))
                return mDefaultValue;

            bool output = false;
            if (bool.TryParse(val, out output))
            {
                return output;
            }
            else
            {
                Debug.LogError("can not parse key " + mKey + " with value " + val + " to bool");
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