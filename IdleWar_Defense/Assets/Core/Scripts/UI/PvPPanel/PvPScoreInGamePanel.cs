using FoodZombie;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PvPScoreInGamePanel : MonoBehaviour
{
    public UserView UserPlayer, UserEnemy;
    public Image Image_ScoreProcess;
    public Text Text_ScorePlayer, Text_ScoreEnemy, Text_Time;

    [HideInInspector]
    public int score_player, score_enemy, score_player_add, score_enemy_add;
    public int dam_enemy_receiver, dam_player_receiver;
    public int score_player_final => score_player + score_player_add;
    public int score_enemy_final => score_enemy + score_enemy_add;
    [HideInInspector]
    public float timeToPlay;
    [HideInInspector]
    public bool isPlayerDie;
    float score_precent;
    // Start is called before the first frame update

    PvPRecordPlayManager RecordPlayer;
    public void CallStart()
    {
        try
        {
            UserModel user = UserGroup.UserData;
            UserPlayer.Text_UserName.text = user.UserName;
            UserPlayer.Image_UserView.sprite = user.GetAvatar();

            //UserEnemy.Text_UserName.text = "Ahahaaha";
            score_player = 0; score_enemy = 0;
            dam_enemy_receiver = 0; dam_player_receiver = 0;
            Text_ScorePlayer.text = score_player.ToString();
            Text_ScoreEnemy.text = score_enemy.ToString();

            timeToPlay = 150;
            UpdateTime();
        }
        catch (System.Exception ex)
        {

            Debug.LogError(ex.ToString());
        }

    }

    public void StartGame()
    {
        UserModel user = UserGroup.UserData;
        string formation = GameData.Instance.HeroesGroup.GetEquippedHeroesString();
        int CP = GameData.Instance.HeroesGroup.GetEquippedHeroesCP();
        user.Formation = formation;
        user.CP = CP;
        GameRESTController.Instance.APIUser_UpdateInfor(formation, CP, null, (e) => { Debug.LogError(e); });
        //	GameData.Instance.HeroesGroup.CurrentFormation

        RecordModel = new PvPPlayRecordModel();
        RecordModel.RankID = user.GetRank().ID;
        RecordModel.RankGroupID = user.GetRank().RankGroupID;
        RecordModel.CP = CP;
        isPlayerDie = false;
        UserModel userEnemy = Config.PvpConfig.UserEnemy;
        UserEnemy.Text_UserName.text = userEnemy.UserName;
        UserEnemy.Image_UserView.sprite = userEnemy.GetAvatar(false, (s) =>
        {
            Sprite mes = s;
            UserEnemy.Image_UserView.sprite = mes;
        });

        RecordPlayer = new PvPRecordPlayManager();
        RecordPlayer.Start(this);

        timeToPlay = 150;
        if (IEDoTime != null)
            StopCoroutine(IEDoTime);
        IEDoTime = DoTime();
        StartCoroutine(IEDoTime);
    }
    void UpdateTime()
    {
        Text_Time.text = ((int)(timeToPlay / 60)).ToString("00") + ":" + ((int)(timeToPlay % 60)).ToString("00");
    }
    IEnumerator IEDoTime;
    IEnumerator DoTime()
    {
        while (timeToPlay >= 0)
        {
            UpdateTime();
            yield return new WaitForSecondsRealtime(1);
            timeToPlay -= 1;
            if (timeToPlay <= 0) break;
        }
        RecordPlayer.Stop();
        GameplayController.Instance.PvPEndMatch();
    }
    public void SetUserDied()
    {
        if (IEDoTime != null)
        {
            StopCoroutine(IEDoTime);
            IEDoTime = null;
        }
        isPlayerDie = true;
        Record_PackCurrentRecord();
    }

    public void UpdateBarriers(List<HeroControlsPool> barriersPool)
    {
        List<HeroControl> barries = barriersPool[1].list;
        float _percent = 0;
        float totalHP = 0;
        foreach (HeroControl barrie in barries)
        {
            _percent += barrie.GetHPPercent;
            totalHP += barrie.HP_MAX;
        }
        _percent = _percent / (float)barries.Count;
        this.score_player_add = (int)(_percent * 50);

        float _hp_enemy = totalHP - dam_enemy_receiver;
        if (_hp_enemy < 0) _hp_enemy = 0;
        float _percent_enemy = _hp_enemy / totalHP;
        this.score_enemy_add = (int)(_percent_enemy * 50);
    }
    public void SetScore(int playerIndex, int ScoreAdd)
    {
        if (playerIndex == 0)
        {
            score_player += ScoreAdd;
            RecordDatas.Add((int)(Time.time - RecordTimeStart));
            RecordDatas.Add(ScoreAdd);
            RecordTotalScore += ScoreAdd;
            if (RecordPlayer != null)
                RecordPlayer.FakeMode_SetScore(ScoreAdd);
        }
        else
        {
            score_enemy += ScoreAdd;
        }
        float score_total = score_player + score_enemy;
        if (score_total == 0)
            score_precent = 0.5f;
        else
            score_precent = (float)score_player / (float)score_total;
        Text_ScorePlayer.text = score_player.ToString();
        Text_ScoreEnemy.text = score_enemy.ToString();
        Image_ScoreProcess.fillAmount = (float)(score_player / score_total);
    }
    public void SetDamageToPlayer(int playerIndex, int damAdd)
    {
        if (playerIndex == 0)
        {
            dam_player_receiver += damAdd;
            if (RecordPlayer != null)
                RecordPlayer.FakeMode_SetDamScore(damAdd);
        }
        else
        { dam_enemy_receiver += damAdd; }
    }

    #region Record
    PvPPlayRecordModel RecordModel;
    PvPPlayRecordNote RecordNote;
    List<int> RecordDatas = new List<int>();
    float RecordTimeStart = 0;
    int RecordTotalScore = 0;
    public void Record_InitLevel(int levelIndex)
    {
        RecordNote = new PvPPlayRecordNote();
        RecordNote.level = levelIndex;
        RecordDatas.Clear();
        RecordTimeStart = Time.time;
        RecordTotalScore = 0;
        if (RecordPlayer != null)
            RecordPlayer.OnPlayerLevelStart(RecordNote.level);
    }
    public void Record_LogBarrierTakeDamage(float dam)
    {
        if (RecordNote == null || RecordDatas == null) return;
        RecordDatas.Add((int)(Time.time - RecordTimeStart));
        RecordDatas.Add(-1 * (int)dam);
    }
    public void Record_PackCurrentRecord()
    {
        if (RecordNote == null) return;
        int playerDead = isPlayerDie ? 1 : 0;
        int TotalTimePlay = (int)(Time.time - RecordTimeStart);
        RecordDatas.InsertRange(0, new int[] { TotalTimePlay, RecordTotalScore, playerDead });
        Debug.Log("Pack :" + RecordDatas.ToString());
        RecordModel.datas.Add(new PvPPlayRecordNote()
        {
            level = RecordNote.level,
            datas = RecordDatas.ToArray()
        });
        if (RecordPlayer != null)
            RecordPlayer.OnPlayerLevelEnd(RecordNote.level);
        RecordNote = null;

    }
    public void Record_FinishRecord()
    {
        if (RecordModel != null && RecordModel.datas != null && RecordModel.datas.Count > 0)
        {
            Debug.Log("Finish Record :" + RecordModel.ToString());
            GameRESTController.Instance.APIRecord_AddRecord(RecordModel, null, (error) =>
            {
                Debug.LogError(error);
            });
        }
    }
    #endregion
}
