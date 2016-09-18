using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Importers.Helpers
{
    internal static class GeneralHelpers
    {
        internal static int ShiftPlayerSeat(int seat, int shift, int tableType)
        {
            return (seat + shift + tableType - 1) % tableType + 1;
        }
    }
}
