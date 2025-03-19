using System;
using System.Collections;
using System.Collections.Generic;
using HedgehogTeam.EasyTouch;
using UnityEngine;
using Utilities.Common;
using Utilities.Components;

namespace FoodZombie.UI
{
    public class TutorialAutoSkillGameplay : TutorialController
    {
        private JustButton btnAutoPlay;
        private JustButton btnHome;
        private ButtonItemGameplay btnBarrier;
        private bool pressedBtn;

        public TutorialAutoSkillGameplay(int pId, bool pIsToolTips) : base(pId, pIsToolTips)
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
            yield return new WaitUntil(() => (MainGamePanel != null && MainGamePanel.HubPanel != null
                                                                && MainGamePanel.TopPanel is HubPanel));
            //=== step 1:
            pressedBtn = false;
            GameplayController.Instance.PauseGame();
            
            btnAutoPlay = MainGamePanel.HubPanel.btnAutoPlay;
            TutorialsManager.InputMasker.Lock();
            //string s = "The auto cast skill feature has appeared. Use it to free yourself while your heroes are fighting.";
            var s = Localization.Get(Localization.ID.TUTORIAL_78);
            var board = MainGamePanel.ShowNotificationBoard(btnAutoPlay.rectTransform, s, Alignment.Bot,new Vector2(144f, -515f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            
            TutorialsManager.InputMasker.Lock();
            //s = "Press the auto button to use it. Now enjoy it.";
            s = Localization.Get(Localization.ID.TUTORIAL_79);
            board = MainGamePanel.ShowNotificationBoard(btnAutoPlay.rectTransform, s, Alignment.Bot,new Vector2(144f, -515f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            
            TutorialsManager.InputMasker.FocusToTargetImmediately(btnAutoPlay.rectTransform);
            board = MainGamePanel.ShowNotificationBoard(btnAutoPlay.rectTransform, "", Alignment.TopRight, new Vector2(144f, -515f));
            board.transform.SetParent(TutorialsManager.transform);
            btnAutoPlay.onClick.RemoveListener(OnBtnAutoPlay_Pressed);
            btnAutoPlay.onClick.AddListener(OnBtnAutoPlay_Pressed);
            yield return new WaitUntil(() => pressedBtn);
            yield return null;

            GameplayController.Instance.ResumeGame();
            End(false);

            if (TutorialsManager.tutorialActive)
            {
                yield return new WaitForSeconds(20f);

                var barrierData = GameData.Instance.BaseGroup.GetTrapData(IDs.ITEM_TRAP_BARRIER);
                var imgZoneDragForTutorial = MainGamePanel.HubPanel.imgZoneDragForTutorial;
                if (barrierData.CanUse())
                {
                    //
                    pressedBtn = false;
                    GameplayController.Instance.PauseGame();
                    GameplayController.Instance.ShowTrapDatas();

                    //bật cái imgLock lên
                    imgZoneDragForTutorial.SetActive(true);

                    btnBarrier = MainGamePanel.HubPanel.btnBarrier;
                    TutorialsManager.InputMasker.FocusToTargetImmediately(imgZoneDragForTutorial);
                    //s = "Here. I give you 5 barrier, barrier will increase the defense and tolerance for heroes.";
                    s = Localization.Get(Localization.ID.TUTORIAL_80);
                    board = MainGamePanel.ShowNotificationBoard(btnBarrier.rectTransform(), s, Alignment.TopRight,
                        new Vector2(144f, -620f));
                    board.transform.SetParent(TutorialsManager.transform);
                    yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f +
                                                            Config.TIME_TUTORIAL_TEXT_WAIT);

                    TutorialsManager.InputMasker.FocusToTargetImmediately(imgZoneDragForTutorial);
                    //s = "Drag the barrier and place it to position where you specified it!";
                    s = Localization.Get(Localization.ID.TUTORIAL_81);
                    board = MainGamePanel.ShowNotificationBoard(btnBarrier.rectTransform(), s, Alignment.TopRight,
                        new Vector2(144f, -620f));
                    board.transform.SetParent(TutorialsManager.transform);
                    yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f +
                                                            Config.TIME_TUTORIAL_TEXT_WAIT);

                    //tắt cái imgLock đi
                    imgZoneDragForTutorial.SetActive(false);

                    //tắt cái tay trên tut
                    board = MainGamePanel.ShowNotificationBoard(btnBarrier.rectTransform(), "", Alignment.TopRight,
                        new Vector2(144f, -620f), false, true);
                    board.transform.SetParent(TutorialsManager.transform);
                    MainGamePanel.HubPanel.barrierTutorial1.SetActive(true);

                    btnBarrier.onDragEnd.RemoveListener(OnBtnBarrier_DragEnd);
                    btnBarrier.onDragEnd.AddListener(OnBtnBarrier_DragEnd);
                    yield return new WaitUntil(() => pressedBtn || !barrierData.CanUse());
                    yield return null;
                    MainGamePanel.HubPanel.barrierTutorial1.SetActive(false);
                }

                //
                if (barrierData.CanUse())
                {
                    pressedBtn = false;

                    //bật cái imgLock lên
                    imgZoneDragForTutorial.SetActive(true);

                    TutorialsManager.InputMasker.FocusToTargetImmediately(imgZoneDragForTutorial);
                    //s = "Drag the barrier and place it to position!";
                    s = Localization.Get(Localization.ID.TUTORIAL_82);
                    board = MainGamePanel.ShowNotificationBoard(btnBarrier.rectTransform(), s, Alignment.TopRight,
                        new Vector2(144f, -620f), false, true);
                    board.transform.SetParent(TutorialsManager.transform);
                    yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f +
                                                            Config.TIME_TUTORIAL_TEXT_WAIT);

                    //tắt cái imgLock đi
                    imgZoneDragForTutorial.SetActive(false);

                    MainGamePanel.HubPanel.barrierTutorial2.SetActive(true);

                    btnBarrier.onDragEnd.RemoveListener(OnBtnBarrier_DragEnd);
                    btnBarrier.onDragEnd.AddListener(OnBtnBarrier_DragEnd);
                    yield return new WaitUntil(() => pressedBtn || !barrierData.CanUse());
                    yield return null;
                    MainGamePanel.HubPanel.barrierTutorial2.SetActive(false);
                }
                //

                if (barrierData.CanUse())
                {
                    pressedBtn = false;

                    //tắt cái imgLock đi
                    imgZoneDragForTutorial.SetActive(false);

                    TutorialsManager.InputMasker.FocusToTargetImmediately(imgZoneDragForTutorial);
                    //s = "Drag the barrier and place it to position!";
                    s = Localization.Get(Localization.ID.TUTORIAL_83);
                    board = MainGamePanel.ShowNotificationBoard(btnBarrier.rectTransform(), s, Alignment.TopRight,
                        new Vector2(144f, -620f), false, true);
                    board.transform.SetParent(TutorialsManager.transform);
                    yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f +
                                                            Config.TIME_TUTORIAL_TEXT_WAIT);

                    //tắt cái imgLock đi
                    imgZoneDragForTutorial.SetActive(false);

                    MainGamePanel.HubPanel.barrierTutorial3.SetActive(true);

                    btnBarrier.onDragEnd.RemoveListener(OnBtnBarrier_DragEnd);
                    btnBarrier.onDragEnd.AddListener(OnBtnBarrier_DragEnd);
                    yield return new WaitUntil(() => pressedBtn || !barrierData.CanUse());
                    yield return null;
                    MainGamePanel.HubPanel.barrierTutorial3.SetActive(false);
                }

                GameplayController.Instance.ResumeGame();
                End(false);
            }
            else
            {
                //bật trap lên
                GameplayController.Instance.ShowTrapDatas();
            }

            yield return new WaitUntil(() => (MainGamePanel.WinPanel.IsActiveAndEnabled() || MainGamePanel.LosePanel.IsActiveAndEnabled()));
            //=== step 2:
            if(MainGamePanel.WinPanel.IsActiveAndEnabled()) btnHome = MainGamePanel.WinPanel.btnHome;
            else btnHome = MainGamePanel.LosePanel.btnHome;
            TutorialsManager.InputMasker.Lock();
            //s = "You have finished mission 1-2.";
            s = Localization.Get(Localization.ID.TUTORIAL_84);
            board = MainGamePanel.ShowNotificationBoard(btnHome.rectTransform, s, Alignment.Bot,new Vector2(144f, -515f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            
            TutorialsManager.InputMasker.Lock();
            //s = "Now go back to the main screen. I will guide you on how to upgrade your hero's power.";
            s = Localization.Get(Localization.ID.TUTORIAL_85);
            board = MainGamePanel.ShowNotificationBoard(btnHome.rectTransform, s, Alignment.Bot,new Vector2(144f, -515f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            
            //Focus to home button
            if(MainGamePanel.WinPanel.IsActiveAndEnabled()) btnHome = MainGamePanel.WinPanel.btnHome;
            else btnHome = MainGamePanel.LosePanel.btnHome;
            TutorialsManager.InputMasker.FocusToTargetImmediately(btnHome.rectTransform);
            board = MainGamePanel.ShowNotificationBoard(btnHome.rectTransform, "", Alignment.Top, new Vector2(144f, -515f), true, true);
            board.transform.SetParent(TutorialsManager.transform);
            
            btnHome.onClick.RemoveListener(OnBtnHome_Pressed);
            btnHome.onClick.AddListener(OnBtnHome_Pressed);
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
            End(true);
            btnHome.onClick.RemoveListener(OnBtnHome_Pressed);
        }
    }
}
