using UnityEngine;

namespace Utilities.Common
{
    public static class ColorExtension
    {
        private const float LightOffset = 0.0625f;
        private const float DarkerFactor = 0.9f;

        /// <summary>
        /// Returns the same color, but with the specified alpha.
        /// </summary>
        public static Color SetAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        /// <summary>
        /// Returns a new color that is this color inverted.
        /// </summary>
        public static Color Invert(this Color color)
        {
            return new Color(1 - color.r, 1 - color.g, 1 - color.b, color.a);
        }

        /// <summary>
		/// Returns an opaque version of the given color.
		/// </summary>
        public static Color Opaque(this Color color)
        {
            return new Color(color.r, color.g, color.b);
        }

        /// <summary>
        /// Returns whether the color is black or almost black.
        /// </summary>
        public static bool IsApproximatelyBlack(this Color color)
        {
            return color.r + color.g + color.b <= Mathf.Epsilon;
        }

        /// <summary>
        /// Returns whether the color is white or almost white.
        /// </summary>
        public static bool IsApproximatelyWhite(this Color color)
        {
            return color.r + color.g + color.b >= 1 - Mathf.Epsilon;
        }

        /// <summary>
        /// Returns a color lighter than the given color.
        /// </summary>
        public static Color Lighter(this Color color)
        {
            return new Color(
                color.r + LightOffset,
                color.g + LightOffset,
                color.b + LightOffset,
                color.a);
        }

        /// <summary>
        /// Returns a color darker than the given color.
        /// </summary>
        public static Color Darker(this Color color)
        {
            return new Color(
                color.r - LightOffset,
                color.g - LightOffset,
                color.b - LightOffset,
                color.a);
        }

        /// <summary>
        /// Returns the brightness of the color, 
        /// defined as the average off the three color channels.
        /// </summary>
        public static float Brightness(this Color color)
        {
            return (color.r + color.g + color.b) / 3;
        }

        public static string ToRGBAHexString(this Color color)
        {
            return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", color.r, color.g, color.b, color.a);
        }

        public static int ToRGBAHex(this Color color)
        {
            Color32 c = color;
            return c.ToRGBAHex();
        }

        public static int ToRGBAHex(this Color32 color)
        {
            return (color.r << 24) | (color.g << 16) | (color.b << 8) | (color.a);
        }

        public static int ToInt(this Color c)
        {
            int retVal = 0;
            retVal |= Mathf.RoundToInt(c.r * 255f) << 24;
            retVal |= Mathf.RoundToInt(c.g * 255f) << 16;
            retVal |= Mathf.RoundToInt(c.b * 255f) << 8;
            retVal |= Mathf.RoundToInt(c.a * 255f);
            return retVal;
        }
    }

    public static class ColorHelper
    {
        public static readonly Color CGBlue = HexToColor("#007AA5");
        public static readonly Color ElectricBlue = HexToColor("#7DF9FF");
        public static readonly Color CGRed = HexToColor("#E03C31");
        public static readonly Color BrickRed = HexToColor("#CB4154");
        public static readonly Color AcidGreen = HexToColor("#B0BF1A");
        public static readonly Color AppleGreen = HexToColor("#8DB600");
        public static readonly Color DarkPurple = HexToColor("#301934");
        public static readonly Color ElectricPurple = HexToColor("#BF00FF");
        public static readonly Color DarkOrange = HexToColor("#FF8C00");
        public static readonly Color DeepPink = HexToColor("#FF1493");
        public static readonly Color AmaranthPink = HexToColor("#F19CBB");
        public static readonly Color AfricanViolet = HexToColor("#B284BE");
        public static readonly Color DarkViolet = HexToColor("#9400D3");

        public static string ToHex(Color pColor)
        {
            string hex = pColor.r.ToString("X2") + pColor.g.ToString("X2") + pColor.b.ToString("X2");
            return hex;
        }

        public static Color HexToColor(string pHex)
        {
            pHex = pHex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
            pHex = pHex.Replace("#", "");//in case the string is formatted #FFFFFF
            byte a = 255;//assume fully visible unless specified in hex
            byte r = byte.Parse(pHex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(pHex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(pHex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            //Only use alpha if the string has enough characters
            if (pHex.Length == 8)
            {
                a = byte.Parse(pHex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return new Color32(r, g, b, a);
        }

        public static int ColorToInt(Color c)
        {
            int retVal = 0;
            retVal |= Mathf.RoundToInt(c.r * 255f) << 24;
            retVal |= Mathf.RoundToInt(c.g * 255f) << 16;
            retVal |= Mathf.RoundToInt(c.b * 255f) << 8;
            retVal |= Mathf.RoundToInt(c.a * 255f);
            return retVal;
        }

        public static Color IntToColor(int val)
        {
            float inv = 1f / 255f;
            Color c = Color.black;
            c.r = inv * ((val >> 24) & 0xFF);
            c.g = inv * ((val >> 16) & 0xFF);
            c.b = inv * ((val >> 8) & 0xFF);
            c.a = inv * (val & 0xFF);
            return c;
        }
    }
}