using System;
using System.Windows;
using System.Windows.Threading;
using AcePokerSolutions.BusinessHelper.ApplicationSettings;

namespace AcePokerSolutions.PlayerXRay.CustomControls
{
    public static class MessageBoxHelper
    {
        public static void ShowErrorMessageBox(string message)
        {
            ShowMessageBox(message, "Error", MessageBoxImage.Error, NotesAppSettingsHelper.MainWindow);
        }

        public static void ShowErrorMessageBox(string message, Window owner)
        {
            ShowMessageBox(message, "Error", MessageBoxImage.Error, owner);
        }

        public static void ShowInfoMessageBox(string message)
        {
            ShowMessageBox(message, "Info", MessageBoxImage.Information, NotesAppSettingsHelper.MainWindow);
        }

        public static void ShowWarningMessageBox(string message)
        {
            ShowMessageBox(message, "Warning", MessageBoxImage.Warning, NotesAppSettingsHelper.MainWindow);
        }

        public static void ShowWarningMessageBox(string message, Window owner)
        {
            ShowMessageBox(message, "Warning", MessageBoxImage.Warning, owner);
        }

        private static void ShowMessageBox(string message, string header, MessageBoxImage icon, DispatcherObject owner)
        {
            owner.Dispatcher.Invoke(new Action(() => UIControls.MessageBox.MessageBox.Show(message, header, MessageBoxButton.OK, icon)));
        }

        public static bool ShowYesNoDialogBox(string message, Window owner)
        {
            bool result = false;

            owner.Dispatcher.Invoke(new Action(delegate
            {
                result =
                    UIControls.MessageBox.MessageBox.Show(message, "Confirmation dialog",
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Question) ==
                    MessageBoxResult.Yes;
            }));

            return result;
        }
    }
}
