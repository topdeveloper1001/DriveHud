using DriveHUD.Common.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    public class UtilsTests
    {
        [Test]
        [TestCase("", false)]
        [TestCase("1", false)]
        [TestCase("22@22.232", true)]
        [TestCase("peon84@yandex.ru", true)]
        [TestCase("al.v.dan@gmail.com", true)]

        public void TestIsValidEmail(string email, bool expected)
        {
            var actual = Utils.IsValidEmail(email);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
