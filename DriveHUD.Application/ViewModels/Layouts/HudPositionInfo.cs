using System;
using System.Windows;
using System.Xml.Serialization;

namespace DriveHUD.Application.ViewModels.Layouts
{
    [Serializable]
    public class HudPositionInfo
    {
        public int Seat { get; set; }
        public Point Position { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public HudPositionInfo Clone()
        {
            return new HudPositionInfo
            {
                Position = new Point {X = Position.X, Y = Position.Y},
                Width = Width,
                Height = Height,
                Seat = Seat
            };
        }
    }
}