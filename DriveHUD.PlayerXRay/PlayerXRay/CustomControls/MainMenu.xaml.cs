using System.Windows;
using System.Windows.Input;
using AcePokerSolutions.PlayerXRay.Helpers;

namespace AcePokerSolutions.PlayerXRay.CustomControls
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu
    {
        public delegate void MainMenuDelegate(MainMenuType selectedMenuType);

        public event MainMenuDelegate MainMenuChanged;

        public MainMenu()
        {
            Initialized += MainMenuInitialized;
            InitializeComponent();
        }

        void MainMenuInitialized(object sender, System.EventArgs e)
        {
            img.Source = UIHelpers.GetBitmapSource(DriveHUD.PlayerXRay.Properties.Resources.tabMainBack);
        }

        public void SelectMenu(MainMenuType type)
        {
            foreach (MainMenuButton btn in stack.Children)
            {
                if (btn.MenuType != type) continue;
                btn.IsSelected = true;
                if (MainMenuChanged != null)
                    MainMenuChanged(btn.MenuType);
                break;
            }
        }

        public void ShowMenu(MainMenuType type)
        {
            foreach (MainMenuButton btn in stack.Children)
            {
                if (btn.MenuType != type) continue;
                btn.Visibility = Visibility.Visible;
                return;
            }
        }

        public void HideMenu(MainMenuType type)
        {
            foreach (MainMenuButton btn in stack.Children)
            {
                if (btn.MenuType != type) continue;
                btn.Visibility = Visibility.Collapsed;
                
                if (btn.IsSelected)
                {
                    btn.IsSelected = false;
                    foreach (MainMenuButton btn2 in stack.Children)
                    {
                        btn2.IsSelected = true;
                        if (MainMenuChanged != null)
                            MainMenuChanged(btn2.MenuType);
                        break;
                    }
                }
                return;
            }
        }

        public bool ContainsMenu(MainMenuType type)
        {
            foreach (UIElement elem in stack.Children)
            {
                if (!(elem is MainMenuButton))
                    continue;

                MainMenuButton btn = (MainMenuButton) elem;

                if (btn.MenuType == type)
                    return true;
            }          
            return false;
        }

        public void AddMenuItem(MainMenuType type)
        {
            MainMenuButton btn = new MainMenuButton();
            btn.MenuSelected += BtnMenuSelected;
            stack.Children.Add(btn);
            switch (type)
            {
                case MainMenuType.Run:
                    btn.Header = "Run";
                    btn.MenuType = MainMenuType.Run;
                    btn.img.Source = UIHelpers.GetBitmapSource(DriveHUD.PlayerXRay.Properties.Resources.runImage);
                    break;
                case MainMenuType.Help:
                    btn.Header = "Help";
                    btn.MenuType = MainMenuType.Help;
                    btn.img.Source = UIHelpers.GetBitmapSource(DriveHUD.PlayerXRay.Properties.Resources.helpImage);
                    break;
                case MainMenuType.Settings:
                    btn.MenuType = MainMenuType.Settings;
                    btn.img.Source = UIHelpers.GetBitmapSource(DriveHUD.PlayerXRay.Properties.Resources.settingsImage);
                    btn.Header = "Settings";
                    break;
                case MainMenuType.HUD:
                    btn.MenuType = MainMenuType.HUD;
                    btn.Header = "HUD";
                    btn.img.Source = UIHelpers.GetBitmapSource(DriveHUD.PlayerXRay.Properties.Resources.noteImage);
                    break;
                case MainMenuType.Notes:
                    btn.MenuType = MainMenuType.Notes;
                    btn.Header = "Notes";
                    btn.img.Source = UIHelpers.GetBitmapSource(DriveHUD.PlayerXRay.Properties.Resources.noteImage);
                    break;
                case MainMenuType.Profiles:
                    btn.MenuType = MainMenuType.Profiles;
                    btn.Header = "Profiles";
                    btn.img.Source = UIHelpers.GetBitmapSource(DriveHUD.PlayerXRay.Properties.Resources.profiles);
                    break;
                case MainMenuType.Debug:
                    btn.MenuType = MainMenuType.Debug;
                    btn.Header = "Debug";
                    btn.img.Source = UIHelpers.GetBitmapSource(DriveHUD.PlayerXRay.Properties.Resources.database);
                    break;
            }
        }

        void BtnMenuSelected(object sender, MouseButtonEventArgs e)
        {
            MainMenuButton btn = (MainMenuButton) sender;

            if (btn.MenuType != MainMenuType.HUD)
            {
                foreach (MainMenuButton button in stack.Children)
                {
                    button.IsSelected = false;
                }
                btn.IsSelected = true;
            }
            else
                btn.IsSelected = false;

            MainMenuChanged?.Invoke(btn.MenuType);
        }
    }

    public enum MainMenuType
    {
        Run, 
        Notes,
        Profiles,
        Settings,
        HUD,
        Help,
        Debug
    }
}
