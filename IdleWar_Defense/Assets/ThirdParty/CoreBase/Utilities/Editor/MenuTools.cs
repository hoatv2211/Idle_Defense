using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities.Common;
using Utilities.Components;
using Debug = UnityEngine.Debug;

namespace Utilities.Editor
{
    public class MenuTools : UnityEditor.Editor
    {
        private const string ALT = "&";
        private const string SHIFT = "#";
        private const string CTRL = "%";

        [MenuItem("DevTools/Save Assets " + SHIFT + "_1", priority = 41)]
        private static void SaveAssets()
        {
            AssetDatabase.SaveAssets();
        }

        [MenuItem("DevTools/Dirty Selections " + SHIFT + "_2", priority = 42)]
        private static void Dirty()
        {
            var objs = Selection.gameObjects;
            foreach (var obj in objs)
                EditorUtility.SetDirty(obj);
        }

        //==========================================================

        [MenuItem("DevTools/Group Scene Objects " + ALT + "_F1", priority = 61)]
        private static void GroupSceneObjects()
        {
            var objs = Selection.gameObjects;
            if (objs.Length > 1)
            {
                var group = new GameObject();
                for (int i = 0; i < objs.Length; i++)
                {
                    objs[i].transform.SetParent(group.transform);
                }
                Selection.activeObject = group;
            }
        }

        [MenuItem("DevTools/Ungroup Scene Objects " + ALT + "_F2", priority = 62)]
        private static void UngroupSceneObjects()
        {
            var objs = Selection.gameObjects;
            if (objs.Length > 1)
            {
                for (int i = 0; i < objs.Length; i++)
                    objs[i].transform.SetParent(null);
            }
        }

        //==========================================================

        [MenuItem("DevTools/Run _F5", priority = 81)]
        private static void Run()
        {
            EditorApplication.isPlaying = true;
        }

        [MenuItem("DevTools/Stop #_F5", priority = 82)]
        private static void Stop()
        {
            EditorApplication.isPlaying = false;
        }

        //[MenuItem("DevTools/Just Crash", priority = 63)]
        //private static void JustCrash()
        //{
        //    //It used to test game behaviour if crashing happen
        //    throw new NotImplementedException();
        //}

        //==========================================================

        [MenuItem("CONTEXT/Collider/Create a child object with this collider")]
        public static void Menu_AttachBeam(MenuCommand menuCommand)
        {
            var collider = menuCommand.context as Collider;
            if (collider)
            {
                var obj = Instantiate(collider);
                obj.transform.SetParent(collider.transform);
            }
        }
    }
}