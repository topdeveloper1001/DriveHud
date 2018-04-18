using DriveHUD.Entities;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Xml;

namespace ExportTools
{
    class Program
    {
        private const string ExportFolder = @"d:\DH-Exported";

        private const int handHistoryRowsPerQuery = 10000;

        static void Main(string[] args)
        {
            try
            {
                if (!Directory.Exists(ExportFolder))
                {
                    Directory.CreateDirectory(ExportFolder);
                }

                ExportACRTournamentHands();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void ExportACRTournamentHands()
        {
            using (var session = SessionFactory.Instance.OpenStatelessSession())
            {
                var entitiesCount = session.Query<Handhistory>().Count(x => x.PokersiteId == (int)EnumPokerSites.AmericasCardroom && 
                    x.Tourneynumber != null && x.Tourneynumber != string.Empty);

                var numOfQueries = (int)Math.Ceiling((double)entitiesCount / handHistoryRowsPerQuery);

                for (var i = 0; i < numOfQueries; i++)
                {
                    Console.WriteLine($"Processing {i}/{numOfQueries}...");

                    var numOfRowToStartQuery = i * handHistoryRowsPerQuery;

                    var handHistories = session.Query<Handhistory>()
                        .Where(x => x.PokersiteId == (int)EnumPokerSites.AmericasCardroom && x.Tourneynumber != null && x.Tourneynumber != string.Empty)
                        .OrderBy(x => x.HandhistoryId)
                        .Skip(numOfRowToStartQuery)
                        .Take(handHistoryRowsPerQuery)
                        .ToArray();

                    foreach (var handHistory in handHistories)
                    {
                        var fileName = Path.Combine(ExportFolder, $"{handHistory.Tourneynumber}.txt");

                        File.AppendAllLines(fileName, new[] { handHistory.HandhistoryVal, string.Empty });
                    }
                }
            }
        }

        private static void ExportBetOnlineHands()
        {
            using (var session = SessionFactory.Instance.OpenStatelessSession())
            {
                var entitiesCount = session.Query<Handhistory>().Count(x => x.PokersiteId == (int)EnumPokerSites.BetOnline);

                var numOfQueries = (int)Math.Ceiling((double)entitiesCount / handHistoryRowsPerQuery);

                for (var i = 0; i < numOfQueries; i++)
                {
                    Console.WriteLine($"Processing {i}/{numOfQueries}...");

                    var numOfRowToStartQuery = i * handHistoryRowsPerQuery;

                    var handHistories = session.Query<Handhistory>()
                        .Where(x => x.PokersiteId == (int)EnumPokerSites.BetOnline)
                        .OrderBy(x => x.HandhistoryId)
                        .Skip(numOfRowToStartQuery)
                        .Take(handHistoryRowsPerQuery)
                        .ToArray();

                    var handHistoriesBySessionCode = BuildHandHistories(handHistories);

                    foreach (KeyValuePair<string, XDocument> handHistoryBySessionCode in handHistoriesBySessionCode)
                    {
                        SaveHandHistoryToFile(handHistoryBySessionCode.Key, handHistoryBySessionCode.Value);
                    }
                }
            }
        }

        private static Dictionary<string, XDocument> BuildHandHistories(Handhistory[] handHistories)
        {
            var handHistoriesBySessionCode = new Dictionary<string, XDocument>();

            foreach (var handHistory in handHistories)
            {
                try
                {
                    var handHistoryXml = XDocument.Parse(handHistory.HandhistoryVal);

                    var sessionCodeAtt = handHistoryXml.Root.Attribute("sessioncode");

                    if (sessionCodeAtt == null)
                    {
                        Console.WriteLine($"Hand {handHistory.HandhistoryId} has no sessioncode");
                        continue;
                    }

                    var sessionCode = sessionCodeAtt.Value;

                    if (!handHistoriesBySessionCode.ContainsKey(sessionCode))
                    {
                        handHistoriesBySessionCode.Add(sessionCode, handHistoryXml);
                        continue;
                    }

                    var game = handHistoryXml.Descendants("game").FirstOrDefault();

                    handHistoriesBySessionCode[sessionCode].Root.Add(game);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Hand {handHistory.HandhistoryId} could not be processed: {e}");
                }
            }

            return handHistoriesBySessionCode;
        }

        private static void SaveHandHistoryToFile(string sessionCode, XDocument handHistoryXml)
        {
            try
            {
                var handHistoryFile = Path.Combine(ExportFolder, $"{sessionCode}.xml");

                if (!File.Exists(handHistoryFile))
                {
                    SaveXml(handHistoryXml, handHistoryFile);
                    return;
                }

                var existingHandHistoryXml = XDocument.Load(handHistoryFile);

                var games = handHistoryXml.Descendants("game").ToArray();

                foreach (var game in games)
                {
                    existingHandHistoryXml.Root.Add(game);
                }

                SaveXml(existingHandHistoryXml, handHistoryFile);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Hands with sessioncode={sessionCode} could not be saved: {e}");
            }
        }

        private static void SaveXml(XDocument xml, string path)
        {
            try
            {
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    Encoding = new UTF8Encoding(false)
                };

                using (var writer = XmlWriter.Create(path, settings))
                {
                    xml.Save(writer);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"File {path} could not be saved: {e}");
            }
        }
    }
}
