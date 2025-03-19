using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using FoodZombie;
using FoodZombie.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;
using Utilities.Pattern.UI;

public class HeroView : EnhancedScrollerCellView
{
	public Image imgBg;
	public Image imgIcon;
	public Image imgElement;
	public Image imgRank;
	public Image imgRankBG;
	public Color[] colorRanks;
	public Image imgRankElement;
	public Image imgLevel;
	public TextMeshProUGUI txtLevel;
	public GameObject[] imgStars;
	public Image imgEquiped;
	public SimpleTMPButton btnChoice;

	public GameObject imgHighlight;
	public Image objectUp;

	public HeroData heroData;
	private UnityAction<HeroView> choiceAction;

	private UnityAction<HeroView, bool> addAction;
	private bool add = false;

	public GameObject imgHeroNoti;

	private PanelController panel = null;
	private bool mRegisteredEvents;

	private void Start()
	{
		if (btnChoice != null) btnChoice.onClick.AddListener(BtnChoice_Pressed);
	}

	private void OnEnable()
	{
		RegisterEvents();
	}

	private void OnDisable()
	{
		UnregisterEvents();
	}

	public void Refresh()
	{
		try
		{
			if (heroData == null)
			{
				imgIcon?.SetActive(false);
				imgElement?.SetActive(false);
				imgRank?.SetActive(false);
				imgRankElement?.SetActive(false);
				imgLevel?.SetActive(false);

				var count = imgStars.Length;
				for (int i = 0; i < count; i++)
				{
					imgStars[i].SetActive(false);
				}

				imgEquiped?.SetActive(false);
			}
			else
			{
				imgIcon?.SetActive(true);
				if (imgIcon != null)
					imgIcon.sprite = heroData.GetIcon();
				if (imgElement != null)
					imgElement.sprite = heroData.GetElementIcon();
				SetImgRank(heroData.Rank);
				SetRankElement(heroData.Rank);
				if (txtLevel != null)
					txtLevel.text = heroData.levelUnlocked + "";
				var star = heroData.star;
				var count = imgStars.Length;
				for (int i = 0; i < star; i++)
				{
					imgStars[i].SetActive(true);
				}

				for (int i = star; i < count; i++)
				{
					imgStars[i].SetActive(false);
				}

				SetEquipedSize(heroData.IsEquipped());
				SetLevelSize();
			}

			ShowHeroBtn();
		}
		catch (System.Exception ex)
		{

			Config.LogEvent(TrackingConstants.ERROR_CATCHING, "Heroview", ex.ToString());
		}

	}

	//delay for grid layout
	public void Init(HeroData _heroData, PanelController _panel)
	{
		CoroutineUtil.StartCoroutine(IEInit(_heroData, _panel));
	}

	private IEnumerator IEInit(HeroData _heroData, PanelController _panel)
	{
		heroData = _heroData;
		panel = _panel;

		yield return null;

		Refresh();
		RegisterEvents();
	}

	public void Init(HeroData _heroData, PanelController _panel, UnityAction<HeroView> _choiceAction = null, bool _add = false)
	{
		Init(_heroData, _panel);
		choiceAction = _choiceAction;
		addAction = null;
		objectUp.gameObject.SetActive(false);
		if (imgHighlight != null) imgHighlight.SetActive(_add);
		UpdateArrow();
	}

	public void Init(HeroData _heroData, PanelController _panel, UnityAction<HeroView, bool> _addAction, bool _add = false)
	{
		Init(_heroData, _panel);
		choiceAction = null;
		addAction = _addAction;

		add = _add;
		if (imgHighlight != null) imgHighlight.SetActive(add);
		UpdateArrow();
	}
	public void UpdateAdd(bool add)
	{
		if (imgHighlight != null) imgHighlight.SetActive(add);
		UpdateArrow();
	}
	private void RegisterEvents()
	{
		if (mRegisteredEvents || heroData == null || !heroData.IsEquipped())
			return;

		mRegisteredEvents = true;

		EventDispatcher.AddListener<CurrencyChangedEvent>(OnCurrencyChanged);
		EventDispatcher.AddListener<ChangeGearEvent>(OnChangeGear);
	}

	private void UnregisterEvents()
	{
		if (!mRegisteredEvents)
			return;

		mRegisteredEvents = false;

		EventDispatcher.RemoveListener<CurrencyChangedEvent>(OnCurrencyChanged);
		EventDispatcher.RemoveListener<ChangeGearEvent>(OnChangeGear);
	}

	public void BtnChoice_Pressed()
	{
		if (choiceAction != null)
		{
			choiceAction(this);
		}
		if (addAction != null)
		{
			add = !add;
			addAction(this, add);

			imgHighlight.SetActive(add);
		}
	}

	public void Choice()
	{
		if (imgHighlight != null) imgHighlight.SetActive(true);
	}

	public void UnChoice()
	{
		if (imgHighlight != null) imgHighlight.SetActive(false);
	}
	public bool isTOPHero { get; set; }
	void UpdateArrow()
	{
		if (!canUpdateArrow) return;
		if (objectUp != null)
		{
			if (!imgEquiped.gameObject.activeSelf)
			{
				objectUp.SetActive(isTOPHero);
				objectUp.transform.localScale = Vector3.one;
				objectUp.color = Color.green;
			}
			else
			{
				objectUp.SetActive(!isTOPHero);
				objectUp.transform.localScale = new Vector3(-1, 1, 0);
				objectUp.color = Color.red;
			}
		}
	}

	bool canUpdateArrow = true;
	public void ActiveArrow(bool active)
	{
		if (objectUp != null) objectUp.SetActive(active);
		canUpdateArrow = false;
	}

	private void OnCurrencyChanged(CurrencyChangedEvent e)
	{
		if (e.id == IDs.CURRENCY_COIN
			|| e.id == IDs.CURRENCY_EXP_HERO
			|| e.id == IDs.CURRENCY_DUST_LAVA
			|| e.id == IDs.CURRENCY_DUST_METALIC
			|| e.id == IDs.CURRENCY_DUST_ELECTRIC
			|| e.id == IDs.CURRENCY_DUST_NITROGEN)
		{
			ShowHeroBtn();
		}
	}

	private void ShowHeroBtn()
	{
		if (imgHeroNoti != null)
		{
			if (panel is HeroPanel) imgHeroNoti.SetActive(heroData.IsEquipped() && (heroData.CanLevelUp() || heroData.CanOneClickEquip()));
			else if (panel is LaboratoryPanel) imgHeroNoti.SetActive(heroData.IsEquipped() && heroData.CanStarUp());
		}
	}

	private void OnChangeGear(ChangeGearEvent e)
	{
		ShowHeroBtn();
	}

	private void SetImgRank(int rank)
	{
		if (imgRank == null) return;

		imgRank.sprite = AssetsCollection.instance.GetRankIcon(rank);
		imgRank.SetActive(true);

		imgRankBG.color = colorRanks[rank - 1];
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

	private void SetEquipedSize(bool equip)
	{
		float alpha = imgBg.rectTransform.sizeDelta.y / 200f;

		imgEquiped.SetActive(equip);
		imgEquiped.rectTransform.sizeDelta = new Vector2(40f * alpha, 40f * alpha);
		UpdateArrow();

	}

	private void SetLevelSize()
	{
		float alpha = imgBg.rectTransform.sizeDelta.y / 200f;

		imgLevel.SetActive(true);
		imgLevel.rectTransform.sizeDelta = new Vector2(40f * alpha, 40f * alpha);
	}

	public void SetEmpty()
	{
		this.imgRankBG.gameObject.SetActive(true);
		this.imgLevel.gameObject.SetActive(false);
		this.imgRankElement.gameObject.SetActive(false);
	}
	public void ShowLevel(bool isshow)
	{
		txtLevel.gameObject.SetActive(isshow);
	}
}
