﻿#if UNITY_EDITOR
using Utilities.AntiCheat;
using UnityEditor;
using UnityEngine;

namespace Utilities.AntiCheat.EditorCode.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(ObscuredInt))]
    internal class ObscuredIntDrawer : ObscuredPropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            var hiddenValue = prop.FindPropertyRelative("hiddenValue");
            SetBoldIfValueOverridePrefab(prop, hiddenValue);

            var cryptoKey = prop.FindPropertyRelative("currentCryptoKey");
            var inited = prop.FindPropertyRelative("inited");

            var currentCryptoKey = cryptoKey.intValue;
            var val = 0;

            if (!inited.boolValue)
            {
                if (currentCryptoKey == 0)
                {
                    currentCryptoKey = cryptoKey.intValue = ObscuredInt.cryptoKeyEditor;
                }
                hiddenValue.intValue = ObscuredInt.Encrypt(0, currentCryptoKey);
                inited.boolValue = true;
            }
            else
            {
                val = ObscuredInt.Decrypt(hiddenValue.intValue, currentCryptoKey);
            }

            EditorGUI.BeginChangeCheck();
            val = EditorGUI.IntField(position, label, val);
            if (EditorGUI.EndChangeCheck())
            {
                hiddenValue.intValue = ObscuredInt.Encrypt(val, currentCryptoKey);
            }

            ResetBoldFont();
        }
    }
}
#endif