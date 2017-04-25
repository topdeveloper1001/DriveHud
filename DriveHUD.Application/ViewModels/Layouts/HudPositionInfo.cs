//-----------------------------------------------------------------------
// <copyright file="HudPositionInfo.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Windows;

namespace DriveHUD.Application.ViewModels.Layouts
{
    /// <summary>
    /// This class represents the information of the position on the poker table
    /// </summary>
    [Serializable]
    public class HudPositionInfo
    {
        /// <summary>
        /// Gets or sets the seat number of the position
        /// </summary>
        public int Seat { get; set; }

        /// <summary>
        /// Gets or sets the coordinates of the position
        /// </summary>
        public Point Position { get; set; }

        /// <summary>
        /// Gets or sets the width
        /// </summary>
        [DefaultValue(0d)]
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets the height
        /// </summary>
        [DefaultValue(0d)]
        public double Height { get; set; }

        /// <summary>
        /// Creates a copy of current <see cref="HudPositionInfo"/>
        /// </summary>
        /// <returns>Copy of current <see cref="HudPositionInfo"/></returns>
        public HudPositionInfo Clone()
        {
            return new HudPositionInfo
            {
                Position = new Point(Position.X, Position.Y),
                Width = Width,
                Height = Height,
                Seat = Seat
            };
        }
    }
}