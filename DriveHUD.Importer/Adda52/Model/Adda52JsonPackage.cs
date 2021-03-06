﻿//-----------------------------------------------------------------------
// <copyright file="Adda52JsonPackage.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Sfs2X.Entities.Data;
using Sfs2X.Protocol.Serialization;
using Sfs2X.Util;
using System;
using System.Linq;
using System.Reflection;

namespace DriveHUD.Importers.Adda52.Model
{
    internal sealed class Adda52JsonPackage
    {
        public Adda52PackageType PackageType { get; set; }

        public int RoomId { get; set; }

        public string JsonData { get; set; }

        public DateTime TimestampUtc { get; set; }

        private readonly static string[] validPrefixes = new string[] { "game", "mtt", "user" };

        public static bool TryParse(string jsonData, out Adda52JsonPackage package)
        {
            package = null;

            try
            {

                var packageTypeStartIndex = -1;

                foreach (var validPrefix in validPrefixes)
                {
                    packageTypeStartIndex = jsonData.IndexOf($"\"{validPrefix}.", StringComparison.OrdinalIgnoreCase) + 1;

                    if (packageTypeStartIndex > 1)
                    {
                        break;
                    }
                }

                if (packageTypeStartIndex <= 1)
                {
                    return false;
                }

                var packageTypeEndIndex = jsonData.IndexOf("\"", packageTypeStartIndex, StringComparison.OrdinalIgnoreCase);

                if (packageTypeEndIndex <= 0)
                {
                    return false;
                }

                var packageTypeText = jsonData.Substring(packageTypeStartIndex, packageTypeEndIndex - packageTypeStartIndex);

                Adda52PackageType packageType;

                switch (packageTypeText)
                {
                    case RoomData.Command:
                        packageType = Adda52PackageType.RoomData;
                        break;
                    case RoomSeatInfo.Command:
                        packageType = Adda52PackageType.SeatInfo;
                        break;
                    case Ante.Command:
                        packageType = Adda52PackageType.Ante;
                        break;
                    case Blinds.Command:
                        packageType = Adda52PackageType.Blinds;
                        break;
                    case Dealer.Command:
                        packageType = Adda52PackageType.Dealer;
                        break;
                    case UserAction.Command:
                        packageType = Adda52PackageType.UserAction;
                        break;
                    case GameStart.Command:
                        packageType = Adda52PackageType.GameStart;
                        break;
                    case RoundEnd.Command:
                        packageType = Adda52PackageType.RoundEnd;
                        break;
                    case CommunityCardInfo.Command:
                        packageType = Adda52PackageType.CommunityCard;
                        break;
                    case Winner.Command:
                        packageType = Adda52PackageType.Winner;
                        break;
                    case UncalledBet.Command:
                        packageType = Adda52PackageType.UncalledBet;
                        break;
                    case HoleCard.Command:
                        packageType = Adda52PackageType.HoleCard;
                        break;
                    case AccessToken.Command:
                        packageType = Adda52PackageType.AccessToken;
                        break;
                    case MTTInfo.Command:
                        packageType = Adda52PackageType.MTTInfo;
                        break;
                    case MTTTables.Command:
                        packageType = Adda52PackageType.MTTTables;
                        break;
                    case MTTPrizes.Command:
                        packageType = Adda52PackageType.MTTPrizes;
                        break;
                    default:
                        return false;
                }

                int roomId = 0;

                if (packageType != Adda52PackageType.AccessToken)
                {
                    var roomIdStartIndex = jsonData.IndexOf("\"r\":") + 4;

                    if (roomIdStartIndex <= 4)
                    {
                        return false;
                    }

                    var roomIdEndIndex = jsonData.IndexOf(",", roomIdStartIndex);

                    if (roomIdEndIndex <= 0)
                    {
                        return false;
                    }

                    var roomIdText = jsonData.Substring(roomIdStartIndex, roomIdEndIndex - roomIdStartIndex);

                    if (!int.TryParse(roomIdText, out roomId))
                    {
                        return false;
                    }
                }

                package = new Adda52JsonPackage
                {
                    PackageType = packageType,
                    RoomId = roomId,
                    JsonData = FormatJson(jsonData),
                    TimestampUtc = DateTime.UtcNow
                };

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool TryParse(byte[] bytes, out Adda52JsonPackage package)
        {
            package = null;

            if (bytes == null || bytes.Length == 0)
            {
                return false;
            }

            if (!TryParseJsonData(bytes, out string jsonData))
            {
                return false;
            }

            return TryParse(jsonData, out package);
        }

        public static bool TryParseJsonData(byte[] bytes, out string json)
        {
            json = null;

            if (bytes == null || bytes.Length < 4)
            {
                return false;
            }

            if (DefaultSFSDataSerializer.RunningAssembly == null)
            {
                DefaultSFSDataSerializer.RunningAssembly = Assembly.GetExecutingAssembly();
            }

            try
            {
                var compressed = bytes[0] == 0xA0;
                var byteArray = new ByteArray(bytes.Skip(3).ToArray());

                if (compressed)
                {
                    byteArray.Uncompress();
                }

                var sFSObject = SFSObject.NewFromBinaryData(byteArray);

                json = sFSObject.ToJson();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string FormatJson(string json)
        {
            return json != null ? json.Replace(":\"{", ":{").Replace("}\",", "},").Replace("\\\"", "\"") : json;
        }
    }
}