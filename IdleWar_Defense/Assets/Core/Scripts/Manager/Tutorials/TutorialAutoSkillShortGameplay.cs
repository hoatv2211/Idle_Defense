using System;
using System.Collections;
using System.Collections.Generic;
using FoodZombie.UI;
using HedgehogTeam.EasyTouch;
using UnityEngine;
using Utilities.Common;
using Utilities.Components;

namespace FoodZombie.UI
{
    public class TutorialAutoSkillShortGameplay : TutorialController
    {
        private JustButton btnAutoPlay;
        private JustButton btnHome;
        private ButtonItemGameplay btnBarrier;
        private bool pressedBtn;

        public TutorialAutoSkillShortGameplay(int pId, bool pIsToolTips) : base(pId, pIsToolTips)
        {
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Pause()
        {
        }

        public override void Resume()
        {
        }

        protected override void OnAnyChildHide(MyGamesBasePanel pPanel)
        {
        }

        protected override void OnAnyChildShow(MyGamesBasePanel pPanel)
        {
        }

        public override IEnumerator IEProcess()
        {
            if (!GameUnityData.instance.GameRemoteConfig.Function_Autoplay_Active)
            {
                End(true);
                yield break;
            }
            else
            {
                int _levelActiveAutoplay = GameUnityData.instance.GameRemoteConfig.Function_Autoplay_ActiveLevel;
                if (_levelActiveAutoplay > 0 && GameData.Instance.MissionsGroup.CurrentMissionId < _levelActiveAutoplay)
                {
                    End(true);
                    yield break;
                }
            }

            yield return new WaitUntil(() => (MainGamePanel != null && MainGamePanel.HubPanel != null
                                                                 && MainGamePanel.TopPanel is HubPanel));

            string s;
            MessageWithPointer board;
            //=== step 1:
            pressedBtn = false;
            btnAutoPlay = MainGamePanel.HubPanel.btnAutoPlay;


            GameplayController.Instance.SetAutoPlay(false);
            //btnAutoPlay.SetActive(false);
            GameplayController.Instance.PauseGame();
            //  yield return new WaitForSecondsRealtime(1);

            TutorialsManager.InputMasker.Lock();

            //MainGamePanel.HubPanel.autoplayTutorial.SetActive(true);
            //yield return new WaitForSecondsRealtime(MainGamePanel.HubPanel.autoplayTutorial.doTime + 2);
            //MainGamePanel.HubPanel.autoplayTutorial.SetActive(false);

            //	var btnAuto = MainGamePanel.HubPanel.btnAutoPlay;
            btnAutoPlay.onClick.RemoveListener(OnBtnHome_Pressed);
            btnAutoPlay.onClick.AddListener(OnBtnHome_Pressed);

            //s = "You can activate the Auto Button to let the Heroes automatically use the Skills";
            //	s = Localization.Get(Localization.ID.TUTORIAL_90);
            s = "";
            TutorialsManager.InputMasker.FocusToTargetImmediately(btnAutoPlay.rectTransform);
            board = MainGamePanel.ShowNotificationBoard(btnAutoPlay.rectTransform, s, Alignment.Top, new Vector2(144f, -515f), true, false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);


            yield return new WaitUntil(() => pressedBtn);

            GameplayController.Instance.ResumeGame();
            //GameplayController.Instance.SetAutoPlay(true);

            End(true);



            // btnHome.onClick.RemoveListener(OnBtnHome_Pressed);
            //   btnHome.onClick.AddListener(OnBtnHome_Pressed);
        }

        public override void End(bool pFinished)
        {
            base.End(pFinished);

            MainGamePanel.UITooltips.LockOptionsGrid(false);

            //Unlock input
            TutorialsManager.InputMasker.Active(false);

            //Hide notification board
            MainGamePanel.HideNotificationBoard(0);

            //Unlock panel
            // TutorialsManager.UnlockPanel(MainGamePanel.WinPanel);
            // TutorialsManager.UnlockPanel(MainGamePanel.LosePanel);
        }

        private void OnBtnAutoPlay_Pressed()
        {
            pressedBtn = true;
            btnAutoPlay.onClick.RemoveListener(OnBtnAutoPlay_Pressed);
        }

        private void OnBtnBarrier_DragEnd(Gesture gesture)
        {
            pressedBtn = true;
            btnBarrier.onDragEnd.RemoveListener(OnBtnBarrier_DragEnd);
        }

        private void OnBtnHome_Pressed()
        {
            //End(true);
            pressedBtn = true;
            btnAutoPlay.onClick.RemoveListener(OnBtnHome_Pressed);
        }
    }
}
