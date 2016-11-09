//-----------------------------------------------------------------------
// <copyright file="Poker888HudPanelService.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.Views;
using DriveHUD.Common;
using Model.Enums;
using System;
using System.Collections.Generic;

namespace DriveHUD.Application.ViewModels.Hud
{
    internal class Poker888HudPanelService : HudPanelService
    {
        private readonly Dictionary<int, int[,]> positionsShifts = new Dictionary<int, int[,]>
        {
            { 2, new int[,] { { 7, -29 }, { 7, -4 } } },           
            { 5, new int[,] { { 10, -41 }, { 10, -33 }, { -27, -33 }, { -21, -41 }, { -21, -41 } } },
            { 6, new int[,] { { 0, -45 }, { -25, -66 }, { -25, -25 }, { 0, -44 }, { 30, -25 }, { 30, -66} } },
            { 8, new int[,] { { 13, -44 }, { -17, -15 }, { 0, -27 }, { -17, -72 }, { 13, -42}, { 21, -72 }, { 13, -27 }, { 21, -15 } } },
            { 9, new int[,] { { 28, -29 }, { -17, -29 }, { 10, -41 }, { 10, -33 }, { 19, -4 }, { 7, -4}, { -8, -4 }, { -27, -33 }, { -21, -41 } } },
            { 10, new int[,] { { 13, -44 }, { -17, -15 }, { 0, -27 }, { 0, -64 }, { -17, -72 }, { 13, -42}, { 21, -72 }, { 13, -64 }, { 13, -27 }, { 21, -15 } } }
        };

        private readonly Dictionary<int, int[,]> plainPositionsShifts = new Dictionary<int, int[,]>
        {
            { 2, new int[,] { { -2, -9 }, { -2, 26}  } },            
            { 5, new int[,] { { 0, -26 }, { 0, -13 }, { -22, 2 }, { -22, -6 }, { -22, -6 } } },
            { 6, new int[,] { { -2, -4 }, { 0, -30 }, { 0, -12 }, { -2, 31}, { -22, 2 }, { -22, -28 }  } },
            { 8, new int[,] { { 0, -16 }, { 18, -2 }, { -28, -9 }, { 18, -19 }, { 0, -2 }, { -32, -16 }, { 20, -9 }, { -38, 11 } } },
            { 9, new int[,] { { 0, -36 }, { 0, -28 }, { 9, 1 }, { -2, 1}, { -18, 1 }, { -22, -28 }, { -22, -36 }, { 18, -24 }, { -27, -24 } } },
            { 10, new int[,] { { 0, -36 }, { 0, -28 }, { 9, 1 }, { -2, 1}, { -18, 1 }, { -22, -28 }, { -22, -36 }, { 18, -24 }, { -2, -24 }, { -27, -24 } } }
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

            if (!positionsShifts.ContainsKey(maxSeats))
            {
                return new Tuple<double, double>(hudElement.Position.X * window.XFraction, hudElement.Position.Y * window.YFraction);
            }

            var shifts = window.Layout.HudType == HudType.Default ? positionsShifts[maxSeats] : plainPositionsShifts[maxSeats];

            var xPosition = panelOffset.X != 0 ? panelOffset.X : hudElement.Position.X + shifts[hudElement.Seat - 1, 0];
            var yPosition = panelOffset.Y != 0 ? panelOffset.Y : hudElement.Position.Y + shifts[hudElement.Seat - 1, 1];

            return new Tuple<double, double>(xPosition * window.XFraction, yPosition * window.YFraction);
        }

        /// <summary>
        /// Get initial table size 
        /// </summary>
        /// <returns>Return dimensions of initial table, Item1 - Width, Item - Height</returns>
        public override Tuple<double, double> GetInitialTableSize()
        {
            return new Tuple<double, double>(810, 585);
            // client rect = 794x546
        }

        public override Tuple<double, double> GetInitialTrackConditionMeterPosition()
        {
            return new Tuple<double, double>(220, 0);
        }
    }
}
