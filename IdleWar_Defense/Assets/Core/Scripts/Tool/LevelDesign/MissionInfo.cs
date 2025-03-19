using System.Collections;
using System.Collections.Generic;
using FoodZombie;

[System.Serializable]
public class MissionInfo
{
	public int id;
	public int waveNumber;
	public float hpx;
	public float damx;
	//first reward
	public int coinBonus;
	public int userEXPBonus;
	public int heroEXPBonus;
	public int gemBonus;
	public int gearBonus;
	//afk reward
	public int coinAFK;
	public int userEXPAFK;
	public int heroEXPAFK;
	public int gemAFK;
	public int gearAFK;
	public int rateGearAFK;
	//win boss reward
	public int coinBoss;
	public int userEXPBoss;
	public int heroEXPBoss;
	public int gemBoss;
	public int gearBoss;
	public int powerFragmentBoss;
	public int powerCrystalBoss;
	public int powerDevineBoss;

	public List<WaveInfo> waveInfos;
	public List<EnemyInfo> enemyInfos;

	public MissionInfo(int _id,
					int _waveNumber, float _hpx, float _damx,
					int _coinBonus,
					int _userEXPBonus,
					int _heroEXPBonus,
					int _gemBonus,
					List<WaveInfo> _waveInfos,
					List<EnemyInfo> _enemyInfos)
	{
		id = _id;
		waveNumber = _waveNumber;
		hpx = _hpx;
		damx = _damx;
		coinBonus = _coinBonus;
		userEXPBonus = _userEXPBonus;
		heroEXPBonus = _heroEXPBonus;
		gemBonus = _gemBonus;
		waveInfos = _waveInfos;
		enemyInfos = _enemyInfos;
	}
}

[System.Serializable]
public class WaveInfo
{
	public int id;
	public float time;
	public EnemyInfo[] enemyInfos;
	public bool powActive = false;
	/// <summary>
	///  SPOW sẽ random drop tại vị trí của 1 trong powValue unit đầu tiên của wave bị tiêu diệt 
	///  (x mặc định =3, có thể điều chỉnh ở các version sau)
	/// </summary>
	public int powValue = 3;

	public WaveInfo(int _id, float _time, EnemyInfo[] _enemyInfos)
	{
		id = _id;
		time = _time;
		enemyInfos = _enemyInfos;
	}
}

[System.Serializable]
public class EnemyInfo
{
	public int id;
	public int level;

	public EnemyInfo()
	{
		id = 0;
		level = 0;
	}

	public EnemyInfo(int _id, int _level)
	{
		id = _id;
		level = _level;
	}
}
