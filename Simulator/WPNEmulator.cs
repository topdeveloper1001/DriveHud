using DriveHUD.Entities;

namespace Simulator
{
    public class WPNEmulator : PokerStarsEmulator
    {
        public WPNEmulator()
        {
            // default settings
            HandHistoryLocation = @"d:\Git\DriveHUD\Simulator\TestData\WPN";
            Destination = @"c:\Users\Freeman\Desktop\DriveHUD\ACR";
            WindowTitle = "$2 Jackpot Holdem No Limit Hold'em - 10/20 (8952347)";
        }

        public override string Name => "WPN Emulator";

        public override EnumPokerSites Site => EnumPokerSites.AmericasCardroom;
    }
}