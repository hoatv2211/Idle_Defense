using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MapInfo
{
    public int id;
    public string mapName;
    public int missionNumber;
    public int coinStartBonus;
    public int coinNextMissionAdd;
    public int userEXPStartBonus;
    public int userEXPNextMissionAdd;
    public int heroEXPStartBonus;
    public int heroEXPNextMissionAdd;
    public int gemBonus;

    public MapInfo(int _id, 
                    string _mapName, 
                    int _missionNumber, 
                    int _coinStartBonus, 
                    int _coinNextMissionAdd, 
                    int _userEXPStartBonus, 
                    int _userEXPNextMissionAdd,
                    int _heroEXPStartBonus,
                    int _heroEXPNextMissionAdd,
                    int _gemBonus)
    {
        id = _id;
        mapName = _mapName;
        missionNumber = _missionNumber;
        coinStartBonus = _coinStartBonus;
        coinNextMissionAdd = _coinNextMissionAdd;
        userEXPStartBonus = _userEXPStartBonus; 
        userEXPNextMissionAdd = _userEXPNextMissionAdd;
        heroEXPStartBonus = _heroEXPStartBonus;
        heroEXPNextMissionAdd = _heroEXPNextMissionAdd;
        gemBonus = _gemBonus;
    }
}

[System.Serializable]
public class TotalInfo
{
    public int countMap;
    public int countDiscovery;
    
    public TotalInfo(int _countMap, int _countDiscovery)
    {
        countMap = _countMap;
        countDiscovery = _countDiscovery;
    }
}
