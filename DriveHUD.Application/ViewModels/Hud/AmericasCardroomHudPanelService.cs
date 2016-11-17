using DriveHUD.Application.Views;
using DriveHUD.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.ViewModels.Hud
{
    internal class AmericasCardroomHudPanelService : HudPanelService
    {
        private readonly Dictionary<int, int[,]> plainPositionsShifts = new Dictionary<int, int[,]>
        {
            //todo: 3, 4, 8
            //done
            { 2, new int[,] { { 63, 15 }, { 106, 168}  } },
            { 3, new  int[,] { { 181, -2 }, { 95, 164 }, { -18, 1 } } },
            { 4, new int[,] { { 63, 15 }, { 179, 29 }, { 106, 168 }, { -41, 34 } } },
            //done
            { 6, new int[,] { { 66, 28 }, { 179, 29 }, { 208, 46 }, { 109, 166}, { -70, 46 }, { -41, 34 }  } },
            { 8, new int[,] { { 66, 28 }, { 181, -2 }, { 215, 52 }, { 228, 115}, { 95, 164 }, { -43, 120 }, { -45, 52 }, { -18, 1 } } },
            //done
            { 9, new int[,] { { 185, 11 }, { 181, -2 }, { 215, 52 }, { 228, 115}, { 95, 164 }, { -43, 120 }, { -45, 52 }, { -18, 1 }, { 12, 13 } } },
        };

        /// <summary>
        /// Calculates hudElement position in window
        /// </summary>
        /// <param name="hudElement">HUD element view model</param>
        /// <param name="window">Overlay window</param>
        /// <returns>Item1 - X, Item2 - Y</returns>
        public override Tuple<double, double> CalculatePositions(HudElementViewModel hudElement, HudWindow window)
        {
            Check.ArgumentNotNull(() => hudElement);
            Check.ArgumentNotNull(() => window);
            Check.ArgumentNotNull(() => window.Layout);
            Check.ArgumentNotNull(() => window.Layout.TableHud);
            Check.ArgumentNotNull(() => window.Layout.TableHud.TableLayout);

            var maxSeats = (int)window.Layout.TableHud.TableLayout.TableType;

            var panelOffset = window.GetPanelOffset(hudElement);

            if (!plainPositionsShifts.ContainsKey(maxSeats))
            {
                return new Tuple<double, double>(hudElement.Position.X * window.XFraction, hudElement.Position.Y * window.YFraction);
            }

            var shifts = plainPositionsShifts[maxSeats];

            var xPosition = panelOffset.X != 0 ? panelOffset.X : hudElement.Position.X + shifts[hudElement.Seat - 1, 0];
            var yPosition = panelOffset.Y != 0 ? panelOffset.Y : hudElement.Position.Y + shifts[hudElement.Seat - 1, 1];

            Debug.WriteLine($"seat: {hudElement.Seat}; xPosition: {xPosition}; yPosition: {yPosition}");
            Debug.WriteLine($"************************************************************************");

            return new Tuple<double, double>(xPosition * window.XFraction, yPosition * window.YFraction);
        }

        /// <summary>
        /// Get initial table size 
        /// </summary>
        /// <returns>Return dimensions of initial table, Item1 - Width, Item - Height</returns>
        public override Tuple<double, double> GetInitialTableSize()
        {
            return new Tuple<double, double>(1016, 759);
        }

        public override Tuple<double, double> GetInitialTrackConditionMeterPosition()
        {
            return new Tuple<double, double>(220, 0);
        }
    }
}
