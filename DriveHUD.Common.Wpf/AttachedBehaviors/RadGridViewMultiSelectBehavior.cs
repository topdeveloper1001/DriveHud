//-----------------------------------------------------------------------
// <copyright file="RadGridViewMultiSelectBehavior.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Interactivity;
using Telerik.Windows.Controls;

namespace DriveHUD.Common.Wpf.AttachedBehaviors
{
    public class RadGridViewMultiSelectBehavior : Behavior<RadGridView>
    {
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(nameof(SelectedItems), typeof(INotifyCollectionChanged), typeof(RadGridViewMultiSelectBehavior), new PropertyMetadata(OnSelectedItemsPropertyChanged));

        public INotifyCollectionChanged SelectedItems
        {
            get
            {
                return (INotifyCollectionChanged)GetValue(SelectedItemsProperty);
            }
            set
            {
                SetValue(SelectedItemsProperty, value);
            }
        }

        private static void OnSelectedItemsPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            var collection = args.NewValue as INotifyCollectionChanged;

            var behavior = source as RadGridViewMultiSelectBehavior;

            if (collection != null && behavior != null)
            {
                collection.CollectionChanged += behavior.OnContextSelectedItemsCollectionChanged;
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.SelectedItems.CollectionChanged += OnGridSelectedItemsCollectionChanged;
        }

        private void OnGridSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UnsubscribeFromEvents();

            UpdateItems(AssociatedObject.SelectedItems, SelectedItems as IList);

            SubscribeToEvents();
        }

        private void OnContextSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UnsubscribeFromEvents();

            UpdateItems(SelectedItems as IList, AssociatedObject.SelectedItems);

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            AssociatedObject.SelectedItems.CollectionChanged += OnGridSelectedItemsCollectionChanged;

            if (SelectedItems != null)
            {
                SelectedItems.CollectionChanged += OnContextSelectedItemsCollectionChanged;
            }
        }

        private void UnsubscribeFromEvents()
        {
            AssociatedObject.SelectedItems.CollectionChanged -= OnGridSelectedItemsCollectionChanged;

            if (SelectedItems != null)
            {
                SelectedItems.CollectionChanged -= OnContextSelectedItemsCollectionChanged;
            }
        }

        private static void UpdateItems(IList source, IList target)
        {
            if (source == null || target == null)
            {
                return;
            }

            target.Clear();

            foreach (var obj in source)
            {
                target.Add(obj);
            }
        }
    }
}