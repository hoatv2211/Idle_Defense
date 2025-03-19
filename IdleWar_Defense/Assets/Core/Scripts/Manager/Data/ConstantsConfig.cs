using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ConstantsConfig", menuName = "Assets/Scriptable Objects/ConstantsConfig")]
public class ConstantsConfig : ScriptableObject
{
	[Header("In-game")]
	public float SpeedX1;
	public float SpeedX2;
	public float SpeedPvP;

	[Header("Server Setting")]
	public ServerHostType SeverHost;
	public bool RESTDebug;
	public enum ServerHostType
	{
		Local, GG_Clound
	};
}
