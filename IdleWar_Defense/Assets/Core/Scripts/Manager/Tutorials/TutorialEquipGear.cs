using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Components;

namespace FoodZombie.UI
{
    public class TutorialEquipGear : TutorialController
    {
        private JustButton btnPlay;
        private JustButton btnOneClickEquip;
        private bool pressedBtn;
        
        public TutorialEquipGear(int pId, bool pIsToolTips) : base(pId, pIsToolTips)
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
            yield return new WaitUntil(() => MainPanel.HeroPanel.IsActiveAndEnabled()                                                        
                                                    && !MainPanel.MessagesPopup.IsActiveAndEnabled());
            TutorialsManager.LockPanel(MainPanel.HeroPanel);

            //=== step 1:
            //Focus to coin cost
            var togGears = MainPanel.HeroPanel.togGears;
            var togGearsRect = togGears.rectTransform();
            TutorialsManager.InputMasker.FocusToTargetImmediately(togGearsRect);
            var board = MainPanel.ShowNotificationBoard(togGearsRect, "", Alignment.Bot, new Vector2(480, 150));
            board.transform.SetParent(TutorialsManager.transform);

            yield return new WaitUntil(() => MainPanel.HeroPanel.tabGears.activeSelf);

            //=== step 2:
            //Focus to one key equip all
            btnOneClickEquip = MainPanel.HeroPanel.btnOneClickEquip;
            TutorialsManager.InputMasker.FocusToTargetImmediately(btnOneClickEquip.rectTransform);
            board = MainPanel.ShowNotificationBoard(btnOneClickEquip.rectTransform, "Click to auto equip", Alignment.Top, new Vector2(600, 150));
            board.transform.SetParent(TutorialsManager.transform);

            btnOneClickEquip.onClick.RemoveListener(OnBtnOneClickEquip_Pressed);
            btnOneClickEquip.onClick.AddListener(OnBtnOneClickEquip_Pressed);
            yield return new WaitUntil(() => pressedBtn);
            yield return null;
            
            End(true);
            
            // //=== step 3:
            // //close formation popup
            // var btnMain = MainPanel.HeroPanel.btnMain;
            // TutorialsManager.InputMasker.FocusToTargetImmediately(btnMain.rectTransform);
            // board = MainPanel.ShowNotificationBoard(btnMain.rectTransform, "", Alignment.Top, new Vector2(480, 150));
            // board.transform.SetParent(TutorialsManager.transform);
            //
            // TutorialsManager.UnlockPanel(MainPanel.HeroPanel);
            // yield return new WaitUntil(() => !MainPanel.HeroPanel.IsActiveAndEnabled());
            //
            // //=== step 4:
            // //open map popup
            // var btnFight = MainPanel.MainMenuPanel.btnFight;
            // TutorialsManager.InputMasker.FocusToTargetImmediately(btnFight.rectTransform);
            // board = MainPanel.ShowNotificationBoard(btnFight.rectTransform, "", Alignment.Top, new Vector2(480, 150));
            // board.transform.SetParent(TutorialsManager.transform);
            //
            // yield return new WaitUntil(() => MainPanel.MapPanel.IsActiveAndEnabled());
            // TutorialsManager.LockPanel(MainPanel.MapPanel);
            //
            // //=== step 5:
            // //open mission detail popup
            // JustButton btnMissionPlay = null;
            // var btnMissions = MainPanel.MapPanel.btnMissions;
            // var count = btnMissions.Length;
            // for (int i = 0; i < count; i++)
            // {
            //     var btnMission = btnMissions[i];
            //     if (btnMission.btnPlay.Enabled())
            //     {
            //         btnMissionPlay = btnMission.btnPlay;
            //         break;
            //     }
            // }
            // TutorialsManager.InputMasker.FocusToTargetImmediately(btnMissionPlay.rectTransform);
            // board = MainPanel.ShowNotificationBoard(btnMissionPlay.rectTransform, "", Alignment.Bot, new Vector2(480, 150));
            // board.transform.SetParent(TutorialsManager.transform);
            //
            // yield return new WaitUntil(() => MainPanel.MissionDetailPanel.IsActiveAndEnabled());
            // TutorialsManager.LockPanel(MainPanel.MissionDetailPanel);
            //
            // //=== step 6:
            // //PLAY MISSIONNNNNNNNNNNNNNNN
            // pressedBtn = false;
            // btnPlay = MainPanel.MissionDetailPanel.btnPlay;
            // TutorialsManager.InputMasker.FocusToTargetImmediately(btnPlay.rectTransform);
            // board = MainPanel.ShowNotificationBoard(btnPlay.rectTransform, "", Alignment.Bot, new Vector2(480, 150));
            // board.transform.SetParent(TutorialsManager.transform);
            //
            // btnPlay.onClick.RemoveListener(OnBtnPlay_Pressed);
            // btnPlay.onClick.AddListener(OnBtnPlay_Pressed);
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
            TutorialsManager.UnlockPanel(MainPanel.HeroPanel);
            TutorialsManager.UnlockPanel(MainPanel.MissionDetailPanel);
        }

        private void OnBtnOneClickEquip_Pressed()
        {
            pressedBtn = true;
            btnOneClickEquip.onClick.RemoveListener(OnBtnOneClickEquip_Pressed);
        }
        
        private void OnBtnPlay_Pressed()
        {
            End(true);
            btnOneClickEquip.onClick.RemoveListener(OnBtnPlay_Pressed);
        }
    }
}
