using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Inspector;
using Utilities.Components;
using System;
using Spine.Unity;
using Utilities.Common;
using UnityEngine.SceneManagement;

namespace FoodZombie.UI
{
	public class PausePanel : MyGamesBasePanel
	{
		#region Members

		public GameObject mainPanel;
		public SimpleTMPButton btnQuit;
		public SimpleTMPButton btnRestart;

		public GameObject confirmQuitPanel;
		public TextMeshProUGUI Txt_Quit;
		public SimpleTMPButton btnOkQuit;
		public SimpleTMPButton btnCancelQuit;

		public GameObject confirmRestartPanel;
		public SimpleTMPButton btnOkRestart;
		public SimpleTMPButton btnCancelRestart;

		#endregion

		//=============================================

		#region MonoBehaviour

		private void Start()
		{
			btnQuit.onClick.AddListener(BtnQuit_Pressed);
			btnRestart.onClick.AddListener(BtnRestart_Pressed);

			btnOkQuit.onClick.AddListener(BtnOkQuit_Pressed);
			btnCancelQuit.onClick.AddListener(BtnCancelQuit_Pressed);

			btnOkRestart.onClick.AddListener(BtnOkRestart_Pressed);
			btnCancelRestart.onClick.AddListener(BtnCancelRestart_Pressed);
		}

		private void OnEnable()
		{
			if (Config.typeModeInGame != Config.TYPE_MODE_PvP)
			{
				GameplayController.Instance.PauseGame();
			}
			else
			{
				btnRestart.gameObject.SetActive(false);
			}
		}

		#endregion

		//=============================================

		#region Public

		#endregion

		//==============================================

		#region Private

		internal override void Back()
		{
			GameplayController.Instance.ResumeGame();
			base.Back();
		}

		private void BtnQuit_Pressed()
		{
			mainPanel.SetActive(false);
			if (Config.typeModeInGame == Config.TYPE_MODE_PvP)
				Txt_Quit.text = "Quit and lose this match?";
			confirmQuitPanel.SetActive(true);
		}

		private void BtnRestart_Pressed()
		{
			mainPanel.SetActive(false);
			confirmRestartPanel.SetActive(true);
		}

		private void BtnOkQuit_Pressed()
		{
			if (Config.typeModeInGame == Config.TYPE_MODE_PvP)
			{
				//GameplayController.Instance.PauseGame();
				GameplayController.Instance.CallShowLose(0);
				Config.LogEvent(TrackingConstants.CLICK_PVP_MATCH_END_FROMPAUSE);
			}
			else
				GameplayController.Instance.BackToHome();
		}

		private void BtnCancelQuit_Pressed()
		{
			confirmQuitPanel.SetActive(false);
			mainPanel.SetActive(true);
		}

		private void BtnOkRestart_Pressed()
		{
			GameplayController.Instance.RestartGame();
		}

		private void BtnCancelRestart_Pressed()
		{
			confirmRestartPanel.SetActive(false);
			mainPanel.SetActive(true);
		}

		#endregion
	}
}