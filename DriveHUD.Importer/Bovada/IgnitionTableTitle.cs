//-----------------------------------------------------------------------
// <copyright file="IgnitionTableTitle.cs" company="Ace Poker Solutions">
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
using System;
using System.Text.RegularExpressions;

namespace DriveHUD.Importers.Bovada
{
    internal class IgnitionTableTitle
    {
        private const string PlayMoneyPrefix = "Play ";

        public IgnitionTableTitle(string title)
        {
            OriginalTitle = title;

            if (string.IsNullOrEmpty(title) ||
                title.StartsWith("#", StringComparison.InvariantCultureIgnoreCase) ||
                title.StartsWith("Tournament #", StringComparison.InvariantCultureIgnoreCase) ||
                title.IndexOf("Lobby", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                title.IndexOf("Ignition", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                title.IndexOf("Bodog", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                title.IndexOf("Bovada", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return;
            }

            try
            {
                // Zone title
                if (title.IndexOf("Zone Poker", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    IsZone = true;
                    return;
                }

                // Jackpot title
                if (title.StartsWith("Jackpot"))
                {
                    ParseJackpotTitle(title);
                    return;
                }

                var firstDash = title.IndexOf('-');
                var lastDash = title.LastIndexOf('-');

                // Tournament titles
                if (firstDash > 0 && lastDash > 0 && firstDash != lastDash)
                {
                    var gameTypeWithStacks = title.Substring(0, firstDash).Trim();
                    gameTypeWithStacks = gameTypeWithStacks.Replace(PlayMoneyPrefix, string.Empty);

                    var regex = new Regex(@"(?<stacks>[^\s,]+),?(?<ante>\s\d+\sAnte|\s)(?<gametype>.*)");
                    var gameTypeWithStacksMatch = regex.Match(gameTypeWithStacks);

                    if (gameTypeWithStacksMatch.Success)
                    {
                        Stacks = gameTypeWithStacksMatch.Groups["stacks"].Value;
                        GameType = gameTypeWithStacksMatch.Groups["gametype"].Value.Trim();
                        Ante = gameTypeWithStacksMatch.Groups["ante"].Value.Replace(" Ante", string.Empty).Trim();
                    }

                    TableName = title.Substring(firstDash + 1, lastDash - firstDash - 1).Trim();
                    TableIdText = title.Substring(lastDash + 1).Trim();

                    var sharpIndex = TableIdText.IndexOf("#");
                    TableId = TableIdText.Substring(sharpIndex + 1);

                    IsValid = true;

                    return;
                }

                // Play money
                if (title.StartsWith(PlayMoneyPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    IsPlayMoney = true;
                    title = title.Replace(PlayMoneyPrefix, string.Empty).Trim();
                }

                // Cash titles
                var firstSpace = title.IndexOf(' ');

                if (firstSpace > 0)
                {
                    Stacks = title.Substring(0, firstSpace).Trim();

                    if (Stacks.Contains("/"))
                    {
                        GameType = title.Substring(firstSpace).Trim();
                        IsValid = true;
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Title '{title}' has not been parsed [Ignition]", e);
            }
        }

        public string OriginalTitle
        {
            get;
            private set;
        }

        public bool IsValid
        {
            get;
            private set;
        }

        public string GameType
        {
            get;
            private set;
        }

        public string TableName
        {
            get;
            private set;
        }

        public string TableIdText
        {
            get;
            private set;
        }

        public string TableId
        {
            get;
            private set;
        }

        public string TournamentId
        {
            get;
            private set;
        }

        public bool IsTournament
        {
            get
            {
                return TableId != null && TableId.Length < 5 || TournamentId != null;
            }
        }

        public bool IsPlayMoney
        {
            get;
            private set;
        }

        public string Stacks
        {
            get;
            private set;
        }

        public string Ante
        {
            get;
            private set;
        }

        public bool IsJackpot
        {
            get;
            private set;
        }

        public bool IsZone
        {
            get;
            private set;
        }

        private void ParseJackpotTitle(string title)
        {
            var tournamentWordIndex = title.IndexOf("Tournament", StringComparison.OrdinalIgnoreCase);

            if (tournamentWordIndex < 0)
            {
                return;
            }

            TournamentId = title.Substring(tournamentWordIndex + 11).Trim();

            var dollarSignIndex = title.IndexOf("$", StringComparison.OrdinalIgnoreCase);

            if (dollarSignIndex < 0)
            {
                return;
            }

            var spaceAfterDollarSign = title.IndexOf(" ", dollarSignIndex);

            if (spaceAfterDollarSign < 0)
            {
                return;
            }

            TableName = title.Substring(0, spaceAfterDollarSign);

            var spaceBeforeStacks = title.IndexOf(" ", spaceAfterDollarSign);

            if (spaceBeforeStacks < 0)
            {
                return;
            }

            var spaceAfterStacks = title.IndexOf(" ", spaceBeforeStacks + 1);

            if (spaceAfterStacks < 0)
            {
                return;
            }

            Stacks = title.Substring(spaceBeforeStacks + 1, spaceAfterStacks - spaceBeforeStacks - 1);

            GameType = "No limit Hold'em";
            TableId = "1";
            IsJackpot = true;

            IsValid = true;
        }
    }
}