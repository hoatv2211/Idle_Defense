using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities.Components
{
    [ExecuteInEditMode]
    public class ResolutionFixer : MonoBehaviour
    {
        public Camera camera;

        private void OnEnable()
        {
            Match();
        }

        public void Match()
        {
            //độ phân giải tiêu chuẩn
            //720x1560
            //1170x1560
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            float screenAspect = screenWidth * 1.0f / screenHeight;
            float milestoneAspect = 9f / 16f;

            if (screenAspect <= milestoneAspect)
            {
                camera.orthographicSize = (1080f / 100.0f) / (2 * screenAspect);
            }
            else
            {
                camera.orthographicSize = 9.6f; //1920f / 200f
            }
        }
    }

#if UNITY_EDITOR

    [UnityEditor.CustomEditor(typeof(ResolutionFixer))]
    public class ResolutionMatchEditor : UnityEditor.Editor
    {
        private ResolutionFixer script;

        private void OnEnable()
        {
            script = (ResolutionFixer)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Match")) { script.Match(); }
        }
    }
#endif

}