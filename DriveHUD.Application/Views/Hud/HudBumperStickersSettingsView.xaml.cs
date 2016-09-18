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
    /// Interaction logic for HudBumperStickersSettingsView.xaml
    /// </summary>
    public partial class HudBumperStickersSettingsView : UserControl
    {
        public HudBumperStickersSettingsView()
        {
            InitializeComponent();
        }
    }

    public class TestBumperStickersSettingsViewModel
    {
        public TestBumperStickersSettingsViewModel()
        {
            BumperStickersList = new List<TestBumperSticker>()
            {
                new TestBumperSticker { Caption = "One and Done", StickerCaption = "OD" },
                new TestBumperSticker { Caption = "Pre-Flop Reg", StickerCaption = "PR" },
                new TestBumperSticker { Caption = "Barrelling", StickerCaption = "B" },
                new TestBumperSticker { Caption = "ThreeForFree", StickerCaption = "TFF" },
                new TestBumperSticker { Caption = "Way Too Early", StickerCaption = "WTE" },
                new TestBumperSticker { Caption = "Sticky Fish", StickerCaption = "SF" },
                new TestBumperSticker { Caption = "Yummy Fish", StickerCaption = "YF" },
            };

        }

        public List<TestBumperSticker> BumperStickersList { get; set; }
        public List<TestBumperStickerStats> BumperStickerStats { get; set; }
    }

    public class TestBumperSticker
    {
        public string Caption { get; set; }
        public string StickerCaption { get; set; }
    }

    public class TestBumperStickerStats
    {
    }

}
