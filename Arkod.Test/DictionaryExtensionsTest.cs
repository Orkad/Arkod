using Arkod.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;
using System.Collections.Generic;

namespace Arkod.Test
{
    [TestClass]
    public class DictionaryExtensionsTest
    {
        [TestMethod]
        public void TrackChanges()
        {
            var olds = new Dictionary<int, string>();
            olds.Add(1, "test");
            olds.Add(2, "not exists");
            var news = new Dictionary<int, string>();
            news.Add(1, "toto");
            news.Add(3, "tata");
            olds.TrackChanges(news, true, out var changed, out var added, out var removed);
            Check.That(changed).HasSize(1).And.ContainsKey(1).And.ContainsValue("toto");
            Check.That(added).HasSize(1).And.ContainsKey(3).And.ContainsValue("tata");
            Check.That(removed).HasSize(1).And.ContainsKey(2).And.ContainsValue("not exists");

            // Check Update option
            Check.That(olds).IsEquivalentTo(news);
        }
    }
}
