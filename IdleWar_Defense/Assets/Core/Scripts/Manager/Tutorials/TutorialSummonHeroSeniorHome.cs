using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Components;

namespace FoodZombie.UI
{
	public class TutorialSummonHeroSeniorHome : TutorialController
	{
		private JustButton btnHeroView;
		private JustButton btnSave;
		private JustButton btnChoiceFormation;
		private JustButton btnPlay;
		private bool pressedBtn;

		public TutorialSummonHeroSeniorHome(int pId, bool pIsToolTips) : base(pId, pIsToolTips)
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
			yield return new WaitUntil(() => (MainPanel != null && MainPanel.MainMenuPanel != null
																&& MainPanel.TopPanel is SummonGatePanel));

			while (!(MainPanel.TopPanel is SummonGatePanel))
			{
				MainPanel.Back();
			}

			//bỏ đoạn cho một mảnh, thay bằng 1 lượt free mỗi ngày
			//nếu không có thì next tut luôn
			MessageWithPointer board;
			string s;
			if (GameData.Instance.HeroesGroup.FreePowerCrystalSummon)
			{
				//=== step 1:
				//Focus to summon button
				//var imgSummonGate = MainPanel.MainMenuPanel.imgSummonGate;
				//TutorialsManager.InputMasker.Lock();
				//s = "Here.\nYour gift is a Power Crystal.";
				//board = MainPanel.ShowNotificationBoard(imgSummonGate, s, Alignment.Bot, new Vector2(144f, -515f), false);
				//board.transform.SetParent(TutorialsManager.transform);
				//yield return new WaitForSeconds(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

				//TutorialsManager.InputMasker.FocusToTargetImmediately(imgSummonGate);
				//board = MainPanel.ShowNotificationBoard(imgSummonGate, "", Alignment.Bot, new Vector2(144f, -515f), true);
				//board.transform.SetParent(TutorialsManager.transform);

				yield return new WaitUntil(() => MainPanel.SummonGatePanel.IsActiveAndEnabled());
				//đợi cho đến khi popup summon hiện lên thì khóa popup
				TutorialsManager.LockPanel(MainPanel.SummonGatePanel);

				//=== step 2:
				var btnPowerCrytal = MainPanel.SummonGatePanel.btnPowerCrytal.rectTransform;
				TutorialsManager.InputMasker.Lock();
				//s = "Hello Captain.Let me guide you to use Summon Gate";
				s = Localization.Get(Localization.ID.TUTORIAL_64);
				board = MainPanel.ShowNotificationBoard(btnPowerCrytal, s, Alignment.Bot, new Vector2(144f, -315f), false);
				board.transform.SetParent(TutorialsManager.transform);
				yield return new WaitForSeconds(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);


				//s = "Using Power Crystal you can summon more powerful heroes.";
				s = Localization.Get(Localization.ID.TUTORIAL_65);
				board = MainPanel.ShowNotificationBoard(btnPowerCrytal, s, Alignment.Bot, new Vector2(144f, -315f), false);
				board.transform.SetParent(TutorialsManager.transform);
				yield return new WaitForSeconds(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
				//focus to basic summon
				// //cho một mảnh mà có tiền summon
				// GameData.CurrenciesGroup.Add(IDs.CURRENCY_POWER_CRYSTAL, 1);

				TutorialsManager.InputMasker.FocusToTargetImmediately(btnPowerCrytal);
				board = MainPanel.ShowNotificationBoard(btnPowerCrytal, "", Alignment.Bot, new Vector2(144f, -315f));
				board.transform.SetParent(TutorialsManager.transform);


				yield return new WaitUntil(() => MainPanel.SummonGatePanel.isOpeningGem);
				End(false);
				//   TutorialsManager.InputMasker.Lock();
				yield return new WaitUntil(() => MainPanel.RewardsPopup.IsActiveAndEnabled());
				yield return null;
				yield return new WaitUntil(() => !MainPanel.RewardsPopup.IsActiveAndEnabled());

				//=== step 3:
				var btnBack = MainPanel.SummonGatePanel.btnBack.rectTransform();
				TutorialsManager.InputMasker.Lock();
				//s = "Now go back to Main Menu screen";
				s = Localization.Get(Localization.ID.TUTORIAL_66);
				board = MainPanel.ShowNotificationBoard(btnBack, s, Alignment.Bot, new Vector2(144f, -315f), false);
				board.transform.SetParent(TutorialsManager.transform);
				yield return new WaitForSeconds(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
				GameData.Instance.UserGroup.RateCanOpen = false;
				//close summon popup
				TutorialsManager.InputMasker.FocusToTargetImmediately(btnBack);
				board = MainPanel.ShowNotificationBoard(btnBack, "", Alignment.Top, new Vector2(144f, -315f));
				board.transform.SetParent(TutorialsManager.transform);

				yield return new WaitUntil(() => !MainPanel.SummonGatePanel.IsActiveAndEnabled());
			}

			bool hasHeroInFormation = false;
			var formation = GameData.Instance.HeroesGroup.GetEquippedHeroes();
			if (formation[1] != null)
			{
				hasHeroInFormation = true;
			}

			if (!hasHeroInFormation)
			{
				//=== step 4:
				var btnFormation = MainPanel.MainMenuPanel.btnFormation.rectTransform;
				TutorialsManager.InputMasker.Lock();
				//s = "Then. Please arrange the newly summoned hero into your defensive formation.";
				s = Localization.Get(Localization.ID.TUTORIAL_67);
				board = MainPanel.ShowNotificationBoard(btnFormation, s, Alignment.Bot, new Vector2(144f, -315f), false);
				board.transform.SetParent(TutorialsManager.transform);
				yield return new WaitForSeconds(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

				//open formation popup
				TutorialsManager.InputMasker.FocusToTargetImmediately(btnFormation);
				board = MainPanel.ShowNotificationBoard(btnFormation, "", Alignment.Top, new Vector2(144f, -315f));
				board.transform.SetParent(TutorialsManager.transform);

				yield return new WaitUntil(() => MainPanel.FormationPanel.IsActiveAndEnabled());
				//đợi cho đến khi popup FormationPanel hiện lên thì khóa popup
				TutorialsManager.LockPanel(MainPanel.FormationPanel);

				//=== step 5:
				btnHeroView = MainPanel.FormationPanel.HerosInListScrollerController.GetFirstCellSelect();
				var btnHeroViewRectTransform = btnHeroView.rectTransform;
				TutorialsManager.InputMasker.Lock();
				s = "Click hero " + GameData.Instance.HeroesGroup.GetHeroDefinition(Constants.SUMMON_HERO_ID_2).name + ", they will automatically be assigned to the empty position in formation.";
				board = MainPanel.ShowNotificationBoard(btnHeroViewRectTransform, s, Alignment.Bot, new Vector2(144f, -745f), false);
				board.transform.SetParent(TutorialsManager.transform);
				yield return new WaitForSeconds(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
				//choice hero
				pressedBtn = false;
				TutorialsManager.InputMasker.FocusToTargetImmediately(btnHeroViewRectTransform);
				board = MainPanel.ShowNotificationBoard(btnHeroViewRectTransform, "", Alignment.TopRight, new Vector2(144f, -745f));
				board.transform.SetParent(TutorialsManager.transform);

				btnHeroView.onClick.RemoveListener(OnBtnHeroView_Pressed);
				btnHeroView.onClick.AddListener(OnBtnHeroView_Pressed);
				yield return new WaitUntil(() => pressedBtn);
				yield return null;

				// //=== step 6:
				// //save formation
				// pressedBtn = false;
				// btnSave = MainPanel.FormationPanel.btnSave;
				// TutorialsManager.InputMasker.FocusToTargetImmediately(btnSave.rectTransform);
				// board = MainPanel.ShowNotificationBoard(btnSave.rectTransform, "", Alignment.Bot, new Vector2(480, 150));
				// board.transform.SetParent(TutorialsManager.transform);
				//
				// btnSave.onClick.RemoveListener(OnBtnSave_Pressed);
				// btnSave.onClick.AddListener(OnBtnSave_Pressed);
				// yield return new WaitUntil(() => pressedBtn);
				// yield return null;

				//=== step 7:
				//var btnMain = MainPanel.FormationPanel.btnMain.rectTransform;
				//TutorialsManager.InputMasker.Lock();
				//s = "Now let's challenge with a more difficult battle.";
				//board = MainPanel.ShowNotificationBoard(btnMain, s, Alignment.Bot, new Vector2(144f, -515f), false);
				//board.transform.SetParent(TutorialsManager.transform);
				//yield return new WaitForSeconds(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

				////close formation popup
				//TutorialsManager.InputMasker.FocusToTargetImmediately(btnMain);
				//board = MainPanel.ShowNotificationBoard(btnMain, "", Alignment.Top, new Vector2(144f, -515f), true);
				//board.transform.SetParent(TutorialsManager.transform);

				//TutorialsManager.UnlockPanel(MainPanel.FormationPanel);
				//yield return new WaitUntil(() => !MainPanel.FormationPanel.IsActiveAndEnabled());

				//Khong play mission thi End Luon
				End(true);
			}

			//=== step 8:
			//open map popup
			//var btnFight = MainPanel.MainMenuPanel.btnFight;
			//TutorialsManager.InputMasker.FocusToTargetImmediately(btnFight.rectTransform);
			//board = MainPanel.ShowNotificationBoard(btnFight.rectTransform, "", Alignment.Top, new Vector2(144f, -215f), true, false);
			//board.transform.SetParent(TutorialsManager.transform);

			//yield return new WaitUntil(() => MainPanel.MapPanel.IsActiveAndEnabled());
			//TutorialsManager.LockPanel(MainPanel.MapPanel);

			////=== step 9:
			////open mission detail popup
			//JustButton btnMissionPlay = null;
			//var btnMissions = MainPanel.MapPanel.btnMissions;
			//var count = btnMissions.Length;
			//for (int i = 0; i < count; i++)
			//{
			//    var btnMission = btnMissions[i];
			//    if (btnMission.btnPlay.Enabled())
			//    {
			//        btnMissionPlay = btnMission.btnPlay;
			//        break;
			//    }
			//}
			//TutorialsManager.InputMasker.FocusToTargetImmediately(btnMissionPlay.rectTransform);
			//board = MainPanel.ShowNotificationBoard(btnMissionPlay.rectTransform, "", Alignment.Bot, new Vector2(144f, -215f), true, false);
			//board.transform.SetParent(TutorialsManager.transform);

			//yield return new WaitUntil(() => MainPanel.MissionDetailPanel.IsActiveAndEnabled());
			//TutorialsManager.LockPanel(MainPanel.MissionDetailPanel);

			// //=== step 10:
			// //choice formation
			// var btnChange = MainPanel.MissionDetailPanel.btnChange;
			// TutorialsManager.InputMasker.FocusToTargetImmediately(btnChange.rectTransform);
			// board = MainPanel.ShowNotificationBoard(btnChange.rectTransform, "", Alignment.Top, new Vector2(480, 150));
			// board.transform.SetParent(TutorialsManager.transform);
			//
			// yield return new WaitUntil(() => MainPanel.MissionDetailPanel.groupFormation.activeSelf);
			//
			// //=== step 10:
			// //choice formation toggle
			// pressedBtn = false;
			// btnChoiceFormation = MainPanel.MissionDetailPanel.btnChoiceFormations[0];
			// TutorialsManager.InputMasker.FocusToTargetImmediately(btnChoiceFormation.rectTransform);
			// board = MainPanel.ShowNotificationBoard(btnChoiceFormation.rectTransform, "", Alignment.BotRight, new Vector2(480, 150));
			// board.transform.SetParent(TutorialsManager.transform);
			//
			// btnChoiceFormation.onClick.RemoveListener(OnBtnChoiceFormation_Pressed);
			// btnChoiceFormation.onClick.AddListener(OnBtnChoiceFormation_Pressed);
			// yield return new WaitUntil(() => pressedBtn);
			// yield return null;
			//
			// //=== step 11:
			// //choice formation save
			// var btnSaveFormation = MainPanel.MissionDetailPanel.btnSaveFormation;
			// TutorialsManager.InputMasker.FocusToTargetImmediately(btnSaveFormation.rectTransform);
			// board = MainPanel.ShowNotificationBoard(btnSaveFormation.rectTransform, "", Alignment.Bot, new Vector2(480, 150));
			// board.transform.SetParent(TutorialsManager.transform);
			//
			// yield return new WaitUntil(() => !MainPanel.MissionDetailPanel.groupFormation.activeSelf);

			//=== step 11:
			//PLAY MISSIONNNNNNNNNNNNNNNN
			//pressedBtn = false;
			//btnPlay = MainPanel.MissionDetailPanel.btnPlay;
			//TutorialsManager.InputMasker.FocusToTargetImmediately(btnPlay.rectTransform);
			//board = MainPanel.ShowNotificationBoard(btnPlay.rectTransform, "", Alignment.Bot, new Vector2(144f, -215f), true, false);
			//board.transform.SetParent(TutorialsManager.transform);

			//btnPlay.onClick.RemoveListener(OnBtnPlay_Pressed);
			//btnPlay.onClick.AddListener(OnBtnPlay_Pressed);
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
			TutorialsManager.UnlockPanel(MainPanel.SummonGatePanel);
			TutorialsManager.UnlockPanel(MainPanel.FormationPanel);
			TutorialsManager.UnlockPanel(MainPanel.MapPanel);
			TutorialsManager.UnlockPanel(MainPanel.MissionDetailPanel);
		}

		private void OnBtnHeroView_Pressed()
		{
			pressedBtn = true;
			btnHeroView.onClick.RemoveListener(OnBtnHeroView_Pressed);
		}

		private void OnBtnSave_Pressed()
		{
			pressedBtn = true;
			btnSave.onClick.RemoveListener(OnBtnSave_Pressed);
		}

		private void OnBtnChoiceFormation_Pressed()
		{
			pressedBtn = true;
			btnChoiceFormation.onClick.RemoveListener(OnBtnChoiceFormation_Pressed);
		}

		private void OnBtnPlay_Pressed()
		{
			End(true);
			btnPlay.onClick.RemoveListener(OnBtnPlay_Pressed);
		}
	}
}
