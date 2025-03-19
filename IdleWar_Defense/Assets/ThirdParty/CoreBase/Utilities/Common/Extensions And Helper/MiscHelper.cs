using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

namespace Utilities.Common
{
    public delegate void VoidDelegate();
    public delegate void IntDelegate(int value);
    public delegate void BoolDelegate(bool value);
    public delegate void FloatDelegate(float value);
    public delegate bool ConditionalDelegate();

    public static class MiscHelper
    {
        public static void SeparateStringAndNum(string pStr, out string pNumberPart, out string pStringPart)
        {
            pNumberPart = "";
            var regexObj = new Regex(@"[-+]?[0-9]*\.?[0-9]+(?:[eE][-+]?[0-9]+)?", RegexOptions.IgnorePatternWhitespace);
            var match = regexObj.Match(pStr);
            pNumberPart = match.ToString();

            pStringPart = pStr.Replace(pNumberPart, "");
        }

        public static string JoinString(string seperator, params string[] strs)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < strs.Length; i++)
            {
                if (!string.IsNullOrEmpty(strs[i]))
                    list.Add(strs[i]);
            }
            return string.Join(seperator, list.ToArray());
        }

        public static void Reverse(StringBuilder sb)
        {
            char t;
            int end = sb.Length - 1;
            int start = 0;

            while (end - start > 0)
            {
                t = sb[end];
                sb[end] = sb[start];
                sb[start] = t;
                start++;
                end--;
            }
        }

        public static void CollectGC()
        {
#if UNITY_EDITOR
            var used = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong();
            var heap = UnityEngine.Profiling.Profiler.GetMonoHeapSizeLong();
            Debug.Log("Before. used:" + used + ", heap:" + heap);
#endif

            GC.Collect();

#if UNITY_EDITOR
            used = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong();
            heap = UnityEngine.Profiling.Profiler.GetMonoHeapSizeLong();
            Debug.Log("After. used:" + used + ", heap:" + heap);
#endif
        }
    }

    public static class MiscExtension
    {
        public static string ToSentenceCase(this string pString)
        {
            var lowerCase = pString.ToLower();
            // matches the first sentence of a string, as well as subsequent sentences
            var r = new Regex(@"(^[a-z])|\.\s+(.)", RegexOptions.ExplicitCapture);
            // MatchEvaluator delegate defines replacement of setence starts to uppercase
            var result = r.Replace(lowerCase, s => s.Value.ToUpper());
            return result;
        }

        public static string ToCapitalizeEachWord(this string pString)
        {
            // Creates a TextInfo based on the "en-US" culture.
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(pString);
        }

        public static bool InsideBounds(this Vector2 pPosition, Bounds pBounds)
        {
            if (pPosition.x < pBounds.min.x)
                return false;
            if (pPosition.x > pBounds.max.x)
                return false;
            if (pPosition.y < pBounds.min.y)
                return false;
            if (pPosition.y > pBounds.max.y)
                return false;
            return true;
        }

        public static void Raise(this Action pAction)
        {
            if (pAction != null) pAction();
        }

        public static void Raise(this UnityAction pAction)
        {
            if (pAction != null) pAction();
        }

        public static void Raise<T>(this Action<T> pAction, T pParam)
        {
            if (pAction != null) pAction(pParam);
        }

        public static void Raise<T>(this UnityAction<T> pAction, T pParam)
        {
            if (pAction != null) pAction(pParam);
        }

        public static void Raise<T, M>(this Action<T, M> pAction, T pParam1, M pParam2)
        {
            if (pAction != null) pAction(pParam1, pParam2);
        }

        public static void Raise<T, M>(this UnityAction<T, M> pAction, T pParam1, M pParam2)
        {
            if (pAction != null) pAction(pParam1, pParam2);
        }
    }
}