using System.Collections.Generic;
using UnityEngine;

public static class UnityExtensions
{
    //Storing Main Camera
    private static Camera _mainCamera;
    public static Camera MainCamera 
    {
        get 
        {
            if(_mainCamera == null) _mainCamera = Camera.main;
            return _mainCamera;
        }
    }

    //Non-allocating WaitForSeconds
    private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = 
        new Dictionary<float, WaitForSeconds>();

    public static WaitForSeconds Wait(float waitTime)
    {
        if (WaitDictionary.TryGetValue(waitTime, out var waitForSeconds))
        {
            return waitForSeconds;
        }

        WaitDictionary[waitTime] = new WaitForSeconds(waitTime);
        return WaitDictionary[waitTime];
    }

    //World Position of Canvas element
    public static Vector2 GetWorldPositionOfCanvasElement(RectTransform rectTransform)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, 
            rectTransform.position, 
            MainCamera,
            out var result);
        return result;
    }

    //Destroy all kids of object
    public static void DestroyChildren(this Transform transform)
    {
        foreach (Transform childTransform in transform)
        {
            Object.Destroy(childTransform.gameObject);
        }
    }
}
