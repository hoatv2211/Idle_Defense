#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Utilities.Components
{
    public class AudioMenuTools : Editor
    {
        [MenuItem("DevTools/Audio/Add Audio Manager")]
        private static void AddAudioManager()
        {
            var manager = GameObject.FindObjectOfType<AudioManager>();
            if (manager == null)
            {
                var obj = new GameObject("AudioManager");
                obj.AddComponent<AudioManager>();
            }
        }

        [MenuItem("DevTools/Audio/Add Hybird Audio Manager")]
        private static void AddHybirdAudioManager()
        {
            var manager = GameObject.FindObjectOfType<HybirdAudioManager>();
            if (manager == null)
            {
                var obj = new GameObject("HybirdAudioManager");
                obj.AddComponent<HybirdAudioManager>();
            }
        }

        [MenuItem("DevTools/Audio/Open Audio Collection")]
        private static void OpenAudioCollection()
        {
            Selection.activeObject = AudioCollection.Instance;
        }

        [MenuItem("DevTools/Audio/Open Hybird Audio Collection")]
        private static void OpenHybirdAudioCollection()
        {
            Selection.activeObject = HybirdAudioCollection.Instance;
        }
    }
}
#endif