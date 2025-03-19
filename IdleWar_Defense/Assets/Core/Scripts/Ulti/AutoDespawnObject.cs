using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AutoDespawnObject : MonoBehaviour
{
	public TextMeshPro TextMesh;
	public Color[] ColorPreset;
	public void TextShow(string text, int colorset, float timeDespawn)
	{
		this.TextMesh.text = text;
		SetDespawnTime(timeDespawn);
		TextMesh.color = ColorPreset[colorset];
	}
	public void SetDespawnTime(float time)
	{
		StopAllCoroutines();
		StartCoroutine(IDDoDespawn(time));
	}

	IEnumerator IDDoDespawn(float time)
	{
		yield return Yielders.Get(time);
		SimplePool.Despawn(gameObject);

	}
}