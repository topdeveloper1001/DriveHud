using System;
using System.Collections.Generic;
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

namespace DriveHUD.Application.Views.Replayer
{
    /// <summary>
    /// Interaction logic for ReplayerHudPanel.xaml
    /// </summary>
    public partial class ReplayerHudPanel : UserControl
    {
        private System.Windows.Point _startPoint;
        private bool _dragged = false;

        public ReplayerHudPanel()
        {
            InitializeComponent();
            this.PreviewMouseMove += HudRichPanel_PreviewMouseMove;
            this.PreviewMouseRightButtonUp += HudRichPanel_PreviewMouseRightButtonUp;
            this.AddHandler(UserControl.PreviewMouseRightButtonDownEvent, new MouseButtonEventHandler(HudRichPanel_PreviewMouseRightButtonDown), true);
        }

        private void HudRichPanel_PreviewMouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_dragged)
            {
                e.Handled = true;
            }
            _dragged = false;
        }

        private void HudRichPanel_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                System.Windows.Point position = e.GetPosition(null);

                if (Math.Abs(position.X - _startPoint.X) > System.Windows.SystemParameters.MinimumHorizontalDragDistance ||
                        Math.Abs(position.Y - _startPoint.Y) > System.Windows.SystemParameters.MinimumVerticalDragDistance)
                {
                    _dragged = true;
                }
            }
        }

        private void HudRichPanel_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
            _dragged = false;
        }
    }
}
