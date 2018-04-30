using DriveHUD.Entities;

namespace Simulator
{
    public class PartyPokerEmulator : PokerStarsEmulator
    {
        public PartyPokerEmulator()
        {
            HandHistoryLocation = @"d:\Git\DriveHUD\Simulator\TestData\PartyPoker";
        }

        public override string Name => "PartyPoker Emulator";

        public override EnumPokerSites Site => EnumPokerSites.PartyPoker;

        protected override string HandHistoryFilter => "*.txt";
    }
}
