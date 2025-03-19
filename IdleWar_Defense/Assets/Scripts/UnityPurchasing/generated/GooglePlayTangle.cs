#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("ZL0lw8L1BbZiYYF81l8vpWJ22gu9Qs21dx+d+xYUAagXOYxFwkOwkVrU0jmmgSITgc7lX9FcwiB0adLlw3Hy0cP+9frZdbt1BP7y8vL28/DxBGk7HAKhkI/XRuh/BA91/N5LQXHy/PPDcfL58XHy8vNJJPgnk0dmFTe5oLB2M4z/yshrDIRq5RmEHhwG59T8AWWKRVgUp8cp4eNTWnJKnGYqUK+eRz+3q1D1P1VefGmEWohMEuevM06nq3IP2Xo6luurXqL0O8S4VSk8DPumzTbV1/rD7hNQK9p60RlJkdMzIplDI3SWkMVJY3+QKRXrApNv/Z5ADW62Cr5jGq8A7+eRbCJHYFO3JxBM42y9UaPS8rgF7Gg8WC0pJUobDc4JgvHw8vPy");
        private static int[] order = new int[] { 4,2,12,4,6,12,6,13,9,9,13,13,13,13,14 };
        private static int key = 243;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
