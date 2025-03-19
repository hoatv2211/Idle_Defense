using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;
using TMPro;

namespace FoodZombie.UI
{
    public class LoginPanel : MyGamesBasePanel
    {
        [Header("Main")]
        [SerializeField] private TMP_InputField inputName;
        [SerializeField] private SimpleTMPButton btnConfirm;
        [SerializeField] private SimpleTMPButton btnFBLogin;
        [SerializeField] private SimpleTMPButton btnGGLogin;

        internal override void Init()
        {
            base.Init();

            btnConfirm.SetUpEvent(Action_BtnConfirm);
            btnFBLogin.SetUpEvent(Action_BtnFBLogin);
            btnGGLogin.SetUpEvent(Action_BtnGGLogin);
        }


        private void Action_BtnConfirm()
        {
            //xử lý tạo account nhanh tại đây


        }

        private void Action_BtnFBLogin()
        {
            FBManager.Instance.FBLogin();
        }

        private void Action_BtnGGLogin()
        {
            //GG Login
        }


    }

}
