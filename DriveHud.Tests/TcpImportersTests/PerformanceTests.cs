using DriveHUD.Common.Infrastructure.CustomServices;
using DriveHUD.Common.Log;
using DriveHUD.Importers.Adda52;
using DriveHUD.Importers.AndroidBase;
using DriveHUD.Importers.PokerKing;
using DriveHUD.Importers.PokerKing.Model;
using DriveHUD.Importers.PokerMaster;
using DriveHUD.Importers.PokerMaster.Model;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using NSubstitute;
using NUnit.Framework;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandHistories.Objects.Hand;

namespace DriveHud.Tests.TcpImportersTests
{
    [TestFixture]
    class PerformanceTests
    {
        private IDHLog testLogger;

        [OneTimeSetUp]

        public virtual void SetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;

            var unityContainer = new UnityContainer();

            unityContainer.RegisterType<IPackageBuilder<PokerKingPackage>, PokerKingPackageBuilder>();
            unityContainer.RegisterType<IPackageBuilder<PokerMasterPackage>, PokerMasterPackageBuilder>();
            unityContainer.RegisterType<IPackageBuilder<Adda52Package>, Adda52PackageBuilder>();
            unityContainer.RegisterType<IPacketManager<PokerKingPackage>, PokerKingPacketManager>();
            unityContainer.RegisterType<IPKHandBuilder, PKHandBuilder>();
            unityContainer.RegisterType<INetworkConnectionsService, NetworkConnectionsService>();

            var eventAggregator = Substitute.For<IEventAggregator>();
            unityContainer.RegisterInstance(eventAggregator);

            var tableWindowProvider = Substitute.For<ITableWindowProvider>();
            tableWindowProvider.GetTableWindowHandle(Arg.Any<Process>()).Returns(new IntPtr(1500));
            unityContainer.RegisterInstance(tableWindowProvider);
           
            var pkCatcherService = Substitute.For<IPKCatcherService>();
            pkCatcherService.CheckHand(Arg.Any<HandHistory>()).Returns(true);
            unityContainer.RegisterInstance(pkCatcherService);

            var locator = new UnityServiceLocator(unityContainer);

            ServiceLocator.SetLocatorProvider(() => locator);

            testLogger = new TestLogger();
            LogProvider.SetCustomLogger(testLogger);
        }

        [SetCulture("zh-CN")]
        //[TestCase(@"d:\Git\Temp\PKLogs\")]
        public void TestPerformance(string logPath)
        {
            var logs = Directory.GetFiles(logPath, "*-*.*.log");

            var capturedPackets = new List<CapturedPacketAdvanced>();

            foreach (var log in logs)
            {
                capturedPackets.AddRange(TcpImporterTestUtils.ReadCapturedPackets(log, "yyyy/M/d dddd tt H:mm:ss", true).Select(x => new CapturedPacketAdvanced(x)));
            }

            CollectionAssert.IsNotEmpty(capturedPackets);

            capturedPackets = capturedPackets.OrderBy(x => x.CreatedTimeStamp).ToList();

            var importer = new PKTestImporter();

            importer.Start();

            CapturedPacket previousCapturedPacket = null;

            foreach (var capturedPacket in capturedPackets)
            {
                if (previousCapturedPacket != null)
                {
                    var delay = capturedPacket.CreatedTimeStamp - previousCapturedPacket.CreatedTimeStamp;
                    Task.Delay(delay).Wait();
                }

                capturedPacket.BufferedTime = DateTime.Now;

                importer.AddPacket(capturedPacket);
                previousCapturedPacket = capturedPacket;
            }

            importer.Stop();
        }

        private class PKTestImporter : PKImporter
        {
            public PKTestImporter()
            {
                IsAdvancedLogEnabled = true;
            }

            public override bool IsDisabled()
            {
                return false;
            }

            protected override void ExportHandHistory(List<HandHistoryData> handHistories)
            {               
            }

            protected override void InitializeLogger()
            {
            }

            protected override void InitializeSettings()
            {
            }

            protected override void LogPacket(CapturedPacket capturedPacket, string ext)
            {
                var packet = capturedPacket as CapturedPacketAdvanced;
                packet.ProcessedTime = DateTime.Now;

                if (packet.ProcessedTime - packet.BufferedTime > TimeSpan.FromSeconds(30))
                {
                    Debug.WriteLine($"Packet {capturedPacket}, processed time: {packet.ProcessedTime - packet.BufferedTime}");
                }
            }

            protected override void LogPackage(PokerKingPackage package)
            {
            }

            protected override void SendPreImporedData(string loadingTextKey, IntPtr windowHandle)
            {
            }
        }

        private class CapturedPacketAdvanced : CapturedPacket
        {
            public CapturedPacketAdvanced(CapturedPacket capturedPacket)
            {
                Bytes = capturedPacket.Bytes;
                Source = capturedPacket.Source;
                Destination = capturedPacket.Destination;
                CreatedTimeStamp = capturedPacket.CreatedTimeStamp;
                SequenceNumber = capturedPacket.SequenceNumber;
            }

            public DateTime BufferedTime { get; set; }

            public DateTime ProcessedTime { get; set; }
        }

        private class TestLogger : IDHLog
        {
            public void Log(Type senderType, object message, LogMessageType logMessageType)
            {
            }

            public void Log(Type senderType, object message, Exception exception, LogMessageType logMessageType)
            {
            }

            public void Log(string loggerName, object message, LogMessageType logMessageType)
            {
                Debug.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {message}");
            }

            public void Log(string loggerName, object message, Exception exception, LogMessageType logMessageType)
            {
                Debug.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {message}: {exception}");
            }
        }
    }
}