using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Components;

namespace FoodZombie.UI
{
	public class TutorialBase : TutorialController
	{
		private JustButton btnHeroView;
		private JustButton btnSave;
		private JustButton btnChoiceFormation;
		private JustButton btnPlay;
		private bool pressedBtn;

		public TutorialBase(int pId, bool pIsToolTips) : base(pId, pIsToolTips)
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
																		&& MainPanel.TopPanel is MainMenuPanel));

			MainMenuPanel.canShowFirstPopup = false;
			while (!(MainPanel.TopPanel is MainMenuPanel))
			{
				MainPanel.Back();
			}

			//if (GameData.Instance.CurrenciesGroup.GetValue(IDs.CURRENCY_POWER_CRYSTAL) < 10)
			//{
			//	End(true);
			//	yield break;
			//}


			TutorialsManager.InputMasker.Lock();
			string s = "";
			MessageWithPointer board = null;

			//base
			var baseDefinitions = GameData.Instance.BaseGroup.BaseDefinitions;
			var totalCoin = baseDefinitions[0].levelUpCost + baseDefinitions[1].levelUpCost + baseDefinitions[2].levelUpCost;
			var currentCoin = GameData.Instance.CurrenciesGroup.GetValue(IDs.CURRENCY_COIN);
			if (currentCoin < totalCoin)
			{
				GameData.Instance.CurrenciesGroup.Add(IDs.CURRENCY_COIN, totalCoin - currentCoin, TrackingConstants.VALUE_TUTORIAL);
			}

			var btnBase = MainPanel.MainMenuPanel.btnBase.rectTransform();
			TutorialsManager.InputMasker.Lock();
			//s = "This is the base where our defenses are upgraded.";
			//s = Localization.Get(Localization.ID.TUTORIAL_86);
			//board = MainPanel.ShowNotificationBoard(btnBase, s, Alignment.Bot, new Vector2(144f, -515f), false);
			//board.transform.SetParent(TutorialsManager.transform);
			//yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

			TutorialsManager.InputMasker.FocusToTargetImmediately(btnBase);
			board = MainPanel.ShowNotificationBoard(btnBase, "", Alignment.Top, new Vector2(144f, -515f));
			board.transform.SetParent(TutorialsManager.transform);

			yield return new WaitUntil(() => MainPanel.BasePanel.IsActiveAndEnabled());
			//đợi cho đến khi popup summon hiện lên thì khóa popup
			TutorialsManager.LockPanel(MainPanel.BasePanel);

			var btnUpgradeBase = MainPanel.BasePanel.btnUpgradeBase.rectTransform;
			TutorialsManager.InputMasker.Lock();
			//var s = "This is the base where our defenses are upgraded.";
			//s = Localization.Get(Localization.ID.TUTORIAL_10);
			//board = MainPanel.ShowNotificationBoard(btnUpgradeBase, s, Alignment.Bot, new Vector2(144f, 335f), false);
			//board.transform.SetParent(TutorialsManager.transform);
			//yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

			//TutorialsManager.InputMasker.Lock();
			////s = "Upgrade your Base to unlock more defensive positions and place heroes and unlock weapons to make killing enemies easier.";
			//s = Localization.Get(Localization.ID.TUTORIAL_11);
			//board = MainPanel.ShowNotificationBoard(btnUpgradeBase, s, Alignment.Bot, new Vector2(144f, 335f), false);
			//board.transform.SetParent(TutorialsManager.transform);
			//yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

			//TutorialsManager.InputMasker.Lock();
			////s = "Now try to upgrade your Base to level 3.";
			//s = Localization.Get(Localization.ID.TUTORIAL_12);
			//board = MainPanel.ShowNotificationBoard(btnUpgradeBase, s, Alignment.Bot, new Vector2(144f, 335f), false);
			//board.transform.SetParent(TutorialsManager.transform);
			//yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

			TutorialsManager.InputMasker.FocusToTargetImmediately(btnUpgradeBase);
			board = MainPanel.ShowNotificationBoard(btnUpgradeBase, "", Alignment.Bot, new Vector2(144f, 335f));
			board.transform.SetParent(TutorialsManager.transform);

			var baseGroup = GameData.Instance.BaseGroup;
			yield return new WaitUntil(() => baseGroup.Level >= 3);

			//Barrier
			var barrier = GameData.Instance.BaseGroup.GetTrapData(IDs.ITEM_TRAP_BARRIER);
			totalCoin = barrier.GetLevelUpCoinByLevel(1) + barrier.GetLevelUpCoinByLevel(1) + barrier.GetLevelUpCoinByLevel(2);
			currentCoin = GameData.Instance.CurrenciesGroup.GetValue(IDs.CURRENCY_COIN);
			if (currentCoin < totalCoin)
			{
				GameData.Instance.CurrenciesGroup.Add(IDs.CURRENCY_COIN, totalCoin - currentCoin, TrackingConstants.VALUE_TUTORIAL);
			}

			var btnUpgradeBarrier = MainPanel.BasePanel.trapViews[0].btnUpgrade.rectTransform;
			TutorialsManager.InputMasker.Lock();
			//s = "Barrier is the main shield to help block the enemy's steps. Next, upgrade barrier to level 3.";
			//s = Localization.Get(Localization.ID.TUTORIAL_13);
			//board = MainPanel.ShowNotificationBoard(btnUpgradeBarrier, s, Alignment.Bot, new Vector2(144f, 50f), false);
			//board.transform.SetParent(TutorialsManager.transform);
			//yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

			TutorialsManager.InputMasker.FocusToTargetImmediately(btnUpgradeBarrier);
			board = MainPanel.ShowNotificationBoard(btnUpgradeBarrier, "", Alignment.Bot, new Vector2(144f, 50f));
			board.transform.SetParent(TutorialsManager.transform);

			var barrierData = GameData.Instance.BaseGroup.GetTrapData(IDs.ITEM_TRAP_BARRIER);
			yield return new WaitUntil(() => barrierData.Level >= 3);

			yield return BackAndBattle(MainPanel.BasePanel.btnMain, MainPanel.BasePanel);
			//Khong back nua ,End luon
			MainMenuPanel.canShowFirstPopup = true;
			End(true);
			//================================================


		}
		private void OnBtnOneClickEquip_Pressed()
		{
			pressedBtn = true;
			MainPanel.FormationPanel.btnAuto.onClick.RemoveListener(OnBtnOneClickEquip_Pressed);
			//btnOneClickEquip.onClick.RemoveListener(OnBtnOneClickEquip_Pressed);
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
			TutorialsManager.UnlockPanel(MainPanel.BasePanel);
		}
	}
}
