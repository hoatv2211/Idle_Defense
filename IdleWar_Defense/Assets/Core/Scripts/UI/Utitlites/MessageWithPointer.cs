using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;

namespace FoodZombie.UI
{
    public class MessageWithPointer : MonoBehaviour
    {
        [SerializeField] private RectTransform mRectMessage;
        [SerializeField] private ScrollRect mScrollRect;
        [SerializeField] private TextMeshProUGUI mTxtMessage;
        [SerializeField] private RectTransform mRectPointer;
        [SerializeField] private Button btnSkip;
        [SerializeField] private SkeletonGraphic mModelNPC, mModelHero;

        public enum CharacerType
        {
            NPC, Hero
        }
        CharacerType characterType;
        public int id;
        public RectTransform RectMessage => mRectMessage;
        public RectTransform RectPointer => mRectPointer;
        public Button ButtonSkip => btnSkip;
        private void Awake()
        {
            this.btnSkip.onClick.AddListener(DoSkip);
        }
        private void OnDisable()
        {
            SetCharacter(CharacerType.NPC);
        }
        Action OnSkip;
        public void ActiveSkip(Action OnSkip)
        {
            this.btnSkip.gameObject.SetActive(true);
            this.OnSkip = OnSkip;
        }
        void DoSkip()
        {
            this.StopAllCoroutines();
            if (this.OnSkip != null)
                this.OnSkip();
            //if (GameplayController.Instance != null)
            //    GameplayController.Instance.ResumeGame();
            TutorialsManager.Instance.SkipFixedTutotiral();
            this.btnSkip.gameObject.SetActive(false);

            DoSkip_Step1_CheckReward();
        }
        #region UnlockTutorialValues:
        void DoSkip_Step1_CheckReward()
        {
            GameData.Instance.UserGroup.SkipTut = true;
            //Time.timeScale = 1;
            //if (GameplayController.Instance != null)
            //{
            //    //     GameplayController.Instance.ResumeGame();

            //    GameplayController.Instance.BreakGame();
            //}
            #region UnlockGameLevel
            int currentLevel = GameData.Instance.MissionsGroup.LastWinMissionId;
            if (currentLevel < 1003)
            {
                GameData.Instance.MissionsGroup.SetCurrentMission(1003);
            }
            #endregion
            var heroesGroup = GameData.Instance.HeroesGroup;
            var allHeroes = heroesGroup.GetAllHeroDatas();
            int heroUnlockedNumber = 0;
            if (allHeroes != null && allHeroes.Count > 0) heroUnlockedNumber = allHeroes.Count;
            List<RewardInfo> rewardsTut = new List<RewardInfo>();
            #region AFK
            List<RewardInfo> rewardsInfos = GameData.Instance.MissionsGroup.GetAFKRewards(60 * 5);
            if (rewardsInfos.Count > 0)
                rewardsTut.AddRange(rewardsInfos);
            GameData.Instance.MissionsGroup.ClaimAFKRewards();
            #endregion
            #region Get10Hero
            if (allHeroes.Count < 10)
            {
                //open gem:
                var xCount = 10;
                var currenciesGroup = GameData.Instance.CurrenciesGroup;
                Debug.Log(currenciesGroup.GetValue(IDs.CURRENCY_POWER_CRYSTAL));
                if (currenciesGroup.GetValue(IDs.CURRENCY_POWER_CRYSTAL) < 10)
                    currenciesGroup.Add(IDs.CURRENCY_POWER_CRYSTAL, 10 - currenciesGroup.GetValue(IDs.CURRENCY_POWER_CRYSTAL), TrackingConstants.VALUE_TUTORIAL);
                Debug.Log(currenciesGroup.GetValue(IDs.CURRENCY_POWER_CRYSTAL));
                currenciesGroup.Pay(IDs.CURRENCY_POWER_CRYSTAL, 10, TrackingConstants.VALUE_SUMMON);
                Debug.Log(currenciesGroup.GetValue(IDs.CURRENCY_POWER_CRYSTAL));
                for (int i = 0; i < xCount; i++)
                {
                    RewardInfo rewardInfo = GameData.Instance.HeroesGroup.SummonByPowerCrytal(1);
                    GameData.Instance.HeroesGroup.UpdataClaimHeroValues();
                    //  if (rewardsInfos.Count > 0)
                    rewardsTut.Add(rewardInfo);
                    // LogicAPI.ClaimReward(rewardInfo, null, true, RewardsPopup.AlignType.Horizontal);
                    //  CheckRewardInfo(rewardInfo);
                    //Summon 15 senior sẽ được free 1 Devine crystal.
                    rewardInfo = GameData.Instance.HeroesGroup.AddCountPowerCrystalSummon();
                    if (rewardInfo != null)
                    {
                        rewardsTut.Add(rewardInfo);
                        //  LogicAPI.ClaimReward(rewardInfo, null, true, RewardsPopup.AlignType.Cicler);
                    }
                }
                //  CheckPowerCrytal();
                if (MainPanel.instance != null)
                    MainPanel.instance.MainMenuPanel.CheckNotif();
            }
            #endregion
            if (rewardsTut.Count > 0)
            {
                RewardsPopup.skipTweenTime = true;
                LogicAPI.ClaimRewards(rewardsTut, TrackingConstants.VALUE_TUTORIAL, true, DoSkip_Step2_UpdateOther);
            }
            else
                DoSkip_Step2_UpdateOther();
            //   if(GameplayController.Instance.)
        }
        void DoSkip_Step2_UpdateOther()
        {
            RewardsPopup.skipTweenTime = false;
            #region CheckUI
            EventDispatcher.Raise(new UIChangeEvent());
            #endregion
            var heroesGroup = GameData.Instance.HeroesGroup;
            var allHeroes = heroesGroup.GetAllHeroDatas();
            #region Setup Formation
            //Add All Hero Unlocked to Formation:
            var formations = new HeroData[HeroesGroup.MAX_FORMATION][];
            for (int i = 0; i < HeroesGroup.MAX_FORMATION; i++)
            {
                formations[i] = heroesGroup.GetEquippedHeroes(i);
            }
            allHeroes = heroesGroup.GetAllHeroDatas();
            allHeroes = allHeroes.OrderBy(o => -o.Power).ToList();
            //var allHeroes = heroesGroup.GetAllHeroDatas();
            for (int i = 0; i < 2; i++)
            {
                if (i < formations[0].Length)
                    formations[0][i] = allHeroes[i];
            }
            //Save Formation:
            var count = formations.Length;

            var newFormations = new List<List<string>>();
            for (int i = 0; i < count; i++)
            {
                var newFormation = new List<string>();
                var formation = formations[i];
                var fCount = formation.Length;
                for (int j = 0; j < fCount; j++)
                {
                    var heroData = formation[j];
                    if (heroData == null) newFormation.Add("-1");
                    else newFormation.Add(formation[j].id + "");
                }

                newFormations.Add(newFormation);
            }
            heroesGroup.SaveEquippedHeros(newFormations);

            #endregion
            #region UpLevel Hero1
            var formationZero = formations[0];
            HeroData _hero_First = null;
            if (formationZero.Length > 0)
            {
                _hero_First = formationZero[0];
                for (int i = 0; i < 5; i++)
                    _hero_First.LevelUp();
            }


            if (_hero_First != null)
            {
                var maxGear = 5;
                for (int i = 0; i < maxGear; i++)
                {
                    //tìm trong mỗi slot một gear có power mạnh nhất thì mặc vào
                    var gearDatas = GameData.Instance.GearsGroup.GetAllGearDatasUnEquip(i + 1);
                    if (gearDatas != null && gearDatas.Count > 0)
                    {
                        gearDatas = gearDatas.OrderBy(o => -o.Power).ToList();
                        var bestGearInSlot = gearDatas[0];
                        var oldGear = _hero_First.GetEquippedGear(i + 1);
                        if (oldGear == null || bestGearInSlot.Power > oldGear.Power)
                        {
                            _hero_First.EquipGear(i + 1, bestGearInSlot.id);
                        }
                    }
                }
            }
            #endregion
            #region Setup X2Game
            GameData.Instance.GameConfigGroup.SetX2Speed(true);
            #endregion
            #region GoToMapPanel

            if (GameplayController.Instance != null)
            {
                Config.backToHomePanel = SceneName.MapPanel;
                GameplayController.Instance.BackToHome();
            }
            #endregion
        }
        #endregion

        public void PointToTarget(RectTransform pTarget, Alignment pAlignment, float pOffset = 0, bool pPostValidate = true)
        {
            mRectPointer.SetActive(true);

            mRectPointer.position = pTarget.position;
            var targetPivot = pTarget.pivot;
            var x = mRectPointer.anchoredPosition.x - pTarget.rect.width * targetPivot.x + pTarget.rect.width / 2f;
            var y = mRectPointer.anchoredPosition.y - pTarget.rect.height * targetPivot.y + pTarget.rect.height / 2f;
            mRectPointer.anchoredPosition = new Vector2(x, y);

            var targetBounds = pTarget.Bounds();
            var arrowBounds = mRectPointer.Bounds();
            var arrowPos = mRectPointer.anchoredPosition;

            switch (pAlignment)
            {
                case Alignment.TopLeft:
                    arrowPos.y = arrowPos.y + targetBounds.size.y / 2 + arrowBounds.size.y / 2 + pOffset;
                    arrowPos.x = arrowPos.x - targetBounds.size.x / 2 - arrowBounds.size.x / 2 - pOffset;
                    mRectPointer.eulerAngles = (new Vector3(0, 0, 45));
                    break;
                case Alignment.Top:
                    arrowPos.y = arrowPos.y + targetBounds.size.y / 2 + arrowBounds.size.y / 2 + pOffset;
                    mRectPointer.eulerAngles = (new Vector3(0, 0, 0));
                    break;
                case Alignment.TopRight:
                    arrowPos.y = arrowPos.y + targetBounds.size.y / 2 + arrowBounds.size.y / 2 + pOffset;
                    arrowPos.x = arrowPos.x + targetBounds.size.x / 2 + arrowBounds.size.x / 2 + pOffset;
                    mRectPointer.eulerAngles = (new Vector3(0, 0, -45));
                    break;
                case Alignment.Left:
                    arrowPos.x = arrowPos.x - targetBounds.size.x / 2 - arrowBounds.size.x / 2 + pOffset;
                    mRectPointer.eulerAngles = (new Vector3(0, 0, 90));
                    break;
                case Alignment.Center:
                    break;
                case Alignment.Right:
                    arrowPos.x = arrowPos.x + targetBounds.size.x / 2 + arrowBounds.size.x / 2 + pOffset;
                    mRectPointer.eulerAngles = (new Vector3(0, 0, -90));
                    break;
                case Alignment.BotLeft:
                    arrowPos.y = arrowPos.y - targetBounds.size.y / 2 - arrowBounds.size.y / 2 - pOffset;
                    arrowPos.x = arrowPos.x - targetBounds.size.x / 2 - arrowBounds.size.x / 2 - pOffset;
                    mRectPointer.eulerAngles = (new Vector3(0, 0, -235));
                    break;
                case Alignment.Bot:
                    arrowPos.y = arrowPos.y - targetBounds.size.y / 2 - arrowBounds.size.y / 2 - pOffset;
                    mRectPointer.eulerAngles = (new Vector3(0, 0, 180));
                    break;
                case Alignment.BotRight:
                    arrowPos.y = arrowPos.y - targetBounds.size.y / 2 - arrowBounds.size.y / 2 - pOffset;
                    arrowPos.x = arrowPos.x + targetBounds.size.x / 2 + arrowBounds.size.x / 2 + pOffset;
                    mRectPointer.eulerAngles = (new Vector3(0, 0, 235));
                    break;
            }

            mRectPointer.anchoredPosition = arrowPos;
            enabled = true;

            if (pPostValidate)
                CoroutineUtil.StartCoroutine(IEPostValidatingPointer(pTarget, pAlignment, pOffset));
        }

        public void MessageToTarget(string pMessage, Alignment pAlignment, Vector2 pPos, bool pPostValidate = true)
        {
            mRectMessage.SetActive(true);
            if (pPos != null)
                mRectMessage.anchoredPosition = pPos;
            enabled = true;

            if (pPostValidate)
            {
                CoroutineUtil.StartCoroutine(IEPostValidatingMessage(pMessage, pAlignment, pPos));

                //npc
                switch (characterType)
                {
                    case CharacerType.NPC:
                        mModelNPC.Initialize(false);
                        mModelNPC.Skeleton.SetSkin("emo" + Config.EasyRandom(1, 4));
                        mModelNPC.Skeleton.SetSlotsToSetupPose();
                        mModelNPC.AnimationState.SetAnimation(0, "animation", true);
                        break;
                    case CharacerType.Hero:
                        // mModelHero.Initialize(false);
                        //.Skeleton.SetSkin("1");
                        // mModelHero.Skeleton.SetSlotsToSetupPose();
                        //mModelHero.AnimationState.SetAnimation(0, "animation", true);
                        break;
                    default:
                        break;
                }


                CoroutineUtil.StartCoroutine(IERunMessage(pMessage));
            }
        }

        public void SetCharacter(CharacerType type)
        {
            this.characterType = type;
            switch (type)
            {
                case CharacerType.NPC:
                    mModelNPC.gameObject.SetActive(true);
                    mModelHero.gameObject.SetActive(false);
                    break;
                case CharacerType.Hero:
                    mModelNPC.gameObject.SetActive(false);
                    mModelHero.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }
        }

        private IEnumerator IEPostValidatingPointer(RectTransform pTarget, Alignment pAlignment, float pOffset = 0)
        {
            for (int i = 0; i < 5; i++)
            {
                yield return null;
                PointToTarget(pTarget, pAlignment, pOffset, false);
            }
        }

        private IEnumerator IEPostValidatingMessage(string pMessage, Alignment pAlignment, Vector2 pPos)
        {
            for (int i = 0; i < 5; i++)
            {
                yield return null;
                MessageToTarget(pMessage, pAlignment, pPos, false);
            }
        }

        private IEnumerator IERunMessage(string pMessage)
        {
            //cache lại dòng text cuối cùng
            if (pMessage.Equals("")) yield break;

            var count = pMessage.Length;
            string s = "";
            for (int i = 0; i < count; i++)
            {
                s += pMessage[i];
                mTxtMessage.text = s;
                if (mScrollRect != null)
                    mScrollRect.verticalNormalizedPosition = 0f;
                if (i % 8 == 7) yield return new WaitForSecondsRealtime(Config.TIME_TUTORIAL_TEXT_RUN);
                if (mScrollRect != null)
                    mScrollRect.verticalNormalizedPosition = 0f;
            }
            yield return new WaitForEndOfFrame();
            if (mScrollRect != null)
                mScrollRect.verticalNormalizedPosition = 0f;
        }
    }
}