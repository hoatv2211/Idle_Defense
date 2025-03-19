using FoodZombie.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class WaitingPanel : MyGamesBasePanel
{
	public Text Text_infor;
	string infor;
	protected override void BeforeShowing()
	{
		base.BeforeShowing();
		Lock(true);
	}
	protected override void BeforeHiding()
	{
		Lock(false);
		base.BeforeHiding();
	}
	public void SetText(string infor)
	{
		this.infor = infor.ToUpper();
		Text_infor.DOKill();
		Text_infor.text = this.infor;
		Text_infor.DOText(this.infor + "...", 2).SetLoops(-1, LoopType.Yoyo);
	}
	public void NotShow()
	{
		if (!gameObject.activeSelf) return;
		this.Lock(false);
		//gameObject.SetActive(false);
		Back();
	}
}
