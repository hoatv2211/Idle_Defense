using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;
using Utilities.Inspector;
using Utilities.Pattern.UI;

namespace FoodZombie.UI
{
    public class EvolutionPanel : MyGamesBasePanel
    {
        public GameObject groupListHeroes;

        public CurrencyView coinView;
        public CurrencyView gemView;
        public CurrencyView expHeroView;
        
        [Separator("Evolution")]
        public GameObject groupModel;
        private int selectHeroModel = -1;
        public GameObject[] modelHeroes;
        public Canvas layoutPrecedeModel;
        public HeroView[] materialSubs;
        public SimpleTMPButton btnFormulaBook;
        public SimpleTMPButton btnMaterial2;
        public SimpleTMPButton btnMaterial3;
        public SimpleTMPButton btnMaterial4;
        public SimpleTMPButton btnMaterial5;
        public SimpleTMPButton btnEvolution;
        public TextMeshProUGUI txtEvolutionCost;
        public Transform transformEvolutionPool;
        [SerializeField, Tooltip("Buildin Pool")] private List<HeroView> evolutionHeroViewsPool;
        private HeroData materialMain;

        [Separator("List Hero")]
        public SimpleTMPButton btnCloseListHeroPanel1;
        public SimpleTMPButton btnCloseListHeroPanel2;
        public Transform transformListHeroPool;
        [SerializeField, Tooltip("Buildin Pool")] private List<HeroView> listHeroViewsPool;
        private HeroView heroChoice;
        private int slotChoice;
        public SimpleTMPButton btnChoiceHero;
        
        //list all hero data
        private List<HeroData> heroDatas;
        private HeroEvolutionInfo heroEvolutionInfo;

        private UnityAction<HeroData> evolutionSuccess;
        
        private void Start()
        {
            btnFormulaBook.onClick.AddListener(BtnFormulaBook_Pressed);
            btnMaterial2.onClick.AddListener(BtnMaterial2_Pressed);
            btnMaterial3.onClick.AddListener(BtnMaterial3_Pressed);
            btnMaterial4.onClick.AddListener(BtnMaterial4_Pressed);
            btnMaterial5.onClick.AddListener(BtnMaterial5_Pressed);
            btnCloseListHeroPanel1.onClick.AddListener(BtnCloseListHeroPanel_Pressed);
            btnCloseListHeroPanel2.onClick.AddListener(BtnCloseListHeroPanel_Pressed);
            btnChoiceHero.onClick.AddListener(BtnChoiceHero_Pressed);
            btnEvolution.onClick.AddListener(BtnEvolution_Pressed);
            
            MainPanel.instance.onAnyChildHide += OnMainPanelChildHide;
            MainPanel.instance.onAnyChildShow += OnMainPanelChildShow;
        }
        
        private void OnDestroy()
        {
            MainPanel.instance.onAnyChildHide -= OnMainPanelChildHide;
            MainPanel.instance.onAnyChildShow -= OnMainPanelChildShow;
        }

        public void Init(HeroData heroData, UnityAction<HeroData> _evolutionSuccess)
        {
            coinView.Init(IDs.CURRENCY_COIN);
            gemView.Init(IDs.CURRENCY_GEM);
            expHeroView.Init(IDs.CURRENCY_EXP_HERO);

            evolutionSuccess = _evolutionSuccess;
            RefreshEvolution(heroData);
        }
        
        private void OnMainPanelChildHide(PanelController pPanel)
        {
            if (MainPanel.instance.TopPanel is EvolutionPanel)
            {
                groupModel.SetActive(true);
                layoutPrecedeModel.overrideSorting = true;
            }
            else
            {
                groupModel.SetActive(false);
                layoutPrecedeModel.overrideSorting = false;
            }
        }

        private void OnMainPanelChildShow(PanelController pPanel)
        {
            if (MainPanel.instance.TopPanel is EvolutionPanel)
            {
                groupModel.SetActive(true);
                layoutPrecedeModel.overrideSorting = true;
            }
            else
            {
                groupModel.SetActive(false);
                layoutPrecedeModel.overrideSorting = false;
            }
        }

        #region Evolution
        
        private void RefreshEvolution(HeroData heroData)
        {
            //cache
            heroDatas = GameData.Instance.HeroesGroup.GetAllHeroDatas();
            
            var count = materialSubs.Length;
            for (int i = 0; i < count; i++)
            {
                materialSubs[i].SetActive(false);
            }
            
            txtEvolutionCost.transform.parent.SetActive(false);
            
            materialMain = heroData;
            ShowHero(materialMain);
        }
        
        private void ShowHero(HeroData heroData)
        {
            if (heroData != null)
            {
                if (selectHeroModel != -1) modelHeroes[selectHeroModel].SetActive(false);
                selectHeroModel = heroData.baseId - 1;
                modelHeroes[selectHeroModel].SetActive(true);
                
                var count = materialSubs.Length;
                for (int i = 0; i < count; i++)
                {
                    materialSubs[i].SetActive(false);
                }
                heroEvolutionInfo = GameData.Instance.HeroesGroup.GetEvolutionInfoFromHeroBaseId(heroData.baseId);
                if (heroEvolutionInfo == null)
                {
                    MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_20));
                    //MainPanel.instance.ShowWarningPopup("This hero is max of rank");
                    btnEvolution.SetEnable(false);
                    txtEvolutionCost.transform.parent.SetActive(false);
                }
                else
                {
                    btnEvolution.SetEnable(true);
                    txtEvolutionCost.text = heroEvolutionInfo.coinCost + "";
                    txtEvolutionCost.transform.parent.SetActive(true);
                }
            }

            ShowHeroesForEvolution();
        }

        private void ChoiceEvolutionHeroView(HeroView heroView, bool isAdd)
        {
            if (isAdd)
            {
                if (!materialSubs[0].gameObject.activeSelf
                    && heroView.heroData.baseId == heroEvolutionInfo.idMaterial_2)
                {
                    materialSubs[0].Init(heroView.heroData, this);
                    materialSubs[0].ActiveArrow(false);
                    materialSubs[0].SetActive(true);

                    ShowHeroesForEvolution();
                }
                else if (!materialSubs[1].gameObject.activeSelf
                         && heroView.heroData.baseId == heroEvolutionInfo.idMaterial_3)
                {
                    materialSubs[1].Init(heroView.heroData, this);
                    materialSubs[1].ActiveArrow(false);
                    materialSubs[1].SetActive(true);

                    ShowHeroesForEvolution();
                }
                else if (!materialSubs[2].gameObject.activeSelf
                         && heroView.heroData.Rank == heroEvolutionInfo.rankMaterial_4
                         && heroView.heroData.Element == materialMain.Element)
                {
                    materialSubs[2].Init(heroView.heroData, this);
                    materialSubs[2].ActiveArrow(false);
                    materialSubs[2].SetActive(true);

                    ShowHeroesForEvolution();
                }
                else if (!materialSubs[3].gameObject.activeSelf
                         && heroView.heroData.Rank == heroEvolutionInfo.rankMaterial_5)
                {
                    materialSubs[3].Init(heroView.heroData, this);
                    materialSubs[3].ActiveArrow(false);
                    materialSubs[3].SetActive(true);

                    ShowHeroesForEvolution();
                }
            }
            else
            {
                var count = materialSubs.Length;
                for (int i = 0; i < count; i++)
                {
                    if (heroView.heroData.id == materialSubs[i].heroData.id)
                    {
                        materialSubs[i].SetActive(false);
                        ShowHeroesForEvolution();
                        break;
                    }
                }
            }
        }

        private void BtnFormulaBook_Pressed()
        {
            MainPanel.instance.ShowFormulaBookPanel(materialMain);

        }

        private List<HeroData> GetHeroesNotPick()
        {
            var heroes = new List<HeroData>(heroDatas);
            //lấy ra danh sách các hero chưa pick để evolution
            //bỏ những thằng đang equip + thằng main evolution
            var countHeroes = heroes.Count;
            for (int j = countHeroes - 1; j >= 0; j--)
            {
                var hero = heroes[j];
                if (heroes[j].IsEquipped() || hero.id == materialMain.id)
                {
                    heroes.RemoveAt(j);
                }
            }
            //4 thằng phụ material
            var count = materialSubs.Length;
            for (int i = 0; i < count; i++)
            {
                var heroNeedForEvolution = materialSubs[i];
                if (heroNeedForEvolution.gameObject.activeSelf)
                {
                    countHeroes = heroes.Count;
                    for (int j = 0; j < countHeroes; j++)
                    {
                        var hero = heroes[j];
                        if (hero.id == heroNeedForEvolution.heroData.id)
                        {
                            heroes.RemoveAt(j);
                            break;
                        }
                    }
                }
            }

            return heroes;
        }

        private void ShowHeroesForEvolution()
        {
            evolutionHeroViewsPool.Free();

            //đã có hero nào thì show ra
            var count = materialSubs.Length;
            for (int i = 0; i < count; i++)
            {
                var heroNeedForEvolution = materialSubs[i];
                if (heroNeedForEvolution.gameObject.activeSelf)
                {
                    var heroView = evolutionHeroViewsPool.Obtain(transformEvolutionPool);
                    heroView.Init(heroNeedForEvolution.heroData, this, ChoiceEvolutionHeroView, true);
                    heroView.SetActive(true);
                    heroView.Choice();
                }
            }

            //còn lại là các hero phù hợp với công thức
            var heroes = new List<HeroData>();
            var heroesNotPick = GetHeroesNotPick();
            count = heroesNotPick.Count;
            //2
            if (!materialSubs[0].gameObject.activeSelf)
            {
                for (int i = count - 1; i >= 0; i--)
                {
                    var hero = heroesNotPick[i];

                    if (heroEvolutionInfo.idMaterial_2 == hero.baseId)
                    {
                        heroes.Add(hero);
                        heroesNotPick.RemoveAt(i);
                    }
                }
            }

            //3
            if (!materialSubs[1].gameObject.activeSelf)
            {
                count = heroesNotPick.Count;
                for (int i = count - 1; i >= 0; i--)
                {
                    var hero = heroesNotPick[i];

                    if (heroEvolutionInfo.idMaterial_3 == hero.baseId)
                    {
                        heroes.Add(hero);
                        heroesNotPick.RemoveAt(i);
                    }
                }
            }

            //4
            if (!materialSubs[2].gameObject.activeSelf)
            {
                count = heroesNotPick.Count;
                for (int i = count - 1; i >= 0; i--)
                {
                    var hero = heroesNotPick[i];

                    if (heroEvolutionInfo.rankMaterial_4 == hero.Rank
                        && materialMain.Element == hero.Element)
                    {
                        heroes.Add(hero);
                        heroesNotPick.RemoveAt(i);
                    }
                }
            }

            //5
            if (!materialSubs[3].gameObject.activeSelf)
            {
                count = heroesNotPick.Count;
                for (int i = count - 1; i >= 0; i--)
                {
                    var hero = heroesNotPick[i];

                    if (heroEvolutionInfo.rankMaterial_5 == hero.Rank)
                    {
                        heroes.Add(hero);
                        // heroesNotPick.RemoveAt(i); //phat cuoi khoi remove
                    }
                }
            }

            //show
            count = heroes.Count;
            for (int i = 0; i < count; i++)
            {
                var heroView = evolutionHeroViewsPool.Obtain(transformEvolutionPool);
                heroView.Init(heroes[i], this, ChoiceEvolutionHeroView, false);
                heroView.SetActive(true);
            }
        }

        private void BtnMaterial2_Pressed()
        {
            if (selectHeroModel == -1)
            {
                MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_21));
                //MainPanel.instance.ShowWarningPopup("Please select below the other heroes to evolution first");
                return;
            }
            
            groupModel.SetActive(false);
            layoutPrecedeModel.overrideSorting = false;
            
            groupListHeroes.SetActive(true);
            
            listHeroViewsPool.Free();
            var heroes = GetHeroesNotPick();
            var count = heroes.Count;
            for (int i = 0; i < count; i++)
            {
                var hero = heroes[i];

                if (heroEvolutionInfo.idMaterial_2 == hero.baseId)
                {
                    var heroView = listHeroViewsPool.Obtain(transformListHeroPool);
                    heroView.Init(hero, this, ChoiceHero2ToEvolution);
                    heroView.SetActive(true);
                }
            }
            
            btnChoiceHero.SetEnable(false);
        }
        
        private void ChoiceHero2ToEvolution(HeroView heroView)
        {
            var count = listHeroViewsPool.Count;
            for (int i = 0; i < count; i++)
            {
                listHeroViewsPool[i].UnChoice();
            }

            heroView.Choice();
            
            slotChoice = 0;
            heroChoice = heroView;
            
            btnChoiceHero.SetEnable(true);
        }

        private void BtnMaterial3_Pressed()
        {
            if (selectHeroModel == -1)
            {
                MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_21));
                //MainPanel.instance.ShowWarningPopup("Please select below the other heroes to evolution first");
                return;
            }
            
            groupModel.SetActive(false);
            layoutPrecedeModel.overrideSorting = false;
            
            groupListHeroes.SetActive(true);
            
            listHeroViewsPool.Free();
            var heroes = GetHeroesNotPick();
            var count = heroes.Count;
            for (int i = 0; i < count; i++)
            {
                var hero = heroes[i];

                if (heroEvolutionInfo.idMaterial_3 == hero.baseId)
                {
                    var heroView = listHeroViewsPool.Obtain(transformListHeroPool);
                    heroView.Init(hero, this, ChoiceHero3ToEvolution);
                    heroView.SetActive(true);
                }
            }
            
            btnChoiceHero.SetEnable(false);
        }

        private void ChoiceHero3ToEvolution(HeroView heroView)
        {
            var count = listHeroViewsPool.Count;
            for (int i = 0; i < count; i++)
            {
                listHeroViewsPool[i].UnChoice();
            }

            heroView.Choice();
            
            slotChoice = 1;
            heroChoice = heroView;
            
            btnChoiceHero.SetEnable(true);
        }
        
        private void BtnMaterial4_Pressed()
        {
            if (selectHeroModel == -1)
            {
                MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_21));
                //MainPanel.instance.ShowWarningPopup("Please select below the other heroes to evolution first");
                return;
            }
            
            groupModel.SetActive(false);
            layoutPrecedeModel.overrideSorting = false;
            
            groupListHeroes.SetActive(true);
            
            listHeroViewsPool.Free();
            var heroes = GetHeroesNotPick();
            var count = heroes.Count;
            for (int i = 0; i < count; i++)
            {
                var hero = heroes[i];

                //Random Rank A same element Hero
                if (heroEvolutionInfo.rankMaterial_4 == hero.Rank
                    && materialMain.Element == hero.Element)
                {
                    var heroView = listHeroViewsPool.Obtain(transformListHeroPool);
                    heroView.Init(hero, this, ChoiceHero4ToEvolution);
                    heroView.SetActive(true);
                }
            }
            
            btnChoiceHero.SetEnable(false);
        }

        private void ChoiceHero4ToEvolution(HeroView heroView)
        {
            var count = listHeroViewsPool.Count;
            for (int i = 0; i < count; i++)
            {
                listHeroViewsPool[i].UnChoice();
            }

            heroView.Choice();
            
            slotChoice = 2;
            heroChoice = heroView;
            
            btnChoiceHero.SetEnable(true);
        }

        private void BtnMaterial5_Pressed()
        {
            if (selectHeroModel == -1)
            {
                MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_21));
                //MainPanel.instance.ShowWarningPopup("Please select below the other heroes to evolution first");
                return;
            }
            
            groupModel.SetActive(false);
            layoutPrecedeModel.overrideSorting = false;
            
            groupListHeroes.SetActive(true);
            
            listHeroViewsPool.Free();
            var heroes = GetHeroesNotPick();
            var count = heroes.Count;
            for (int i = 0; i < count; i++)
            {
                var hero = heroes[i];

                if (heroEvolutionInfo.rankMaterial_5 == hero.Rank)
                {
                    var heroView = listHeroViewsPool.Obtain(transformListHeroPool);
                    heroView.Init(hero, this, ChoiceHero5ToEvolution);
                    heroView.SetActive(true);
                }
            }
            
            btnChoiceHero.SetEnable(false);
        }

        private void ChoiceHero5ToEvolution(HeroView heroView)
        {
            var count = listHeroViewsPool.Count;
            for (int i = 0; i < count; i++)
            {
                listHeroViewsPool[i].UnChoice();
            }

            heroView.Choice();
            
            slotChoice = 3;
            heroChoice = heroView;
            
            btnChoiceHero.SetEnable(true);
        }
        
        private void BtnEvolution_Pressed()
        {
            if (selectHeroModel == -1
                || !materialSubs[0].gameObject.activeSelf
                || !materialSubs[1].gameObject.activeSelf
                || !materialSubs[2].gameObject.activeSelf
                || !materialSubs[3].gameObject.activeSelf)
            {
                MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_22));
                //MainPanel.instance.ShowWarningPopup("Please select heroes below to evolution first");
                return;
            }
            
            var coin = heroEvolutionInfo.coinCost;
            var currenciesGroup = GameData.Instance.CurrenciesGroup;
            if (!currenciesGroup.CanPay(IDs.CURRENCY_COIN, coin))
            {
                MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_9));
               // MainPanel.instance.ShowWarningPopup("Not enough coin");
                return;
            }
            
            currenciesGroup.Pay(IDs.CURRENCY_COIN, coin, TrackingConstants.VALUE_HERO_EVOLUTION);
            var heroData = GameData.Instance.HeroesGroup.Evolution(heroEvolutionInfo,
                                        materialMain,
                                        materialSubs[0].heroData, 
                                        materialSubs[1].heroData, 
                                        materialSubs[2].heroData, 
                                        materialSubs[3].heroData);

            SoundManager.Instance.PlaySFX(IDs.SOUND_EVOLUTION);
            Back();
            //MainPanel.instance.ShowRewardPopup(new RewardInfo(IDs.REWARD_TYPE_UNLOCK_CHARACTER, heroData.baseId, 1));
            evolutionSuccess(heroData);
        }
        
        //pick hero to evolution
        private void BtnCloseListHeroPanel_Pressed()
        {
            groupModel.SetActive(true);
            layoutPrecedeModel.overrideSorting = true;
            
            groupListHeroes.SetActive(false);
        }

        private void BtnChoiceHero_Pressed()
        {
            groupModel.SetActive(true);
            layoutPrecedeModel.overrideSorting = true;
            
            groupListHeroes.SetActive(false);

            materialSubs[slotChoice].Init(heroChoice.heroData, this);
            materialSubs[slotChoice].SetActive(true);
            
            ShowHeroesForEvolution();
        }

        #endregion
    }
}
