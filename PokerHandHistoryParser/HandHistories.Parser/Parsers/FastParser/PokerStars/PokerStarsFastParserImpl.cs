//-----------------------------------------------------------------------
// <copyright file="PokerStarsFastParserImpl.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using DriveHUD.Entities;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.Base;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Parsers.FastParser.Base;
using HandHistories.Parser.Utils.Extensions;
using HandHistories.Parser.Utils.FastParsing;
using HandHistories.Parser.Utils.Strings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace HandHistories.Parser.Parsers.FastParser.PokerStars
{
    internal sealed class PokerStarsFastParserImpl : HandHistoryParserFastImpl, IThreeStateParser
    {
        static readonly TimeZoneInfo PokerStarsTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        private const string tournamentSummaryHeader = "PokerStars Tournament #";

        private int GameIdStartIndex = 17;
        private int TournamentIdStartindex = 43;

        public override bool RequiresAdjustedRaiseSizes
        {
            get { return true; }
        }

        public override bool SupportRunItTwice
        {
            get { return true; }
        }

        public override bool RequiresBetWinAdjustment
        {
            get { return true; }
        }

        private readonly EnumPokerSites _siteName;

        public override EnumPokerSites SiteName
        {
            get { return _siteName; }
        }

        private readonly NumberFormatInfo _numberFormatInfo;
        private readonly bool _throwExceptionOnError;

        // So the same parser can be used for It and Fr variations
        public PokerStarsFastParserImpl(EnumPokerSites siteName = EnumPokerSites.PokerStars, bool throwExceptionOnError = false)
        {
            _siteName = siteName;
            _throwExceptionOnError = throwExceptionOnError;

            _numberFormatInfo = new NumberFormatInfo
            {
                NegativeSign = "-",
                CurrencyDecimalSeparator = ".",
                CurrencyGroupSeparator = ",",
                CurrencySymbol = "$"
            };
        }

        private static readonly Regex HandSplitRegex = new Regex("(PokerStars Game #)|(PokerStars Hand #)|(PokerStars Zoom Hand #)", RegexOptions.Compiled);

        public override IEnumerable<string> SplitUpMultipleHands(string rawHandHistories)
        {
            var handLines = SplitHandsLines(rawHandHistories);

            if (IsSummaryHand(handLines))
            {
                return new[] { rawHandHistories };
            }

            rawHandHistories = rawHandHistories.Replace("\r", "");

            return rawHandHistories.LazyStringSplit("\n\n").Where(s => string.IsNullOrWhiteSpace(s) == false && s.Equals("\r\n") == false);
        }

        public override IEnumerable<string[]> SplitUpMultipleHandsToLines(string rawHandHistories)
        {
            var allLines = rawHandHistories.LazyStringSplitFastSkip('\n', jump: 10, jumpAfter: 2);

            List<string> handLines = new List<string>(50);

            foreach (var item in allLines)
            {
                string line = item.TrimEnd('\r', ' ');

                if (string.IsNullOrWhiteSpace(line))
                {
                    if (handLines.Count > 0)
                    {
                        yield return handLines.ToArray();
                        handLines = new List<string>(50);
                    }
                    continue;
                }
                handLines.Add(line);
            }

            if (handLines.Count > 0)
            {
                yield return handLines.ToArray();
            }
        }

        protected override int ParseDealerPosition(string[] handLines)
        {
            // Expect the 2nd line to look like this:
            // Table 'Alemannia IV' 6-max Seat #2 is the button
            string line = handLines[1];

            int startIndex = line.LastIndexOf('#') + 1;

            return FastInt.Parse(line, startIndex);
        }

        protected override DateTime ParseDateUtc(string[] handLines)
        {
            // Expect the first line to look like this: 
            // PokerStars Hand #78453197174:  Hold'em No Limit ($0.08/$0.16 USD) - 2012/04/06 20:56:40 ET

            // or

            // PokerStars Game #61777648755:  Hold'em No Limit ($0.50/$1.00 USD) - 2011/05/06 20:51:38 PT [2011/05/06 23:51:38 ET]

            string line = handLines[0];

            var regex = new Regex(@"(?<year>\d{4})/(?<month>\d{2})/(?<day>\d{2}) (?<hour>\d{1,2}):(?<minute>\d{2}):(?<second>\d{2})");

            var match = regex.Match(line);

            if (!match.Success)
            {
                throw new ParseHandDateException("Date couldn't be recognized.", line);
            }

            var dateString = match.Value;

            // DateString is one of:
            // 2012/04/07 2:58:27
            // 2012/04/07 18:58:27

            int year = FastInt.Parse(dateString);
            int month = FastInt.Parse(dateString, 5);
            int day = FastInt.Parse(dateString, 8);

            int hour = FastInt.Parse(dateString, 11);

            int minuteStartIndex = dateString.IndexOf(':', 12) + 1;

            int minute = FastInt.Parse(dateString, minuteStartIndex);

            int second = FastInt.Parse(dateString, minuteStartIndex + 3);

            DateTime dateTime = new DateTime(year, month, day, hour, minute, second);

            DateTime converted = TimeZoneInfo.ConvertTimeToUtc(dateTime, PokerStarsTimeZone);

            return DateTime.SpecifyKind(converted, DateTimeKind.Utc);
        }

        protected override void ParseExtraHandInformation(string[] handLines, HandHistorySummary handHistorySummary)
        {
            if (handHistorySummary.Cancelled)
            {
                return;
            }

            for (int i = handLines.Length - 1; i >= 0; i--)
            {
                string line = handLines[i];

                // Check for summary line:
                //  *** SUMMARY ***
                if (line[0] == '*' && line[4] == 'S')
                {
                    // Line after summary line is:
                    //  Total pot $13.12 | Rake $0.59 
                    // or
                    //  Total pot $62.63 Main pot $54.75. Side pot $5.38. | Rake $2.50 
                    string totalLine = handLines[i + 1];

                    int lastSpaceIndex = totalLine.LastIndexOf(' ');
                    int spaceAfterFirstNumber = totalLine.IndexOf(' ', 11);

                    string rake = totalLine.Substring(lastSpaceIndex + 1, totalLine.Length - lastSpaceIndex - 1);

                    handHistorySummary.Rake = decimal.Parse(rake, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, _numberFormatInfo);

                    string totalPot = totalLine.Substring(10, spaceAfterFirstNumber - 10);

                    handHistorySummary.TotalPot = decimal.Parse(totalPot, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, _numberFormatInfo);

                    break;
                }
            }
        }

        protected override PokerFormat ParsePokerFormat(string[] handLines)
        {
            var tournamentStartIndex = handLines[0].IndexOf("Tournament #");

            if (tournamentStartIndex > 0)
            {
                TournamentIdStartindex = tournamentStartIndex + 12;

                // this can actually also be a MTT
                return PokerFormat.Tournament;
            }
            return PokerFormat.CashGame;
        }

        protected override long ParseHandId(string[] handLines)
        {
            // Expect the first line to look like this: 
            //   PokerStars Hand #78453197174:  Hold'em No Limit ($0.08/$0.16 USD) - 2012/04/06 20:56:40 ET
            // or
            //   PokerStars Game #PokerStars Zoom Hand #84414134468:  Omaha Pot Limit ($0.05/$0.10 USD) - 2012/08/07 14:40:01 ET            
            // Zoom format   
            //   PokerStars Zoom Hand #132630000000:
            // Sng 
            //   PokerStars Hand #121732531381: Tournament #974085159, $5.20+$1.30+$0.50 USD Hold'em No Limit - Level IV (50/100) - 2014/09/18 16:58:15 ET
            //   PokerStars Game #121732531381: Tournament #974085159, $5.20+$1.30+$0.50 USD Hold'em No Limit - Level IV (50/100) - 2014/09/18 16:58:15 ET
            const int zoomHandIdStartIndex = 22;//"PokerStars Zoom Hand #".Length

            string line = handLines[0];

            int firstDigitIndex;// = handLines[0][38] == '#' ? 39 : 17;

            char handIDchar = line[11];
            switch (handIDchar)
            {
                case 'Z':
                    firstDigitIndex = zoomHandIdStartIndex;
                    break;
                case 'H':
                    firstDigitIndex = GameIdStartIndex;
                    break;
                default:
                    if (line[TournamentIdStartindex - 1] == '#')
                    {
                        firstDigitIndex = line.IndexOf('#') + 1;
                    }
                    else
                    {
                        firstDigitIndex = line.LastIndexOf('#') + 1;
                    }
                    break;
            }

            int lastDigitIndex = line.IndexOf(':');

            string handId = line.Substring(firstDigitIndex, lastDigitIndex - firstDigitIndex);
            return long.Parse(handId);
        }

        protected override TournamentDescriptor ParseTournament(string[] handLines)
        {
            //   PokerStars Hand #121732531381: Tournament #974085159, $5.20+$1.30+$0.50 USD Hold'em No Limit - Level IV (50/100) - 2014/09/18 16:58:15 ET
            var endIndex = handLines[0].IndexOf(",", TournamentIdStartindex, StringComparison.Ordinal);

            var tournamentId = long.Parse(handLines[0].Substring(TournamentIdStartindex, endIndex - TournamentIdStartindex));

            var buyin = ParseBuyin(handLines);

            var tournamentDescriptor = new TournamentDescriptor
            {
                TournamentId = tournamentId.ToString(),
                Speed = TournamentSpeed.Regular,
                TournamentName = string.Format("Tournament #{0}", tournamentId),
                BuyIn = buyin
            };

            return tournamentDescriptor;
        }

        protected override bool IsSummaryHand(string[] handLines)
        {
            return handLines.Length > 0 && handLines.Take(10).Any(x => x.StartsWith(tournamentSummaryHeader, StringComparison.InvariantCultureIgnoreCase));
        }

        private const string summaryYouFinishedInText = "You finished in ";
        private const string summaryBuyInText = "Buy-In: ";
        private static readonly Regex summaryPlayerDataRegex = new Regex(@"\d+:\s(?<player>.*[^\]]+)(\s\[\d+\])?\s\([^\)]+\),(\s(?<won>[^\(]+).*)?", RegexOptions.Compiled);

        protected override HandHistory ParseSummaryHand(string[] handLines, HandHistory handHistory)
        {
            var tournament = new TournamentDescriptor
            {
                Summary = string.Join(Environment.NewLine, handLines)
            };

            var handLineIndex = 0;

            foreach (var handLine in handLines)
            {
                handLineIndex++;

                var tournamentIndex = handLine.IndexOf(tournamentSummaryHeader, StringComparison.InvariantCultureIgnoreCase);

                // parse tournament id
                if (tournamentIndex >= 0)
                {
                    var indexOfComma = handLine.IndexOf(",", tournamentIndex);
                    tournament.TournamentId = handLine.Substring(tournamentIndex + tournamentSummaryHeader.Length, indexOfComma - tournamentIndex - tournamentSummaryHeader.Length);
                    break;
                }
                else if (string.IsNullOrEmpty(tournament.TournamentId))
                {
                    continue;
                }
            }

            if (string.IsNullOrEmpty(tournament.TournamentId))
            {
                return handHistory;
            }

            // parse place from the end
            for (var i = handLines.Length - 1; i > 0; i--)
            {
                if (handLines[i].StartsWith(summaryYouFinishedInText, StringComparison.InvariantCultureIgnoreCase))
                {
                    var indexOfNextSpace = handLines[i].IndexOf(" ", summaryYouFinishedInText.Length);
                    var placeText = handLines[i].Substring(summaryYouFinishedInText.Length, indexOfNextSpace - summaryYouFinishedInText.Length - 2);

                    short place = 0;

                    if (!short.TryParse(placeText, out place))
                    {
                        LogProvider.Log.Error(string.Format("'{0}' place wasn't parsed", handLines[i]));
                        break;
                    }

                    tournament.FinishPosition = place;
                    break;
                }
            }

            // parse remaining data
            foreach (var handLine in handLines.Skip(handLineIndex - 1))
            {
                // parse total players
                if (tournament.TotalPlayers == 0 && handLine.IndexOf("players", StringComparison.InvariantCultureIgnoreCase) > 0)
                {
                    var totalPlayersText = new string(handLine.TakeWhile(x => x != ' ').ToArray());

                    short totalPlayers = 0;

                    if (!short.TryParse(totalPlayersText, out totalPlayers))
                    {
                        LogProvider.Log.Error(string.Format("'{0}' place wasn't parsed", handLine));
                        continue;
                    }

                    tournament.TotalPlayers = totalPlayers;
                }

                // parse buyin
                if (tournament.BuyIn == null && handLine.StartsWith(summaryBuyInText, StringComparison.InvariantCultureIgnoreCase))
                {
                    // Buy-In: $24.55/$2.45 USD
                    // Buy-In: 250 SC
                    var buyInText = handLine.Substring(summaryBuyInText.Length);

                    var indexOfLastSpace = buyInText.LastIndexOf(' ');

                    if (indexOfLastSpace > 0)
                    {
                        buyInText = buyInText.Substring(0, indexOfLastSpace);
                    }

                    var buyInTextSplit = buyInText.Split('/');

                    if (buyInTextSplit.Length != 2)
                    {
                        tournament.BuyIn = Buyin.FromBuyinRake(0, 0, Currency.All);
                        continue;
                    }
                    
                    var currency = ParseCurrency(handLine, buyInText[0]);

                    decimal buyIn = 0;
                    decimal rake = 0;
                    

                    ParserUtils.TryParseMoney(buyInTextSplit[0], out buyIn);
                    ParserUtils.TryParseMoney(buyInTextSplit[1], out rake);

                    tournament.BuyIn = Buyin.FromBuyinRake(buyIn, rake, Currency.USD);
                    continue;
                }

                // parse player info
                if (handLine.StartsWith($"{tournament.FinishPosition}: ", StringComparison.InvariantCultureIgnoreCase))
                {
                    var match = summaryPlayerDataRegex.Match(handLine);

                    if (!match.Success)
                    {
                        LogProvider.Log.Error(string.Format("'{0}' wasn't parsed", handLine));
                        continue;
                    }

                    var playerName = match.Groups["player"].Value;
                    var wonText = match.Groups["won"] != null ? match.Groups["won"].Value.Replace(",", ".").Trim() : string.Empty;

                    decimal won = 0;                    

                    if (wonText.Count(x => x == '.') > 1)
                    {
                        var lastIndexOfDot = wonText.LastIndexOf(".");

                        var indexOfDot = wonText.IndexOf(".");

                        while (indexOfDot < lastIndexOfDot)
                        {
                            wonText = wonText.Remove(indexOfDot, 1);
                            indexOfDot = wonText.IndexOf(".", indexOfDot);
                            lastIndexOfDot--;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(wonText) && !ParserUtils.TryParseMoney(wonText, out won))
                    {
                        LogProvider.Log.Error(string.Format("'{0}' won data wasn't parsed", handLine));
                        continue;
                    }

                    handHistory.Hero = new Player(playerName, 0, 0);

                    tournament.Winning = won;
                }
            }

            handHistory.GameDescription = new GameDescriptor(EnumPokerSites.PokerStars,
               GameType.Unknown,
               null,
               TableType.FromTableTypeDescriptions(),
               SeatType.AllSeatType(),
               tournament);

            return handHistory;
        }

        protected override string ParseTableName(string[] handLines)
        {
            // Line two is in form:
            // Cash: Table 'Centaurus VIII' 6-max Seat #2 is the button
            // SNG:  Table '1147201581 7' 9-max Seat #9 is the button
            const int firstDashIndex = 7;
            int secondDash = handLines[1].LastIndexOf('\'');

            return handLines[1].Substring(firstDashIndex, secondDash - firstDashIndex);
        }

        protected override SeatType ParseSeatType(string[] handLines)
        {
            // line two looks like :
            // Table 'Alcor V' 6-max Seat #4 is the button
            int secondDash = handLines[1].LastIndexOf('\'');

            // 2-max, 6-max or 9-max
            int maxPlayers = FastInt.Parse(handLines[1][secondDash + 2], 1);

            // can't have 1max so must be 10max
            if (maxPlayers == 1)
            {
                maxPlayers = 10;
            }

            return SeatType.FromMaxPlayers(maxPlayers);
        }

        protected override GameType ParseGameType(string[] handLines)
        {
            var header = handLines[0];

            if (header.Contains("Hold'em No Limit"))
            {
                return GameType.NoLimitHoldem;
            }

            if (header.Contains("Hold'em Limit"))
            {
                return GameType.FixedLimitHoldem;
            }

            if (header.Contains("Hold'em Pot Limit"))
            {
                return GameType.PotLimitHoldem;
            }

            if (header.Contains("Omaha Hi/Lo No Limit"))
            {
                return GameType.NoLimitOmahaHiLo;
            }

            if (header.Contains("Limit Omaha Hi/Lo"))
            {
                return GameType.FixedLimitOmahaHiLo;
            }

            if (header.Contains("Omaha Hi/Lo Pot Limit"))
            {
                return GameType.PotLimitOmahaHiLo;
            }

            if (header.Contains("Omaha Pot Limit"))
            {
                return GameType.PotLimitOmaha;
            }

            if (header.Contains("Omaha Limit"))
            {
                return GameType.FixedLimitOmaha;
            }

            throw new UnrecognizedGameTypeException(handLines[0], "Unrecognized game-type: " + header);
        }

        protected override TableType ParseTableType(string[] handLines)
        {
            string line0 = handLines[0];
            string line1 = handLines[1];

            var format = ParsePokerFormat(handLines);

            // SNGs have no additional information in the hand history :-(
            // we could consider guessing the tabletype by taking a look at the buyin/rake/bounty/seats/gametype, but this isn't bulletproof
            if (format.Equals(PokerFormat.Tournament))
            {
                return TableType.FromTableTypeDescriptions(TableTypeDescription.Regular);
            }

            // Stars does not right out things such as speed/shallow/fast to hands right now.
            if (line1.Contains(" Zoom") || line0.Contains(" Zoom"))
            {
                return TableType.FromTableTypeDescriptions(TableTypeDescription.Zoom);
            }

            if (line1.Contains("100-250 bb"))
            {
                return TableType.FromTableTypeDescriptions(TableTypeDescription.Deep);
            }

            // older hand history files have the cap mark in the first line
            if (line1.LastIndexOf(" CAP", StringComparison.Ordinal) != -1 ||
               line0.LastIndexOf(" Cap ", StringComparison.Ordinal) != -1)
            {
                return TableType.FromTableTypeDescriptions(TableTypeDescription.Cap);
            }

            return TableType.FromTableTypeDescriptions(TableTypeDescription.Regular);
        }

        protected override Limit ParseLimit(string[] handLines)
        {
            // Expect the first line to look like:
            // Cash: PokerStars Hand #78441538809:  Hold'em Limit ($30/$60 USD) - 2012/04/06 16:45:19 ET
            // SNG:  PokerStars Hand #121732709812: Tournament #974092011, $55.56+$4.44 USD Hold'em No Limit - Level VI (100/200) - 2014/09/18 17:02:21 ET
            // for SNGs we use the current BlindLevel as Limit

            int startIndex = handLines[0].IndexOf('(', GameIdStartIndex) + 1;
            int lastIndex = handLines[0].IndexOf(')', startIndex) - 1;

            string limitSubstring = handLines[0].Substring(startIndex, lastIndex - startIndex + 1);

            // if the currencyIndex is Zero, we need to parse the Currency, otherwise we assume it's no defined currency
            Currency currency;
            try
            {
                currency = ParseCurrency(handLines[0], limitSubstring[0]);
            }
            catch (CurrencyException)
            {
                var format = ParsePokerFormat(handLines);
                if (format.Equals(PokerFormat.Tournament))
                {
                    currency = Currency.All;
                }
                else
                {
                    throw;
                }
            }

            int slashIndex = limitSubstring.IndexOf('/');
            int endIndex = limitSubstring.IndexOf(' ');
            if (endIndex == -1) endIndex = limitSubstring.IndexOf(')');
            if (endIndex == -1) endIndex = limitSubstring.Length - 1;

            string smallBlind = limitSubstring.Substring(0, slashIndex);
            string bigBlind = limitSubstring.Substring(slashIndex + 1, endIndex - (slashIndex + 1) + 1);

            decimal small = decimal.Parse(smallBlind, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, _numberFormatInfo);
            decimal big = decimal.Parse(bigBlind, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, _numberFormatInfo);


            // If it is an ante table we expect to see an ante line after the big blind
            // TODO: correctly implement antes
            // const decimal ante = 0;
            // const bool isAnte = false;

            return Limit.FromSmallBlindBigBlind(small, big, currency);
        }

        protected override Buyin ParseBuyin(string[] handLines)
        {
            // Expect the first line to look like:
            // PokerStars Hand #121723607468: Tournament #973955807, $2.30+$2.30+$0.40 USD Hold'em No Limit - Level XIII (600/1200)
            // PokerStars Hand #121732709812: Tournament #974092011, $55.56+$4.44 USD Hold'em No Limit - Level VI (100/200) - 2014/09/18 17:02:21 ET
            // this is obviously not needed for CashGame

            int startIndex = handLines[0].IndexOf(',', TournamentIdStartindex) + 2;
            int endIndex = handLines[0].IndexOf(' ', startIndex);

            string buyinSubstring = handLines[0].Substring(startIndex, endIndex - startIndex);

            var currency = ParseCurrency(handLines[0], buyinSubstring[0]);

            decimal prizePoolValue = 0;
            decimal rake = 0;
            decimal knockoutValue = 0m;

            var buyinSplit = buyinSubstring.Split('+');

            if (buyinSplit.Length == 3)
            {
                prizePoolValue = decimal.Parse(buyinSplit[0], NumberStyles.AllowCurrencySymbol | NumberStyles.Number, _numberFormatInfo);
                knockoutValue = decimal.Parse(buyinSplit[1], NumberStyles.AllowCurrencySymbol | NumberStyles.Number, _numberFormatInfo);
                rake = decimal.Parse(buyinSplit[2], NumberStyles.AllowCurrencySymbol | NumberStyles.Number, _numberFormatInfo);
            }
            else if (buyinSplit.Length == 2)
            {
                prizePoolValue = decimal.Parse(buyinSplit[0], NumberStyles.Currency, _numberFormatInfo);
                rake = decimal.Parse(buyinSplit[1], NumberStyles.AllowCurrencySymbol | NumberStyles.Number, _numberFormatInfo);
            }
            else if (_throwExceptionOnError)
            {
                throw new BuyinException(handLines[0], "Unrecognized Buyin Format");
            }

            return Buyin.FromBuyinRake(prizePoolValue, rake, currency, knockoutValue != 0m, knockoutValue);
        }

        private Currency ParseCurrency(string handLine, char currencySymbol)
        {
            switch (currencySymbol)
            {
                case '$':
                    _numberFormatInfo.CurrencySymbol = "$";
                    return Currency.USD;
                case '€':
                    _numberFormatInfo.CurrencySymbol = "€";
                    return Currency.EURO;
                case '£':
                    _numberFormatInfo.CurrencySymbol = "£";
                    return Currency.GBP;
                default:
                    Currency currency;

                    if (!TryParseCurrency(handLine, out currency) && _throwExceptionOnError)
                    {
                        throw new CurrencyException(handLine, "Unrecognized currency symbol " + currencySymbol);
                    }

                    return currency;
            }
        }

        private bool TryParseCurrency(string str, out Currency currency)
        {
            // Cash: PokerStars Hand #78441538809:  Hold'em Limit ($30/$60 USD) - 2012/04/06 16:45:19 ET
            // SNG:  PokerStars Hand #121732709812: Tournament #974092011, $55.56+$4.44 USD Hold'em No Limit - Level VI (100/200) - 2014/09/18 17:02:21 ET
            string currencyString = str.Substring(str.LastIndexOf(' ') + 1);

            switch (currencyString)
            {
                case "USD":
                    _numberFormatInfo.CurrencySymbol = "$";
                    currency = Currency.USD;
                    return true;
                case "GBP":
                    _numberFormatInfo.CurrencySymbol = "£";
                    currency = Currency.GBP;
                    return true;
                case "EUR":
                    _numberFormatInfo.CurrencySymbol = "€";
                    currency = Currency.EURO;
                    return true;
                default:
                    currency = Currency.All;
                    return false;
            }
        }

        public override bool IsValidHand(string[] handLines)
        {
            bool isCancelled; // in this case eat it
            return IsValidOrCancelledHand(handLines, out isCancelled);
        }

        public override bool IsValidOrCancelledHand(string[] handLines, out bool isCancelled)
        {
            isCancelled = false;

            for (int i = handLines.Length - 1; i > 0; i--)
            {
                string line = handLines[i];

                if (line.StartsWith("*** SU", StringComparison.Ordinal)) // if its the summary line
                {
                    // actually 'Hand canceled' can be in any line between line 2 and i-1
                    for (int k = i - 1; k >= 2; k--)
                    {
                        var cancelledLine = handLines[k];
                        bool cancelled = (cancelledLine[0] == 'H' && cancelledLine[cancelledLine.Length - 1] == 'd' && cancelledLine[cancelledLine.Length - 2] == 'e');

                        if (cancelled)
                        {
                            isCancelled = true;
                            break;
                        }
                    }

                    return true;
                }
            }

            // doesn't contain a summary line
            return false;
        }

        protected override List<HandAction> ParseHandActions(string[] handLines, GameType gameType = GameType.Unknown)
        {
            // actions take place from the last seat info until the *** SUMMARY *** line            

            int actionIndex = GetFirstActionIndex(handLines);

            List<HandAction> handActions = new List<HandAction>(handLines.Length - actionIndex);

            actionIndex = ParseBlindActions(handLines, ref handActions, actionIndex);

            Street currentStreet;

            actionIndex = ParseGameActions(handLines, ref handActions, actionIndex, out currentStreet);

            if (currentStreet == Street.Showdown)
            {
                ParseShowDown(handLines, ref handActions, actionIndex, gameType);
            }

            return handActions;
        }

        static bool IsJoinTableLine(string line)
        {
            int length = line.Length;
            return line[length - 2] == '#' || line[length - 3] == '#';
        }

        static bool IsRebuyLine(string line)
        {
            // solopercy1 re-buys and receives 1500 chips for $3.19
            var lastSpaceIndex = line.LastIndexOf(' ');

            return (line[lastSpaceIndex - 1] == 'r' && line[lastSpaceIndex - 4] == ' ' && line[lastSpaceIndex - 5] == 's');
        }

        /// <summary>
        /// Parse all blind actions from the specified index, returns the index where HandActions will start
        /// </summary>
        /// <param name="handLines"></param>
        /// <param name="handActions"></param>
        /// <param name="firstActionIndex"></param>
        /// <returns>Index where HandActions will Start</returns>
        public int ParseBlindActions(string[] handLines, ref List<HandAction> handActions, int firstActionIndex)
        {
            // required for distinction between smallblind/bigblind/posts 
            var smallBlind = false;
            var bigBlind = false;

            for (int i = firstActionIndex; i < handLines.Length; i++)
            {
                var line = handLines[i];

                var lastChar = line[line.Length - 1];

                switch (lastChar)
                {
                    //blind actions(BB, SB ANTE) may end in a number during the blinds
                    //reto27 joins the table at seat #3
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        if (IsJoinTableLine(line))
                        {
                            continue;
                        }
                        break;

                    case 'n':
                        // Gaby66916: posts big blind $0.25 and is all-in <--- this is okay
                        if (line[line.Length - 2] == 'o' || line[line.Length - 3] == 'o')
                            continue;
                        break;

                    //*** HOLE CARDS ***
                    case '*':
                        return i + 1;

                    //Draw for dealer [Qs]
                    //*** FLOP *** [6d 7c 6h]
                    //*** TURN *** [6d 7c 6h] [2s]
                    //*** RIVER *** [6d 7c 6h 2s] [Qc]
                    case ']':

                        //Draw for dealer [Qs]
                        if (line[line.Length - 6] == 'r')
                        {
                            continue;
                        }

                        throw new HandActionException(string.Join(Environment.NewLine, handLines), "Unexpected Line: " + line);

                    default:
                        continue;
                }

                int colonIndex = line.LastIndexOf(':');

                if (colonIndex > -1)
                {
                    var action = ParsePostingActionLine(line, colonIndex, smallBlind, bigBlind);

                    if (action != null)
                    {
                        if (action.HandActionType == HandActionType.SMALL_BLIND) smallBlind = true;
                        if (action.HandActionType == HandActionType.BIG_BLIND) bigBlind = true;

                        handActions.Add(action);
                    }

                }
            }
            throw new HandActionException(string.Join(Environment.NewLine, handLines), "No end of posting actions");
        }

        public void ParseShowDown(string[] handLines, ref List<HandAction> handActions, int actionIndex, GameType gameType)
        {
            for (int i = actionIndex; i < handLines.Length; i++)
            {
                var line = handLines[i];

                var lastChar = line[line.Length - 1];

                switch (lastChar)
                {
                    // woezelenpip collected $7.50 from pot
                    // kiljka: sits out 
                    case 't':
                        if (line.EndsWith("pot", StringComparison.Ordinal))
                        {
                            handActions.Add(ParseCollectedLine(line, Street.Showdown));
                        }
                        continue;
                    // templargio collected €6.08 from side pot-2
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':

                        // skip lines like
                        // Hudison747 was removed from the table for failing to post
                        if (line[line.Length - 1] == 't' && line[line.Length - 2] == 's')
                            continue;

                        if (line[line.Length - 2] == '-')
                        {
                            handActions.Add(ParseCollectedLine(line, Street.Showdown));
                        }
                        continue;

                    //*** FLOP *** [6d 7c 6h]
                    //*** TURN *** [6d 7c 6h] [2s]
                    //*** RIVER *** [6d 7c 6h 2s] [Qc]
                    case ']':
                        continue;

                    //*** SUMMARY ***
                    //*** SHOW DOWN ***
                    //*** FIRST SHOW DOWN ***
                    //*** SECOND SHOW DOWN ***
                    case '*':
                        char starId = line[5];

                        switch (starId)
                        {
                            //*** SHOW DOWN ***
                            //*** FIRST SHOW DOWN ***
                            case 'H':
                            case 'I':
                                continue;

                            //*** SUMMARY ***
                            case 'U':
                                return;
                            //Skipping Second showdown, that is parsed with ParseRunItTwice
                            //*** SECOND SHOW DOWN ***
                            case 'E':
                                continue;

                            default:
                                throw new ArgumentException("Unhandled line: " + line);
                        }

                    //No low hand qualified
                    //EASSA: mucks hand
                    case 'd':
                        if (line.EndsWith("hand", StringComparison.Ordinal))
                        {
                            break;
                        }
                        continue;

                    //Player1: shows [6d Ad] (a pair of Sixes)
                    case ')':
                        break;

                    //skip unidentified actions such as
                    //leaves table
                    //stands up
                    default:
                        continue;
                }

                int colonIndex = line.LastIndexOf(':'); // do backwards as players can have : in their name

                var action = ParseMiscShowdownLine(line, colonIndex, gameType);
                handActions.Add(action);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="line"></param>
        /// <param name="currentStreet"></param>
        /// <param name="handActions"></param>
        /// <returns>True if we have reached the end of the action block.</returns>
        private bool ParseLine(string line, ref Street currentStreet, ref List<HandAction> handActions)
        {
            //We filter out only possible line endings we want
            char lastChar = line[line.Length - 1];

            // Uncalled bet lines look like:
            // Uncalled bet ($6) returned to woezelenpip
            if (line.Length > 29 && line[13] == '(')
            {
                handActions.Add(ParseUncalledBetLine(line, currentStreet));
                currentStreet = Street.Showdown;
                return true;
            }

            switch (lastChar)
            {
                //All actions with an amount(BET, CALL, RAISE)
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case 's':
                    if (IsJoinTableLine(line))
                        return false;

                    break;

                // 2As88 will be allowed to play after the button
                // matze1987: raises $8.94 to $10.94 and is all-in
                // PLR_3866114TO: Re-join
                case 'n':
                    if (line.EndsWith("on", StringComparison.Ordinal) || line.EndsWith("oin", StringComparison.Ordinal))
                    {
                        return false;
                    }
                    break;

                // golfiarzp has timed out
                // Hudison747 was removed from the table for failing to post
                case 't':
                    if (line[line.Length - 2] == 'u' || line[line.Length - 2] == 's')
                        return false;
                    break;


                //*** SUMMARY ***
                //*** SHOW DOWN ***
                case '*':
                //*** FLOP *** [Qs Js 3h]
                //Dealt to PS_Hero [4s 7h]
                case ']':
                    char firstChar = line[0];

                    if (firstChar == '*')
                    {
                        return ParseCurrentStreet(line, ref currentStreet);
                    }
                    return false;

                // molotork65: raises $2.50 to $6.50 and has reached the $10 cap
                case 'p':
                    break;

                default:
                    return false;
            }

            //zeranex88 joins the table at seat #5 
            if (line[line.Length - 2] == '#')
            {
                // joins action
                // don't bother parsing it or adding it
                return false;
            }

            int colonIndex = line.LastIndexOf(':'); // do backwards as players can have : in their name

            HandAction handAction;

            switch (currentStreet)
            {
                case Street.Showdown:
                case Street.Null:
                    throw new HandActionException("", "Invalid State: Street");

                default:

                    if (colonIndex > -1 && line[line.Length - 1] != 't')
                    {
                        handAction = ParseRegularActionLine(line, colonIndex, currentStreet);
                    }
                    else
                    {
                        if (IsRebuyLine(line))
                        {
                            return false;
                        }

                        handAction = ParseCollectedLine(line, Street.Showdown);
                    }

                    break;
            }

            handActions.Add(handAction);

            return false;
        }

        private static bool ParseCurrentStreet(string line, ref Street currentStreet)
        {
            char typeOfEventChar = line[7];

            // this way we implement the collected lines in the regular showdown for the hand
            // both showdowns will be included in the regular hand actions, so the regular hand actions can be used for betting/pot/rake verification
            // might be readjusted so that only the first one is the regular handactions, and the second one goes to runittwice

            // *** FIRST FLOP
            // *** FIRST TURN
            if (typeOfEventChar == 'S')
                typeOfEventChar = line[13];

            // *** SECOND FLOP
            // *** SECOND TURN
            if (typeOfEventChar == 'O')
                typeOfEventChar = line[14];

            switch (typeOfEventChar)
            {
                case 'P':
                    currentStreet = Street.Flop;
                    return false;
                case 'N':
                    currentStreet = Street.Turn;
                    return false;
                case 'E':
                    currentStreet = Street.River;
                    return false;
                case 'W':
                    currentStreet = Street.Showdown;
                    return true;
                case 'M':
                    return true;
                default:
                    throw new HandActionException(line, "Unrecognized line w/ a *:" + line);
            }
        }

        public HandAction ParseMiscShowdownLine(string actionLine, int colonIndex, GameType gameType = GameType.Unknown)
        {
            // if the game type is Omaha HiLo can get colons like this after the Hi
            //  DOT19: shows [As 8h Ac Kd] (HI: two pair, Aces and Sixes)            
            while ((gameType == GameType.PotLimitOmahaHiLo || gameType == GameType.FixedLimitOmahaHiLo || gameType == GameType.NoLimitOmahaHiLo) &&
                actionLine.Count(c => c == ':') > 1 &&
                actionLine.Contains("(HI:") || actionLine.Contains("; LO:"))
            {
                int lastColon = actionLine.LastIndexOf(':');
                actionLine = actionLine.Remove(lastColon - 1);
                colonIndex = actionLine.LastIndexOf(':');
            }

            string playerName = actionLine.Substring(0, colonIndex);

            char actionIdentifier = char.ToLowerInvariant(actionLine[colonIndex + 2]);

            switch (actionIdentifier)
            {
                case 's': // RECHUK: shows [Ac Qh] (a full house, Aces full of Queens)
                    return new HandAction(playerName, HandActionType.SHOW, Street.Showdown);
                case 'd': // woezelenpip: doesn't show hand 
                case 'm': // Fjell_konge: mucks hand
                    return new HandAction(playerName, HandActionType.MUCKS, Street.Showdown);
                default:
                    throw new HandActionException(actionLine, "ParseMiscShowdownLine: Unrecognized line '" + actionLine + "'");
            }
        }

        public HandAction ParsePostingActionLine(string actionLine, int colonIndex, bool smallBlindPosted, bool bigBlindPosted)
        {
            string playerName = actionLine.Substring(0, colonIndex);
            bool isAllIn = false;

            // Expect lines to look like one of these:

            // bingo185: posts small blind $0.50
            // bingo185: posts big blind $0.50
            // bingo185: posts the ante $0.05
            // bingo185: posts small & big blinds $0.75

            // the column w/ the & is a unique identifier
            char identifierChar = actionLine[colonIndex + 14];
            char lastChar = actionLine[actionLine.Length - 1];

            // Gaby66916: posts big blind $0.25 and is all-in
            if (lastChar == 'n')
            {
                isAllIn = true;
                actionLine = actionLine.Substring(0, actionLine.Length - 14);
            }

            //Mi$iek_PL :D is connected 
            if (lastChar == 'd')
                return null;

            int firstDigitIndex;
            HandActionType handActionType;



            switch (identifierChar)
            {
                // this is important, because we need to adjust the raise sizes accordingly
                case 'b':
                    firstDigitIndex = colonIndex + 20;
                    handActionType = smallBlindPosted ? HandActionType.POSTS : HandActionType.SMALL_BLIND;
                    break;
                case 'i':
                    firstDigitIndex = colonIndex + 18;
                    handActionType = bigBlindPosted ? HandActionType.POSTS : HandActionType.BIG_BLIND;
                    break;

                case 't':
                    firstDigitIndex = colonIndex + 17;
                    handActionType = HandActionType.ANTE;
                    break;

                case '&':
                    firstDigitIndex = colonIndex + 27;
                    handActionType = HandActionType.POSTS;
                    break;
                default:
                    throw new HandActionException(actionLine, "ParsePostingActionLine: Unregonized lined " + actionLine);
            }

            decimal amount = decimal.Parse(actionLine.Substring(firstDigitIndex, actionLine.Length - firstDigitIndex), NumberStyles.AllowCurrencySymbol | NumberStyles.Number, _numberFormatInfo);
            return new HandAction(playerName, handActionType, amount, Street.Preflop, isAllIn);
        }

        public HandAction ParseRegularActionLine(string actionLine, int colonIndex, Street currentStreet)
        {
            string playerName = actionLine.Substring(0, colonIndex);

            // all-in likes look like: 'Piotr280688: raises $8.32 to $12.88 and is all-in' 
            bool isAllIn = actionLine[actionLine.Length - 1] == 'n';
            // Remove the  ' and is all in' and just proceed like normal
            if (isAllIn)
            {
                actionLine = actionLine.Remove(actionLine.Length - 14);
            }

            // lines that reach the cap look like tzuiop23: calls $62 and has reached the $80 cap
            bool hasReachedCap = actionLine[actionLine.Length - 1] == 'p';

            // Remove the  ' and has reached the $80 cap' and just proceed like normal
            if (hasReachedCap)
            {
                int lastNonCapCharacter = actionLine.LastIndexOf('n') - 2;  // find the n in the and
                actionLine = actionLine.Remove(lastNonCapCharacter);
            }

            char actionIdentifier = actionLine[colonIndex + 2];

            HandActionType actionType;
            decimal amount;
            int firstDigitIndex;

            switch (actionIdentifier)
            {
                //gaydaddy: folds
                case 'f':
                    return new HandAction(playerName, HandActionType.FOLD, currentStreet);

                case 'c':
                    //Piotr280688: checks
                    if (actionLine[colonIndex + 3] == 'h')
                    {
                        return new HandAction(playerName, HandActionType.CHECK, currentStreet);
                    }
                    //MECO-LEO: calls $1.23
                    firstDigitIndex = actionLine.LastIndexOf(' ') + 1;
                    amount = decimal.Parse(actionLine.Substring(firstDigitIndex, actionLine.Length - firstDigitIndex), NumberStyles.AllowCurrencySymbol | NumberStyles.Number, _numberFormatInfo);
                    actionType = HandActionType.CALL;
                    break;

                //MS13ZEN: bets $1.76
                case 'b':
                    firstDigitIndex = actionLine.LastIndexOf(' ') + 1;
                    amount = decimal.Parse(actionLine.Substring(firstDigitIndex, actionLine.Length - firstDigitIndex), NumberStyles.AllowCurrencySymbol | NumberStyles.Number, _numberFormatInfo);
                    actionType = HandActionType.BET;
                    break;

                //Zypherin: raises $6400 to $8300              
                case 'r':
                    firstDigitIndex = actionLine.LastIndexOf(' ') + 1;
                    amount = decimal.Parse(actionLine.Substring(firstDigitIndex, actionLine.Length - firstDigitIndex), NumberStyles.AllowCurrencySymbol | NumberStyles.Number, _numberFormatInfo);
                    actionType = HandActionType.RAISE;
                    break;
                default:
                    throw new HandActionException(actionLine, "ParseRegularActionLine: Unrecognized line:" + actionLine);
            }

            return new HandAction(playerName, actionType, amount, currentStreet, isAllIn);
        }

        public HandAction ParseCollectedLine(string actionLine, Street currentStreet)
        {
            // 0 = main pot
            int potNumber = 0;
            HandActionType handActionType = HandActionType.WINS;

            // check for side pot lines like
            //  CinderellaBD collected $7 from side pot-2
            if (actionLine[actionLine.Length - 2] == '-')
            {
                handActionType = HandActionType.WINS_SIDE_POT;
                potNumber = Int32.Parse(actionLine[actionLine.Length - 1].ToString());
                // This removes the ' from side pot-2' from the line
                actionLine = actionLine.Substring(0, actionLine.Length - 16);
            }
            // check for a side pot line like
            // bozzoTHEclow collected $136.80 from side pot
            else if (actionLine[actionLine.Length - 8] == 's')
            {
                potNumber = 1;
                handActionType = HandActionType.WINS_SIDE_POT;
                // This removes the ' from side pot' from the line
                actionLine = actionLine.Substring(0, actionLine.Length - 14);
            }
            // check for main pot line like 
            //bozzoTHEclow collected $245.20 from main pot
            else if (actionLine[actionLine.Length - 8] == 'm')
            {
                // This removes the ' from main pot' from the line
                actionLine = actionLine.Substring(0, actionLine.Length - 14);
            }
            // otherwise is basic line like
            // alecc frost collected $1.25 from pot
            else
            {
                // This removes the ' from pot' from the line
                actionLine = actionLine.Substring(0, actionLine.Length - 9);
            }

            // Collected bet lines look like:
            // alecc frost collected $1.25 from pot
            int firstAmountDigit = actionLine.LastIndexOf(' ') + 1;
            decimal amount = decimal.Parse(actionLine.Substring(firstAmountDigit, actionLine.Length - firstAmountDigit), NumberStyles.AllowCurrencySymbol | NumberStyles.Number, _numberFormatInfo);

            // 12 characters from first digit to the space infront of collected
            string playerName = actionLine.Substring(0, firstAmountDigit - 11);


            return new WinningsAction(playerName, handActionType, amount, potNumber);
        }

        public HandAction ParseUncalledBetLine(string actionLine, Street currentStreet)
        {
            // Uncalled bet lines look like:
            // Uncalled bet ($6) returned to woezelenpip

            // position 14 is after the open paren
            int closeParenIndex = actionLine.IndexOf(')', 14);
            decimal amount = decimal.Parse(actionLine.Substring(14, closeParenIndex - 14), NumberStyles.AllowCurrencySymbol | NumberStyles.Number, _numberFormatInfo);

            int firstLetterOfName = closeParenIndex + 14; // ' returned to ' is length 14

            string playerName = actionLine.Substring(firstLetterOfName, actionLine.Length - firstLetterOfName);

            return new HandAction(playerName, HandActionType.UNCALLED_BET, amount, currentStreet);
        }

        private int GetFirstActionIndex(string[] handLines)
        {
            for (int lineNumber = 2; lineNumber < handLines.Length; lineNumber++)
            {
                //Seat 8: Zockermicha ($1613.51 in chips) 
                //BoomDoon: posts small blind $5
                string line = handLines[lineNumber];
                if (line[0] != 'S' || line[line.Length - 1] != ')')
                {
                    return lineNumber;
                }
            }

            throw new HandActionException(string.Empty, "GetFirstActionIndex: Couldn't find it.");
        }

        protected override PlayerList ParsePlayers(string[] handLines)
        {
            PlayerList playerList = new PlayerList();

            int lastLineRead = -1;
            bool foundSeats = false;
            // We start on line index 2 as first 2 lines are table and limit info.
            for (int lineNumber = 2; lineNumber < handLines.Length - 1; lineNumber++)
            {
                string line = handLines[lineNumber];

                // in tournaments the lines 3 to x can include addons/rebuys, skip these
                if (!foundSeats && !line.StartsWith("Seat ") && line[6] != ':')
                {
                    continue;
                }
                else if (foundSeats && !line.StartsWith("Seat "))
                {
                    lastLineRead = lineNumber;
                    break;
                }
                foundSeats = true;

                char endChar = line[line.Length - 1];

                //Seat 1: thaiJhonny ($16.08 in chips)
                //Seat 1: thaiJhonny ($16.08 in chips) is sitting out
                if (endChar != ')' && endChar != 't')
                {
                    lastLineRead = lineNumber;
                    break;
                }

                // seat info expected in format: 
                //Seat 1: thaiJhonny ($16.08 in chips)
                const int seatNumberStartIndex = 4;
                int spaceIndex = line.IndexOf(' ', seatNumberStartIndex);
                int colonIndex = line.IndexOf(':', spaceIndex + 1);
                int seatNumber = FastInt.Parse(line, spaceIndex + 1);

                // we need to find the ( before the number. players can have ( in their name so we need to go backwards and skip the last one
                int openParenIndex = line.LastIndexOf('(');

                //Seat 2: ZamaskaStars (1660 in chips) out of hand (moved from another table into small blind)
                if (line[openParenIndex + 1] == 'm')
                {
                    line = line.Remove(openParenIndex);
                    openParenIndex = line.LastIndexOf('(');
                }

                int spaceAfterOpenParen = line.IndexOf(' ', openParenIndex);

                string playerName = line.Substring(colonIndex + 2, (openParenIndex - 1) - (colonIndex + 2));

                string stackString = line.Substring(openParenIndex + 1, spaceAfterOpenParen - (openParenIndex + 1));
                decimal stack = decimal.Parse(stackString, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, _numberFormatInfo);

                playerList.Add(new Player(playerName, stack, seatNumber));
            }

            if (lastLineRead == -1)
            {
                throw new PlayersException(string.Empty, "Didn't break out of the seat reading block.");
            }

            // Looking for the showdown info which looks like this
            //*** SHOW DOWN ***
            //JokerTKD: shows [2s 3s Ah Kh] (HI: a pair of Sixes)
            //DOT19: shows [As 8h Ac Kd] (HI: two pair, Aces and Sixes)
            //DOT19 collected $24.45 from pot
            //No low hand qualified
            //*** SUMMARY ***
            //or
            //*** FIRST SHOW DOWN ***

            int summaryIndex = GetSummaryStartIndex(handLines, lastLineRead);
            int showDownIndex = GetShowDownStartIndex(handLines, lastLineRead, summaryIndex);
            //Starting from the bottom to parse faster
            if (showDownIndex != -1)
            {
                for (int lineNumber = showDownIndex + 1; lineNumber < summaryIndex; lineNumber++)
                {
                    //jimmyhoo: shows [7h 6h] (a full house, Sevens full of Jacks)
                    //EASSA: mucks hand 
                    //jimmyhoo collected $562 from pot
                    string line = handLines[lineNumber];
                    //Skip when player mucks, collects or says sth.
                    //EASSA: mucks hand 
                    char lastChar = line[line.Length - 1];

                    if (lastChar == '*')
                    {
                        break;
                    }

                    if (lastChar == 'd' || lastChar == 't' || lastChar == '"')
                    {
                        continue;
                    }

                    int lastSquareBracket = line.LastIndexLoopsBackward(']', line.Length - 1);

                    if (lastSquareBracket == -1)
                    {
                        continue;
                    }

                    int firstSquareBracket = line.LastIndexOf('[', lastSquareBracket);

                    // can show single cards:
                    // Zaza5573: shows [Qc]
                    if (line[firstSquareBracket + 3] == ']')
                    {
                        continue;
                    }

                    int colonIndex = line.LastIndexOf(':', firstSquareBracket);

                    if (colonIndex == -1)
                    {
                        // players with [ in their name
                        // [PS_UA]Tarik collected $18.57 from pot
                        continue;
                    }

                    string playerName = line.Substring(0, colonIndex);

                    // cards is set already, so skip them
                    // PLR_6521304KV: BOUNTY PRIZE [$25]
                    if (playerList[playerName].HoleCards != null)
                    {
                        continue;
                    }

                    string cards = line.Substring(firstSquareBracket + 1, lastSquareBracket - (firstSquareBracket + 1));

                    playerList[playerName].HoleCards = HoleCards.FromCards(cards);
                }
            }
            else
            {
                //Check for player shows
                for (int i = summaryIndex - 1; i > 0; i--)
                {
                    string line = handLines[i];

                    if (line.EndsWith(")") && line.Contains(": shows ["))
                    {
                        int nameEndIndex = line.IndexOf(": shows [", StringComparison.Ordinal);

                        string playerName = line.Remove(nameEndIndex);

                        int cardsStartIndex = nameEndIndex + 9;
                        int cardsEndIndex = line.IndexOf(']', cardsStartIndex);

                        string cards = line.Substring(cardsStartIndex, cardsEndIndex - cardsStartIndex);

                        playerList[playerName].HoleCards = HoleCards.FromCards(cards);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return playerList;
        }

        private int GetShowDownStartIndex(string[] handLines, int lastLineRead, int summaryIndex)
        {
            for (int i = lastLineRead; i < summaryIndex; i++)
            {
                string line = handLines[i];

                char lastChar = line[line.Length - 1];

                if (lastChar != '*')
                {
                    continue;
                }

                //*** SHOW DOWN ***
                if (line.StartsWith("*** SHOW", StringComparison.Ordinal))//handLines[i].StartsWith("*** SH"))
                {
                    return i;
                }

                //*** FIRST SHOW DOWN ***
                if (line.StartsWith("*** FIRST", StringComparison.Ordinal))
                {
                    return i;
                }
            }
            return -1;
        }

        private int GetSummaryStartIndex(string[] handLines, int lastLineRead)
        {
            for (int lineNumber = handLines.Length - 3; lineNumber > lastLineRead; lineNumber--)
            {
                string line = handLines[lineNumber];

                if (line[0] != 'S' &&
                    line[0] != 'T' &&
                    line[0] != 'B')
                {
                    return lineNumber;
                }
            }
            //Summary must exist or it is not a valid Pokerstars Hand
            throw new IndexOutOfRangeException("Could not find *** Summary ***");
        }

        protected override BoardCards ParseCommunityCards(string[] handLines)
        {
            // Expect the end of the hand to have something like this:
            //*** SUMMARY ***
            //Total pot $90 | Rake $2.80 
            //Board [4s 7d Ad]
            //Seat 4: TopKat5757 (small blind) folded before Flop
            //Seat 5: Masic.Almir (big blind) folded before Flop

            BoardCards boardCards = BoardCards.ForPreflop();
            for (int lineNumber = handLines.Length - 2; lineNumber >= 0; lineNumber--)
            {
                string line = handLines[lineNumber];

                if (line[0] == '*')
                {
                    return boardCards;
                }

                if (line[0] == 'B')
                {
                    const int firstSquareBracket = 7;
                    int lastSquareBracket = line.Length - 1;

                    return ParseBoard(line, firstSquareBracket, lastSquareBracket);
                }

                if (line[0] == 'F')
                {
                    //FIRST Board [3d Kd 9h 8h 4s]
                    const int firstSquareBracket = 13;
                    int lastSquareBracket = line.Length - 1;

                    return ParseBoard(line, firstSquareBracket, lastSquareBracket);
                }
            }

            throw new CardException(string.Empty, "Read through hand backwards and didn't find a board or summary.");
        }

        private static BoardCards ParseBoard(string line, int firstSquareBracket, int lastSquareBracket)
        {
            return BoardCards.FromCards(line.Substring(firstSquareBracket, lastSquareBracket - firstSquareBracket));
        }

        protected override string ParseHeroName(string[] handlines, PlayerList playerList)
        {
            for (int i = 0; i < handlines.Length; i++)
            {
                string line = handlines[i];

                if (line.StartsWith("Dealt to ", StringComparison.Ordinal))
                {
                    int startIndex = line.IndexOf('[');

                    var heroName = line.Substring(9, startIndex - 10);

                    if (playerList != null && playerList[heroName] != null && playerList[heroName].HoleCards == null)
                    {
                        var cards = line.Substring(startIndex + 1, line.Length - startIndex - 2)
                                    .Replace("[", string.Empty).Replace("]", string.Empty).Replace(",", " ");

                        playerList[heroName].HoleCards = HoleCards.FromCards(cards);
                    }

                    return heroName;
                }
            }
            return null;
        }

        public int ParseGameActions(string[] handLines, ref List<HandAction> handActions, int firstActionIndex, out Street currentStreet)
        {
            currentStreet = Street.Preflop;

            for (int lineNumber = firstActionIndex; lineNumber < handLines.Length; lineNumber++)
            {
                string handLine = handLines[lineNumber];

                try
                {
                    bool isFinished = ParseLine(handLine, ref currentStreet, ref handActions);

                    if (isFinished)
                    {
                        return lineNumber + 1;
                    }
                }
                catch
                {
                    continue;
                }
            }

            throw new InvalidHandException(string.Join(Environment.NewLine, handLines));
        }
    }
}
