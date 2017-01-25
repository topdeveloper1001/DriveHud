using System.Windows;

using DriveHUD.Application.ViewModels;

namespace DriveHUD.Application.ViewModels
{
    /// <summary>
    /// Interaction logic for EditHandNote.xaml
    /// </summary>
    public partial class HandNoteView : Window
    {        
        public HandNoteView(HandNoteViewModel vm)
        {
            InitializeComponent();
            vm.CloseAction = this.Close;
            DataContext = vm;
        }
    }
}
