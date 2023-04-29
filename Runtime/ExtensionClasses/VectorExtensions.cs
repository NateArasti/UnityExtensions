using UnityEngine;

namespace UnityExtensions
{
    public static class VectorExtensions
    {
        /// <summary>
        /// Returns new Vector2 with x as x and z as y
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector2 ToXZVector2(this Vector3 vector) => new Vector2(vector.x, vector.z);

        /// <summary>
        /// Returns new Vector2 with y as x and z as y
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector2 ToYZVector2(this Vector3 vector) => new Vector2(vector.y, vector.z);

        /// <summary>
        /// Return copy of a vector with only x and y coordinates
        /// </summary>
        /// <param name="vector">Current vector</param>
        /// <param name="zOverride">Value that will be put instead of previous z</param>
        /// <returns></returns>
        public static Vector3 GetXY(this Vector3 vector, float zOverride = 0) => new Vector3(vector.x, vector.y, zOverride);

        /// <summary>
        /// Return copy of a vector with only x and z coordinates
        /// </summary>
        /// <param name="vector">Current vector</param>
        /// <param name="yOverride">Value that will be put instead of previous y</param>
        /// <returns></returns>
        public static Vector3 GetXZ(this Vector3 vector, float yOverride) => new Vector3(vector.x, yOverride, vector.z);

        /// <summary>
        /// Return copy of a vector with only y and z coordinates
        /// </summary>
        /// <param name="vector">Current vector</param>
        /// <param name="xOverride">Value that will be put instead of previous x</param>
        /// <returns></returns>
        public static Vector3 GetYZ(this Vector3 vector, float xOverride) => new Vector3(xOverride, vector.y, vector.z);
    }
}