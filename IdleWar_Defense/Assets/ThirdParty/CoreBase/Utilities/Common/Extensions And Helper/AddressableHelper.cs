#if ADDRESSABLE
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

namespace Utilities.Common
{
    public class ComponentReference<T> : AssetReference where T : Component
    {
        public ComponentReference(string guid) : base(guid) { }

        public void LoadAsset()
        {
            LoadAssetAsync<T>();
        }

        public override bool ValidateAsset(Object obj)
        {
            var go = obj as GameObject;
            return go != null && go.GetComponent<T>() != null;
        }

        public override bool ValidateAsset(string path)
        {
#if UNITY_EDITOR
            //this load can be expensive...
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            return go != null && go.GetComponent<T>() != null;
#else
            return false;
#endif
        }
    }

    /// <summary>
    /// Example class
    /// </summary>
    [Serializable]
    public class ComponentReferenceSpriteRenderer : ComponentReference<SpriteRenderer>
    {
        public ComponentReferenceSpriteRenderer(string guid) : base(guid) { }
    }

    /// <summary>
    /// Example class
    /// </summary>
    [Serializable]
    public class AssetReferenceAtlas : AssetReferenceT<SpriteAtlas>
    {
        public AssetReferenceAtlas(string guid) : base(guid) { }
    }

    //================================================================================

    public static class AddressableUtil
    {
        //================ Basic

        public static AsyncOperationHandle<SceneInstance> LoadSceneAsync(string pAddress, LoadSceneMode pMode, Action<float> pProgress = null, Action<SceneInstance> pOnCompleted = null)
        {
            var operation = Addressables.LoadSceneAsync(pAddress, pMode);
            WaitLoadTask(operation, pProgress, pOnCompleted);
            return operation;
        }

        public static AsyncOperationHandle<SceneInstance> UnloadSceneAsync(SceneInstance pScene, Action<float> pProgress = null, Action<bool> pOnCompleted = null)
        {
            var operation = Addressables.UnloadSceneAsync(pScene);
            WaitUnloadTask(operation, pProgress, pOnCompleted);
            return operation;
        }

        public static AsyncOperationHandle<TextAsset> LoadTextAssetAsync(string pAddress, Action<float> pProgress = null, Action<TextAsset> pOnCompleted = null)
        {
            var operation = Addressables.LoadAssetAsync<TextAsset>(pAddress);
            WaitLoadTask(operation, pProgress, pOnCompleted);
            return operation;
        }

        public static AsyncOperationHandle<T> LoadAssetAsync<T>(string pAddress, Action<float> pProgress = null, Action<T> pOnCompleted = null) where T : UnityEngine.Object
        {
            var operation = Addressables.LoadAssetAsync<T>(pAddress);
            WaitLoadTask(operation, pProgress, pOnCompleted);
            return operation;
        }

        public static AsyncOperationHandle<GameObject> InstantiateAsync(string pAddress, Action<float> pProgress = null, Action<GameObject> pOnCompleted = null)
        {
            var operation = Addressables.InstantiateAsync(pAddress);
            WaitLoadTask(operation, pProgress, pOnCompleted);
            return operation;
        }

        //================ Asset Reference

        public static AsyncOperationHandle<T> LoadAssetAsync<T>(AssetReference pAsset, Action<float> pProgress = null, Action<T> pOnCompleted = null) where T : UnityEngine.Object
        {
            var operation = pAsset.LoadAssetAsync<T>();
            WaitLoadTask(operation, pProgress, pOnCompleted);
            return operation;
        }

        public static AsyncOperationHandle<T> LoadAssetAsync<T>(AssetReferenceT<T> pAsset, Action<float> pProgress = null, Action<T> pOnCompleted = null) where T : Object
        {
            var operation = pAsset.LoadAssetAsync();
            WaitLoadTask(operation, pProgress, pOnCompleted);
            return operation;
        }

        public static AsyncOperationHandle<Sprite> LoadSpriteAsync(AssetReferenceSprite pAsset, Action<float> pProgress = null, Action<Sprite> pOnCompleted = null)
        {
            var operation = pAsset.LoadAssetAsync();
            WaitLoadTask(operation, pProgress, pOnCompleted);
            return operation;
        }

        public static AsyncOperationHandle<IList<Sprite>> LoadSpritesAsync(AssetReference pAsset, Action<float> pProgress = null, Action<IList<Sprite>> pOnCompleted = null)
        {
            var operation = pAsset.LoadAssetAsync<IList<Sprite>>();
            WaitLoadTask(operation, pProgress, pOnCompleted);
            return operation;
        }

        public static AsyncOperationHandle<GameObject> LoadGameObjectAsync(AssetReferenceGameObject pAsset, Action<float> pProgress = null, Action<GameObject> pOnCompleted = null)
        {
            var operation = pAsset.LoadAssetAsync();
            WaitLoadTask(operation, pProgress, pOnCompleted);
            return operation;
        }

        public static AsyncOperationHandle<Texture> LoadTextureAsync(AssetReferenceTexture pAsset, Action<float> pProgress = null, Action<Texture> pOnCompleted = null)
        {
            var operation = pAsset.LoadAssetAsync();
            WaitLoadTask(operation, pProgress, pOnCompleted);
            return operation;
        }

        public static AsyncOperationHandle<Texture2D> LoadTexture2DAsync(AssetReferenceTexture2D pAsset, Action<float> pProgress = null, Action<Texture2D> pOnCompleted = null)
        {
            var operation = pAsset.LoadAssetAsync();
            WaitLoadTask(operation, pProgress, pOnCompleted);
            return operation;
        }

        public static AsyncOperationHandle<Texture3D> LoadTexture3DAsync(AssetReferenceTexture3D pAsset, Action<float> pProgress = null, Action<Texture3D> pOnCompleted = null)
        {
            var operation = pAsset.LoadAssetAsync();
            WaitLoadTask(operation, pProgress, pOnCompleted);
            return operation;
        }

        //================ Variation

        public static AsyncOperationHandle<IList<T>> LoadAssetsAsync<T>(string pAddress, string pLabel, Action<float> pProgress = null, Action<IList<T>> pOnCompleted = null)
        {
            var operation = Addressables.LoadAssetsAsync<T>(new List<object> { pAddress, pLabel }, null, Addressables.MergeMode.Intersection);
            WaitLoadTask(operation, pProgress, pOnCompleted);
            return operation;
        }

        //=========================================================================================================================================

        private static void WaitCompleteTask<T>(AsyncOperationHandle<T> pOperation, Action<T> pOnCompleted = null)
        {
            pOperation.Completed += (obj) =>
            {
                if (pOnCompleted != null)
                    pOnCompleted(pOperation.Result);

                if (obj.Status == AsyncOperationStatus.Failed)
                    Debug.LogError("Failed to load asset: " + pOperation.OperationException.ToString());
            };
        }

        private static void WaitLoadTask<T>(AsyncOperationHandle<T> operation, Action<float> pProgress = null, Action<T> pOnCompleted = null)
        {
            WaitUtil.Start(new WaitUtil.ConditionEvent()
            {
                triggerCondition = () => operation.IsDone,
                onUpdate = () =>
                {
                    if (pProgress != null)
                        pProgress(operation.PercentComplete);
                },
                onTrigger = () =>
                {
                    if (pOnCompleted != null)
                        pOnCompleted(operation.Result);

                    if (operation.Status == AsyncOperationStatus.Failed)
                        Debug.LogError("Failed to load asset: " + operation.OperationException.ToString());
                },
            });
        }

        private static void WaitUnloadTask<T>(AsyncOperationHandle<T> operation, Action<float> pProgress = null, Action<bool> pOnCompleted = null)
        {
            WaitUtil.Start(new WaitUtil.ConditionEvent()
            {
                triggerCondition = () => operation.IsDone,
                onUpdate = () =>
                {
                    if (pProgress != null)
                        pProgress(operation.PercentComplete);
                },
                onTrigger = () =>
                {
                    if (pOnCompleted != null)
                        pOnCompleted(true);

                    if (operation.Status == AsyncOperationStatus.Failed)
                        Debug.LogError("Failed to unload asset: " + operation.OperationException.ToString());
                },
            });
        }
    }
}
#endif