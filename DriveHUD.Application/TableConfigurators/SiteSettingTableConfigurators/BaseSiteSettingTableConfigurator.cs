using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Enums;
using Model.Settings;
using Telerik.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace DriveHUD.Application.TableConfigurators
{
    internal abstract class BaseSiteSettingTableConfigurator : ISiteSettingTableConfigurator
    {
        private const string BackgroundPlayerImage = "/DriveHUD.Common.Resources;component/images/settings/Player.png";

        private const double TABLE_HEIGHT = 205.0;
        private const double TABLE_WIDTH = 383.0;
        private const double PLAYER_WIDTH = 45.0;
        private const double PLAYER_HEIGHT = 45.0;
        private const double ACTIVE_INDICATOR_HEIGHT = 2.0;
        private const double ACTIVE_INDICATOR_WIDTH = 25.0;

        private readonly Point tablePosition = new Point(48.0, 50.0);
        private readonly Point activeIndicatorRelativePosition = new Point(10, 33);

        protected abstract Dictionary<int, double[,]> PredefinedPlayerPositions { get; }


        public void ConfigureTable(RadDiagram diagram, SiteModel viewModel, EnumTableType tableType)
        {
            diagram.Clear();

            var table = new RadDiagramShape()
            {
                Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(diagram), GetBackgroundImage(tableType)))),
                StrokeThickness = 0,
                IsEnabled = false,
                Height = TABLE_HEIGHT,
                Width = TABLE_WIDTH,
                X = tablePosition.X,
                Y = tablePosition.Y,
            };

            diagram.AddShape(table);

            CreatePlayerLabels(diagram, (int)tableType);
        }

        private void CreatePlayerLabels(RadDiagram diagram, int seats)
        {
            var positions = PredefinedPlayerPositions[seats];


            for (int i = 0; i < seats; i++)
            {
                RadDiagramShape player = new RadDiagramShape()
                {
                    Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(diagram), BackgroundPlayerImage))),
                    Height = PLAYER_HEIGHT,
                    Width = PLAYER_WIDTH,
                    StrokeThickness = 0,
                    BorderThickness = new Thickness(0),
                    ToolTip = i,
                    //IsResizingEnabled = false,
                    //IsRotationEnabled = false,
                    //IsDraggingEnabled = false,
                    //IsManipulationEnabled = false,
                    //IsManipulationAdornerVisible = false,
                    //IsHitTestVisible = true,
                    FontSize = 13,
                };
                player.X = positions[i, 0];
                player.Y = positions[i, 1];

                var indicator = AddActiveIndicator(player);

                diagram.AddShape(player);
                diagram.AddShape(indicator);
            }
        }

        private RadDiagramShape AddActiveIndicator(RadDiagramShape player)
        {
            RadDiagramShape indicator = new RadDiagramShape()
            {
                Height = ACTIVE_INDICATOR_HEIGHT,
                Width = ACTIVE_INDICATOR_WIDTH,
                StrokeThickness = 0,
                BorderThickness = new Thickness(0),
                IsResizingEnabled = false,
                IsRotationEnabled = false,
                IsDraggingEnabled = false,
                IsManipulationEnabled = false,
                IsManipulationAdornerVisible = false,
                IsHitTestVisible = true,
                FontSize = 13,
                X = player.X + activeIndicatorRelativePosition.X,
                Y = player.Y + activeIndicatorRelativePosition.Y
            };

            return indicator;
        }


        protected abstract string GetBackgroundImage(EnumTableType tableType);
    }
}
