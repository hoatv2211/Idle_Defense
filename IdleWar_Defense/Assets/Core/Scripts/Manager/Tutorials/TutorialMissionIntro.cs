using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Common;
using Utilities.Components;

namespace FoodZombie.UI
{
    public class TutorialMissionIntro : TutorialController
    {
        private JustButton btnHeroSkill;
        private JustButton btnHome;

        private bool pressedBtn;


        public TutorialMissionIntro(int pId, bool pIsToolTips) : base(pId, pIsToolTips)
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
            yield return new WaitUntil(() => (MainGamePanel != null 
                                              && MainGamePanel.HubPanel != null
                                              && MainGamePanel.HubPanel.initialized));

            //đoạn này để cho mission này diễn ra 1 lần thôi, tránh lỗi
            GameData.TutorialsGroup.Finish(id);
            if (GameData.Instance.MissionsGroup.IsWinIntroMission())
            {
                End(true);
                yield break;
            }

            //=== step 1:
            //target to btn hero skill
            yield return new WaitForSeconds(5f);
            btnHeroSkill = null;
            pressedBtn = false;
            
            while (btnHeroSkill == null)
            {
                yield return new WaitForSeconds(0.25f);
                btnHeroSkill = MainGamePanel.HubPanel.GetBtnHeroSkill(4);
            }
            var heroes = GameplayController.Instance.GetHeroExs();
            heroes[4].FullCooldown();
            GameplayController.Instance.PauseGame();
            
            //say hello
            TutorialsManager.InputMasker.Lock();
            //string s = "Hello Captain!\nPlease help us!!!";
            var s = Localization.Get(Localization.ID.TUTORIAL_46);
            var board = MainGamePanel.ShowNotificationBoard(btnHeroSkill.rectTransform, s, Alignment.Bot,new Vector2(144f, -120f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            
            TutorialsManager.InputMasker.Lock();
            //s = "Our captain went missing. We need you to command the team now.";
            s = Localization.Get(Localization.ID.TUTORIAL_47);
            board = MainGamePanel.ShowNotificationBoard(btnHeroSkill.rectTransform, s, Alignment.Bot,new Vector2(144f, -120f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            
            TutorialsManager.InputMasker.Lock();
            //s = "Tap on the hero to play his skill.";
            s = Localization.Get(Localization.ID.TUTORIAL_48);
            board = MainGamePanel.ShowNotificationBoard(btnHeroSkill.rectTransform, s, Alignment.Bot,new Vector2(144f, -120f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

            TutorialsManager.InputMasker.FocusToTargetImmediately(btnHeroSkill.rectTransform);
            board = MainGamePanel.ShowNotificationBoard(btnHeroSkill.rectTransform, "", Alignment.Top, new Vector2(144f, -120f));
            board.transform.SetParent(TutorialsManager.transform);

            btnHeroSkill.onClick.RemoveListener(OnBtnHeroSkill_Pressed);
            btnHeroSkill.onClick.AddListener(OnBtnHeroSkill_Pressed);
            yield return new WaitUntil(() => pressedBtn);
            yield return null;

            GameplayController.Instance.ResumeGame();
            End(false);
            
            //=== step 2:
            //target to btn hero skill
            yield return new WaitForSeconds(10f);
            btnHeroSkill = null;
            pressedBtn = false;
            
            while (btnHeroSkill == null)
            {
                yield return new WaitForSeconds(0.25f);
                btnHeroSkill = MainGamePanel.HubPanel.GetBtnHeroSkill(0);
            }
            heroes[0].FullCooldown();
            GameplayController.Instance.PauseGame();
            
            //say hello
            TutorialsManager.InputMasker.Lock();
            //s = "Tap on the hero to play his skill.";
            s = Localization.Get(Localization.ID.TUTORIAL_49);
            board = MainGamePanel.ShowNotificationBoard(btnHeroSkill.rectTransform, s, Alignment.Bot,new Vector2(144f, -120f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

            TutorialsManager.InputMasker.FocusToTargetImmediately(btnHeroSkill.rectTransform);
            board = MainGamePanel.ShowNotificationBoard(btnHeroSkill.rectTransform, "", Alignment.Top, new Vector2(144f, -120f));
            board.transform.SetParent(TutorialsManager.transform);

            btnHeroSkill.onClick.RemoveListener(OnBtnHeroSkill_Pressed);
            btnHeroSkill.onClick.AddListener(OnBtnHeroSkill_Pressed);
            yield return new WaitUntil(() => pressedBtn);
            yield return null;

            GameplayController.Instance.ResumeGame();
            End(false);
            
            //=== step 3:
            //target to btn hero skill
            yield return new WaitForSeconds(5f);
            btnHeroSkill = null;
            pressedBtn = false;
            
            while (btnHeroSkill == null)
            {
                yield return new WaitForSeconds(0.25f);
                btnHeroSkill = MainGamePanel.HubPanel.GetBtnHeroSkill(5);
            }
            heroes[5].FullCooldown();
            GameplayController.Instance.PauseGame();
            
            //say hello
            TutorialsManager.InputMasker.Lock();
            //s = "Tap on the hero to play her skill.";
            s = Localization.Get(Localization.ID.TUTORIAL_50);
            board = MainGamePanel.ShowNotificationBoard(btnHeroSkill.rectTransform, s, Alignment.Bot,new Vector2(144f, -120f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

            TutorialsManager.InputMasker.FocusToTargetImmediately(btnHeroSkill.rectTransform);
            board = MainGamePanel.ShowNotificationBoard(btnHeroSkill.rectTransform, "", Alignment.Top, new Vector2(144f, -120f));
            board.transform.SetParent(TutorialsManager.transform);

            btnHeroSkill.onClick.RemoveListener(OnBtnHeroSkill_Pressed);
            btnHeroSkill.onClick.AddListener(OnBtnHeroSkill_Pressed);
            yield return new WaitUntil(() => pressedBtn);
            yield return null;

            GameplayController.Instance.ResumeGame();
            End(true);
            
            //=== step 4:
            //full skill
            yield return new WaitForSeconds(8f);
            heroes[1].FullCooldown();
            heroes[2].FullCooldown();
            heroes[3].FullCooldown();

            //=== step 4:
            yield return new WaitUntil(() => (MainGamePanel.WinPanel.IsActiveAndEnabled()
                                                  || MainGamePanel.LosePanel.IsActiveAndEnabled()));
            //Focus to home button
            if(MainGamePanel.WinPanel.IsActiveAndEnabled()) btnHome = MainGamePanel.WinPanel.btnHome;
            else btnHome = MainGamePanel.LosePanel.btnHome;
            TutorialsManager.InputMasker.FocusToTargetImmediately(btnHome.rectTransform);
            board = MainGamePanel.ShowNotificationBoard(btnHome.rectTransform, "", Alignment.Top, new Vector2(144f, -120f), true, false);
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
            // TutorialsManager.UnlockPanel(MainGamePanel.SpinningWheelPanel);
        }

        private void OnBtnHeroSkill_Pressed()
        {
            pressedBtn = true;
            btnHeroSkill.onClick.RemoveListener(OnBtnHeroSkill_Pressed);
        }
        
        private void OnBtnHome_Pressed()
        {
            End(true);
            btnHome.onClick.RemoveListener(OnBtnHome_Pressed);
        }
    }
}
