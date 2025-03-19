using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Common;

namespace Utilities.Pattern.Data
{
    #region To Parse Data from Json Data Source

    [Serializable]
    public class AttributesCollection<T> where T : AttributeParse
    {
        public List<T> Attributes;

        public T GetAtt(int pId)
        {
            for (int i = 0; i < Attributes.Count; i++)
                if (Attributes[i].id == pId)
                    return Attributes[i];
            return null;
        }

        public float GetAttValue(int pId, float pDefault = 0)
        {
            var att = GetAtt(pId);
            return att == null ? pDefault : att.value;
        }
        public float GetAttValue(int pId, int pLevel, float pDefault = 0)
        {
            var att = GetAtt(pId);
            return att == null ? pDefault : att.GetValue(pLevel);
        }
        public int GetAttIntValue(int pId, int pDefault = 0)
        {
            var att = GetAtt(pId);
            return att == null ? pDefault : Mathf.RoundToInt(att.value);
        }
        public int GetAttIntValue(int pId, int pLevel, int pDefault = 0)
        {
            var att = GetAtt(pId);
            return att == null ? pDefault : Mathf.RoundToInt(att.GetValue(pLevel));
        }
        public bool GetAttBoolValue(int pId, bool pDefault = false)
        {
            var att = GetAtt(pId);
            return att == null ? pDefault : att.value >= 1;
        }
        public bool GetAttBoolValue(int pId, int pLevel, bool pDefault = false)
        {
            var att = GetAtt(pId);
            return att == null ? pDefault : att.GetValue(pLevel) >= 1;
        }
        public string LogValues()
        {
            string text = "";
            for (int i = 0; i < Attributes.Count; i++)
            {
                text += string.Format("Id:{0}, Value:{1}", Attributes[i].id, Attributes[i].value);
                if (i + 1 <= Attributes.Count - 1)
                    text += "/";
            }
            return text;
        }
        public Dictionary<int, float> AssembleAtts(int pLevel)
        {
            var dict = new Dictionary<int, float>();
            foreach (var item in Attributes)
            {
                if (!dict.ContainsKey(item.id))
                    dict.Add(item.id, item.GetValue(pLevel));
                else
                    dict[item.id] += item.GetValue(pLevel);
            }
            return dict;
        }
    }

    [Serializable]
    public class AttributeParse
    {
        //=== MAIN
        public int id;
        public float value;
        public int unlock;
        public float increase;
        public float max;
        //=== Optional
        public float[] values; //Sometime we have more than one value eg. value-0 for Gold value-1 for Gem
        public float[] increases; //Sometime Increase is defined by multi values eg. increase-0 for level, increase-1 for rarity
        public float[] unlocks; //Sometime Unlock is defined for many purposes eg. level to unlock or rarity to unlock
        public float[] maxes; //Sometime max is defined for diffirent value eg. max value when rarity == 1 or max value when rarity == 2
        public string valueString;

        public float GetValue(int pLevel = 1, int pValueIndex = -1, int pIncreaseIndex = -1, int pUnlockIndex = -1, int pMaxIndex = -1)
        {
            float value = this.value;
            float unlock = this.unlock;
            float max = this.max;
            float increase = this.increase;
            if (value == 0 && pValueIndex >= 0) value = values[pValueIndex];
            if (unlock == 0 && pUnlockIndex >= 0) unlock = unlocks[pUnlockIndex];
            if (increase == 0 && pIncreaseIndex >= 0) unlock = increases[pIncreaseIndex];
            if (max == 0 && pMaxIndex >= 0) max = maxes[pMaxIndex];

            if (pLevel < unlock || pLevel == 0)
                return 0;

            float inc = 0;
            if (unlock > 0)
                inc = (pLevel - unlock) * increase;
            else
                inc = (pLevel - 1) * increase;
            if (inc > max && max > 0)
                inc = max;
            return value + inc;
        }
        public int GetIntValue(int pLevel = 1, int pValueIndex = -1, int pIncreaseIndex = -1, int pUnlockIndex = -1, int pMaxIndex = -1)
        {
            return Mathf.RoundToInt(GetValue(pLevel, pValueIndex, pIncreaseIndex, pUnlockIndex, pMaxIndex));
        }
        public bool GetBoolValue(int pLevel = 1, int pValueIndex = -1, int pIncreaseIndex = -1, int pUnlockIndex = -1, int pMaxIndex = -1)
        {
            return GetValue(pLevel, pValueIndex, pIncreaseIndex, pUnlockIndex, pMaxIndex) >= 1;
        }
        public string GetDescription(int pLevel = 1, int pNextLevel = 1, int pValueIndex = -1, int pIncreaseIndex = -1, int pUnlockIndex = -1, int pMaxIndex = -1)
        {
            float curValue = GetValue(pLevel, pValueIndex, pIncreaseIndex, pUnlockIndex, pMaxIndex);
            if (pNextLevel != pLevel)
            {
                string str = "";
                float nextValue = GetValue(pNextLevel);
                if (nextValue != curValue)
                    str = $"{MathHelper.Round(curValue, 2)} (<color=#299110>+{MathHelper.Round(nextValue - curValue, 2)}</color>)";
                else
                    str = curValue.ToString();
                return str;
            }
            return curValue.ToString();
        }
    }

    public static class AttributeParseExtenstion
    {
        public static string LogValues(this List<AttributeParse> attributes)
        {
            string text = "";
            for (int i = 0; i < attributes.Count; i++)
            {
                text += string.Format("Id:{0}, Value:{1}, Increase:{2}, Max:{3}", attributes[i].id, attributes[i].value, attributes[i].increase, attributes[i].max);
                if (i + 1 <= attributes.Count - 1)
                    text += "\n";
            }
            return text;
        }
        public static float GetValueOf(this List<AttributeParse> attributes, int pId, int pLevel = 1)
        {
            for (int i = 0; i < attributes.Count; i++)
            {
                if (attributes[i].id == pId)
                    return attributes[i].GetValue(pLevel);
            }
            return 0;
        }
        public static AttributeParse Find(this List<AttributeParse> attributes, int pId)
        {
            for (int i = 0; i < attributes.Count; i++)
            {
                if (attributes[i].id == pId)
                    return attributes[i];
            }
            return null;
        }
    }

    #endregion

    //=====================================================

    #region To Handle

    public class AttributesContainer<T> where T : BaseAttribute
    {
        public List<T> attributes;
        public int Count => attributes == null ? 0 : attributes.Count;

        public AttributesContainer()
        {
            attributes = new List<T>();
        }
        public void Add(T pAttribute)
        {
            attributes.Add(pAttribute);
        }
        public void Add(List<T> pAttributes)
        {
            attributes.AddRange(pAttributes);
        }
        public T GetAttById(int pId)
        {
            for (int i = 0; i < attributes.Count; i++)
            {
                if (attributes[i].id == pId)
                    return attributes[i] as T;
            }
            return null;
        }
        public T GetAttByIndex(int pIndex)
        {
            if (pIndex >= 0 && pIndex < attributes.Count)
                return attributes[pIndex] as T;
            return null;
        }
        public float GetAttValue(int pId, int pLevel = 1, float pDefault = 0)
        {
            var att = GetAttById(pId);
            return att == null ? pDefault : att.GetValue(pLevel);
        }
        public int GetAttIntValue(int pId, int pDefault = 0)
        {
            var att = GetAttById(pId);
            return att == null ? pDefault : Mathf.RoundToInt(att.value);
        }
        public int GetAttIntValue(int pId, int pLevel, int pDefault = 0)
        {
            var att = GetAttById(pId);
            return att == null ? pDefault : Mathf.RoundToInt(att.GetValue(pLevel));
        }
        public bool GetAttBoolValue(int pId, bool pDefault = false)
        {
            var att = GetAttById(pId);
            return att == null ? pDefault : att.value >= 1;
        }
        public bool GetAttBoolValue(int pId, int pLevel, bool pDefault = false)
        {
            var att = GetAttById(pId);
            return att == null ? pDefault : att.GetValue(pLevel) >= 1;
        }
        public string LogValues()
        {
            string text = "";
            for (int i = 0; i < attributes.Count; i++)
            {
                text += string.Format("Id:{0}, Value:{1}", attributes[i].id, attributes[i].value);
                if (i + 1 <= attributes.Count - 1)
                    text += "/";
            }
            return text;
        }
        public Dictionary<int, float> AssembleAtts(int pLevel)
        {
            var dict = new Dictionary<int, float>();
            foreach (var item in attributes)
            {
                if (!dict.ContainsKey(item.id))
                    dict.Add(item.id, item.GetValue(pLevel));
                else
                    dict[item.id] += item.GetValue(pLevel);
            }
            return dict;
        }
    }

    public class BaseAttribute
    {
        protected int mId;
        protected int mUnlock;
        protected float mIncrease;
        protected float mValue;
        protected float mMax;

        public int id { get { return mId; } }
        public int unlock { get { return mUnlock; } }
        public float increase { get { return mIncrease; } }
        public float value { get { return mValue; } }
        public float max { get { return mMax; } }

        public BaseAttribute()
        {
            mId = -1;
            mValue = 0;
        }
        public BaseAttribute(int pId, float pValue)
        {
            mId = pId;
            mValue = pValue;
            mUnlock = 0;
        }
        public BaseAttribute(int pId, float pValue, int pUnlock)
        {
            mId = pId;
            mValue = pValue;
            mUnlock = pUnlock;
        }
        public BaseAttribute(int pId, float pValue, int pUnlock, float pIncrease)
        {
            mId = pId;
            mValue = pValue;
            mUnlock = pUnlock;
            mIncrease = pIncrease;
        }
        public BaseAttribute(int pId, float pValue, int pUnlock, float pIncrease, float pMax)
        {
            mId = pId;
            mValue = pValue;
            mUnlock = pUnlock;
            mIncrease = pIncrease;
            mMax = pMax;
        }
        public BaseAttribute(AttributeParse pAttribute)
        {
            mId = pAttribute.id;
            mValue = pAttribute.value;
            mUnlock = pAttribute.unlock;
            mIncrease = pAttribute.increase;
            mMax = pAttribute.max;
        }
        public float GetValue(int pLevel = 1)
        {
            if (pLevel < unlock || pLevel == 0)
                return 0;

            float inc = 0;
            if (unlock > 0)
                inc = (pLevel - unlock) * increase;
            else
                inc = (pLevel - 1) * increase;
            if (inc > max && max > 0)
                inc = max;
            return value + inc;
        }
        public int GetIntValue(int pLevel = 1)
        {
            return Mathf.RoundToInt(GetValue(pLevel));
        }
        public bool GetBoolValue(int pLevel = 1)
        {
            return GetValue(pLevel) >= 1;
        }
        public virtual string GetDescription(int pLevel = 1, int pNextLevel = 1)
        {
            string str = "";
            float curValue = GetValue(pLevel);
            if (pNextLevel != pLevel)
            {
                float nextValue = GetValue(pNextLevel);
                if (nextValue > curValue)
                    str = $"{MathHelper.Round(curValue, 2)} (+{MathHelper.Round(nextValue - curValue, 2)})";
                else
                    str = curValue.ToString();
                return str;
            }
            return curValue.ToString();
        }
    }

    #endregion
}