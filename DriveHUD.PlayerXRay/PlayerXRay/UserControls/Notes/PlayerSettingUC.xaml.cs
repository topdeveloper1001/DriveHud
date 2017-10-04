using DriveHUD.PlayerXRay.DataTypes;
using DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects;
using DriveHUD.PlayerXRay.Helpers;

namespace DriveHUD.PlayerXRay.UserControls.Notes
{
    /// <summary>
    /// Interaction logic for PlayerSettingUC.xaml
    /// </summary>
    public partial class PlayerSettingUC
    {
        private PlayerObject m_playerObject;

        public PlayerSettingUC()
        {
            InitializeComponent();
        }

        public PlayerObject GetPlayerObject()
        {
            PlayerObject obj = new PlayerObject
                                   {
                                       Include = (bool) chkInclude.IsChecked,
                                       PlayerType = m_playerObject.PlayerType,
                                       VpIpMin = (double)txtVpIpMin.Value,
                                       VpIpMax = (double)txtVpIpMax.Value,
                                       AggMin = (double)txtAggMin.Value,
                                       AggMax = (double)txtAggMax.Value,
                                       WtsdMin = (double)txtWtsdMin.Value,
                                       WtsdMax = (double)txtWtsdMax.Value,
                                       WwsfMin = (double)txtWwsfMin.Value,
                                       WwsfMax = (double)txtWwsfMax.Value,
                                       PfrMin = (double)txtPfrMin.Value,
                                       PfrMax = (double)txtPfrMax.Value,
                                       ThreeBetMin = (double)txtThreeBetMin.Value,
                                       ThreeBetMax = (double)txtThreeBetMax.Value,
                                       WsdMin = (double)txtWsdMin.Value,
                                       WsdMax = (double)txtWsdMax.Value
                                   };

            return obj;
        }

        public void Initialize(PlayerObject player)
        {
            m_playerObject = player;

            lblDescription.Text = m_playerObject.PlayerType.ToString().ToUpper();

            chkInclude.IsChecked = m_playerObject.Include;
            txtVpIpMin.Value = m_playerObject.VpIpMin;
            txtVpIpMax.Value = m_playerObject.VpIpMax;
            txtAggMin.Value = m_playerObject.AggMin;
            txtAggMax.Value = m_playerObject.AggMax;
            txtWtsdMin.Value = m_playerObject.WtsdMin;
            txtWtsdMax.Value = m_playerObject.WtsdMax;
            txtWwsfMin.Value = m_playerObject.WwsfMin;
            txtWwsfMax.Value = m_playerObject.WwsfMax;
            txtPfrMin.Value = m_playerObject.PfrMin;
            txtPfrMax.Value = m_playerObject.PfrMax;
            txtThreeBetMin.Value = m_playerObject.ThreeBetMin;
            txtThreeBetMax.Value = m_playerObject.ThreeBetMax;
            txtWsdMin.Value = m_playerObject.WsdMin;
            txtWsdMax.Value = m_playerObject.WsdMax;


            switch (m_playerObject.PlayerType)
            {
                case PlayerTypeEnum.Fish:
                    imgPlayer.Source = UIHelpers.GetBitmapSource(DriveHUD.PlayerXRay.Properties.Resources.PLR_FISH);
                    break;
                case PlayerTypeEnum.Gambler:
                    imgPlayer.Source = UIHelpers.GetBitmapSource(DriveHUD.PlayerXRay.Properties.Resources.PLR_GAMBLER);
                    break;
                case PlayerTypeEnum.Lag:
                    imgPlayer.Source = UIHelpers.GetBitmapSource(DriveHUD.PlayerXRay.Properties.Resources.PLR_LAG1);
                    break;
                case PlayerTypeEnum.Nit:
                    imgPlayer.Source = UIHelpers.GetBitmapSource(DriveHUD.PlayerXRay.Properties.Resources.PLR_TIGHT_TAG);
                    break;
                case PlayerTypeEnum.Rock:
                    imgPlayer.Source = UIHelpers.GetBitmapSource(DriveHUD.PlayerXRay.Properties.Resources.PLR_NIT);
                    break;
                case PlayerTypeEnum.Tag:
                    imgPlayer.Source = UIHelpers.GetBitmapSource(DriveHUD.PlayerXRay.Properties.Resources.PLR_TAG);
                    break;
                case PlayerTypeEnum.Whale:
                    imgPlayer.Source = UIHelpers.GetBitmapSource(DriveHUD.PlayerXRay.Properties.Resources.PLR_WHALE);
                    break;
            }
        }
    }
}
