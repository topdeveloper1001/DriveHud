//-----------------------------------------------------------------------
// <copyright file="CommonHandHistoryParser.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using DriveHUD.Common.Log;
using DriveHUD.Entities;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.Base;
using HandHistories.Parser.Utils.FastParsing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HandHistories.Parser.Parsers.FastParser.Common
{
    public class CommonHandHistoryParser : IHandHistoryParser
    {
        public CommonHandHistoryParser(EnumPokerSites site)
        {
            SiteName = site;
        }

        public EnumPokerSites SiteName
        {
            get;
            private set;
        }

        public HandHistory ParseFullHandHistory(string handText, bool rethrowExceptions = false)
        {
            if (string.IsNullOrEmpty(handText))
            {
                throw new ArgumentNullException(nameof(handText));
            }

            try
            {
                var handHistory = SerializationHelper.DeserializeObject<HandHistory>(handText);

                if (!string.IsNullOrEmpty(handHistory.HeroName))
                {
                    handHistory.Hero = handHistory.Players.FirstOrDefault(x => x.PlayerName.Equals(handHistory.HeroName));
                }

                handHistory.FullHandHistoryText = handText;

                return handHistory;
            }
            catch (Exception ex)
            {
                LogProvider.Log.Warn(this, string.Format("Couldn't parse hand {0} with error {1} and trace {2}", handText, ex.Message, ex.StackTrace));

                if (rethrowExceptions)
                {
                    throw;
                }

                var handHistory = new HandHistory
                {
                    Exception = ex
                };

                return handHistory;
            }
        }

        public IEnumerable<string> SplitUpMultipleHands(string rawHandHistories)
        {
            return ParserUtils.SplitUpMultipleHands(rawHandHistories);
        }

        #region Not implemented methods

        public string GetTournamentIdFromFileName(string filename)
        {
            throw new NotImplementedException();
        }

        public bool IsValidHand(string handText)
        {
            throw new NotImplementedException();
        }

        public bool IsValidOrCanceledHand(string handText, out bool isCancelled)
        {
            throw new NotImplementedException();
        }

        public BoardCards ParseCommunityCards(string handText)
        {
            throw new NotImplementedException();
        }

        public DateTime ParseDateUtc(string handText)
        {
            throw new NotImplementedException();
        }

        public int ParseDealerPosition(string handText)
        {
            throw new NotImplementedException();
        }

        public HandHistorySummary ParseFullHandSummary(string handText, bool rethrowExceptions = false)
        {
            throw new NotImplementedException();
        }

        public GameDescriptor ParseGameDescriptor(string handText)
        {
            throw new NotImplementedException();
        }

        public GameType ParseGameType(string handText)
        {
            throw new NotImplementedException();
        }     

        public long ParseHandId(string handText)
        {
            throw new NotImplementedException();
        }

        public Limit ParseLimit(string handText)
        {
            throw new NotImplementedException();
        }

        public int ParseNumPlayers(string handText)
        {
            throw new NotImplementedException();
        }

        public PlayerList ParsePlayers(string handText)
        {
            throw new NotImplementedException();
        }

        public SeatType ParseSeatType(string handText)
        {
            throw new NotImplementedException();
        }

        public string ParseTableName(string handText)
        {
            throw new NotImplementedException();
        }

        public TableType ParseTableType(string handText)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}