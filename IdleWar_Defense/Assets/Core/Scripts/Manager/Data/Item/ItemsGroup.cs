
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Common;
using Utilities.Pattern.Data;
using Random = UnityEngine.Random;

namespace FoodZombie
{
    public class ItemsGroup : DataGroup
    {
        #region Members

        private DataGroup heroFragmentItemsGroup;

        #endregion

        //=============================================

        #region Public

        public ItemsGroup(int pId) : base(pId)
        {
            heroFragmentItemsGroup = AddData(new DataGroup(0));

            InitHeroFragmentItems();
        }
        
        //=== Hero Fragment

        public HeroFragmentItemData GetHeroFragmentItem(int pId)
        {
            return heroFragmentItemsGroup.GetData<HeroFragmentItemData>(pId);
        }

        public List<HeroFragmentItemData> GetHeroFragmentItems(bool pSort = true)
        {
            var items = new List<HeroFragmentItemData>();
            foreach (HeroFragmentItemData item in heroFragmentItemsGroup.Children)
                items.Add(item);
            if (pSort)
                items.Sort();
            return items;
        }

        public HeroFragmentItemData GetRandomHeroFragmentItem()
        {
            var items = new List<HeroFragmentItemData>();
            foreach (HeroFragmentItemData item in heroFragmentItemsGroup.Children)
                items.Add(item);
            return items[Random.Range(0, items.Count)];
        }

        public HeroFragmentItemData AddHeroFragmentItem(int _id, int _value)
        {
            var heroFragment = GetHeroFragmentItem(_id);
            if (heroFragment != null)
                heroFragment.AddToStock(_value);
            return heroFragment;
        }

        public HeroFragmentItemData AddHeroFragmentItemRandomly(int pValue)
        {
            var heroFragment = GetRandomHeroFragmentItem();
            if (heroFragment != null)
            {
                heroFragment.AddToStock(pValue);
                return heroFragment;
            }
            return heroFragment;
        }

        public bool CheckNoti()
        {
            var currenciesGroup = GameData.Instance.CurrenciesGroup;
            var blueHeroFragment = currenciesGroup.GetValue(IDs.CURRENCY_BLUE_HERO_FRAGMENT);
            if (blueHeroFragment > Constants.COST_BLUE_HERO_FRAGMENT) return true;
            
            var epicHeroFragment = currenciesGroup.GetValue(IDs.CURRENCY_EPIC_HERO_FRAGMENT);
            if (epicHeroFragment > Constants.COST_EPIC_HERO_FRAGMENT) return true;

            var heroFragments = GetHeroFragmentItems();
            var count = heroFragments.Count;
            for (int i = 0; i < count; i++)
            {
                var heroFragment = heroFragments[i];
                if (heroFragment.CheckNoti()) return true;
            }
            
            return false;
        }

        public bool CheckNotiItem()
        {
            var currenciesGroup = GameData.Instance.CurrenciesGroup;
            var blueHeroFragment = currenciesGroup.GetValue(IDs.CURRENCY_BLUE_HERO_FRAGMENT);
            if (blueHeroFragment > Constants.COST_BLUE_HERO_FRAGMENT) return true;

            var epicHeroFragment = currenciesGroup.GetValue(IDs.CURRENCY_EPIC_HERO_FRAGMENT);
            if (epicHeroFragment > Constants.COST_EPIC_HERO_FRAGMENT) return true;

            return false;
        }

        public bool CheckNoticePart()
        {
            var heroFragments = GetHeroFragmentItems();
            var count = heroFragments.Count;
            for (int i = 0; i < count; i++)
            {
                var heroFragment = heroFragments[i];
                if (heroFragment.CheckNoti()) return true;
            }

            return false;
        }
        
        #endregion

        //==============================================

        #region Private
        
        private void InitHeroFragmentItems()
        {
            var dataContent = GameData.GetTextContent("Data/HeroFragment");
            var collection = JsonHelper.GetJsonList<HeroFragment>(dataContent);
            if (collection != null)
                foreach (var item in collection)
                {
                    var data = new HeroFragmentItemData(item.id, item);
                    heroFragmentItemsGroup.AddData(data);
                }

            //Debug.Log(collection);
        }

        #endregion
    }
}