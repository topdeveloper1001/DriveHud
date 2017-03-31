//-----------------------------------------------------------------------
// <copyright file="PokerStarsHudPanelService.cs" company="Ace Poker Solutions">
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
    internal class PokerStarsHudPanelService : HudPanelService
    {
        private readonly Dictionary<int, int[,]> positionsShifts = new Dictionary<int, int[,]>
        {
            { 2, new int[,] { { 7, -29 }, { 7, -4 } } },
            { 3, new int[,] { { 7, -29 }, { 19, -4 }, { -8, -4 } } },
            { 4, new int[,] { { 10, -41 }, { 10, -33 }, { -27, -33 }, { -21, -41 } } },
            { 6, new int[,] { { 0, -45 }, { -25, -66 }, { -25, -25 }, { 0, -44 }, { 30, -25 }, { 30, -66} } },
            { 8, new int[,] { { 13, -44 }, { -17, -15 }, { 0, -27 }, { -17, -72 }, { 13, -42}, { 21, -72 }, { 13, -27 }, { 21, -15 } } },
            { 9, new int[,] { { 28, -29 }, { -17, -29 }, { 10, -41 }, { 10, -33 }, { 19, -4 }, { 7, -4}, { -8, -4 }, { -27, -33 }, { -21, -41 } } },
            { 10, new int[,] { { 13, -44 }, { -17, -15 }, { 0, -27 }, { 0, -64 }, { -17, -72 }, { 13, -42}, { 21, -72 }, { 13, -64 }, { 13, -27 }, { 21, -15 } } }
        };

        private readonly Dictionary<int, int[,]> plainPositionsShifts = new Dictionary<int, int[,]>
        {
            { 2, new int[,] { { -5, -40 }, { -8, -38}  } },
            { 3, new int[,] { { -2, -24 }, { 9, 1 }, { -18, 1 } } },
            { 4, new int[,] { { -5, -57 }, { -7, -65 }, { -8, -38 }, { -26, -62 } } },
            { 6, new int[,] { { -6, -47 }, { 5, -63 }, { 5, -62 }, { -8, -24 }, { -46, -62}, { -46, -61 } } },
            { 8, new int[,] { { 116, -50 }, { 123, 20 }, { -52, 4 }, { -76, -29 }, { -116, -47 }, { -152, -131 }, { 31, -110 }, { 27, -67 } } },
            { 9, new int[,] { { -79, -49 }, { -52, -83 }, { -8, -112 }, { 29, -76}, { -6, -23 }, { -41, -76 }, { -12, -112 }, { 31, -83 }, { 66, -49 } } },
            { 10, new int[,] { { -32, -40 }, { -21, -37 }, { -26, -53 }, { -12, -43}, { -29, -46 }, { -11, -46 }, { 5, -43 }, { 13, -53 }, { -13, -37 }, { -8, -40 } } }
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
            Check.Require(window.Layout != null, "HudWindow.Layout must be set.");

            var maxSeats = (int)window.Layout.TableType;

            var panelOffset = window.GetPanelOffset(hudElement);

            if (!positionsShifts.ContainsKey(maxSeats))
            {
                return new Tuple<double, double>(hudElement.Position.X * window.XFraction, hudElement.Position.Y * window.YFraction);
            }

            var shifts = plainPositionsShifts[maxSeats];

            var xPosition = panelOffset.X != 0 ? panelOffset.X : hudElement.Position.X + shifts[hudElement.Seat - 1, 0];
            var yPosition = panelOffset.Y != 0 ? panelOffset.Y : hudElement.Position.Y + shifts[hudElement.Seat - 1, 1];

            return new Tuple<double, double>(xPosition * window.XFraction, yPosition * window.YFraction);
        }

        /// <summary>
        /// Converts offset values into position value
        /// </summary>
        /// <param name="hudElement">HUD element view model</param>
        /// <param name="window">Overlay window</param>
        /// <returns>Item1 - X, Item2 - Y</returns>
        public override Tuple<double, double> GetOffsetPosition(HudElementViewModel hudElement, HudWindow window)
        {
            Check.ArgumentNotNull(() => hudElement);
            Check.ArgumentNotNull(() => window);
            Check.Require(window.Layout != null, "HudWindow.Layout must be set.");

            var maxSeats = (int)window.Layout.TableType;

            var panelOffset = window.GetPanelOffset(hudElement);

            if (!positionsShifts.ContainsKey(maxSeats))
            {
                return new Tuple<double, double>(hudElement.Position.X * window.XFraction, hudElement.Position.Y * window.YFraction);
            }

            var shifts = plainPositionsShifts[maxSeats];

            var xPosition = panelOffset.X != 0 ? panelOffset.X - shifts[hudElement.Seat - 1, 0] : hudElement.Position.X;
            var yPosition = panelOffset.Y != 0 ? panelOffset.Y - shifts[hudElement.Seat - 1, 1] : hudElement.Position.Y;

            return new Tuple<double, double>(xPosition, yPosition);
        }

        /// <summary>
        /// Get initial table size 
        /// </summary>
        /// <returns>Return dimensions of initial table, Item1 - Width, Item - Height</returns>
        public override Tuple<double, double> GetInitialTableSize()
        {
            return new Tuple<double, double>(808, 585);
        }

        public override Tuple<double, double> GetInitialTrackConditionMeterPosition()
        {
            return new Tuple<double, double>(220, 0);
        }
    }
}