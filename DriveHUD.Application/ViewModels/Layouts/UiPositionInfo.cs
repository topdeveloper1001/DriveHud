using System;
using System.Windows;

namespace DriveHUD.Application.ViewModels.Layouts
{
    [Serializable]
    public class UiPositionInfo
    {
        public int Seat { get; set; }
        public Point Position { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public UiPositionInfo Clone()
        {
            return new UiPositionInfo
            {
                Seat = Seat,
                Position = new Point {X = Position.X, Y = Position.Y},
                Width = Width,
                Height = Height
            };
        }
    }
}