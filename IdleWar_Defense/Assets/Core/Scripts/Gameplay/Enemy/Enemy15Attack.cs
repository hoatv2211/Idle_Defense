using FoodZombie;
using MagicArsenal;
using Spine;
using UnityEngine;
using Utilities.Common;

namespace HedgehogTeam.EasyTouch.Enemy
{
    public class Enemy15Attack : EnemyAttack
    {
        public MagicBeamStatic BeamStatic;
        
        public override void Init(EnemyExControl _enemyExControl)
        {
            base.Init(_enemyExControl);
            BeamStatic.OnBeamEnd.RemoveAllListeners();
            BeamStatic.OnBeamEnd.AddListener(Damaged);
        }
        public override void TriggerAttack()
        {
            if (enemyExControl.IsDead()) return;

            if (!soundTriggerAttack.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerAttack);

            BeamStatic.gameObject.SetActive(true);
        }

        private void Damaged()
        {
            InfoAttacker infoAttacker = new InfoAttacker(false, 
                InfoAttacker.TYPE_NORMAL, 
                null, 
                enemyExControl, 
                null, 
                enemyExControl.Damage,
                enemyExControl.CritRate,
                enemyExControl.CritDamage, 
                enemyExControl.Accuracy);
        
            if (maxGun > 0)
            {
                //đánh range thì spawn bullet
                if (muzzles != null && muzzles.Length > 0)
                {
                    muzzles[countGun].Play();
                }
            
                shots[countGun].Shot(infoAttacker);

                countGun++;
                if (countGun >= maxGun) countGun = 0;
            }
            else
            {
                //đánh melee thì check ray cast
                RaycastHit2D[] hits = CheckHits();
                foreach (var item in hits)
                {
                    if (item.collider.CompareTag(Config.TAG_HERO))
                    {
                        var heroControl = item.collider.GetComponent<HeroControl>();
                        // if (heroControl == enemyExControl.target)
                        // {
                        heroControl.GetHit(infoAttacker);
                        return;
                        // }
                    }
                }
            }
        }
        protected override void AnimAttack_Complete(TrackEntry trackEntry)
        {
            base.AnimAttack_Complete(trackEntry);
            BeamStatic.gameObject.SetActive(false);
        }
    }
}