using System;
using System.Collections.Generic;
using Utilities.Common;
using Debug = UnityEngine.Debug;

namespace Utilities.Pattern.Data
{
    public class ListData<T> : FunData
    {
        private List<T> mValues;
        private List<T> mDefaultValues;
        private bool mChanged;

        /// <summary>
        /// NOTE: This type of data should not has Get method
        /// If you are looking for a way to get all values, please use for GetValues method and read it's notice carefully
        /// </summary>
        public List<T> Values
        {
            set
            {
                if (value != mValues)
                {
                    mValues = value;
                    mChanged = true;
                }
            }
        }

        public int Count
        {
            get { return mValues == null ? 0 : mValues.Count; }
        }

        /// <summary>
        /// NOTE: use this carefully because we can not detect the change made in an element
        /// </summary>
        public T this[int index]
        {
            get { return mValues[index]; }
            set
            {
                mValues[index] = value;
                mChanged = true;
            }
        }

        public ListData(int pId, List<T> pDefaultValues = null) : base(pId)
        {
            mDefaultValues = pDefaultValues;
        }

        public ListData(int pId, params T[] pDefaultValues) : base(pId)
        {
            mDefaultValues = new List<T>();
            mDefaultValues.AddRange(pDefaultValues);
        }

        public override void Load(string pBaseKey, string pSaverIdString)
        {
            base.Load(pBaseKey, pSaverIdString);
            mValues = GetSavedValues();
        }

        public void Add(T value)
        {
            if (mValues == null)
                mValues = new List<T>();
            mValues.Add(value);
            mChanged = true;
        }

        public void AddRange(params T[] values)
        {
            if (mValues == null)
                mValues = new List<T>();
            mValues.AddRange(values);
            mChanged = true;
        }

        public void Remove(T value)
        {
            mValues.Remove(value);
            mChanged = true;
        }

        public bool Contain(T value)
        {
            return mValues.Contains(value);
        }

        public override bool Stage()
        {
            if (mChanged)
            {
                SetStringValue(JsonHelper.ListToJson(mValues));
                mChanged = false;
                return true;
            }
            return false;
        }

        private List<T> GetSavedValues()
        {
            string val = GetStringValue();
            if (string.IsNullOrEmpty(val))
                return mDefaultValues;

            try
            {
                mValues = JsonHelper.GetJsonList<T>(val);
                return mValues;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());

                Values = mDefaultValues;
                return mDefaultValues;
            }
        }

        public override void Reload(bool pClearIndex)
        {
            base.Reload(pClearIndex);
            mValues = GetSavedValues();
            mChanged = false;
        }

        public override void Reset()
        {
            mValues = mDefaultValues;
            mChanged = true;
        }

        public override bool Cleanable()
        {
            return false;
        }

        public void Sort()
        {
            mValues.Sort();
            mChanged = true;
        }

        public void MarkChange()
        {
            mChanged = true;
        }

        [Obsolete("NOTE: Because It is hard to track the change if we directly add/insert/update internal data of list. Therefore" +
        "If you are planing to change directly data inside list. You must set mChanged manually by MarkChange method")]
        public List<T> GetValues()
        {
            return mValues;
        }
    }
}