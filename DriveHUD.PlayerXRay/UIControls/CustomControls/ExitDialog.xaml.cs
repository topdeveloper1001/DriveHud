#region Usings

using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

#endregion

namespace AcePokerSolutions.UIControls.CustomControls
{
    public partial class ExitDialog
    {
        #region Delegates

        public delegate void ExitDialogDelegate(ExitDialogResult result, CancelEventArgs args);

        #endregion

        private readonly CancelEventArgs m_args;

        #region Constructor

        public ExitDialog(CancelEventArgs args)
        {
            m_args = args;
            InitializeComponent();
        }

        #endregion

        #region Button Events

        private void NotifyEvent(ExitDialogResult result)
        {
            Hide();
            if (ExitDialogEvent != null)
                ExitDialogEvent(result, m_args);
            Close();
        }

        private void BtnCancelClick(object sender, RoutedEventArgs e)
        {
            NotifyEvent(ExitDialogResult.Cancel);
        }

        private void BtnRestartClick(object sender, RoutedEventArgs e)
        {
            NotifyEvent(ExitDialogResult.Restart);
        }

        private void BtnQuitClick(object sender, RoutedEventArgs e)
        {
            NotifyEvent(ExitDialogResult.Quit);
        }

        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.C:
                case Key.Escape:
                    NotifyEvent(ExitDialogResult.Cancel);
                    break;
                case Key.R:
                    NotifyEvent(ExitDialogResult.Restart);
                    break;
                case Key.Q:
                case Key.Return:
                    NotifyEvent(ExitDialogResult.Quit);
                    break;
            }
        }

        #endregion

        public event ExitDialogDelegate ExitDialogEvent;

        public enum ExitDialogResult
        {
            Cancel,
            Quit,
            Restart
        }
    }
}