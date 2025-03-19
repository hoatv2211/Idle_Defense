using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities.Components
{
    [AddComponentMenu("Utitlies/UI/PriceTMPButton")]
    public class PriceTMPButton : SimpleTMPButton
    {
        [SerializeField]
        protected TextMeshProUGUI mLabelTMPCost;
        public TextMeshProUGUI labelTMPCost
        {
            get { return mLabelTMPCost; }
        }

        [SerializeField]
        protected Image mImgCurrency;
        public Image imgCurrency
        {
            get { return mImgCurrency; }
        }
    }
}