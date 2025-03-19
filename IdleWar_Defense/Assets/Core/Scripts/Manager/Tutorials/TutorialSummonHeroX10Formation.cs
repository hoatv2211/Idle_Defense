using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Components;

namespace FoodZombie.UI
{
    public class TutorialSummonHeroX10formation : TutorialController
    {
        private JustButton btnHeroView;
        private JustButton btnSave;
        private JustButton btnChoiceFormation;
        private JustButton btnPlay;
        private bool pressedBtn;

        public TutorialSummonHeroX10formation(int pId, bool pIsToolTips) : base(pId, pIsToolTips)
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

            while (!(MainPanel.TopPanel is MainMenuPanel))
            {
                MainPanel.Back();
            }

            /*
						TutorialsManager.InputMasker.Lock();
						s = "But the number of defensive positions is always limited, so we need to upgrade our Base to get more Hero positions.";
						board = MainPanel.ShowNotificationBoard(btnBack, s, Alignment.Bot, new Vector2(144f, -315f), false);
						board.transform.SetParent(TutorialsManager.transform);
						yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

						TutorialsManager.InputMasker.Lock();
						s = "Now I will guide you to do that.";
						board = MainPanel.ShowNotificationBoard(btnBack, s, Alignment.Bot, new Vector2(144f, -315f), false);
						board.transform.SetParent(TutorialsManager.transform);
						yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

						TutorialsManager.InputMasker.FocusToTargetImmediately(btnBack);
						board = MainPanel.ShowNotificationBoard(btnBack, "", Alignment.Top, new Vector2(144f, -315f));
						board.transform.SetParent(TutorialsManager.transform);

						yield return new WaitUntil(() => !MainPanel.SummonGatePanel.IsActiveAndEnabled());

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
						s = "This is the base where our defenses are upgraded.";
						board = MainPanel.ShowNotificationBoard(btnBase, s, Alignment.Bot, new Vector2(144f, -515f), false);
						board.transform.SetParent(TutorialsManager.transform);
						yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

						TutorialsManager.InputMasker.FocusToTargetImmediately(btnBase);
						board = MainPanel.ShowNotificationBoard(btnBase, "", Alignment.Top, new Vector2(144f, -515f));
						board.transform.SetParent(TutorialsManager.transform);

						yield return new WaitUntil(() => MainPanel.BasePanel.IsActiveAndEnabled());
						//đợi cho đến khi popup summon hiện lên thì khóa popup
						TutorialsManager.LockPanel(MainPanel.BasePanel);

						var btnUpgradeBase = MainPanel.BasePanel.btnUpgradeBase.rectTransform;
						TutorialsManager.InputMasker.Lock();
						s = "Upgrade your Base to unlock more defensive positions and place heroes and unlock weapons to make killing enemies easier.";
						board = MainPanel.ShowNotificationBoard(btnUpgradeBase, s, Alignment.Bot, new Vector2(144f, 335f), false);
						board.transform.SetParent(TutorialsManager.transform);
						yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

						TutorialsManager.InputMasker.Lock();
						s = "Now try to upgrade your Base to level 3.";
						board = MainPanel.ShowNotificationBoard(btnUpgradeBase, s, Alignment.Bot, new Vector2(144f, 335f), false);
						board.transform.SetParent(TutorialsManager.transform);
						yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

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
						s = "Barrier is the main shield to help block the enemy's steps. Next, upgrade barrier to level 3.";
						board = MainPanel.ShowNotificationBoard(btnUpgradeBarrier, s, Alignment.Bot, new Vector2(144f, 50f), false);
						board.transform.SetParent(TutorialsManager.transform);
						yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

						TutorialsManager.InputMasker.FocusToTargetImmediately(btnUpgradeBarrier);
						board = MainPanel.ShowNotificationBoard(btnUpgradeBarrier, "", Alignment.Bot, new Vector2(144f, 50f));
						board.transform.SetParent(TutorialsManager.transform);

						var barrierData = GameData.Instance.BaseGroup.GetTrapData(IDs.ITEM_TRAP_BARRIER);
						yield return new WaitUntil(() => barrierData.Level >= 3);
			*/
            //================================================
            string s = "";
            var btnFormation = MainPanel.MainMenuPanel.btnFormation.rectTransform;
            TutorialsManager.InputMasker.Lock();
            //s = "Now the Formation has a new position. Arrange the hero you want on so we prepare for a new battle.";
            //var s = Localization.Get(Localization.ID.TUTORIAL_74);
            //var board = MainPanel.ShowNotificationBoard(btnFormation, s, Alignment.Top, new Vector2(144f, -515f), false);
            //board.transform.SetParent(TutorialsManager.transform);
            //yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

            TutorialsManager.InputMasker.FocusToTargetImmediately(btnFormation);
            var board = MainPanel.ShowNotificationBoard(btnFormation, "", Alignment.Top, new Vector2(144f, -515f), true);
            board.transform.SetParent(TutorialsManager.transform);

            TutorialsManager.UnlockPanel(MainPanel.BasePanel);
            yield return new WaitUntil(() => MainPanel.FormationPanel.IsActiveAndEnabled());

            //Click auto
            pressedBtn = false;
            var btnFormationAuto = MainPanel.FormationPanel.btnAuto.rectTransform;
            MainPanel.FormationPanel.btnAuto.onClick.RemoveListener(OnBtnOneClickEquip_Pressed);
            MainPanel.FormationPanel.btnAuto.onClick.AddListener(OnBtnOneClickEquip_Pressed);
            TutorialsManager.InputMasker.FocusToTargetImmediately(btnFormationAuto);
            board = MainPanel.ShowNotificationBoard(btnFormationAuto, "", Alignment.Top, new Vector2(144f, -515f), true);
            board.transform.SetParent(TutorialsManager.transform);

            yield return new WaitUntil(() => pressedBtn);
            GameData.Instance.TutorialsGroup.Finish(id);
            //back
            var btnBack = MainPanel.FormationPanel.btnMain.rectTransform();
            TutorialsManager.InputMasker.Lock();
            //s = "Okay, you've completed your first x10 summon. Now we have quite a few heroes.";
            //s = Localization.Get(Localization.ID.TUTORIAL_73);
            //board = MainPanel.ShowNotificationBoard(btnBack, s, Alignment.Top, new Vector2(144f, -315f), false);
            //board.transform.SetParent(TutorialsManager.transform);
            //yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

            TutorialsManager.InputMasker.FocusToTargetImmediately(btnBack);
            board = MainPanel.ShowNotificationBoard(btnBack, "", Alignment.Top, new Vector2(144f, -315f));
            board.transform.SetParent(TutorialsManager.transform);

            yield return new WaitUntil(() => !MainPanel.FormationPanel.IsActiveAndEnabled());
            GameData.Instance.UserGroup.RateCanOpen = true;


            End(true);
            TutorialsManager.Instance.Ready();
            yield break;



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
