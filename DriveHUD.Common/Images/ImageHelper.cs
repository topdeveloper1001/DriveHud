//-----------------------------------------------------------------------
// <copyright file="ImageHelper.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace DriveHUD.Common.Images
{
    public class ImageHelper
    {
        /// <summary>
        /// Copy and resize image to destination folder
        /// </summary>
        /// <param name="path">Path to image</param>
        /// <param name="destination">Destination folder</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns></returns>
        public static string CopyAndProcessImage(string path, string destination, int width, int height)
        {
            if (!File.Exists(path) || !Directory.Exists(destination) || width == 0 || height == 0)
            {
                return null;
            }

            var source = Path.GetDirectoryName(path);

            if (source.Equals(destination, StringComparison.InvariantCultureIgnoreCase))
            {
                return path;
            }

            var destinationFile = Path.Combine(destination, Path.GetFileName(path));
  
            var image = new Bitmap(path);

            var resizedImage = ResizeImage(image, width, height);

            resizedImage.Save(destinationFile);

            return destinationFile;
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}