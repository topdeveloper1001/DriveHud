using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Common;
using DriveHUD.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Telerik.Windows.Controls;

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

        public override HudViewType HudViewType
        {
            get
            {
                return HudViewType.Vertical_1;
            }
        }

        public override void ConfigureTable(RadDiagram diagram, HudTableViewModel hudTable, int seats)
        {
            Check.ArgumentNotNull(() => diagram);
            Check.ArgumentNotNull(() => hudTable);
            Check.Require(hudTable.HudElements != null);
            Check.Require(hudTable.HudElements.Count != seats);

            InitializeTable(diagram, hudTable, seats);       

            var labelPositions = GetPredefinedLabelPositions();

            foreach (var hudElement in hudTable.HudElements.ToArray())
            {
                var label = CreatePlayerLabel(string.Format("Player {0}", hudElement.Seat));

                label.X = labelPositions[seats][hudElement.Seat - 1, 0];
                label.Y = labelPositions[seats][hudElement.Seat - 1, 1];

                diagram.AddShape(label);

                var hud = CreateHudLabel(hudElement);

                if (hudElement.HudViewType == HudViewType.Plain)
                {
                    hud.Position = new Point(label.X - (hud.Width - label.Width)/2, label.Y + label.Height);
                }
                if (hudElement.HudViewType == HudViewType.Horizontal)
                {
                    hud.Position = hudElement.IsRightOriented ? new Point(label.X, label.Y - 60) : new Point(label.X-30, label.Y - 60);
                }
                if (hudElement.HudViewType == HudViewType.Vertical_1)
                {
                    hud.Position = hudElement.IsRightOriented ? new Point(label.X, label.Y - 60) : new Point(label.X - 30, label.Y - 60);
                }
                if (hudElement.HudViewType == HudViewType.Vertical_2)
                {
                    hud.Position = hudElement.IsRightOriented ? new Point(label.X, label.Y - 60) : new Point(label.X - 30, label.Y - 60);
                }
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
                            let isRightOriented = (seats > 6 && seat < 5) || (seats < 7 && seats > 2 && seat < 3) || (seats < 3 && seat < 1)
                            select new HudElementViewModel
                            {
                                Width = HudElementWidth,
                                Height = HudElementHeight,
                                Seat = seat + 1,
                                IsRightOriented = isRightOriented,
                                TiltMeter = 100,
                                HudViewType = HudViewType,                                
                                Position = new Point(hudElementPositionX, hudElementPositionY),
                            }).ToArray();

            return elements;
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
       
        protected virtual Dictionary<int, int[,]> GetPredefinedLabelPositions()
        {
            var predefinedLablelPositions = new Dictionary<int, int[,]>
            {
                { 2, new int[,] { { 352, 105 }, { 352, 411 } } },
                { 3, new int[,] { { 352, 105 }, { 541, 422 }, { 161, 422 } } },
                { 4, new int[,] { { 352, 105 }, { 660, 256 }, { 352, 411 }, { 57, 256 } } },
                { 5, new int[,] { { 352, 105 }, { 660, 256 }, { 352, 411 }, { 57, 256 }, { 0, 0 } } },
                { 6, new int[,] { { 352, 105 }, { 638, 180 }, { 638, 353 }, { 352, 411 }, { 96, 353 }, { 96, 180 } } },
                { 8, new int[,] { { 352, 105 }, { 529, 128 }, { 698, 258 }, { 529, 393 }, { 352, 411 }, { 196, 393 },  { 13, 258 }, { 194, 122 } } },
                { 9, new int[,] { { 415, 118 }, { 636, 211 }, { 636, 318 }, { 490, 409 }, { 355, 409 }, { 220, 409 }, { 72, 318 }, { 72, 211 }, { 273, 118 }  } },
                { 10, new int[,] { { 352, 105 }, { 529, 128 }, { 678, 200 }, { 678, 309 }, { 529, 393 }, { 352, 411 }, { 196, 393 }, { 27, 309 }, { 33, 200 }, { 194, 122 } } }
            };

            return predefinedLablelPositions;
        }
    }
}