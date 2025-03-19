using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Common;
using Utilities.Components;

namespace FoodZombie.UI
{
	public class TutorialDissambleHero : TutorialController
    {
        private const int MAX_HERO = 2;
        private JustButton[] btnHeroChoices = new JustButton[MAX_HERO];
        private JustButton btnHero;
        private JustButton btnUpgrade;
        private bool pressedBtn;
        
        public TutorialDissambleHero(int pId, bool pIsToolTips) : base(pId, pIsToolTips)
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

            //nếu không có gear thừa thì tằng một cái gear trắng
            bool hasHeroUseless = false;
            var heroDatasUnEquips = GameData.HeroesGroup.GetAllHeroDatasUnEquip();
            var count = heroDatasUnEquips.Count;
            for (int i = 0; i < count; i++)
            {
                var heroData = heroDatasUnEquips[i];
                if (heroData.star <= 1 && (heroData.Rank == IDs.RANK_C || heroData.Rank == IDs.RANK_B))
                {
                    hasHeroUseless = true;
                }
            }

            //tặng cái gear trắng rank C, và không hiển thị FX
            if (!hasHeroUseless)
            {
                var reward = new RewardInfo(IDs.REWARD_TYPE_UNLOCK_CHARACTER, IDs.HC4, 1);
                LogicAPI.ClaimReward(reward, TrackingConstants.VALUE_TUTORIAL, false);
            }
            
            //
            var btnLaboratory = MainPanel.MainMenuPanel.imgLaboratory;
            //Focus to summon button
            TutorialsManager.InputMasker.Lock();
            string s = "Hero Lab has opened.";
            var board = MainPanel.ShowNotificationBoard(btnLaboratory, s, Alignment.Bot,new Vector2(144f, -650f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            
            TutorialsManager.InputMasker.Lock();
            s = "This is the place to disassemble and upgrade hero more powerful.";
            board = MainPanel.ShowNotificationBoard(btnLaboratory, s, Alignment.Bot,new Vector2(144f, -650f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            
            TutorialsManager.InputMasker.FocusToTargetImmediately(btnLaboratory);
            board = MainPanel.ShowNotificationBoard(btnLaboratory, "", Alignment.Bot, new Vector2(144f, -650f));
            board.transform.SetParent(TutorialsManager.transform);
            
            yield return new WaitUntil(() => MainPanel.LaboratoryPanel.IsActiveAndEnabled());
            var laboratoryPanel = MainPanel.LaboratoryPanel;
            //đợi cho đến khi popup summon hiện lên thì khóa popup
            TutorialsManager.LockPanel(laboratoryPanel);
            
            var togDisassemble = laboratoryPanel.togDisassemble.rectTransform();
            
            TutorialsManager.InputMasker.Lock();
            s = "First I will guide you to disassemble excess Heroes.";
            board = MainPanel.ShowNotificationBoard(togDisassemble, s, Alignment.Bot,new Vector2(144f, -650f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            
            if (!laboratoryPanel.groupDisassemble.activeSelf)
            {
                TutorialsManager.InputMasker.Lock();
                s = "Select the disassemble tab";
                board = MainPanel.ShowNotificationBoard(togDisassemble, s, Alignment.Bot,new Vector2(144f, -650f), false);
                board.transform.SetParent(TutorialsManager.transform);
                yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
                
                TutorialsManager.InputMasker.FocusToTargetImmediately(togDisassemble);
                board = MainPanel.ShowNotificationBoard(togDisassemble, "", Alignment.Bot, new Vector2(144f, -650f));
                board.transform.SetParent(TutorialsManager.transform);
                
                yield return new WaitUntil(() => laboratoryPanel.groupDisassemble.activeSelf);
            }

            //tìm trong list gear, gear rank B, C star bằng 1 thôi
            var heroViews = laboratoryPanel.disassembleHeroViewsPool;
            count = heroViews.Count;
            int index = 0;
            for (int i = 0; i < count; i++)
            {
                var heroView = heroViews[i];
                if (heroView.gameObject.activeSelf && heroView.heroData.Rank <= IDs.RANK_B && heroView.heroData.star <= 1)
                {
                    btnHeroChoices[index] = heroView.btnChoice;
                    index++;
                    if (index >= MAX_HERO) break;
                }
            }

            for (int i = 0; i < MAX_HERO; i++)
            {
                var btnHeroChoice = btnHeroChoices[i];
                if (btnHeroChoice == null) break;
                
                TutorialsManager.InputMasker.Lock();
                s = "Choose the Hero you want to disassemble.";
                board = MainPanel.ShowNotificationBoard(btnHeroChoice.rectTransform, s, Alignment.BotRight,new Vector2(144f, -650f), false);
                board.transform.SetParent(TutorialsManager.transform);
                yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
                
                pressedBtn = false;
                TutorialsManager.InputMasker.FocusToTargetImmediately(btnHeroChoice.rectTransform);
                board = MainPanel.ShowNotificationBoard(btnHeroChoice.rectTransform, "", Alignment.Top, new Vector2(144f, -650f));
                board.transform.SetParent(TutorialsManager.transform);

                if (i == 0)
                {
                    btnHeroChoice.onClick.RemoveListener(OnBtnHeroChoice0_Pressed);
                    btnHeroChoice.onClick.AddListener(OnBtnHeroChoice0_Pressed);
                }
                else if (i == 1)
                {
                    btnHeroChoice.onClick.RemoveListener(OnBtnHeroChoice1_Pressed);
                    btnHeroChoice.onClick.AddListener(OnBtnHeroChoice1_Pressed);
                }

                yield return new WaitUntil(() => pressedBtn);
                yield return null;
            }
            
            var btnDisassemble = laboratoryPanel.btnDisassemble.rectTransform;
            TutorialsManager.InputMasker.Lock();
            s = "Then press the Disassemble button.";
            board = MainPanel.ShowNotificationBoard(btnDisassemble, s, Alignment.Bot,new Vector2(144f, -650f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            
            TutorialsManager.InputMasker.FocusToTargetImmediately(btnDisassemble);
            board = MainPanel.ShowNotificationBoard(btnDisassemble, "", Alignment.Bot, new Vector2(144f, -650f));
            board.transform.SetParent(TutorialsManager.transform);
            
            yield return new WaitUntil(() => MainPanel.RewardsPopup.IsActiveAndEnabled());
            End(false);
            yield return null;
            yield return new WaitUntil(() => !MainPanel.RewardsPopup.IsActiveAndEnabled());
            
            var togUpgrade = laboratoryPanel.togStarUp.rectTransform();
            TutorialsManager.InputMasker.Lock();
            s = "After the item disassemble you will receive all kinds of Core,";
            board = MainPanel.ShowNotificationBoard(togUpgrade, s, Alignment.Bot,new Vector2(144f, -650f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            
            TutorialsManager.InputMasker.Lock();
            s = "Core is the material for you to upgrade to the Hero.";
            board = MainPanel.ShowNotificationBoard(togUpgrade, s, Alignment.Bot,new Vector2(144f, -650f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            
            TutorialsManager.InputMasker.Lock();
            s = "Now try Star Upgrade for heroes";
            board = MainPanel.ShowNotificationBoard(togUpgrade, s, Alignment.Bot,new Vector2(144f, -650f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
                
            TutorialsManager.InputMasker.FocusToTargetImmediately(togUpgrade);
            board = MainPanel.ShowNotificationBoard(togUpgrade, "", Alignment.Bot, new Vector2(144f, -650f));
            board.transform.SetParent(TutorialsManager.transform);

            yield return new WaitUntil(() => laboratoryPanel.groupStarUp.activeSelf);

            //chọn hero
            HeroView starUpHeroView = null;
            count = laboratoryPanel.starUpHeroViewsPool.Count;
            for (int i = 0; i < count; i++)
            {
                var heroView = laboratoryPanel.starUpHeroViewsPool[i];
                if (!heroView.heroData.IsMaxStarUp())
                {
                    starUpHeroView = heroView;
                    break;
                }
            }

            if (starUpHeroView == null)
            {
                End(true);
                yield break;
            }
            var starUpHeroData = starUpHeroView.heroData;
            btnHero = starUpHeroView.btnChoice;

            //nếu không đủ element thì cộng thêm cho nó
            var elementCost = starUpHeroData.GetHeroStarUpCost();
            var currencyId = CurrenciesGroup.GetCurrencyIDFromElementID(starUpHeroData.Element);
            var currentElement = GameData.Instance.CurrenciesGroup.GetValue(currencyId);
            if (currentElement < elementCost)
            {
                GameData.Instance.CurrenciesGroup.Add(currencyId, elementCost - currentElement, TrackingConstants.VALUE_TUTORIAL);
            }

            pressedBtn = false;
            TutorialsManager.InputMasker.Lock();
            s = "Choose the Hero you want to upgrade";
            board = MainPanel.ShowNotificationBoard(btnHero.rectTransform, s, Alignment.Bot,new Vector2(144f, -650f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            
            TutorialsManager.InputMasker.FocusToTargetImmediately(btnHero.rectTransform);
            board = MainPanel.ShowNotificationBoard(btnHero.rectTransform, "", Alignment.Bot, new Vector2(144f, -650f));
            board.transform.SetParent(TutorialsManager.transform);
            
            btnHero.onClick.RemoveListener(OnBtnHero_Pressed);
            btnHero.onClick.AddListener(OnBtnHero_Pressed);
            yield return new WaitUntil(() => pressedBtn);
            yield return null;
            
            //
            var imgLockGroupInfo = laboratoryPanel.imgLockGroupInfo;
            imgLockGroupInfo.SetActive(true);
            
            var groupInfo = laboratoryPanel.layoutPrecedeModel.rectTransform();
            btnUpgrade = laboratoryPanel.btnStarUp;
            pressedBtn = false;
            TutorialsManager.InputMasker.FocusToTargetImmediately(groupInfo);
            s = "To Star Up hero you need Element Core.";
            board = MainPanel.ShowNotificationBoard(groupInfo, s, Alignment.Bot,new Vector2(144f, -650f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            
            TutorialsManager.InputMasker.FocusToTargetImmediately(groupInfo);
            s = "Heroes with different Element system need to use the types of Core corresponding to that Element system.";
            board = MainPanel.ShowNotificationBoard(groupInfo, s, Alignment.Bot,new Vector2(144f, -650f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
                
            TutorialsManager.InputMasker.FocusToTargetImmediately(groupInfo);
            s = "Click Upgrade to proceed with the Star Up.";
            board = MainPanel.ShowNotificationBoard(btnUpgrade.rectTransform, s, Alignment.Bot,new Vector2(144f, -650f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            
            imgLockGroupInfo.SetActive(false);

            TutorialsManager.InputMasker.FocusToTargetImmediately(groupInfo);
            board = MainPanel.ShowNotificationBoard(btnUpgrade.rectTransform, "", Alignment.Left, new Vector2(144f, -650f));
            board.transform.SetParent(TutorialsManager.transform);
            
            btnUpgrade.onClick.RemoveListener(OnBtnUpgrade_Pressed);
            btnUpgrade.onClick.AddListener(OnBtnUpgrade_Pressed);
            yield return new WaitUntil(() => pressedBtn);
            yield return null;
            
            //
            var stars = laboratoryPanel.imgStarsOld[0].transform.parent.rectTransform();
            TutorialsManager.InputMasker.FocusToTargetImmediately(stars);
            s = "After upgrading, Hero will gain stars and stats,";
            board = MainPanel.ShowNotificationBoard(stars, s, Alignment.Top,new Vector2(144f, -650f));
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            
            TutorialsManager.InputMasker.FocusToTargetImmediately(stars);
            s = "the more advanced hero, the more stats you can upgrade and the more times you can upgrade,";
            board = MainPanel.ShowNotificationBoard(stars, s, Alignment.Top,new Vector2(144f, -650f));
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

            TutorialsManager.InputMasker.Lock();
            s = "of course, it will cost more element cores";
            board = MainPanel.ShowNotificationBoard(stars, s, Alignment.Top,new Vector2(144f, -650f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

            yield return null;
            TutorialsManager.UnlockPanel(MainPanel.LaboratoryPanel);
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
            TutorialsManager.UnlockPanel(MainPanel.LaboratoryPanel);
        }
        
        private void OnBtnHeroChoice0_Pressed()
        {
            pressedBtn = true;
            btnHeroChoices[0].onClick.RemoveListener(OnBtnHeroChoice0_Pressed);
        }
        
        private void OnBtnHeroChoice1_Pressed()
        {
            pressedBtn = true;
            btnHeroChoices[1].onClick.RemoveListener(OnBtnHeroChoice1_Pressed);
        }
        
        private void OnBtnHero_Pressed()
        {
            pressedBtn = true;
            btnHero.onClick.RemoveListener(OnBtnHero_Pressed);
        }
        
        private void OnBtnUpgrade_Pressed()
        {
            pressedBtn = true;
            btnUpgrade.onClick.RemoveListener(OnBtnUpgrade_Pressed);
        }
    }
}
