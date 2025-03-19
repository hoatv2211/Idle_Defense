using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;
using Facebook.Unity;
using FoodZombie;
using FoodZombie.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;
using Utilities.Inspector;
using Utilities.Service;

namespace FoodZombie.UI
{
    public class FormulaBookPanel : MyGamesBasePanel
    {
        public CurrencyView coinView;
        public CurrencyView gemView;
        public CurrencyView expHeroView;

        public SimpleTMPButton[] btnElements;
        public GameObject[] imgElementsActive;

        public Transform transformPoolSSTier;
        [SerializeField, Tooltip("Buildin Pool")] private List<FormulaView> listSSTier;
        public Transform transformPoolSTier;
        [SerializeField, Tooltip("Buildin Pool")] private List<FormulaView> listSTier;

        public RewardView heroNew;
        public RewardView heroMaterial1;
        public RewardView heroMaterial2;
        public RewardView heroMaterial3;
        public Color[] colorRanks;
        public Image imgElementMaterial4;
        public Image imgRankElementMaterial4;
        public Image imgRankMaterial4;
        public Image imgRankMaterial5;
        public Image imgRankBGMaterial4;
        public Image imgRankBGMaterial5;
        
        private int elementType;

        private void Start()
        {
            var count = btnElements.Length;
            for (int i = 0; i < count; i++)
            {
                int index = i + 1;
                btnElements[i].onClick.AddListener(() =>
                {
                    BtnElement_Pressed(index);
                });
            }
        }

        public void Init(HeroData heroDataMaterialMain)
        {
            elementType = heroDataMaterialMain.Element;

            var count = imgElementsActive.Length;
            for (int i = 0; i < count; i++)
            {
                imgElementsActive[i].SetActive(false);
            }
            imgElementsActive[elementType - 1].SetActive(true);
            
            coinView.Init(IDs.CURRENCY_COIN);
            gemView.Init(IDs.CURRENCY_GEM);
            expHeroView.Init(IDs.CURRENCY_EXP_HERO);

            //list
            listSSTier.Free();
            listSTier.Free();
            var heroEvolutionInfos = GameData.Instance.HeroesGroup.GetAllHeroEvolutionInfos();
            count = heroEvolutionInfos.Count;
            int baseHeroId = -1;
            for (int i = 0; i < count; i++)
            {
                var heroEvolutionInfo = heroEvolutionInfos[i];
                var heroDefinition = GameData.Instance.HeroesGroup.GetHeroDefinition(heroEvolutionInfo.heroId);
                if (heroDefinition.element == elementType)
                {
                    if (heroDefinition.rank == IDs.RANK_SS)
                    {
                        var formulaView = listSSTier.Obtain(transformPoolSSTier);
                        formulaView.Init(heroDefinition, ShowHeroFormula);
                        formulaView.SetActive(true);
                    }
                    else if (heroDefinition.rank == IDs.RANK_S)
                    {
                        var formulaView = listSTier.Obtain(transformPoolSTier);
                        formulaView.Init(heroDefinition, ShowHeroFormula);
                        formulaView.SetActive(true);
                    }

                    if (heroEvolutionInfo.idMaterial_1 == heroDataMaterialMain.baseId)
                    {
                        baseHeroId = heroEvolutionInfo.heroId;
                    }
                }
            }
            if (baseHeroId == -1)
            {
                ShowHeroFormula(listSSTier[0]);
                return;
            }
            else
            {
                count = listSSTier.Count;
                for (int i = 0; i < count; i++)
                {
                    var formulaView = listSSTier[i];
                    if (formulaView.heroDefinition.id == baseHeroId)
                    {
                        ShowHeroFormula(formulaView);
                        return;
                    }
                }

                count = listSTier.Count;
                for (int i = 0; i < count; i++)
                {
                    var formulaView = listSTier[i];
                    if (formulaView.heroDefinition.id == baseHeroId)
                    {
                        ShowHeroFormula(formulaView);
                        return;
                    }
                }
            }
        }

        private void BtnElement_Pressed(int element)
        {
            elementType = element;
            
            var count = imgElementsActive.Length;
            for (int i = 0; i < count; i++)
            {
                imgElementsActive[i].SetActive(false);
            }
            imgElementsActive[elementType - 1].SetActive(true);
            
            coinView.Init(IDs.CURRENCY_COIN);
            gemView.Init(IDs.CURRENCY_GEM);
            expHeroView.Init(IDs.CURRENCY_EXP_HERO);

            //list
            listSSTier.Free();
            listSTier.Free();
            var heroEvolutionInfos = GameData.Instance.HeroesGroup.GetAllHeroEvolutionInfos();
            count = heroEvolutionInfos.Count;
            bool isShow = false;
            for (int i = 0; i < count; i++)
            {
                var heroEvolutionInfo = heroEvolutionInfos[i];
                var heroDefinition = GameData.Instance.HeroesGroup.GetHeroDefinition(heroEvolutionInfo.heroId);
                if (heroDefinition.element == elementType)
                {
                    if (heroDefinition.rank == IDs.RANK_SS)
                    {
                        var formulaView = listSSTier.Obtain(transformPoolSSTier);
                        formulaView.Init(heroDefinition, ShowHeroFormula);
                        formulaView.SetActive(true);
                    }
                    else if (heroDefinition.rank == IDs.RANK_S)
                    {
                        var formulaView = listSTier.Obtain(transformPoolSTier);
                        formulaView.Init(heroDefinition, ShowHeroFormula);
                        formulaView.SetActive(true);
                    }
                }
            }
            ShowHeroFormula(listSSTier[0]);
        }

        private void ShowHeroFormula(FormulaView formulaView)
        {
            var heroId = formulaView.heroDefinition.id;
                
            var count = listSSTier.Count;
            for (int i = 0; i < count; i++)
            {
                listSSTier[i].UnChoice();
            }
            count = listSTier.Count;
            for (int i = 0; i < count; i++)
            {
                listSTier[i].UnChoice();
            }
            
            formulaView.Choice();
            
            //show
            var heroEvolutionInfos = GameData.Instance.HeroesGroup.GetAllHeroEvolutionInfos();
            count = heroEvolutionInfos.Count;
            for (int i = 0; i < count; i++)
            {
                var heroEvolutionInfo = heroEvolutionInfos[i];
                if (heroId == heroEvolutionInfo.heroId)
                {
                    var rewardInfo = new RewardInfo(IDs.REWARD_TYPE_UNLOCK_CHARACTER, heroEvolutionInfo.heroId, 1);
                    heroNew.Init(rewardInfo);
                    
                    rewardInfo = new RewardInfo(IDs.REWARD_TYPE_UNLOCK_CHARACTER, heroEvolutionInfo.idMaterial_1, 1);
                    heroMaterial1.Init(rewardInfo);
                    
                    rewardInfo = new RewardInfo(IDs.REWARD_TYPE_UNLOCK_CHARACTER, heroEvolutionInfo.idMaterial_2, 1);
                    heroMaterial2.Init(rewardInfo);
                    rewardInfo = new RewardInfo(IDs.REWARD_TYPE_UNLOCK_CHARACTER, heroEvolutionInfo.idMaterial_3, 1);
                    heroMaterial3.Init(rewardInfo);
                    
                    var rank4 = heroEvolutionInfo.rankMaterial_4;
                    imgRankMaterial4.sprite = AssetsCollection.instance.GetRankIcon(rank4);
                    imgRankBGMaterial4.color = colorRanks[rank4 - 1];
                    imgElementMaterial4.sprite = formulaView.heroDefinition.GetElementIcon();
                    imgRankElementMaterial4.sprite = AssetsCollection.instance.GetRankElementIcon(rank4);
                    var rank5 = heroEvolutionInfo.rankMaterial_5;
                    imgRankMaterial5.sprite = AssetsCollection.instance.GetRankIcon(rank5);
                    imgRankBGMaterial5.color = colorRanks[rank5 - 1];
                    
                    break;
                }
            }
        }
    }
}