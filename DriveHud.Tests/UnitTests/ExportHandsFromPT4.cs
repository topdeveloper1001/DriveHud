using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class ExportHandsFromPT4
    {
        private string exportFolder = @"d:\AcePokerSolutions\Temp\ExportedHands";

        internal enum HandType
        {
            Cash,
            Tournament
        }

        [Test]
        [TestCase("JPT", HandType.Cash)]
        [TestCase("JPT", HandType.Tournament)]
        [TestCase("PT4_2016_08_01_165115", HandType.Cash)]
        [TestCase("PT4_2016_08_01_165115", HandType.Tournament)]
        public void Export(string db, HandType handType)
        {
            var pt4Repository = new PT4Repository("Server=127.0.0.1;Port=5432;Database=" + db + ";User Id=sa;Password=sa12345;");

            var allHands = handType == HandType.Cash ? pt4Repository.GetAllCashHands() : pt4Repository.GetAllTournamentsHands();

            if (!Directory.Exists(exportFolder))
            {
                Directory.CreateDirectory(exportFolder);
            }

            var cashFile = Path.Combine(exportFolder, string.Format("{0}_{1}.txt", db, handType.ToString()));

            using (var sw = new StreamWriter(cashFile))
            {
                foreach (var hand in allHands)
                {
                    sw.Write(hand.History);
                    sw.WriteLine();
                    sw.WriteLine();
                    sw.WriteLine();
                    sw.WriteLine();
                }
            }

            Assert.That(File.Exists(cashFile));
        }

        [Test]
        [TestCase("drivehud")]
        public void ExportFromDH(string db)
        {
            if (!Directory.Exists(exportFolder))
            {
                Directory.CreateDirectory(exportFolder);
            }

            var pt4Repository = new PT4Repository("Server=127.0.0.1;Port=5433;Database=" + db + ";User Id=postgres;Password=postgrespass;");

            var hands = pt4Repository.GetAllDHHands();

            var sessions = new Dictionary<long, List<XDocument>>();

            foreach (var hand in hands)
            {
                var handXml = XDocument.Parse(hand);
                var sessionCodeText = handXml.Root.Attribute("sessioncode").Value;
                var sessionCode = long.Parse(sessionCodeText);

                if (!sessions.ContainsKey(sessionCode))
                {
                    sessions.Add(sessionCode, new List<XDocument>());
                }

                sessions[sessionCode].Add(handXml);
            }

            foreach (var session in sessions)
            {
                var hhFile = Path.Combine(exportFolder, string.Format("{0}_{1}.xml", db, session.Key));

                var result = session.Value.FirstOrDefault();

                IEnumerable<XElement> games = null;

                if (session.Value.Count > 1)
                {
                    games = session.Value.Skip(1).Select(x => x.Descendants("game").FirstOrDefault());
                }

                if (games != null)
                {
                    foreach (var game in games)
                    {
                        result.Root.Add(game);
                    }
                }

                result.Save(hhFile);
            }

            Assert.That(hands.Count(), Is.EqualTo(8547));
        }
    }
}