using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Pattern.Data;

namespace FoodZombie
{
    public class Attribute : BaseAttribute
    {
        private LocalizationGetter mLocalizedName;
        private LocalizationGetter mLocalizedBonus;

        public Attribute()
        {
        }

        public Attribute(int pId, float pValue) : base(pId, pValue)
        {
        }

        public Attribute(int pId, float pValue, int pUnlock) : base(pId, pValue, pUnlock)
        {
        }

        public Attribute(int pId, float pValue, int pUnlock, float pIncrease) : base(pId, pValue, pUnlock, pIncrease)
        {
        }

        public Attribute(int pId, float pValue, int pUnlock, float pIncrease, float pMax) : base(pId, pValue, pUnlock, pIncrease, pMax)
        {
        }

        public string GetDisplayName()
        {
            string key = string.Format("ATT_NAME_{0}", mId);
            if (mLocalizedName == null)
                mLocalizedName = new LocalizationGetter(key, key);
            string text = mLocalizedName.Get();
            if (string.IsNullOrEmpty(text))
                return key;
            else
                return text;
        }

        public string GetDisplayBonus(int pLevel = 1, bool pShowIcon = false)
        {
            string key = string.Format("ATT_BONUS_{0}", mId);
            if (mLocalizedBonus == null)
                mLocalizedBonus = new LocalizationGetter(key, "");
            string text = "";

            if (pShowIcon)
            {
                switch (mId)
                {
                    //case IDs.ATT_HP:
                    //case IDs.ATT_BUFF_HP:
                    //    text = FixedNames.TEXT_HP + " {0}{1}";
                    //    break;
                    //case IDs.ATT_BUFF_PERCENT_HP:
                    //    text = FixedNames.TEXT_HP + " {0}{1}%";
                    //    break;
                    //case IDs.ATT_DMG:
                    //case IDs.ATT_BUFF_DMG:
                    //    text = FixedNames.TEXT_ATK + " {0}{1}";
                    //    break;
                    //case IDs.ATT_BUFF_PERCENT_DMG:
                    //    text = FixedNames.TEXT_ATK + " {0}{1}%";
                    //    break;
                    //case IDs.ATT_BUFF_MELEE:
                    //    text = FixedNames.TEXT_KNIFE + " {0}{1}";
                    //    break;
                    //case IDs.ATT_BUFF_PERCENT_MELEE:
                    //    text = FixedNames.TEXT_KNIFE + " {0}{1}%";
                    //    break;
                    //case IDs.ATT_BUFF_RANGER:
                    //    text = FixedNames.TEXT_GUN + " {0}{1}";
                    //    break;
                    //case IDs.ATT_BUFF_PERCENT_RANGER:
                    //    text = FixedNames.TEXT_GUN + " {0}{1}%";
                    //    break;
                    //case IDs.ATT_BUFF_LUCK:
                    //    text = FixedNames.TEXT_LUCK + " {0}{1}";
                    //    break;
                    //case IDs.ATT_BUFF_PERCENT_LUCK:
                    //    text = FixedNames.TEXT_LUCK + " {0}{1}%";
                    //    break;
                    default:
                        text = mLocalizedBonus.Get();
                        break;
                }
            }
            else
                text = mLocalizedBonus.Get();
            float value = GetValue(pLevel);
            string sign = value >= 0 ? "+" : "";
            if (string.IsNullOrEmpty(text))
                return string.Format("{0}{1}{2}", key, sign, value);
            else
            {
                if (text.Contains("{1}"))
                    return string.Format(text, sign, value);
                else
                    return string.Format(text, value);
            }
        }
    }
}