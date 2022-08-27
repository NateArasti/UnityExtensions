using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace UnityExtensions
{
    public static class CollectionsExtensions
    {
        ///<summary>
        /// Gets random object from list
        ///</summary>
        public static T GetRandomObject<T>(this IList<T> list)
        {
            if (list.Count == 0) throw new UnityException("Can't get random object from empty list");
            return list[Random.Range(0, list.Count)];
        }

        ///<summary>
        /// Gets random object from collection
        /// O(rand_index)
        ///</summary>
        public static T GetRandomObject<T>(this ICollection<T> collection)
        {
            if (collection.Count == 0) throw new UnityException("Can't get random object from empty collection");
            var returnIndex = Random.Range(0, collection.Count);
            var count = 0;
            foreach (var obj in collection)
            {
                if (count == returnIndex) return obj;
                count += 1;
            }

            return default;
        }

        ///<summary>
        /// Remove random object from list
        ///</summary>
        public static T RemoveRandomObject<T>(this IList<T> list)
        {
            if (list.Count == 0) throw new UnityException("Can't remove random object from empty list");
            var index = Random.Range(0, list.Count);
            var randomObject = list[index];
            list.RemoveAt(index);
            return randomObject;
        }

        ///<summary>
        /// Call Action for each object in collection
        ///</summary>
        public static void ForEachAction<T>(this IEnumerable<T> collection, UnityAction<T> action)
        {
            foreach (var obj in collection)
            {
                action.Invoke(obj);
            }
        }

        ///<summary>
        /// Trying to find object in collection
        ///</summary>
        public static bool TryGetObject<T1, T2>(this IEnumerable<T1> collection,
            T2 compareValue, Func<T1, T2, bool> compareFunction, out T1 result)
        {
            foreach (var obj in collection)
            {
                if (!compareFunction(obj, compareValue)) continue;
                result = obj;
                return true;
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Shuffle the list in place using the Fisher-Yates method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                var k = Random.Range(0, n + 1);
                (list[k], list[n]) = (list[n], list[k]);

                n -= 1;
            }
        }
    }
}