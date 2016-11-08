//-----------------------------------------------------------------------
// <copyright file="Pacific888Importer.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using HandHistories.Parser.Parsers;

namespace DriveHUD.Importers.Pacific888
{
    internal class Pacific888Importer : FileBasedImporter, IPacific888Importer
    {
        protected override string ProcessName
        {
            get
            {
                return "poker";
            }
        }

        protected override string HandHistoryFilter
        {
            get
            {
                return "*.txt";
            }
        }

        public override string Site
        {
            get
            {
                return EnumPokerSites.Poker888.ToString();
            }
        }

        private const string tournamentPattern = "#{0} Table {1}";

        protected override bool Match(string title, ParsingResult parsingResult)
        {
            if (string.IsNullOrWhiteSpace(title) || parsingResult == null || !parsingResult.WasImported ||
                parsingResult.Source == null || parsingResult.Source.GameDescription == null)
            {
                return false;
            }

            if (parsingResult.Source.GameDescription.IsTournament)
            {
                var tournamentTitle = string.Format(tournamentPattern, parsingResult.Source.GameDescription.Tournament.TournamentId, parsingResult.Source.TableName);

                return title.Contains(tournamentTitle);
            }

            return title.Contains(parsingResult.Source.TableName);
        }
    }
}