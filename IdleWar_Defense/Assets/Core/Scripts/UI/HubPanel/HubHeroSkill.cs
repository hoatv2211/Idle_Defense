using FoodZombie;
using HedgehogTeam.EasyTouch;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;
using FoodZombie.UI;

public class HubHeroSkill : MonoBehaviour
{

	[SerializeField]
	GameObject[] Effect;
	public Image Image_Shawdown, Image_HeroAvatar, Image_HeroBoundRank;
	public Image imgLockDie;

	public TypeSkill typeSkill;

	public SimpleTMPButton Button;

	public ButtonItemGameplay buttonItem;


	float percent;
	bool lastState = false;
	private void Awake()
	{
		foreach (GameObject item in Effect)
		{
			item.SetActive(false);
		}

		imgLockDie.SetActive(false);
		//	SetPercent(0, true);
	}

    #region New
	private Image imgDrag;
	private Image imgNotDrag;
	//Wall 
	private Image[] imgWalls;
	private Vector2 posDragStart = Vector2.zero;
	private HeroExControl heroControl;

	private bool isCanDag(){ return	heroControl.isCanSkill; }

	public void SetTypeSkill(TypeSkill _type, HeroExControl _heroCtr)
    {
		typeSkill = _type;
		heroControl = _heroCtr;

        switch (_type)
        {
			default:
				Button.onClick.AddListener(heroControl.BtnSkill_Pressed);
				break;
			case TypeSkill.CastAoe:
				imgDrag = HubPanel.Instance.imgCastAoe;
				imgNotDrag = HubPanel.Instance.imgNotCastAoe;
				buttonItem.onDragStart.AddListener(Aoe_OnDragStart);
				buttonItem.onDrag.AddListener(Aoe_OnDrag);
				buttonItem.onDragEnd.AddListener(Aoe_OnDragEnd);
				break;
			case TypeSkill.CastTarget:
				objectBlockRayCast = HubPanel.Instance.objectBlockRayCast;
                imgDrag = HubPanel.Instance.imgCastAim;
                imgNotDrag = HubPanel.Instance.imgNotCastAim;
                buttonItem.onDragStart.AddListener(Aim_OnDragStart);
                buttonItem.onDrag.AddListener(Aim_OnDrag);
                buttonItem.onDragEnd.AddListener(Aim_OnDragEnd);
				objAim = null;
				//Button.SetUpEvent(()=>GameplayController.Instance.castSkillAim.Init(heroControl.skill, heroControl.target));				
				break;
			case TypeSkill.CastLineOne:
				imgDrag = HubPanel.Instance.imgCastLine;
				imgNotDrag = HubPanel.Instance.imgNotCastLine;
				buttonItem.onDragStart.AddListener(Line_OnDragStart);
				buttonItem.onDrag.AddListener(Line_OnDrag);
				buttonItem.onDragEnd.AddListener(Line_OnDragEnd);

				break;
			case TypeSkill.CastWall:
				imgDrag = HubPanel.Instance.imgWall;
				imgWalls = HubPanel.Instance.imgCastWall;
				imgNotDrag = HubPanel.Instance.imgNotCastWall;
				buttonItem.onDragStart.AddListener(WallAoe_OnDragStart);
				buttonItem.onDrag.AddListener(WallAoe_OnDrag);
				buttonItem.onDragEnd.AddListener(WallAoe_OnDragEnd);

				var skillValues = heroControl.SkillValues;
				countWall = (int)skillValues[1];
				
				break;
        }
	}

	#region Drag Line
	private void Line_OnDragStart(Gesture gesture)
	{
		if (!isCanDag()) return;
		var posHero = heroControl.transform.position;
		imgDrag.rectTransform.anchoredPosition = new Vector3(posHero.x, posHero.y, 0) * 100;
		imgDrag.SetActive(true);
		imgDrag.color = new Color32(116, 250, 172, 255);
	}

	private void Line_OnDrag(Gesture gesture)
	{
		if (!isCanDag()) return;
		var pos = gesture.GetTouchToWorldPoint(gesture.position);
		//Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);		
		//Vector3 direction = pos - (Vector3)transform.position;
		//imgDrag.transform.up = direction;

		var posHero = heroControl.transform.position;
		var _shootingAngle = Mathf.Rad2Deg * Mathf.Atan2(pos.y- posHero.y, pos.x-posHero.x);
		_shootingAngle = Mathf.Clamp(_shootingAngle, 0.0f + 15, 180.0f - 15);

		imgDrag.transform.rotation = Quaternion.Euler(0, 0, _shootingAngle - 90.0f);


		if (pos.y < -4.6f)
		{
			imgNotDrag.SetActive(true);
			var c = Color.red;
			c.a = 0.5f;
			imgDrag.color = c;
		}
		else
		{
			imgNotDrag.SetActive(false);
			imgDrag.color = new Color32(116, 250, 172, 255);
		}
		
	}

	private void Line_OnDragEnd(Gesture gesture)
	{
		if (!isCanDag()) return;
		imgDrag.SetActive(false);
		if (imgNotDrag.gameObject.activeSelf) return;

		var pos = gesture.GetTouchToWorldPoint(gesture.position);
		//Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		//Debug.LogError(pos + "----" + mousePosition);
		heroControl.skill.LineCastSkill(pos);
		//heroControl.BtnSkill_Pressed();
	}

	#endregion

	#region Drag Aoe
	private void Aoe_OnDragStart(Gesture gesture)
	{
		if (!isCanDag()) return;
		var pos = gesture.GetTouchToWorldPoint(gesture.position);
		posDragStart = this.transform.position;
		imgDrag.rectTransform.anchoredPosition = pos * 100f;
		imgDrag.SetActive(true);
		imgDrag.color = new Color32(116, 250, 172, 255);
	}

	private void Aoe_OnDrag(Gesture gesture)
	{
		if (!isCanDag()) return;
		var pos = gesture.GetTouchToWorldPoint(gesture.position);
		if (pos.y < -4.6f || Vector2.Distance(pos, posDragStart) <= 0.45f)
		{
			imgNotDrag.SetActive(true);
			var c = imgDrag.color = Color.red;
			c.a = 0.5f;
			imgDrag.color = c;
		}
		else
		{
			imgNotDrag.SetActive(false);
			imgDrag.color = new Color32(116, 250, 172, 255);
		}
		imgDrag.rectTransform.anchoredPosition = pos * 100f;
	}

	private void Aoe_OnDragEnd(Gesture gesture)
	{
		if (!isCanDag()) return;
		imgDrag.SetActive(false);
		if (imgNotDrag.gameObject.activeSelf) return;

		var pos = gesture.GetTouchToWorldPoint(gesture.position);

		heroControl.skill.AoeCastSkill(pos);
		heroControl.BtnSkill_Pressed();
		
	}

	#endregion

	#region Drag Wall
	int countWall = 1;
	private void WallAoe_OnDragStart(Gesture gesture)
	{
		if (!isCanDag()) return;
		var pos = gesture.GetTouchToWorldPoint(gesture.position);
		posDragStart = this.transform.position;
		imgDrag.rectTransform.anchoredPosition = pos * 100f;
		imgDrag.SetActive(true);

		for (int i = 0; i < 2; i++)
		{
			imgWalls[i].SetActive(i <= countWall - 1);
			imgWalls[i].color = new Color32(255, 0, 0, 128);
		}

	}

	private void WallAoe_OnDrag(Gesture gesture)
	{
		if (!isCanDag()) return;
		var pos = gesture.GetTouchToWorldPoint(gesture.position);
		if (pos.y < -4.6f || Vector2.Distance(pos, posDragStart) <= 0.45f)
		{
			imgNotDrag.SetActive(true);
			for(int i = 0; i < countWall; i++)
            {
				imgWalls[i].color = new Color32(255, 0, 0, 128);
			}
			
		}
		else
		{
			imgNotDrag.SetActive(false);
			for (int i = 0; i < countWall; i++)
			{
				imgWalls[i].color = new Color32(255, 255, 255, 128);
			}
		}
		imgDrag.rectTransform.anchoredPosition = pos * 100f;
	}

	private void WallAoe_OnDragEnd(Gesture gesture)
	{
		if (!isCanDag()) return;
		imgDrag.SetActive(false);
		if (imgNotDrag.gameObject.activeSelf) return;

		var pos = gesture.GetTouchToWorldPoint(gesture.position);

		heroControl.skill.AoeCastSkill(pos);
		heroControl.BtnSkill_Pressed();

	}



	#endregion

	#region Drag Aim
	private void Aim_OnDragStart(Gesture gesture)
	{
		if (!isCanDag()) return;
		var pos = gesture.GetTouchToWorldPoint(gesture.position);
		posDragStart = this.transform.position;
		imgDrag.rectTransform.anchoredPosition = pos * 100f;
		imgDrag.SetActive(true);
		imgDrag.color = Color.white;
		objectBlockRayCast.SetActive(false);
	}

	private Transform objAim;
	private GameObject objectBlockRayCast;
	private void Aim_OnDrag(Gesture gesture)
	{
		if (!isCanDag()) return;
		var pos = gesture.GetTouchToWorldPoint(gesture.position);
		if (pos.y < -4.6f || Vector2.Distance(pos, posDragStart) <= 0.45f)
		{
			imgNotDrag.SetActive(true);
			var c = imgDrag.color = Color.red;
			c.a = 0.5f;
			imgDrag.color = c;
		}
		else
		{
			imgNotDrag.SetActive(false);
			imgDrag.color = Color.white;
		}
		imgDrag.rectTransform.anchoredPosition = pos * 100f;

		Vector2 cubeRay = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		RaycastHit2D cubeHit = Physics2D.Raycast(cubeRay, Vector2.zero);

		if (cubeHit)
		{
			if (cubeHit.transform.tag == Config.TAG_ENEMY)
			{
			    objAim = cubeHit.transform;
				var posHit = cubeHit.transform.position + new Vector3(0, 0.5f, 0);
				imgDrag.rectTransform.anchoredPosition = posHit * 100f;
			}

		}
		
	}

	private void Aim_OnDragEnd(Gesture gesture)
	{
		if (!isCanDag()) return;
		imgDrag.SetActive(false);
		objectBlockRayCast.SetActive(true);
		if (imgNotDrag.gameObject.activeSelf) return;

		if (objAim != null)
			heroControl.skill.AimCastSkill(objAim.GetComponent<EnemyControl>());
		else
			heroControl.BtnSkill_Pressed();

	}

	#endregion

	//Khi hero death
	public void HeroDeath()
    {
		//Stop drag
		if (isCanDag()&&imgDrag!=null)
			imgDrag.SetActive(false);

		heroControl.SetCountDown();
		imgLockDie.SetActive(true);
		buttonItem.enabled = false;
		Button.enabled = false;

		//Off effect
		foreach (GameObject item in Effect)
			item.SetActive(false);

	}


    #endregion

    #region More

    public void SetRankColor(int rank)
	{
		Image_HeroBoundRank.sprite = AssetsCollection.instance.GetRankIcon(rank);
	}

	public void SetPercent(float value, bool reset = false)
	{
		if (!reset)
			if (value == percent) return;
		percent = value;
		if (percent <= 0)
		{
			if (!lastState)
			{
				foreach (GameObject item in Effect)
				{
					item.SetActive(true);
				}
				lastState = true;
			}
		}
		else
		{
			if (lastState)
			{
				foreach (GameObject item in Effect)
				{
					item.SetActive(false);
				}
				lastState = false;
			}
		}

		Image_Shawdown.fillAmount = percent;
	}

	public bool isFull()
	{
		return percent < 0;
	}

    #endregion
}
