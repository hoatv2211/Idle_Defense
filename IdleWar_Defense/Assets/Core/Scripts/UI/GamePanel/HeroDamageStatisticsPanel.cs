using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using FoodZombie;
using FoodZombie.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;

public class HeroDamageStatisticsPanel : MyGamesBasePanel
{
	public GameObject[] heroDamageGroup;
	public HeroView[] heroViews;
	public TextMeshProUGUI[] heroDamageText;
	public Image[] heroDamageSlider;
	public void Init(Dictionary<int, double> heroDamageDict)
	{
		if (heroDamageDict == null) return;
		var length = heroDamageDict.Count;
		var listId = heroDamageDict.Keys.ToList();
		long max = 0;
		foreach (var keyValuePair in heroDamageDict)
		{
			if (keyValuePair.Value > max) max = (long)keyValuePair.Value;
		}
		for (var i = 0; i < heroDamageGroup.Length; i++)
		{
			heroDamageGroup[i].gameObject.SetActive(i < length);
		}
		for (var i = 0; i < length; i++)
		{
			var heroData = GameData.Instance.HeroesGroup.GetHeroData(listId[i]);
			heroViews[i].Init(heroData, this);
			heroViews[i].ActiveArrow(false);
			var value = (long)heroDamageDict[listId[i]];
			heroDamageText[i].text = BigNumberAlpha.Create(value).GetKKKString();
			heroDamageSlider[i].fillAmount = 0f;
			heroDamageSlider[i].fillAmount = value * 1f / (max * 1f);
			// DOTween.To(()=>heroDamageSlider[i].fillAmount,x=> heroDamageSlider[i].fillAmount = x,
			//    value*1f/ (max*1f), 0.5f);
		}
	}
}
