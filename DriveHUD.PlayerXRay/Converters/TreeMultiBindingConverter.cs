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

using DriveHUD.Common.Linq;
using DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace DriveHUD.PlayerXRay.Converters
{
    public class TreeMultiBindingConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var result = new ReactiveList<object>();

            foreach (var obj in values)
            {
                if (obj is ReactiveList<InnerGroupObject>)
                {
                    var groups = (ReactiveList<InnerGroupObject>)obj;

                    foreach (var group in groups)
                    {
                        result.Add(group);
                    }

                    groups.Changed.Subscribe(x => OnCollectionChanged<InnerGroupObject>(result, x));
                }
                else if (obj is ReactiveList<NoteObject>)
                {
                    var notes = (ReactiveList<NoteObject>)obj;

                    foreach (var note in notes)
                    {
                        result.Add(note);
                    }

                    notes.Changed.Subscribe(x => OnCollectionChanged<NoteObject>(result, x));
                }
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new[] { DependencyProperty.UnsetValue };
        }

        private void OnCollectionChanged<T>(ReactiveList<object> result, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                var addedItems = e.NewItems.OfType<T>();

                if (addedItems != null)
                {
                    addedItems.ForEach(p => result.Add(p));
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                var removedItems = e.OldItems.OfType<T>();

                if (removedItems != null)
                {
                    removedItems.ForEach(p => result.Remove(p));
                }
            }
        }

        #endregion
    }
}