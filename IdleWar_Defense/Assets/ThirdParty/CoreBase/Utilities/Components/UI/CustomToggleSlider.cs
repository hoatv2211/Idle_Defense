using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Utilities.Common;
using System;

namespace Utilities.Components
{
    [AddComponentMenu("Utitlies/UI/CustomToggleSlider")]
    public class CustomToggleSlider : Toggle
    {
        public TextMeshProUGUI txtLabel;

        [Tooltip("Marker which move to On/Off position")]
        public RectTransform toggleTransform;
        [Tooltip("Position that marker move to when toggle is on")]
        public Vector2 onPosition;
        [Tooltip("Position that marker move to when toggle is off")]
        public Vector2 offPosition;

        public bool enableOnOffContent;
        [Tooltip("Object which active when toggle is on")]
        public GameObject onObject;
        [Tooltip("Object which active when toggle is off")]
        public GameObject offObject;

        public bool enableOnOffColor;
        public Color onColor;
        public Color offColor;

        protected override void OnEnable()
        {
            base.OnEnable();

            Refresh();
            onValueChanged.AddListener(OnValueChanged);
        }

        protected override void OnDisable()
        {
            onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(bool pIsOn)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (enableOnOffContent)
            {
                if (onObject != null) onObject.SetActive(isOn);
                if (offObject != null) offObject.SetActive(!isOn);
            }
            if (toggleTransform != null)
                toggleTransform.anchoredPosition = isOn ? onPosition : offPosition;
            if (enableOnOffColor)
            {
                var targetImg = toggleTransform.GetComponent<Image>();
                if (targetImg != null)
                    targetImg.color = isOn ? onColor : offColor;
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (txtLabel == null)
                txtLabel = gameObject.GetComponentInChildren<TextMeshProUGUI>();

            if (graphic != null)
                graphic.gameObject.SetActive(isOn);

            if (toggleTransform != null)
                toggleTransform.anchoredPosition = isOn ? onPosition : offPosition;

            if (enableOnOffContent)
            {
                if (onObject != null) onObject.SetActive(isOn);
                if (offObject != null) offObject.SetActive(!isOn);
            }

            if (targetGraphic == null)
            {
                var images = GetComponentsInChildren<Image>();
                if (images.Length > 0)
                    targetGraphic = images[0];
            }

            if (enableOnOffColor)
            {
                var targetImg = toggleTransform.GetComponent<Image>();
                if (targetImg != null)
                    targetImg.color = isOn ? onColor : offColor;
            }
        }
#endif
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(CustomToggleSlider), true)]
    class CustomToggleEditor : UnityEditor.UI.ToggleEditor
    {
        private CustomToggleSlider mToggle;

        protected override void OnEnable()
        {
            base.OnEnable();

            mToggle = (CustomToggleSlider)target;
        }

        public override void OnInspectorGUI()
        {
            UnityEditor.EditorGUILayout.BeginVertical("box");
            {
                EditorHelper.SerializeField(serializedObject, "txtLabel");
                EditorHelper.SerializeField(serializedObject, "onPosition");
                EditorHelper.SerializeField(serializedObject, "offPosition");
                EditorHelper.SerializeField(serializedObject, "toggleTransform");
                var property1 = EditorHelper.SerializeField(serializedObject, "enableOnOffContent");
                if (property1.boolValue)
                {
                    UnityEditor.EditorGUI.indentLevel++;
                    UnityEditor.EditorGUILayout.BeginVertical("box");
                    EditorHelper.SerializeField(serializedObject, "onObject");
                    EditorHelper.SerializeField(serializedObject, "offObject");
                    UnityEditor.EditorGUILayout.EndVertical();
                    UnityEditor.EditorGUI.indentLevel--;
                }
                var property2 = EditorHelper.SerializeField(serializedObject, "enableOnOffColor");
                if (property2.boolValue)
                {
                    UnityEditor.EditorGUI.indentLevel++;
                    UnityEditor.EditorGUILayout.BeginVertical("box");
                    EditorHelper.SerializeField(serializedObject, "onColor");
                    EditorHelper.SerializeField(serializedObject, "offColor");
                    UnityEditor.EditorGUILayout.EndVertical();
                    UnityEditor.EditorGUI.indentLevel--;
                }
                EditorHelper.SerializeField(serializedObject, "customTargetGraphic");
                EditorHelper.SerializeField(serializedObject, "m_TargetGraphic");

                if (mToggle.txtLabel != null)
                    mToggle.txtLabel.text = UnityEditor.EditorGUILayout.TextField("Label", mToggle.txtLabel.text);

                serializedObject.ApplyModifiedProperties();
            }
            UnityEditor.EditorGUILayout.EndVertical();

            base.OnInspectorGUI();
        }
    }
#endif
}
