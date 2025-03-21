using Spine;
using Spine.Unity.Modules;
using System.Collections;
using UnityEngine;
using Utilities.Common;
using Utilities.Components;
using System.Collections.Generic;

#if UNITY_IOS
using UnityEngine.iOS;
using Unity.Advertisement.IosSupport;
#endif

namespace FoodZombie.UI
{
    public class IntroPanel : MyGamesBasePanel
    {
        public LoadingPanel loadingPanel;

        // public SkeletonGraphicMultiObject mSkeleton;
        // public SimpleTMPButton btnSkip;
        private bool isWinIntroMission;
        private bool isOpenGameBefore;
        internal override void Init()
        {

#if UNITY_IOS

            if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() ==
            ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
            {

                ATTrackingStatusBinding.RequestAuthorizationTracking();

            }

#endif
            //  NetIDs.IDS;

            PushPanelToTop(ref loadingPanel);
            isWinIntroMission = GameData.Instance.MissionsGroup.IsWinIntroMission();
            isOpenGameBefore = GameData.Instance.UserGroup.OpenGameBefore;
            if (!GameData.Instance.MissionsGroup.IsFirstMission())
            {
                isOpenGameBefore = true;
                GameData.Instance.UserGroup.OpenGameBefore = true;
            }
            ShowLoadingPanel();

            // var trackEntry = mSkeleton.AnimationState.SetAnimation(0, "action", false);
            // trackEntry.Complete += delegate (Spine.TrackEntry entry)
            // {
            //     ShowLoadingPanel();
            // };
            //
            // isFirstMission = GameData.Instance.MissionsGroup.IsFirstMission();
            // if (isFirstMission)
            // {
            //     SoundManager.Instance.PlayMusic(IDs.MUSIC_INTRO);
            // }
            // else
            // {
            //     ShowLoadingPanel();
            // }
        }

        // private void Start()
        // {
        //     btnSkip.onClick.AddListener(BtnSkip_Pressed);
        // }
        //
        // private void BtnSkip_Pressed()
        // {
        //     ShowLoadingPanel();
        // }

        private void ShowLoadingPanel()
        {
            SoundManager.Instance.StopMusic(true);
            PushPanelToTop(ref loadingPanel);

            if (isOpenGameBefore)
            {
                loadingPanel.LoadHomeScene();
            }
            else
            {
                //var task = FirebaseApp.CheckAndFixDependenciesAsync();
                //WaitUtil.WaitTask(task, () =>
                //{
                //    if (task.Result == DependencyStatus.Available)
                //    {
                //        //  Config.LogEvent(TrackingConstants.MISSION_PLAY_COUNT, TrackingConstants.PARAM_MISSION, TrackingConstants.VALUE_INTRO_MISSION);
                //    }
                //});

                //Unlock John and add to Formations:
                var heroesGroup = GameData.Instance.HeroesGroup;
                var allHeroes = heroesGroup.GetAllHeroDatas();
                int heroUnlockedNumber = 0;
                if (allHeroes != null && allHeroes.Count > 0) heroUnlockedNumber = allHeroes.Count;

                if (heroUnlockedNumber == 0)
                {
                    //Unlock John
                    var rewardInfo = GameData.Instance.HeroesGroup.SummonByPowerFragment();
                    LogicAPI.ClaimReward(rewardInfo, "Intro", false);
                    //Add All Hero Unlocked to Formation: Now only John:
                    var formations = new HeroData[HeroesGroup.MAX_FORMATION][];
                    for (int i = 0; i < HeroesGroup.MAX_FORMATION; i++)
                    {
                        formations[i] = heroesGroup.GetEquippedHeroes(i);
                    }
                    allHeroes = heroesGroup.GetAllHeroDatas();
                    //var allHeroes = heroesGroup.GetAllHeroDatas();
                    for (int i = 0; i < allHeroes.Count; i++)
                    {
                        if (i < formations[0].Length)
                            formations[0][i] = allHeroes[i];
                    }

                    //Save Formation:
                    var count = formations.Length;

                    var newFormations = new List<List<string>>();
                    for (int i = 0; i < count; i++)
                    {
                        var newFormation = new List<string>();
                        var formation = formations[i];
                        var fCount = formation.Length;
                        for (int j = 0; j < fCount; j++)
                        {
                            var heroData = formation[j];
                            if (heroData == null) newFormation.Add("-1");
                            else newFormation.Add(formation[j].id + "");
                        }

                        newFormations.Add(newFormation);
                    }
                    heroesGroup.SaveEquippedHeros(newFormations);
                }
                ////Khi mới mở game lên lần đầu, chạy thẳng map 1 luôn chứ ko vào qua world map nữa
                //loadingPanel.LoadGamePlayScreen();
                loadingPanel.LoadHomeScene();

                AppsFlyerObjectScript.LogFirstOpenApp();
                //AdjustManager.LogFirstOpenApp();



            }

            Debug.LogError(Application.systemLanguage);
            Localization.currentLanguage = "english";

            //AdjustManager.LogOpenApp();
            GameData.Instance.UserGroup.OpenGameBefore = true;
        }
    }
}