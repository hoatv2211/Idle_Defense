using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using HedgehogTeam.EasyTouch;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;

public class TeamMemberSlot : MonoBehaviour
{
	public Image imgSlot;
	public Text txtSlotUnlock;
	public ButtonItemGameplay dragger;
	public GameObject effectSlot;

	public HeroData heroData;
	private UnityAction<TeamMemberSlot> changeMemberAction;
	private UnityAction<TeamMemberSlot> removeMemberAction;

	public Transform posModelHero;
	public GameObject[] modelHeroPrefabs;

	public GameObject imgNoti;
	public GameObject imgArrow;

	private GameObject modelHero;
	[HideInInspector]
	public Vector3 posStart;

	private bool draged = false;
	[HideInInspector]
	public bool unlocked = false;
	private Vector2 offset;

	private static Material greyMat;

	private void Start()
	{
		posStart = transform.position;
		dragger.onDragStart.AddListener(Dragger_OnDragStart);
		dragger.onDrag.AddListener(Dragger_OnDrag);
		dragger.onDragEnd.AddListener(Dragger_OnDragEnd);
	}

	public void Init(int slotIndex, bool _unlocked, UnityAction<TeamMemberSlot> _changeMemberAction, UnityAction<TeamMemberSlot> _removeMemberAction)
	{
		if (txtSlotUnlock != null)
		{
			int levelToUnlock = Constants.FORMATION_SLOTUNLOCK(slotIndex);
			//txtSlotUnlock.text = string.Format("CLEAR {0}\nTO\nUNLOCK", GameData.Instance.MissionLevelToString(levelToUnlock));
			txtSlotUnlock.text = string.Format(Localization.Get(Localization.ID.PANEL_TITLE_72), GameData.Instance.MissionLevelToString(levelToUnlock));
			txtSlotUnlock.gameObject.SetActive(!_unlocked);
		}
		unlocked = _unlocked;
		if (!unlocked)
			ShowArrowDown(false);
		changeMemberAction = _changeMemberAction;
		removeMemberAction = _removeMemberAction;
	}

	public void ShowHero(HeroData _heroData = null)
	{
		heroData = _heroData;

		if (modelHero != null)
		{
			Destroy(modelHero);
			modelHero = null;
		}

		if (heroData != null)
		{
			imgNoti.SetActive(false);

			var selectHeroModel = heroData.baseId - 1;
			modelHero = GameObject.Instantiate(modelHeroPrefabs[selectHeroModel], posModelHero);
			modelHero.transform.localPosition = Vector3.zero;
		}
		else
		{
			ShowArrowDown(false);
		}

		if (unlocked)
		{
			imgSlot.material = null;
			effectSlot.SetActive(true);

			if (heroData == null) imgNoti.SetActive(GameData.Instance.HeroesGroup.CheckFormationNoti());
		}
		else
		{
			imgSlot.material = GetGreyMat();
			effectSlot.SetActive(false);

			imgNoti.SetActive(false);
			ShowArrowDown(false);
		}
	}
	public void ShowArrowDown(bool isShow)
	{
		imgArrow.gameObject.SetActive(false);
	}
	private void Dragger_OnDragStart(Gesture gesture)
	{
		if (!unlocked || heroData == null) return;

		Vector2 dragPos = gesture.GetTouchToWorldPoint(gesture.position);
		offset = (Vector2)posStart - dragPos;
		Vector2 pos = dragPos + offset;
		transform.position = new Vector3(pos.x, pos.y, -1f);

		draged = false;
	}

	private void Dragger_OnDrag(Gesture gesture)
	{
		if (!unlocked || heroData == null) return;

		Vector2 dragPos = gesture.GetTouchToWorldPoint(gesture.position);
		Vector2 pos = dragPos + offset;
		transform.position = new Vector3(pos.x, pos.y, -1f);

		draged = true;
	}

	private void Dragger_OnDragEnd(Gesture gesture)
	{
		if (!unlocked || heroData == null) return;

		if (!draged && Mathf.Abs(transform.position.x - posStart.x) < 0.0001f && Mathf.Abs(transform.position.y - posStart.y) < 0.0001f)
		{
			removeMemberAction(this);
		}
		else
		{
			changeMemberAction(this);
		}
	}

	public void Repos()
	{
		transform.position = posStart;
	}

	public Material GetGreyMat()
	{
		if (greyMat == null)
			//mGreyMat = new Material(Shader.Find("NBCustom/Sprites/Greyscale"));
			greyMat = Resources.Load<Material>("Greyscale");
		return greyMat;
	}
}
