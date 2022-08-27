using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityExtensions;

public class CoroutineExtensionsTests
{
    #region Invokes

    [UnityTest]
    public IEnumerator InvokeAfter()
    {
        var checkGameObject1 = new GameObject("Check1");
        var checkGameObject2 = new GameObject("Check2");
        CoroutineExtensions.InvokeAfter(() => checkGameObject1 == null, () => checkGameObject2.name = "New Name");
        yield return new WaitForSeconds(1);
        if (checkGameObject2.name == "New Name") Assert.Fail();
        Object.Destroy(checkGameObject1);
        yield return null;
        if (checkGameObject1 != null) Assert.Fail();
        Assert.AreEqual(checkGameObject2.name, "New Name");
    }

    [UnityTest]
    public IEnumerator InvokeSecondsDelay()
    {
        var checkGameObject = new GameObject("Check");
        CoroutineExtensions.InvokeSecondsDelayed(() => checkGameObject.name = "New Name 1", 1);
        yield return new WaitForSeconds(1);
        if (checkGameObject.name != "New Name 1") Assert.Fail();
        yield return null;
        CoroutineExtensions.InvokeSecondsDelayed(() => checkGameObject.name = "New Name 2", 0);
        yield return null;
        if (checkGameObject.name != "New Name 2") Assert.Fail();
        yield return null;
        CoroutineExtensions.InvokeSecondsDelayed(() => checkGameObject.name = "New Name 3", -1);
        yield return null;
        if (checkGameObject.name != "New Name 3") Assert.Fail();
        Assert.Pass();
    }

    [UnityTest]
    public IEnumerator InvokeFramesDelay()
    {
        var checkGameObject = new GameObject("Check");
        CoroutineExtensions.InvokeFramesDelayed(() => checkGameObject.name = "New Name 1", 2);
        yield return null;
        yield return null;
        if (checkGameObject.name != "New Name 1") Assert.Fail();
        yield return null;
        CoroutineExtensions.InvokeFramesDelayed(() => checkGameObject.name = "New Name 2", 0);
        yield return null;
        if (checkGameObject.name != "New Name 2") Assert.Fail();
        yield return null;
        CoroutineExtensions.InvokeFramesDelayed(() => checkGameObject.name = "New Name 3", -1);
        yield return null;
        if (checkGameObject.name != "New Name 3") Assert.Fail();
        Assert.Pass();
    }

    #endregion
}
