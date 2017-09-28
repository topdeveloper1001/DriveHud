#region Usings

using System.Windows.Input;

#endregion

namespace AcePokerSolutions.HUDHelper
{
    /// <summary>
    /// Interaction logic for HudWindow.xaml
    /// </summary>
    public partial class HudWindow
    {
        public HudWindow()
        {
            InitializeComponent();
        }

        private void WindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}