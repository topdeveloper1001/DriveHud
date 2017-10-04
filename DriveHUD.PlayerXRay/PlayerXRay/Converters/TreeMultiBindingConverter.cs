//-----------------------------------------------------------------------
// <copyright file="TreeMultiBindingConverter.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DriveHUD.PlayerXRay.Converters
{
    public class TreeMultiBindingConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var result = new List<object>();

            foreach (var obj in values)
            {
                if (obj is IEnumerable<InnerGroupObject>)
                {
                    foreach (var group in (IEnumerable<InnerGroupObject>)obj)
                    {
                        result.Add(group);
                    }
                }
                else if (obj is IEnumerable<NoteObject>)
                {
                    foreach (var note in (IEnumerable<NoteObject>)obj)
                    {
                        result.Add(note);
                    }
                }
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new[] { DependencyProperty.UnsetValue };
        }

        #endregion
    }
}