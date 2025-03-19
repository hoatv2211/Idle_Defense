using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;
using Utilities.Service.RFirebase;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

[System.Serializable]
public class HeroViewList
{
    public HeroView[] list;
}

namespace FoodZombie.UI
{
    public class MissionDetailPanel : IHeroFormationPanel
    {
        public SimpleTMPButton btnChange;
        public SimpleTMPButton btnCloseFormation;
        public SimpleTMPButton btnSaveFormation;
        public GameObject groupMissionInfo;
        public GameObject groupFormation;

        public SimpleTMPButton btnPlay, btnPlayWithTrap;
        public Button btnExit;
        public GameObject PanelQuit;
        public SimpleTMPButton btnQuitYes, btnQUitNo;
        public TextMeshProUGUI txtMission;
        public TextMeshProUGUI txtMission1;

        public TMP_InputField txtIdMap;
        public TMP_InputField txtIdMission;

        public SimpleTMPButton btnNextScroll;
        public SimpleTMPButton btnPreScroll;

        public Transform transformEnemyPool;
        public ScrollRect scroll;
        [SerializeField, Tooltip("Buildin Pool")] private List<EnemyView> enemyViewsPool;
        public HeroView[] currentFormation;
        public SimpleButton[] btnSlots;
        public Text[] btnSlotsTextes;

        public HeroViewList[] formations;
        //public TextMeshProUGUI[] txtFormationNames;

        public GameObject[] imgChoiceFormations;
        public SimpleTMPButton[] btnChoiceFormations;

        public GameObject rewardPanel;
        public RewardView[] rewardViewMain, rewardViewBonus;

        private MissionData currentMission;
        private int choiceFormation = 0;


        private void Start()
        {
            btnChange?.onClick.AddListener(BtnChange_Pressed);
            btnCloseFormation?.onClick.AddListener(BtnCloseFormation_Pressed);
            btnSaveFormation?.onClick.AddListener(BtnSaveFormation_Pressed);
            btnPlay.onClick.AddListener(BtnPlay_Pressed);
            if (Config.typeModeInGame == Config.TYPE_MODE_PvP)
                btnPlayWithTrap.gameObject.SetActive(false);
            else
            {
                btnPlayWithTrap.gameObject.SetActive(GameData.Instance.LevelUnlockContent > Constants.UNLOCK_BASE);
                btnPlayWithTrap.onClick.AddListener(BtnPlayWithTrap_Pressed);
            }
            btnNextScroll?.onClick.AddListener(BtnNextScroll_Pressed);
            btnPreScroll?.onClick.AddListener(BtnPreScroll_Pressed);
            btnFormationAuto.onClick.AddListener(QuickEquip);
            btnExit.onClick.AddListener(BtnExitPress);
            btnQuitYes.onClick.AddListener(BtnOkQuit_Pressed);
            btnQUitNo.onClick.AddListener(BtnCancelQuit_Pressed);
            for (int i = 0; i < btnSlots.Length; i++)
            {
                int index = i;

                if (GameUnityData.instance.GameRemoteConfig.Function_ChangeFormationInMissionDetail)
                {
                    btnSlots[i].onClick.AddListener(() => BtnSlot_Click(index));
                    currentFormation[i].btnChoice.onClick.AddListener(() => BtnSlot_Click(index));
                }
                //currentFormation[i].btnChoice.onClick.AddListener(() => BtnHeroSlot_Click(index));
            }
            for (int i = 0; i < HeroesGroup.MAX_FORMATION; i++)
            {
                int index = i;
                var btnChoiceFormation = btnChoiceFormations[i];
                btnChoiceFormation?.onClick.AddListener(() =>
                {
                    BtnChoiceFormation_Pressed(index);
                });
            }
        }

        private void Update()
        {
            if (scroll == null) return;
            if (scroll.horizontalNormalizedPosition >= 1f)
            {
                btnPreScroll.SetEnable(true);
                btnNextScroll.SetEnable(false);
            }
            else if (scroll.horizontalNormalizedPosition <= 0f)
            {
                btnPreScroll.SetEnable(false);
                btnNextScroll.SetEnable(true);
            }
            else
            {
                btnPreScroll.SetEnable(true);
                btnNextScroll.SetEnable(true);
            }
        }
        public Action OnShowDone;
        bool isTriggerTut = false;
        internal override void Init()
        {
            Lock(true);
            btnPlayWithTrap.gameObject.SetActive(GameData.Instance.LevelUnlockContent > Constants.UNLOCK_BASE);
            if (enemyViewsPool != null || enemyViewsPool.Count <= 0)
                enemyViewsPool.Free();
            heroesGroup.CheckFormation();
            this.FormationInit();

            string missionName = "";
            if (Config.typeModeInGame == Config.TYPE_MODE_PvP)
            {
                missionName = "PvP";
                GameData.Instance.HeroesGroup.SetCurrentFormation(1);
            }
            else
            {
                GameData.Instance.HeroesGroup.SetCurrentFormation(0);
                if (Config.typeModeInGame == Config.TYPE_MODE_NORMAL)
                {
                    currentMission = GameData.Instance.MissionsGroup.GetCurrentMissionData();
                    missionName = Localization.Get(Localization.ID.MISSION).ToUpper() + " " + currentMission.GetName();
                }
                else
                {
                    currentMission = GameData.Instance.DiscoveriesGroup.GetCurrentMissionData();
                    missionName = currentMission.GetName();
                }
            }

            //currentMission = GameData.Instance.MissionsGroup.GetCurrentMissionData();
            //if(Config.typeModeInGame)
            if (txtMission != null)
                txtMission.text = missionName;/*"MISSION " + currentMission.GetName();*/
            if (txtMission1 != null)
                txtMission1.text = missionName;/*"MISSION " + currentMission.GetName();*/
            if (currentMission != null)
            {
                List<EnemyInfo> enemyInfos = currentMission.baseData.enemyInfos;
                int count = enemyInfos.Count;
                for (int i = count - 1; i >= 0; i--)
                {
                    var item = GameData.Instance.EnemiesGroup.GetEnemyData(enemyInfos[i].id);
                    var enemyView = enemyViewsPool.Obtain(transformEnemyPool);
                    enemyView.Init(item);
                    enemyView.SetActive(true);
                }
                if (count > 3)
                {
                    btnNextScroll.SetActive(true);
                    btnPreScroll.SetActive(true);
                }
                else
                {
                    btnNextScroll.SetActive(false);
                    btnPreScroll.SetActive(false);
                }
            }

            ShowCurrentFormation();

            if (formations.Length > 0)
                for (int i = 0; i < HeroesGroup.MAX_FORMATION; i++)
                {
                    //txtFormationNames[i].text = "Formation " + i;
                    HeroData[] formationData = GameData.Instance.HeroesGroup.GetEquippedHeroes(i);
                    int count = formationData.Length;
                    var formation = formations[i];
                    for (int j = 0; j < count; j++)
                    {
                        var item = formationData[j];
                        var heroView = formation.list[j];
                        if (item != null)
                            heroView.Init(item, this);
                    }

                    imgChoiceFormations[i].SetActive(i == choiceFormation);
                }

            if (Config.typeModeInGame == Config.TYPE_MODE_NORMAL)
            {
                if (rewardViewMain.Length > 0 && rewardViewBonus.Length > 0)
                {
                    List<RewardInfo> rewards = new List<RewardInfo>();
                    //	rewards.AddRange(currentMission.GetMainRewardInfos());
                    if (currentMission != null)
                    {
                        rewards.AddRange(currentMission.GetBonusRewardInfos());
                        rewards.AddRange(currentMission.GetBossRewardInfos());
                    }
                    rewardPanel.SetActive(false);
                    LoadRewardsView(rewardViewMain, currentMission.GetMainRewardInfos());
                    LoadRewardsView(rewardViewBonus, rewards);
                }
            }
            else
            {
                rewardPanel.SetActive(false);
            }
            //	currentMission.
            //#if DEVELOPMENT
            //			txtIdMap.text = (currentMission.Id / 1000) + "";
            //			txtIdMap.SetActive(true);
            //			txtIdMission.text = currentMission.Id % 1000 + "";
            //			txtIdMission.SetActive(true);
            //#endif
        }
        void LoadRewardsView(RewardView[] rewardView, List<RewardInfo> rewards)
        {
            if (rewards != null && rewards.Count > 0)
            {
                //	imgLineBossReward.SetActive(true);
                rewardPanel.SetActive(true);
                for (int i = 0; i < rewardView.Length; i++)
                {
                    RewardView _currentView = rewardView[i];
                    if (i < rewards.Count && rewards[i] != null)
                    {
                        _currentView.SetActive(true);
                        _currentView.Init(rewards[i]);
                    }
                    else
                        _currentView.SetActive(false);
                }
            }
            else
                rewardView[0].transform.parent.gameObject.SetActive(false);
        }
        //protected override void AfterShowing()
        //{
        //	base.AfterShowing();
        //	if (btnPlayWithTrap.gameObject.activeSelf && !isTriggerTut)
        //		if (!TutorialsManager.Instance.IsCompleted(TutorialsGroup.PLAYWITHTRAP))
        //		{
        //			isTriggerTut = true;
        //			EventDispatcher.Raise(new TutorialTriggeredEvent(TutorialsGroup.PLAYWITHTRAP));
        //		}
        //}
        public override void ShowCurrentFormation()
        {
            choiceFormation = GameData.Instance.HeroesGroup.CurrentFormation;
            HeroData[] formationData = GameData.Instance.HeroesGroup.GetEquippedHeroes();
            int count = formationData.Length;
            for (int i = 0; i < count; i++)
            {
                int _maxLevel = GameData.Instance.MissionsGroup.CurrentMissionId;
                int levelToUnlock = Constants.FORMATION_SLOTUNLOCK(i + 1);
                HeroData item = formationData[i];
                HeroView heroView = currentFormation[i];
                SimpleButton btnSlot = btnSlots[i];
                if (_maxLevel - 1 >= levelToUnlock)
                {
                    //Unlocked:
                    btnSlotsTextes[i].gameObject.SetActive(false);
                    btnSlot.image.color = Color.white;
                    if (item == null)
                    {
                        heroView.SetActive(false);
                        btnSlot.gameObject.SetActive(true);
                        btnSlot.enabled = true;
                        btnSlot.transform.DOKill();
                        btnSlot.transform.localScale = Vector3.one;
                        //	btnSlot.image.color = Color.black;
                        if (GameUnityData.instance.GameRemoteConfig.Function_ChangeFormationInMissionDetail)
                            btnSlot.rectTransform.DOScale(1.1f * Vector3.one, 1).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
                    }
                    else
                    {
                        heroView.SetActive(true);
                        heroView.isTOPHero = !HaveOtherStrongerNotSelect(item);
                        btnSlot.gameObject.SetActive(false);
                        btnSlot.transform.DOKill();
                        btnSlot.transform.localScale = Vector3.one;
                        heroView.Init(item, this);
                    }
                }
                else
                {
                    //Lock
                    btnSlotsTextes[i].gameObject.SetActive(true);
                    btnSlot.gameObject.SetActive(true);
                    btnSlot.enabled = false;
                    btnSlot.transform.DOKill();
                    btnSlot.transform.localScale = Vector3.one;
                    btnSlot.image.color = Color.gray;
                    btnSlotsTextes[i].text = string.Format(Localization.Get(Localization.ID.UNLOCK_AT).ToUpper() + "\n{0}", GameData.Instance.MissionLevelToString(levelToUnlock));
                    heroView.SetActive(false);
                }
            }
        }

        private void BtnChange_Pressed()
        {
            groupMissionInfo.SetActive(false);
            groupFormation.SetActive(true);
        }

        private void BtnCloseFormation_Pressed()
        {
            groupMissionInfo.SetActive(true);
            groupFormation.SetActive(false);
        }

        private void BtnSaveFormation_Pressed()
        {
            //txtFormationNames[i].text = "Formation " + i;
            var formationData = GameData.Instance.HeroesGroup.GetEquippedHeroes(choiceFormation);
            var hasHero = false;
            var count = formationData.Length;
            for (int i = 0; i < count; i++)
            {
                var item = formationData[i];
                if (item != null)
                {
                    hasHero = true;
                    break;
                }
            }

            if (hasHero)
            {
                GameData.Instance.HeroesGroup.SetCurrentFormation(choiceFormation);
                BtnCloseFormation_Pressed();
                ShowCurrentFormation();
            }
            else
            {
                MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_6));
                //MainPanel.instance.ShowWarningPopup("You must choose a formation with some heroes to fight.");
            }
        }

        private void BtnChoiceFormation_Pressed(int index)
        {
            choiceFormation = index;

            for (int i = 0; i < HeroesGroup.MAX_FORMATION; i++)
            {
                imgChoiceFormations[i].SetActive(i == choiceFormation);
            }
        }
        private void BtnExitPress()
        {
            if (GameUnityData.instance.GameRemoteConfig.Function_ShowMap && MainPanel.instance != null)
            {
                //Back on Map
                Lock(false);
                Back();
            }
            else
            {
                //Back on Game
                PanelQuit.SetActive(true);
            }
        }
        private void BtnOkQuit_Pressed()
        {
            GameplayController.Instance.BackToHome();
        }

        private void BtnCancelQuit_Pressed()
        {
            PanelQuit.SetActive(false);
        }
        private void BtnPlay_Pressed()
        {
            //GameData.Instance.UserGroup.SetGameStep(UserGroup.UserGameStep.CLICK_MISSION_PLAY);
            GameData.Instance.UserGroup.SetGameStep(UserGroup.UserGameStep.START_MISSION_CLICK_STARTGAME);
            if (CheckCanStartGame())
            {
                Config.LogEvent(TrackingConstants.CLICK_BATTLE_DETAIL_PLAY);
                StartGame();
            }
        }
        private void BtnPlayWithTrap_Pressed()
        {
            if (!CheckCanStartGame()) return;
            if (!AdsHelper.__IsVideoRewardedAdReady())
            {
                if (MainGamePanel.instance != null)
                    MainGamePanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_1));
                else
                    MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_1));
                //MainGamePanel.instance.ShowWarningPopup("Ads not available");
            }
            else
            {
                AdsHelper.__ShowVideoRewardedAd(TrackingConstants.ADS_REWARD_START_WITH_TRAP, (isCompleted) =>
                {
                    if (isCompleted)
                    {
                        RewardInfo _trap = new RewardInfo(IDs.REWARD_TYPE_TRAP, UnityEngine.Random.Range(IDs.ITEM_TRAP_BARRIER, IDs.ITEM_TRAP_TRAP + 1), 1);
                        LogicAPI.ClaimReward(_trap, "StartWithTrap", true, RewardsPopup.AlignType.Horizontal, () =>
                        {
                            GameData.Instance.UserGroup.SetGameStep(UserGroup.UserGameStep.START_MISSION_CLICK_STARTGAME);
                            if (CheckCanStartGame())
                            {
                                Config.LogEvent(TrackingConstants.CLICK_BATTLE_DETAIL_PLAY_WITH_TRAP);
                                StartGame();
                            }
                        });

                    }
                });
            }

        }

        bool CheckCanStartGame()
        {
            var formationData = GameData.Instance.HeroesGroup.GetEquippedHeroes();
            var hasHero = false;
            var count = formationData.Length;
            for (int i = 0; i < count; i++)
            {
                var item = formationData[i];
                if (item != null)
                {
                    hasHero = true;
                    break;
                }
            }
            if (!hasHero)
            {
                //var message = new MessagesPopup.Message()
                //{
                //	yesAction = () =>
                //	{
                //		Back();
                //		// MainPanel.instance.TopPanel.Back();
                //		MainPanel.instance.ShowFormationPanel();
                //	},
                //	noAction = () => { },
                //	additionalAction = null,
                //	yesActionLabel = "FORMATION",
                //	noActionLabel = "BACK",
                //	allowIgnore = true,
                //	title = "",
                //	content = "You must choose a formation with some heroes to fight.",
                //	popupSize = new Vector2(900, 660),
                //	contentAignment = TextAlignmentOptions.Center
                //};
                if (MainGamePanel.instance != null)
                    MainGamePanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_6));
                else
                    MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_6));
                //MainGamePanel.instance.ShowWarningPopup("You must choose a formation with some heroes to fight.");
                return false;
            }
            return true;
        }
        void StartGame()
        {
            //Daily Quest và Achievement
            //	GameData.Instance.DailyQuestsGroup.PlayMission();

            //#if DEVELOPMENT
            //			int indexMap = int.Parse(txtIdMap.text);
            //			int indexMission = int.Parse(txtIdMission.text) + indexMap * 1000;
            //			GameData.Instance.MissionsGroup.SetCurrentMission(indexMission);
            //#endif
            if (Config.typeModeInGame == Config.TYPE_MODE_NORMAL)
            {
                int currentMissionId = currentMission.Id;
                // if (currentMissionId <= 4000 || currentMissionId % 5 == 0)
                {
                    Config.LogEvent(TrackingConstants.MISSION_PLAY_COUNT, TrackingConstants.PARAM_MISSION, currentMissionId);
                    Config.LogEvent(TrackingConstants.MISSION_PLAY_DAY_COUNT, TrackingConstants.PARAM_MISSION, currentMissionId.ToString(), TrackingConstants.PARAM_DAY, GameData.Instance.UserGroup.playDayCount.Value);
                    //AppsFlyerObjectScript.LogLevelAchieved(currentMissionId);
                  
                }
                GameData.Instance.UserGroup.SetGameStep(UserGroup.UserGameStep.START_MISSION_CLICK_STARTGAME_SUCCESS);
            }

            //Config.typeModeInGame = Config.TYPE_MODE_NORMAL;
            Lock(false);
            if (GameUnityData.instance.GameRemoteConfig.Function_ShowMap&&MainPanel.instance!=null)
                MainPanel.instance.LoadGamePlayScreen();
            else
                Back();


        }

        private void BtnNextScroll_Pressed()
        {
            scroll.DOHorizontalNormalizedPos(1f, 0.5f);
        }

        private void BtnPreScroll_Pressed()
        {
            scroll.DOHorizontalNormalizedPos(0f, 0.5f);
        }

        private void BtnSlot_Click(int index)
        {
            //if (this.allHeroesToView.Count <= 0)
            //{
            //	MainGamePanel.instance.ShowWarningPopup("Have no more heroes\nPlay more levels to unlock new heroes");
            //}
            //else
            //{
            Config.LogEvent(TrackingConstants.CLICK_BATTLE_DETAIL_OPENFORMATION);
            GameData.Instance.UserGroup.SetGameStep(UserGroup.UserGameStep.START_MISSION_CLICK_HEROFORMATION);
            currentFormationSlotIndexSelect = index;
            FormationSetupPanel.gameObject.SetActive(true);
            FormationInit();
            //   }
        }
        private void BtnHeroSlot_Click(int index)
        {
            Config.LogEvent(TrackingConstants.CLICK_BATTLE_DETAIL_HEROBUTTON_SELECT);
            currentFormationSlotIndexSelect = index;
            formationsDatas = new HeroData[HeroesGroup.MAX_FORMATION][];
            for (int i = 0; i < HeroesGroup.MAX_FORMATION; i++)
            {
                formationsDatas[i] = heroesGroup.GetEquippedHeroes(i);
            }
            HeroData[] formation = formationsDatas[currentFormationIndex];
            formation[currentFormationSlotIndexSelect] = null;
            SaveFormations();
            //FormationSetupPanel.gameObject.SetActive(true);
            //	FormationInit();
            ShowCurrentFormation();
        }
        protected override void BeforeHiding()
        {
            base.BeforeHiding();
            if (OnShowDone != null)
                OnShowDone();
        }

    }
}