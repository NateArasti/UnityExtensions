using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public static class CollectionsExtensions
{
    ///<summary>
    /// Gets random object from list
    ///</summary>
    public static T GetRandomObject<T>(this IReadOnlyList<T> list)
    {
        if (list.Count == 0) throw new UnityException("Can't get random object from empty list");
        return list[Random.Range(0, list.Count)];
    }

    ///<summary>
    /// Gets random object from collection
    /// O(rand_index)
    ///</summary>
    public static T GetRandomObject<T>(this IReadOnlyCollection<T> collection)
    {
        if (collection.Count == 0) throw new UnityException("Can't get random object from empty list");
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
    public static void RemoveRandomObject<T>(this IList<T> list)
    {
        if (list.Count == 0) throw new UnityException("Can't remove random object from empty list");
        list.RemoveAt(Random.Range(0, list.Count));
    }

    ///<summary>
    /// Call Action for each object in collection
    ///</summary>
    public static void ForEachAction<T>(this ICollection<T> collection, UnityAction<T> action)
    {
        foreach (var obj in collection)
        {
            action.Invoke(obj);
        }
    }

    ///<summary>
    /// Trying to find object in collection
    ///</summary>
    public static bool TryGetObject<T1, T2>(this IReadOnlyCollection<T1> collection,
        T2 compareValue, Func<T1, T2, bool> compareFunc, out T1 result)
    {
        foreach (var obj in collection)
        {
            if (!compareFunc(obj, compareValue)) continue;
            result = obj;
            return true;
        }

        result = default;
        return false;
    }
}