using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Components;

namespace FoodZombie.UI
{
	public class TutorialSummonHeroX10Gameplay : TutorialController
	{
		private JustButton btnHome;

		public TutorialSummonHeroX10Gameplay(int pId, bool pIsToolTips) : base(pId, pIsToolTips)
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
											  && MainGamePanel.WinPanel.IsActiveAndEnabled()));
			//=== step 1:
			//Focus to home button
			btnHome = MainGamePanel.WinPanel.btnHome;
			TutorialsManager.InputMasker.FocusToTargetImmediately(btnHome.rectTransform);
			string s = Localization.Get(Localization.ID.TUTORIAL_92);
			//string s = "Now go back to the main screen.I will guide you on how to summon more heroes";
			var board = MainGamePanel.ShowNotificationBoard(btnHome.rectTransform, s, Alignment.Top, new Vector2(144f, -515f), true, false);
			board.transform.SetParent(TutorialsManager.transform);
		//	yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

			btnHome.onClick.RemoveListener(OnBtnHome_Pressed);
			btnHome.onClick.AddListener(OnBtnHome_Pressed);
			//End(true);
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
			End(true);
			btnHome.onClick.RemoveListener(OnBtnHome_Pressed);
		}
	}
}
