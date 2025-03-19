using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Utilities.Common;
using Utilities.Components;

namespace FoodZombie.UI
{
	public class ItemDetailPanel : MyGamesBasePanel
	{
		public ItemView itemView;
		public TextMeshProUGUI txtName;
		public TextMeshProUGUI txtShortInfo;
		public TextMeshProUGUI txtInfo;
		public SimpleTMPButton btnUse;

		public TrapData trapData;
		private int currencyId = -1;
		private UnityAction refreshAction;

		private void Start()
		{
			btnUse.onClick.AddListener(BtnUse_Pressed);
		}
		public bool InitPlayerEXP(RewardInfo rewardInfor, UnityAction _refreshAction)
		{
			if (rewardInfor.Type == IDs.REWARD_TYPE_EXP_USER)
			{
				refreshAction = _refreshAction;
				itemView.SetImgRank(IDs.RANK_A);
				itemView.SetIconBigSize();
				txtShortInfo.text = "";
				txtName.text = "Exp Player";
				txtInfo.text = "Use to upgrade Player's Level";
				itemView.imgIcon.sprite = rewardInfor.GetIcon();
				//   itemView.txtStockNumber.text = rewardInfor.Value + "";
				itemView.txtStockNumber.text = "";
				btnUse.SetActive(false);
				return true;
			}
			if (rewardInfor.Type == IDs.REWARD_TYPE_FRAGMENT)
			{
				refreshAction = _refreshAction;
				itemView.SetImgRank(IDs.RANK_A);
				itemView.SetIconBigSize();
				txtShortInfo.text = "";
				txtName.text = "Fragment";
				txtInfo.text = "Use to summon Hero";
				itemView.imgIcon.sprite = rewardInfor.GetIcon();
				// itemView.txtStockNumber.text = rewardInfor.Value + "";
				itemView.txtStockNumber.text = "";
				btnUse.SetActive(false);
				return true;
			}
			if (rewardInfor.Type == IDs.REWARD_TYPE_VIP)
			{
				refreshAction = _refreshAction;
				itemView.SetImgRank(IDs.RANK_SS);
				itemView.SetIconBigSize();
				txtShortInfo.text = "";
				txtName.text = "Exp VIP";
				txtInfo.text = "Use to upgrade Player's VIP Level";
				itemView.imgIcon.sprite = rewardInfor.GetIcon();
				// itemView.txtStockNumber.text = rewardInfor.Value + "";
				itemView.txtStockNumber.text = "";
				btnUse.SetActive(false);
				return true;
			}
			return false;
		}
		public bool Init(TrapData TrapData, int currencyId, UnityAction _refreshAction)
		{
			this.trapData = TrapData;
			this.currencyId = currencyId;
			refreshAction = _refreshAction;

			if (trapData != null)
			{
				itemView.Init(trapData, null, false);
				if (txtName != null)
				{
					try
					{
						//txtName.text = trapData.Name;
						txtName.text = trapData.NameLocal;
					}
					catch (Exception ex)
					{
						Debug.LogError(ex.ToString());
						txtName.text = "";
						Config.LogEvent(TrackingConstants.ERROR_CATCHING, "trap", JsonUtility.ToJson(trapData));
					}

				}
				try
				{
					txtShortInfo.text = "Level: " + trapData.Level;
				}
				catch (Exception ex)
				{

					txtShortInfo.text = "";
					Config.LogEvent(TrackingConstants.ERROR_CATCHING, "trap", JsonUtility.ToJson(trapData));

				}
				try
				{
					txtInfo.text = trapData.Description;
					
				}
				catch (Exception ex)
				{
					Config.LogEvent(TrackingConstants.ERROR_CATCHING, "trap", JsonUtility.ToJson(trapData));
					txtInfo.text = "";
				}

				itemView.txtStockNumber.text = "";
				btnUse.SetActive(false);
				return true;
			}
			else
			{
				itemView.Init(currencyId, null, false);
				// txtShortInfo.text = "Amount:";
				itemView.txtStockNumber.text = "";
				txtShortInfo.text = "";
				txtName.text = Localization.Get("CURRENCY_NAME_" + currencyId);
				txtInfo.text = Localization.Get("CURRENCY_DESC_" + currencyId);
				switch (currencyId)
				{
					case IDs.CURRENCY_POWER_FRAGMENT:
						btnUse.SetActive(false);

						//txtName.text = "Power Fragment";
						//txtInfo.text = "Can be use to summon hero rank C to S with low rate";
						return true;
						break;
					case IDs.CURRENCY_POWER_CRYSTAL:
						btnUse.SetActive(false);

						//txtName.text = "Power Crystal";
						//txtInfo.text = "Can be use to summon hero rank B to S with high rate";
						return true;
						break;
					case IDs.CURRENCY_DEVINE_CRYSTAL:
						btnUse.SetActive(false);

						//txtName.text = "Devine Crystal";
						//txtInfo.text = "Can be use to summon hero rank A to S with very high rate";
						return true;
						break;
					case IDs.CURRENCY_TICKET:
						btnUse.SetActive(false);

						//txtName.text = "Ticket";
						//txtInfo.text = "Use to Fast collect";
						return true;

					case IDs.CURRENCY_TICKET_PVP:
						btnUse.SetActive(false);

						//txtName.text = "Ticket";
						//txtInfo.text = "Use to PvP";
						return true;

					case IDs.CURRENCY_HONOR:
						btnUse.SetActive(false);

						//txtName.text = "Honor";
						//txtInfo.text = "Use to shop PvP";
						return true;

					case IDs.CURRENCY_MATERIAL:
						btnUse.SetActive(false);

						//txtName.text = "Scrap";
						//txtInfo.text = "Use to upgrade Gear";
						return true;

					case IDs.CURRENCY_EXP_HERO:
						btnUse.SetActive(false);

						//txtName.text = "Exp Hero";
						//txtInfo.text = "Use to upgrade level Hero";
						return true;
						break;
					case IDs.CURRENCY_GEM:
						btnUse.SetActive(false);

						//txtName.text = "Gem";
						//txtInfo.text = "";
						//itemView.txtStockNumber.text = rewa
						// itemView.txtStockNumber.text = GameData.Instance.CurrenciesGroup.GetValue(IDs.CURRENCY_GEM).ToString();
						return true;
						break;
					case IDs.CURRENCY_COIN:
						btnUse.SetActive(false);

						//txtName.text = "Coin";
						//txtInfo.text = "";
						//itemView.txtStockNumber.text = GameData.Instance.CurrenciesGroup.GetValue(IDs.CURRENCY_COIN).ToString();
						return true;
						break;
					case IDs.CURRENCY_DUST_ELECTRIC:
						btnUse.SetActive(false);

						//txtName.text = "Electric Core";
						//txtInfo.text = "Use to upgrade star of Electric Hero";
						return true;
						break;
					case IDs.CURRENCY_DUST_METALIC:
						btnUse.SetActive(false);

						//txtName.text = "Metalic Core";
						//txtInfo.text = "Use to upgrade star of Metalic Hero";
						return true;
						break;
					case IDs.CURRENCY_DUST_NITROGEN:
						btnUse.SetActive(false);

						//txtName.text = "Nitrogen Core";
						//txtInfo.text = "Use to upgrade star of Nitrogen Hero";
						return true;
						break;
					case IDs.CURRENCY_DUST_LAVA:
						btnUse.SetActive(false);

						//txtName.text = "Lava Core";
						//txtInfo.text = "Use to upgrade star of Lava Hero";
						return true;
						break;
					case IDs.CURRENCY_BLUE_CHIP:
						btnUse.SetActive(false);

						//txtName.text = "Blue Chip";
						//txtInfo.text = "Use to spin General Lucky Wheel";
						return true;
						break;
					case IDs.CURRENCY_GOLDEN_CHIP:
						btnUse.SetActive(false);

						//txtName.text = "Golden Chip";
						//txtInfo.text = "Use to spin Royale Lucky Wheel";
						return true;
						break;
					case IDs.CURRENCY_BLUE_HERO_FRAGMENT:
						btnUse.SetActive(true);

						//txtName.text = "Blue Hero Fragment";
						//txtInfo.text = "Collect " + Constants.COST_BLUE_HERO_FRAGMENT + " part to summon random C to S hero with low rate";
						txtInfo.text = string.Format(Localization.Get("CURRENCY_DESC_" + currencyId), Constants.COST_BLUE_HERO_FRAGMENT);

						var cost = Constants.COST_BLUE_HERO_FRAGMENT;
						// itemView.txtStockNumber.text += "/" + cost;
						var value = GameData.Instance.CurrenciesGroup.GetValue(currencyId);
						if (value >= cost) btnUse.SetEnable(true);
						else btnUse.SetEnable(false);
						return true;
						break;
					case IDs.CURRENCY_EPIC_HERO_FRAGMENT:
						btnUse.SetActive(true);

						//txtName.text = "Epic Hero Fragment";
						//txtInfo.text = "Collect " + Constants.COST_EPIC_HERO_FRAGMENT + " part to summon random C to S hero with high rate";
						txtInfo.text = string.Format(Localization.Get("CURRENCY_DESC_" + currencyId), Constants.COST_EPIC_HERO_FRAGMENT);

						cost = Constants.COST_EPIC_HERO_FRAGMENT;
						//   itemView.txtStockNumber.text += "/" + cost;
						value = GameData.Instance.CurrenciesGroup.GetValue(currencyId);
						if (value >= cost) btnUse.SetEnable(true);
						else btnUse.SetEnable(false);
						return true;
						break;
				}
			}
			return false;
		}
		public void Init(ItemView _itemView, UnityAction _refreshAction)
		{
			trapData = _itemView.trapData;
			currencyId = _itemView.currencyId;
			Init(trapData, currencyId, _refreshAction);
		}

		private void BtnUse_Pressed()
		{
			Back();

			if (currencyId == IDs.CURRENCY_BLUE_HERO_FRAGMENT)
			{
				var cost = Constants.COST_BLUE_HERO_FRAGMENT;
				var currenciesGroup = GameData.Instance.CurrenciesGroup;
				if (!currenciesGroup.CanPay(IDs.CURRENCY_BLUE_HERO_FRAGMENT, cost))
				{
					MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_18));
					//MainPanel.instance.ShowWarningPopup("Not enough blue hero fragments");
					return;
				}

				currenciesGroup.Pay(IDs.CURRENCY_BLUE_HERO_FRAGMENT, cost);
				var rewardInfo = GameData.Instance.HeroesGroup.SummonByBlueHeroFragment();
				LogicAPI.ClaimReward(rewardInfo);
			}
			else if (currencyId == IDs.CURRENCY_EPIC_HERO_FRAGMENT)
			{
				var cost = Constants.COST_EPIC_HERO_FRAGMENT;
				var currenciesGroup = GameData.Instance.CurrenciesGroup;
				if (!currenciesGroup.CanPay(IDs.CURRENCY_EPIC_HERO_FRAGMENT, cost))
				{
					MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_19));
					//MainPanel.instance.ShowWarningPopup("Not enough epic hero fragments");
					return;
				}

				currenciesGroup.Pay(IDs.CURRENCY_EPIC_HERO_FRAGMENT, cost);
				var rewardInfo = GameData.Instance.HeroesGroup.SummonByEpicHeroFragment();
				LogicAPI.ClaimReward(rewardInfo);
			}

			if (refreshAction != null) refreshAction();
		}
	}
}
