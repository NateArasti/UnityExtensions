using UnityEngine;

namespace UnityExtensions
{
    public static class CameraExtensions
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
            set
            {
                if (value.CompareTag("MainCamera"))
                    _mainCamera = value;
            }
        }
    }
}
