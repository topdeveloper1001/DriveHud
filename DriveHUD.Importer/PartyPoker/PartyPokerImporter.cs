using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriveHUD.Entities;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers;
using HandHistories.Parser.Utils.FastParsing;
using Model.Settings;
using DriveHUD.Importers.Helpers;
using Microsoft.Practices.ServiceLocation;

namespace DriveHUD.Importers.PartyPoker
{
    internal class PartyPokerImporter : FileBasedImporter, IPartyPokerImporter
    {
        protected override string HandHistoryFilter
        {
            get
            {
                return "*.txt";
            }
        }

        protected override string ProcessName
        {
            get
            {
                return "PartyGaming";
            }
        }

        protected override EnumPokerSites Site
        {
            get
            {
                return EnumPokerSites.PartyPoker;
            }
        }

        private const string tournamentPattern = "({0}) Table {1}";

        protected override bool InternalMatch(string title, ParsingResult parsingResult)
        {
            if (string.IsNullOrWhiteSpace(title) || parsingResult == null ||
              parsingResult.Source == null || parsingResult.Source.GameDescription == null || string.IsNullOrEmpty(parsingResult.Source.TableName))
            {
                return false;
            }

            if (parsingResult.Source.GameDescription.IsTournament)
            {
                var tableNumber = parsingResult.Source.TableName.Substring(parsingResult.Source.TableName.LastIndexOf(' ') + 1);

                var tournamentTitle = string.Format(tournamentPattern, parsingResult.Source.GameDescription.Tournament.TournamentId, tableNumber);

                return title.Contains(tournamentTitle);
            }

            return title.Contains(parsingResult.Source.TableName);
        }

        protected override PlayerList GetPlayerList(HandHistory handHistory)
        {
            var playerList = handHistory.Players;

            var maxPlayers = handHistory.GameDescription.SeatType.MaxPlayers;

            var heroSeat = handHistory.Hero != null ? handHistory.Hero.SeatNumber : 0;

            if (heroSeat != 0)
            {
                var preferredSeats = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings().
                                        SiteSettings.SitesModelList.FirstOrDefault(x => x.PokerSite == EnumPokerSites.PartyPoker)?.PrefferedSeats;

                var prefferedSeat = preferredSeats.FirstOrDefault(x => (int)x.TableType == maxPlayers && x.IsPreferredSeatEnabled);

                if (prefferedSeat != null)
                {
                    var shift = (prefferedSeat.PreferredSeat - heroSeat) % maxPlayers;

                    foreach (var player in playerList)
                    {
                        player.SeatNumber = GeneralHelpers.ShiftPlayerSeat(player.SeatNumber, shift, maxPlayers);
                    }
                }

            }

            return playerList;
        }
    }
}
