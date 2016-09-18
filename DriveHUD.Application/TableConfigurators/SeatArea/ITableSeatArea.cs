using DriveHUD.Entities;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.TableConfigurators
{
    internal interface ITableSeatArea
    {
        RadDiagramShape SeatShape { get; set; }

        Point RelativePosition { get; set; }

        Point StartPoint { get; set; }

        int SeatNumber { get; set; }

        EnumPokerSites PokerSite { get; set; }

        EnumTableType TableType { get; set; }

        bool IsContextMenuEnabled { get; }

        bool IsVisible { get; }

        bool IsPreferredSeat { get; set; }

        void SetContextMenuEnabled(bool isEnabled);

        void Initialize();
    }
}
