using DriveHUD.Entities;

namespace Simulator
{
    public class WPNEmulator : PokerStarsEmulator
    {
        public WPNEmulator()
        {
            // default settings
            HandHistoryLocation = @"d:\Temp\ACRHandsTocheck\";
            Destination = @"c:\Users\Freeman\Desktop\DriveHUD\ACR\Peon_84";
            WindowTitle = " The Venom Step1/6-113SeatsGTD to Step2/6, Table 23 - No Limit - 500 / 1 000  Ante 150 Hold'em (9230580)";
        }

        public override string Name => "WPN Emulator";

        public override EnumPokerSites Site => EnumPokerSites.AmericasCardroom;
    }
}