using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandHistories.Parser.Parsers;
using DriveHUD.Common.Progress;
using Microsoft.Practices.ServiceLocation;
using HandHistories.Parser.Parsers.Exceptions;
using DriveHUD.Common.Log;

namespace DriveHUD.Importers.WinningPokerNetwork
{
    internal class AmericasCardroomImporter : FileBasedImporter, IAmericasCardroomImporter
    {
        public override string Site
        {
            get { return EnumPokerSites.AmericasCardroom.ToString(); }
        }

        protected override string HandHistoryFilter
        {
            get { return "*.txt"; }
        }

        protected override string ProcessName
        {
            get { return "AmericasCardroom"; }
        }

        protected override Encoding HandHistoryFileEncoding
        {
            get { return Encoding.Unicode; }
        }

        // Import hand
        protected override void ImportHand(string handHistory, GameInfo gameInfo, out bool handProcessed)
        {
            handProcessed = true;

            var dbImporter = ServiceLocator.Current.GetInstance<IFileImporter>();
            var progress = new DHProgress();

            IEnumerable<ParsingResult> parsingResult = null;

            try
            {
                // client window contains some additional information about the game, so add it to the HH if possible
                handHistory = AddAdditionalData(handHistory);

                // ACP appends the current action to file right after it was performed instead of making the chunk update after the hand had been finished
                parsingResult = dbImporter.Import(handHistory, progress, gameInfo, true);
            }
            catch (InvalidHandException)
            {
                //hand is not finished yet
                handProcessed = false;
                return;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, string.Format("Hand(s) has not been imported"), e);
            }

            if (parsingResult == null)
            {
                return;
            }

            foreach (var result in parsingResult)
            {
                if (result.HandHistory == null)
                {
                    continue;
                }

                if (result.IsDuplicate)
                {
                    LogProvider.Log.Info(this, string.Format("Hand {0} has not been imported. Duplicate.", result.HandHistory.Gamenumber));
                    continue;
                }

                if (!result.WasImported)
                {
                    LogProvider.Log.Info(this, string.Format("Hand {0} has not been imported.", result.HandHistory.Gamenumber));
                    continue;
                }

                LogProvider.Log.Info(this, string.Format("Hand {0} imported", result.HandHistory.Gamenumber));

                var playerList = GetPlayerList(result.Source);

                gameInfo.WindowHandle = FindWindow(result).ToInt32();
                gameInfo.GameFormat = ParseGameFormat(result);
                gameInfo.GameType = ParseGameType(result);
                gameInfo.TableType = ParseTableType(result);

                var dataImportedArgs = new DataImportedEventArgs(playerList, gameInfo);

                eventAggregator.GetEvent<DataImportedEvent>().Publish(dataImportedArgs);
            }
        }

        protected override bool Match(string title, ParsingResult parsingResult)
        {
            var tableName = parsingResult.Source.TableName;

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(tableName))
            {
                return false;
            }

            var isTitleMatch = title.Contains(tableName);

            if (isTitleMatch && parsingResult.GameType.Istourney && !string.IsNullOrWhiteSpace(parsingResult.HandHistory.Tourneynumber))
            {
                return title.Contains(parsingResult.HandHistory.Tourneynumber);
            }

            return isTitleMatch;

            //// " - No Limit "
            //// " - Fixed "
            //// " - Pot Limit "
            //return base.Match(title, parsingResult);
        }

        private string AddAdditionalData(string handHistory)
        {
            if (string.IsNullOrWhiteSpace(handHistory))
            {
                return handHistory;
            }
            
            
            
            return handHistory;
        }
    }
}
