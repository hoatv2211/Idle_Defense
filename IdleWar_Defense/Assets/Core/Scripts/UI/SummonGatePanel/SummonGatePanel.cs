using System;
using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using FoodZombie.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;
using Utilities.Pattern.UI;

namespace FoodZombie.UI
{
    public class SummonGatePanel : MyGamesBasePanel
    {
        public CurrencyView coinView;
        public CurrencyView gemView;
        public CurrencyView expHeroView;

        public CurrencyView powerFragmentView;
        public CurrencyView powerCrytalView;
        public CurrencyView devineCrytalView;

        public SimpleTMPButton btnPowerFragment;
        public SimpleTMPButton btnPowerFragmentX10;
        public SimpleTMPButton btnPowerCrytal;
        public SimpleTMPButton btnPowerCrytalX10;
        public SimpleTMPButton btnDevineCrytal;
        public SimpleTMPButton btnDevineCrytalX10;

        //free / 100 Gem / 1000 Gem
        public Image imgPowerFragment;
        public TextMeshProUGUI txtPowerFragment;
        public GameObject txtFreePowerFragment;

        public Image imgPowerCrytal;
        public TextMeshProUGUI txtPowerCrytal;
        public GameObject imgGem;
        public GameObject txtGem;
        public GameObject txtFreePowerCrytal;

        public Image imgPowerCrytalX10;
        public TextMeshProUGUI txtPowerCrytalX10;
        public GameObject imgGemX10;
        public GameObject txtGemX10;
        //end

        public GameObject groupBall, openBallEffect;
        public Canvas layoutPrecedeBall;

        [HideInInspector] public int TypeSummon = 1;     // Loại button summon hiện 
        [HideInInspector] public bool isTut = false;    //Check xem đã xong tutorial chưa
        

        public GameObject BlockAllActionPanel;
        public bool isOpeningGem { get; set; }
        // Start is called before the first frame update
        void Start()
        {
            btnPowerFragment.onClick.AddListener(BtnPowerFragment_Pressed);
            btnPowerFragmentX10.onClick.AddListener(BtnPowerFragmentX10_Pressed);
            btnPowerCrytal.onClick.AddListener(BtnPowerCrytal_Pressed);
            btnPowerCrytalX10.onClick.AddListener(BtnPowerCrytalX10_Pressed);
            btnDevineCrytal.onClick.AddListener(BtnDevineCrytal_Pressed);
            btnDevineCrytalX10.onClick.AddListener(BtnDevineCrytalX10_Pressed);
            
            // EventDispatcher.AddListener<CurrencyChangedEvent>(OnCurrencyChanged);

            MainPanel.instance.onAnyChildHide += OnMainPanelChildHide;
            MainPanel.instance.onAnyChildShow += OnMainPanelChildShow;
        }

        private void OnDestroy()
        {
            // EventDispatcher.RemoveListener<CurrencyChangedEvent>(OnCurrencyChanged);

            MainPanel.instance.onAnyChildHide -= OnMainPanelChildHide;
            MainPanel.instance.onAnyChildShow -= OnMainPanelChildShow;
        }

        private void OnEnable()
        {
            CheckPowerFrament();
            CheckPowerCrytal();
            BlockAllActionPanel.SetActive(false);

            isTut = !TutorialsManager.Instance.IsCompleted(TutorialsGroup.SUMMON_HERO_X10_HOME);
            //if (!TutorialsManager.Instance.IsCompleted(TutorialsGroup.SUMMON_HERO_SENIOR_HOME))
            //{
            //    EventDispatcher.Raise(new TutorialTriggeredEvent(TutorialsGroup.SUMMON_HERO_SENIOR_HOME));
            //}
            //else

        }

        public void Refresh()
        {
            CheckPowerFrament();
            CheckPowerCrytal();
            BlockAllActionPanel.SetActive(false);
        }

        internal override void Init()
        {
            coinView.Init(IDs.CURRENCY_COIN);
            gemView.Init(IDs.CURRENCY_GEM);
            expHeroView.Init(IDs.CURRENCY_EXP_HERO);

            powerFragmentView.Init(IDs.CURRENCY_POWER_FRAGMENT);
            powerCrytalView.Init(IDs.CURRENCY_POWER_CRYSTAL);
            devineCrytalView.Init(IDs.CURRENCY_DEVINE_CRYSTAL);

            
            //if (!TutorialsManager.Instance.IsCompleted(TutorialsGroup.SUMMON_HERO_X10_HOME))
            //{
            //    if (GameData.Instance.CurrenciesGroup.GetValue(IDs.CURRENCY_POWER_CRYSTAL) >= 10)
            //        EventDispatcher.Raise(new TutorialTriggeredEvent(TutorialsGroup.SUMMON_HERO_X10_HOME));

            //    isTut = true;
            //}
        }

        private void OnMainPanelChildHide(PanelController pPanel)
        {
            if (MainPanel.instance.TopPanel is SummonGatePanel)
            {
                groupBall.SetActive(true);
                layoutPrecedeBall.overrideSorting = true;
            }
            else
            {
                groupBall.SetActive(false);
                layoutPrecedeBall.overrideSorting = false;
            }
        }

        private void OnMainPanelChildShow(PanelController pPanel)
        {
            if (MainPanel.instance.TopPanel is SummonGatePanel)
            {
                groupBall.SetActive(true);
                layoutPrecedeBall.overrideSorting = true;
            }
            else
            {
                groupBall.SetActive(false);
                layoutPrecedeBall.overrideSorting = false;
            }
        }

        public void BtnPowerFragment_Pressed()
        {
            TypeSummon = 1;
            if (coroutineOpenGem != null) return;
            //Summon bổ sung : khi reset daily quest thì Basic summon và senior summon sẽ được free 1 lần ( sau 24h )
            if (!GameData.Instance.HeroesGroup.FreePowerFragmentSummon)
            {
                var cost = 1;
                var currenciesGroup = GameData.Instance.CurrenciesGroup;
                if (!currenciesGroup.CanPay(IDs.CURRENCY_POWER_FRAGMENT, cost))
                {
                    MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_34));
                   // MainPanel.instance.ShowWarningPopup("Not enough power fragments");
                    return;
                }

                currenciesGroup.Pay(IDs.CURRENCY_POWER_FRAGMENT, cost);
            }
            else
            {
                GameData.Instance.HeroesGroup.ClaimFreePowerFragmentSummon();
            }

            OpenGem(() =>
            {
                var rewardInfo = GameData.Instance.HeroesGroup.SummonByPowerFragment();
                LogicAPI.ClaimReward(rewardInfo, null, true, RewardsPopup.AlignType.Horizontal);
                CheckRewardInfo(rewardInfo);
                CheckPowerFrament();
                MainPanel.instance.MainMenuPanel.CheckNotif();

            });


        }

        public void BtnPowerFragmentX10_Pressed()
        {
            TypeSummon = 1;
            if (coroutineOpenGem != null) return;
            var xCount = 10;
            var cost = 1 * xCount; //x10
            var currenciesGroup = GameData.Instance.CurrenciesGroup;
            if (!currenciesGroup.CanPay(IDs.CURRENCY_POWER_FRAGMENT, cost))
            {
                MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_34));
                //MainPanel.instance.ShowWarningPopup("Not enough power fragments");
                return;
            }

            OpenGem(() =>
            {
                currenciesGroup.Pay(IDs.CURRENCY_POWER_FRAGMENT, cost);

                for (int i = 0; i < xCount; i++)
                {
                    var rewardInfo = GameData.Instance.HeroesGroup.SummonByPowerFragment();
                    LogicAPI.ClaimReward(rewardInfo, null, true, RewardsPopup.AlignType.Cicler);
                    CheckRewardInfo(rewardInfo);
                }

                CheckPowerFrament();
                MainPanel.instance.MainMenuPanel.CheckNotif();

            });


        }

        public void BtnPowerCrytal_Pressed()
        {
            TypeSummon = 2;
            if (coroutineOpenGem != null) return;
            //Summon bổ sung : khi reset daily quest thì Basic summon và senior summon sẽ được free 1 lần ( sau 24h )
            if (!GameData.Instance.HeroesGroup.FreePowerCrystalSummon)
            {
                var cost = 1;
                var currenciesGroup = GameData.Instance.CurrenciesGroup;
                if (!currenciesGroup.CanPay(IDs.CURRENCY_POWER_CRYSTAL, cost))
                {
                    cost = 100;
                    if (!currenciesGroup.CanPay(IDs.CURRENCY_GEM, cost))
                    {
                        MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_35));
                        //MainPanel.instance.ShowWarningPopup("Not enough power crystals or gems");
                        GameData.Instance.UserGroup.MissGemCount++;
                        return;
                    }
                    OpenGem(() =>
                    {
                        currenciesGroup.Pay(IDs.CURRENCY_GEM, cost, TrackingConstants.VALUE_SUMMON);
                        var rewardInfo = GameData.Instance.HeroesGroup.SummonByPowerCrytal();
                        LogicAPI.ClaimReward(rewardInfo);
                        CheckRewardInfo(rewardInfo);
                        //Summon 15 senior sẽ được free 1 Devine crystal.
                        rewardInfo = GameData.Instance.HeroesGroup.AddCountPowerCrystalSummon();
                        if (rewardInfo != null) LogicAPI.ClaimReward(rewardInfo);
                        CheckPowerCrytal();
                        MainPanel.instance.MainMenuPanel.CheckNotif();
                    });

                }
                else
                {
                    OpenGem(() =>
                    {
                        currenciesGroup.Pay(IDs.CURRENCY_POWER_CRYSTAL, cost);
                        var rewardInfo = GameData.Instance.HeroesGroup.SummonByPowerCrytal();
                        LogicAPI.ClaimReward(rewardInfo);
                        CheckRewardInfo(rewardInfo);
                        //Summon 15 senior sẽ được free 1 Devine crystal.
                        rewardInfo = GameData.Instance.HeroesGroup.AddCountPowerCrystalSummon();
                        if (rewardInfo != null) LogicAPI.ClaimReward(rewardInfo);
                        CheckPowerCrytal();
                        MainPanel.instance.MainMenuPanel.CheckNotif();
                    });

                }
            }
            else
            {
                OpenGem(() =>
                {
                    GameData.Instance.HeroesGroup.ClaimFreePowerCrystalSummon();

                    var rewardInfo = GameData.Instance.HeroesGroup.SummonByPowerCrytal();
                    LogicAPI.ClaimReward(rewardInfo); CheckRewardInfo(rewardInfo);
                    CheckPowerCrytal();
                    MainPanel.instance.MainMenuPanel.CheckNotif();
                });

            }

            CheckPowerCrytal();
            MainPanel.instance.MainMenuPanel.CheckNotif();
        }

        //x10 Tutorial
        public void BtnPowerCrytalX10_Pressed()
        {
            TypeSummon = 2;
            if (coroutineOpenGem != null) return;
            var xCount = 10;
            var cost = 1 * xCount; //x10
            var currenciesGroup = GameData.Instance.CurrenciesGroup;
            if (!currenciesGroup.CanPay(IDs.CURRENCY_POWER_CRYSTAL, cost))
            {
                cost = 100 * xCount;
                if (!currenciesGroup.CanPay(IDs.CURRENCY_GEM, cost))
                {
                    MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_35));
                    //MainPanel.instance.ShowWarningPopup("Not enough power crystals or gems");
                    GameData.Instance.UserGroup.MissGemCount++;
                    return;
                }
                OpenGem(() =>
                {
                    currenciesGroup.Pay(IDs.CURRENCY_GEM, cost, TrackingConstants.VALUE_SUMMON);
                    for (int i = 0; i < xCount; i++)
                    {
                        var rewardInfo = GameData.Instance.HeroesGroup.SummonByPowerCrytal(1);
                        LogicAPI.ClaimReward(rewardInfo, null, true, RewardsPopup.AlignType.Cicler);
                        CheckRewardInfo(rewardInfo);
                        //Summon 15 senior sẽ được free 1 Devine crystal.
                        rewardInfo = GameData.Instance.HeroesGroup.AddCountPowerCrystalSummon();
                        if (rewardInfo != null) LogicAPI.ClaimReward(rewardInfo, null, true, RewardsPopup.AlignType.Cicler);
                    }
                    CheckPowerCrytal();
                    MainPanel.instance.MainMenuPanel.CheckNotif();

                });

            }
            else
            {
                OpenGem(() =>
                {
                    currenciesGroup.Pay(IDs.CURRENCY_POWER_CRYSTAL, cost);
                    for (int i = 0; i < xCount; i++)
                    {
                        var rewardInfo = GameData.Instance.HeroesGroup.SummonByPowerCrytal(1);
                        LogicAPI.ClaimReward(rewardInfo, null, true, RewardsPopup.AlignType.Cicler);
                        CheckRewardInfo(rewardInfo);
                        //Summon 15 senior sẽ được free 1 Devine crystal.
                        rewardInfo = GameData.Instance.HeroesGroup.AddCountPowerCrystalSummon();
                        if (rewardInfo != null) LogicAPI.ClaimReward(rewardInfo, null, true, RewardsPopup.AlignType.Cicler);
                    }
                    CheckPowerCrytal();
                    MainPanel.instance.MainMenuPanel.CheckNotif();
                });

            }

            CheckPowerCrytal();
            MainPanel.instance.MainMenuPanel.CheckNotif();
        }

        public void BtnDevineCrytal_Pressed()
        {
            TypeSummon = 3;
            if (coroutineOpenGem != null) return;
            var cost = 1;
            var currenciesGroup = GameData.Instance.CurrenciesGroup;
            if (!currenciesGroup.CanPay(IDs.CURRENCY_DEVINE_CRYSTAL, cost))
            {
                MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_36));
                //MainPanel.instance.ShowWarningPopup("Not enough devine crystals");
                return;
            }
            OpenGem(() =>
            {

                currenciesGroup.Pay(IDs.CURRENCY_DEVINE_CRYSTAL, cost);

                var rewardInfo = GameData.Instance.HeroesGroup.SummonByDevineCrystal();
                LogicAPI.ClaimReward(rewardInfo);
                CheckRewardInfo(rewardInfo);
            });

        }

        public void BtnDevineCrytalX10_Pressed()
        {
            TypeSummon = 3;
            if (coroutineOpenGem != null) return;
            var xCount = 10;
            var cost = 1 * xCount; //x10
            var currenciesGroup = GameData.Instance.CurrenciesGroup;
            if (!currenciesGroup.CanPay(IDs.CURRENCY_DEVINE_CRYSTAL, cost))
            {
                MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_36));
                //MainPanel.instance.ShowWarningPopup("Not enough devine crystals");
                return;
            }
            OpenGem(() =>
            {

                currenciesGroup.Pay(IDs.CURRENCY_DEVINE_CRYSTAL, cost);

                for (int i = 0; i < xCount; i++)
                {
                    var rewardInfo = GameData.Instance.HeroesGroup.SummonByDevineCrystal();
                    LogicAPI.ClaimReward(rewardInfo, null, true, RewardsPopup.AlignType.Cicler);
                    CheckRewardInfo(rewardInfo);
                }
                MainPanel.instance.MainMenuPanel.CheckNotif();
            });

        }

        // private void OnCurrencyChanged(CurrencyChangedEvent e)
        // {
        //     if (e.id == IDs.CURRENCY_POWER_FRAGMENT)
        //     {
        //         CheckPowerFrament();
        //     }
        //     else if (e.id == IDs.CURRENCY_POWER_CRYSTAL)
        //     {
        //         CheckPowerCrytal();
        //     }
        // }

        private void CheckPowerFrament()
        {
            if (GameData.Instance.HeroesGroup.FreePowerFragmentSummon)
            {
                imgPowerFragment.SetActive(false);
                txtPowerFragment.SetActive(false);
                txtFreePowerFragment.SetActive(true);
            }
            else
            {
                imgPowerFragment.SetActive(true);
                txtPowerFragment.SetActive(true);
                txtFreePowerFragment.SetActive(false);
            }
        }

        private void CheckPowerCrytal()
        {
            if (GameData.Instance.HeroesGroup.FreePowerCrystalSummon)
            {
                imgPowerCrytal.SetActive(false);
                txtPowerCrytal.SetActive(false);
                txtFreePowerCrytal.SetActive(true);
            }
            else
            {
                if (GameData.Instance.CurrenciesGroup.GetValue(IDs.CURRENCY_POWER_CRYSTAL) <= 0)
                {
                    imgPowerCrytal.SetActive(false);
                    txtPowerCrytal.SetActive(false);
                    imgGem.SetActive(true);
                    txtGem.SetActive(true);
                    txtFreePowerCrytal.SetActive(false);
                }
                else
                {
                    imgPowerCrytal.SetActive(true);
                    txtPowerCrytal.SetActive(true);
                    imgGem.SetActive(false);
                    txtGem.SetActive(false);
                    txtFreePowerCrytal.SetActive(false);
                }
            }

            if (GameData.Instance.CurrenciesGroup.GetValue(IDs.CURRENCY_POWER_CRYSTAL) < 10)
            {
                imgPowerCrytalX10.SetActive(false);
                txtPowerCrytalX10.SetActive(false);
                imgGemX10.SetActive(true);
                txtGemX10.SetActive(true);
            }
            else
            {
                imgPowerCrytalX10.SetActive(true);
                txtPowerCrytalX10.SetActive(true);
                imgGemX10.SetActive(false);
                txtGemX10.SetActive(false);
            }
        }

        private IEnumerator coroutineOpenGem;
        private void OpenGem(Action OnPlayOpenEffectDone)
        {
            SoundManager.Instance.PlaySFX(IDs.SOUND_EVOLUTION);
            openBallEffect.SetActive(false);
            if (coroutineOpenGem != null)
            {
                StopCoroutine(coroutineOpenGem);
                coroutineOpenGem = null;
            }
            coroutineOpenGem = IEOpenGem(OnPlayOpenEffectDone);
            StartCoroutine(coroutineOpenGem);
        }

        IEnumerator IEOpenGem(Action OnPlayOpenEffectDone)
        {
            BlockAllActionPanel.SetActive(true);
            yield return null;
            btnPowerFragment.interactable = false;
            btnPowerFragmentX10.interactable = false;
            btnPowerCrytal.interactable = false;
            btnPowerCrytalX10.interactable = false;
            btnDevineCrytal.interactable = false;
            btnDevineCrytalX10.interactable = false;

            btnPowerFragment.SetEnable(false);
            btnPowerFragmentX10.SetEnable(false);
            btnPowerCrytal.SetEnable(false);
            btnPowerCrytalX10.SetEnable(false);
            btnDevineCrytal.SetEnable(false);
            btnDevineCrytalX10.SetEnable(false);

            btnBack.interactable = false;
            Lock(true);

            openBallEffect.SetActive(true);
            isOpeningGem = true;
            yield return new WaitForSeconds(4);
            openBallEffect.SetActive(false);
            coroutineOpenGem = null;

            btnPowerFragment.interactable = true;
            btnPowerFragmentX10.interactable = true;
            btnPowerCrytal.interactable = true;
            btnPowerCrytalX10.interactable = true;
            btnDevineCrytal.interactable = true;
            btnDevineCrytalX10.interactable = true;
            btnBack.interactable = true;

            btnPowerFragment.SetEnable(true);
            btnPowerFragmentX10.SetEnable(true);
            btnPowerCrytal.SetEnable(true);
            btnPowerCrytalX10.SetEnable(true);
            btnDevineCrytal.SetEnable(true);
            btnDevineCrytalX10.SetEnable(true);

            isOpeningGem = false;


            if (OnPlayOpenEffectDone != null)
                OnPlayOpenEffectDone();
            Lock(false);
            BlockAllActionPanel.SetActive(false);
        }


        private void CheckRewardInfo(RewardInfo reward)
        {
            if (reward.Type == IDs.REWARD_TYPE_UNLOCK_CHARACTER)
            {
                Debug.Log("REWARD_TYPE_UNLOCK_CHARACTER");
                var heroDefinition = GameData.Instance.HeroesGroup.GetHeroDefinition(reward.Id);
                Debug.Log(heroDefinition.rank);
                if (!GameData.Instance.GameConfigGroup.ShowedDailyRate && heroDefinition.rank == 4)
                {
                    GameData.Instance.UserGroup.RateCanOpen = true;
                }
            }
            if (reward.Type == IDs.REWARD_TYPE_FRAGMENT)
            {
                Debug.Log("REWARD_TYPE_FRAGMENT");
                var heroDefinition = GameData.Instance.HeroesGroup.GetHeroDefinition(reward.Id);
                Debug.Log(heroDefinition.rank);
            }
        }

        internal override void Back()
        {
            base.Back();
            if (!TutorialsManager.Instance.IsShowingFixedTut)
                MainPanel.instance.MainMenuPanel.ShowFirstPanel();
        }


    }


}
