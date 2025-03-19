using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities.Components;

namespace FoodZombie.UI
{
    public abstract class TutorialController
    {
        public int id { get; protected set; }
        public bool isToolTips { get; protected set; }
        public bool ended { get; protected set; }
        public MainPanel MainPanel => MainPanel.instance;
        public MainGamePanel MainGamePanel => MainGamePanel.instance;

        public GameData GameData => GameData.Instance;
        public TutorialsManager TutorialsManager => TutorialsManager.Instance;
        public TutorialController(int pId, bool pIsToolTips)
        {
            id = pId;
            isToolTips = pIsToolTips;
        }

        public IEnumerator CurrentProcess;
        public virtual void Start()
        {
            ended = false;

            if (SceneManager.GetActiveScene().name.Equals("Home"))
            {
                MainPanel.instance.onAnyChildHide += OnAnyChildHide;
                MainPanel.instance.onAnyChildShow += OnAnyChildShow;
            }
            else
            {
                MainGamePanel.instance.onAnyChildHide += OnAnyChildHide;
                MainGamePanel.instance.onAnyChildShow += OnAnyChildShow;
            }
        }
        public virtual void End(bool pFinished)
        {
            if (pFinished)
            {
                GameData.TutorialsGroup.Finish(id);
                ended = true;
                if (SceneManager.GetActiveScene().name.Equals("Home"))
                {
                    MainPanel.instance.onAnyChildHide -= OnAnyChildHide;
                    MainPanel.instance.onAnyChildShow -= OnAnyChildShow;
                }
                else
                {
                    MainGamePanel.instance.onAnyChildHide -= OnAnyChildHide;
                    MainGamePanel.instance.onAnyChildShow -= OnAnyChildShow;
                }
            }
        }
        bool pressedMainBtn = false;
        SimpleTMPButton btnMain;
        public virtual IEnumerator BackAndBattle(SimpleTMPButton _btnMain, MyGamesBasePanel panelToUnlock, bool closeCurrentPanel = true)
        {
            string s = "";
            MessageWithPointer board = null;
            if (closeCurrentPanel)
            {
                if (_btnMain == null)
                    this.btnMain = MainPanel.MainMenuPanel.btnMainMenu;
                else
                    this.btnMain = _btnMain;
                btnMain.onClick.RemoveListener(OnBtnMainPress);
                btnMain.onClick.AddListener(OnBtnMainPress);
                TutorialsManager.InputMasker.Lock();
                //var s = "Now try to fight with new powers.";
                //s = Localization.Get(Localization.ID.TUTORIAL_18);
                //board = MainPanel.ShowNotificationBoard(btnMain.rectTransform, s, Alignment.Bot, new Vector2(144f, 335f), false);
                //board.transform.SetParent(TutorialsManager.transform);
                //yield return new WaitForSeconds(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

                //close hero popup
                TutorialsManager.InputMasker.FocusToTargetImmediately(btnMain.rectTransform);
                board = MainPanel.ShowNotificationBoard(btnMain.rectTransform, "", Alignment.Top, new Vector2(144f, 335f), true);
                board.transform.SetParent(TutorialsManager.transform);

                TutorialsManager.UnlockPanel(panelToUnlock);
                yield return new WaitUntil(() => pressedMainBtn);
            }
            if (GameUnityData.instance.GameRemoteConfig.Function_ShowMap)
            {
                //=== step 8:
                //open map popup
                var btnFight = MainPanel.MainMenuPanel.btnFight;
                TutorialsManager.InputMasker.FocusToTargetImmediately(btnFight.rectTransform);
                board = MainPanel.ShowNotificationBoard(btnFight.rectTransform, "", Alignment.Top, new Vector2(144f, 335f), true, false);
                board.transform.SetParent(TutorialsManager.transform);

                yield return new WaitUntil(() => MainPanel.MapPanel.IsActiveAndEnabled());
                TutorialsManager.LockPanel(MainPanel.MapPanel);

                //=== step 9:
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
                pressedMainBtn = false;
                JustButton btnPlay = MainPanel.MissionDetailPanel.btnPlay;
                TutorialsManager.InputMasker.FocusToTargetImmediately(btnPlay.rectTransform);
                board = MainPanel.ShowNotificationBoard(btnPlay.rectTransform, "", Alignment.Bot, new Vector2(144f, -215f), true, false);
                board.transform.SetParent(TutorialsManager.transform);

                btnPlay.onClick.RemoveListener(OnBtnPlay_Pressed);
                btnPlay.onClick.AddListener(OnBtnPlay_Pressed);
                yield return new WaitUntil(() => pressedMainBtn);
            }
            else
            {
                TutorialsManager.ShowPlayOnGameplay = true;
                _pressedBtn = false;
                var btnFight = MainPanel.MainMenuPanel.btnFight;
                TutorialsManager.InputMasker.FocusToTargetImmediately(btnFight.rectTransform);
                board = MainPanel.ShowNotificationBoard(btnFight.rectTransform, "", Alignment.Top, new Vector2(144f, 335f), true, false);
                board.transform.SetParent(TutorialsManager.transform);
                btnFight.onClick.AddListener(OnBtnBattle_PressedPrivate);
                yield return new WaitUntil(() => _pressedBtn);
                End(true);
            }

        }

        bool _pressedBtn = false;
        void OnBtnMainPress()
        {
            pressedMainBtn = true;
            btnMain.onClick.RemoveListener(OnBtnMainPress);
        }
        private void OnBtnPlay_Pressed()
        {
            pressedMainBtn = true;
            End(true);

        }
        public abstract IEnumerator IEProcess();
        /// <summary>
        /// Used only for ToolTip Tutorial, we can not Pause Fixed Tutorial
        /// </summary>
        public abstract void Pause();
        /// <summary>
        /// Used only for ToolTip Tutorial, we can not Pause Fixed Tutorial
        /// </summary>
        public abstract void Resume();
        protected abstract void OnAnyChildHide(MyGamesBasePanel obj);
        protected abstract void OnAnyChildShow(MyGamesBasePanel obj);
        private void OnBtnBattle_PressedPrivate()
        {
            _pressedBtn = true;
            //	End(true);
            //btnOneClickEquip.onClick.RemoveListener(OnBtnPlay_Pressed);
        }
    }
}