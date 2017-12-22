using DriveHUD.Entities;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ExportTools
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var tournamentNumber = "101810121";
                var fileToBuild = @"d:\101810121.xml";

                Handhistory[] handHistories;

                using (var session = SessionFactory.Instance.OpenSession())
                {
                    handHistories = session.Query<Handhistory>().Where(x => x.Tourneynumber == tournamentNumber).ToArray();
                }

                BuildHandHistoryFile(fileToBuild, handHistories);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void BuildHandHistoryFile(string fileName, Handhistory[] handHistories)
        {
            XDocument handHistoriesXml = null;

            foreach (var handHistory in handHistories)
            {
                var handHistoryXml = XDocument.Parse(handHistory.HandhistoryVal);

                if (handHistoriesXml == null)
                {
                    handHistoriesXml = handHistoryXml;
                    continue;
                }

                var game = handHistoryXml.Descendants("game").FirstOrDefault();                
                handHistoriesXml.Root.Add(game);

            }
        }
    }
}
