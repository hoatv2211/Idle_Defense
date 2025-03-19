using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using EnhancedUI.EnhancedScroller;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;
using Utilities.Service.RFirebase;
using UnityEngine.SceneManagement;
namespace FoodZombie.UI
{
	public class PvPMainPanel : IHeroFormationPanel
	{
		[SerializeField] private Text TxT_SsInfor, Txt_Time;
		[Header("Ticket")]
		[SerializeField] private TextMeshProUGUI txtTicket;
		[SerializeField] private SimpleTMPButton btnAddTicket;

		//
		[SerializeField] private TextMeshProUGUI txtTicketInButton;
		[SerializeField] private GameObject imgTicketInButton;
		[SerializeField] private GameObject imgGemInButton;

		[Header("Main")]
		[SerializeField] private SimpleTMPButton btnInfoEvent;
		[SerializeField] private SimpleTMPButton btnFormChange;
		[SerializeField] private SimpleTMPButton btnPlay;
		[SerializeField] private SimpleTMPButton btnBackHome;
		[SerializeField] private ScrollViewLeadBoard mScrollViewLeadBoard;
		[SerializeField] private BuyTicketPvPPanel mBuyTicketPvPPanel;
		[SerializeField] private InfoSeasonRewardPanel mInfoSeasonRewardPanel;
		[SerializeField] private FindMatchPanel mFindMatchPanel;
		[SerializeField] private ResultFindMatchPanel mResultFindMatchPanel;
		public WaitingPanel mWaitingPanel;
		public GameObject mMainPanel;

		[Header("Formation Main UI")]
		public TextMeshProUGUI txtCP;
		public HeroView[] currentFormation;
		public SimpleButton[] btnSlots;
		public Text[] btnSlotsTextes;
		[HideInInspector] public int choiceFormation = 0;

		public Action OnShowDone;

		int RankCurrent, RankTotalPlayer;

		bool canUpdateTime = false;


		private void Awake()
		{
			//	Instance = this;
			Txt_Time.text = "";
			EventDispatcher.AddListener<PvPUpdateTime>(OnPvPTimeUpdate);
		}

		private void Update()
		{
			if (canUpdateTime)
			{
				Txt_Time.text = Config.PvpConfig.DeadlineTimeString();
				//Txt_Time.text = "End in " + (Config.PvpConfig.TimeLocalDeadline - DateTime.Now).ToString(@"hh\:mm");
			}
		}

		private void OnPvPTimeUpdate(PvPUpdateTime e)
		{
			canUpdateTime = true;
		}
		bool isInHome = false;
		public void CallStartInHome()
		{
			Init();
			isInHome = true;
			Config.LogScene(TrackingConstants.SceneName.Screen_Home_PvP);
			mScrollViewLeadBoard.CallStart();
			btnPlay.SetUpEvent(Action_BtnFight);
			btnBackHome.SetUpEvent(Action_BackHome);
			btnInfoEvent.SetUpEvent(Action_BtnInfoEvent);
			btnFormChange.SetUpEvent(Action_BtnFormChange);
			btnAddTicket.SetUpEvent(Show_BuyTicketPvPPanel);


			for (int i = 0; i < btnSlots.Length; i++)
			{
				int index = i;
				btnSlots[i].onClick.AddListener(() => BtnSlot_Click(index));
				currentFormation[i].btnChoice.onClick.AddListener(() => BtnSlot_Click(index));

			}
		}

		public void CallStartInGame()
		{
			isInHome = false;
			Config.LogScene(TrackingConstants.SceneName.Screen_Play_PvP);
			mMainPanel.SetActive(false); ;
			mFindMatchPanel.CallStart(Show_ResultFindMatchPanel);

		}
		internal override void Init()
		{
			mMainPanel.SetActive(true);
			GameData.Instance.HeroesGroup.SetCurrentFormation(1);
			heroesGroup.CheckFormation();
			ShowCurrentFormation();
			this.FormationInit();
			RefreshTicket();
			Config.PvpConfig.UpdateTime();
		}
		protected override void AfterShowing()
		{
			base.AfterShowing();

			#region PvPGetUserData
			if (MainPanel.instance)
			{
				//MainPanel.instance.ShowWaitingPanel(true, "Get data");
				mWaitingPanel.SetActive(true);
				mWaitingPanel.SetText("Get data");
				GameRESTController.Instance.APIUser_getCurrentUserInfor(OnGetUserInforDone, OnGetDataError);
				GameRESTController.Instance.APIPVPInfor_GetInfor(OnGetPvPInforDone, (e) => { Debug.LogError(e); });
			}
			#endregion
		}



		protected override void BeforeHiding()
		{
			base.BeforeHiding();
			if (OnShowDone != null)
				OnShowDone();
			if (isInHome)
				Config.LogScene(TrackingConstants.SceneName.Screen_Home_MainMenu);
			else
				Config.LogScene(TrackingConstants.SceneName.Screen_Play_PvP);
		}
		#region PvPCallback

		private void OnGetDataError(string obj)
		{
			//MainPanel.instance.ShowWaitingPanel(false);
			mWaitingPanel.SetActive(false);
			if (MainPanel.instance != null)
				MainPanel.instance.ShowWarningPopup(obj);
			if (MainGamePanel.instance != null)
				MainGamePanel.instance.ShowWarningPopup(obj);
		}
		private void OnGetPvPInforDone(HttpREsultObject obj)
		{
			try
			{
				HttpResultData result = obj.Data[0];
				Config.PvpConfig.SeasonTime = result.SeasonTime;
				Config.PvpConfig.SaveCache();
				if (UserGroup.UserData != null)
					if (UserGroup.UserData.SeasonPlaying != Config.PvpConfig.SeasonTime)
					{
						UserGroup.UserData.SeasonPlaying = Config.PvpConfig.SeasonTime;
						UserGroup.UserData.RWClear();
						UserGroup.UserData.SaveToGameSave();
					}
				TxT_SsInfor.text = "SEASON-" + result.SeasonTime;
			}
			catch (Exception exx)
			{

				Debug.LogError(exx.ToString());
			}

		}
		private void OnGetUserInforDone(HttpREsultObject obj)
		{
			//MainPanel.instance.ShowWaitingPanel(false);
			mWaitingPanel.SetActive(false);
			HttpResultData result = obj.Data[0];
			UserGroup.UserData.UpdateBaseData(result);
			UserGroup.UserData.SaveToGameSave();
			//MainPanel.instance.ShowWaitingPanel(true, "Get data");
			//mWaitingPanel.SetActive(true);
			//mWaitingPanel.SetText("Get data");
			//	GameRESTController.Instance.APIUser_GetRank(OnGetRankInforDone, OnGetDataError);
			this.mScrollViewLeadBoard.UpdateUserView();
		}

		private void OnGetRankInforDone(HttpREsultObject obj)
		{
			//MainPanel.instance.ShowWaitingPanel(false);

			mWaitingPanel.SetActive(false);
			HttpResultData result = obj.Data[0];
			RankCurrent = result.RankCurrentPlay;
			RankTotalPlayer = result.TotalPlayer;
			Debug.Log("Get Rank Data:" + RankCurrent + "/" + RankTotalPlayer);
		}
		#endregion
		public void RefreshTicket()
		{
			txtTicket.text = GameData.Instance.CurrenciesGroup.GetValue(IDs.CURRENCY_TICKET_PVP).ToString();
			txtTicketInButton.text = "x" + GameData.Instance.CurrenciesGroup.GetValue(IDs.CURRENCY_TICKET_PVP).ToString();

			bool isTicket = GameData.Instance.CurrenciesGroup.GetValue(IDs.CURRENCY_TICKET_PVP) > 0;
			imgGemInButton.SetActive(!isTicket);
			imgTicketInButton.SetActive(isTicket);
		}
		private void Action_BtnFight()
		{

			if (!CheckCanStartGame()) return;

			var currenciesGroup = GameData.Instance.CurrenciesGroup;
			if (currenciesGroup.CanPay(IDs.CURRENCY_TICKET_PVP, 1))
			{
				Config.PvpConfig.Pay_Type = IDs.CURRENCY_TICKET_PVP;
				Config.PvpConfig.Pay_Value = 1;
				//	currenciesGroup.Pay(IDs.CURRENCY_TICKET_PVP, 1);
				MainPanel.instance.StartGamePvP();
			}
			else
			{

				var gem = Constants.GEM_TO_TICKET_PVP_SHOP;
				if (!currenciesGroup.CanPay(IDs.CURRENCY_GEM, gem))
				{
					MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_2));
					//MainPanel.instance.ShowWarningPopup("Not enough gem");
					return;
				}

				Config.PvpConfig.Pay_Type = IDs.CURRENCY_GEM;
				Config.PvpConfig.Pay_Value = gem;
				//	currenciesGroup.Pay(IDs.CURRENCY_GEM, gem);
				MainPanel.instance.StartGamePvP();
			}
		}

		private void Show_ResultFindMatchPanel()
		{
			mResultFindMatchPanel.CallStart(this);
		}

		private void Action_BtnFormChange()
		{
			FormationSetupPanel.gameObject.SetActive(true);
			FormationInit();
			Config.LogEvent(TrackingConstants.CLICK_PVP_FORMATION);
		}

		private void Action_BtnInfoEvent()
		{
			mInfoSeasonRewardPanel.CallStart();
			Config.LogEvent(TrackingConstants.CLICK_PVP_INFOR);

		}

		private void Show_BuyTicketPvPPanel()
		{
			mBuyTicketPvPPanel.gameObject.SetActive(true);
			mBuyTicketPvPPanel.Init();
		}

		public override void ShowCurrentFormation()
		{
			choiceFormation = GameData.Instance.HeroesGroup.CurrentFormation;
			txtCP.text = "CP: " + GameData.Instance.HeroesGroup.GetEquippedHeroesCP();
			//Debug.LogError("Current Formation " + choiceFormation);
			HeroData[] formationData = GameData.Instance.HeroesGroup.GetEquippedHeroes();
			int count = formationData.Length;
			for (int i = 0; i < count; i++)
			{
				int _maxLevel = GameData.Instance.MissionsGroup.CurrentMissionId;
				int levelToUnlock = Constants.FORMATION_SLOTUNLOCK(i + 1);
				HeroData item = formationData[i];
				HeroView heroView = currentFormation[i];
				SimpleButton btnSlot = btnSlots[i];
				if (_maxLevel - 1 >= levelToUnlock)
				{
					//Unlocked:
					btnSlotsTextes[i].gameObject.SetActive(false);
					btnSlot.image.color = Color.white;
					if (item == null)
					{
						heroView.SetActive(false);
						btnSlot.gameObject.SetActive(true);
						btnSlot.enabled = true;
						btnSlot.transform.DOKill();
						btnSlot.transform.localScale = Vector3.one;
						//	btnSlot.image.color = Color.black;
						//btnSlot.rectTransform.DOScale(1.1f * Vector3.one, 1).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
					}
					else
					{
						heroView.SetActive(true);
						heroView.isTOPHero = !HaveOtherStrongerNotSelect(item);
						btnSlot.gameObject.SetActive(false);
						btnSlot.transform.DOKill();
						btnSlot.transform.localScale = Vector3.one;
						heroView.Init(item, this);
					}
				}
				else
				{
					//Lock
					btnSlotsTextes[i].gameObject.SetActive(true);
					btnSlot.gameObject.SetActive(true);
					btnSlot.enabled = false;
					btnSlot.transform.DOKill();
					btnSlot.transform.localScale = Vector3.one;
					btnSlot.image.color = Color.gray;
					btnSlotsTextes[i].text = string.Format("UNLOCK AT\n{0}", GameData.Instance.MissionLevelToString(levelToUnlock));
					heroView.SetActive(false);
				}
			}
		}

		private void Action_BackHome()
		{
			Back();

			if (!MainPanel.mInstance.MainMenuPanel.gameObject.activeSelf)
			{
				//Fix login
				MainPanel.instance.ShowMainMenuPanel2();
			}

		}
		public void BtnSlot_Click(int index)
		{
			currentFormationSlotIndexSelect = index;
			FormationSetupPanel.gameObject.SetActive(true);
			FormationInit();
		}
		bool CheckCanStartGame()
		{
			var formationData = GameData.Instance.HeroesGroup.GetEquippedHeroes();
			var hasHero = false;
			var count = formationData.Length;
			for (int i = 0; i < count; i++)
			{
				var item = formationData[i];
				if (item != null)
				{
					hasHero = true;
					break;
				}
			}
			if (!hasHero)
			{
				MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_6));
				//MainPanel.instance.ShowWarningPopup("You must choose a formation with some heroes to fight.");
				return false;
			}
			return true;
		}

		protected override void AfterHiding()
		{
			base.AfterHiding();
			if (MainPanel.instance != null)
				MainPanel.instance.MainMenuPanel.ShowPvpBtn();
		}

	}
}




