using UnityEngine;

namespace UnityExtensions
{
    public static class MathExtensions
    {
        /// <summary>
        /// Round a value to nearest int value determined by stepValue
        /// So if stepValue is 5, we round 11 to 10 because we want to go in steps of 5 such as 0, 5, 10, 15
        /// </summary>
        /// <param name="value"></param>
        /// <param name="stepValue"></param>
        /// <returns></returns>
        public static int RoundValueToStep(float value, float stepValue)
        {
            if (stepValue <= 0) throw new UnityException($"List size should be positive  - {stepValue}");
            return (int)(Mathf.Round(value / stepValue) * stepValue);
        }

        /// <summary>
        /// Clamps list indices.
        /// Will even work if index is larger/smaller than listSize, so can loop multiple times
        /// </summary>
        /// <param name="index"></param>
        /// <param name="listSize"></param>
        /// <returns>Clamped index</returns>
        public static int ClampListIndex(int index, int listSize)
        {
            if (listSize <= 0) throw new UnityException($"List size should be positive  - {listSize}");
            return (index % listSize + listSize) % listSize;
        }

        /// <summary>
        /// Adds value to average
        /// </summary>
        /// <param name="currentAverage"></param>
        /// <param name="valueToAdd"></param>
        /// <param name="count">defines how many values does the average consist of</param>
        /// <returns></returns>
        public static float AddValueToAverage(float currentAverage, float valueToAdd, float count)
        {
            return (currentAverage * count + valueToAdd) / (count + 1f);
        }

        #region Determination

        /// <summary>
        /// Returns the determinant of the 2x2 matrix defined as
        /// <code>
        /// | x1 x2 |
        /// | y1 y2 |
        /// </code>
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public static float Det2(float x1, float x2, float y1, float y2)
        {
            return x1 * y2 - y1 * x2;
        }

        /// <summary>
        /// Returns the determinant of the 2x2 matrix defined as
        /// <code>
        /// | x1 x2 |
        /// | y1 y2 |
        /// </code>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float Det2(Vector2 a, Vector2 b)
        {
            return Det2(a.x, b.x, a.y, b.y);
        }

        #endregion

        #region Remap

        /// <summary>
        /// Remaps from first range to second range
        /// </summary>
        /// <param name="value"></param>
        /// <param name="firstRangeStart"></param>
        /// <param name="firstRangeEnd"></param>
        /// <param name="secondRangeStart"></param>
        /// <param name="secondRangeEnd"></param>
        /// <returns></returns>
        public static float Remap(
            float value,
            float firstRangeStart, float firstRangeEnd,
            float secondRangeStart, float secondRangeEnd
            )
        {
            return secondRangeStart + (value - firstRangeStart) *
                ((secondRangeEnd - secondRangeStart) / (firstRangeEnd - firstRangeStart));
        }

        /// <summary>
        /// Remaps from first range to second range
        /// </summary>
        /// <param name="value"></param>
        /// <param name="firstRange"></param>
        /// <param name="secondRange"></param>
        /// <returns></returns>
        public static float Remap(
            float value,
            Vector2 firstRange,
            Vector2 secondRange
            )
        {
            return Remap(
                value, 
                firstRange.x, 
                firstRange.y,
                secondRange.x,
                secondRange.y
                );
        }

        #endregion

        #region Matricies

        /// <summary>
        /// Sums one matrix to another and returns result matrix
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix4x4 Add(this Matrix4x4 a, Matrix4x4 b)
        {
            //Matrix addition is just adding element by element
            return new Matrix4x4(
                new Vector4(a[0, 0] + b[0, 0], a[1, 0] + b[1, 0], a[2, 0] + b[2, 0], a[3, 0] + b[3, 0]),
                new Vector4(a[0, 1] + b[0, 1], a[1, 1] + b[1, 1], a[2, 1] + b[2, 1], a[3, 1] + b[3, 1]),
                new Vector4(a[0, 2] + b[0, 2], a[1, 2] + b[1, 2], a[2, 2] + b[2, 2], a[3, 2] + b[3, 2]),
                new Vector4(a[0, 3] + b[0, 3], a[1, 3] + b[1, 3], a[2, 3] + b[2, 3], a[3, 3] + b[3, 3])
            );
        }

        /// <summary>
        /// Multiplying matrix by given value
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Matrix4x4 Multiply(this Matrix4x4 matrix, float a)
        {
            //Matrix multiplication is just multiplying each element by a
            return new Matrix4x4(
                new Vector4(matrix[0, 0] * a, matrix[1, 0] * a, matrix[2, 0] * a, matrix[3, 0] * a),
                new Vector4(matrix[0, 1] * a, matrix[1, 1] * a, matrix[2, 1] * a, matrix[3, 1] * a),
                new Vector4(matrix[0, 2] * a, matrix[1, 2] * a, matrix[2, 2] * a, matrix[3, 2] * a),
                new Vector4(matrix[0, 3] * a, matrix[1, 3] * a, matrix[2, 3] * a, matrix[3, 3] * a)
            );
        }

        #endregion
    }
}