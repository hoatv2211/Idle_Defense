using System;
using System.Collections.Generic;

using UnityEngine;
using Utilities.Common;
using Utilities.Pattern.Data;

namespace FoodZombie
{
    [System.Serializable]
    public class HeroFragment
    {
        public int id; 
        public int requireFragment;
    }

    //======================================

    public class HeroFragmentItemData : ConsumableItemData
    {
        #region Members

        public HeroFragment baseData { get; private set; }
        public HeroDefinition heroBase;
        
        #endregion

        //=============================================

        #region Public

        public HeroFragmentItemData(int _id, HeroFragment _baseData) : base(_id)
        {
            baseData = _baseData;
        }

        public override void AddToStock(int _value)
        {
            base.AddToStock(_value);
            
            EventDispatcher.Raise(new HeroFragmentChangeEvent());
        }

        public override Sprite GetIcon()
        {
            if (heroBase == null) heroBase = GameData.Instance.HeroesGroup.GetHeroDefinition(baseData.id);
            return heroBase.GetIcon();
        }
        
        public Sprite GetElementIcon()
        {
            if (heroBase == null) heroBase = GameData.Instance.HeroesGroup.GetHeroDefinition(baseData.id);
            return AssetsCollection.instance.elementIcon.GetAsset(heroBase.element - 1);
        }
        
        public Sprite GetRankIcon()
        {
            if (heroBase == null) heroBase = GameData.Instance.HeroesGroup.GetHeroDefinition(baseData.id);
            return AssetsCollection.instance.GetRankIcon(heroBase.rank);
        }

        public string GetName()
        {
            if (heroBase == null) heroBase = GameData.Instance.HeroesGroup.GetHeroDefinition(baseData.id);
            return "Fragments of " + heroBase.name + "";
        }
        
        public string GetRankName()
        {
            if (heroBase == null) heroBase = GameData.Instance.HeroesGroup.GetHeroDefinition(baseData.id);
            return heroBase.GetRankName();
        }

        public bool CanCraftHero()
        {
            return StockNumber >= baseData.requireFragment;
        }

        public List<RewardInfo> CraftHero()
        {
            if (CanCraftHero())
            {
                var rewards = new List<RewardInfo>();
                var count = StockNumber / baseData.requireFragment;
                for (int i = 0; i < count; i++)
                {
                    Use(baseData.requireFragment);
                    rewards.Add(new RewardInfo(IDs.REWARD_TYPE_UNLOCK_CHARACTER, baseData.id, 1));
                }

                return rewards;
            }

            return null;
        }

        public bool CheckNoti()
        {
            return CanCraftHero();
        }

        #endregion

        //==============================================

        #region Private

        #endregion
    }
}
