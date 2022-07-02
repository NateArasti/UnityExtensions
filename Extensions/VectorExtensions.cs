using UnityEngine;

public static class VectorExtensions
{
    /// <summary>
    /// Return copy of a vector with only x and y coordinates
    /// </summary>
    /// <param name="vector">Current vector</param>
    /// <returns></returns>
    public static Vector3 GetXY(this Vector3 vector) => new(vector.x, vector.y, 0);

    /// <summary>
    /// Return copy of a vector with only x and z coordinates
    /// </summary>
    /// <param name="vector">Current vector</param>
    /// <returns></returns>
    public static Vector3 GetXZ(this Vector3 vector) => new(vector.x, 0, vector.z);

    /// <summary>
    /// Return copy of a vector with only y and z coordinates
    /// </summary>
    /// <param name="vector">Current vector</param>
    /// <returns></returns>
    public static Vector3 GetYZ(this Vector3 vector) => new(0, vector.y, vector.z);
}