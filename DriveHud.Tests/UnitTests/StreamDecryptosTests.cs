using DriveHUD.Importers.BetOnline;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class StreamDecryptosTests
    {
        [Test]
        public void TestDescryption()
        {
            var str = @"xLml4LoQsIfZzDTBd4R1ctKKgppRmD8cfcMTQbPA8Nw5z5AoLv+LSidq89cVzDH7GcRgSV7D
clPwpIuUC2dq/+J2Zm+Q80k1d2FelWo7VmywvFA22xiuIQVJJEKx8/lbia3UbSaPFAfnpXe3
2p797euPrQuWC42KnISdeodyXD2hgc4myMJazkIUag/WvYAtakonbcgBOvLfPC7IeX4b/fQM
Ijs3bThDAxc5c1aBOd6F3myyOJ6DlY329+bXnZfShYRC78SW/11DpuvEAQc6ME743aPOZ3IJ
ekRgIL0o5PPzafxA4QEvOArEyPw/U3tTsLj8JHhwbyrkK6Ez7PHMtMv5KJJWXCnMQfYSdzLF
5jXbHQuqm6oPCYbz90KvrrOiXLbx6fAPwT6VjMpz9j4D7LMhMtNpkp3u4ud1IKzINEOekKiz
6tW3R/7a+kYOZ5JTxGLSQTDMxPhr0JFNmFhXk/pXytrH/xxFVxe5oqV91+iS4lCRzTjYPeaq";

            var dataManager = new BetOnlineBaseDataManagerStub();

            Assert.DoesNotThrow(() =>
            {
                var xml = dataManager.Decrypt(str);
                Debug.WriteLine(xml);
            });
        }

        private class BetOnlineBaseDataManagerStub : BetOnlineBaseDataManager
        {
            public new string Decrypt(string encryptedXml)
            {
                return base.Decrypt(encryptedXml);
            }
        }
    }
}
