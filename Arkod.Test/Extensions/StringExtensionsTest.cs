using Arkod.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;

namespace Arkod.Test.Extensions
{
    [TestClass]
    public class StringExtensionsTest
    {

        [TestMethod]
        public void ReplaceTest()
        {
            var i = 1;
            Check.That("$guid$! test $guid$, here another test string and then $guid$".Replace("$guid$", () => (i++).ToString()))
                .IsEqualTo("1! test 2, here another test string and then 3");
            var j = 1;
            Check.That("xxxxxx:123456".Replace("x", () => (j++).ToString()))
                .IsEqualTo("123456:123456");
        }
    }
}
