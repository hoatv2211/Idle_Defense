﻿#if UNITY_EDITOR
using Utilities.AntiCheat;
using UnityEditor;
using UnityEngine;

namespace Utilities.AntiCheat.EditorCode.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(ObscuredVector3))]
    internal class ObscuredVector3Drawer : ObscuredPropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            var hiddenValue = prop.FindPropertyRelative("hiddenValue");
            var hiddenValueX = hiddenValue.FindPropertyRelative("x");
            var hiddenValueY = hiddenValue.FindPropertyRelative("y");
            var hiddenValueZ = hiddenValue.FindPropertyRelative("z");
            SetBoldIfValueOverridePrefab(prop, hiddenValue);

            var cryptoKey = prop.FindPropertyRelative("currentCryptoKey");
            var inited = prop.FindPropertyRelative("inited");

            var currentCryptoKey = cryptoKey.intValue;
            var val = Vector3.zero;

            if (!inited.boolValue)
            {
                if (currentCryptoKey == 0)
                {
                    currentCryptoKey = cryptoKey.intValue = ObscuredVector3.cryptoKeyEditor;
                }
                var ev = ObscuredVector3.Encrypt(Vector3.zero, currentCryptoKey);
                hiddenValueX.intValue = ev.x;
                hiddenValueY.intValue = ev.y;
                hiddenValueZ.intValue = ev.z;
                inited.boolValue = true;
            }
            else
            {
                var ev = new ObscuredVector3.RawEncryptedVector3
                {
                    x = hiddenValueX.intValue,
                    y = hiddenValueY.intValue,
                    z = hiddenValueZ.intValue
                };
                val = ObscuredVector3.Decrypt(ev, currentCryptoKey);
            }

            EditorGUI.BeginChangeCheck();
            val = EditorGUI.Vector3Field(position, label, val);
            if (EditorGUI.EndChangeCheck())
            {
                var ev = ObscuredVector3.Encrypt(val, currentCryptoKey);
                hiddenValueX.intValue = ev.x;
                hiddenValueY.intValue = ev.y;
                hiddenValueZ.intValue = ev.z;
            }

            ResetBoldFont();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.wideMode ? EditorGUIUtility.singleLineHeight : EditorGUIUtility.singleLineHeight * 2f;
        }
    }
}
#endif