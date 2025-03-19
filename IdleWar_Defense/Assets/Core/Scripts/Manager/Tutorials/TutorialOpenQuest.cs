using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Components;

namespace FoodZombie.UI
{
    public class TutorialOpenQuest : TutorialController
    {
        private JustButton btnQuest;
        private SimpleTMPButton btnClaim;
        private bool pressedBtn;

        public TutorialOpenQuest(int pId, bool pIsToolTips) : base(pId, pIsToolTips)
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
                                                                && MainPanel.TopPanel is DailyQuestPanel));

            //while (!(MainPanel.TopPanel is MainMenuPanel))
            //{
            //    MainPanel.Back();
            //}

            //bỏ đoạn cho một mảnh, thay bằng 1 lượt free mỗi ngày
            //nếu không có thì next tut luôn
            MessageWithPointer board;
            string s;

            pressedBtn = false;
            //btnQuest = MainPanel.MainMenuPanel.btnQuest;
            //btnQuest.onClick.RemoveListener(OnQuestClick);
            //btnQuest.onClick.AddListener(OnQuestClick);
            //TutorialsManager.InputMasker.Lock();
            //s = "Let's me show you the Quest system";
            //board = MainPanel.ShowNotificationBoard(btnQuest.rectTransform, s, Alignment.Bot, new Vector2(144f, -315f), true);
            //board.transform.SetParent(TutorialsManager.transform);
            //yield return new WaitForSeconds(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

            //TutorialsManager.InputMasker.FocusToTargetImmediately(btnQuest.rectTransform);
            //board = MainPanel.ShowNotificationBoard(btnQuest.rectTransform, "", Alignment.Bot, new Vector2(144f, -315f), true);
            //board.transform.SetParent(TutorialsManager.transform);
            //yield return new WaitUntil(() => pressedBtn);
            //yield return new WaitUntil(() => MainPanel.DailyQuestPanel.isActiveAndEnabled && MainPanel.DailyQuestPanel.isInitDone);

            btnClaim = null;
            List<DailyQuestView> _quests = MainPanel.DailyQuestPanel.dailyQuestViewsPool;
            //   int i = 0;
            foreach (var quest in _quests)
            {
                //   i++;
                //    if (i >= 1) break;
                if (!quest.isClaimed && quest.btnClaim.gameObject.activeInHierarchy)
                {
                    btnClaim = quest.btnClaim; break;
                }
            }
            if (btnClaim != null)
            {
                pressedBtn = false;
                //s = "Let's collect the reward from the quest";
                s = Localization.Get(Localization.ID.TUTORIAL_51);
                TutorialsManager.InputMasker.FocusToTargetImmediately(btnClaim.rectTransform);
                btnClaim.onClick.RemoveListener(OnClaimClick);
                btnClaim.onClick.AddListener(OnClaimClick);
                board = MainPanel.ShowNotificationBoard(btnClaim.rectTransform, s, Alignment.Bot, new Vector2(144f, -315f), true);
                board.transform.SetParent(TutorialsManager.transform);
                yield return new WaitForSeconds(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);


                board = MainPanel.ShowNotificationBoard(btnClaim.rectTransform, "", Alignment.Bot, new Vector2(144f, -315f), true);
                board.transform.SetParent(TutorialsManager.transform);
                yield return new WaitUntil(() => pressedBtn);
                yield return new WaitUntil(() => MainPanel.RewardsPopup.IsActiveAndEnabled());
                End(false);
                yield return null;
                yield return new WaitUntil(() => !MainPanel.RewardsPopup.IsActiveAndEnabled());
            }

            TutorialsManager.InputMasker.Lock();
            //s = "Let's complete more quests to get more rewards";
            s = Localization.Get(Localization.ID.TUTORIAL_52);
            if (btnClaim != null)
                board = MainPanel.ShowNotificationBoard(btnClaim.rectTransform, s, Alignment.Bot, new Vector2(144f, -315f), false);
            else
                board = MainPanel.ShowNotificationBoard(MainPanel.DailyQuestPanel.GetComponent<RectTransform>(), s, Alignment.Bot, new Vector2(144f, -315f), false);
            board.transform.SetParent(TutorialsManager.transform);
            yield return new WaitForSeconds(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
            //  TutorialsManager.UnlockAllPanels();
            //  yield return new WaitForSeconds(2);
            End(true);

        }
        void OnQuestClick()
        {
            pressedBtn = true;
            btnQuest.onClick.RemoveListener(OnQuestClick);
        }
        void OnClaimClick()
        {
            pressedBtn = true;
            btnClaim.onClick.RemoveListener(OnQuestClick);
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


    }
}
