using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Components;

namespace FoodZombie.UI
{
    public class TutorialWellcomeToNoTut : TutorialController
    {
        private JustButton btnHeroView;
        private JustButton btnSave;
        private JustButton btnChoiceFormation;
        private JustButton btnPlay;
        private bool pressedBtn;

        public TutorialWellcomeToNoTut(int pId, bool pIsToolTips) : base(pId, pIsToolTips)
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

            //bỏ đoạn cho một mảnh, thay bằng 1 lượt free mỗi ngày
            //nếu không có thì next tut luôn
            MessageWithPointer board;
            string s;
            btnPlay = MainPanel.MissionDetailPanel.btnPlay;
            var imgSummonGate = MainPanel.MainMenuPanel.imgSummonGate;


            TutorialsManager.InputMasker.Lock();
            //	s = "Hello Captain!\nThe enemy is attacking!";
            //s = Localization.Get(Localization.ID.TUTORIAL_75);
            //board = MainPanel.ShowNotificationBoard(imgSummonGate, s, Alignment.Bot, new Vector2(144f, -215f), false);
            //board.SetCharacter(MessageWithPointer.CharacerType.NPC);
            //board.transform.SetParent(TutorialsManager.transform);
            //yield return new WaitForSeconds(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            //yield return new WaitForSeconds(1);

            ////   s = "Urgent situation!\nYou have to go find more Heroes\nI'll be here to stop them";
            //s = Localization.Get(Localization.ID.TUTORIAL_76);
            //board = MainPanel.ShowNotificationBoard(imgSummonGate, s, Alignment.Bot, new Vector2(144f, -215f), false);
            //board.SetCharacter(MessageWithPointer.CharacerType.Hero);
            //board.transform.SetParent(TutorialsManager.transform);
            //yield return new WaitForSeconds(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            //yield return new WaitForSeconds(1);

            //// s = "Then wait for me\nI'll bring more Heroes as quickly as I can";
            //s = Localization.Get(Localization.ID.TUTORIAL_77);
            //board = MainPanel.ShowNotificationBoard(imgSummonGate, s, Alignment.Bot, new Vector2(144f, -215f), false);
            //board.SetCharacter(MessageWithPointer.CharacerType.NPC);
            //board.transform.SetParent(TutorialsManager.transform);
            //yield return new WaitForSeconds(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            //yield return new WaitForSeconds(1);

            //Unlock Hero 
            //board = MainPanel.ShowNotificationBoard(MainPanel.RewardsPopup.rectTransform(), "", Alignment.Bot, new Vector2(144f, -315f));
            //board.transform.SetParent(TutorialsManager.transform);
            //var rewardInfo = GameData.Instance.HeroesGroup.SummonByPowerFragment();
            //LogicAPI.ClaimReward(rewardInfo);
            //yield return new WaitUntil(() => MainPanel.RewardsPopup.IsActiveAndEnabled());
            //End(false);
            //yield return null;
            //yield return new WaitUntil(() => !MainPanel.RewardsPopup.IsActiveAndEnabled());
            //MainPanel.FormationPanel.QuickEquip();

            if (GameUnityData.instance.GameRemoteConfig.Function_ShowMap)
            {
                //open map popup
                var btnFight = MainPanel.MainMenuPanel.btnFight;
                TutorialsManager.InputMasker.FocusToTargetImmediately(btnFight.rectTransform);
                board = MainPanel.ShowNotificationBoard(btnFight.rectTransform, "", Alignment.Top, new Vector2(144f, -215f), true, false);
                board.transform.SetParent(TutorialsManager.transform);
                yield return new WaitUntil(() => MainPanel.MapPanel.IsActiveAndEnabled());
                TutorialsManager.LockPanel(MainPanel.MapPanel);


                //open mission detail popup
                JustButton btnMissionPlay = null;
                var btnMissions = MainPanel.MapPanel.btnMissions;
                var count = btnMissions.Length;
                for (int i = 0; i < count; i++)
                {
                    var btnMission = btnMissions[i];
                    if (btnMission.btnPlay.Enabled())
                    {
                        btnMissionPlay = btnMission.btnPlay;
                        break;
                    }
                }
                TutorialsManager.InputMasker.FocusToTargetImmediately(btnMissionPlay.rectTransform);
                board = MainPanel.ShowNotificationBoard(btnMissionPlay.rectTransform, "", Alignment.Bot, new Vector2(144f, -215f), true, false);
                board.transform.SetParent(TutorialsManager.transform);
                yield return new WaitUntil(() => MainPanel.MissionDetailPanel.IsActiveAndEnabled());
                TutorialsManager.LockPanel(MainPanel.MissionDetailPanel);

                //PLAY MISSIONNNNNNNNNNNNNNNN
                pressedBtn = false;
                btnPlay = MainPanel.MissionDetailPanel.btnPlay;
                TutorialsManager.InputMasker.FocusToTargetImmediately(btnPlay.rectTransform);
                board = MainPanel.ShowNotificationBoard(btnPlay.rectTransform, "", Alignment.Bot, new Vector2(144f, -215f), true, false);
                board.transform.SetParent(TutorialsManager.transform);

                btnPlay.onClick.RemoveListener(OnBtnPlay_Pressed);
                btnPlay.onClick.AddListener(OnBtnPlay_Pressed);
            }
            else
            {
                TutorialsManager.ShowPlayOnGameplay = true;
                pressedBtn = false;
                var btnFight = MainPanel.MainMenuPanel.btnFight;
                TutorialsManager.InputMasker.FocusToTargetImmediately(btnFight.rectTransform);
                board = MainPanel.ShowNotificationBoard(btnFight.rectTransform, "", Alignment.Top, new Vector2(144f, 335f), true, false);
                board.transform.SetParent(TutorialsManager.transform);
                btnFight.onClick.AddListener(OnBtnBattle_Pressed);
                yield return new WaitUntil(() => pressedBtn);
                End(true);
            }

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
        private void OnBtnBattle_Pressed()
        {
            pressedBtn = true;
            //	End(true);
            //btnOneClickEquip.onClick.RemoveListener(OnBtnPlay_Pressed);
        }
    }


}
