using System;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Inspector;
using Object = UnityEngine.Object;

namespace FoodZombie
{
	[System.Serializable]
	public class ListSprite/* : Object*/
	{
		public List<Sprite> sprites;

		public ListSprite()
		{

		}

		public ListSprite(List<Sprite> _list)
		{
			sprites = _list;
		}
	}

	public class AssetGetter<T> where T : Object
	{
		public List<T> source;

		public AssetGetter(List<T> pSource)
		{
			source = pSource;
		}

		public AssetGetter<T> Init(List<T> pSource)
		{
			source = pSource;
			return this;
		}

		public T GetAsset(string pName)
		{
			foreach (var s in source)
				if (s != null && pName != null && s.name.ToLower() == pName.ToLower())
					return s;

			Debug.LogError(string.Format("Not found {0} with name {1}", typeof(T).Name, pName));
			return null;
		}

		public T GetAsset(int pIndex)
		{
			if (pIndex < 0 || pIndex >= source.Count)
			{
				Debug.LogError(string.Format("Index {0} {1} is invalid!", pIndex, typeof(T).Name));
				return default(T);
			}
			return source[pIndex];
		}

		public int GetAssetIndex(string pName)
		{
			for (int i = 0; i < source.Count; i++)
			{
				if (source[i].name == pName)
					return i;
			}
			Debug.LogError(string.Format("Not found {0} with name {1}", typeof(T).Name, pName));
			return -1;
		}
	}

	//============================================================

	public static class FixedNames
	{
		public const string ICON_RANDOM = "random";
		public const string ICON_COIN = "coin";
		public const string ICON_GEM = "gem";
		public const string ICON_EXP_USER = "expUser";
		public const string ICON_VIP = "vip";
		public const string ICON_EXP_HERO = "expHero";
		public const string ICON_ELEMENT_ELECTRIC = "elementCurrencyElectric";
		public const string ICON_ELEMENT_METALIC = "elementCurrencyMetalic";
		public const string ICON_ELEMENT_NITROGEN = "elementCurrencyNitrogen";
		public const string ICON_ELEMENT_LAVA = "elementCurrencyLava";
		public const string ICON_TICKET = "ticket";
		public const string ICON_TICKET_PVP = "ticketPvP";
		public const string ICON_HONOR = "medal";
		public const string ICON_POWER_FRAGMENT = "powerFragment";
		public const string ICON_POWER_CRYTAL = "powerCrytal";
		public const string ICON_DEVINE_CRYTAL = "devineCrytal";
		public const string ICON_MATERIAL = "material";
		public const string ICON_BLUE_CHIP = "blueChip";
		public const string ICON_GOLDEN_CHIP = "goldenChip";
		public const string ICON_BLUE_HERO_FRAGMENT = "blueHeroFragment";
		public const string ICON_EPIC_HERO_FRAGMENT = "epicHeroFragment";
	}

	//============================================================

	[CreateAssetMenu(fileName = "AssetsCollection", menuName = "Assets/Scriptable Objects/Create Assets Collection")]
	public class AssetsCollection : ScriptableObject
	{
		#region Constants

		#endregion

		//========================================================

		#region Members 

		private static AssetsCollection mInstance;
		public static AssetsCollection instance
		{
			get
			{
				if (mInstance == null)
					mInstance = Resources.Load<AssetsCollection>("Collection/AssetsCollection");
				return mInstance;
			}
		}

		//private Material mMatGrey;
		//public Material matGrey
		//{
		//    get
		//    {
		//        if (mMatGrey == null)
		//            mMatGrey = new Material(Shader.Find("NBCustom/Sprites/Greyscale"));
		//        return mMatGrey;
		//    }
		//}

		[Separator("Common")]
		public Material matDefaultSekelton;
		public Material matImageFill;

		[Separator("Icons Managed by Name")]
		public List<Sprite> commonSprites;

		[Separator("Icons Managed by Id")]
		public List<Sprite> heroIconSprites;
		public List<Sprite> enemyIconSprites;
		public List<Sprite> gearIconSprites;
		public List<Sprite> elementIconSprites;
		public List<Sprite> rankSprites;
		public List<Sprite> rankElementSprites;
		public List<Sprite> trapSprites;
		public List<Sprite> heroWhoSprites;
		// public List<ListSprite> heroIconSkillSprites;
		public List<ListSprite> heroIconSkills;
		public List<GameObject> heroCardEffectOpen;
		public List<GameObject> heroCardEffectLoop;
		[Separator("Animation")]
		public List<SkeletonDataAsset> heroAnimationDataAssets;
		public List<SkeletonDataAsset> heroMainAnimationDataAssets;
		public List<SkeletonDataAsset> enemyAnimationDataAssets;

		private AssetGetter<Sprite> mSpritesGetter = new AssetGetter<Sprite>(null);
		public AssetGetter<Sprite> common => mSpritesGetter.Init(commonSprites);
		public AssetGetter<Sprite> heroIcon => mSpritesGetter.Init(heroIconSprites);
		public AssetGetter<Sprite> enemyIcon => mSpritesGetter.Init(enemyIconSprites);
		public AssetGetter<Sprite> gearIcon => mSpritesGetter.Init(gearIconSprites);
		public AssetGetter<Sprite> elementIcon => mSpritesGetter.Init(elementIconSprites);
		public AssetGetter<Sprite> ranks => mSpritesGetter.Init(rankSprites);
		public AssetGetter<Sprite> rankElements => mSpritesGetter.Init(rankElementSprites);
		public AssetGetter<Sprite> traps => mSpritesGetter.Init(trapSprites);


		// private AssetGetter<ListSprite> mListSpritesGetter = new AssetGetter<ListSprite>(null);
		// public AssetGetter<ListSprite> heroIconSkills => mListSpritesGetter.Init(heroIconSkillSprites);


		private AssetGetter<SkeletonDataAsset> mAnimationsGetter = new AssetGetter<SkeletonDataAsset>(null);
		public AssetGetter<SkeletonDataAsset> heroAnimations => mAnimationsGetter.Init(heroAnimationDataAssets);
		public AssetGetter<SkeletonDataAsset> heroMainAnimations => mAnimationsGetter.Init(heroMainAnimationDataAssets);
		public AssetGetter<SkeletonDataAsset> enemyAnimations => mAnimationsGetter.Init(enemyAnimationDataAssets);


		#endregion

		//=======================================================

		#region Private

		private T GetAsset<T>(List<T> pSource, string pName) where T : UnityEngine.Object
		{
			foreach (var s in pSource)
				if (s != null && pName != null && s.name.ToLower() == pName.ToLower())
					return s;

			Debug.LogError(string.Format("Not found {0} with name {1}", typeof(T).Name, pName));
			return null;
		}

		private T GetAsset<T>(List<T> pSource, int pIndex)
		{
			if (pIndex < 0 || pIndex >= pSource.Count)
			{
				Debug.LogError(string.Format("Index {0} {1} is invalid!", pIndex, typeof(T).Name));
				return default(T);
			}
			return pSource[pIndex];
		}

		private int GetAssetIndex<T>(List<T> pSource, string pName) where T : UnityEngine.Object
		{
			for (int i = 0; i < pSource.Count; i++)
			{
				if (pSource[i].name == pName)
					return i;
			}
			Debug.LogError(string.Format("Not found {0} with name {1}", typeof(T).Name, pName));
			return -1;
		}

		#endregion

		//=======================================================

		#region Public

		public Sprite GetCurrencyIcon(int pType, int pValue = -1)
		{
			switch (pType)
			{
				case IDs.CURRENCY_GEM:
					return common.GetAsset(FixedNames.ICON_GEM);
				case IDs.CURRENCY_COIN:
					if (pValue == -1) return common.GetAsset(FixedNames.ICON_COIN);
					else return GetCoinIcon();
				case IDs.CURRENCY_EXP_HERO:
					return common.GetAsset(FixedNames.ICON_EXP_HERO);
				case IDs.CURRENCY_DUST_ELECTRIC:
					return common.GetAsset(FixedNames.ICON_ELEMENT_ELECTRIC);
				case IDs.CURRENCY_DUST_METALIC:
					return common.GetAsset(FixedNames.ICON_ELEMENT_METALIC);
				case IDs.CURRENCY_DUST_NITROGEN:
					return common.GetAsset(FixedNames.ICON_ELEMENT_NITROGEN);
				case IDs.CURRENCY_DUST_LAVA:
					return common.GetAsset(FixedNames.ICON_ELEMENT_LAVA);
				case IDs.CURRENCY_TICKET:
					return common.GetAsset(FixedNames.ICON_TICKET);
				case IDs.CURRENCY_TICKET_PVP:
					return common.GetAsset(FixedNames.ICON_TICKET_PVP);
				case IDs.CURRENCY_HONOR:
					return common.GetAsset(FixedNames.ICON_HONOR);
				case IDs.CURRENCY_POWER_FRAGMENT:
					return common.GetAsset(FixedNames.ICON_POWER_FRAGMENT);
				case IDs.CURRENCY_POWER_CRYSTAL:
					return common.GetAsset(FixedNames.ICON_POWER_CRYTAL);
				case IDs.CURRENCY_DEVINE_CRYSTAL:
					return common.GetAsset(FixedNames.ICON_DEVINE_CRYTAL);
				case IDs.CURRENCY_MATERIAL:
					return common.GetAsset(FixedNames.ICON_MATERIAL);
				case IDs.CURRENCY_BLUE_CHIP:
					return common.GetAsset(FixedNames.ICON_BLUE_CHIP);
				case IDs.CURRENCY_GOLDEN_CHIP:
					return common.GetAsset(FixedNames.ICON_GOLDEN_CHIP);
				case IDs.CURRENCY_BLUE_HERO_FRAGMENT:
					return common.GetAsset(FixedNames.ICON_BLUE_HERO_FRAGMENT);
				case IDs.CURRENCY_EPIC_HERO_FRAGMENT:
					return common.GetAsset(FixedNames.ICON_EPIC_HERO_FRAGMENT);
				default:
					return null;
			}
		}

		public Sprite GetCoinIcon()
		{
			/*if (pValue <= -1)*/
			return common.GetAsset(FixedNames.ICON_COIN);
			// else if (pValue <= LogicAPI.GetRewardGoldByMission(1, 1)) return common.GetAsset(FixedNames.ICON_GOLD_REWARD_1);
			// else if (pValue <= LogicAPI.GetRewardGoldByMission(1, 2)) return common.GetAsset(FixedNames.ICON_GOLD_REWARD_2);
			// else return common.GetAsset(FixedNames.ICON_GOLD_REWARD_3);
		}

		public Sprite GetGemIcon()
		{
			return common.GetAsset(FixedNames.ICON_GEM);
		}

		public Sprite GetElementIcon(int elementId)
		{
			switch (elementId)
			{
				case IDs.ELEMENT_ELECTRIC:
					return common.GetAsset(FixedNames.ICON_ELEMENT_ELECTRIC);
				case IDs.ELEMENT_METALIC:
					return common.GetAsset(FixedNames.ICON_ELEMENT_METALIC);
				case IDs.ELEMENT_NITROGEN:
					return common.GetAsset(FixedNames.ICON_ELEMENT_NITROGEN);
				case IDs.ELEMENT_LAVA:
					return common.GetAsset(FixedNames.ICON_ELEMENT_LAVA);
				default:
					return null;
			}
		}

		public Sprite GetExpUserIcon()
		{
			return common.GetAsset(FixedNames.ICON_EXP_USER);
		}

		public Sprite GetVipIcon()
		{
			return common.GetAsset(FixedNames.ICON_VIP);
		}

		public Sprite GetRandomHeroIcon()
		{
			return common.GetAsset(FixedNames.ICON_RANDOM);
		}

		public Sprite GetRandomGearIcon()
		{
			return common.GetAsset(FixedNames.ICON_RANDOM);
		}

		public Sprite GetRandomTrapIcon()
		{
			return common.GetAsset(FixedNames.ICON_RANDOM);
		}

		public Sprite GetRankIcon(int pRank)
		{
			if (pRank > ranks.source.Count) pRank = ranks.source.Count;
			return ranks.GetAsset(pRank - 1);
		}

		public Sprite GetRankElementIcon(int pRank)
		{
			if (pRank > rankElements.source.Count) pRank = rankElements.source.Count;
			return rankElements.GetAsset(pRank - 1);
		}

		public Sprite GetHeroSkillIcon(int pHeroId, int pSkillIndex)
		{
			// return heroIconSkills.GetAsset(pHeroId - 1).sprites[pSkillIndex];
			return heroIconSkills[pHeroId - 1].sprites[pSkillIndex];
		}
		public Sprite GetHeroWhoSpriteByRank(int pRank)
		{
			if (pRank > heroWhoSprites.Count) pRank = heroWhoSprites.Count;
			return heroWhoSprites[pRank - 1];
		}
		public GameObject GetHeroCardEffectOpenByRank(int pRank)
		{
			if (pRank > heroCardEffectOpen.Count) pRank = heroCardEffectOpen.Count;
			return heroCardEffectOpen[pRank - 1];
		}
		public GameObject GetHeroCardEffectLoopByRank(int pRank)
		{
			if (pRank > heroCardEffectLoop.Count) pRank = heroCardEffectLoop.Count;
			return heroCardEffectLoop[pRank - 1];
		}
		public void Init()
		{
			if (heroAnimationDataAssets != null)
			{
				int length = heroAnimationDataAssets.Count;
				for (int i = 0; i < length; i++)
				{
					heroAnimationDataAssets[i].GetSkeletonData(false);
				}
			}
			if (heroMainAnimationDataAssets != null)
			{
				int length = heroMainAnimationDataAssets.Count;
				for (int i = 0; i < length; i++)
				{
					heroMainAnimationDataAssets[i].GetSkeletonData(false);
				}
			}
			if (enemyAnimationDataAssets != null)
			{
				int length = enemyAnimationDataAssets.Count;
				for (int i = 0; i < length; i++)
				{
					enemyAnimationDataAssets[i].GetSkeletonData(false);
				}
			}
		}
		#endregion
	}
}