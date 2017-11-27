//-----------------------------------------------------------------------
// <copyright file="RequestUtils.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using DriveHUD.Common.Web;
using DriveHUD.Entities;
using Model;
using System;
using System.Text;

namespace DriveHUD.Importers.GGNetwork.Network
{
    internal class RequestUtils
    {
        private const int ProtocolVersionWebRequestTimeout = 3000;

        private const int AttemptsToReadProtocolVersion = 3;

        private static readonly Random rnd = new Random();

        private static string protocolVersion = string.Empty;

        public static string GetProtocolVersion()
        {
            if (!string.IsNullOrEmpty(protocolVersion))
            {
                return protocolVersion;
            }

            string downloadedProtocolVersion = null;

            var attemptToReadProtocolVersion = 0;

            while (string.IsNullOrEmpty(downloadedProtocolVersion) &&
                attemptToReadProtocolVersion < AttemptsToReadProtocolVersion)
            {
                downloadedProtocolVersion = DownloadProtocolVersion();

                if (!string.IsNullOrEmpty(downloadedProtocolVersion))
                {
                    protocolVersion = downloadedProtocolVersion;
                    return protocolVersion;
                }

                attemptToReadProtocolVersion++;
            }

            LogProvider.Log.Error(typeof(RequestUtils), $"Protocol version has not been obtained after {attemptToReadProtocolVersion} attempts.");

            return protocolVersion;
        }

        public static string CreateFakeHardwareSerialNumber()
        {
            const string pattern = "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx";

            var sb = new StringBuilder();

            var rnd = new Random();

            foreach (char t in pattern)
            {
                switch (t)
                {
                    case 'x':
                        switch (rnd.Next(0, 2))
                        {
                            case 0:
                                sb.Append(GetRandomChar());
                                break;
                            case 1:
                                sb.Append(GetRandomNumber());
                                break;
                        }
                        break;
                    case '4':
                        sb.Append('4');
                        break;
                    case '-':
                        sb.Append('-');
                        break;
                }
            }
            return sb.ToString();
        }

        public static string CreateFakeMacAddress()
        {
            const string pattern = "xx:xx:xx:xx:xx:xx";

            StringBuilder sb = new StringBuilder();

            foreach (char t in pattern)
            {
                switch (t)
                {
                    case 'x':
                        {
                            sb.Append(GetRandomChar());
                            break;
                        }
                    case ':':
                        {
                            sb.Append(':');
                            break;
                        }
                }
            }

            return sb.ToString();
        }

        private static char GetRandomChar()
        {
            const string chars = "ABCDEF0123456789";
            int value = rnd.Next(0, chars.Length);
            return char.ToLower(chars[value]);
        }

        public static int GetRandomNumber()
        {
            return rnd.Next(0, 9);
        }

        private static string DownloadProtocolVersion()
        {
            try
            {
                var protocolVersionUrl = StringFormatter.GetGGNProtocolVersionUrl();

                using (var webClient = new WebClientWithTimeout(ProtocolVersionWebRequestTimeout))
                {
                    var protocolVersion = webClient.DownloadString(protocolVersionUrl);
                    return protocolVersion;
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(typeof(RequestUtils), $"Protocol version has not been obtained. [{EnumPokerSites.GGN}]", e);
            }

            return null;
        }
    }
}