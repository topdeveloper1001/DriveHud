using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DriveHUD.Controls.ActionTreeControl
{
    /// <summary>
    /// Interaction logic for NodeControl.xaml
    /// </summary>
    public partial class ActionTree : UserControl
    {
        private int _rows;

        public int Rows
        {
            get { return _rows; }
            set { _rows = value; }
        }

        public object[] ItemsSource
        {
            get { return (object[])GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public ActionTree LeftTree
        {
            get { return (ActionTree)GetValue(LeftTreeProperty); }
            set { SetValue(LeftTreeProperty, value); }
        }

        public ActionTree RightTree
        {
            get { return (ActionTree)GetValue(RightTreeProperty); }
            set { SetValue(RightTreeProperty, value); }
        }

        public static readonly DependencyProperty RightTreeProperty =
            DependencyProperty.Register("RightTree", typeof(ActionTree), typeof(ActionTree), new PropertyMetadata(null, new PropertyChangedCallback(OnRightTreeChanged)));

        public static readonly DependencyProperty LeftTreeProperty =
            DependencyProperty.Register("LeftTree", typeof(ActionTree), typeof(ActionTree), new PropertyMetadata(null, new PropertyChangedCallback(OnLeftTreeChanged)));

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<object>), typeof(ActionTree), new PropertyMetadata(new PropertyChangedCallback(OnItemsSourcePropertyChanged)));

        private static void OnItemsSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as ActionTree;
            if (control != null)
            {
                control.OnItemsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
                control.UpdateConnectors();
                control.Rows = control.ItemsSource.Count();
            }
        }

        private void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            var oldValueINotifyCollectionChanged = oldValue as INotifyCollectionChanged;
            if (null != oldValueINotifyCollectionChanged)
            {
                oldValueINotifyCollectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(newValueINotifyCollectionChanged_CollectionChanged);
            }

            var newValueINotifyCollectionChanged = newValue as INotifyCollectionChanged;
            if (null != newValueINotifyCollectionChanged)
            {
                newValueINotifyCollectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(newValueINotifyCollectionChanged_CollectionChanged);
            }
        }

        void newValueINotifyCollectionChanged_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var control = sender as ActionTree;
            if (control != null)
            {
                Debug.WriteLine("new collectionChanged");
                control.UpdateConnectors();
            }
        }

        private static void OnLeftTreeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as ActionTree;
            if (control != null)
            {
                control.UpdateConnectors();
            }
        }

        private static void OnRightTreeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as ActionTree;
            if (control != null)
            {
                control.UpdateConnectors();
            }
        }

        private void OnItemSelectedChanged(object sender, RoutedEventArgs e)
        {
            var control = sender as ActionTree;
            if (control != null)
            {
                control.UpdateConnectors();
                if (control.LeftTree != null)
                {
                    control.LeftTree.UpdateConnectors();
                }
                if (control.RightTree != null)
                {
                    control.RightTree.UpdateConnectors();
                }
            }
        }

        public void UpdateConnectors()
        {
            UpdateLeftTreeConnectors();
            UpdateRightTreeConnectors();
        }

        private void UpdateRightTreeConnectors()
        {
            var rightTree = RightTree as ActionTree;
            bool isVisible = rightTree != null && rightTree.ItemsSource != null && rightTree.ItemsSource.Any();
            var itemsSource = ItemsSource.OfType<ActionTreeNode>().ToList();

            for (int i = 0; i < itemsSource.Count(); i++)
            {
                var node = itemsSource.ElementAt(i);
                if (node == null)
                    continue;

                node.IsRightLineVisible = isVisible && !node.IsEmptyNode ? Visibility.Visible : Visibility.Collapsed;
                if (isVisible)
                {
                    var rightNodes = rightTree.ItemsSource.OfType<ActionTreeNode>().ToList();

                    node.IsTopRightLineVisible = i == 0 ? Visibility.Collapsed : Visibility.Visible;
                    node.IsBottomRightLineVisible = (i == itemsSource.Count - 1) && (rightNodes.Count <= itemsSource.Count) ? Visibility.Collapsed : Visibility.Visible;

                    if (rightNodes != null && rightNodes.Any(x => x.IsSelected))
                    {
                        int maxSelectedIndex = Math.Max(itemsSource.FindLastIndex(x => x.IsSelected), rightNodes.FindLastIndex(x => x.IsSelected));
                        int minSelectedIndex = Math.Min(itemsSource.FindIndex(x => x.IsSelected), rightNodes.FindIndex(x => x.IsSelected));

                        node.IsRightLineHighlighted = node.IsSelected && rightNodes.Any(x => x.IsSelected);
                        node.IsTopRightLineHighlighted = (minSelectedIndex >= 0) && (i > minSelectedIndex) && (i <= maxSelectedIndex);
                        node.IsBottomRightLineHighlighted = (minSelectedIndex >= 0) && (i >= minSelectedIndex) && (i < maxSelectedIndex);
                    }
                    else
                    {
                        node.IsRightLineHighlighted = false;
                        node.IsTopRightLineHighlighted = false;
                        node.IsBottomRightLineHighlighted = false;
                    }
                }
            }
        }

        private void UpdateLeftTreeConnectors()
        {
            var leftTree = LeftTree as ActionTree;
            bool isVisible = leftTree != null && leftTree.ItemsSource != null && leftTree.ItemsSource.Any();

            for (int i = 0; i < ItemsSource.Count(); i++)
            {
                var node = ItemsSource.ElementAt(i) as ActionTreeNode;
                if (node == null)
                    continue;

                node.IsLeftLineVisible = isVisible ? Visibility.Visible : Visibility.Collapsed;

                if (isVisible)
                {
                    node.IsLeftLineHighlighted = node.IsSelected && leftTree.ItemsSource.OfType<ActionTreeNode>().Any(x => x.IsSelected);
                }
            }
        }

        public ActionTree()
        {
            InitializeComponent();
            this.AddHandler(ActionTreeNode.IsSelectedChangedEvent, new RoutedEventHandler(OnItemSelectedChanged));
        }

        ~ActionTree()
        {
            this.RemoveHandler(ActionTreeNode.IsSelectedChangedEvent, new RoutedEventHandler(OnItemSelectedChanged));
        }
    }
}
