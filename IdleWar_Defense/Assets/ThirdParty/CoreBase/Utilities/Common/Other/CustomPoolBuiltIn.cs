using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Utilities.Inspector;

namespace Utilities.Common
{
    /// <summary>
    /// BuiltIn Pool, which use preload with objects in scene
    /// </summary>
    [Serializable]
    public class CustomPoolBuiltIn<T> : CustomPool<T> where T : Component
    {
        [SerializeField] public List<T> mBuiltInObjs;

        public CustomPoolBuiltIn(T pPrefab, int pInitialCount, Transform pParent, bool pBuildinPrefab, string pName = "", bool pAutoRelocate = true) : base(pPrefab, pInitialCount, pParent, pBuildinPrefab, pName, pAutoRelocate)
        {
        }

        public CustomPoolBuiltIn(GameObject pPrefab, int pInitialCount, Transform pParent, bool pBuildinPrefab, string pName = "", bool pAutoRelocate = true) : base(pPrefab, pInitialCount, pParent, pBuildinPrefab, pName, pAutoRelocate)
        {
        }

        public void Init(Transform pParent = null, string pName = "")
        {
            if (!string.IsNullOrEmpty(pName))
                mName = pName;

            if (string.IsNullOrEmpty(mName))
                mName = mPrefab.name;

            if (pParent != null)
                mParent = pParent;

            if (mParent == null)
            {
                mParent = new GameObject().transform;
                mParent.name = mName + "_BuiltInPool";
            }

            while (mBuiltInObjs.Count > 0)
            {
                AddOutsider(mBuiltInObjs[0]);
                mBuiltInObjs.RemoveAt(0);
            }
        }
    }
}