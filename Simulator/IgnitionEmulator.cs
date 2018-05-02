using DriveHUD.Entities;

namespace Simulator
{
    public class IgnitionEmulator : PokerStarsEmulator
    {
        public IgnitionEmulator()
        {
            HandHistoryLocation = @"d:\Git\DriveHUD\Simulator\TestData\Ignition";
        }

        public override string Name => "Ignition Emulator";

        public override EnumPokerSites Site => EnumPokerSites.Ignition;

        protected override string HandHistoryFilter => "*.xml";
    }
}