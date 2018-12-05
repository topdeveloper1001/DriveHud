//-----------------------------------------------------------------------
// <copyright file="CollectionViewGroupConverter.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace DriveHUD.Application.ValueConverters
{
    public class CollectionViewGroupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is IEnumerable<object> enumerable))
            {
                return value;
            }

            var collectionViewGroup = new ObservableCollection<CollectionViewGroupWrapper>(enumerable
                .OfType<CollectionViewGroup>()
                .Select(x => new CollectionViewGroupWrapper(x)));

            if (!(value is INotifyCollectionChanged collection))
            {
                return collectionViewGroup;
            }

            collection.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
                {
                    var itemsToAdd = e.NewItems.OfType<CollectionViewGroup>().Select(x => new CollectionViewGroupWrapper(x));
                    collectionViewGroup.AddRange(itemsToAdd);
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
                {
                    var itemsToRemove = e.OldItems.OfType<CollectionViewGroup>();

                    foreach (var itemToRemove in itemsToRemove)
                    {
                        var collectionViewGroupItem = collectionViewGroup.FirstOrDefault(x => x.Name == itemToRemove.Name);

                        if (collectionViewGroupItem != null)
                        {
                            collectionViewGroup.Remove(collectionViewGroupItem);
                        }
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    collectionViewGroup.Clear();
                }
            };

            return collectionViewGroup;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}