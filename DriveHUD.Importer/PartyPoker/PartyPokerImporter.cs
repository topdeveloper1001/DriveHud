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
using DriveHUD.Common.Log;
using DriveHUD.Common.Utils;
using DriveHUD.Common.Linq;

namespace DriveHUD.Importers.PartyPoker
{
    internal class PartyPokerImporter : FileBasedImporter, IPartyPokerImporter
    {
        private readonly Dictionary<string, string[]> _playerNamesDictionary;

        public PartyPokerImporter()
        {
            _playerNamesDictionary = new Dictionary<string, string[]>();
        }

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

        protected override void ProcessHand(string handHistory, GameInfo gameInfo)
        {
            gameInfo.UpdateAction = UpdatePlayerNames;

            base.ProcessHand(handHistory, gameInfo);
        }

        protected override void Clean()
        {
            _playerNamesDictionary.Clear();
            base.Clean();
        }

        private void UpdatePlayerNames(IEnumerable<ParsingResult> parsingResults, GameInfo gameInfo)
        {
            parsingResults?.ForEach(x => UpdatePlayerName(x, gameInfo));
        }

        private void UpdatePlayerName(ParsingResult parsingResult, GameInfo gameInfo)
        {
            if (parsingResult == null || parsingResult.Source == null || gameInfo == null || parsingResult.GameType == null || parsingResult.GameType.Istourney)
            {
                return;
            }

            var playersToUpdate = parsingResult.Source.Players.Where(x => x.PlayerName != parsingResult.Source.Hero?.PlayerName);
            if (!playersToUpdate.All(x => x.PlayerName == $"Player{x.SeatNumber}"))
            {
                return;
            }

            if (!_playerNamesDictionary.ContainsKey(parsingResult.Source.TableName))
            {
                _playerNamesDictionary.Add(parsingResult.Source.TableName, new string[parsingResult.GameType.Tablesize]);
            }

            var dictEntry = _playerNamesDictionary[parsingResult.Source.TableName];

            for (int i = 0; i < parsingResult.GameType.Tablesize; i++)
            {
                if (i > dictEntry.Length)
                {
                    break;
                }

                var player = playersToUpdate.FirstOrDefault(x => x.SeatNumber == i + 1);
                if(player == null)
                {
                    dictEntry[i] = null;
                    continue;
                }

                var currentName = dictEntry[i];
                if (string.IsNullOrWhiteSpace(currentName))
                {
                    currentName = Utils.GenerateRandomPlayerName(player.SeatNumber);
                    dictEntry[player.SeatNumber - 1] = currentName;
                }

                var originalPlayerName = $"Player{player.SeatNumber}";

                foreach (var action in parsingResult.Source.HandActions)
                {
                    if (action.PlayerName == originalPlayerName)
                    {
                        action.PlayerName = currentName;
                    }
                }

                var dbPlayer = parsingResult.Players.FirstOrDefault(x => x.Playername == originalPlayerName);
                if (dbPlayer != null)
                {
                    dbPlayer.Playername = currentName;
                }

                player.PlayerName = currentName;
            }
        }
    }
}
