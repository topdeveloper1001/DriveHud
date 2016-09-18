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

        protected override ITableSeatAreaConfigurator TableSeatAreaConfigurator
        {
            get
            {
                return new CommonTableSeatAreaConfigurator();
            }
        }

        public override EnumPokerSites Type
        {
            get { return EnumPokerSites.Unknown; }
        }

        public override HudType HudType
        {
            get { return HudType.Plain; }
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

            foreach (var hudElement in hudTable.HudElements.Where(x => x.HudType == HudType))
            {
                var label = CreatePlayerLabel(string.Format("Player {0}", hudElement.Seat));

                label.X = labelPositions[seats][hudElement.Seat - 1, 0];
                label.Y = labelPositions[seats][hudElement.Seat - 1, 1];

                diagram.AddShape(label);

                var hud = CreateHudLabel(hudElement);
                hud.ZIndex = 100;
                diagram.AddShape(hud);
            }

            CreateSeatAreas(diagram, hudTable, seats);
            CreatePreferredSeatMarkers(diagram, hudTable, seats);
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
                                HudType = HudType,
                                Position = new Point(hudElementPositionX, hudElementPositionY)
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
                { 2, new int[,] { { 352, 105 }, { 352, 411 } } },
                { 3, new int[,] { { 352, 105 }, { 541, 422 }, { 161, 422 } } },
                { 4, new int[,] { { 352, 105 }, { 660, 256 }, { 352, 411 }, { 57, 256 } } },
                { 6, new int[,] { { 352, 105 }, { 638, 180 }, { 638, 353 }, { 352, 411 }, { 96, 353 }, { 96, 180 } } },
                { 8, new int[,] { { 352, 105 }, { 529, 128 }, { 698, 258 }, { 529, 393 }, { 352, 411 }, { 196, 393 },  { 13, 258 }, { 194, 122 } } },
                { 9, new int[,] { { 670, 200 }, { 670, 342 }, { 541, 422 }, { 352, 411 }, { 161, 422 }, { 40, 342 }, { 40, 200 }, { 161, 113 }, { 541, 113 } } },
                { 10, new int[,] { { 352, 105 }, { 529, 128 }, { 678, 200 }, { 678, 309 }, { 529, 393 }, { 352, 411 }, { 196, 393 }, { 27, 309 }, { 33, 200 }, { 194, 122 } } }
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

        protected override Dictionary<int, int[,]> GetPredefinedMarkersPositions()
        {
            var predefinedPositions = new Dictionary<int, int[,]>
            {
                { 2, new int[,] { { 425, 87 }, { 425, 396 } } },
                { 3, new int[,] { { 425, 87 }, { 615, 396 }, { 235, 396 } } },
                { 4, new int[,] { { 743, 174 }, { 743, 316 }, { 147, 316 }, { 147, 174 } } },
                { 6, new int[,] { { 425, 87 }, { 743, 174 }, { 743, 316 }, { 425, 396 }, { 147, 316 }, { 147, 174 } } },
                { 8, new int[,] { { 425, 87 }, { 614, 87 }, { 743, 174 }, { 614, 396 }, { 425, 396 }, { 235, 396 }, { 147, 174 }, { 235, 87 }  } },
                { 10, new int[,] { { 425, 87 }, { 614, 87 }, { 743, 174 }, { 743, 316 }, { 614, 396 }, { 425, 396 }, { 235, 396 }, { 147, 316 }, { 147, 174 }, { 235, 87 }  } }
            };

            return predefinedPositions;
        }
    }
}