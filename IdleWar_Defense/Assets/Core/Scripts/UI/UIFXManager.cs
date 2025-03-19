using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Common;

namespace FoodZombie.UI
{
    public class UIFXManager : MonoBehaviour
    {
        #region Members

        private static UIFXManager mInstance;
        public static UIFXManager instance => mInstance;

        [SerializeField] private UIFXParticle mFXGold;
        [SerializeField] private UIFXParticle mFXGoldMini;
        [SerializeField] private UIFXParticle mFXStar;
        [SerializeField] private UIFXParticle mFXStarMini;

        private Dictionary<int, CustomPool<UIFXParticle>> mParticlePools;

        #endregion

        //=============================================

        #region MonoBehaviour

        private void Awake()
        {
            if (mInstance == null)
            {
                mInstance = this;
            }
            else if (mInstance != this)
            {
                Destroy(gameObject);
            }

            mParticlePools = new Dictionary<int, CustomPool<UIFXParticle>>();
            mParticlePools.Add(mFXGold.GetInstanceID(), new CustomPool<UIFXParticle>(mFXGold, 0, transform, true));
            mParticlePools.Add(mFXGoldMini.GetInstanceID(), new CustomPool<UIFXParticle>(mFXGoldMini, 0, transform, true));
            mParticlePools.Add(mFXStar.GetInstanceID(), new CustomPool<UIFXParticle>(mFXStar, 0, transform, true));
            mParticlePools.Add(mFXStarMini.GetInstanceID(), new CustomPool<UIFXParticle>(mFXStarMini, 0, transform, true));
        }

        #endregion

        //=============================================

        #region Public

        public UIFXParticle PlayStarFX(Transform pTarget)
        {
            return PlayFX(mFXStar, pTarget);
        }

        public UIFXParticle PlayStarMiniFX(Transform pTarget)
        {
            return PlayFX(mFXStarMini, pTarget);
        }

        public UIFXParticle PlayGoldMiniFX(Transform pTarget)
        {
            return PlayFX(mFXGoldMini, pTarget);
        }

        #endregion

        //==============================================

        #region Private

        private UIFXParticle PlayFX(UIFXParticle pFX, Transform pTarget)
        {
            if (pTarget == null)
            {
                Debug.LogError("No Target");
                return null;
            }

            if (mParticlePools.ContainsKey(pFX.GetInstanceID()))
            {
                int id = pFX.GetInstanceID();
                var fx = mParticlePools[id].Spawn(pTarget.transform.position, true);
                fx.fx.onHidden = () =>
                {
                    mParticlePools[id].Release(fx);
                };
                fx.fx.Play(true);
                return fx;
            }
            return null;
        }

        #endregion
    }
}