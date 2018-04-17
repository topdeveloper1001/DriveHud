using DriveHUD.Entities;

namespace Simulator
{
    public class BetOnlineEmulator : PokerStarsEmulator
    {
        public BetOnlineEmulator()
        {
            HandHistoryLocation = @"d:\Git\DriveHUD\Simulator\TestData\";
        }

        public override string Name => "BetOnline Emulator";

        public override EnumPokerSites Site => EnumPokerSites.BetOnline;

        protected override string HandHistoryFilter => "*.xml";
    }
}