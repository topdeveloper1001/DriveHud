using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.EquityCalculator.Analyzer
{
    internal class Settings
    {
        internal static Settings CurrentSettings = new Settings();
        internal bool AlwaysLoadAPCWithReplayer = true;
        internal bool LoadLastSavedHandOnStartup = true;
        internal bool AlwaysSkipFoldedPreflopHands = true;
        internal int LastSavedHand = -1;

        internal bool MissedEVCalculation_StreetByStreet = false;//true->street by street, false->final
        internal bool UseOpponentSpecificStats = false;
        internal bool TurnOffPlayerModelAdjustments = false;
        internal bool UseOpponentsStatsOnlyWhenOpponentHasXHands = true;
        internal int OpponentXHands = 100;
        internal String PlayerCustomPic = "";
        internal bool IsCustomPlayerPic = false;

        internal static Settings GetCurrentSettings()
        {
            return new Settings();
        }
    }
}
