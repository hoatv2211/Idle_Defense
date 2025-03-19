using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Common;
using Utilities.Components;

namespace FoodZombie.UI
{
    public class TutorialDiscovery : TutorialController
    {
        public TutorialDiscovery(int pId, bool pIsToolTips) : base(pId, pIsToolTips)
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
            // yield return new WaitUntil(() => (MainPanel != null && MainPanel.MainMenuPanel != null
            //                                                             && MainPanel.TopPanel is MainMenuPanel));
            //
            // while (!(MainPanel.TopPanel is MainMenuPanel))
            // {
            //     MainPanel.Back();
            // }
            
            //
            // var btnDiscovery = MainPanel.MainMenuPanel.btnDiscovery.rectTransform;
            // //Focus to summon button
            // TutorialsManager.InputMasker.Lock();
            // string s = "Hello Captian, now I will introduce you to the discovery feature.";
            // var board = MainPanel.ShowNotificationBoard(btnDiscovery, s, Alignment.Bot,new Vector2(144f, -515f), false);
            // board.transform.SetParent(TutorialsManager.transform);
            // yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            //
            // TutorialsManager.InputMasker.FocusToTargetImmediately(btnDiscovery);
            // board = MainPanel.ShowNotificationBoard(btnDiscovery, "", Alignment.Bot, new Vector2(144f, -515f));
            // board.transform.SetParent(TutorialsManager.transform);
            
            yield return new WaitUntil(() => MainPanel.DiscoveryPanel.IsActiveAndEnabled());
            //đợi cho đến khi popup summon hiện lên thì khóa popup
            TutorialsManager.LockPanel(MainPanel.DiscoveryPanel);
            
            var imgLock = MainPanel.DiscoveryPanel.imgLock;
            imgLock.SetActive(true);

            TutorialsManager.InputMasker.FocusToTargetImmediately(imgLock);
            //string s = "This is where you will find various resources from mines and research facilities.";
            var s = Localization.Get(Localization.ID.TUTORIAL_19);
            var board = MainPanel.ShowNotificationBoard(imgLock, s, Alignment.Bot,new Vector2(144f, -650f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            
            TutorialsManager.InputMasker.FocusToTargetImmediately(imgLock);
            //s = "Each resource type will be open for a fixed date on the respective days of the week.";
            s = Localization.Get(Localization.ID.TUTORIAL_20);
            board = MainPanel.ShowNotificationBoard(imgLock, s, Alignment.Bot,new Vector2(144f, -650f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            
            TutorialsManager.InputMasker.FocusToTargetImmediately(imgLock);
            //s = "Captian shouldn't miss the opportunity to find the kind of resources you want.";
            s = Localization.Get(Localization.ID.TUTORIAL_21);
            board = MainPanel.ShowNotificationBoard(imgLock, s, Alignment.Bot,new Vector2(144f, -650f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            
            TutorialsManager.InputMasker.FocusToTargetImmediately(imgLock);
            //s = "Now, please experience the new feature for yourself.";
            s = Localization.Get(Localization.ID.TUTORIAL_22);
            board = MainPanel.ShowNotificationBoard(imgLock, s, Alignment.Bot,new Vector2(144f, -650f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

            // TutorialsManager.InputMasker.FocusToTargetImmediately(btnOpen);
            // board = MainPanel.ShowNotificationBoard(btnOpen, "", Alignment.Top, new Vector2(144f, -65f));
            // board.transform.SetParent(TutorialsManager.transform);
            
            yield return new WaitForSecondsRealtime(1f);

            imgLock.SetActive(false);
            TutorialsManager.UnlockPanel(MainPanel.DiscoveryPanel);

            End(true);
        }

        public override void End(bool pFinished)
        {
            base.End(pFinished);

            MainPanel.UITooltips.LockOptionsGrid(false);

            //Unlock input
            TutorialsManager.InputMasker.Active(false);

            //Hide notification board
            MainPanel.HideNotificationBoard(0);

            //Unlock panel
            TutorialsManager.UnlockPanel(MainPanel.DiscoveryPanel);
        }
    }
}
