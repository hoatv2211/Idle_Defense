using UnityEngine;
using Utilities.Common;
using Random = UnityEngine.Random;
using UnityEngine.Events;
using Utilities.Inspector;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
#endif

namespace Utilities.Components
{
    public class AudioSFX : MonoBehaviour
    {
        [SerializeField, ReadOnly] private int[] mIndexs;
        [SerializeField] private string[] mClips;
        [SerializeField] private bool mIsLoop;
        [SerializeField, Range(0.5f, 2f)] private float mPitchRandomMultiplier = 1f;

        private bool mInitialized;

        private void Init()
        {
            if (mInitialized)
                return;

            mIndexs = new int[mClips.Length];
            for (int i = 0; i < mClips.Length; i++)
            {
                var clips = AudioCollection.Instance.sfxClips;
                for (int j = 0; j < clips.Length; i++)
                {
                    if (clips[j] != null && clips[j].name == mClips[i])
                    {
                        mIndexs[i] = j;
                        break;
                    }
                }
            }

            mInitialized = true;
        }

        public void PlaySFX()
        {
            Init();

            if (mIndexs.Length > 0)
            {
                var clipIndex = mIndexs[Random.Range(0, mClips.Length)];
                AudioManager.Instance?.PlaySFX(clipIndex, 0, mIsLoop, mPitchRandomMultiplier);
            }
        }

        public void StopSFX()
        {
            Init();

            var clips = AudioCollection.Instance.sfxClips;
            for (int i = 0; i < mIndexs.Length; i++)
            {
                var clip = clips[mIndexs[i]];
                HybirdAudioManager.Instance?.StopSFX(clip);
            }
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(AudioSFX))]
        private class AudioSFXEditor : Editor
        {
            private AudioSFX mScript;
            private string mSearch = "";
            private UnityEngine.UI.Button mButton;

            private void OnEnable()
            {
                mScript = target as AudioSFX;

                if (mScript.mClips == null)
                    mScript.mClips = new string[0];

                mButton = mScript.GetComponent<UnityEngine.UI.Button>();
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                if (AudioCollection.Instance == null)
                {
                    if (AudioCollection.Instance == null)
                        EditorGUILayout.HelpBox("AudioSFX require AudioCollection. " +
                            "To create AudioCollection, select Resources folder then from Create Menu " +
                            "select RUtilities/Create Audio Collection", MessageType.Error);
                    return;
                }

                if (mScript.mClips.Length > 0)
                    EditorHelper.BoxVertical(() =>
                    {
                        for (int i = 0; i < mScript.mClips.Length; i++)
                        {
                            EditorHelper.BoxHorizontal(() =>
                            {
                                EditorHelper.TextField(mScript.mClips[i], "");
                                if (EditorHelper.ButtonColor("x", Color.red, 24))
                                {
                                    var list = mScript.mClips.ToList();
                                    list.Remove(mScript.mClips[i]);
                                    mScript.mClips = list.ToArray();
                                }
                            });
                        }
                    }, Color.yellow, true);

                EditorHelper.BoxVertical(() =>
                {
                    mSearch = EditorHelper.TextField(mSearch, "Search");
                    if (!string.IsNullOrEmpty(mSearch))
                    {
                        var clips = AudioCollection.Instance.sfxClips;
                        if (clips != null && clips.Length > 0)
                        {
                            for (int i = 0; i < clips.Length; i++)
                            {
                                if (clips[i].name.ToLower().Contains(mSearch.ToLower()))
                                {
                                    if (GUILayout.Button(clips[i].name))
                                    {
                                        var list = mScript.mClips.ToList();
                                        if (!list.Contains(clips[i].name))
                                        {
                                            list.Add(clips[i].name);
                                            mScript.mClips = list.ToArray();
                                            mSearch = "";
                                            EditorGUI.FocusTextInControl(null);
                                        }
                                    }
                                }
                            }
                        }
                        else
                            EditorGUILayout.HelpBox("No results", MessageType.Warning);
                    }
                }, Color.white, true);

                if (EditorHelper.ButtonColor("Open Sounds Collection"))
                    Selection.activeObject = AudioCollection.Instance;

                if (mButton != null)
                {
                    if (EditorHelper.ButtonColor("Add to OnClick event"))
                    {
                        UnityAction action = new UnityAction(mScript.PlaySFX);
                        UnityEventTools.AddVoidPersistentListener(mButton.onClick, action);
                    }
                }

                if (GUI.changed)
                    EditorUtility.SetDirty(mScript);
            }
        }
#endif
    }
}
