using System.Collections.Generic;
using UnityEngine;

public static class CoroutineExtensions
{
    private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new();

    ///<summary>
    /// Non-allocated WaitForSeconds
    ///</summary>
    public static WaitForSeconds Wait(float waitTime)
    {
        if (WaitDictionary.TryGetValue(waitTime, out var waitForSeconds))
        {
            return waitForSeconds;
        }

        WaitDictionary[waitTime] = new WaitForSeconds(waitTime);
        return WaitDictionary[waitTime];
    }
}