
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;
using FoodZombie.UI;
using UnityEngine.SceneManagement;
using Utilities.Service.RFirebase;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FoodZombie.UI
{
    public class TutorialsManager : MonoBehaviour
    {
        public const bool SKIP_MISSION_INTRO = true;
        #region Members
        public static bool tutorialActive = false;

        private static TutorialsManager mInstance;
        public static TutorialsManager Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = FindObjectOfType<TutorialsManager>();
                return mInstance;
            }
        }

        [SerializeField] private Image mImgBlocker;
        [SerializeField] private HoledLayerMask mInputMasker;

        private List<TutorialController> mAllTutorials;
        private List<TutorialController> mCurrentTooltipTutorials;
        private Queue<TutorialController> mCurrentFixedTutorials;
        private TutorialController mCurrentFixedTutorial;
        private bool mInitialized;
        private bool mIsShowingFixedTut;

        private GameData GameData => GameData.Instance;
        public HoledLayerMask InputMasker => mInputMasker;
        public bool IsShowingFixedTut => mIsShowingFixedTut;

        private List<MyGamesBasePanel> mLockedPanels = new List<MyGamesBasePanel>();

        #endregion

        //=============================================

        #region MonoBehaviour

        private void Awake()
        {
            if (mInstance == null)
                mInstance = this;
            else if (mInstance != this)
                Destroy(gameObject);
        }

        private void OnDestroy()
        {
            EventDispatcher.RemoveListener<TutorialTriggeredEvent>(OnTutorialTriggered);
            EventDispatcher.RemoveListener<TutorialFinishedEvent>(OnTutorialFinished);
        }

        #endregion

        //=============================================

        #region Public

        public void Init()
        {
            EventDispatcher.AddListener<TutorialTriggeredEvent>(OnTutorialTriggered);
            EventDispatcher.AddListener<TutorialFinishedEvent>(OnTutorialFinished);
            mInitialized = true;

            tutorialActive = RFirebaseRemote.Instance.GetBoolValue("tutorial_active");

            mAllTutorials = new List<TutorialController>();
            if (SKIP_MISSION_INTRO && !tutorialActive)
            {
                mAllTutorials.Add(new TutorialWellcomeToNoTut(TutorialsGroup.WELLCOME_NOTUT, false));
                // mAllTutorials.Add(new TutorialSummonHeroSeniorGameplay(TutorialsGroup.SUMMON_HERO_SENIOR_GAMEPLAY, false));
                //mAllTutorials.Add(new TutorialSummonHeroSeniorHome(TutorialsGroup.SUMMON_HERO_SENIOR_HOME, false));
                mAllTutorials.Add(new TutorialGameplayWellcome(TutorialsGroup.WELLCOME_1, false));

                mAllTutorials.Add(new TutorialSummonHeroX10Gameplay(TutorialsGroup.SUMMON_HERO_X10_GAMEPLAY, false));
                mAllTutorials.Add(new TutorialSummonHeroX10(TutorialsGroup.SUMMON_HERO_X10_HOME, false));
                mAllTutorials.Add(new TutorialSummonHeroX10formation(TutorialsGroup.SUMMON_HERO_X10_HOME_FORMATION, false));

                mAllTutorials.Add(new TutorialPointPlayOnGameplay(TutorialsGroup.PLAY_ON_GAMEPLAY, false));
                //================Old==================


                mAllTutorials.Add(new TutorialAutoSkillShortGameplay(TutorialsGroup.SUMMON_AUTO_SKILL_GAMEPLAY, false));

                //mAllTutorials.Add(new TutorialGameplayBackHome(TutorialsGroup.LEVELUP_HERO_BACKHOME, false));
                mAllTutorials.Add(new TutorialLevelUpHero(TutorialsGroup.LEVELUP_HERO, false));

                // mAllTutorials.Add(new TutorialSummonHeroX10Gameplay(TutorialsGroup.SUMMON_HERO_X10_GAMEPLAY, false));


                //  mAllTutorials.Add(new TutorialGameplayBackHome(TutorialsGroup.QUEST_GAMEPLAY, false));
                // mAllTutorials.Add(new TutorialOpenQuest(TutorialsGroup.QUEST_HOME, false));

                mAllTutorials.Add(new TutorialAutoBattleRewardBackHome(TutorialsGroup.AUTO_BATTLE_REWARD_GAMEPLAY, false));
                mAllTutorials.Add(new TutorialAutoBattleReward(TutorialsGroup.AUTO_BATTLE_REWARD, false));

                mAllTutorials.Add(new TutorialGameplayBackHome(TutorialsGroup.USE_BASE_GAMEPLAY, false));
                mAllTutorials.Add(new TutorialBase(TutorialsGroup.USE_BASE_HOME, false));
                mAllTutorials.Add(new TutorialBasePlay(TutorialsGroup.USE_BASEPLAY_GAMEPLAY, false));
                //     mAllTutorials.Add(new TutorialPlayWithAds(TutorialsGroup.PLAYWITHTRAP, false));
            }
            else
            {
                mAllTutorials.Add(new TutorialMissionIntro(TutorialsGroup.MISSION_INTRO, false));
                mAllTutorials.Add(new TutorialSummonHero(TutorialsGroup.SUMMON_HERO, false));
                mAllTutorials.Add(new TutorialSummonHeroSeniorGameplay(TutorialsGroup.SUMMON_HERO_SENIOR_GAMEPLAY, false));
                mAllTutorials.Add(new TutorialSummonHeroSeniorHome(TutorialsGroup.SUMMON_HERO_SENIOR_HOME, false));
                mAllTutorials.Add(new TutorialAutoSkillShortGameplay(TutorialsGroup.SUMMON_AUTO_SKILL_GAMEPLAY, false));

                //Skip when disactive Firebase: 
                //   mAllTutorials.Add(new TutorialGameplayBackHome(TutorialsGroup.LEVELUP_HERO_BACKHOME, false));
                mAllTutorials.Add(new TutorialLevelUpHero(TutorialsGroup.LEVELUP_HERO, false));
                mAllTutorials.Add(new TutorialSummonHeroX10Gameplay(TutorialsGroup.SUMMON_HERO_X10_GAMEPLAY, false));
                mAllTutorials.Add(new TutorialSummonHeroX10(TutorialsGroup.SUMMON_HERO_X10_HOME, false));

                mAllTutorials.Add(new TutorialGameplayBackHome(TutorialsGroup.QUEST_GAMEPLAY, false));
                mAllTutorials.Add(new TutorialOpenQuest(TutorialsGroup.QUEST_HOME, false));

                mAllTutorials.Add(new TutorialAutoBattleRewardBackHome(TutorialsGroup.AUTO_BATTLE_REWARD_GAMEPLAY, false));
                mAllTutorials.Add(new TutorialAutoBattleReward(TutorialsGroup.AUTO_BATTLE_REWARD, false));

                mAllTutorials.Add(new TutorialGameplayBackHome(TutorialsGroup.USE_BASE_GAMEPLAY, false));
                mAllTutorials.Add(new TutorialBase(TutorialsGroup.USE_BASE_HOME, false));
                mAllTutorials.Add(new TutorialBasePlay(TutorialsGroup.USE_BASEPLAY_GAMEPLAY, false));
                //      mAllTutorials.Add(new TutorialPlayWithAds(TutorialsGroup.PLAYWITHTRAP, false));

                mAllTutorials.Add(new TutorialDiscovery(TutorialsGroup.DISCOVERY, false));
                mAllTutorials.Add(new TutorialDissambleGear(TutorialsGroup.DISSAMBLE_GEAR, false));
                mAllTutorials.Add(new TutorialDissambleHero(TutorialsGroup.DISSAMBLE_HERO, false));
            }
            if (!tutorialActive)
            {
                //Let disactive all:
                if (SKIP_MISSION_INTRO)
                {
                    Complete(TutorialsGroup.MISSION_INTRO);
                    Complete(TutorialsGroup.SUMMON_HERO);
                    // Complete(TutorialsGroup.SUMMON_HERO_SENIOR_GAMEPLAY);
                    // Complete(TutorialsGroup.SUMMON_HERO_SENIOR_HOME);
                    // Complete(TutorialsGroup.SUMMON_AUTO_SKILL_GAMEPLAY);
                }
                //old disactive:
                // Complete(TutorialsGroup.LEVELUP_HERO);
                //Complete(TutorialsGroup.SUMMON_HERO_X10_GAMEPLAY);
                //Complete(TutorialsGroup.SUMMON_HERO_X10_HOME);

                //Complete(TutorialsGroup.AUTO_BATTLE_REWARD);
                Complete(TutorialsGroup.DISCOVERY);
                Complete(TutorialsGroup.DISSAMBLE_GEAR);
                Complete(TutorialsGroup.DISSAMBLE_HERO);
            }

            mCurrentTooltipTutorials = new List<TutorialController>();
            mCurrentFixedTutorials = new Queue<TutorialController>();
        }

        public void Ready()
        {
            StartCoroutine(IECheckConditions());
        }

        public IEnumerator IEBlockInputWhileWaiting(float pTime, bool pClearMasker)
        {
            if (pClearMasker)
                mInputMasker.Active(false);

            mImgBlocker.SetActive(true);
            yield return new WaitForSeconds(pTime);
            mImgBlocker.SetActive(false);
        }

        public void LockPanel(MyGamesBasePanel pPanel)
        {
            pPanel.Lock(true);
            mLockedPanels.Add(pPanel);
        }

        public void UnlockPanel(MyGamesBasePanel pPanel)
        {
            pPanel.Lock(false);
            mLockedPanels.Remove(pPanel);
        }

        public void UnlockAllPanels()
        {
            for (int i = 0; i < mLockedPanels.Count; i++)
                mLockedPanels[i].Lock(false);
            mLockedPanels.Clear();
        }

        public void SkipFixedTutotiral()
        {
            // var tut = mCurrentFixedTutorials.Peek();
            // tut.End(true);

            // StopCoroutine(tut.IEProcess());

            if (mCurrentFixedTutorial != null)
            {
                mCurrentFixedTutorial.End(true);
                StopCoroutine(mCurrentFixedTutorial.CurrentProcess);
            }
            InputMasker.SetActive(false);
            UnlockAllPanels();
            if (SceneManager.GetActiveScene().name.Equals("Home")) MainPanel.instance.HideNotificationBoard(0);
            else MainGamePanel.instance.HideNotificationBoard(0);
        }
        public void SkipTutorial(TutorialController tut)
        {
            tut.End(true);
            StopCoroutine(tut.CurrentProcess);
            InputMasker.SetActive(false);
            UnlockAllPanels();
            if (SceneManager.GetActiveScene().name.Equals("Home")) MainPanel.instance.HideNotificationBoard(0);
            else MainGamePanel.instance.HideNotificationBoard(0);
        }

        #endregion

        //==============================================

        #region Private

        public static bool ShowPlayOnGameplay = false;
        private IEnumerator IECheckConditions()
        {
            //   yield return new WaitForSeconds(0.1f);
            if (GameData.Instance.UserGroup.SkipTut) yield break;
            yield return new WaitForEndOfFrame();
            if (SKIP_MISSION_INTRO)
            {
                tutorialActive = RFirebaseRemote.Instance.GetBoolValue("tutorial_active");
                if (!tutorialActive)
                {
                    //Debug.LogError("Last mission ID "+ GameData.Instance.MissionsGroup.LastWinMissionId);
                    //if (!IsCompleted(TutorialsGroup.WELLCOME_NOTUT) && GameData.Instance.HeroesGroup.GetAllHeroDatas().Count == 0)
                    if (!IsCompleted(TutorialsGroup.WELLCOME_NOTUT) && GameData.Instance.MissionsGroup.LastWinMissionId == -1)
                    {
                        TriggerTutorial(TutorialsGroup.WELLCOME_NOTUT);
                        yield break;
                    }
                }
            }

            var currentMissionId = GameData.Instance.MissionsGroup.CurrentMissionId;

            if (SceneManager.GetActiveScene().name.Equals("GamePlay"))
            {
                //GameData.Instance.TutorialsGroup.Reset();
                //TriggerTutorial(TutorialsGroup.PLAYWITHTRAP);
                //yield break;
                //GameData.Instance.TutorialsGroup.Reset();
                //TriggerTutorial(TutorialsGroup.USE_BASEPLAY_GAMEPLAY);
                //yield break;
                //if (!GameData.Instance.MissionsGroup.IsWinIntroMission()
                //    && !IsCompleted(TutorialsGroup.MISSION_INTRO))
                //{
                //    TriggerTutorial(TutorialsGroup.MISSION_INTRO);
                //}
                //else if (IsCompleted(TutorialsGroup.SUMMON_HERO)
                //         && !IsCompleted(TutorialsGroup.SUMMON_HERO_SENIOR_GAMEPLAY))
                //{
                //    if (currentMissionId == 1001)
                //        TriggerTutorial(TutorialsGroup.SUMMON_HERO_SENIOR_GAMEPLAY);
                //}
                //else if (!IsCompleted(TutorialsGroup.LEVELUP_HERO_BACKHOME))
                //{
                //    if (currentMissionId == 1002) TriggerTutorial(TutorialsGroup.LEVELUP_HERO_BACKHOME);
                //}
                // else if (!IsCompleted(TutorialsGroup.SUMMON_X2_SPEED_GAMEPLAY))
                // {
                //     if (currentMissionId == 1004) TriggerTutorial(TutorialsGroup.SUMMON_X2_SPEED_GAMEPLAY);
                // }
                //else if (!IsCompleted(TutorialsGroup.SUMMON_HERO_X10_GAMEPLAY))
                //{
                //	if (currentMissionId == 1005) TriggerTutorial(TutorialsGroup.SUMMON_HERO_X10_GAMEPLAY);
                //}
                //else if (!IsCompleted(TutorialsGroup.QUEST_GAMEPLAY))
                //{
                //	if (currentMissionId == 1008) TriggerTutorial(TutorialsGroup.QUEST_GAMEPLAY);
                //}
                //else if (!IsCompleted(TutorialsGroup.AUTO_BATTLE_REWARD_GAMEPLAY))
                //{
                //	if (currentMissionId == 1010) TriggerTutorial(TutorialsGroup.AUTO_BATTLE_REWARD_GAMEPLAY);
                //}
                //else if (!IsCompleted(TutorialsGroup.USE_BASE_GAMEPLAY))
                //{
                //	if (currentMissionId == 2001) TriggerTutorial(TutorialsGroup.USE_BASE_GAMEPLAY);
                //}
                // else
                if (Config.typeModeInGame == Config.TYPE_MODE_NORMAL)
                {
                    if (ShowPlayOnGameplay)
                    {
                        ShowPlayOnGameplay = false;
                        TriggerTutorial(TutorialsGroup.PLAY_ON_GAMEPLAY);
                    }
                    else
                    {
                        if (!IsCompleted(TutorialsGroup.WELLCOME_1))
                        {
                            if (currentMissionId == 1001)
                                TriggerTutorial(TutorialsGroup.WELLCOME_1);
                        }

                        if (!IsCompleted(TutorialsGroup.SUMMON_HERO_X10_GAMEPLAY))
                        {
                            if (currentMissionId == 1001)
                                TriggerTutorial(TutorialsGroup.SUMMON_HERO_X10_GAMEPLAY);
                        }

                        if (!IsCompleted(TutorialsGroup.AUTO_BATTLE_REWARD_GAMEPLAY))
                        {
                            if (currentMissionId == 1002)
                                TriggerTutorial(TutorialsGroup.AUTO_BATTLE_REWARD_GAMEPLAY);
                        }

                        if (!IsCompleted(TutorialsGroup.USE_BASE_HOME) && !IsCompleted(TutorialsGroup.USE_BASE_GAMEPLAY))
                        {
                            if (currentMissionId == 2001)
                                TriggerTutorial(TutorialsGroup.USE_BASE_GAMEPLAY);
                        }

                    }

                }


                if (Config.typeModeInGame == Config.TYPE_MODE_NORMAL)
                    if (GameUnityData.instance.GameRemoteConfig.Function_Autoplay_Active && !IsCompleted(TutorialsGroup.SUMMON_AUTO_SKILL_GAMEPLAY))
                    {
                        int _levelActiveAutoplay = GameUnityData.instance.GameRemoteConfig.Function_Autoplay_ActiveLevel;
                        if (_levelActiveAutoplay <= 0)
                        { if (currentMissionId == 1003) TriggerTutorial(TutorialsGroup.SUMMON_AUTO_SKILL_GAMEPLAY); }
                        else
                        { if (currentMissionId == _levelActiveAutoplay) TriggerTutorial(TutorialsGroup.SUMMON_AUTO_SKILL_GAMEPLAY); }
                    }

                ////TEST
                //GameData.Instance.BaseGroup.GetTrapData(IDs.ITEM_TRAP_BARRIER).SetStock(10);
                //if (currentMissionId == 2002) TriggerTutorial(TutorialsGroup.USE_BASEPLAY_GAMEPLAY);
                if (Config.typeModeInGame == Config.TYPE_MODE_NORMAL)
                    if (!IsCompleted(TutorialsGroup.USE_BASEPLAY_GAMEPLAY))
                    {
                        if (currentMissionId == 2002) TriggerTutorial(TutorialsGroup.USE_BASEPLAY_GAMEPLAY);
                    }
            }
            else
            {

                if (!IsCompleted(TutorialsGroup.SUMMON_HERO_X10_HOME))
                {
                    if (currentMissionId == 1002)
                        TriggerTutorial(TutorialsGroup.SUMMON_HERO_X10_HOME);
                }
                else
                if (!IsCompleted(TutorialsGroup.SUMMON_HERO_X10_HOME_FORMATION))
                {
                    if (currentMissionId == 1002)
                        TriggerTutorial(TutorialsGroup.SUMMON_HERO_X10_HOME_FORMATION);
                }
                else
                if (!IsCompleted(TutorialsGroup.LEVELUP_HERO))
                {
                    if (currentMissionId == 1002)
                        TriggerTutorial(TutorialsGroup.LEVELUP_HERO);
                }

                if (IsCompleted(TutorialsGroup.AUTO_BATTLE_REWARD_GAMEPLAY) && !IsCompleted(TutorialsGroup.AUTO_BATTLE_REWARD))
                {
                    if (currentMissionId > 1002)
                        TriggerTutorial(TutorialsGroup.AUTO_BATTLE_REWARD);
                }
                //if (!IsCompleted(TutorialsGroup.SUMMON_HERO))
                //{
                //    TriggerTutorial(TutorialsGroup.SUMMON_HERO);
                //}
                //else if (IsCompleted(TutorialsGroup.SUMMON_HERO_SENIOR_GAMEPLAY)
                //         && !IsCompleted(TutorialsGroup.SUMMON_HERO_SENIOR_HOME))
                //{
                //    TriggerTutorial(TutorialsGroup.SUMMON_HERO_SENIOR_HOME);
                //}
                //else if (IsCompleted(TutorialsGroup.SUMMON_AUTO_SKILL_GAMEPLAY)
                //      && IsCompleted(TutorialsGroup.LEVELUP_HERO_BACKHOME)
                //        && !IsCompleted(TutorialsGroup.LEVELUP_HERO))
                //{
                //    TriggerTutorial(TutorialsGroup.LEVELUP_HERO);
                //}
                //else if (IsCompleted(TutorialsGroup.SUMMON_HERO_X10_GAMEPLAY)
                //         && !IsCompleted(TutorialsGroup.SUMMON_HERO_X10_HOME))
                //{
                //    if (currentMissionId > 1005)
                //    {
                //        TriggerTutorial(TutorialsGroup.SUMMON_HERO_X10_HOME);
                //    }
                //}
                //if (IsCompleted(TutorialsGroup.QUEST_GAMEPLAY)
                //        && !IsCompleted(TutorialsGroup.QUEST_HOME))
                //{
                //    if (currentMissionId > 1008)
                //    {
                //        TriggerTutorial(TutorialsGroup.QUEST_HOME);
                //    }
                //}

                //if (GameData.Instance.LevelUnlockContent >= Constants.UNLOCK_AFK_REWARD
                //    && !IsCompleted(TutorialsGroup.AUTO_BATTLE_REWARD))
                //{
                //    if (currentMissionId > 1010)
                //        TriggerTutorial(TutorialsGroup.AUTO_BATTLE_REWARD);
                //}

                if (IsCompleted(TutorialsGroup.USE_BASE_GAMEPLAY)
                                    && !IsCompleted(TutorialsGroup.USE_BASE_HOME))
                {
                    if (currentMissionId > 2001)
                    {
                        TriggerTutorial(TutorialsGroup.USE_BASE_HOME);
                    }
                }

                // if (GameData.Instance.UserGroup.Level >= Constants.UNLOCK_DISCOVERY 
                //     && !IsCompleted(TutorialsGroup.DISCOVERY))
                // {
                //     TriggerTutorial(TutorialsGroup.DISCOVERY);
                // }

                if (GameData.Instance.LevelUnlockContent >= Constants.UNLOCK_FACTORY
                    && !IsCompleted(TutorialsGroup.DISSAMBLE_GEAR))
                {
                    var gearDatasEquip = GameData.Instance.GearsGroup.GetAllGearDatasEquip();
                    bool hasGearToStarUp = false;
                    var count = gearDatasEquip.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (gearDatasEquip[i].star <= 1)
                        {
                            hasGearToStarUp = true;
                            break;
                        }
                    }
                    if (hasGearToStarUp) TriggerTutorial(TutorialsGroup.DISSAMBLE_GEAR);
                }

                if (GameData.Instance.LevelUnlockContent >= Constants.UNLOCK_DISASSEMBLE_HERO
                    && !IsCompleted(TutorialsGroup.DISSAMBLE_HERO))
                {
                    var heroDatasEquip = GameData.Instance.HeroesGroup.GetEquippedHeroes();
                    bool hasHeroToStarUp = false;
                    var count = heroDatasEquip.Length;
                    for (int i = 0; i < count; i++)
                    {
                        if (heroDatasEquip[i] != null && heroDatasEquip[i].star <= 1)
                        {
                            hasHeroToStarUp = true;
                            break;
                        }
                    }
                    if (hasHeroToStarUp) TriggerTutorial(TutorialsGroup.DISSAMBLE_HERO);
                }
            }
        }

        private IEnumerator IEProcessFixedTutorial()
        {
            mIsShowingFixedTut = true;

            PauseToolTipTutorials();

            if (SceneManager.GetActiveScene().name.Equals("Home")) yield return new WaitUntil(() => MainPanel.instance.Initialized);
            else yield return new WaitUntil(() => MainGamePanel.instance.Initialized);

            mCurrentFixedTutorial = null;
            while (mCurrentFixedTutorials.Count > 0)
            {
                // yield return new WaitForSeconds(0.25f);
                // // có 1 tut ở ngay lúc hiện RescueHeroPanel
                // if(SceneManager.GetActiveScene().name.Equals("Home")) 
                // {
                //     yield return new WaitUntil(() => !MainPanel.instance.IsBusy()
                //                                   && MainPanel.instance.TopPanel is MainMenuPanel
                //                            );
                // }
                // else
                // {
                //     yield return new WaitUntil(() => !MainGamePanel.instance.IsBusy()
                //                                   && MainGamePanel.instance.TopPanel is HubPanel
                //                            );
                // }
                yield return null;
                mCurrentFixedTutorial = mCurrentFixedTutorials.Peek();
                mCurrentFixedTutorial.Start();
                mCurrentFixedTutorial.CurrentProcess = mCurrentFixedTutorial.IEProcess();
                StartCoroutine(mCurrentFixedTutorial.CurrentProcess);

                yield return new WaitUntil(() => mCurrentFixedTutorial.ended);
                mCurrentFixedTutorial = null;

                mCurrentFixedTutorials.Dequeue();
            }
            mIsShowingFixedTut = false;

            ResumeToolTipTutorials();
        }

        /// <summary>
        /// NOTE: we have two type of tutorials, ToolTips and Fixed
        /// ToolTips are basically notifications which player can ignore, it can be done anytime
        /// Fixed ones are unignorable, and more important than ToolTips
        /// When Fixed run, we must pause all ToolTips if there are some available and resume when Fixed ones all done
        /// </summary>
        private void TriggerTutorial(int pId)
        {
            if (IsCompleted(pId))
                return;

            TutorialController tut = null;
            for (int i = 0; i < mAllTutorials.Count; i++)
                if (mAllTutorials[i].id == pId)
                {
                    tut = mAllTutorials[i];
                    break;
                }

            if (mCurrentFixedTutorials.Contains(tut) || mCurrentTooltipTutorials.Contains(tut))
                return;

            if (tut != null)
            {
                Debug.Log("Trigger Tutorial " + tut.id);

                if (tut.isToolTips)
                {
                    //Tooltip Tutorials should not interupt Fixed Tutorial
                    if (mIsShowingFixedTut)
                    {
                        //Wait until there is no Fixed tutorial
                        WaitUtil.Start(new WaitUtil.ConditionEvent()
                        {
                            id = tut.id,
                            onTrigger = () => tut.Start(),
                            triggerCondition = () => !mIsShowingFixedTut
                        });
                    }
                    else
                        tut.Start();

                    mCurrentTooltipTutorials.Add(tut);
                }
                else
                {
                    mCurrentFixedTutorials.Enqueue(tut);

                    if (!mIsShowingFixedTut)
                        StartCoroutine(IEProcessFixedTutorial());
                }
            }
        }

        public bool IsCompleted(int pId)
        {
            //	Debug.Log("Check TuT");
#if TUTORIAL
            var data = GameData.Instance.TutorialsGroup.GetTutorial(pId);
            //Debug.Log("Check TuT TuT");
            return data != null ? data.Completed : true;
#else
//Debug.Log("Check TuT true");
            return true;
#endif
        }

        private void Complete(int pId)
        {
            var data = GameData.Instance.TutorialsGroup.GetTutorial(pId);
            if (data != null)
            {
                data.SetComplete(true);

                if (mCurrentTooltipTutorials != null && mCurrentTooltipTutorials.Count > 0)
                {
                    for (int i = mCurrentTooltipTutorials.Count - 1; i >= 0; i--)
                    {
                        if (mCurrentTooltipTutorials[i].id == pId)
                        {
                            mCurrentTooltipTutorials.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
        }

        private void OnTutorialFinished(TutorialFinishedEvent e)
        {
            for (int i = 0; i < mCurrentTooltipTutorials.Count; i++)
                if (mCurrentTooltipTutorials[i].id == e.tutorial)
                {
                    mCurrentTooltipTutorials.RemoveAt(i);
                    break;
                }
        }

        private void OnTutorialTriggered(TutorialTriggeredEvent e)
        {
            TriggerTutorial(e.tutorial);
        }

        /// <summary>
        /// When fixed tutorial run, we must not let anything interupt it
        /// </summary>
        private void PauseToolTipTutorials()
        {
            for (int i = 0; i < mCurrentTooltipTutorials.Count; i++)
                mCurrentTooltipTutorials[i].Pause();
        }

        private void ResumeToolTipTutorials()
        {
            for (int i = 0; i < mCurrentTooltipTutorials.Count; i++)
                mCurrentTooltipTutorials[i].Resume();
        }

        #endregion
    }
}