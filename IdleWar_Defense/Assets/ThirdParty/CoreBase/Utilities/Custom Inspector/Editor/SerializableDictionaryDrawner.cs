using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Utilities.Common;

namespace Utilities.Inspector
{
    [CustomPropertyDrawer(typeof(StringStringDictionary))]
    [CustomPropertyDrawer(typeof(ObjectColorDictionary))]
    [CustomPropertyDrawer(typeof(ObjectObjectDictionary))]
    public class SerializableDictionaryDrawer : SerializableDictionaryPropertyDrawer { }
}