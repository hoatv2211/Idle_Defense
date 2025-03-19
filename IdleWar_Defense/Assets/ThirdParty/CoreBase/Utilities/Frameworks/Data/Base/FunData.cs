using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Pattern.Data
{
    public abstract class FunData
    {
        #region Members

        /// <summary>
        /// Unique of id of data in it's group
        /// </summary>
        protected int mId;
        /// <summary>
        /// Key of data, used to load data from data list
        /// </summary>
        protected string mKey;
        /// <summary>
        /// Cached index of saved data, used to quick get/set
        /// </summary>
        protected int mIndex = -1;
        /// <summary>
        /// Id String of data saver
        /// </summary>
        protected string mSaverIdString;

        public string Key { get { return mKey; } }
        public int Id { get { return mId; } }
        public int Index { get { return mIndex; } }
        public DataSaver DataSaver { get { return DataSaverContainer.GetSaver(mSaverIdString); } }

        public virtual List<FunData> Children { get { return null; } }

        #endregion

        //=======================================

        #region Common Methods

        public FunData(int pId)
        {
            mId = pId;
        }

        /// <summary>
        /// Build Key used in Data Saver
        /// </summary>
        /// <param name="pBaseKey">Key of Data Group (Its parent)</param>
        /// <param name="pSaverIdString"></param>
        public virtual void Load(string pBaseKey, string pSaverIdString)
        {
            Debug.Assert(pSaverIdString != null, "Data saver cannot be null!");

            if (!string.IsNullOrEmpty(pBaseKey))
                mKey = string.Format("{0}.{1}", pBaseKey, mId);
            else
                mKey = mId.ToString();
            mSaverIdString = pSaverIdString;
        }
        public virtual void PostLoad() { }
        public virtual void SetStringValue(string pValue)
        {
            try
            {
                if (mIndex == -1)
                    mIndex = DataSaver.Set(mKey, pValue);
                else
                    DataSaver.Set(mIndex, pValue);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
        public virtual string GetStringValue()
        {
            if (mIndex == -1)
                return DataSaver.Get(mKey, out mIndex);
            else
                return DataSaver.Get(mIndex);
        }
        public virtual void ClearIndex()
        {
            mIndex = -1;
        }
        public virtual void OnApplicationPaused(bool pPaused) { }
        public virtual void OnApplicationQuit() { }
        /// <summary>
        /// Reload data back to last saved
        /// </summary>
        /// <param name="pClearIndex">Clear cached index of data in data list</param>
        public virtual void Reload(bool pClearIndex)
        {
            if (pClearIndex)
                mIndex = -1;
        }

        public abstract void Reset();
        public abstract bool Stage();
        public abstract bool Cleanable();

        #endregion
    }
}