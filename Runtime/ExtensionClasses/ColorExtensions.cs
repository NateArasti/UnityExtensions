using UnityEngine;

namespace UnityExtensions
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Returns a copy of a color with same RGB and alpha = 1
        /// </summary>
        /// <param name="color">Current color</param>
        /// <param name="alpha">Alpha channel parameter, by default is set to 1</param>
        /// <returns>Returns color with only RGB params and specified alpha</returns>
        public static Color GetRGB(this Color color, float alpha = 1) => new Color(color.r, color.g, color.b, alpha);
    }
}
