//-----------------------------------------------------------------------
// <copyright file="HudDefaultSettings.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace DriveHUD.Common.Resources
{
    /// <summary>
    /// Predefined internal hud settings
    /// </summary>
    public static class HudDefaultSettings
    {
        public const double HudIconHeaderHeight = 35d;

        public const double BovadaRichHudElementWidth = 144;

        public const double BovadaRichHudElementHeight = 215;

        public const double HudTableHeight = 568;

        public const double HudTableWidth = 812;

        public const int TableHeight = 320;

        public const int TableWidth = 600;

        public const string TableBackgroundImage = "/DriveHUD.Common.Resources;component/images/Table.png";

        public const string TableRadDiagramGroup = "tableGroup";

        public const int TablePlayerLabelWidth = 110;

        public const int TablePlayerLabelHeight = 35;

        public const int TablePlayerLabelActualHeight = 28;

        public const string TablePlayerNameFormat = "Player {0}";

        public const decimal TablePlayerBank = 10m;

        public readonly static ReadOnlyDictionary<int, int[,]> TablePlayerLabelPositions = new ReadOnlyDictionary<int, int[,]>(new Dictionary<int, int[,]>
        {
            { 1, new int[,] { { 355, 118 } } },
            { 2, new int[,] { { 355, 118 }, { 355, 409 } } },
            { 3, new int[,] { { 636, 262 }, { 355, 409 }, { 72, 262 } } },
            { 4, new int[,] { { 355, 118 }, { 636, 262 }, { 355, 409 }, { 72, 262 } } },
            { 5, new int[,] { { 490, 118 }, { 636, 318 }, { 355, 409 }, { 72, 318 }, { 220, 118 } } },
            { 6, new int[,] { { 422, 118 }, { 636, 262 }, { 422, 409 }, { 264, 409 }, { 72, 262 }, { 264, 118 } } },
            { 8, new int[,] { { 422, 118 }, { 636, 211 }, { 636, 318 }, { 422, 409 }, { 264, 409 }, { 72, 318 }, { 72, 211 }, { 264, 118 } } },
            { 9, new int[,] { { 415, 118 }, { 636, 211 }, { 636, 318 }, { 490, 409 }, { 355, 409 }, { 220, 409 }, { 72, 318 }, { 72, 211 }, { 273, 118 }  } },
            { 10, new int[,] { { 490, 118 }, { 636, 211 }, { 636, 318 }, { 490, 409 }, { 355, 409 }, { 220, 409 }, { 72, 318 }, { 72, 211 }, { 220, 118 }, { 355, 118 } } }
        });

        public const int DesignerDefaultSeat = 1;

        public readonly static Color StatInfoDefaultColor = Colors.Gray;

        public readonly static Color StatInfoActiveColor = Colors.White;

        public const int PopupShowDelay = 500;

        public const int ToolTipShowDelay = 200;

        #region Tools sizes

        public const double PlainStatBoxWidth = 135d;

        public const double PlainStatBoxHeight = double.NaN;

        public const double PlainStatBoxMinHeight = 35d;

        public const double GaugeIndicatorWidth = double.NaN;

        public const double GaugeIndicatorMinWidth = 300d;

        public const double GaugeIndicatorHeight = double.NaN;

        public const double GaugeIndicatorMinHeight = 50d;

        public const double FourStatBoxWidth = 142d;

        public const double FourStatBoxHeight = 19d;

        public const double FourStatVerticalBoxWidth = 30d;

        public const double FourStatVerticalBoxHeight = 63d;

        public const double FourStatBoxMinHeight = 19d;

        public const double TiltMeterWidth = 13d;

        public const double TiltMeterHeight = 27d;

        public const double TiltMeterMinHeight = 27d;

        public const decimal TiltMeterDesignerValue = 100m;

        public const double PlayerIconWidth = 28d;

        public const double PlayerIconHeight = 27d;

        public const double PlayerIconMinHeight = 27d;

        public const double GraphWidth = double.NaN;

        public const double GraphHeight = double.NaN;

        public const double TextBoxWidth = 25d;

        public const double TextBoxHeight = 25d;

        public const double BumperStickersWidth = 100d;

        public const double BumperStickersHeight = 13d;

        public const double HeatMapWidth = 393d;

        public const double HeatMapHeight = 364d;

        #endregion
    }
}