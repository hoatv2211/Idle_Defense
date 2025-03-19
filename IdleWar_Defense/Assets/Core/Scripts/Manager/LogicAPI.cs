using FoodZombie.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Common;
using Random = UnityEngine.Random;

namespace FoodZombie
{
    public static class LogicAPI
    {
        private static GameData GameData => GameData.Instance;

        public static List<RewardInfo> ClaimRewards(List<RewardInfo> pRewards, string source = TrackingConstants.VALUE_EMPTY, bool showFx = true, Action OnshowDone = null)
        {
            var list = new List<RewardInfo>();
            foreach (var reward in pRewards)
            {
                var r = ClaimReward(reward, source, showFx, RewardsPopup.AlignType.Horizontal, OnshowDone);
                list.Add(r);
            }
            return list;
        }

        public static RewardInfo ClaimReward(RewardInfo pReward,
            string source = TrackingConstants.VALUE_EMPTY,
            bool showFx = true,
            RewardsPopup.AlignType mAlign = RewardsPopup.AlignType.Horizontal,
            Action OnShowDone = null
            )
        {
            var gameData = GameData.Instance;
            var currenciesGroup = gameData.CurrenciesGroup;
            var missionsGroup = gameData.MissionsGroup;
            var heroesGroup = gameData.HeroesGroup;
            var gearsGroup = gameData.GearsGroup;
            var wheelData = gameData.WheelData;
            var userGroup = gameData.UserGroup;
            var baseGroup = gameData.BaseGroup;
            bool randomize = pReward.Id <= -1;

            switch (pReward.Type)
            {
                case IDs.REWARD_TYPE_CURRENCY:
                    currenciesGroup.Add(pReward.Id, pReward.Value, source);
                    break;
                case IDs.REWARD_TYPE_UNLOCK_CHARACTER:
                    if (!randomize)
                    {
                        for (int i = 0; i < pReward.Value; i++)
                        {
                            heroesGroup.ClaimHero(pReward.Id);
                        }
                    }
                    break;
                case IDs.REWARD_TYPE_FRAGMENT:
                    if (!randomize)
                    {
                        gameData.ItemsGroup.AddHeroFragmentItem(pReward.Id, pReward.Value);
                    }
                    else
                    {
                        gameData.ItemsGroup.AddHeroFragmentItemRandomly(pReward.Value);
                    }
                    break;
                case IDs.REWARD_TYPE_GEAR:
                    if (!randomize)
                    {
                        for (int i = 0; i < pReward.Value; i++)
                        {
                            gearsGroup.ClaimGear(pReward.Id);
                        }
                    }
                    break;
                case IDs.REWARD_TYPE_EXP_USER:
                    userGroup.AddExp(pReward.Value);
                    break;
                case IDs.REWARD_TYPE_VIP:
                    userGroup.AddVipExp(pReward.Value);
                    break;
                case IDs.REWARD_TYPE_TRAP:
                    if (!randomize)
                    {
                        baseGroup.AddTrapItem(pReward.Id, pReward.Value);
                    }
                    else
                    {
                        var id = Config.EasyRandom(IDs.ITEM_TRAP_BARRIER, IDs.ITEM_TRAP_CANON);
                        pReward.SetId(id);
                        baseGroup.AddTrapItem(pReward.Id, pReward.Value);
                    }
                    break;
            }

            if (showFx)
            {
                if (MainPanel.instance != null)
                {
                    MainPanel.instance.ShowRewardPopup(pReward, null, mAlign, OnShowDone);
                }
                else if (MainGamePanel.instance != null)
                {
                    MainGamePanel.instance.ShowRewardPopup(pReward, null, mAlign, OnShowDone);
                }
            }

            return pReward;
        }

        public static int GetRandomRewardId(int mType)
        {
            switch (mType)
            {
                case IDs.REWARD_TYPE_CURRENCY:
                    return IDs.CURRENCY_COIN;
            }
            return -1;
        }

        public static string GetRewardName(int pType, int pId)
        {
            bool randomize = pId <= -1;

            switch (pType)
            {
                case IDs.REWARD_TYPE_CURRENCY:
                    return Localization.Get("CURRENCY_" + pId);
                    break;
                case IDs.REWARD_TYPE_UNLOCK_CHARACTER:
                    if (!randomize)
                    {
                        var heroDefinition = GameData.Instance.HeroesGroup.GetHeroDefinition(pId);
                        if (heroDefinition != null) return heroDefinition.name;
                    }
                    else
                    {
                        return Localization.Get(Localization.RANDOM_HERO);
                    }
                    break;
                case IDs.REWARD_TYPE_FRAGMENT:
                    if (!randomize)
                    {
                        var heroDefinition = GameData.Instance.HeroesGroup.GetHeroDefinition(pId);
                        if (heroDefinition != null) return "Fragments " + heroDefinition.name;
                    }
                    else
                    {
                        return Localization.Get(Localization.FRAGMENTS_HERO);
                    }
                    break;
                case IDs.REWARD_TYPE_TRAP:
                    if (!randomize)
                    {
                        var trapData = GameData.Instance.BaseGroup.GetTrapData(pId);
                        if (trapData != null)
                            return trapData.Name;
                    }
                    else
                    {
                        return Localization.Get(Localization.RANDOM_TRAP);
                    }
                    break;
                case IDs.REWARD_TYPE_COIN_BY_MISSION:
                    return Localization.Get("CURRENCY_" + IDs.CURRENCY_COIN);
                    break;
            }
            return "";
        }

        public static string GetShortDescription(int pType, int pId, int pValue)
        {
            bool randomize = pId <= -1;

            switch (pType)
            {
                case IDs.REWARD_TYPE_CURRENCY:
                    switch (pId)
                    {
                        case IDs.CURRENCY_GEM:
                            return "" + pValue;
                        case IDs.CURRENCY_COIN:
                            return "" + pValue;
                    }
                    break;
            }
            return "";
        }

        public static string GetDescription(int pType, int pId, int pValue)
        {
            bool randomize = pId <= -1;

            switch (pType)
            {
                case IDs.REWARD_TYPE_CURRENCY:
                    switch (pId)
                    {
                        case IDs.CURRENCY_GEM:
                            return "x" + pValue;
                        case IDs.CURRENCY_COIN:
                            return "x" + pValue;
                    }
                    break;
                case IDs.REWARD_TYPE_UNLOCK_CHARACTER:
                    {
                        if (!randomize)
                        {
                            var data = GameData.Instance.HeroesGroup.GetHeroData(pId);
                            if (data != null)
                                return string.Format(Localization.Get(Localization.GET_HERO), data.Name);
                        }
                        else
                        {
                            return string.Format(Localization.Get(Localization.GET_RANDOM_HERO));
                        }
                    }
                    break;
            }
            return "";
        }

        public static Sprite GetRewardIcon(int pType, int pId, int pValue = -1)
        {
            bool randomize = pId <= -1;

            var gameData = GameData.Instance;
            switch (pType)
            {
                case IDs.REWARD_TYPE_CURRENCY:
                    return AssetsCollection.instance.GetCurrencyIcon(pId, pValue);
                case IDs.REWARD_TYPE_EXP_USER:
                    return AssetsCollection.instance.GetExpUserIcon();
                case IDs.REWARD_TYPE_VIP:
                    return AssetsCollection.instance.GetVipIcon();
                case IDs.REWARD_TYPE_UNLOCK_CHARACTER:
                case IDs.REWARD_TYPE_FRAGMENT:
                    if (!randomize)
                    {
                        var heroDefinition = gameData.HeroesGroup.GetHeroDefinition(pId);
                        if (heroDefinition != null)
                            return heroDefinition.GetIcon();
                    }
                    else
                    {
                        return AssetsCollection.instance.GetRandomHeroIcon();
                    }

                    break;
                case IDs.REWARD_TYPE_GEAR:
                    if (!randomize)
                    {
                        var gearDefinition = gameData.GearsGroup.GetGearDefinition(pId);
                        if (gearDefinition != null)
                            return gearDefinition.GetIcon();
                    }
                    else
                    {
                        return AssetsCollection.instance.GetRandomGearIcon();
                    }

                    break;
                case IDs.REWARD_TYPE_TRAP:
                    if (!randomize)
                    {
                        var trapData = gameData.BaseGroup.GetTrapData(pId);
                        if (trapData != null)
                            return trapData.GetIcon();
                    }
                    else
                    {
                        return AssetsCollection.instance.GetRandomTrapIcon();
                    }

                    break;
            }
            return AssetsCollection.instance.common.GetAsset(FixedNames.ICON_RANDOM);
        }

        /// <summary>
        /// Return a random index from an array of chances
        /// NOTE: total of chances value does not need to match 100
        /// </summary>
        public static int CalcRandomWithChances(float[] chances)
        {
            int index = 0;
            float totalRatios = 0;
            for (int i = 0; i < chances.Length; i++)
                totalRatios += chances[i];

            float random = Random.Range(0, totalRatios);
            float temp2 = 0;
            for (int i = 0; i < chances.Length; i++)
            {
                temp2 += chances[i];
                if (temp2 > random)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        public static T CalcRandomWithDictionary<T>(Dictionary<T, float> pChoices)
        {
            float[] chances = new float[pChoices.Count];
            int index = 0;
            foreach (var choice in pChoices)
            {
                chances[index] = choice.Value;
                index++;
            }

            T firstChoice = default(T);
            int randomIndex = CalcRandomWithChances(chances);
            index = 0;
            foreach (var choice in pChoices)
            {
                if (index == 0)
                    firstChoice = choice.Key;
                if (index == randomIndex)
                    return choice.Key;
                index++;
            }
            return firstChoice;
        }

        public static bool CanShowRatePanel()
        {
            return (GameData.MissionsGroup.CurrentMissionId > 1005
                    && !GameData.Instance.GameConfigGroup.Rated
                    /*|| RatePanel.isShowed*/
                    && GameData.Instance.UserGroup.RateCanOpen);
        }

        public static bool CanShowSpecialPackInMainMenu()
        {
            return false;
            return GameData.MissionsGroup.CurrentMissionId > 1005;
        }

        public static void LoadAllHeroToFormation()
        {
            var formations = new HeroData[HeroesGroup.MAX_FORMATION][];
            for (int i = 0; i < HeroesGroup.MAX_FORMATION; i++)
            {
                formations[i] = GameData.Instance.HeroesGroup.GetEquippedHeroes(i);
            }
            var allHeroes = GameData.Instance.HeroesGroup.GetAllHeroDatas();
            for (int i = 0; i < allHeroes.Count; i++)
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
            GameData.Instance.HeroesGroup.SaveEquippedHeros(newFormations);
        }
    }
}