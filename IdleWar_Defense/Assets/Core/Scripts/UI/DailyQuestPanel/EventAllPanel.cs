using FoodZombie.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EventAllPanel : MyGamesBasePanel
{
	[SerializeField]
	private Button btn_servenDay, btn_dailyBonus, btn_FreeGift;
	[SerializeField]
	private SevenDaysBonusPanel sevenDaysPanel;
	[SerializeField]
	private DailyLoginPanel dailyLoginPanel;
	[SerializeField]
	private DailyGiftPanel dailygiftPanel;
	[SerializeField]
	private GameObject sevenDayNotif, dailyLoginNotif, dailyGiftNotif;

	public Sprite Sprite_Select, Sprite_UnSelect;
	public void SetNotif(bool sevenDay, bool dailyLog, bool dailyGift)
	{
		sevenDayNotif.SetActive(sevenDay);
		dailyLoginNotif.SetActive(dailyLog);
		dailyGiftNotif.SetActive(dailyGift);
	}
	// Start is called before the first frame update
	void Start()
	{
		//btn_servenDay.onClick.AddListener();
		btn_servenDay.onClick.AddListener(() =>
		{
			//	Debug.Log("btn_7day press " + show);
			//if (show)
			{
				ResetButtons();
				btn_servenDay.image.sprite = Sprite_Select;
				dailyLoginPanel.Hide();
				dailygiftPanel.Hide();
				sevenDaysPanel.Show();
			}
		});
		btn_dailyBonus.onClick.AddListener(() =>
		{
			//Debug.Log("btn_dailyBonus press " + show);
			//if (show)
			{
				ResetButtons();
				btn_dailyBonus.image.sprite = Sprite_Select;
				sevenDaysPanel.Hide();
				dailygiftPanel.Hide();
				dailyLoginPanel.Show();
			}
		});
		btn_FreeGift.onClick.AddListener(() =>
		{
			//	Debug.Log("btn_FreeGift press " + show);
			//	if (show)
			{
				ResetButtons();
				btn_FreeGift.image.sprite = Sprite_Select;
				sevenDaysPanel.Hide();
				dailyLoginPanel.Hide();
				dailygiftPanel.Show();
			}
		});
		//btn_servenDay.Select();
		//btn_servenDay.group.NotifyToggleOn(btn_servenDay);
		ResetButtons();
		btn_servenDay.image.sprite = Sprite_Select;
		dailyLoginPanel.Hide();
		dailygiftPanel.Hide();
		//sevenDaysPanel.Show();
		sevenDaysPanel.Show();
	}
	private void ResetButtons()
	{
		btn_dailyBonus.image.sprite = Sprite_UnSelect;
		btn_FreeGift.image.sprite = Sprite_UnSelect;
		btn_servenDay.image.sprite = Sprite_UnSelect;
	}
	public void Init()
	{
		sevenDaysPanel.Init(this);
		dailyLoginPanel.Init(this);
		dailygiftPanel.Init(this);
	}
	//internal override void Show(UnityAction pOnDidShow = null)
	//{
	//    base.Show(pOnDidShow);
	//}
}
