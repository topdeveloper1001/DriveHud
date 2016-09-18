//-----------------------------------------------------------------------
// <copyright file="WindowBase.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;

namespace DriveHUD.Updater
{
    public class WindowBase: Window
    {
        #region Private View Members

        private Thumb _thumbResize;
        private Thumb _topThumb;
        private Thumb _btmThumb;
        private Thumb _leftThumb;       
        private Thumb _rightThumb;
        private Button _btClose;
        private Button _btMinimize;
        private Button _btMaximize;
        private DockPanel _dpTitle;        

        #endregion

        public WindowBase()
        {
           
        }

        #region DependencyProperties

        public static DependencyProperty MinimizeButtonProperty = DependencyProperty.Register("MinimizeButton", typeof(Visibility), typeof(WindowBase));
        public static DependencyProperty MaximizeButtonProperty = DependencyProperty.Register("MaximizeButton", typeof(Visibility), typeof(WindowBase));
        public static DependencyProperty IsResizableProperty = DependencyProperty.Register("IsResizable", typeof(bool), typeof(WindowBase), new FrameworkPropertyMetadata(true, onResizableChange));

        public Visibility MinimizeButton
        {
            get
            {
                return (Visibility)this.GetValue(MinimizeButtonProperty);
            }
            set
            {                
                this.SetValue(MinimizeButtonProperty, value);
            }
        }

        public Visibility MaximizeButton
        {
            get
            {
                return (Visibility)this.GetValue(MaximizeButtonProperty);
            }
            set
            {
                this.SetValue(MaximizeButtonProperty, value);
            }
        }

        public bool IsResizable
        {
            get
            {
                return (bool)this.GetValue(IsResizableProperty);
            }
            set
            {
                this.SetValue(IsResizableProperty, value);
            }
        }

        #endregion

        private static void onResizableChange(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            WindowBase window = source as WindowBase;

            if (window != null)
            {
                window.UpdateResizability((bool)e.NewValue);
            }
        }

        private void UpdateResizability(bool val)
        {
            if (_thumbResize != null)
            {
                _thumbResize.Visibility = (val) ? Visibility.Visible : Visibility.Collapsed;
            }
            
            if (_topThumb != null) _topThumb.IsEnabled = val;
            if (_btmThumb != null) _btmThumb.IsEnabled = val;
            if (_leftThumb != null) _leftThumb.IsEnabled = val;
            if (_rightThumb != null) _rightThumb.IsEnabled = val;            
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _thumbResize = (Thumb)this.Template.FindName("thumbResize", this);
            _topThumb = (Thumb)this.Template.FindName("topThumb", this);
            _btmThumb = (Thumb)this.Template.FindName("btmThumb", this);
            _leftThumb = (Thumb)this.Template.FindName("leftThumb", this);
            _rightThumb = (Thumb)this.Template.FindName("rightThumb", this);
         
            _thumbResize.DragDelta += new DragDeltaEventHandler(thumbResize_DragDelta);
            _topThumb.DragDelta += new DragDeltaEventHandler(topThumb_DragDelta);            
            _btmThumb.DragDelta += new DragDeltaEventHandler(btmThumb_DragDelta);                
            _leftThumb.DragDelta += new DragDeltaEventHandler(leftThumb_DragDelta);                
            _rightThumb.DragDelta += new DragDeltaEventHandler(rightThumb_DragDelta);

            this.UpdateResizability(IsResizable);

            _dpTitle = (DockPanel)this.Template.FindName("MWTitleBar", this);
            _dpTitle.MouseLeftButtonDown += TitlePanel_MouseLeftButtonDown;

            _btClose = (Button)this.Template.FindName("btMWClose", this);
            _btMaximize = (Button)this.Template.FindName("btMWMaximize", this);
            _btMinimize = (Button)this.Template.FindName("btMWMinimize", this);

            _btClose.Click += Click_Close;
            _btMaximize.Click += Click_Maximize;
            _btMinimize.Click += Click_Minimize;
        }

        #region base window functions

        void rightThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {            
            if (this.Width >= this.MinWidth)
            {
                this.Width += e.HorizontalChange;
            }
            else
            {
                this.Width = this.MinWidth + 1;
                _thumbResize.ReleaseMouseCapture();
            }
        }

        void leftThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (this.Width >= this.MinWidth)
            {
                this.Width -= e.HorizontalChange;
                this.Left += e.HorizontalChange;
            }
            else
            {
                this.Width = this.MinWidth + 1;
                _thumbResize.ReleaseMouseCapture();
            }
        }

        void btmThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (this.Height >= this.MinHeight)
            {
                this.Height += e.VerticalChange;
            }
            else
            {
                this.Height = this.MinHeight + 1;
                _thumbResize.ReleaseMouseCapture();
            }
        }

        void topThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (this.Height >= this.MinHeight)
            {
                this.Height -= e.VerticalChange;
                this.Top += e.VerticalChange;
            }
            else
            {
                this.Height = this.MinHeight + 1;
                _thumbResize.ReleaseMouseCapture();
            }
        }

        void thumbResize_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (this.Width >= this.MinWidth)
            {
                this.Width += e.HorizontalChange;
            }
            else
            {
                this.Width = this.MinWidth + 1;
                _thumbResize.ReleaseMouseCapture();
            }

            if (this.Height >= this.MinHeight)
            {
                this.Height += e.VerticalChange;
            }
            else
            {
                this.Height = this.MinHeight + 1;
                _thumbResize.ReleaseMouseCapture();
            }
        }

        private void Click_Minimize(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Click_Maximize(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                _btMaximize.Style = (Style)FindResource("MaximizeButton");
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                _btMaximize.Style = (Style)FindResource("MaximizeButtonSel");
            }
        }

        private void Click_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TitlePanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        #endregion
      
    }
}