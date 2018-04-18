//-----------------------------------------------------------------------
// <copyright file="IconExtension.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Wpf.Helpers;
using System;
using System.Windows.Markup;

namespace DriveHUD.Common.Wpf.MarkupExtensions
{
    /// <summary>
    /// Simple extension for icon, to let you choose icon with specific size.
    /// Usage sample:
    /// Image Stretch="None" Source="{common:Icon /Controls;component/icons/custom.ico, 16}"
    /// </summary> 
    public class IconExtension : MarkupExtension
    {
        private string _source;

        public string Source
        {
            get
            {
                return _source;
            }
            set
            {
                // Have to make full pack URI from short form, so System.Uri recognizes it.
                _source = "pack://application:,,," + value;
            }
        }

        public int Size { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return IconHelper.CreateIcon(Source, Size);
        }

        public IconExtension(string source, int size)
        {
            Source = source;
            Size = size;
        }

        public IconExtension() { }
    }
}