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

          //  CreatePlayerLabels(diagram, viewModel, seats);
        }

        protected abstract string GetBackgroundImage(EnumTableType tableType);
    }
}
