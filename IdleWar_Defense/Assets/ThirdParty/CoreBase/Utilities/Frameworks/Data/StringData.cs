using System;

namespace Utilities.Pattern.Data
{
    public class StringData : FunData
    {
        private string mValue = null;
        private string mDefaultValue = null;
        private bool mChanged;

        public string Value
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

        public StringData(int pId, string pDefaultValue = "") : base(pId)
        {
            mDefaultValue = pDefaultValue;
        }

        public override void Load(string pBaseKey, string pSaverIdString)
        {
            base.Load(pBaseKey, pSaverIdString);

            mValue = GetSavedValue();
            if (mValue == null)
                mValue = mDefaultValue;
        }

        public override bool Stage()
        {
            if (mChanged)
            {
                SetStringValue(mValue);
                mChanged = false;
                return true;
            }
            return false;
        }

        private string GetSavedValue()
        {
            return GetStringValue();
        }

        public override void Reload(bool pClearIndex)
        {
            base.Reload(pClearIndex);
            Value = GetSavedValue();
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