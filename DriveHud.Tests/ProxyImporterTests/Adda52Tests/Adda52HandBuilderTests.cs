using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHud.Tests.ProxyImporterTests.Adda52Tests
{
    [TestFixture]
    class Adda52HandBuilderTests
    {
        [Test]
        public void Test()
        {
            var uri = new Uri("https://www.adda52.com:8893/websocket");

            Assert.That(uri.Port, Is.EqualTo(8893));
        }
    }
}
