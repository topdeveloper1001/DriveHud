//-----------------------------------------------------------------------
// <copyright file="CommonTableConfigurator.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Linq;
using System.Windows;
using System.Windows.Media;
using Telerik.Windows.Controls;
using Model.Enums;
using DriveHUD.Application.ViewModels;
using System.Collections.Generic;
using DriveHUD.Common;
using DriveHUD.Entities;

namespace DriveHUD.Application.TableConfigurators
{
    internal class CommonTableConfigurator : BaseTableConfigurator
    {
        protected const int Height = 320;
        protected const int Width = 600;

        protected const int hudElementHeight = 75;
        protected const int hudElementWidth = 135;

        protected const int labelElementWidth = 110;
        protected const int labelElementHeight = 35;

        protected const string backgroundImage = "/DriveHUD.Common.Resources;component/images/Table.png";

        public override HudViewType HudViewType
        {
            get { return HudViewType.Plain; }
        }

        protected override string BackgroundImage
        {
            get
            {
                return backgroundImage;
            }
        }

        public virtual int HudElementWidth
        {
            get
            {
                return hudElementWidth;
            }
        }

        public virtual int HudElementHeight
        {
            get
            {
                return hudElementHeight;
            }
        }

        public override void ConfigureTable(RadDiagram diagram, HudTableViewModel hudTable, int seats)
        {
            Check.ArgumentNotNull(() => diagram);
            Check.ArgumentNotNull(() => hudTable);
            Check.Require(hudTable.HudElements != null);
            Check.Require(hudTable.HudElements.Count != seats);

            var table = InitializeTable(diagram, hudTable, seats);            

            var labelPositions = GetPredefinedLabelPositions();

            foreach (var hudElement in hudTable.HudElements.ToArray())
            {
                // create shapes with player name
                var label = CreatePlayerLabel(string.Format("Player {0}", hudElement.Seat));

                label.X = labelPositions[seats][hudElement.Seat - 1, 0];
                label.Y = labelPositions[seats][hudElement.Seat - 1, 1];

                diagram.AddShape(label);

                // create shapes with stats
                var hud = CreateHudLabel(hudElement);
                diagram.AddShape(hud);
            }
        }

        public override IEnumerable<HudElementViewModel> GenerateElements(int seats)
        {
            var predefinedPositions = GetPredefinedPositions();

            Check.Require(predefinedPositions.ContainsKey(seats));

            var elements = (from seat in Enumerable.Range(0, seats)
                            let hudElementPositionX = predefinedPositions[seats][seat, 0]
                            let hudElementPositionY = predefinedPositions[seats][seat, 1]
                            select new HudElementViewModel
                            {
                                Width = HudElementWidth,
                                Height = HudElementHeight,
                                Seat = seat + 1,
                                TiltMeter = 100,
                                IsRightOriented = IsRightOriented(seats, seat),
                                HudViewType = HudViewType,
                                Position = new Point(hudElementPositionX, hudElementPositionY),
                            }).ToArray();

            return elements;
        }

        protected override RadDiagramShape CreateTableRadDiagramShape()
        {
            var table = new RadDiagramShape()
            {
                Height = Height,
                Width = Width,
                StrokeThickness = 0,
                IsEnabled = false,
                SnapsToDevicePixels = true
            };

            return table;
        }

        protected virtual Dictionary<int, int[,]> GetPredefinedPositions()
        {
            var predefinedPositions = GetPredefinedLabelPositions();

            foreach (var predefinedPosition in predefinedPositions)
            {
                for (var i = 0; i < predefinedPosition.Key; i++)
                {
                    // x coordinate
                    predefinedPosition.Value[i, 0] -= (hudElementWidth - labelElementWidth) / 2;
                    // y coordinate
                    predefinedPosition.Value[i, 1] += labelElementHeight + 3;
                }
            }

            return predefinedPositions;
        }

        protected virtual Dictionary<int, int[,]> GetPredefinedLabelPositions()
        {
            var predefinedLablelPositions = new Dictionary<int, int[,]>
            {
                { 2, new int[,] { { 355, 118 }, { 355, 409 } } },
                { 3, new int[,] { { 636, 262 }, { 355, 409 }, { 72, 262 } } },
                { 4, new int[,] { { 355, 118 }, { 636, 262 }, { 355, 409 }, { 72, 262 } } },
                { 5, new int[,] { { 490, 118 }, { 636, 318 }, { 355, 409 }, { 72, 318 }, { 220, 118 } } },
                { 6, new int[,] { { 422, 118 }, { 636, 262 }, { 422, 409 }, { 264, 409 }, { 72, 262 }, { 264, 118 } } },
                { 8, new int[,] { { 422, 118 }, { 636, 211 }, { 636, 318 }, { 422, 409 }, { 264, 409 }, { 72, 318 }, { 72, 211 }, { 264, 118 } } },
                { 9, new int[,] { { 415, 118 }, { 636, 211 }, { 636, 318 }, { 490, 409 }, { 355, 409 }, { 220, 409 }, { 72, 318 }, { 72, 211 }, { 273, 118 }  } },
                { 10, new int[,] { { 490, 118 }, { 636, 211 }, { 636, 318 }, { 490, 409 }, { 355, 409 }, { 220, 409 }, { 72, 318 }, { 72, 211 }, { 220, 118 }, { 355, 118 } } }
            };

            return predefinedLablelPositions;
        }

        private RadDiagramShape CreatePlayerLabel(string player)
        {
            var label = new RadDiagramShape
            {
                DataContext = new HudPlayerViewModel { Player = player, Bank = 10 },
                Height = labelElementHeight,
                Width = labelElementWidth,
                StrokeThickness = 0,
                BorderThickness = new Thickness(0),
                IsEnabled = false,
                IsHitTestVisible = false,
                IsRotationEnabled = false,
                Background = App.Current.Resources["HudPlayerBrush"] as VisualBrush
            };

            return label;
        }

        protected bool IsRightOriented(int seats, int seat)
        {
            return false;
        }
    }
}