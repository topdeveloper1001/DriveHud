//-----------------------------------------------------------------------
// <copyright file="Poker888TableConfigurator.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels;
using DriveHUD.Entities;
using System.Collections.Generic;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using System.Linq;
using Model.Enums;
using System.Drawing;
using System.Windows.Media;
using System.Windows;

namespace DriveHUD.Application.TableConfigurators
{
    internal class Poker888TableConfigurator : CommonTableConfigurator
    {
        public override EnumPokerSites Type
        {
            get { return EnumPokerSites.Poker888; }
        }

        protected override ITableSeatAreaConfigurator TableSeatAreaConfigurator
        {
            get
            {
                return new Poker888TableSeatAreaConfigurator();
            }
        }

        protected override Dictionary<int, int[,]> GetPredefinedLabelPositions()
        {
            var predefinedLablelPositions = new Dictionary<int, int[,]>
            {
                // done
                { 2, new int[,] { { 355, 118 }, { 355, 409 } } },
                // done
                { 3, new int[,] { { 636, 262 }, { 355, 409 }, { 72, 262 } } },
                // done
                { 4, new int[,] { { 355, 118 }, { 636, 262 }, { 355, 409 }, { 72, 262 } } },
                // done
                { 5, new int[,] { { 490, 118 }, { 636, 318 }, { 355, 409 }, { 72, 318 }, { 220, 118 } } },
                // done
                { 6, new int[,] { { 422, 118 }, { 636, 262 }, { 422, 409 }, { 264, 409 }, { 72, 262 }, { 264, 118 } } },
                // done
                { 8, new int[,] { { 422, 118 }, { 636, 211 }, { 636, 318 }, { 422, 409 }, { 264, 409 }, { 72, 318 }, { 72, 211 }, { 264, 118 } } },
                // done
                { 9, new int[,] { { 415, 118 }, { 636, 211 }, { 636, 318 }, { 490, 409 }, { 355, 409 }, { 220, 409 }, { 72, 318 }, { 72, 211 }, { 273, 118 }  } },
                // done
                { 10, new int[,] { { 490, 118 }, { 636, 211 }, { 636, 318 }, { 490, 409 }, { 355, 409 }, { 220, 409 }, { 72, 318 }, { 72, 211 }, { 220, 118 }, { 355, 118 } } }
            };

            return predefinedLablelPositions;
        }

        protected override Dictionary<int, int[,]> GetPredefinedMarkersPositions()
        {
            var predefinedPositions = new Dictionary<int, int[,]>
            {
                { 2, new int[,] { { 425, 87 }, { 425, 396 } } },
                { 3, new int[,] { { 425, 87 }, { 615, 396 }, { 235, 396 } } },
                { 4, new int[,] { { 743, 174 }, { 743, 316 }, { 147, 316 }, { 147, 174 } } },
                { 5, new int[,] { { 743, 174 }, { 743, 316 }, { 147, 316 }, { 147, 174 }, { 147, 174 } } },
                { 6, new int[,] { { 425, 87 }, { 743, 174 }, { 743, 316 }, { 425, 396 }, { 147, 316 }, { 147, 174 } } },
                { 8, new int[,] { { 425, 87 }, { 614, 87 }, { 743, 174 }, { 614, 396 }, { 425, 396 }, { 235, 396 }, { 147, 174 }, { 235, 87 }  } },
                { 9, new int[,] { { 425, 87 }, { 614, 87 }, { 743, 174 }, { 743, 316 }, { 614, 396 }, { 235, 396 }, { 147, 316 }, { 147, 174 }, { 235, 87 } } },
                { 10, new int[,] { { 425, 87 }, { 614, 87 }, { 743, 174 }, { 743, 316 }, { 614, 396 }, { 425, 396 }, { 235, 396 }, { 147, 316 }, { 147, 174 }, { 235, 87 } } }
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
                    IsDraggingEnabled = false
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