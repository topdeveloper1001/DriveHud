using DriveHUD.Application.ValueConverters;
using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Log;
using DriveHUD.Common.Reflection;
using Model.Enums;
using Model.Replayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.ViewModels.Replayer
{
    public class ReplayerChipsContainer : BaseViewModel
    {
        private const int CHIP_VIEW_WIDTH = 35;
        private const int CHIP_VIEW_HEIGHT = 35;

        private SortedDictionary<decimal, EnumChipColor> ChipRates = new SortedDictionary<decimal, EnumChipColor>()
        {
            { 5000, EnumChipColor.Red }, { 2000, EnumChipColor.Yellow }, { 1000, EnumChipColor.Grey }, { 500, EnumChipColor.Purple },
            { 100, EnumChipColor.Black }, { 50, EnumChipColor.Orange }, { 10, EnumChipColor.Blue }, { 5, EnumChipColor.Green },
            { 1, EnumChipColor.Grey }, { 0.25m, EnumChipColor.Purple }, { 0.10m, EnumChipColor.Red }, { 0.05m, EnumChipColor.Yellow }
        };

        internal ReplayerChipsContainer( )
        {
            Chips = new ObservableCollection<ChipModel>();

            ChipsShape = new RadDiagramShape()
            {
                Height =  CHIP_VIEW_HEIGHT,
                Width = CHIP_VIEW_WIDTH,
                StrokeThickness = 0,
                BorderThickness = new Thickness(0),
                Tag = "ChipsInfo",
                Visibility = Visibility.Visible,
                Background = new SolidColorBrush(Colors.Transparent)
            };
        }

        internal void UpdateChips(decimal amount)
        {
            ChipsShape.Content = null;
            ConvertToChips(amount);

            if (!Chips.Any())
            {
                return;
            }

            StackPanel sp = new StackPanel();
            sp.HorizontalAlignment = HorizontalAlignment.Left;
            sp.VerticalAlignment = VerticalAlignment.Bottom;

            foreach (var chip in Chips)
            {
                for (int j = 0; j < chip.Count; j++)
                {

                    var chipShape = new RadDiagramShape()
                    {
                        Height = 25,
                        Width = 25,
                        StrokeThickness = 0,
                        BorderThickness = new Thickness(0),
                        Tag = "ChipsInfo",
                        Visibility = Visibility.Visible,
                        DataContext = chip,
                        Margin = new Thickness(0, -22, 0, 0),
                        ZIndex = sp.Children.Count
                    };

                    Binding chipBinding = new Binding(ReflectionHelper.GetPath<ChipModel>(o => o.ChipColor)) { Source = chip, Mode = BindingMode.OneTime, Converter = new ColorToChipsConverter(), ConverterParameter = chipShape };
                    chipShape.SetBinding(Control.BackgroundProperty, chipBinding);

                    sp.Children.Insert(0, chipShape);
                }
            }

            ChipsShape.Height = sp.Height;
            ChipsShape.Content = sp;

        }

        private void ConvertToChips(decimal value)
        {
            Chips.Clear();

            if (value == 0)
            {
                return;
            }

            foreach (var key in ChipRates.Keys.Reverse())
            {
                int chipAmount = (int)(value / key);
                if (chipAmount > 0)
                {
                    Chips.Add(new ChipModel() { ChipColor = ChipRates[key], Count = chipAmount });
                    value %= key;

                    if (value == 0)
                    {
                        break;
                    }
                }
            }
        }

        #region Properties
        private RadDiagramShape _chipsShape;
        private ObservableCollection<ChipModel> _chips;

        public RadDiagramShape ChipsShape
        {
            get { return _chipsShape; }
            set { SetProperty(ref _chipsShape, value); }
        }

        public ObservableCollection<ChipModel> Chips
        {
            get { return _chips; }
            set { _chips = value; }
        }

        #endregion
    }
}
