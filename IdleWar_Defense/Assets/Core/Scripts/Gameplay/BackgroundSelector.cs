using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using UnityEngine;
using Random = System.Random;

public class BackgroundSelector : MonoBehaviour
{
	public SpriteRenderer imgBG;
	public Sprite[] imgBGs;

	// Start is called before the first frame update
	void Start()
	{
		var mapId = 1;
		try
		{
			mapId = GameData.Instance.MissionsGroup.GetCurrentMissionData().mapId;
		}
		catch (System.Exception ex)
		{

			Debug.LogError(ex.ToString());
			mapId = 1;
		}

		imgBG.sprite = imgBGs[(mapId - 1) % imgBGs.Length]; //mapId bắt đầu từ 1
	}

	// Update is called once per frame
	void Update()
	{

	}
}
