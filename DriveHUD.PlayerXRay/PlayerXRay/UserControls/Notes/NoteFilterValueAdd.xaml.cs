namespace AcePokerSolutions.PlayerXRay.UserControls.Notes
{
    /// <summary>
    /// Interaction logic for NoteFilterValueAdd.xaml
    /// </summary>
    public partial class NoteFilterValueAdd
    {
        public double? SelectedValue { get; private set; }

        public NoteFilterValueAdd(string description, double? value)
        {
            InitializeComponent();
            lblDescription.Text = description;
            txtValue.Value = value.HasValue ? value.Value : 0;
            SelectedValue = null;
        }

        private void BtnCancelClick(object sender, System.Windows.RoutedEventArgs e)
        {
        	Close();
        }

        private void BtnSaveClick(object sender, System.Windows.RoutedEventArgs e)
        {
            SelectedValue = (double)txtValue.Value;
            Close();
        }
    }
}
