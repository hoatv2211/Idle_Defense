using Spine;
using Spine.Unity;
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
    public class EarlyAccessWarningPopup : MyGamesBasePanel
    {
        [Header("Build-in prefab")]
        [SerializeField] private CustomToggleTab mTogIknow;
        [SerializeField] private JustButton mBtnPlay;

        public void Init(UnityAction OnBtnPlay_Pressed)
        {
            mTogIknow.isOn = true;
            mBtnPlay.SetEnable(mTogIknow.isOn);
            mTogIknow.onValueChanged.AddListener(OnTogIknow_Changed);
            mBtnPlay.onClick.AddListener(() => {
                OnBtnPlay_Pressed();
                Back();
            });
        }

        private void OnTogIknow_Changed(bool pIsOn)
        {
            mBtnPlay.SetEnable(pIsOn);
        }
    }
}