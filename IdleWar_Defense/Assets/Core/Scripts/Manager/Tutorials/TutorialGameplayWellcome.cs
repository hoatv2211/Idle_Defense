using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Components;

namespace FoodZombie.UI
{
    public class TutorialGameplayWellcome : TutorialController
    {
        private JustButton btnHome;
        private JustButton btnHeroSkill;
        private bool pressedBtn;
        public TutorialGameplayWellcome(int pId, bool pIsToolTips) : base(pId, pIsToolTips)
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
            //yield return new WaitUntil(() => (MainGamePanel != null
            //											  && MainGamePanel.LosePanel.IsActiveAndEnabled()));
            yield return new WaitUntil(() => (MainGamePanel != null && MainGamePanel.HubPanel != null
                                                                    && MainGamePanel.TopPanel is HubPanel));
            string s;
            MessageWithPointer board;
            //=== step 1:
            //skip this step



            btnHeroSkill = MainGamePanel.HubPanel.GetBtnHeroSkillHard(0);
            while (btnHeroSkill == null)
            {
                yield return new WaitForSeconds(0.25f);
                btnHeroSkill = MainGamePanel.HubPanel.GetBtnHeroSkillHard(0);
            }
            //GameplayController.Instance.PauseGame();
            ////	s = "Your Heroes will automatically attack normally";
            //s = Localization.Get(Localization.ID.TUTORIAL_93);
            //board = MainGamePanel.ShowNotificationBoard(btnHeroSkill.rectTransform, s, Alignment.Top, new Vector2(144f, -515f), false, true);
            //board.ActiveSkip(() =>
            //{
            //    board = MainGamePanel.ShowNotificationBoard(btnHeroSkill.rectTransform, "", Alignment.Top, new Vector2(144f, -215f), false, false, false);
            //    TutorialsManager.Instance.SkipTutorial(this);
            //    pressedBtn = true;
            //    btnHeroSkill.onClick.RemoveListener(OnBtnNext_Pressed);
            //    GameplayController.Instance.ResumeGame();
            //    //  yield break;
            //    // End(true);
            //});
            //board.transform.SetParent(TutorialsManager.transform);
            //yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

            ////   s = "This's the Hero's Skill Bar";
            //s = Localization.Get(Localization.ID.TUTORIAL_94);
            //board = MainGamePanel.ShowNotificationBoard(btnHeroSkill.rectTransform, s, Alignment.Top, new Vector2(144f, -515f), true, true, true);
            //board.transform.SetParent(TutorialsManager.transform);
            //yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

            ////  s = "When The Skill Bar is full\nYou can activate the skill";
            //s = Localization.Get(Localization.ID.TUTORIAL_95);
            //board = MainGamePanel.ShowNotificationBoard(btnHeroSkill.rectTransform, s, Alignment.Top, new Vector2(144f, -515f), true, true, true);
            //board.transform.SetParent(TutorialsManager.transform);
            //yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

            //GameplayController.Instance.ResumeGame();
            //btnX2Speed = MainGamePanel.HubPanel.btnTimeScale;
            //TutorialsManager.InputMasker.Lock();
            //s = "The X2 speed feature is open, press the X2 Speed button to enjoy the feeling of a more intense battle.";
            // var board = MainGamePanel.ShowNotificationBoard(btnX2Speed.rectTransform, s, Alignment.Bot,new Vector2(144f, -515f), false);
            //board.transform.SetParent(TutorialsManager.transform);
            //yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

            //TutorialsManager.InputMasker.FocusToTargetImmediately(btnX2Speed.rectTransform);
            //board = MainGamePanel.ShowNotificationBoard(btnX2Speed.rectTransform, "", Alignment.TopRight, new Vector2(144f, -515f), true, true);
            //board.transform.SetParent(TutorialsManager.transform);
            //btnX2Speed.onClick.RemoveListener(OnBtnX2Speed_Pressed);
            //btnX2Speed.onClick.AddListener(OnBtnX2Speed_Pressed);

            //TutorialsManager.InputMasker.FocusToTargetImmediately(btnHeroSkill.rectTransform);
            //board = MainGamePanel.ShowNotificationBoard(btnHeroSkill.rectTransform, "", Alignment.Top, new Vector2(144f, -215f), false, false, true);
            //board.transform.SetParent(TutorialsManager.transform);


            yield return new WaitUntil(() => (MainGamePanel.HubPanel.HaveHeroSkillFull()));
            pressedBtn = false;
            GameplayController.Instance.PauseGame();
            //  s = "The Skill Bar is FULL\nactivate the skill NOW!";
            //s = Localization.Get(Localization.ID.TUTORIAL_96);
            TutorialsManager.InputMasker.FocusToTargetImmediately(btnHeroSkill.rectTransform);
            s = "";
            board = MainGamePanel.ShowNotificationBoard(btnHeroSkill.rectTransform, s, Alignment.Top, new Vector2(144f, -515f), true, false, true);
            board.transform.SetParent(TutorialsManager.transform);
            btnHeroSkill.onClick.AddListener(OnBtnNext_Pressed);
            yield return new WaitUntil(() => (pressedBtn));

            board = MainGamePanel.ShowNotificationBoard(btnHeroSkill.rectTransform, "", Alignment.Top, new Vector2(144f, -215f), false, false, false);
            board.transform.SetParent(TutorialsManager.transform);
            End(true);


            //=== step 1:
            //say hello
            /*	if (MainGamePanel.WinPanel.IsActiveAndEnabled()) btnHome = MainGamePanel.WinPanel.btnHome;
				else btnHome = MainGamePanel.LosePanel.btnHome;
				TutorialsManager.InputMasker.Lock();

				s = "You have finished mission 1-1.";
				board = MainGamePanel.ShowNotificationBoard(btnHome.rectTransform, s, Alignment.Bot, new Vector2(144f, -515f), false);
				board.transform.SetParent(TutorialsManager.transform);
				yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

				TutorialsManager.InputMasker.Lock();
				s = "Now go back to the main screen I will give you a special gift.";
				board = MainGamePanel.ShowNotificationBoard(btnHome.rectTransform, s, Alignment.Bot, new Vector2(144f, -515f), false);
				board.transform.SetParent(TutorialsManager.transform);
				yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
	*/
            //Focus to home button
            //if (MainGamePanel.LosePanel.IsActiveAndEnabled()) btnLVUP = MainGamePanel.LosePanel.btnLevelUpHero;
            //else btnHome = MainGamePanel.LosePanel.btnHome;
            //TutorialsManager.InputMasker.FocusToTargetImmediately(btnLVUP.rectTransform);

            //s = "Hey,Don't giveup! Let's Level-Up our heros for more power";
            //board = MainGamePanel.ShowNotificationBoard(btnHeroSkill.rectTransform, s, Alignment.Top, new Vector2(144f, -515f), true, true);
            //board.transform.SetParent(TutorialsManager.transform);

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



        private void OnBtnNext_Pressed()
        {
            GameplayController.Instance.ResumeGame();
            pressedBtn = true;
            btnHeroSkill.onClick.RemoveListener(OnBtnNext_Pressed);
            //End(true);
        }
        private void OnBtnHome_Pressed()
        {
            End(true);
            btnHome.onClick.RemoveListener(OnBtnHome_Pressed);
        }
    }
}
