using DriveHUD.Importers.Adda52.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
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
            var bytes = File.ReadAllBytes(@"d:\adda52-package.json");

            Adda52JsonPackage.TryParse(bytes, out Adda52JsonPackage package);

            Assert.IsNotNull(package);
        }        
    }
}