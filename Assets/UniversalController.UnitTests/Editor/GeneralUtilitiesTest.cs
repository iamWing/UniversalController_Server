using NUnit.Framework;
using AlphaOwl.UniversalController.Utilities;

public class GeneralUtilitiesTest
{

    [Test]
    public void ArrayCopyTest()
    {
        string[] source = new string[] { "a", "b", "c", "d" };

        CollectionAssert.AreEqual(
            new string[] { "b", "c" },
            GeneralUtilities.ArrayCopy<string>(source, 1, 2)
        );
        CollectionAssert.AreEqual(
            new string[] { "a", "b", "c", "d" },
            GeneralUtilities.ArrayCopy<string>(source, 0, 3)
        );
    }

}
