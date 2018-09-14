//-----------------------------------------------------------------------
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

using System;
using System.Text;

namespace DriveHUD.Importers.Adda52.Model
{
    internal class Adda52JsonPackage
    {
        public Adda52PackageType PackageType { get; set; }

        public string JsonData { get; set; }

        #region Parsing

        private const string Blinds = "game.Blinds";
        private const string Dealer = "game.Dealer";
        private const string UserAction = "game.useraction";
        private const string GameEnd = "game.roundend";
        private const string UncalledBet = "game.returnPotMoney";
        private const string CommunityCard = "game.communitycard";
        private const string Pot = "game.pot";
        private const string Winner = "game.winner";
        private const string GameStart = "game.started";

        public static bool TryParse(byte[] bytes, out Adda52JsonPackage package)
        {
            package = null;

            if (bytes == null || bytes.Length == 0)
            {
                return false;
            }

            try
            {
                var jsonData = Encoding.UTF8.GetString(bytes);

                var packageTypeStartIndex = jsonData.IndexOf("\"game.", StringComparison.OrdinalIgnoreCase) + 1;

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
                    case Blinds:
                        packageType = Adda52PackageType.Blinds;
                        break;
                    case Dealer:
                        packageType = Adda52PackageType.Dealer;
                        break;
                    case UserAction:
                        packageType = Adda52PackageType.UserAction;
                        break;
                    case GameStart:
                        packageType = Adda52PackageType.GameStart;
                        break;
                    case GameEnd:
                        packageType = Adda52PackageType.GameEnd;
                        break;
                    case CommunityCard:
                        packageType = Adda52PackageType.CommunityCard;
                        break;
                    case Pot:
                        packageType = Adda52PackageType.Pot;
                        break;
                    case Winner:
                        packageType = Adda52PackageType.Winner;
                        break;
                    case UncalledBet:
                        packageType = Adda52PackageType.UncalledBet;
                        break;
                    default:
                        return false;
                }

                package = new Adda52JsonPackage
                {
                    PackageType = packageType,
                    JsonData = jsonData
                };

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion 
    }
}