using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Enums;
using Telerik.Windows.Controls;
using DriveHUD.Application.ViewModels;
using System.Windows.Controls;
using DriveHUD.Entities;
using DriveHUD.Application.TableConfigurators.SeatArea;

namespace DriveHUD.Application.TableConfigurators
{
    internal class AmericasCardroomTableConfigurator : CommonTableConfigurator
    {
        public override EnumPokerSites Type
        {
            get { return EnumPokerSites.AmericasCardroom; }
        }

        protected override ITableSeatAreaConfigurator TableSeatAreaConfigurator
        {
            get
            {
                return new AmericasCardroomTableSeatAreaConfigurator();
            }
        }

        protected override Dictionary<int, int[,]> GetPredefinedLabelPositions()
        {
            var predefinedLablelPositions = new Dictionary<int, int[,]>
            {
                { 2, new int[,] { { 355, 118 }, { 355, 409 } } },
                { 3, new int[,] { { 636, 211 }, { 355, 409 }, { 72, 211 } } },
                { 4, new int[,] { { 352, 105 }, { 638, 180 }, { 352, 411 }, { 96, 180 } } },
                { 6, new int[,] { { 352, 105 }, { 638, 180 }, { 638, 353 }, { 352, 411 }, { 96, 353 }, { 96, 180 } } },
                { 8, new int[,] { { 352, 105 }, { 636, 211 }, { 636, 318 }, { 490, 409 }, { 355, 409 }, { 220, 409 }, { 72, 318 }, { 72, 211 }  } },
                { 9, new int[,] { { 415, 118 }, { 636, 211 }, { 636, 318 }, { 490, 409 }, { 355, 409 }, { 220, 409 }, { 72, 318 }, { 72, 211 }, { 273, 118 }  } },
            };

            return predefinedLablelPositions;
        }

        protected override Dictionary<int, int[,]> GetPredefinedMarkersPositions()
        {
            var predefinedPositions = new Dictionary<int, int[,]>
            {
                { 2, new int[,] { { 425, 87 }, { 425, 396 } } },
                { 3, new int[,] { { 614, 87 }, { 614, 396 }, { 147, 174 } } },
                { 4, new int[,] { { 425, 87 }, { 743, 174 }, { 425, 396 }, { 147, 174 } } },
                { 6, new int[,] { { 425, 87 }, { 743, 174 }, { 743, 316 }, { 425, 396 }, { 147, 316 }, { 147, 174 } } },
                { 8, new int[,] { { 425, 87 }, { 614, 87 }, { 743, 174 }, { 743, 316 }, { 614, 396 }, { 235, 396 }, { 147, 316 }, { 147, 174 } } },
                { 9, new int[,] { { 425, 87 }, { 614, 87 }, { 743, 174 }, { 743, 316 }, { 614, 396 }, { 235, 396 }, { 147, 316 }, { 147, 174 }, { 235, 87 } } },
            };

            return predefinedPositions;
        }

        protected override void CreatePreferredSeatMarkers(RadDiagram diagram, HudTableViewModel hudTable, int seats)
        {
            var predefinedPositions = GetPredefinedLabelPositions();

            if (!predefinedPositions.ContainsKey(seats))
            {
                return;
            }

            for (int i = 0; i < seats; i++)
            {
                var hudElementPositionX = predefinedPositions[seats][i, 0] + labelElementWidth;
                var hudElementPositionY = predefinedPositions[seats][i, 1] - 35;
                var datacontext = hudTable.TableSeatAreaCollection?.ElementAt(i);

                var shape = new RadDiagramShape
                {
                    Height = 30,
                    Width = 30,
                    IsEnabled = true,
                    SnapsToDevicePixels = true,
                    X = hudElementPositionX,
                    Y = hudElementPositionY,
                    DataContext = datacontext,
                    Template = App.Current.Resources["PreferredSeatControlTemplate"] as ControlTemplate,
                };

                diagram.Items.Add(shape);
            }
        }

        protected override IEnumerable<ITableSeatArea> CreateSeatAreas(RadDiagram diagram, HudTableViewModel hudTable, int seats)
        {
            var seatAreas = TableSeatAreaConfigurator.GetTableSeatAreas((EnumTableType)seats);
            var tableSeatSetting = TableSeatAreaHelpers.GetSeatSetting((EnumTableType)seats, Type);

            foreach (var v in seatAreas)
            {
                v.StartPoint = new System.Windows.Point(0, 0);
                v.PokerSite = Type;
                v.TableType = (EnumTableType)seats;
                v.SetContextMenuEnabled(tableSeatSetting.IsPreferredSeatEnabled);

                if (v.SeatNumber == tableSeatSetting.PreferredSeat)
                {
                    v.IsPreferredSeat = true;
                }
                else
                {
                    v.IsPreferredSeat = false;
                }

                diagram.AddShape(v.SeatShape);

            }
            hudTable.TableSeatAreaCollection = new System.Collections.ObjectModel.ObservableCollection<ITableSeatArea>(seatAreas);
            return seatAreas;
        }
    }
}
