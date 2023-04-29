using UnityEngine;
using UnityEngine.Scripting;

[assembly: AlwaysLinkAssembly]

namespace UnityExtensions
{

    /// <summary>
    /// This classed is used to pre-load some systems that were defined in System.prefab in Resources folder 
    /// </summary>
    public static class Bootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Execute()
        {
            var systemPrefab = Resources.Load("Systems");
            if (systemPrefab != null)
                Object.DontDestroyOnLoad(Object.Instantiate(systemPrefab));
        }
    }
}