using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace DriveHUD.PlayerXRay.Helpers
{
    public static class UIHelpers
    {
        public static BitmapSource GetBitmapSource(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return null;
            }

            BitmapSource source = Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            if (source.CanFreeze)
            {
                source.Freeze();
            }

            return source;
        }
    }
}
