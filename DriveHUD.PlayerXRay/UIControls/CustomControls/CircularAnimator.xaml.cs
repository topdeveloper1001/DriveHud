//***********************************************************************
// Assembly         : Xrio.Enterprise.UIControls
// Author           : Cotzo
// Created          : 06-30-2010
//
// Last Modified By : Cotzo
// Last Modified On : 10-11-2010
// Description      : 
//
// Copyright        : (c) Microsoft. All rights reserved.
//***********************************************************************

#region Usings

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

#endregion

namespace AcePokerSolutions.UIControls.CustomControls
{
    public sealed partial class CircularAnimator
    {
        #region Data
        private readonly DispatcherTimer m_animationTimer;
        #endregion

        #region Constructor

        public CircularAnimator()
        {
            InitializeComponent();

            m_animationTimer = new DispatcherTimer(
                DispatcherPriority.Render, Dispatcher) {Interval = new TimeSpan(0, 0, 0, 0, 100)};
        }

        #endregion

        #region Private Methods

        private void Start()
        {
            Cursor = Cursors.AppStarting;
            m_animationTimer.Tick += HandleAnimationTick;
            m_animationTimer.Start();
        }

        private void Stop()
        {
            m_animationTimer.Stop();
            Cursor = Cursors.Arrow;
            m_animationTimer.Tick -= HandleAnimationTick;
        }

        private void HandleAnimationTick(object sender, EventArgs e)
        {
            SpinnerRotate.Angle = (SpinnerRotate.Angle + 36) % 360;
        }

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            const double offset = Math.PI;
            const double step = Math.PI * 2 / 10.0;

            SetPosition(C0, offset, 0.0, step);
            SetPosition(C1, offset, 1.0, step);
            SetPosition(C2, offset, 2.0, step);
            SetPosition(C3, offset, 3.0, step);
            SetPosition(C4, offset, 4.0, step);
            SetPosition(C5, offset, 5.0, step);
            SetPosition(C6, offset, 6.0, step);
            SetPosition(C7, offset, 7.0, step);
            SetPosition(C8, offset, 8.0, step);
        }

        private static void SetPosition(DependencyObject ellipse, double offset,
                                        double posOffSet, double step)
        {
            ellipse.SetValue(Canvas.LeftProperty, 9.5
                                                  + Math.Sin(offset + posOffSet * step) * 9.5);

            ellipse.SetValue(Canvas.TopProperty, 9.5
                                                 + Math.Cos(offset + posOffSet * step) * 9.5);
        }

        private void HandleUnloaded(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        private void HandleVisibleChanged(object sender,
                                          DependencyPropertyChangedEventArgs e)
        {
            bool isVisible = (bool)e.NewValue;

            if (isVisible)
                Start();
            else
                Stop();
        }
        #endregion
    }
}