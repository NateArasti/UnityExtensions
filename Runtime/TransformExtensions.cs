using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UnityExtensions
{
    public static class TransformExtensions
    {
        /// <summary>
        /// Resets local position, rotation and scale to default values
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="resetPosition"></param>
        /// <param name="resetRotation"></param>
        /// <param name="resetScale"></param>
        public static void ResetLocally(
            this Transform transform,
            bool resetPosition = true,
            bool resetRotation = true,
            bool resetScale = true
        )
        {
            if (resetPosition)
                transform.localPosition = Vector3.zero;
            if (resetRotation)
                transform.localRotation = Quaternion.identity;
            if (resetScale)
                transform.localScale = Vector3.zero;
        }

        /// <summary>
        /// Resets world position and rotation to default values
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="resetPosition"></param>
        /// <param name="resetRotation"></param>
        public static void ResetWorld(
            this Transform transform,
            bool resetPosition = true,
            bool resetRotation = true
        )
        {
            if (resetPosition)
                transform.position = Vector3.zero;
            if (resetRotation)
                transform.rotation = Quaternion.identity;
        }

        /// <summary>
        /// Destroy all kids of object with certain delay
        /// </summary>
        /// <param name="transform">Parent transform</param>
        /// <param name="delayBetween">Delay between destroy call</param>
        public static void DestroyChildren(this Transform transform, float delayBetween = 0)
        {
            var count = 1;
            foreach (Transform childTransform in transform)
            {
                Object.Destroy(childTransform.gameObject, delayBetween * count);
                count++;
            }
        }
        
        /// <summary>
        /// For each object from the given collection, spawns an instance of given prefab.
        /// Also stores everything in dictionary and invoke specified action on spawn
        /// TIP: Useful for generic scrolls in UI
        /// </summary>
        /// <typeparam name="T1">Should be derived from MonoBehaviour</typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="pivot">Will be set as parent of spawned objects</param>
        /// <param name="itemPrefab"></param>
        /// <param name="collection"></param>
        /// <param name="spawnEvent"></param>
        /// <param name="destroyChildren">If entered true, all current kids of pivot will be destroyed</param>
        /// <returns>Dictionary with elements of collection as keys and spawned instances as values</returns>
        public static IReadOnlyDictionary<T2, T1> SpawnPrefabForEachObjectInCollection<T1, T2>(
            this Transform pivot,
            T1 itemPrefab,
            IEnumerable<T2> collection,
            UnityAction<T1, T2> spawnEvent = null,
            bool destroyChildren = false) where T1 : MonoBehaviour
        {
            if (destroyChildren)
            {
                pivot.DestroyChildren();
            }

            var spawnedObjects = new Dictionary<T2, T1>();

            collection.ForEachAction(collectionItem =>
            {
                var prefabInstance = Object.Instantiate(itemPrefab, pivot);
                spawnedObjects.Add(collectionItem, prefabInstance);
                if (spawnEvent != null)
                    spawnEvent.Invoke(prefabInstance, collectionItem);
            });

            return spawnedObjects;
        }
    }
}