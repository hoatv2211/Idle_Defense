using System.Collections.Generic;
using FoodZombie;
using UnityEngine;

namespace HedgehogTeam.EasyTouch.UI
{
    public class EnemyCounter : MonoBehaviour
    {
        private static EnemyCounter mInstance;
        public static EnemyCounter instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = FindObjectOfType<EnemyCounter>();
                return mInstance;
            }
        }

        [SerializeField]private int totalEnemy;
        [SerializeField]private int totalWave;
        [SerializeField]private int enemyDieCounter;

        public void Init(List<WaveInfo> waveInfos)
        {
            int numberEnemyInLine = 11;
            totalWave = waveInfos.Count;
            totalEnemy = 0;
            for (var i = 0; i < totalWave; i++)
            {
                var count = waveInfos[i].enemyInfos.Length / numberEnemyInLine;
                for (int j = count - 1; j >= 0; j--)
                {
                    for (int k = 0; k < numberEnemyInLine; k++)
                    {
                        var enemyInfo =  waveInfos[i].enemyInfos[j * 11 + k];
                        var id = enemyInfo.id;
                        if (id != 0)
                        {
                            totalEnemy++;
                        }
                    }
                }
                //totalEnemy += waveInfos[i].enemyInfos.Length;
            }
            
            GameplayController.Instance.hubPanel.UpdateWaveInfo(enemyDieCounter,totalEnemy);
        }

        public void OnEnemyDie()
        {
            enemyDieCounter++;

            //Show Score TEST
           // GameplayController.Instance.hubPanel.PvpScoreInGamePanel.SetScore(0, Random.RandomRange(1000, 4000));
           // GameplayController.Instance.hubPanel.PvpScoreInGamePanel.SetScore(1, Random.RandomRange(1000, 4000));

            GameplayController.Instance.hubPanel.UpdateWaveInfo(enemyDieCounter,totalEnemy);
        }
    }
}