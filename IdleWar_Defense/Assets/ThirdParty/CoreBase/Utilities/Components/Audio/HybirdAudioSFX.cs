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
    public class HybirdAudioSFX : MonoBehaviour
    {
        [SerializeField] private string[] mIdStrings;
        [SerializeField] private bool mIsLoop;
        [SerializeField, ReadOnly] private int[] mIndexs;
        [SerializeField, Range(0.5f, 2f)] private float mPitchRandomMultiplier = 1f;
        private bool mInitialized;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            if (mInitialized) return;
            mIndexs = new int[mIdStrings.Length];
            for (int i = 0; i < mIdStrings.Length; i++)
            {
                var sound = HybirdAudioCollection.Instance.GetClip(mIdStrings[i], false, out mIndexs[i]);
                if (sound == null)
                    UnityEngine.Debug.LogError("Not found SFX id " + mIdStrings[i]);
            }
            mInitialized = true;
        }

        public void PlaySFX()
        {
            Init();

            if (mIndexs.Length > 0)
            {
                var index = mIndexs[Random.Range(0, mIndexs.Length)];
                HybirdAudioManager.Instance?.PlaySFXByIndex(index, mIsLoop, mPitchRandomMultiplier);
            }
        }

        public void StopSFX()
        {
            for (int i = 0; i < mIndexs.Length; i++)
                HybirdAudioManager.Instance?.StopSFXById(mIndexs[i]);
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(HybirdAudioSFX))]
        private class HybirdAudioSFXEditor : Editor
        {
            private HybirdAudioSFX mScript;
            private string mSearch = "";
            private UnityEngine.UI.Button mButton;

            private void OnEnable()
            {
                mScript = target as HybirdAudioSFX;

                if (mScript.mIdStrings == null)
                    mScript.mIdStrings = new string[0] { };

                mButton = mScript.GetComponent<UnityEngine.UI.Button>();
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                if (HybirdAudioCollection.Instance == null)
                {
                    if (HybirdAudioCollection.Instance == null)
                        EditorGUILayout.HelpBox("HybirdAudioSFX require HybirdAudioCollection. " +
                            "To create HybirdAudioCollection,select Resources folder then from Create Menu " +
                            "select RUtilities/Create Hybird Audio Collection", MessageType.Error);
                    return;
                }

                if (mScript.mIdStrings.Length > 0)
                    EditorHelper.BoxVertical(() =>
                    {
                        for (int i = 0; i < mScript.mIdStrings.Length; i++)
                        {
                            EditorHelper.BoxHorizontal(() =>
                            {
                                EditorHelper.TextField(mScript.mIdStrings[i], "SoundId");
                                if (EditorHelper.ButtonColor("x", Color.red, 24))
                                {
                                    var list = mScript.mIdStrings.ToList();
                                    list.Remove(mScript.mIdStrings[i]);
                                    mScript.mIdStrings = list.ToArray();
                                }
                            });
                        }
                    }, Color.yellow, true);

                EditorHelper.BoxVertical(() =>
                {
                    mSearch = EditorHelper.TextField(mSearch, "Search");
                    if (!string.IsNullOrEmpty(mSearch))
                    {
                        var clips = HybirdAudioCollection.Instance.SFXClips;
                        if (clips != null && clips.Count > 0)
                        {
                            for (int i = 0; i < clips.Count; i++)
                            {
                                if (clips[i].fileName.ToLower().Contains(mSearch.ToLower()))
                                {
                                    if (GUILayout.Button(clips[i].fileName))
                                    {
                                        var list = mScript.mIdStrings.ToList();
                                        if (!list.Contains(clips[i].fileName))
                                        {
                                            list.Add(clips[i].fileName);
                                            mScript.mIdStrings = list.ToArray();
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