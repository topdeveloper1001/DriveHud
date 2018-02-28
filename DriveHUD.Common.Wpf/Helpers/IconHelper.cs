//-----------------------------------------------------------------------
// <copyright file="IconHelper.cs" company="Ace Poker Solutions">
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
using System.Linq;
using System.Windows.Media.Imaging;

namespace DriveHUD.Common.Wpf.Helpers
{
    public static class IconHelper
    {
        public static BitmapFrame CreateIcon(string source, double size)
        {
            var decoder = BitmapDecoder.Create(new Uri(source),
                                               BitmapCreateOptions.DelayCreation,
                                               BitmapCacheOption.OnDemand);

            var result = decoder.Frames.SingleOrDefault(f => f.Width == size);

            if (result == default(BitmapFrame))
            {
                result = decoder.Frames.OrderBy(f => f.Width).First();
            }

            return result;
        }
    }
}