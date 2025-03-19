using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Common;
using Utilities.Components;
using Utilities.Inspector;
using UnityEngine.UI;
using TMPro;
using System;

namespace FoodZombie.UI
{
    public class VictoryPanel : MyGamesBasePanel
    {
        #region Members

        [SerializeField] private List<Image> mImgStars;
        [SerializeField] private TextMeshProUGUI mTxtMissionName;
        [SerializeField] private TextMeshProUGUI mTxtDifficult;
        [SerializeField] private List<ImageWithText> mRewards;
        [SerializeField] private SimpleTMPButton mBtnOk;

        #endregion

        //=============================================

        #region MonoBehaviour

        private void Start()
        {
            mBtnOk.onClick.AddListener(OnBtnOk_Pressed);
        }

        #endregion

        //=============================================

        #region Public

        #endregion

        //==============================================

        #region Private

        private void OnBtnOk_Pressed()
        {
        }

        #endregion
    }
}