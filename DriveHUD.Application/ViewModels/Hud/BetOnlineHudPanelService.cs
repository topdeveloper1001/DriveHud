//-----------------------------------------------------------------------
// <copyright file="BetOnlineHudPanelService.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common.WinApi;
using Model.Enums;
using System;
using System.Collections.Generic;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// Service to calculate hud positions for BetOnline
    /// </summary>z
    internal class BetOnlineHudPanelService : HudPanelService
    {
        private readonly Dictionary<int, int[,]> positionsShifts = new Dictionary<int, int[,]>
        {
            { 2, new int[,] { { 7, -29 }, { 7, -4 } } },
            { 3, new int[,] { { 7, -29 }, { 19, -4 }, { -8, -4 } } },
            { 4, new int[,] { { 10, -41 }, { 10, -33 }, { -27, -33 }, { -21, -41 } } },
            { 6, new int[,] { { 0, -45 }, { -25, -66 }, { -25, -25 }, { 0, -44 }, { 30, -25 }, { 30, -66} } },
            { 8, new int[,] { { 13, -44 }, { -17, -15 }, { 0, -27 }, { -17, -72 }, { 13, -42}, { 21, -72 }, { 13, -27 }, { 21, -15 } } },
            { 9, new int[,] { { -54, 43 }, { -22, 59 }, { -310, -341 }, { -53, 28 }, { -119, 124 }, { 57, 28 }, { 34, 50 }, { 22, 58 }, { 35, 39 } } },
            { 10, new int[,] { { 13, -44 }, { -17, -15 }, { 0, -27 }, { 0, -64 }, { -17, -72 }, { 13, -42}, { 21, -72 }, { 13, -64 }, { 13, -27 }, { 21, -15 } } }
        };

        private readonly Dictionary<int, int[,]> plainPositionsShifts = new Dictionary<int, int[,]>
        {
            { 2, new int[,] { { -2, -9 }, { -2, 26}  } },
            { 3, new int[,] { { -2, -24 }, { 9, 1 }, { -18, 1 } } },
            { 4, new int[,] { { 0, -26 }, { 0, -13 }, { -22, 2 }, { -22, -6 } } },
            { 6, new int[,] { { -2, -4 }, { 0, -30 }, { 0, -12 }, { -2, 31}, { -22, 2 }, { -22, -28 }  } },
            { 8, new int[,] { { 0, -16 }, { 18, -2 }, { -28, -9 }, { 18, -19 }, { 0, -2 }, { -32, -16 }, { 20, -9 }, { -38, 11 } } },
            { 9, new int[,] { { 54, -43 }, { 22, -59 }, { 36, -50 }, { 53, -28 }, { -2, -7 }, { -57, -28 }, { -34, -50 }, { -22, -58 }, { -35, -39 } } },
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

            var maxSeats = (int)window.Layout.TableHud.TableType;

            var panelOffset = window.GetPanelOffset(hudElement);

            if (!positionsShifts.ContainsKey(maxSeats))
            {
                return new Tuple<double, double>(hudElement.Position.X * window.XFraction, hudElement.Position.Y * window.YFraction);
            }

            var shifts = window.Layout.HudType == HudType.Default ? positionsShifts[maxSeats] : plainPositionsShifts[maxSeats];

            var xPosition = panelOffset.X != 0 ? panelOffset.X : hudElement.Position.X + shifts[hudElement.Seat - 1, 0];
            var yPosition = panelOffset.Y != 0 ? panelOffset.Y : hudElement.Position.Y + shifts[hudElement.Seat - 1, 1];


            System.Diagnostics.Debug.WriteLine($"{hudElement.Seat}: {hudElement.Position.X - panelOffset.X}, {hudElement.Position.Y - panelOffset.Y}");

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
            Check.ArgumentNotNull(() => window.Layout);
            Check.ArgumentNotNull(() => window.Layout.TableHud);

            var maxSeats = (int)window.Layout.TableHud.TableType;

            var panelOffset = window.GetPanelOffset(hudElement);

            if (!positionsShifts.ContainsKey(maxSeats))
            {
                return new Tuple<double, double>(hudElement.Position.X * window.XFraction, hudElement.Position.Y * window.YFraction);
            }

            var shifts = window.Layout.HudType == HudType.Default ? positionsShifts[maxSeats] : plainPositionsShifts[maxSeats];

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
            return new Tuple<double, double>(816, 631);
        }

        /// <summary>
        /// Get handle of window on which hud has to be attached
        /// </summary>
        /// <returns>Handle of window</returns>
        public override IntPtr GetWindowHandle(IntPtr handle)
        {
            return FindPanelWindow(handle);
        }

        public override Tuple<double, double> GetInitialTrackConditionMeterPosition()
        {
            return new Tuple<double, double>(220, 0);
        }

        /// <summary>
        /// Find child window with poker table 
        /// </summary>
        /// <param name="hWnd">Main window handle</param>
        /// <returns>Child window handle</returns>
        private IntPtr FindPanelWindow(IntPtr hWnd)
        {
            var panelHandle = WinApi.FindWindowEx(hWnd, IntPtr.Zero, "wxWindowClass", "panel");
            return panelHandle != IntPtr.Zero ? panelHandle : hWnd;
        }
    }
}