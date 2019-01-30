//-----------------------------------------------------------------------
// <copyright file="HandHistoryParserFactoryImpl.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
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
using DriveHUD.Entities;
using HandHistories.Parser.Parsers.Base;
using HandHistories.Parser.Parsers.FastParser._888;
using HandHistories.Parser.Parsers.FastParser.Common;
using HandHistories.Parser.Parsers.FastParser.IPoker;
using HandHistories.Parser.Parsers.FastParser.PartyPoker;
using HandHistories.Parser.Parsers.FastParser.PokerStars;
using HandHistories.Parser.Parsers.FastParser.Horizon;
using HandHistories.Parser.Parsers.FastParser.Winning;
using HandHistories.Parser.Utils.Extensions;
using System;
using HandHistories.Parser.Parsers.FastParser.Winamax;

namespace HandHistories.Parser.Parsers.Factory
{
    public class HandHistoryParserFactoryImpl : IHandHistoryParserFactory
    {
        public EnumPokerSites LastSelected { get; set; }

        public IHandHistoryParser GetFullHandHistoryParser(string handText)
        {
            if (EnumPokerSitesExtension.TryParse(handText, out EnumPokerSites siteName))
            {
                LastSelected = siteName;

                var parser = GetFullHandHistoryParser(siteName, handText);
                return parser;
            }

            throw new DHBusinessException(new NonLocalizableString("Unknown hand format"));
        }

        public IHandHistoryParser GetFullHandHistoryParser(EnumPokerSites siteName, string handText = null)
        {
            switch (siteName)
            {
                case EnumPokerSites.PartyPoker:
                    return new PartyPokerFastParserImpl(siteName);
                case EnumPokerSites.PokerStars:
                    return new PokerStarsFastParserImpl(siteName);
                case EnumPokerSites.IPoker:
                    return new IPokerFastParserImpl();
                case EnumPokerSites.Bodog:
                case EnumPokerSites.Ignition:
                case EnumPokerSites.Bovada:
                case EnumPokerSites.BetOnline:
                case EnumPokerSites.SportsBetting:
                case EnumPokerSites.TigerGaming:
                case EnumPokerSites.SpartanPoker:
                    return new IPokerBovadaFastParserImpl();
                case EnumPokerSites.Poker888:
                    return new Poker888FastParserImpl();
                case EnumPokerSites.WinningPokerNetwork:
                case EnumPokerSites.AmericasCardroom:
                case EnumPokerSites.BlackChipPoker:
                case EnumPokerSites.TruePoker:
                case EnumPokerSites.YaPoker:

                    if (handText != null &&
                        (handText.StartsWith("<Game Information>", StringComparison.OrdinalIgnoreCase) ||
                            handText.StartsWith("Game Hand", StringComparison.OrdinalIgnoreCase)))
                    {
                        return new WinningPokerSnG2FastParserImpl();
                    }

                    return new WinningPokerNetworkFastParserImpl();
                case EnumPokerSites.GGN:
                case EnumPokerSites.PokerMaster:
                case EnumPokerSites.PokerKing:
                case EnumPokerSites.Adda52:
                case EnumPokerSites.PPPoker:
                case EnumPokerSites.PokerBaazi:
                    return new CommonHandHistoryParser(siteName);
                case EnumPokerSites.Horizon:
                    return new HorizonFastParserImpl();
                case EnumPokerSites.Winamax:
                    return new WinamaxFastParserImpl();
                default:
                    throw new NotImplementedException("GetFullHandHistoryParser: No parser for " + siteName);
            }
        }
    }
}