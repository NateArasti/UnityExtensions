using NUnit.Framework;
using UnityExtensions;

public class MathExtensionsTests
{
    [Test]
    public void RoundValueToStep()
    {
        var roundedValue = MathExtensions.RoundValueToStep(11, 5);
        Assert.AreEqual(10, roundedValue);
    }

    [Test]
    public void ClampListIndex()
    {
        var index = MathExtensions.ClampListIndex(5, 5);
        if(index != 0) Assert.Fail($"Expected 0, was {index}");
        index = MathExtensions.ClampListIndex(-1, 5);
        if (index != 4) Assert.Fail($"Expected 4, was {index}");
        index = MathExtensions.ClampListIndex(11, 5);
        if (index != 1) Assert.Fail($"Expected 1, was {index}");
        Assert.Pass();
    }
}
