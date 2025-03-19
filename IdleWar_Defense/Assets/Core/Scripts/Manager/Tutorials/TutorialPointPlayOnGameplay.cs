using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Components;

namespace FoodZombie.UI
{
	public class TutorialPointPlayOnGameplay : TutorialController
	{
		//private JustButton btnHome;
		bool pressButton = false;
		private JustButton btnLVUP;
		public TutorialPointPlayOnGameplay(int pId, bool pIsToolTips) : base(pId, pIsToolTips)
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
														  && MainGamePanel.MissionDetailPanel.IsActiveAndEnabled()));
			//yield return new WaitUntil(() => (MainGamePanel != null && MainGamePanel.HubPanel != null
			//														&& MainGamePanel.TopPanel is HubPanel));
			string s;
			MessageWithPointer board;
			//=== step 1:
			//skip this step
			//GameplayController.Instance.PauseGame();

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

			//yield return new WaitUntil(() => (MainGamePanel.MissionDetailPanel.IsActiveAndEnabled()));
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
			if (MainGamePanel.MissionDetailPanel.IsActiveAndEnabled()) btnLVUP = MainGamePanel.MissionDetailPanel.btnPlay;
			//if (MainGamePanel.WinPanel.IsActiveAndEnabled()) btnLVUP = MainGamePanel.LosePanel.btnHome;
			//else btnHome = MainGamePanel.LosePanel.btnHome;
			TutorialsManager.InputMasker.FocusToTargetImmediately(btnLVUP.rectTransform);
			//	s = "Now go back to the main screen.I will guide you on how to summon more Heros";
			pressButton = false;
			board = MainGamePanel.ShowNotificationBoard(btnLVUP.rectTransform, "", Alignment.Top, new Vector2(144f, -515f), true, false);
			board.transform.SetParent(TutorialsManager.transform);

			btnLVUP.onClick.RemoveListener(OnBtnHome_Pressed);
			btnLVUP.onClick.AddListener(OnBtnHome_Pressed);
			yield return new WaitUntil(() => pressButton);
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



		private void OnBtnHome_Pressed()
		{
			//	End(true);
			End(true);
			btnLVUP.onClick.RemoveListener(OnBtnHome_Pressed);
			pressButton = true;
			GameData.TutorialsGroup.Finish(id,false);
			TutorialsManager.Instance.Ready();
		}
	}
}
