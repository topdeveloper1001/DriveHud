using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DriveHUD.PlayerXRay.DataTypes;
using DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects;

namespace DriveHUD.PlayerXRay.UserControls.Notes
{
    /// <summary>
    /// Interaction logic for ComparisionFiltersWindow.xaml
    /// </summary>
    public partial class ComparisonFiltersWindow
    {
        public NoteObject Note { get; set; }

        public ComparisonFiltersWindow(NoteObject note)
        {
            Note = note;
            InitializeComponent();
        }

        private void WindowInitialized(object sender, EventArgs e)
        {
        	FillFilters();
            Title += " - " + Note.Name;
            SelectExistingFilters();
        }

        private void SelectExistingFilters()
        {
            foreach (TreeViewItem parent in tree.Items)
            {
                foreach (FilterObject obj in parent.ItemsSource)
                {
                    if (Note.Settings.SelectedFiltersComparison.Find(p => p.Tag == obj.Tag) != null)
                        obj.Selected = true;
                }
            }
        }

        private void FillFilters()
        {
            foreach (TreeViewItem item in tree.Items)
            {
                item.ItemsSource = Note.Settings.SelectedFilters.FindAll(p => p.Stage == (NoteStageType)tree.Items.IndexOf(item) && !p.Description.Contains("..."));
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Save(false);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        	Save(true);
        }

        private void Save(bool closing)
        {
            Note.Settings.SelectedFiltersComparison.Clear();

            foreach (TreeViewItem parent in tree.Items)
            {
                foreach (FilterObject filter in parent.ItemsSource)
                {
                    if (filter.Selected)
                        Note.Settings.SelectedFiltersComparison.Add(filter);
                }
            }

            Closing -= Window_Closing;
            if (!closing)
                Close();
        }
    }
}
