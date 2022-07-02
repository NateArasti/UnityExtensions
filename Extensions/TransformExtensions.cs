using UnityEngine;

public static class TransformExtensions
{
    ///<summary>
    ///Destroy all kids of object
    ///</summary>
    public static void DestroyChildren(this Transform transform)
    {
        foreach (Transform childTransform in transform)
        {
            Object.Destroy(childTransform.gameObject);
        }
    }

    /// <summary>
    /// Destroy all kids of object with certain delay
    /// </summary>
    /// <param name="transform">Parent transform</param>
    /// <param name="delayBetween">Delay between destroy call</param>
    public static void DestroyChildren(this Transform transform, float delayBetween)
    {
        var count = 1;
        foreach (Transform childTransform in transform)
        {
            Object.Destroy(childTransform.gameObject, delayBetween * count);
            count++;
        }
    }
}