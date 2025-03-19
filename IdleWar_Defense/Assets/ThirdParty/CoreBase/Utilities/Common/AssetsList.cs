using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Common
{
    [System.Serializable]
    public class AssetsList<T> where T : Object
    {
        public virtual bool showBox { get; }
        public virtual bool @readonly { get; }
        public List<T> source = new List<T>();
        public T defaultAsset;

        public AssetsList() { }

        public AssetsList(List<T> pSource, T pDefault = null)
        {
            source = pSource;
            defaultAsset = pDefault;
        }

        public AssetsList<T> Init(List<T> pSource, T pDefault = null)
        {
            source = pSource;
            defaultAsset = pDefault;
            return this;
        }

        public T GetAsset(string pName)
        {
            foreach (var s in source)
                if (s != null && pName != null && s.name.ToLower() == pName.ToLower())
                    return s;

            Debug.LogError(string.Format("Not found {0} with name {1}", typeof(T).Name, pName));
            return defaultAsset;
        }

        public T GetAsset(int pIndex)
        {
            if (pIndex < 0 || pIndex >= source.Count)
            {
                Debug.LogError(string.Format("Index {0} {1} is invalid!", pIndex, typeof(T).Name));
                return defaultAsset;
            }
            return source[pIndex];
        }

        public int GetAssetIndex(string pName)
        {
            for (int i = 0; i < source.Count; i++)
            {
                if (source[i].name == pName)
                    return i;
            }
            Debug.LogError(string.Format("Not found {0} with name {1}", typeof(T).Name, pName));
            return -1;
        }

#if UNITY_EDITOR
        public void DrawInEditor(string pDisplayName)
        {
            var draw = new EditorObject<T>()
            {
                value = defaultAsset,
                showAsBox = defaultAsset is Sprite,
                label = "Default",
            };
            if (EditorHelper.ListObjects(ref source, pDisplayName, showBox, @readonly, new IDraw[] { draw }))
                defaultAsset = (T)draw.outputValue;
        }
#endif
    }

    [System.Serializable]
    public class AssetsArray<T> where T : Object
    {
        public T[] source;
        public T defaultAsset;

        public AssetsArray() { }

        public AssetsArray(T[] pSource, T pDefault)
        {
            source = pSource;
            defaultAsset = pDefault;
        }

        public AssetsArray<T> Init(T[] pSource, T pDefault = null)
        {
            source = pSource;
            defaultAsset = pDefault;
            return this;
        }

        public T GetAsset(string pName)
        {
            foreach (var s in source)
                if (s != null && pName != null && s.name.ToLower() == pName.ToLower())
                    return s;

            Debug.LogError(string.Format("Not found {0} with name {1}", typeof(T).Name, pName));
            return defaultAsset;
        }

        public T GetAsset(int pIndex)
        {
            if (pIndex < 0 || pIndex >= source.Length)
            {
                Debug.LogError(string.Format("Index {0} {1} is invalid!", pIndex, typeof(T).Name));
                return defaultAsset;
            }
            return source[pIndex];
        }

        public int GetAssetIndex(string pName)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i].name == pName)
                    return i;
            }
            Debug.LogError(string.Format("Not found {0} with name {1}", typeof(T).Name, pName));
            return -1;
        }
    }

    [System.Serializable]
    public class SpritesList : AssetsList<Sprite>
    {
        public override bool showBox => true;
        public override bool @readonly => false;
    }
}
