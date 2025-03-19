using System;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FoodZombie.UI;
using Utilities.Common;
using MyAttribute;

namespace FoodZombie
{
	public class CurrencyView : MonoBehaviour
	{
		#region Members

		[SerializeField] private Image mImgIcon;
		[SerializeField] private TextMeshProUGUI mTxtValue;
		[SerializeField] private bool formatKKK = false;
		[SerializeField] private Button mBtnShortcut;

		public bool canBuyMore = true;
		public Image ImgIcon => mImgIcon;

		public bool isAuto = false;
		[ConditionalField("isAuto", false), Tooltip("1.Coin - 2.Gem - 3.Exp")]
		public int CurrencyID = -1;

		private int mCurrencyId = -1;
		private bool mRegisteredEvents;

		public Button BtnShortcut => mBtnShortcut;
		#endregion

		//=============================================

		#region MonoBehaviour

		private void Start()
		{
			if (mBtnShortcut != null)
				mBtnShortcut.onClick.AddListener(OnBtnShortcut_Pressed);

			if (isAuto)
				Init(CurrencyID);

		}

		private void OnEnable()
		{
			RegisterEvents();
			Refresh();
		}

		private void OnDisable()
		{
			UnregisterEvents();
		}

		#endregion

		//=============================================

		#region Public

		public void Init(int pId)
		{
			mCurrencyId = pId;
			mImgIcon.sprite = AssetsCollection.instance.GetCurrencyIcon(pId);

			Refresh();
			RegisterEvents();
		}

		#endregion

		//==============================================

		#region Private

		private void Refresh()
		{
			if (formatKKK) mTxtValue.text = BigNumberAlpha.Create(GameData.Instance.CurrenciesGroup.GetValue(mCurrencyId)).GetKKKString();
			else mTxtValue.text = GameData.Instance.CurrenciesGroup.GetValue(mCurrencyId).ToString();
		}

		private void RegisterEvents()
		{
			if (mRegisteredEvents || mCurrencyId == -1)
				return;

			mRegisteredEvents = true;

			EventDispatcher.AddListener<CurrencyChangedEvent>(OnCurrencyChanged);
		}

		private void UnregisterEvents()
		{
			if (!mRegisteredEvents)
				return;

			mRegisteredEvents = false;

			EventDispatcher.RemoveListener<CurrencyChangedEvent>(OnCurrencyChanged);
		}

		private void OnCurrencyChanged(CurrencyChangedEvent e)
		{
			if (e.id == mCurrencyId)
			{
				Refresh();
			}
		}

		private void OnBtnShortcut_Pressed()
		{
			if (!canBuyMore) return;
			if (mCurrencyId == IDs.CURRENCY_COIN)
			{
				MainPanel.instance.ShowBuyCoinPanel();
				Config.LogEvent(TrackingConstants.CLICK_BUY_COIN);
			}
			else if (mCurrencyId == IDs.CURRENCY_GEM)
			{
				MainPanel.instance.ShowBuyGemPanel();
				Config.LogEvent(TrackingConstants.CLICK_BUY_GEM);
			}
			else if (mCurrencyId == IDs.CURRENCY_TICKET)
			{
				MainPanel.instance.ShowBuyTicketPanel();
				Config.LogEvent(TrackingConstants.CLICK_BUY_FAST_TICKET);
			}
			else if (mCurrencyId == IDs.CURRENCY_EXP_HERO)
			{
				MainPanel.instance.ShowBuySpecialPackPanel();
			}
			else MainPanel.instance.ShowStorePanel();
		}

		#endregion
	}
}