//-----------------------------------------------------------------------
// <copyright file="PokerBaaziPackage.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using System;

namespace DriveHUD.Importers.PokerBaazi.Model
{
    internal sealed class PokerBaaziPackage
    {
        public PokerBaaziPackageType PackageType { get; set; }

        public string JsonData { get; set; }

        public uint RoomId { get; set; }

        public static bool TryParse(string data, out PokerBaaziPackage package)
        {
            package = null;

            var jsonData = ExtractJson(data);

            var packageType = ParsePackageType(jsonData);

            package = new PokerBaaziPackage
            {
                PackageType = packageType,
                JsonData = jsonData,
                RoomId = ParseRoomId(data)
            };

            return packageType != PokerBaaziPackageType.Unknown;
        }

        public static PokerBaaziPackageType ParsePackageType(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return PokerBaaziPackageType.Unknown;
            }

            try
            {
                var startIndex = 0;
                var actionStartIndex = -1;
                var bracersCounter = 0;

                do
                {
                    actionStartIndex = data.IndexOf("\"action\":", startIndex, StringComparison.OrdinalIgnoreCase);

                    for (var i = startIndex; i < actionStartIndex; i++)
                    {
                        if (data[i] == '{')
                        {
                            bracersCounter++;
                        }
                        else if (data[i] == '}')
                        {
                            bracersCounter--;
                        }
                    }

                    startIndex = actionStartIndex + 8;

                } while (bracersCounter > 2 && actionStartIndex > 0);

                if (actionStartIndex < 0)
                {
                    return PokerBaaziPackageType.Unknown;
                }

                actionStartIndex = data.IndexOf("\"", actionStartIndex + 9, StringComparison.OrdinalIgnoreCase);

                if (actionStartIndex < 0)
                {
                    return PokerBaaziPackageType.Unknown;
                }

                var actionEndIndex = data.IndexOf("\"", actionStartIndex + 1, StringComparison.OrdinalIgnoreCase);

                if (actionEndIndex < 0)
                {
                    return PokerBaaziPackageType.Unknown;
                }

                var actionText = data.Substring(actionStartIndex + 1, actionEndIndex - actionStartIndex - 1);

                switch (actionText)
                {
                    case "SpectatorResponse":
                        return PokerBaaziPackageType.SpectatorResponse;
                    case "InitResponse":
                        return PokerBaaziPackageType.InitResponse;
                    case "RoundResponse":
                        return PokerBaaziPackageType.RoundResponse;
                    case "UserButtonActionResponse":
                        return PokerBaaziPackageType.UserButtonActionResponse;
                    case "WinnerResponse":
                        return PokerBaaziPackageType.WinnerResponse;
                    default:
                        return PokerBaaziPackageType.Unknown;
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(typeof(PokerBaaziPackage), $"Failed to parse PokerBaazi package type from: {Environment.NewLine}{data}", e);
                return PokerBaaziPackageType.Unknown;
            }
        }

        private static uint ParseRoomId(string data)
        {
            var roomId = 0u;

            if (string.IsNullOrEmpty(data))
            {
                return roomId;
            }

            var roomIdEndIndex = data.IndexOf(",", StringComparison.OrdinalIgnoreCase);

            if (roomIdEndIndex < 0)
            {
                return roomId;
            }

            var roomIdStartIndex = data.LastIndexOf("/", roomIdEndIndex, StringComparison.OrdinalIgnoreCase);

            if (roomIdStartIndex < 0 || (roomIdEndIndex - roomIdStartIndex) < 2)
            {
                return roomId;
            }

            var roomIdText = data.Substring(roomIdStartIndex + 1, roomIdEndIndex - roomIdStartIndex - 1);

            uint.TryParse(roomIdText, out roomId);

            return roomId;
        }

        private static string ExtractJson(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }

            var jsonStartIndex = data.IndexOf("\"{", StringComparison.OrdinalIgnoreCase);
            var jsonEndIndex = data.LastIndexOf("}\"", StringComparison.OrdinalIgnoreCase);

            if (jsonStartIndex < 0 || jsonEndIndex < 0 || (jsonEndIndex - jsonStartIndex) < 1)
            {
                return null;
            }

            var jsonText = data.Substring(jsonStartIndex + 1, jsonEndIndex - jsonStartIndex).Replace("\\\"", "\"");

            return jsonText;
        }
    }
}