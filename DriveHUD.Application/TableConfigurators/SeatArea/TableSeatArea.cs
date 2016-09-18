using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model.Enums;
using Model.Events;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.TableConfigurators
{
    internal class TableSeatArea : BaseViewModel, ITableSeatArea
    {
        #region Constructor

        internal TableSeatArea()
        {
            RelativePosition = new Point();
            StartPoint = new Point();
            SeatShape = new RadDiagramShape();
        }

        internal TableSeatArea(int seat, double height, double width, double relativeY, double relativeX, string geometryData) : this()
        {
            SeatNumber = seat;

            SeatShape = new RadDiagramShape()
            {
                Height = height,
                Width = width,
                Geometry = Geometry.Parse(geometryData),
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                IsEditable = false,
                IsManipulationEnabled = false,
                IsConnectorsManipulationEnabled = false,
                IsDraggingEnabled = false,
                IsResizingEnabled = false,
                IsRotationEnabled = false,
                IsManipulationAdornerVisible = false,
                DataContext = this,
            };

            RelativePosition = new Point(relativeX, relativeY);
        }

        #endregion

        #region Properties

        private int _seatNumber;
        private bool _isPrefferedSeat;
        private RadDiagramShape _seatShape;
        private Point _relativePosition;
        private Point _startPoint;
        private EnumPokerSites _pokerSite;
        private EnumTableType _tableType;
        private bool _isContextMenuEnabled;

        public Point RelativePosition
        {
            get { return _relativePosition; }
            set
            {
                _relativePosition = value;
                UpdateSeatShapePosition();
            }
        }

        public Point StartPoint
        {
            get { return _startPoint; }
            set
            {
                _startPoint = value;
                UpdateSeatShapePosition();
            }
        }

        public bool IsPreferredSeat
        {
            get { return _isPrefferedSeat; }
            set
            {
                _isPrefferedSeat = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }

        public int SeatNumber
        {
            get { return _seatNumber; }
            set { _seatNumber = value; }
        }

        public RadDiagramShape SeatShape
        {
            get { return _seatShape; }
            set { _seatShape = value; }
        }

        public EnumPokerSites PokerSite
        {
            get
            {
                return _pokerSite;
            }

            set
            {
                _pokerSite = value;
            }
        }

        public EnumTableType TableType
        {
            get
            {
                return _tableType;
            }

            set
            {
                _tableType = value;
            }
        }

        public bool IsContextMenuEnabled
        {
            get
            {
                return _isContextMenuEnabled;
            }

            private set
            {
                _isContextMenuEnabled = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }

        public bool IsVisible
        {
            get { return IsContextMenuEnabled && IsPreferredSeat; }
        }

        #endregion

        public void SetContextMenuEnabled(bool isEnabled)
        {
            if (isEnabled)
            {
                var radContextMenu = new RadContextMenu();
                var menuItem = new RadMenuItem() { Header = "Choose Seat #" + SeatNumber.ToString() };
                menuItem.Click += MenuItem_Click;

                radContextMenu.Items.Add(menuItem);
                RadContextMenu.SetContextMenu(SeatShape, radContextMenu);

                IsContextMenuEnabled = true;
            }
            else
            {
                RadContextMenu.SetContextMenu(SeatShape, null);

                IsContextMenuEnabled = false;
            }
        }

        private void MenuItem_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            IsPreferredSeat = true;
            TableSeatAreaHelpers.SetPrefferedSeatSetting(SeatNumber, TableType, PokerSite);

            ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<PreferredSeatChangedEvent>().Publish(new PreferredSeatChangedEventArgs(this.SeatNumber));
        }

        public void Initialize()
        {
            IsPreferredSeat = false;
        }

        private void UpdateSeatShapePosition()
        {
            if (SeatShape == null)
            {
                return;
            }
            SeatShape.X = StartPoint.X + RelativePosition.X;
            SeatShape.Y = StartPoint.Y + RelativePosition.Y;
        }

    }
}
