using System.Collections.Generic;
using NUnit.Framework;
using UnityExtensions;

public class CollectionExtensionsTests
{
    [Test]
    public void RemoveRandomObject()
    {
        var list = new List<int> {1, 2, 3};
        var removedObject = list.RemoveRandomObject();
        if(list.Count != 2) Assert.Fail();
        if(list.Contains(removedObject)) Assert.Fail();
        Assert.Pass();
    }

    [Test]
    public void TryGetObject()
    {
        var list = new List<int> {1, 2, 3};
        if(list.TryGetObject(4, (i, j) => i * i == j, out var result))
        {
            Assert.AreEqual(2, result);
        }
        else Assert.Fail();
    }
}
