using System;
using System.Collections.Generic;
using Beemob.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HedgehogTeam.EasyTouch.UI
{
    public class DamageTracker : SerializedMonoBehaviour
    {
        public Dictionary<int, double> heroDamageInBattle;
        private static DamageTracker mInstance;
        public static DamageTracker instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = FindObjectOfType<DamageTracker>();
                return mInstance;
            }
        }

        private void Start()
        {
            heroDamageInBattle = new Dictionary<int, double>();
        }

        public void RecordDamageInBattle(int heroId, double damage)
        {
            if(damage<0) return;
            if (heroDamageInBattle.ContainsKey(heroId))
            {
                heroDamageInBattle[heroId] += damage;
            }
            else
            {
                heroDamageInBattle.Add(heroId,damage);
            }
        }
    }
}