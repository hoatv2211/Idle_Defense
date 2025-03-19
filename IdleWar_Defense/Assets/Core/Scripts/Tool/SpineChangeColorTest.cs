using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpineChangeColorTest : MonoBehaviour
{
	[SerializeField] private SkeletonAnimation _skelAnim;

	public Color[] Colors;


	public List<ListParts> parts;
	// Start is called before the first frame update
	void Start()
	{
		StartCoroutine(DoLoop());
	}

	IEnumerator DoLoop()
	{
		while (true)
		{
			ChangeColor();
			yield return new WaitForSeconds(2);
		
		}
	}
	// Update is called once per frame
	void ChangeColor()
	{
		//if (Input.GetKeyDown(KeyCode.A))
		{
			foreach (var item in parts)
			{
				Color curernt = Colors[Random.Range(0, Colors.Length)];
				foreach (var item1 in item.parts)
				{

					_skelAnim.skeleton.FindSlot(item1).SetColor(curernt);
				}

			}

		}
	}
}

[System.Serializable]
public class ListParts
{
	[SpineSlot]
	public List<string> parts;
}
