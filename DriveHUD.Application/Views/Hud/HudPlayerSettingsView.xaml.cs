using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DriveHUD.Application.Views.Hud
{
    /// <summary>
    /// Interaction logic for HudPlayerSettingsView.xaml
    /// </summary>
    public partial class HudPlayerSettingsView : UserControl
    {
        public HudPlayerSettingsView()
        {
            InitializeComponent();
        }
    }

    public class TestPlayerSettingsViewModel
    {
        public TestPlayerSettingsViewModel()
        {
            PlayerTypes = new List<TestPlayerType>()
            {
                new TestPlayerType { Caption = "Nit", Image = "/DriveHUD.Common.Resources;component/images/PlayerTypes/Nit.png" },
                new TestPlayerType { Caption = "Fish", Image = "/DriveHUD.Common.Resources;component/images/PlayerTypes/Fish.png" },
                new TestPlayerType { Caption = "Standard Reg", Image = "/DriveHUD.Common.Resources;component/images/PlayerTypes/Standard Reg.png" },
                new TestPlayerType { Caption = "Tight Reg", Image = "/DriveHUD.Common.Resources;component/images/PlayerTypes/Tight Reg.png" },
                new TestPlayerType { Caption = "Bad LAG", Image = "/DriveHUD.Common.Resources;component/images/PlayerTypes/Bad LAG.png" },
                new TestPlayerType { Caption = "Tricky LAG", Image = "/DriveHUD.Common.Resources;component/images/PlayerTypes/Tricky LAG.png" },
                new TestPlayerType { Caption = "Whale", Image = "/DriveHUD.Common.Resources;component/images/PlayerTypes/Whale.png" },
                new TestPlayerType { Caption = "Nutball", Image = "/DriveHUD.Common.Resources;component/images/PlayerTypes/Nutball.png" },
                new TestPlayerType { Caption = "Nutball", Image = "/DriveHUD.Common.Resources;component/images/PlayerTypes/Nutball.png" }
            };

            PlayerTypeStats = new List<TestPlayerStat>()
            {
                new TestPlayerStat { Caption = "VPIP%", Low = 0, High = 49.5m },
                new TestPlayerStat { Caption = "PFR%", Low = 0, High = 49.5m },
                new TestPlayerStat { Caption = "AGG%", Low = 0, High = 49.5m },
                new TestPlayerStat { Caption = "3-Bet%", Low = 0, High = 49.5m },
                new TestPlayerStat { Caption = "Flop C-Bet%", Low = 0, High = 49.5m },
                new TestPlayerStat { Caption = "Fold Fl.C-Bet%", Low = 0, High = 49.5m },
                new TestPlayerStat { Caption = "Total hands", Low = 0, High = 49.5m },
                new TestPlayerStat { Caption = "WTSD%", Low = 0, High = 49.5m },
                new TestPlayerStat { Caption = "W$WSF", Low = 0, High = 49.5m },
                new TestPlayerStat { Caption = "W$SD%", Low = 0, High = 49.5m },
                new TestPlayerStat { Caption = "Steal%", Low = 0, High = 49.5m },
                new TestPlayerStat { Caption = "AF", Low = 0, High = 49.5m }
            };
        }

        public List<TestPlayerType> PlayerTypes { get; set; }

        public List<TestPlayerStat> PlayerTypeStats { get; set; }
    }

    public class TestPlayerType
    {
        public string Caption { get; set; }

        public string Image { get; set; }
    }

    public class TestPlayerStat
    {
        public string Caption { get; set; }

        public decimal Low { get; set; }

        public decimal High { get; set; }
    }
}
