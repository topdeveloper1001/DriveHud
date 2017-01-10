using DriveHUD.Application.ViewModels;
using DriveHUD.Common;
using Model.Enums;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Telerik.Windows.Controls;
using Telerik.Windows.Diagrams.Core;
using System.Collections.Generic;
using DriveHUD.Application.TableConfigurators;
using System.Windows.Data;
using System.Windows.Controls;
using DriveHUD.Entities;

namespace DriveHUD.Application.TableConfigurators
{
    internal class BovadaTableConfigurator : BaseTableConfigurator
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
                return new BovadaTableSeatAreaConfigurator();
            }
        }

        public override EnumPokerSites Type
        {
            get { return EnumPokerSites.Ignition; }
        }

        public override HudType HudType
        {
            get { return HudType.Plain; }
        }

        protected override string BackgroundImage
        {
            get { return backgroundImage; }
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

            InitializeTable(diagram, hudTable, seats);
            CreateSeatAreas(diagram, hudTable, seats);

            foreach (var hudElement in hudTable.HudElements.Where(x => x.HudType == HudType))
            {
                var hud = CreateHudLabel(hudElement);
                diagram.AddShape(hud);
            }

            CreatePreferredSeatMarkers(diagram, hudTable, seats);
        }

        public override IEnumerable<HudElementViewModel> GenerateElements(int seats)
        {
            var predefinedPositions = GetPredefinedPositions();

            Check.Require(predefinedPositions.ContainsKey(seats));

            var elements = (from seat in Enumerable.Range(0, seats)
                            let hudElementPositionX = predefinedPositions[seats][seat, 0]
                            let hudElementPositionY = predefinedPositions[seats][seat, 1]
                            let isRightOriented = (seats > 6 && seat < 5) || (seats < 7 && seats > 2 && seat < 3) || (seats < 3 && seat < 1)
                            select new HudElementViewModel
                            {
                                Width = HudElementWidth,
                                Height = HudElementHeight,
                                Seat = seat + 1,
                                IsRightOriented = isRightOriented,
                                TiltMeter = 100,
                                HudType = HudType,
                                HudViewType = HudViewType.Vertical_1,
                                Position = new Point(hudElementPositionX, hudElementPositionY)
                            }).ToArray();

            return elements;
        }

        protected override RadDiagramShape CreateTableRadDiagramShape()
        {
            var table = new RadDiagramShape
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
            var predefinedPositions = new Dictionary<int, int[,]>
            {
                { 2, new int[,] { { 338, 142 }, { 342, 484 } } },
                { 6, new int[,] { { 338, 142 }, { 639, 229 }, { 639, 415 }, { 342, 494 }, { 47, 416 }, { 38, 227 } } },
                { 9, new int[,] { { 455, 142 }, { 639, 213 }, { 648, 362 }, { 524, 481 }, { 340, 493 }, { 156, 481 }, { 29, 361 }, { 41, 214 }, { 225, 146 } } }
            };

            return predefinedPositions;
        }

        protected override Dictionary<int, int[,]> GetPredefinedMarkersPositions()
        {
            var predefinedPositions = new Dictionary<int, int[,]>
            {
                { 2, new int[,] { { 450, 75 }, { 450, 417 } } },
                { 6, new int[,] { { 450, 75 }, { 745, 162 }, { 745, 348 }, { 450, 427 }, { 150, 349 }, { 150, 160 } } },
                { 9, new int[,] { { 567, 75 }, { 745, 146 }, { 760, 292 }, { 636, 413 }, { 448, 426 }, { 260, 414 }, { 137, 293 }, { 153, 146 }, { 332, 75 } } }
            };

            return predefinedPositions;
        }
    }
}