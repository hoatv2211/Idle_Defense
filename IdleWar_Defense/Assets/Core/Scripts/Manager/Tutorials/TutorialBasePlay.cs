using System;
using System.Collections;
using System.Collections.Generic;
using HedgehogTeam.EasyTouch;
using UnityEngine;
using Utilities.Common;
using Utilities.Components;

namespace FoodZombie.UI
{
    public class TutorialBasePlay : TutorialController
    {
        private JustButton btnAutoPlay;
        private JustButton btnHome;
        private ButtonItemGameplay btnBarrier;
        private bool pressedBtn;

        public TutorialBasePlay(int pId, bool pIsToolTips) : base(pId, pIsToolTips)
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


            var barrierData = GameData.Instance.BaseGroup.GetTrapData(IDs.ITEM_TRAP_BARRIER);
            var imgZoneDragForTutorial = MainGamePanel.HubPanel.imgZoneDragForTutorial;
            var stock = barrierData.StockNumber; //Check user barrier complete

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
                ////var s = "Let's me show you how to use barrier, barrier will increase the defense and tolerance for heroes.";
                //var s = Localization.Get(Localization.ID.TUTORIAL_14);
                //var board = MainGamePanel.ShowNotificationBoard(btnBarrier.rectTransform(), s, Alignment.TopRight,
                //	new Vector2(144f, -620f));
                //board.transform.SetParent(TutorialsManager.transform);
                //yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f +
                //										Config.TIME_TUTORIAL_TEXT_WAIT);

                //TutorialsManager.InputMasker.FocusToTargetImmediately(imgZoneDragForTutorial);
                ////s = "Drag the barrier and place it to position where you specified it!";
                //s = Localization.Get(Localization.ID.TUTORIAL_15);
                //board = MainGamePanel.ShowNotificationBoard(btnBarrier.rectTransform(), s, Alignment.TopRight,
                //	new Vector2(144f, -620f));
                //board.transform.SetParent(TutorialsManager.transform);
                //yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f +
                //										Config.TIME_TUTORIAL_TEXT_WAIT);

                //tắt cái tay trên tut
                //board = MainGamePanel.ShowNotificationBoard(btnBarrier.rectTransform(), "", Alignment.TopRight,
                //	new Vector2(144f, -620f), false, true);
                //board.transform.SetParent(TutorialsManager.transform);
                MainGamePanel.HubPanel.barrierTutorial1.SetActive(true);

                //tắt cái imgLock đi
                imgZoneDragForTutorial.SetActive(false);

                btnBarrier.onDragEnd.RemoveListener(OnBtnBarrier_DragEnd);
                btnBarrier.onDragEnd.AddListener(OnBtnBarrier_DragEnd);
                yield return new WaitUntil(() => (pressedBtn || !barrierData.CanUse()) && (stock > barrierData.StockNumber));
                yield return null;

                MainGamePanel.HubPanel.barrierTutorial1.SetActive(false);
            }

            //
            //if (barrierData.CanUse())
            //{
            //	pressedBtn = false;
            //	stock = barrierData.StockNumber;
            //	//bật cái imgLock lên
            //	imgZoneDragForTutorial.SetActive(true);

            //	TutorialsManager.InputMasker.FocusToTargetImmediately(imgZoneDragForTutorial);
            //	//var s = "Drag the barrier and place it to position!";
            //	var s = Localization.Get(Localization.ID.TUTORIAL_16);
            //	var board = MainGamePanel.ShowNotificationBoard(btnBarrier.rectTransform(), s, Alignment.TopRight,
            //			new Vector2(144f, -620f), false, true);
            //	board.transform.SetParent(TutorialsManager.transform);
            //	yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f +
            //											Config.TIME_TUTORIAL_TEXT_WAIT);


            //	MainGamePanel.HubPanel.barrierTutorial2.SetActive(true);
            //	//tắt cái imgLock đi
            //	imgZoneDragForTutorial.SetActive(false);



            //	btnBarrier.onDragEnd.RemoveListener(OnBtnBarrier_DragEnd);
            //	btnBarrier.onDragEnd.AddListener(OnBtnBarrier_DragEnd);
            //	yield return new WaitUntil(() => (pressedBtn || !barrierData.CanUse()) && (stock > barrierData.StockNumber));
            //	yield return null;
            //	MainGamePanel.HubPanel.barrierTutorial2.SetActive(false);
            //}
            //

            //if (barrierData.CanUse())
            //{
            //	pressedBtn = false;
            //	stock = barrierData.StockNumber;
            //	//tắt cái imgLock đi
            //	imgZoneDragForTutorial.SetActive(true);

            //	TutorialsManager.InputMasker.FocusToTargetImmediately(imgZoneDragForTutorial);
            //	//var s = "Drag the barrier and place it to position!";
            //	var s = Localization.Get(Localization.ID.TUTORIAL_17);
            //	var board = MainGamePanel.ShowNotificationBoard(btnBarrier.rectTransform(), s, Alignment.TopRight,
            //			new Vector2(144f, -620f), false, true);
            //	board.transform.SetParent(TutorialsManager.transform);
            //	yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f +
            //											Config.TIME_TUTORIAL_TEXT_WAIT);


            //	MainGamePanel.HubPanel.barrierTutorial3.SetActive(true);

            //	//tắt cái imgLock đi
            //	imgZoneDragForTutorial.SetActive(false);


            //	btnBarrier.onDragEnd.RemoveListener(OnBtnBarrier_DragEnd);
            //	btnBarrier.onDragEnd.AddListener(OnBtnBarrier_DragEnd);
            //	yield return new WaitUntil(() => (pressedBtn || !barrierData.CanUse()) && (stock > barrierData.StockNumber));
            //	yield return null;
            //	MainGamePanel.HubPanel.barrierTutorial3.SetActive(false);
            //}

            GameplayController.Instance.ResumeGame();
            //End(false);

            End(true);
            //yield return new WaitUntil(() => (MainGamePanel.WinPanel.IsActiveAndEnabled() || MainGamePanel.LosePanel.IsActiveAndEnabled()));
            ////=== step 2:
            //if (MainGamePanel.WinPanel.IsActiveAndEnabled()) btnHome = MainGamePanel.WinPanel.btnHome;
            //else btnHome = MainGamePanel.LosePanel.btnHome;
            //TutorialsManager.InputMasker.Lock();
            //s = "You have finished mission 1-2.";
            //board = MainGamePanel.ShowNotificationBoard(btnHome.rectTransform, s, Alignment.Bot, new Vector2(144f, -515f), false);
            //board.transform.SetParent(TutorialsManager.transform);
            //yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

            //TutorialsManager.InputMasker.Lock();
            //s = "Now go back to the main screen. I will guide you on how to upgrade your hero's power.";
            //board = MainGamePanel.ShowNotificationBoard(btnHome.rectTransform, s, Alignment.Bot, new Vector2(144f, -515f), false);
            //board.transform.SetParent(TutorialsManager.transform);
            //yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

            ////Focus to home button
            //if (MainGamePanel.WinPanel.IsActiveAndEnabled()) btnHome = MainGamePanel.WinPanel.btnHome;
            //else btnHome = MainGamePanel.LosePanel.btnHome;
            //TutorialsManager.InputMasker.FocusToTargetImmediately(btnHome.rectTransform);
            //board = MainGamePanel.ShowNotificationBoard(btnHome.rectTransform, "", Alignment.Top, new Vector2(144f, -515f), true, true);
            //board.transform.SetParent(TutorialsManager.transform);

            //btnHome.onClick.RemoveListener(OnBtnHome_Pressed);
            //btnHome.onClick.AddListener(OnBtnHome_Pressed);
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
        Vector3 _dragStartPos, _dragEndPos;

        private void OnBtnBarrier_DragEnd(Gesture gesture)
        {
            //   Debug.Log(Vector2.Distance(gesture.position, gesture.startPosition));
            if (Vector2.Distance(gesture.position, gesture.startPosition) < 100.0f)
                return;

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
