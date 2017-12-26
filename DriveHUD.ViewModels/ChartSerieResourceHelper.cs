//-----------------------------------------------------------------------
// <copyright file="ChartSerieResourceHelper.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Windows.Media;

namespace DriveHUD.ViewModels
{
    public class ChartSerieResourceHelper
    {
        public Color LineColor { get; set; }

        public Color PointColor { get; set; }

        public Color TrackBallColor { get; set; }

        public Color TooltipColor { get; set; }

        public VisualBrush AreaBrush { get; set; }

        public static ChartSerieResourceHelper GetSeriesBluePalette()
        {
            return new ChartSerieResourceHelper()
            {
                LineColor = (Color)ColorConverter.ConvertFromString("#34519C"),
                PointColor = (Color)ColorConverter.ConvertFromString("#115576"),
                TrackBallColor = (Color)ColorConverter.ConvertFromString("#115576"),
                TooltipColor = (Color)ColorConverter.ConvertFromString("#34519C"),
                AreaBrush = (VisualBrush)System.Windows.Application.Current.FindResource("AreaVisualBrushBlue")
            };
        }

        public static ChartSerieResourceHelper GetSeriesYellowPalette()
        {
            return new ChartSerieResourceHelper()
            {
                LineColor = (Color)ColorConverter.ConvertFromString("#FDE40F"),
                PointColor = (Color)ColorConverter.ConvertFromString("#FFF714"),
                TrackBallColor = (Color)ColorConverter.ConvertFromString("#FFF714"),
                TooltipColor = (Color)ColorConverter.ConvertFromString("#FDE40F"),
                AreaBrush = (VisualBrush)System.Windows.Application.Current.FindResource("AreaVisualBrushYellow")
            };

        }

        public static ChartSerieResourceHelper GetSerieOrangePalette()
        {
            return new ChartSerieResourceHelper()
            {
                LineColor = (Color)ColorConverter.ConvertFromString("#bd5922"),
                PointColor = (Color)ColorConverter.ConvertFromString("#ffdc50"),
                TrackBallColor = (Color)ColorConverter.ConvertFromString("#ffbf43"),
                TooltipColor = (Color)ColorConverter.ConvertFromString("#bd5922"),
                AreaBrush = (VisualBrush)System.Windows.Application.Current.FindResource("AreaVisualBrushOrange")
            };
        }

        public static ChartSerieResourceHelper GetSerieGreenPalette()
        {
            return new ChartSerieResourceHelper()
            {
                LineColor = (Color)ColorConverter.ConvertFromString("#4BA516"),
                PointColor = (Color)ColorConverter.ConvertFromString("#93c940"),
                TrackBallColor = (Color)ColorConverter.ConvertFromString("#92c840"),
                TooltipColor = (Color)ColorConverter.ConvertFromString("#4BA516"),
                AreaBrush = (VisualBrush)System.Windows.Application.Current.FindResource("AreaVisualBrushGreen")
            };
        }

        public static ChartSerieResourceHelper GetSerieRedPalette()
        {            
            return new ChartSerieResourceHelper()
            {
                LineColor = (Color)ColorConverter.ConvertFromString("#e60909"),
                PointColor = (Color)ColorConverter.ConvertFromString("#f32e2e"),
                TrackBallColor = (Color)ColorConverter.ConvertFromString("#f32e2e"),
                TooltipColor = (Color)ColorConverter.ConvertFromString("#e60909"),
                AreaBrush = (VisualBrush)System.Windows.Application.Current.FindResource("AreaVisualBrushOrange")
            };
        }
    }
}