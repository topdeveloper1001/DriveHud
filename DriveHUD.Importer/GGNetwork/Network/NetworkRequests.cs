//-----------------------------------------------------------------------
// <copyright file="NetworkRequests.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace DriveHUD.Importers.GGNetwork.Network
{
    internal class NetworkRequests
    {
        private const string Token = "GUEST";
        private const string BrandId = "NATURAL8";
        private const int UserDeviceType = 1;
        private const int UserOsType = 1;

        public static string CreateInitialRequest()
        {
            var initialRequest = new InitialRequest
            {
                Token = Token,
                BrandId = BrandId,
                ProtocolVersion = RequestUtils.GetProtocolVersion(),
                HardwareSerialNumber = RequestUtils.CreateFakeHardwareSerialNumber(),
                MacAddress = RequestUtils.CreateFakeMacAddress(),
                UserDeviceType = UserDeviceType,
                UserOsType = UserOsType
            };

            var serializedInitialRequest = JsonConvert.SerializeObject(initialRequest);

            return serializedInitialRequest;
        }

        public static string CreateUserIdMessage(string userId)
        {
            var sb = new StringBuilder();

            var sw = new StringWriter(sb);

            using (var writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();

                writer.WritePropertyName("UserId");
                writer.WriteValue(userId);

                writer.WriteEndObject();
            }

            return sb.ToString();
        }

        public static string CreateTourneyMessage(string userId)
        {
            var sb = new StringBuilder();

            var sw = new StringWriter(sb);

            using (var writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();

                writer.WritePropertyName("UserId");
                writer.WriteValue(userId);

                writer.WritePropertyName("CompletedTourneyInfoRequest");
                writer.WriteValue(false);

                writer.WriteEndObject();
            }

            return sb.ToString();
        }

        public static string CreateInfoRequest(string userId)
        {
            var sb = new StringBuilder();

            var sw = new StringWriter(sb);

            using (var writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();

                writer.WritePropertyName("UserId");
                writer.WriteValue(userId);

                writer.WritePropertyName("GameType");
                writer.WriteValue(0);

                writer.WritePropertyName("LastRequestedUtcTime");
                writer.WriteValue($"{DateTime.UtcNow.Year}-{DateTime.UtcNow.Month}-{DateTime.UtcNow.Day}T{DateTime.UtcNow.ToString("HH:mm:ss.FFFF", new CultureInfo("en-US"))}");

                writer.WritePropertyName("UserDeviceType");
                writer.WriteValue(UserDeviceType);

                writer.WriteEndObject();
            }

            return sb.ToString();
        }
    }
}