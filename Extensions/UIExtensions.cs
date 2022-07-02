using UnityEngine;
using UnityEngine.UI;

public static class UIExtensions
{
    ///<summary>
    ///World Position of Canvas element
    ///</summary>
    public static Vector2 GetWorldPositionOfCanvasElement(RectTransform rectTransform)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            rectTransform.position,
            CommonUnityExtensions.MainCamera,
            out var result);
        return result;
    }

    ///<summary>
    ///Change alpha of Image to create Fade effect
    ///</summary>
    public static void Fade(this Image image, float alpha)
    {
        var color = image.color;
        color.a = alpha;
        image.color = color;
    }

    ///<summary>
    ///Change alpha of RawImage to create Fade effect
    ///</summary>
    public static void Fade(this RawImage image, float alpha)
    {
        var color = image.color;
        color.a = alpha;
        image.color = color;
    }
}