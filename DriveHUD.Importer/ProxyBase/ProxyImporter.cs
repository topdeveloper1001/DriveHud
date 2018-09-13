//-----------------------------------------------------------------------
// <copyright file="ProxyImporter.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using DriveHUD.Importers.AndroidBase;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Helpers;
using Titanium.Web.Proxy.Models;

namespace DriveHUD.Importers.ProxyBase
{
    internal class ProxyImporter : BaseImporter, IProxyImporter
    {
        private const string CertificateResourceName = "DriveHUD.Importers.ProxyBase.rootCert.pfx";

        private static object locker = new object();

        private readonly BlockingCollection<IProxyPacketImporter> importers = new BlockingCollection<IProxyPacketImporter>();

        private ProxyServer proxyServer;
        private int proxyEndPointPort;

        protected override EnumPokerSites Site => EnumPokerSites.Unknown;

        public override string SiteString => "ProxyImporter";

        private readonly IImporterService importerService;

        public ProxyImporter()
        {
            importerService = ServiceLocator.Current.GetInstance<IImporterService>(); ;
        }

        public void RegisterImporter<T>() where T : IProxyPacketImporter
        {
            if (importerService == null)
            {
                return;
            }

            var importer = importerService.GetImporter<T>();

            if (importer != null)
            {
                importers.Add(importer);
            }
        }

        protected override void DoImport()
        {
            lock (locker)
            {
                try
                {
                    // install certificate
                    var certificate = InitializeCertificate();

                    if (proxyServer == null)
                    {
                        // replace default certificate with custom certificate, because we need to get rid of warning message about certificate
                        proxyServer = new ProxyServer
                        {
                            RootCertificate = certificate,
                            TrustRootCertificate = true,
                            ForwardToUpstreamGateway = true
                        };
                    }

                    if (proxyEndPointPort == 0 || PortInUse(proxyEndPointPort))
                    {
                        proxyEndPointPort = GetAvailablePort();
                    }

                    proxyServer.ProxyEndPoints.Clear();

                    var proxyEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, proxyEndPointPort, true);
                    proxyServer.AddEndPoint(proxyEndPoint);

                    proxyServer.BeforeRequest += OnBeforeRequest;

                    proxyServer.Start();
                    proxyServer.SetAsSystemProxy(proxyEndPoint, ProxyProtocolType.AllHttp);

                    LogProvider.Log.Info(this, $"Proxy server successfully started [{SiteString}].");
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, $"Failed to start proxy server [{SiteString}].", e);

                    Clean();
                    RaiseProcessStopped();
                }
            }
        }

        private async Task OnBeforeRequest(object sender, SessionEventArgs e)
        {
            if (e.WebSession == null || e.WebSession.Request == null)
            {
                return;
            }
#if DEBUG
            // Console.WriteLine($"Request from host: {e.WebSession.Request.RequestUri}");
#endif

            var importer = importers.FirstOrDefault(x => !x.IsDisabled() && x.IsRunning && x.IsMatch(e.WebSession.Request));

            if (importer == null)
            {
                return;
            }

            await Task.Run(() =>
            {
                e.DataReceived += (s, a) =>
                {
                    if (a.Count == 0)
                    {
                        return;
                    }

                    var capturedPacket = new CapturedPacket
                    {
                        Bytes = a.Buffer.ToArray(),
                        CreatedTimeStamp = DateTime.UtcNow,
                        Source = new IPEndPoint(IPAddress.Any, e.WebSession.Request.RequestUri.Port),
                        Destination = e.ClientEndPoint
                    };

                    importer.AddPacket(capturedPacket);
                };

                //e.DataSent += (s, a) =>
                //{
                //    if (a.Count == 0)
                //    {
                //        return;
                //    }

                //    var capturedPacket = new CapturedPacket
                //    {
                //        Bytes = a.Buffer,
                //        CreatedTimeStamp = DateTime.UtcNow,
                //        Source = e.ClientEndPoint,
                //        Destination = new IPEndPoint(IPAddress.Any, e.WebSession.Request.RequestUri.Port)
                //    };

                //    importer.AddPacket(capturedPacket);
                //};
            });
        }

        public override void Stop()
        {
            base.Stop();

            Task.Run(() =>
            {
                lock (locker)
                {
                    Clean();
                    RaiseProcessStopped();
                }
            });
        }

        /// <summary>
        /// Determines whenever importer is disabled
        /// </summary>
        /// <returns>True if disabled; otherwise - false</returns>
        public override bool IsDisabled()
        {
            return importers.All(x => x.IsDisabled());
        }

        /// <summary>
        /// Cleans up resources
        /// </summary>
        protected void Clean()
        {
            if (proxyServer == null)
            {
                return;
            }

            try
            {
                if (proxyServer.ProxyRunning)
                {
                    proxyServer.Stop();
                }

                proxyServer.BeforeRequest -= OnBeforeRequest;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Failed to stop proxy server [{SiteString}]", e);
            }
        }

        /// <summary>
        /// Gets available port for proxy server
        /// </summary>
        /// <returns>Port</returns>
        protected int GetAvailablePort()
        {
            try
            {
                var random = new Random();

                var port = 0;

                var counter = 0;

                do
                {
                    port = random.Next(5000, 9999);
                    counter++;

                    if (counter > 5000)
                    {
                        LogProvider.Log.Warn(this, $"All ports from 5000 to 9999 are in use. Use default port. [{SiteString}]");
                        return 0;
                    }
                }
                while (PortInUse(port));

                LogProvider.Log.Error($"Assigned port {port} to use. [{SiteString}]");

                return port;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Couldn't assign port. Use default. [{SiteString}]", e);
                return 0;
            }
        }

        /// <summary>
        /// Determines whenever the specified port is in use
        /// </summary>
        /// <param name="port">Port to check</param>
        /// <returns>True if port is in use; otherwise - false</returns>
        private static bool PortInUse(int port)
        {
            var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            var ipEndPoints = ipProperties.GetActiveTcpListeners();

            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == port)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Initializes certificate for proxy server to process https traffic
        /// </summary>
        protected X509Certificate2 InitializeCertificate()
        {
            try
            {
                var resourcesAssembly = GetType().Assembly;

                byte[] certificateBytes;

                using (var resourceStream = resourcesAssembly.GetManifestResourceStream(CertificateResourceName))
                {
                    using (var ms = new MemoryStream())
                    {
                        resourceStream.CopyTo(ms);
                        certificateBytes = ms.ToArray();
                    }
                }

                var certificate = new X509Certificate2(certificateBytes, string.Empty, X509KeyStorageFlags.Exportable);

                var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);

                store.Open(OpenFlags.ReadWrite);

                if (!store.Certificates.Contains(certificate))
                {
                    store.Add(certificate);
                    LogProvider.Log.Info(this, $"Certificate for proxy server has been added to the store. [{SiteString}]");
                }

                store.Close();

                return certificate;
            }
            catch (Exception e)
            {
                throw new DHInternalException(new NonLocalizableString("Certificate has not been installed"), e);
            }
        }
    }
}