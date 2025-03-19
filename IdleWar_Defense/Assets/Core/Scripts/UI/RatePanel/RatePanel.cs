using System;
using UnityEngine;
using Utilities.Common;
using Utilities.Components;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace FoodZombie.UI
{
    public class RatePanel : MyGamesBasePanel
    {
        // public static bool isShowed = false;
        public static int lastMissionShow = -5;
        public SimpleTMPButton[] btnStars;
        public SimpleTMPButton btnRate;
        public Material greyMat;

        private int star = 0;

        private void Start()
        {
            btnRate.onClick.AddListener(BtnRate_Pressed);

            int count = btnStars.Length;
            for (int i = 0; i < count; i++)
            {
                int index = i;
                var btnStar = btnStars[index];
                btnStar.onClick.AddListener(() =>
                {
                    BtnStar_Pressed(index);
                });
            }

            // isShowed = true;
        }

        private void OnEnable()
        {
            var count = btnStars.Length;
            for (int i = 0; i < count; i++)
            {
                btnStars[i].img.material = greyMat;
            }

            star = 0;
            // btnRate.SetActive(false);

            lastMissionShow = GameData.Instance.MissionsGroup.CurrentMissionId;
        }

        private void BtnRate_Pressed()
        {
            GameData.Instance.GameConfigGroup.Rate();
            Config.LogEvent(TrackingConstants.CLICK_RATEGAME);
            // if (star >= 4)
            // {
#if UNITY_ANDROID
            Application.OpenURL("market://details?id=" + Application.identifier);
#elif UNITY_IOS
                Device.RequestStoreReview();
#endif
            Back();
            // }
            // else
            // {
            //     Back();
            //     MainPanel.instance.ShowWarningPopup("Thank you for your support");
            // }
        }

        private void BtnStar_Pressed(int index)
        {
            var count = btnStars.Length;
            star = index + 1;
            for (int i = 0; i < star; i++)
            {
                btnStars[i].img.material = null;
            }

            for (int i = star; i < count; i++)
            {
                btnStars[i].img.material = greyMat;
            }

            btnRate.SetActive(true);
        }
    }
}