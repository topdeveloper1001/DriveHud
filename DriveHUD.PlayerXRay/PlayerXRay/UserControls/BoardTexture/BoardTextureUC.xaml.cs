using System.Windows.Controls;
using AcePokerSolutions.DataTypes.NotesTreeObjects;

namespace AcePokerSolutions.PlayerXRay.UserControls.BoardTexture
{
    /// <summary>
    /// Interaction logic for BoardTextureUC.xaml
    /// </summary>
    public partial class BoardTextureUC
    {
        private NoteSettingsObject m_settings;
        private FlopBoardTextureUC m_flop;
        private TurnBoardTextureUC m_turn;
        private RiverBoardTextureUC m_river;

        public BoardTextureUC()
        {
            InitializeComponent();
        }

        public void FillSettings(NoteSettingsObject settings)
        {
            m_flop.FillSettings(settings);
            if (m_turn == null)
                settings.TurnTextureSettings = m_settings.TurnTextureSettings;
            else
                m_turn.FillSettings(settings);

            if (m_river == null)
                settings.RiverTextureSettings = m_settings.RiverTextureSettings;
            else
                m_river.FillSettings(settings);
        }

        public void Initialize(NoteSettingsObject settings)
        {
            m_settings = settings;

            if (m_flop == null)
            {
                m_flop = new FlopBoardTextureUC();
                panel.Content = m_flop;
            }

            m_flop.Initialize(m_settings);

            if (m_turn != null)
                m_turn.Initialize(m_settings);

            if (m_river != null)
                m_river.Initialize(m_settings);
        }

        private void BtnFlopClick(object sender, System.Windows.RoutedEventArgs e)
        {
        	Grid.SetColumn(rectHVSelectedMenu, 0);

            if (m_flop == null)
            {
                m_flop = new FlopBoardTextureUC();
                m_flop.Initialize(m_settings);
            }
            panel.Content = m_flop;
        }

        private void BtnTurnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Grid.SetColumn(rectHVSelectedMenu, 1);
            if (m_turn == null)
            {
                m_turn = new TurnBoardTextureUC();
                m_turn.Initialize(m_settings);
            }
            panel.Content = m_turn;
        }

        private void BtnRiverClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Grid.SetColumn(rectHVSelectedMenu, 2);
            if (m_river == null)
            {
                m_river = new RiverBoardTextureUC();
                m_river.Initialize(m_settings);
            }
            panel.Content = m_river;
        }
    }
}
