using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace DriveHUD.PlayerXRay.CustomControls
{
	/// <summary>
	/// Interaction logic for MainMenuButton.xaml
	/// </summary>
	public partial class MainMenuButton
	{
        private bool m_mIsSelected;
        public event MouseButtonEventHandler MenuSelected;
        private readonly SolidColorBrush m_fillBrush = new SolidColorBrush(Color.FromArgb(20, 255, 255, 255));

		public MainMenuButton()
		{
			InitializeComponent();
		}

        public bool IsSelected
        {
            get
            {
                return m_mIsSelected;
            }
            set
            {
                m_mIsSelected = value;
                if (value)
                    Select();
                else
                    Unselect();
            }
        }

        private void Select()
        {
            lblHeader.BitmapEffect = (BitmapEffect)FindResource("SelectedMenuBE");
            Background = m_fillBrush;
        }

        private void Unselect()
        {
            lblHeader.BitmapEffect = null;
            Background = null;
        }

        public string Header
        {
            get
            {
                return lblHeader.Text;
            }
            set
            {
                lblHeader.Text = value;
            }
        }
       
		private void UserControlMouseEnter(object sender, MouseEventArgs e)
		{
            if (m_mIsSelected)
                return;
            Select();
		}

		private void UserControlMouseLeave(object sender, MouseEventArgs e)
		{
            if (m_mIsSelected)
                return;
            Unselect();
		}

		private void UserControlMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
            if (MenuSelected != null)
                MenuSelected(this, null);
		}

        public MainMenuType MenuType { get; set; }
	}
}