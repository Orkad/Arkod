using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;
using Arkod.Extensions;

namespace Arkod.Test.Extensions
{
    [TestClass]
    public class IntExtensionsTest
    {
        [TestMethod]
        public void GetDigitTest()
        {
            Check.That(423594340.GetDigits()).ContainsExactly(4, 2, 3, 5, 9, 4, 3, 4, 0);
        }
    }

    [TestClass]
    public class FloatExtensionsTest
    {
        [TestMethod]
        public void GetFractionalPartTest()
        {
            Check.That(3.14f.GetFractionalPart()).Equals(14);
        }

        [TestMethod]
        public void GetIntegerPartTest()
        {
            Check.That(3.14f.GetIntegerPart()).Equals(3);
        }
    }
}
