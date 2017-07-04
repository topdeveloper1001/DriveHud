using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.Windows;

namespace DriveHUD.Application.ViewModels.Hud
{
    internal class PartyPokerHudPanelService : HudPanelService
    {
        private readonly Dictionary<int, int[,]> plainPositionsShifts = new Dictionary<int, int[,]>
        {
            { 2, new int[,] { { -5, -40 }, { -8, -38}  } },
            { 3, new int[,] { { -5, -57 }, { -8, -38 }, { -26, -62 } } },
            { 4, new int[,] { { -5, -57 }, { -7, -65 }, { -8, -38 }, { -26, -62 } } },
            { 6, new int[,] { { -6, -47 }, { 5, -63 }, { 5, -62 }, { -8, -24 }, { -46, -62}, { -46, -61 } } },
            { 8, new int[,] { { 116, -50 }, { 123, 20 }, { -52, 4 }, { -76, -29 }, { -116, -47 }, { -152, -131 }, { 31, -110 }, { 27, -67 } } },
            { 9, new int[,] { { -79, -49 }, { -52, -83 }, { -8, -112 }, { 29, -76}, { -6, -23 }, { -41, -76 }, { -12, -112 }, { 31, -83 }, { 66, -49 } } },
            { 10, new int[,] { { -32, -40 }, { -21, -37 }, { -26, -53 }, { -12, -43}, { -29, -46 }, { -11, -46 }, { 5, -43 }, { 13, -53 }, { -13, -37 }, { -8, -40 } } }
       };

        /// <summary>
        /// Get initial table size 
        /// </summary>
        /// <returns>Return dimensions of initial table, Item1 - Width, Item - Height</returns>
        public override Tuple<double, double> GetInitialTableSize()
        {
            return new Tuple<double, double>(800, 566);
        }

        public override Point GetPositionShift(EnumTableType tableType, int seat)
        {
            var tableSize = (int)tableType;

            if (!plainPositionsShifts.ContainsKey(tableSize))
            {
                return base.GetPositionShift(tableType, seat);
            }

            var shift = plainPositionsShifts[tableSize];

            return new Point(shift[seat - 1, 0], shift[seat - 1, 1]);
        }
    }
}
