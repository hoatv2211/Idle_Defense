using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class FacebookLoginCustomPostBuild 
{
#if UNITY_IOS
     [PostProcessBuild(999)]
     public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
     {
         string preprocessorPath = path + "/Classes/Preprocessor.h";
         string text = File.ReadAllText(preprocessorPath);
         text = text.Replace("UNITY_SNAPSHOT_VIEW_ON_APPLICATION_PAUSE 1", "UNITY_SNAPSHOT_VIEW_ON_APPLICATION_PAUSE 0");
         File.WriteAllText(preprocessorPath, text);
     }
#endif
}
