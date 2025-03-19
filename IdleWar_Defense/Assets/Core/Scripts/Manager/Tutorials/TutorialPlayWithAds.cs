using FoodZombie.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Components;

public class TutorialPlayWithAds : TutorialController
{
	private JustButton btnPlayWithAds;
	private bool pressedBtn;
	public TutorialPlayWithAds(int pId, bool pIsToolTips) : base(pId, pIsToolTips)
	{
	}
	public override void Start()
	{
		base.Start();
	}
	public override IEnumerator IEProcess()
	{
		yield return new WaitUntil(() => (MainGamePanel != null && MainGamePanel.HubPanel != null
													&& MainGamePanel.TopPanel is MissionDetailPanel));
		pressedBtn = false;
		btnPlayWithAds = MainGamePanel.MissionDetailPanel.btnPlayWithTrap;
		if (!btnPlayWithAds.gameObject.activeSelf) yield break;
		btnPlayWithAds.onClick.AddListener(OnBtnPlay_Pressed);
		TutorialsManager.InputMasker.FocusToTargetImmediately(btnPlayWithAds.rectTransform);
		//var s = "From now,you can watch video and get free Trap !";
		var s = Localization.Get(Localization.ID.TUTORIAL_53);
		var board = MainGamePanel.ShowNotificationBoard(btnPlayWithAds.rectTransform(), s, Alignment.Bot, new Vector2(144f, -120f));
		board.transform.SetParent(TutorialsManager.transform);
		yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
		yield return new WaitUntil(() => (pressedBtn));
		yield return null;
		End(true);
	}
	private void OnBtnPlay_Pressed()
	{
		Debug.LogError("=======>PRESS");
		pressedBtn = true;
		btnPlayWithAds.onClick.RemoveListener(OnBtnPlay_Pressed);
	}
	public override void Pause()
	{

	}

	public override void Resume()
	{

	}

	protected override void OnAnyChildHide(MyGamesBasePanel obj)
	{

	}

	protected override void OnAnyChildShow(MyGamesBasePanel obj)
	{

	}
	public override void End(bool pFinished)
	{
		base.End(pFinished);

		MainGamePanel.UITooltips.LockOptionsGrid(false);

		//Unlock input
		TutorialsManager.InputMasker.Active(false);

		//Hide notification board
		MainGamePanel.HideNotificationBoard(0);

		//Unlock panel
		// TutorialsManager.UnlockPanel(MainGamePanel.WinPanel);
		// TutorialsManager.UnlockPanel(MainGamePanel.LosePanel);
	}
}
