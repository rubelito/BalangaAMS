using BalangaAMS.DataLayer.EntityFramework;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalangaAMS.Test
{
    [TestFixture]
    class test_Sqlite
    {
        [Test]
        public void test_something()
        {
            string test = "just Test";
            Assert.AreEqual("just Test", test);
        }
    }
}
