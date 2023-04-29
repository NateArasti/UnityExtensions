using UnityEditor;
using UnityEngine;

namespace UnityExtensions
{
    public static class PlayerPrefsTool
    {
        [MenuItem("Tools/Clear PlayerPrefs")]
        public static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}