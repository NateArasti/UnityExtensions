using UnityEngine;
using UnityEngine.UI;

namespace UnityExtensions
{
    public static class UIExtensions
    {
        /// <summary>
        /// Finds first parent canvas component of given object with RectTransform
        /// </summary>
        /// <param name="rectTransform">Object RectTransform</param>
        /// <param name="maxSearchDepth">Method will only search for first couple parents, use this parameter to specify the depth.
        /// By default is set to 50 </param>
        /// <returns></returns>
        public static Canvas GetParentCanvas(this RectTransform rectTransform, int maxSearchDepth = 50)
        {
            if (rectTransform.TryGetComponent<Canvas>(out var canvas))
            {
                return canvas;
            }
            var currentTarget = rectTransform.parent;
            var parentCanvas = currentTarget.GetComponent<Canvas>();
            var searchIndex = 1;

            while (parentCanvas == null || searchIndex < maxSearchDepth)
            {
                currentTarget = currentTarget.parent;
                parentCanvas = currentTarget.GetComponent<Canvas>();
                searchIndex++;
            }
            return parentCanvas;
        }

        /// <summary>
        /// Returns the event camera of canvas or main camera if canvas doesn't have event camera
        /// </summary>
        /// <param name="canvas"></param>
        /// <returns></returns>
        public static Camera GetEventCamera(this Canvas canvas) =>
            canvas.worldCamera == null ? CameraExtensions.MainCamera : canvas.worldCamera;

        ///<summary>
        ///World Position of Canvas element
        /// <param name="rectTransform">Canvas element</param>
        /// <param name="referenceCamera">Camera that will be used to find a position, by default will take MainCamera</param>
        ///</summary>
        public static Vector2 GetWorldPosition(this RectTransform rectTransform, Camera referenceCamera = null)
        {
            if (referenceCamera == null) referenceCamera = CameraExtensions.MainCamera;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                rectTransform.position,
                referenceCamera,
                out var result);

            return result;
        }

        ///<summary>
        ///Change alpha of Image to create Fade effect
        /// <param name="image">Current rawImage</param>
        /// <param name="alpha">New alpha</param>
        ///</summary>
        public static void Fade(this Image image, float alpha)
        {
            var color = image.color;
            color.a = alpha;
            image.color = color;
        }

        ///<summary>
        ///Change alpha of RawImage to create Fade effect
        /// <param name="rawImage">Current RawImage</param>
        /// <param name="alpha">New alpha</param>
        ///</summary>
        public static void Fade(this RawImage rawImage, float alpha)
        {
            var color = rawImage.color;
            color.a = alpha;
            rawImage.color = color;
        }
    }
}