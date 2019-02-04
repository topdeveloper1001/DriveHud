//-----------------------------------------------------------------------
// <copyright file="ReplayerPositionProvider.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Application.TableConfigurators.PositionProviders
{
    internal class ReplayerPositionProvider : IPositionProvider
    {
        private readonly Dictionary<int, int[,]> labelPositions = new Dictionary<int, int[,]>
        {
            {
                6,
                new int[,]
                {
                    { 50, 230 }, { 190, 70 }, { 720, 70 }, { 890, 230 }, { 720, 410 }, { 190, 410 }
                }
            },
            {
                9,
                new int[,]
                {
                    { 75, 310 }, { 75, 130 }, {325, 70 }, {605, 70 }, {835, 130 }, {835, 310 }, {645, 420}, {455, 420}, {245, 420}
                }
            },
            {
                10,
                new int[,]
                {
                    { 125, 90 }, { 315, 70 }, {585, 70 }, {815, 90 }, {825, 260 }, {765, 420 }, {555, 420}, {355, 420}, {145, 420}, {65, 260}
                }
            }
        };

        public ReplayerPositionProvider()
        {
            Positions = Enumerable.Range(2, 9).ToDictionary(x => x, x =>
            {
                if (x < 7)
                {
                    return labelPositions[6];
                }
                else if (x < 10)
                {
                    return labelPositions[9];
                }

                return labelPositions[10];
            });

            PlayerLabelWidth = ReplayerTableConfigurator.PLAYER_WIDTH;
            PlayerLabelHeight = ReplayerTableConfigurator.PLAYER_HEIGHT;
        }

        public Dictionary<int, int[,]> Positions { get; }

        public int PlayerLabelHeight
        {
            get;
        }

        public int PlayerLabelWidth
        {
            get;
        }
    }
}