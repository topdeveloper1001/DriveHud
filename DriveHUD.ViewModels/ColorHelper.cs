using System;
using System.Drawing;
using Color = System.Windows.Media.Color;

namespace DriveHUD.ViewModels
{
    public static class ColorHelper
    {
        private static Random randomGen = new Random((int)DateTime.Now.Ticks);
        public static  Color RandomColor()
        {
            KnownColor[] names = (KnownColor[])Enum.GetValues(typeof(KnownColor));
            KnownColor randomColorName = names[randomGen.Next(names.Length)];

            var cl = System.Drawing.Color.FromKnownColor(randomColorName);

            return Color.FromRgb(cl.R, cl.G, cl.B);
        }
    }
}