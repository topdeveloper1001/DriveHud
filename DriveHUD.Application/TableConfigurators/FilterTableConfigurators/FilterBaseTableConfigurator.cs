using DriveHUD.Application.ValueConverters;
using DriveHUD.Application.ViewModels;
using DriveHUD.Common.Reflection;
using Model.Enums;
using Model.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.TableConfigurators
{
    public class FilterBaseTableConfigurator : IFilterTableConfigurator
    {
        private const string BackgroundImage = "/DriveHUD.Common.Resources;component/images/Filters/Table{0}.png";
        private const string BackgroundPlayerImage = "/DriveHUD.Common.Resources;component/images/Filters/Player.png";

        private const double TABLE_HEIGHT = 275.0;
        private const double TABLE_WIDTH = 439.0;
        private const double PLAYER_WIDTH = 45.0;
        private const double PLAYER_HEIGHT = 45.0;
        private const double ACTIVE_INDICATOR_HEIGHT = 2.0;
        private const double ACTIVE_INDICATOR_WIDTH = 25.0;

        private readonly Point tablePosition = new Point(28.0, 0.0);
        private readonly Point activeIndicatorRelativePosition = new Point(10, 33);

        private readonly Dictionary<int, double[,]> predefinedPlayerPositions = new Dictionary<int, double[,]>()
        {
            {  6, new double[,] { { 120, 220 }, { 50, 125 }, { 120, 30 }, { 330, 30 }, { 400, 125 }, { 330, 220 } } },
            {  9, new double[,] { { 120, 220 }, { 45, 160}, { 45, 90 }, { 120, 30 }, { 330, 30 }, { 405, 90 }, { 405, 160 }, { 330, 220 }, { 225, 220 } } }
        };


        public void ConfigureTable(RadDiagram diagram, FilterStandardViewModel viewModel, int seats)
        {
            diagram.Clear();

            var table = new RadDiagramShape()
            {
                Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(diagram), string.Format(BackgroundImage, seats)))),
                StrokeThickness = 0,
                IsEnabled = false,
                Height = TABLE_HEIGHT,
                Width = TABLE_WIDTH,
                X = tablePosition.X,
                Y = tablePosition.Y
            };

            diagram.AddShape(table);

            CreatePlayerLabels(diagram, viewModel, seats);
        }

        private void CreatePlayerLabels(RadDiagram diagram, FilterStandardViewModel viewModel, int seats)
        {
            var positions = predefinedPlayerPositions[seats];
            IList<TableRingItem> collection;
            switch ((EnumTableType)seats)
            {
                case EnumTableType.Six:
                    collection = viewModel.FilterModel.Table6MaxCollection;
                    break;
                case EnumTableType.Nine:
                default:
                    collection = viewModel.FilterModel.TableFullRingCollection;
                    break;
            }

            for (int i = 0; i < seats && i < collection.Count; i++)
            {
                var tableItem = collection[i];

                RadDiagramShape player = new RadDiagramShape()
                {
                    DataContext = tableItem,
                    Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(diagram), BackgroundPlayerImage))),
                    Height = PLAYER_HEIGHT,
                    Width = PLAYER_WIDTH,
                    StrokeThickness = 0,
                    BorderThickness = new Thickness(0),
                    IsResizingEnabled = false,
                    IsRotationEnabled = false,
                    IsDraggingEnabled = false,
                    IsManipulationEnabled = false,
                    IsManipulationAdornerVisible = false,
                    IsHitTestVisible = true,
                    FontSize = 13,
                };
                player.X = positions[tableItem.Seat - 1, 0];
                player.Y = positions[tableItem.Seat - 1, 1];


                var indicator = AddActiveIndicator(player, tableItem);

                player.MouseLeftButtonUp += PlayerControl_MouseLeftButtonUp;
                indicator.MouseLeftButtonUp += PlayerControl_MouseLeftButtonUp;

                diagram.AddShape(player);
                diagram.AddShape(indicator);
            }
        }

        private RadDiagramShape AddActiveIndicator(RadDiagramShape player, TableRingItem tableItem)
        {
            RadDiagramShape indicator = new RadDiagramShape()
            {
                DataContext = tableItem,
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

            BindingOperations.ClearBinding(indicator, RadDiagramShape.BackgroundProperty);
            Binding backgroundBinding = new Binding() { Path = new PropertyPath(ReflectionHelper.GetPath<TableRingItem>(o => o.IsChecked)), Mode = BindingMode.TwoWay, Converter = new FilterTableRingBooleanToBrushConverter() };
            indicator.SetBinding(RadDiagramShape.BackgroundProperty, backgroundBinding);

            return indicator;
        }

        private void PlayerControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var dataContext = ((RadDiagramShape)sender).DataContext as TableRingItem;
            if (dataContext != null)
            {
                dataContext.IsChecked = !dataContext.IsChecked;
            }
        }
    }
}
