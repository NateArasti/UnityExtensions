using System;
using System.Collections;
using UnityEngine;

public static class CommonUnityExtensions
{
    private static Camera _mainCamera;

    ///<summary>
    /// Stored Main Camera
    ///</summary>
    public static Camera MainCamera
    {
        get
        {
            if (_mainCamera == null) _mainCamera = Camera.main;
            return _mainCamera;
        }
    }

    ///<summary>
    ///Change alpha of SpriteRenderer to create Fade effect
    ///</summary>
    public static void Fade(this SpriteRenderer spriteRenderer, float alpha)
    {
        var color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }

    /// <summary>
    /// Invoking action after delay
    /// </summary>
    /// <param name="behaviour">Which behaviour will start delay coroutine</param>
    /// <param name="action">Action to invoke</param>
    /// <param name="delay">In seconds</param>
    public static void InvokeAction(this MonoBehaviour behaviour, Action action, float delay)
    {
        behaviour.StartCoroutine(DelayCoroutine());
        IEnumerator DelayCoroutine()
        {
            yield return CoroutineExtensions.Wait(delay);
            action.Invoke();
        }
    }
    /// <summary>
    /// Returns a copy of a color with same RGB and alpha = 1
    /// </summary>
    /// <param name="color">Current color</param>
    /// <returns></returns>
    public static Color GetRGB(this Color color) => new(color.r, color.g, color.b, 1);
}