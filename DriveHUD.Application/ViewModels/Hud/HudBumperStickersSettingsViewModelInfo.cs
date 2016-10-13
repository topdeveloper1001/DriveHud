using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.ViewModels
{
    public class HudBumperStickersSettingsViewModelInfo
    {
        public IEnumerable<HudBumperStickerType> BumperStickers { get; set; }

        public Action Save { get; set; }
    }
}
