//-----------------------------------------------------------------------
// <copyright file="ConverterUtils.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Resources;
using DriveHUD.Importers.GGNetwork.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Importers.GGNetwork
{
    internal class ConverterUtils
    {       
        /// <summary>        
        /// </summary>
        /// <param name="cardKind"></param>
        /// <returns></returns>
        public static char GetCardSuitAsChar(CardKind cardKind)
        {
            switch (cardKind)
            {
                case CardKind.Club:
                    return 'c';
                case CardKind.Diamond:
                    return 'd';
                case CardKind.Heart:
                    return 'h';
                case CardKind.Spade:
                    return 's';
                default:
                    throw new ArgumentOutOfRangeException(nameof(cardKind), cardKind, null);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public static CardInfo GetCardInfoByOrdinal(int ordinal)
        {
            return BoardCards.CardList.FirstOrDefault(cardInfo => cardInfo.Ordinal == ordinal);
        }

        /// <summary>
        /// </summary>
        /// <param name="players"></param>
        /// <returns></returns>
        public static int GetDealerPosition(IList<Player> players)
        {
            foreach (var player in players)
            {
                var positionType = (PlayerPositionType)player.PositionType;

                if (positionType.HasFlag(PlayerPositionType.Dealer))
                {
                    var seatIndex = player.SeatIndex;

                    return ++seatIndex;
                }
            }

            return int.MinValue;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<HandHistories.Objects.GameDescription.TableTypeDescription> GetTableTypeDescription()
        {
            return new List<HandHistories.Objects.GameDescription.TableTypeDescription>
            {
                HandHistories.Objects.GameDescription.TableTypeDescription.Regular
            };
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static HandHistories.Objects.GameDescription.TableType GetTableType()
        {
            return new HandHistories.Objects.GameDescription.TableType(GetTableTypeDescription());
        }       
    }
}