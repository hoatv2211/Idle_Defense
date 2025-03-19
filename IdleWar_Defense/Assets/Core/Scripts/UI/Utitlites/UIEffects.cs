using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;

namespace FoodZombie.UI
{
    public class UIEffects : MonoBehaviour
    {
        #region Internal Class

        public class Info
        {
            public Sprite icon;
            public int value;
        }

        #endregion

        //=============================================

        #region Members

        /// <summary>
        /// Effect prefab
        /// </summary>
        [SerializeField] private UIEffectItem mUITooltipEffect;
        [SerializeField] private Transform origin;

        private RectTransform goldTarget;
        private RectTransform cashTarget;
        //private RectTransform troopTarget;

        private bool initialized;

        private CustomPool<UIEffectItem> UIEffectItemsPool;

        #endregion

        //=============================================

        #region MonoBehaviour

        private void Start()
        {
            
        }

        private void Update()
        {
            if (origin.childCount <= 0)
            {
                enabled = false;
                gameObject.SetActive(false);
            }

            transform.SetAsLastSibling();
        }

        #endregion

        //=============================================

        #region Public

        public void SpawnEffect(RectTransform pFrom, Info pInfo, float time = 1.2f)
        {
            if (!initialized) Init();

            UIEffectItem pUITooltipEffect = UIEffectItemsPool.Spawn();
            pUITooltipEffect.SetActive(true);
            pUITooltipEffect.Init(pInfo.icon, pInfo.value, pFrom, null, time, Release);
            enabled = true;
        }

        public void SpawnEffectRandomNearByFrom(RectTransform pFrom, RewardInfo pReward, float time = 1.0f)
        {
            if (!initialized) Init();

            RectTransform pTo = null;

            switch (pReward.Type)
            {
                case IDs.REWARD_TYPE_CURRENCY:
                    if (pReward.Id == IDs.CURRENCY_COIN) pTo = goldTarget;
                    //else if (pReward.Id == IDs.CURRENCY_CASH) pTo = cashTarget;
                    break;
                //case IDs.REWARD_TYPE_UNLOCK_CHARACTER:
                //    pTo = troopTarget;
                //    break;
                default:
                    break;
            }


            UIEffectItem pUITooltipEffect = UIEffectItemsPool.Spawn();
            pUITooltipEffect.SetActive(true);
            pUITooltipEffect.InitRandom(pReward.GetIcon(), pReward.Value, pFrom, pTo, time, Release);
            enabled = true;
        }

        #endregion

        //==============================================

        #region Private

        private void Init()
        {
            goldTarget = MainPanel.instance.MainMenuPanel.coinView.rectTransform();
            cashTarget = MainPanel.instance.MainMenuPanel.gemView.rectTransform();
            //troopTarget = MainPanel.instance.MainMenuPanel.btnEditTeam.rectTransform();

            if (UIEffectItemsPool == null)
                UIEffectItemsPool = new CustomPool<UIEffectItem>(mUITooltipEffect, 6, origin, false, "", false);
            UIEffectItemsPool.ReleaseAll();

            initialized = true;
        }

        private void Release(UIEffectItem uIEffectItem)
        {
            UIEffectItemsPool.Release(uIEffectItem);
        }

        #endregion
    }
}
