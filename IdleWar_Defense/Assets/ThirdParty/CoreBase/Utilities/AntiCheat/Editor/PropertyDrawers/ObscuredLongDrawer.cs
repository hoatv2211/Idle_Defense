#if UNITY_EDITOR

using Utilities.AntiCheat;
using UnityEditor;
using UnityEngine;

namespace Utilities.AntiCheat.EditorCode.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(ObscuredLong))]
    internal class ObscuredLongDrawer : ObscuredPropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            var hiddenValue = prop.FindPropertyRelative("hiddenValue");
            SetBoldIfValueOverridePrefab(prop, hiddenValue);

            var cryptoKey = prop.FindPropertyRelative("currentCryptoKey");
            var inited = prop.FindPropertyRelative("inited");

            var currentCryptoKey = cryptoKey.longValue;
            long val = 0;

            if (!inited.boolValue)
            {
                if (currentCryptoKey == 0)
                {
                    currentCryptoKey = cryptoKey.longValue = ObscuredLong.cryptoKeyEditor;
                }
                hiddenValue.longValue = ObscuredLong.Encrypt(0, currentCryptoKey);
                inited.boolValue = true;
            }
            else
            {
                val = ObscuredLong.Decrypt(hiddenValue.longValue, currentCryptoKey);
            }

            EditorGUI.BeginChangeCheck();
            val = EditorGUI.LongField(position, label, val);
            if (EditorGUI.EndChangeCheck())
            {
                hiddenValue.longValue = ObscuredLong.Encrypt(val, currentCryptoKey);
            }

            ResetBoldFont();
        }
    }
}
#endif