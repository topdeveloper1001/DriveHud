using DriveHUD.Application.ValueConverters;
using DriveHUD.Application.ViewModels.Settings;
using DriveHUD.Common.Reflection;
using DriveHUD.Common.Wpf.Converters;
using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Telerik.Windows.Controls;

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


        public void ConfigureTable(RadDiagram diagram, SettingsSiteViewModel viewModel, EnumTableType tableType)
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

            CreatePlayerLabels(diagram, viewModel, (int)tableType);
        }

        private void CreatePlayerLabels(RadDiagram diagram, SettingsSiteViewModel viewModel, int seats)
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
                    IsResizingEnabled = false,
                    IsRotationEnabled = false,
                    IsDraggingEnabled = false,
                    IsManipulationEnabled = false,
                    IsManipulationAdornerVisible = false,
                    IsHitTestVisible = true,
                    FontSize = 13,
                    DataContext = viewModel.SelectedSiteViewModel,
                    Tag = i + 1,
                    Cursor = Cursors.Hand
                };
                player.X = positions[i, 0];
                player.Y = positions[i, 1];

                var indicator = AddActiveIndicator(player, viewModel, i + 1);

                player.MouseLeftButtonUp += PlayerControl_MouseLeftButtonUp;
                indicator.MouseLeftButtonUp += PlayerControl_MouseLeftButtonUp;

                diagram.AddShape(player);
                diagram.AddShape(indicator);
            }
        }

        private RadDiagramShape AddActiveIndicator(RadDiagramShape player, SettingsSiteViewModel viewModel, int seat)
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
                Y = player.Y + activeIndicatorRelativePosition.Y,
                DataContext = viewModel.SelectedSiteViewModel,
                Tag = seat,
                Cursor = Cursors.Hand
            };

            BindingOperations.ClearBinding(indicator, RadDiagramShape.BackgroundProperty);

            MultiBinding backgroundBinding = new MultiBinding();
            backgroundBinding.Converter = new MultiBooleanAndToBrushConverter();

            Binding seatBinding = new Binding() { Path = new PropertyPath(ReflectionHelper.GetPath<SiteViewModel>(o => o.SelectedSeatModel.PreferredSeat)), Mode = BindingMode.TwoWay, Converter = new ParameterToBoolConverter(), ConverterParameter = seat };
            Binding enabledBinding = new Binding() { Path = new PropertyPath(ReflectionHelper.GetPath<SiteViewModel>(o => o.SelectedSeatModel.IsPreferredSeatEnabled)) };

            backgroundBinding.Bindings.Add(seatBinding);
            backgroundBinding.Bindings.Add(enabledBinding);

            indicator.SetBinding(RadDiagramShape.BackgroundProperty, backgroundBinding);

            return indicator;
        }

        private void PlayerControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is RadDiagramShape indicator)
            {
                int seat = -1;

                var viewModel = indicator.DataContext as SiteViewModel;

                if (int.TryParse(indicator.Tag.ToString(), out seat) && (viewModel != null))
                {
                    if (seat != -1 && (viewModel.SelectedSeatModel.PreferredSeat != seat || !viewModel.SelectedSeatModel.IsPreferredSeatEnabled))
                    {
                        viewModel.SelectedSeatModel.PreferredSeat = seat;
                        viewModel.SelectedSeatModel.IsPreferredSeatEnabled = true;
                    }
                    else
                    {
                        viewModel.SelectedSeatModel.PreferredSeat = -1;
                        viewModel.SelectedSeatModel.IsPreferredSeatEnabled = false;
                    }

                    viewModel.RaisePropertyChanged();
                }
            }
        }


        protected abstract string GetBackgroundImage(EnumTableType tableType);
    }
}
