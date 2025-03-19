#if UNITY_EDITOR

using Utilities.AntiCheat;
using UnityEditor;
using UnityEngine;

namespace Utilities.AntiCheat.EditorCode.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(ObscuredShort))]
    internal class ObscuredShortDrawer : ObscuredPropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            var hiddenValue = prop.FindPropertyRelative("hiddenValue");
            SetBoldIfValueOverridePrefab(prop, hiddenValue);

            var cryptoKey = prop.FindPropertyRelative("currentCryptoKey");
            var inited = prop.FindPropertyRelative("inited");

            var currentCryptoKey = (short)cryptoKey.intValue;
            short val = 0;

            if (!inited.boolValue)
            {
                if (currentCryptoKey == 0)
                {
                    currentCryptoKey = ObscuredShort.cryptoKeyEditor;
                    cryptoKey.intValue = ObscuredShort.cryptoKeyEditor;
                }
                hiddenValue.intValue = ObscuredShort.EncryptDecrypt(0, currentCryptoKey);
                inited.boolValue = true;
            }
            else
            {
                val = ObscuredShort.EncryptDecrypt((short)hiddenValue.intValue, currentCryptoKey);
            }

            EditorGUI.BeginChangeCheck();
            val = (short)EditorGUI.IntField(position, label, val);
            if (EditorGUI.EndChangeCheck())
            {
                hiddenValue.intValue = ObscuredShort.EncryptDecrypt(val, currentCryptoKey);
            }


            ResetBoldFont();
        }
    }
}
#endif