using System;
using System.Windows;

namespace DriveHUD.Application.ViewModels.Layouts
{
    [Serializable]
    public class HudPositionInfo
    {
        public int Seat { get; set; }
        public Point Position { get; set; }

        public HudPositionInfo Clone()
        {
            return new HudPositionInfo {Position = new Point {X = Position.X, Y = Position.Y}, Seat = Seat};
        }
    }
}