#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Beemob.Utilities.PsdToGUI
{
    public class ConvertPsdToGui : OdinEditorWindow
    {
        [MenuItem("DevTools/Psd To GUI", priority = 1)]
        private static void OpenWindow()
        {
            GetWindow<ConvertPsdToGui>().Show();
        }

        public Transform mainCanvas;
        public Vector2 canvasSize;
        [FolderPath(RequireExistingPath = true)]
        public string resourceFolder;

        public TextAsset jsonFile;

        [Button]
        private void Generate()
        {
            var layerUis = JsonUtility.FromJson<LayerUi>(jsonFile.text);
            var files = Directory.GetFiles(resourceFolder, "*.png");
            var spriteData = new List<Object>();
            foreach (var variable in files)
            {
            
                spriteData.AddRange(AssetDatabase.LoadAllAssetRepresentationsAtPath(variable));
            }
            for (var i =  layerUis.UI.Length-1;i>=0; i--)
            {
                var uis = layerUis.UI[i];
                var obj = new GameObject(uis.sprite);
                obj.transform.parent = mainCanvas;
                obj.transform.localScale = Vector3.one;
                var sprite = (Sprite) spriteData.FirstOrDefault(a => a.name.Equals(uis.sprite));
                if (sprite == null)
                {
                    Debug.LogError("Missing file "+uis.sprite+".png");
                }
                obj.AddComponent<Image>().sprite = sprite;
                obj.GetComponent<RectTransform>().localPosition = new Vector3(uis.x-canvasSize.x/2,uis.y-canvasSize.y/2);
                //obj.GetComponent<Image>().SetNativeSize();
                obj.GetComponent<RectTransform>().sizeDelta = new Vector2(uis.width,uis.height);
            }
        }
    }
    [Serializable]
    public class LayerUi
    {
        // ReSharper disable once InconsistentNaming
        public Layer[] UI;
    
    }
    [Serializable]
    public class Layer
    {
        public string sprite;
        public float x;
        public float y;
        public float width;
        public float height;
    }
}

#endif