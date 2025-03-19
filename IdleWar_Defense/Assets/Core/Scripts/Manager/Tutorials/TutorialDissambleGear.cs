using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities.Common;
using Utilities.Components;

namespace FoodZombie.UI
{
    public class TutorialDissambleGear : TutorialController
    {
        private const int MAX_GEAR = 2;
        private JustButton[] btnGearChoices = new JustButton[MAX_GEAR];
        private JustButton btnHero;
        private JustButton btnGear;
        private JustButton btnUpgrade;
        private bool pressedBtn;
        
        public TutorialDissambleGear(int pId, bool pIsToolTips) : base(pId, pIsToolTips)
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
            bool hasGearUseless = false;
            var gearDatasUnEquips = GameData.GearsGroup.GetAllGearDatasUnEquip();
            var count = gearDatasUnEquips.Count;
            for (int i = 0; i < count; i++)
            {
                var gearData = gearDatasUnEquips[i];
                if (gearData.Rank == IDs.RANK_C || gearData.Rank == IDs.RANK_B)
                {
                    hasGearUseless = true;
                }
            }

            //tặng cái gear trắng rank C, và không hiển thị FX
            if (!hasGearUseless)
            {
                var reward = new RewardInfo(IDs.REWARD_TYPE_GEAR, IDs.GC1, 1);
                LogicAPI.ClaimReward(reward, TrackingConstants.VALUE_TUTORIAL, false);
            }
            
            
            //
            var btnFactory = MainPanel.MainMenuPanel.imgFactory;
            //Focus to summon button
            TutorialsManager.InputMasker.Lock();
            //string s = "The Workshop is open.";
            var s = Localization.Get(Localization.ID.TUTORIAL_23);
            var board = MainPanel.ShowNotificationBoard(btnFactory, s, Alignment.Bot,new Vector2(144f, -650f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            
            TutorialsManager.InputMasker.Lock();
            //s = "This is the place to disassemble and upgrade gears to make our hero more powerful.";
            s = Localization.Get(Localization.ID.TUTORIAL_24);
            board = MainPanel.ShowNotificationBoard(btnFactory, s, Alignment.Bot,new Vector2(144f, -650f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            
            TutorialsManager.InputMasker.FocusToTargetImmediately(btnFactory);
            board = MainPanel.ShowNotificationBoard(btnFactory, "", Alignment.Bot, new Vector2(144f, -650f));
            board.transform.SetParent(TutorialsManager.transform);
            
            yield return new WaitUntil(() => MainPanel.FactoryPanel.IsActiveAndEnabled());
            var factoryPanel = MainPanel.FactoryPanel;
            //đợi cho đến khi popup summon hiện lên thì khóa popup
            TutorialsManager.LockPanel(factoryPanel);
            
            var togDisassemble = factoryPanel.togDisassemble.rectTransform();
            
            TutorialsManager.InputMasker.Lock();
            //s = "First I will instruct you to disassemble the redundant gears.";
            s = Localization.Get(Localization.ID.TUTORIAL_25);
            board = MainPanel.ShowNotificationBoard(togDisassemble, s, Alignment.Bot,new Vector2(144f, -650f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            
            if (!factoryPanel.tabDisassemble.activeSelf)
            {
                TutorialsManager.InputMasker.Lock();
                //s = "Select the disassemble tab";
                s = Localization.Get(Localization.ID.TUTORIAL_26);
                board = MainPanel.ShowNotificationBoard(togDisassemble, s, Alignment.Bot,new Vector2(144f, -650f), false);
                board.transform.SetParent(TutorialsManager.transform);
                yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
                
                TutorialsManager.InputMasker.FocusToTargetImmediately(togDisassemble);
                board = MainPanel.ShowNotificationBoard(togDisassemble, "", Alignment.Bot, new Vector2(144f, -650f));
                board.transform.SetParent(TutorialsManager.transform);
                
                yield return new WaitUntil(() => factoryPanel.tabDisassemble.activeSelf);
            }

            //tìm trong list gear, gear rank B, C
            var gearViews = factoryPanel.disassembleGearViewsPool;
            count = gearViews.Count;
            int index = 0;
            for (int i = 0; i < count; i++)
            {
                var gearView = gearViews[i];
                if (gearView.gameObject.activeSelf && gearView.gearData.Rank <= IDs.RANK_B)
                {
                    btnGearChoices[index] = gearView.btnChoice;
                    index++;
                    if (index >= MAX_GEAR) break;
                }
            }

            for (int i = 0; i < MAX_GEAR; i++)
            {
                var btnGearChoice = btnGearChoices[i];
                if (btnGearChoice == null) break;
                
                TutorialsManager.InputMasker.Lock();
                //s = "Select the item you want to disassemble.";
                s = Localization.Get(Localization.ID.TUTORIAL_27);
                board = MainPanel.ShowNotificationBoard(btnGearChoice.rectTransform, s, Alignment.BotRight,new Vector2(144f, -650f), false);
                board.transform.SetParent(TutorialsManager.transform);
                yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
                
                pressedBtn = false;
                TutorialsManager.InputMasker.FocusToTargetImmediately(btnGearChoice.rectTransform);
                board = MainPanel.ShowNotificationBoard(btnGearChoice.rectTransform, "", Alignment.Top, new Vector2(144f, -650f));
                board.transform.SetParent(TutorialsManager.transform);

                if (i == 0)
                {
                    btnGearChoice.onClick.RemoveListener(OnBtnGearChoice0_Pressed);
                    btnGearChoice.onClick.AddListener(OnBtnGearChoice0_Pressed);
                }
                else if (i == 1)
                {
                    btnGearChoice.onClick.RemoveListener(OnBtnGearChoice1_Pressed);
                    btnGearChoice.onClick.AddListener(OnBtnGearChoice1_Pressed);
                }

                yield return new WaitUntil(() => pressedBtn);
                yield return null;
            }
            
            var btnDisassemble = factoryPanel.btnDisassemble.rectTransform;
            TutorialsManager.InputMasker.Lock();
            //s = "Then press the Disassemble button.";
            s = Localization.Get(Localization.ID.TUTORIAL_28);
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
            
            var togUpgrade = factoryPanel.togUpgrade.rectTransform();
            TutorialsManager.InputMasker.Lock();
            //s = "After the item disassemble you will get Scraps,";
            s = Localization.Get(Localization.ID.TUTORIAL_29);
            board = MainPanel.ShowNotificationBoard(togUpgrade, s, Alignment.Bot,new Vector2(144f, -650f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            
            TutorialsManager.InputMasker.Lock();
            //s = "scrap is the material for you to upgrade gears";
            s = Localization.Get(Localization.ID.TUTORIAL_30);
            board = MainPanel.ShowNotificationBoard(togUpgrade, s, Alignment.Bot,new Vector2(144f, -650f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            
            TutorialsManager.InputMasker.Lock();
            //s = "Now try Upgrade your gears";
            s = Localization.Get(Localization.ID.TUTORIAL_31);
            board = MainPanel.ShowNotificationBoard(togUpgrade, s, Alignment.Bot,new Vector2(144f, -650f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
                
            TutorialsManager.InputMasker.FocusToTargetImmediately(togUpgrade);
            board = MainPanel.ShowNotificationBoard(togUpgrade, "", Alignment.Bot, new Vector2(144f, -650f));
            board.transform.SetParent(TutorialsManager.transform);

            yield return new WaitUntil(() => factoryPanel.tabUpgrade.activeSelf);

            //kiểm tra có gear star up chưa, có rồi thì kết thúc tutorial
            var gears = GameData.Instance.GearsGroup.GetAllGearDatas();
            count = gears.Count;
            for (int i = 0; i < count; i++)
            {
                if (gears[i].star > 1)
                {
                    End(true);
                    yield break;
                }
            }
            
            //chọn hero có gear rank C
            var heroViews = factoryPanel.heroViewsPool;
            count = heroViews.Count;
            btnHero = null;
            int slotGear = 1;
            for (int i = 0; i < count; i++)
            {
                var heroView = heroViews[i];
                var gearDatas = heroView.heroData.GearDatas;
                if (gearDatas != null && gearDatas.Count > 0)
                {
                    foreach (var gearData in gearDatas)
                    {
                        if (gearData.Value.Rank == IDs.RANK_C && !gearData.Value.IsMaxStarUp())
                        {
                            btnHero = heroView.btnChoice;
                            slotGear = gearData.Key;

                            //nếu không đủ scrap thì cộng thêm cho nó
                            var starUpCost = gearData.Value.GetGearStarUpCost();
                            
                            if (starUpCost == null)
                            {
                                End(true);
                                yield break;
                            }
                            
                            var coin = starUpCost.coin;
                            var currentCoin = GameData.Instance.CurrenciesGroup.GetValue(IDs.CURRENCY_COIN);
                            if (currentCoin < coin)
                            {
                                GameData.Instance.CurrenciesGroup.Add(IDs.CURRENCY_COIN, coin - currentCoin, TrackingConstants.VALUE_TUTORIAL);
                            }

                            var material = starUpCost.material;
                            var currentMaterial = GameData.Instance.CurrenciesGroup.GetValue(IDs.CURRENCY_MATERIAL);
                            if (currentMaterial < material)
                            {
                                GameData.Instance.CurrenciesGroup.Add(IDs.CURRENCY_MATERIAL, material - currentMaterial, TrackingConstants.VALUE_TUTORIAL);
                            }

                            break;
                        }
                    }

                    if (btnHero != null) break;
                }
            }

            if (btnHero == null)
            {
                End(true);
                yield break;
            }

            pressedBtn = false;
            TutorialsManager.InputMasker.Lock();
            //s = "Select the hero you want to upgrade, the gear they are wearing will be displayed on the board.";
            s = Localization.Get(Localization.ID.TUTORIAL_32);
            board = MainPanel.ShowNotificationBoard(btnHero.rectTransform, s, Alignment.Bot,new Vector2(144f, -650f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
                
            TutorialsManager.InputMasker.FocusToTargetImmediately(btnHero.rectTransform);
            board = MainPanel.ShowNotificationBoard(btnHero.rectTransform, "", Alignment.Top, new Vector2(144f, -650f));
            board.transform.SetParent(TutorialsManager.transform);
            
            btnHero.onClick.RemoveListener(OnBtnHero_Pressed);
            btnHero.onClick.AddListener(OnBtnHero_Pressed);
            yield return new WaitUntil(() => pressedBtn);
            yield return null;

            btnGear = factoryPanel.heroGearSlots[slotGear - 1].btnChoice;
            pressedBtn = false;
            TutorialsManager.InputMasker.Lock();
            //s = "Choose gear you want to upgrade, to upgrade gear you need Scraps and Coins.";
            s = Localization.Get(Localization.ID.TUTORIAL_33);
            board = MainPanel.ShowNotificationBoard(btnGear.rectTransform, s, Alignment.Bot,new Vector2(144f, -650f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
                
            TutorialsManager.InputMasker.FocusToTargetImmediately(btnGear.rectTransform);
            board = MainPanel.ShowNotificationBoard(btnGear.rectTransform, "", Alignment.Bot, new Vector2(144f, -650f));
            board.transform.SetParent(TutorialsManager.transform);
            
            btnGear.onClick.RemoveListener(OnBtnGear_Pressed);
            btnGear.onClick.AddListener(OnBtnGear_Pressed);
            yield return new WaitUntil(() => pressedBtn);
            yield return null;
            
            //
            var groupGearInfo = factoryPanel.groupGearInfo;
            btnUpgrade = factoryPanel.btnUpgrade;
            pressedBtn = false;
            TutorialsManager.InputMasker.Lock();
            //s = "Click Level Up to proceed with the upgrade.";
            s = Localization.Get(Localization.ID.TUTORIAL_34);
            board = MainPanel.ShowNotificationBoard(groupGearInfo, s, Alignment.Bot,new Vector2(144f, -650f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
                
            TutorialsManager.InputMasker.FocusToTargetImmediately(groupGearInfo);
            board = MainPanel.ShowNotificationBoard(btnUpgrade.rectTransform, "", Alignment.Left, new Vector2(144f, -650f));
            board.transform.SetParent(TutorialsManager.transform);
            
            btnUpgrade.onClick.RemoveListener(OnBtnUpgrade_Pressed);
            btnUpgrade.onClick.AddListener(OnBtnUpgrade_Pressed);
            yield return new WaitUntil(() => pressedBtn);
            yield return null;
            
            //
            var stars = factoryPanel.imgStarGears[0].transform.parent.rectTransform();
            TutorialsManager.InputMasker.FocusToTargetImmediately(stars);
            //s = "After upgrading gear will increase stars and stats,";
            s = Localization.Get(Localization.ID.TUTORIAL_35);
            board = MainPanel.ShowNotificationBoard(stars, s, Alignment.Bot,new Vector2(144f, -650f));
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            
            TutorialsManager.InputMasker.FocusToTargetImmediately(stars);
            //s = "the higher the gear level, the more stats you can gain and the more you can upgrade,";
            s = Localization.Get(Localization.ID.TUTORIAL_36);
            board = MainPanel.ShowNotificationBoard(stars, s, Alignment.Bot,new Vector2(144f, -650f));
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

            TutorialsManager.InputMasker.FocusToTargetImmediately(stars);
            //s = "of course, it costs more raw scraps and coins.";
            s = Localization.Get(Localization.ID.TUTORIAL_37);
            board = MainPanel.ShowNotificationBoard(stars, s, Alignment.Bot,new Vector2(144f, -650f));
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

            TutorialsManager.InputMasker.Lock();
            //s = "Now, experience this new feature for yourself.";
            s = Localization.Get(Localization.ID.TUTORIAL_38);
            board = MainPanel.ShowNotificationBoard(stars, s, Alignment.Bot,new Vector2(144f, -650f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            
            yield return null;
            TutorialsManager.UnlockPanel(MainPanel.FactoryPanel);
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
            TutorialsManager.UnlockPanel(MainPanel.FactoryPanel);
        }
        
        private void OnBtnGearChoice0_Pressed()
        {
            pressedBtn = true;
            btnGearChoices[0].onClick.RemoveListener(OnBtnGearChoice0_Pressed);
        }
        
        private void OnBtnGearChoice1_Pressed()
        {
            pressedBtn = true;
            btnGearChoices[1].onClick.RemoveListener(OnBtnGearChoice1_Pressed);
        }
        
        private void OnBtnHero_Pressed()
        {
            pressedBtn = true;
            btnHero.onClick.RemoveListener(OnBtnHero_Pressed);
        }
        
        private void OnBtnGear_Pressed()
        {
            pressedBtn = true;
            btnGear.onClick.RemoveListener(OnBtnGear_Pressed);
        }
        
        private void OnBtnUpgrade_Pressed()
        {
            pressedBtn = true;
            btnUpgrade.onClick.RemoveListener(OnBtnUpgrade_Pressed);
        }
    }
}
