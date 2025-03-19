using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FoodZombie.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;

namespace FoodZombie
{
	public class RewardView : MonoBehaviour
	{
		public bool tweenValue = true;
		public bool formatKKK = false;

		public Image imgBg;
		public Image imgIcon;
		public Image imgElement;
		public Image imgFragment;
		public Image imgRank;
		public Image imgRankBG;
		public Color[] colorRanks;
		public Image imgRankElement;
		public Image imgRewarded;
		public TextMeshProUGUI txtQuantity;

		public bool CanShowInfor = true;
		private RewardInfo _RewardInfo;
		private GameObject _Effect;
		public void OnClickEvent()
		{
			// Debug,K
			if (!CanShowInfor) return;
			if (_RewardInfo != null)
			{
				if (_RewardInfo.Type == IDs.REWARD_TYPE_TRAP)
				{
					TrapData _trap = GameData.Instance.BaseGroup.GetTrapData(_RewardInfo.Id);
					ShowItemDetailPanel(_trap, _RewardInfo.Id);
				}
				else
				if (_RewardInfo.Type == IDs.REWARD_TYPE_GEAR)
				{
					// TrapData _trap = GameData.Instance.BaseGroup.Gear(_RewardInfo.Id);
					// GearDefinition gearDefinition = GameData.Instance.GearsGroup.GetGearDefinition(_RewardInfo.Id);
					GearData _gear = new GearData(0, _RewardInfo.Id);
					ShowGearDetailPanel(_gear);
				}
				else
				 if (_RewardInfo.Type == IDs.REWARD_TYPE_CURRENCY)
					ShowItemDetailPanel(null, _RewardInfo.Id);
				else
					 if (_RewardInfo.Type != IDs.REWARD_TYPE_UNLOCK_CHARACTER)
					ShowItemOtherDetailPanel(_RewardInfo);
			}
		}

		void ShowItemDetailPanel(TrapData _trap, int id)
		{
			if (MainPanel.instance != null)
				MainPanel.instance.ShowItemDetailPanel(_trap, id, null);
			if (MainGamePanel.instance != null)
				MainGamePanel.instance.ShowItemDetailPanel(_trap, id, null);
		}
		void ShowGearDetailPanel(GearData gear)
		{
			if (MainPanel.instance != null)
				MainPanel.instance.ShowGearDetailPanel(gear, null, false);
			if (MainGamePanel.instance != null)
				MainGamePanel.instance.ShowGearDetailPanel(gear, null, false);
		}
		void ShowItemOtherDetailPanel(RewardInfo reward)
		{
			if (MainPanel.instance != null)
				MainPanel.instance.ShowItemOtherDetailPanel(_RewardInfo, null);
			if (MainGamePanel.instance != null)
				MainGamePanel.instance.ShowItemOtherDetailPanel(_RewardInfo, null);
		}
		//delay for grid layout
		public void Init(RewardInfo pReward, bool showValue = true)
		{
			this._RewardInfo = pReward;
			isPlayWho = false;
			CoroutineUtil.StartCoroutine(IEInit(pReward, showValue));
		}

		public void SetRewadInPvP(RewardInfo pReward, bool _rewarded)
		{
			this._RewardInfo = pReward;
			isPlayWho = false;
			imgRewarded.SetActive(_rewarded);
			CoroutineUtil.StartCoroutine(IEInit(pReward, true));
		}
		//public void ReloadEffect()
		//{
		//    if (_Effect == null) return;
		//    _Effect.SetActive(false);
		//    _Effect.SetActive(true);
		//}
		bool isPlayWho = false;
		public void PlayGuestWhoEffect(bool showWho)
		{

			if (this._RewardInfo == null) return;
			if (this._RewardInfo.Type == IDs.REWARD_TYPE_UNLOCK_CHARACTER)
			{
				var heroDefinition = GameData.Instance.HeroesGroup.GetHeroDefinition(_RewardInfo.Id);
				if (imgIcon != null)
				{
					imgIcon.SetActive(true);
					//	imgElement.sprite = heroDefinition.GetElementIcon();
					isPlayWho = showWho;

					if (showWho)
						imgIcon.sprite = AssetsCollection.instance.GetHeroWhoSpriteByRank(heroDefinition.rank);
					else
						imgIcon.sprite = this._RewardInfo.GetIcon();
				}
			}
		}
		public void PlayFlipWhoEffect()
		{
			StartCoroutine(IEPlayFlipWhoEffect());
		}
		IEnumerator IEPlayFlipWhoEffect()
		{
			//isPlayWho = true;
			// Debug.Log(gameObject.name + " PLay flip");
			// yield return null;
			PlayGuestWhoEffect(true);
			PlayEffectOpen();
			yield return new WaitForSecondsRealtime(1);
			PlayGuestWhoEffect(false);
			// yield return new WaitForSeconds(1);
			PlayEffectLoop();
			//	isPlayWho = false;
		}
		public void PlayEffectOpen()
		{
			if (this._RewardInfo == null) return;
			if (this._RewardInfo.Type == IDs.REWARD_TYPE_UNLOCK_CHARACTER)
			{
				var heroDefinition = GameData.Instance.HeroesGroup.GetHeroDefinition(_RewardInfo.Id);
				SetOpenEffect(heroDefinition.rank);
			}
		}
		public void PlayEffectLoop()
		{
			if (this._RewardInfo == null) return;
			if (this._RewardInfo.Type == IDs.REWARD_TYPE_UNLOCK_CHARACTER)
			{
				var heroDefinition = GameData.Instance.HeroesGroup.GetHeroDefinition(_RewardInfo.Id);
				SetLoopEffect(heroDefinition.rank);
			}
		}
		private IEnumerator IEInit(RewardInfo pReward, bool showValue)
		{
			yield return null;
			if (this._Effect != null)
			{
				SimplePool.Despawn(this._Effect);
				this._Effect = null;
			}
			if (!isPlayWho && imgIcon != null)
				imgIcon.sprite = pReward.GetIcon();

			if (pReward.Type == IDs.REWARD_TYPE_CURRENCY)
			{
				//no element
				if (imgElement != null) imgElement.SetActive(false);
				//no fragment
				if (imgFragment != null) imgFragment.SetActive(false);
				//rank theo wheel data ranks
				switch (pReward.Id)
				{
					case IDs.CURRENCY_POWER_FRAGMENT:
						if (pReward.Value >= 1) SetImgRank(IDs.RANK_B);
						break;
					case IDs.CURRENCY_POWER_CRYSTAL:
						if (pReward.Value >= 3) SetImgRank(IDs.RANK_S);
						else if (pReward.Value >= 1) SetImgRank(IDs.RANK_A);
						break;
					case IDs.CURRENCY_DEVINE_CRYSTAL:
						if (pReward.Value >= 1) SetImgRank(IDs.RANK_SS);
						break;
					case IDs.CURRENCY_TICKET:
						if (pReward.Value >= 1) SetImgRank(IDs.RANK_A);
						break;
					case IDs.CURRENCY_TICKET_PVP:
						if (pReward.Value >= 1) SetImgRank(IDs.RANK_A);
						break;
					case IDs.CURRENCY_HONOR:
						if (pReward.Value >= 1) SetImgRank(IDs.RANK_A);
						break;
					case IDs.CURRENCY_MATERIAL:
						if (pReward.Value >= 1000) SetImgRank(IDs.RANK_S);
						else if (pReward.Value >= 100) SetImgRank(IDs.RANK_A);
						else if (pReward.Value >= 1) SetImgRank(IDs.RANK_B);
						break;
					case IDs.CURRENCY_EXP_HERO:
						if (pReward.Value >= 50000) SetImgRank(IDs.RANK_A);
						else if (pReward.Value >= 1) SetImgRank(IDs.RANK_B);
						break;
					case IDs.CURRENCY_GEM:
						if (pReward.Value >= 1000) SetImgRank(IDs.RANK_SS);
						else if (pReward.Value >= 200) SetImgRank(IDs.RANK_S);
						else if (pReward.Value >= 1) SetImgRank(IDs.RANK_A);
						break;
					case IDs.CURRENCY_COIN:
						if (pReward.Value >= 100000) SetImgRank(IDs.RANK_A);
						else if (pReward.Value >= 1) SetImgRank(IDs.RANK_B);
						break;
					case IDs.CURRENCY_DUST_ELECTRIC:
						if (pReward.Value >= 100) SetImgRank(IDs.RANK_S);
						else if (pReward.Value >= 10) SetImgRank(IDs.RANK_A);
						else if (pReward.Value >= 1) SetImgRank(IDs.RANK_B);
						break;
					case IDs.CURRENCY_DUST_METALIC:
						if (pReward.Value >= 100) SetImgRank(IDs.RANK_S);
						else if (pReward.Value >= 10) SetImgRank(IDs.RANK_A);
						else if (pReward.Value >= 1) SetImgRank(IDs.RANK_B);
						break;
					case IDs.CURRENCY_DUST_NITROGEN:
						if (pReward.Value >= 100) SetImgRank(IDs.RANK_S);
						else if (pReward.Value >= 10) SetImgRank(IDs.RANK_A);
						else if (pReward.Value >= 1) SetImgRank(IDs.RANK_B);
						break;
					case IDs.CURRENCY_DUST_LAVA:
						if (pReward.Value >= 100) SetImgRank(IDs.RANK_S);
						else if (pReward.Value >= 10) SetImgRank(IDs.RANK_A);
						else if (pReward.Value >= 1) SetImgRank(IDs.RANK_B);
						break;
					case IDs.CURRENCY_BLUE_CHIP:
						SetImgRank(IDs.RANK_A);
						break;
					case IDs.CURRENCY_GOLDEN_CHIP:
						SetImgRank(IDs.RANK_S);
						break;
					case IDs.CURRENCY_BLUE_HERO_FRAGMENT:
						if (pReward.Value >= 20) SetImgRank(IDs.RANK_A);
						else if (pReward.Value >= 1) SetImgRank(IDs.RANK_B);
						break;
					case IDs.CURRENCY_EPIC_HERO_FRAGMENT:
						if (pReward.Value >= 30) SetImgRank(IDs.RANK_S);
						else if (pReward.Value >= 1) SetImgRank(IDs.RANK_A);
						break;
				}

				if (imgRankElement != null) imgRankElement.SetActive(false);
				SetIconSmallSize(showValue);
			}
			else if (pReward.Type == IDs.REWARD_TYPE_EXP_USER)
			{
				//no element
				if (imgElement != null) imgElement.SetActive(false);
				//no fragment
				if (imgFragment != null) imgFragment.SetActive(false);
				//no rank
				// imgRank.SetActive(false);
				SetImgRank(IDs.RANK_A);
				if (imgRankElement != null) imgRankElement.SetActive(false);
				//nếu tặng tiền hay exp thì hiện theo size ảnh
				SetIconSmallSize(showValue);
			}
			else if (pReward.Type == IDs.REWARD_TYPE_VIP)
			{
				//no element
				if (imgElement != null) imgElement.SetActive(false);
				//no fragment
				if (imgFragment != null) imgFragment.SetActive(false);
				//no rank
				// imgRank.SetActive(false);
				SetImgRank(IDs.RANK_SS);
				if (imgRankElement != null) imgRankElement.SetActive(false);
				//nếu tặng tiền hay exp thì hiện theo size ảnh
				SetIconMediumSize();
			}
			else if (pReward.Type == IDs.REWARD_TYPE_TRAP)
			{
				//no element
				if (imgElement != null) imgElement.SetActive(false);
				//no fragment
				if (imgFragment != null) imgFragment.SetActive(false);
				//rank cái hồi máu và gọi bomb thì rank SS, còn các trap khác rank A
				if (pReward.Id == IDs.ITEM_TRAP_FIRST_AIR_KIT
					|| pReward.Id == IDs.ITEM_TRAP_CALL)
				{
					SetImgRank(IDs.RANK_S);
				}
				else
				{
					SetImgRank(IDs.RANK_A);
				}
				if (imgRankElement != null) imgRankElement.SetActive(false);
				//nếu tặng tiền hay exp thì hiện theo size ảnh
				SetIconMediumSize();
			}
			else if (pReward.Type == IDs.REWARD_TYPE_UNLOCK_CHARACTER)
			{
				var heroDefinition = GameData.Instance.HeroesGroup.GetHeroDefinition(pReward.Id);
				//element
				if (imgElement != null)
				{
					imgElement.SetActive(true);

					imgElement.sprite = heroDefinition.GetElementIcon();

				}
				//no fragment
				if (imgFragment != null) imgFragment.SetActive(false);
				//rank

				SetImgRank(heroDefinition.rank);
				SetRankElement(heroDefinition.rank);

				//icon theo size bg
				SetIconBigSize();
			}
			else if (pReward.Type == IDs.REWARD_TYPE_FRAGMENT)
			{
				var heroDefinition = GameData.Instance.HeroesGroup.GetHeroDefinition(pReward.Id);
				//element
				if (imgElement != null)
				{
					imgElement.SetActive(true);
					imgElement.sprite = heroDefinition.GetElementIcon();

				}

				//no fragment
				if (imgFragment != null) imgFragment.SetActive(true);
				//rank
				SetImgRank(heroDefinition.rank);
				SetRankElement(heroDefinition.rank);

				//icon theo size bg
				SetIconBigSize();
			}
			else if (pReward.Type == IDs.REWARD_TYPE_GEAR)
			{
				var gearDefinition = GameData.Instance.GearsGroup.GetGearDefinition(pReward.Id);
				//no element
				if (imgElement != null) imgElement.SetActive(false);
				//no fragment
				if (imgFragment != null) imgFragment.SetActive(false);
				//rank
				SetImgRank(gearDefinition.rank);
				if (imgRankElement != null) imgRankElement.SetActive(false);
				//icon theo size bg
				SetIconBigSize();
				//value
			}
			else
			{
				//no element
				if (imgElement != null) imgElement.SetActive(false);
				//no fragment
				if (imgFragment != null) imgFragment.SetActive(false);
				//no rank
				SetImgRank(IDs.RANK_A);
				if (imgRankElement != null) imgRankElement.SetActive(false);
				//icon theo size bg
				SetIconBigSize();
				//value
			}

			//value
			if (showValue)
			{
				if (pReward.Type == IDs.REWARD_TYPE_GEAR
					|| pReward.Type == IDs.REWARD_TYPE_UNLOCK_CHARACTER)
				{
					//value
					if (pReward.Value <= 1) txtQuantity.text = "";
					else txtQuantity.text = "" + pReward.Value;
				}
				else
				{
					//value tween
					if (tweenValue)
					{
						float valueFX = 0f;
						int totalValue = pReward.Value;
						DOTween.To(tweenVal => valueFX = tweenVal, 0f, totalValue, 1f)
							.OnUpdate(() => { txtQuantity.text = "" + valueFX.ToString("0"); })
							.OnComplete(() => { ShowQuantity(totalValue); }).SetUpdate(true);
					}
					else
					{
						ShowQuantity(pReward.Value);
					}
				}
			}
			else
			{
				txtQuantity.text = "";
			}
		}

		private void SetImgRank(int rank)
		{
			if (imgRank == null) return;

			imgRank.sprite = AssetsCollection.instance.GetRankIcon(rank);
			imgRank.SetActive(true);

			imgRankBG.color = colorRanks[rank - 1];
		}
		private void SetOpenEffect(int rank)
		{
			GameObject _temp = AssetsCollection.instance.GetHeroCardEffectOpenByRank(rank);
			if (_temp != null)
			{
				if (_Effect != null)
					SimplePool.Despawn(_Effect);
				_Effect = SimplePool.Spawn(_temp, transform.position, _temp.transform.rotation);
				_Effect.transform.SetParent(this.transform, false);
				_Effect.transform.localPosition = Vector3.zero;
			}
		}
		private void SetLoopEffect(int rank)
		{
			GameObject _temp = AssetsCollection.instance.GetHeroCardEffectLoopByRank(rank);
			if (_temp != null)
			{
				if (_Effect != null)
					SimplePool.Despawn(_Effect);
				_Effect = SimplePool.Spawn(_temp, transform.position, _temp.transform.rotation);
				_Effect.transform.SetParent(this.transform, false);
				_Effect.transform.localPosition = Vector3.zero;
			}
		}
		private void SetRankElement(int rank)
		{
			//những chỗ như smart dissamble, thì ko cần tính toán
			if (imgBg == null || imgRankElement == null) return;

			float alpha = imgBg.rectTransform.sizeDelta.y / 200f;

			imgRankElement.SetActive(true);
			imgRankElement.sprite = AssetsCollection.instance.GetRankElementIcon(rank);
			imgRankElement.rectTransform.sizeDelta = new Vector2(69f * alpha, 69f * alpha);
			imgRankElement.rectTransform.localPosition = new Vector3(-76f * alpha, 76f * alpha, 0f);
		}

		private void ShowQuantity(int value)
		{
			if (formatKKK) txtQuantity.text = BigNumberAlpha.Create(value).GetKKKString();
			else txtQuantity.text = "" + value;
		}

		private void SetIconBigSize()
		{
			//những chỗ như smart dissamble, thì ko cần tính toán
			if (imgBg == null) return;

			imgIcon.rectTransform.sizeDelta = imgBg.rectTransform.sizeDelta - new Vector2(20f, 20f);
			imgIcon.preserveAspect = true;
			imgIcon.rectTransform.localPosition = Vector3.zero;
		}

		private void SetIconMediumSize()
		{
			//những chỗ như smart dissamble, thì ko cần tính toán
			if (imgBg == null) return;

			float alpha = imgBg.rectTransform.sizeDelta.y / 200f;

			imgIcon.rectTransform.sizeDelta = new Vector2(140f * alpha, 140f * alpha);
			imgIcon.preserveAspect = true;
			imgIcon.rectTransform.localPosition = new Vector3(0f, 16f * alpha, 0f);
		}

		private void SetIconSmallSize(bool showValue = true)
		{
			//những chỗ như smart dissamble, thì ko cần tính toán
			if (imgBg == null) return;

			float alpha = imgBg.rectTransform.sizeDelta.y / 200f;

			// imgIcon.SetNativeSize();
			if (showValue)
			{
				imgIcon.rectTransform.sizeDelta = new Vector2(90f * alpha, 90f * alpha);
				imgIcon.preserveAspect = true;
				imgIcon.rectTransform.localPosition = new Vector3(0f, 16f * alpha, 0f);
			}
			else
			{
				SetIconBigSize();
			}
		}


	}
}