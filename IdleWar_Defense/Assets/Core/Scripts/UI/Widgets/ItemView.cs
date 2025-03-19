using System;
using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;

public class ItemView : MonoBehaviour
{
	public Image imgBg;
	public Image imgIcon;
	public Image imgRank;
	public Image imgRankBG;
	public Color[] colorRanks;
	public TextMeshProUGUI txtStockNumber;
	public SimpleTMPButton btnChoice;
	public GameObject imgNoti;

	public TrapData trapData;
	public int currencyId = -1;
	private UnityAction<ItemView> choiceAction;

	private void Start()
	{
		if (btnChoice != null) btnChoice.onClick.AddListener(BtnChoice_Pressed);
	}

	public void Init(TrapData _trapData, UnityAction<ItemView> _choiceAction = null, bool showValue = true)
	{
		trapData = _trapData;
		currencyId = -1;
		choiceAction = _choiceAction;

		imgIcon.sprite = trapData.GetIcon();
		SetIconMediumSize(showValue);

		if (imgNoti != null) imgNoti.SetActive(false);

		if (trapData.Id == IDs.ITEM_TRAP_FIRST_AIR_KIT
			|| trapData.Id == IDs.ITEM_TRAP_CALL)
		{
			SetImgRank(IDs.RANK_S);
		}
		else
		{
			SetImgRank(IDs.RANK_A);
		}

		if (txtStockNumber != null)
		{
			try
			{
				txtStockNumber.text = trapData.StockNumber + "";
			}
			catch (Exception ex)
			{

				txtStockNumber.text = "";
			}

		}
	}

	public void Init(int _currencyId, UnityAction<ItemView> _choiceAction = null, bool showValue = true, bool hideZero = false)
	{
		trapData = null;
		currencyId = _currencyId;
		choiceAction = _choiceAction;

		var value = GameData.Instance.CurrenciesGroup.GetValue(currencyId);
		if (hideZero)
			if (value > 0)
			{
				gameObject.SetActive(true);
			}
			else
			{
				gameObject.SetActive(false);
				return;
			}

		imgIcon.sprite = AssetsCollection.instance.GetCurrencyIcon(currencyId);
		//tiền hay exp thì hiện theo size ảnh
		SetIconSmallSize(showValue);

		if (imgNoti != null) imgNoti.SetActive(false);

		switch (currencyId)
		{
			case IDs.CURRENCY_POWER_FRAGMENT:
				if (value >= 1) SetImgRank(IDs.RANK_B);
				break;
			case IDs.CURRENCY_POWER_CRYSTAL:
				if (value >= 3) SetImgRank(IDs.RANK_S);
				else if (value >= 1) SetImgRank(IDs.RANK_A);
				break;
			case IDs.CURRENCY_DEVINE_CRYSTAL:
				if (value >= 1) SetImgRank(IDs.RANK_SS);
				break;
			case IDs.CURRENCY_TICKET:
				if (value >= 1) SetImgRank(IDs.RANK_A);
				break;
			case IDs.CURRENCY_TICKET_PVP:
			case IDs.CURRENCY_HONOR:
				if (value >= 1) SetImgRank(IDs.RANK_A);
				break;
			case IDs.CURRENCY_MATERIAL:
				if (value >= 1000) SetImgRank(IDs.RANK_S);
				else if (value >= 100) SetImgRank(IDs.RANK_A);
				else if (value >= 1) SetImgRank(IDs.RANK_B);
				break;
			case IDs.CURRENCY_EXP_HERO:
				if (value >= 50000) SetImgRank(IDs.RANK_A);
				else if (value >= 1) SetImgRank(IDs.RANK_B);
				break;
			case IDs.CURRENCY_GEM:
				if (value >= 1000) SetImgRank(IDs.RANK_SS);
				else if (value >= 200) SetImgRank(IDs.RANK_S);
				else if (value >= 1) SetImgRank(IDs.RANK_A);
				break;
			case IDs.CURRENCY_COIN:
				if (value >= 100000) SetImgRank(IDs.RANK_A);
				else if (value >= 1) SetImgRank(IDs.RANK_B);
				break;
			case IDs.CURRENCY_DUST_ELECTRIC:
				if (value >= 100) SetImgRank(IDs.RANK_S);
				else if (value >= 10) SetImgRank(IDs.RANK_A);
				else if (value >= 1) SetImgRank(IDs.RANK_B);
				break;
			case IDs.CURRENCY_DUST_METALIC:
				if (value >= 100) SetImgRank(IDs.RANK_S);
				else if (value >= 10) SetImgRank(IDs.RANK_A);
				else if (value >= 1) SetImgRank(IDs.RANK_B);
				break;
			case IDs.CURRENCY_DUST_NITROGEN:
				if (value >= 100) SetImgRank(IDs.RANK_S);
				else if (value >= 10) SetImgRank(IDs.RANK_A);
				else if (value >= 1) SetImgRank(IDs.RANK_B);
				break;
			case IDs.CURRENCY_DUST_LAVA:
				if (value >= 100) SetImgRank(IDs.RANK_S);
				else if (value >= 10) SetImgRank(IDs.RANK_A);
				else if (value >= 1) SetImgRank(IDs.RANK_B);
				break;
			case IDs.CURRENCY_BLUE_CHIP:
				SetImgRank(IDs.RANK_A);
				break;
			case IDs.CURRENCY_GOLDEN_CHIP:
				SetImgRank(IDs.RANK_S);
				break;
			case IDs.CURRENCY_BLUE_HERO_FRAGMENT:
				if (value >= Constants.COST_BLUE_HERO_FRAGMENT)
				{
					SetImgRank(IDs.RANK_A);
					if (imgNoti != null) imgNoti.SetActive(true);
				}
				else if (value >= 1) SetImgRank(IDs.RANK_B);
				break;
			case IDs.CURRENCY_EPIC_HERO_FRAGMENT:
				if (value >= Constants.COST_EPIC_HERO_FRAGMENT)
				{
					SetImgRank(IDs.RANK_S);
					if (imgNoti != null) imgNoti.SetActive(true);
				}
				else if (value >= 1) SetImgRank(IDs.RANK_A);
				break;
		}

		if (txtStockNumber != null) txtStockNumber.text = BigNumberAlpha.Create(value).GetKKKString();
	}

	public void SetImgRank(int rank)
	{
		if (imgRank == null) return;

		imgRank.sprite = AssetsCollection.instance.GetRankIcon(rank);
		imgRank.SetActive(true);

		imgRankBG.color = colorRanks[rank - 1];
	}

	public void SetIconBigSize()
	{
		//những chỗ như smart dissamble, thì ko cần tính toán
		if (imgBg == null) return;

		imgIcon.rectTransform.sizeDelta = imgBg.rectTransform.sizeDelta - new Vector2(20f, 20f);
		imgIcon.preserveAspect = true;
		imgIcon.rectTransform.localPosition = Vector3.zero;
	}

	private void SetIconMediumSize(bool showValue = true)
	{
		//những chỗ như smart dissamble, thì ko cần tính toán
		if (imgBg == null) return;

		float alpha = imgBg.rectTransform.sizeDelta.y / 200f;

		if (showValue)
		{
			imgIcon.rectTransform.sizeDelta = new Vector2(140f * alpha, 140f * alpha);
			imgIcon.preserveAspect = true;
			imgIcon.rectTransform.localPosition = new Vector3(0f, 16f * alpha, 0f);
		}
		else
		{
			SetIconBigSize();
		}
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

	public void BtnChoice_Pressed()
	{
		if (choiceAction != null) choiceAction(this);
	}
}