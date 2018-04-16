using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Windows;

namespace Simulator
{
    internal class Utils
    {
        private static Regex HandSplitRegex = new Regex("\r\n\r\n", RegexOptions.Compiled);

        public static IEnumerable<string> SplitUpMultipleHands(string rawHandHistories)
        {
            return HandSplitRegex.Split(rawHandHistories)
                            .Where(s => string.IsNullOrWhiteSpace(s) == false)
                            .Select(s => s.Trim('\r', '\n'));
        }

        public static Window CreateTableWindow(string title, int width, int height)
        {
            var pokerClient = new PokerTable
            {
                Title = title,
                Width = width,
                Height = height
            };

            pokerClient.Show();

            return pokerClient;
        }

        private static object lockObject = new object();

        private static IDHImporterService dhImporterService;

        public static IDHImporterService GetImporterService(bool reCreate = false)
        {
            if (dhImporterService != null && !reCreate)
            {
                return dhImporterService;
            }

            lock (lockObject)
            {
                if (dhImporterService != null)
                {
                    return dhImporterService;
                }

                var endpointAddress = "net.pipe://localhost/DriveHUD/Importer/ImportHandHistory";
                var pipeFactory = new ChannelFactory<IDHImporterService>(new NetNamedPipeBinding(), new EndpointAddress(endpointAddress));

                dhImporterService = pipeFactory.CreateChannel();

                return dhImporterService;
            }
        }
    }
}