using System;
using System.Collections.Generic;
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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DriveHUD.Controls.ActionTreeControl
{
    /// <summary>
    /// Interaction logic for ActionTreeNode.xaml
    /// </summary>
    [ContentProperty("UserContent")]
    public partial class ActionTreeNode : UserControl
    {
        public bool IsLeftLineHighlighted
        {
            get { return (bool)GetValue(IsLeftLineHighlightedProperty); }
            set { SetValue(IsLeftLineHighlightedProperty, value); }
        }

        public bool IsRightLineHighlighted
        {
            get { return (bool)GetValue(IsRightLineHighlightedProperty); }
            set { SetValue(IsRightLineHighlightedProperty, value); }
        }

        public bool IsTopRightLineHighlighted
        {
            get { return (bool)GetValue(IsTopRightLineHighlightedProperty); }
            set { SetValue(IsTopRightLineHighlightedProperty, value); }
        }

        public bool IsBottomRightLineHighlighted
        {
            get { return (bool)GetValue(IsBottomRightLineHighlightedProperty); }
            set { SetValue(IsBottomRightLineHighlightedProperty, value); }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public Visibility IsLeftLineVisible
        {
            get { return (Visibility)GetValue(IsLeftLineVisibleProperty); }
            set { SetValue(IsLeftLineVisibleProperty, value); }
        }

        public Visibility IsRightLineVisible
        {
            get { return (Visibility)GetValue(IsRightLineVisibleProperty); }
            set { SetValue(IsRightLineVisibleProperty, value); }
        }

        public Visibility IsTopRightLineVisible
        {
            get { return (Visibility)GetValue(IsTopRightLineVisibleProperty); }
            set { SetValue(IsTopRightLineVisibleProperty, value); }
        }

        public Visibility IsBottomRightLineVisible
        {
            get { return (Visibility)GetValue(IsBottomRightLineVisibleProperty); }
            set { SetValue(IsBottomRightLineVisibleProperty, value); }
        }

        public bool IsEmptyNode
        {
            get { return (bool)GetValue(IsEmptyNodeProperty); }
            set { SetValue(IsEmptyNodeProperty, value); }
        }

        public SolidColorBrush ContentBorderBrush
        {
            get { return (SolidColorBrush)GetValue(ContentBorderBrushProperty); }
            set { SetValue(ContentBorderBrushProperty, value); }
        }

        public SolidColorBrush ContentBackgroundBrush
        {
            get { return (SolidColorBrush)GetValue(ContentBackgroundBrushProperty); }
            set { SetValue(ContentBackgroundBrushProperty, value); }
        }

        public Thickness ContentBorderThickness
        {
            get { return (Thickness)GetValue(ContentBorderThicknessProperty); }
            set { SetValue(ContentBorderThicknessProperty, value); }
        }

        public static readonly DependencyProperty ContentBorderThicknessProperty =
            DependencyProperty.Register("ContentBorderThickness", typeof(Thickness), typeof(ActionTreeNode), new PropertyMetadata(new Thickness(0)));

        public static readonly DependencyProperty ContentBackgroundBrushProperty =
            DependencyProperty.Register("ContentBackgroundBrush", typeof(SolidColorBrush), typeof(ActionTreeNode), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public static readonly DependencyProperty ContentBorderBrushProperty =
            DependencyProperty.Register("ContentBorderBrush", typeof(SolidColorBrush), typeof(ActionTreeNode), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public static readonly DependencyProperty IsEmptyNodeProperty =
            DependencyProperty.Register("IsEmptyNode", typeof(bool), typeof(ActionTreeNode), new PropertyMetadata(false));

        public static readonly DependencyProperty IsBottomRightLineVisibleProperty =
            DependencyProperty.Register("IsBottomRightLineVisible", typeof(Visibility), typeof(ActionTreeNode), new PropertyMetadata(Visibility.Collapsed));

        public static readonly DependencyProperty IsTopRightLineVisibleProperty =
            DependencyProperty.Register("IsTopRightLineVisible", typeof(Visibility), typeof(ActionTreeNode), new PropertyMetadata(Visibility.Collapsed));

        public static readonly DependencyProperty IsRightLineVisibleProperty =
            DependencyProperty.Register("IsRightLineVisible", typeof(Visibility), typeof(ActionTreeNode), new PropertyMetadata(Visibility.Collapsed));

        public static readonly DependencyProperty IsLeftLineVisibleProperty =
            DependencyProperty.Register("IsLeftLineVisible", typeof(Visibility), typeof(ActionTreeNode), new PropertyMetadata(Visibility.Collapsed));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(ActionTreeNode), new PropertyMetadata(false, OnIsSelectedChanged));

        public static readonly DependencyProperty IsBottomRightLineHighlightedProperty =
            DependencyProperty.Register("IsBottomRightLineHighlighted", typeof(bool), typeof(ActionTreeNode), new PropertyMetadata(false));

        public static readonly DependencyProperty IsTopRightLineHighlightedProperty =
            DependencyProperty.Register("IsTopRightLineHighlighted", typeof(bool), typeof(ActionTreeNode), new PropertyMetadata(false));

        public static readonly DependencyProperty IsRightLineHighlightedProperty =
            DependencyProperty.Register("IsRightLineHighlighted", typeof(bool), typeof(ActionTreeNode), new PropertyMetadata(false));

        public static readonly DependencyProperty IsLeftLineHighlightedProperty =
            DependencyProperty.Register("IsLeftLineHighlighted", typeof(bool), typeof(ActionTreeNode), new PropertyMetadata(false));


        public static readonly RoutedEvent IsSelectedChangedEvent =
        EventManager.RegisterRoutedEvent("IsSelectedChanged",
                                           RoutingStrategy.Bubble,
                                           typeof(RoutedEventHandler),
                                           typeof(ActionTreeNode));

        public object UserContent
        {
            get { return contentPresenter.Content; }
            set { contentPresenter.Content = value; }
        }

        public ActionTreeNode()
        {
            InitializeComponent();
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ActionTreeNode node = d as ActionTreeNode;
            if (node == null)
                return;

            node.RaiseEvent(new RoutedEventArgs(IsSelectedChangedEvent, d));
        }
    }
}
