using System.Windows;
using System.Windows.Controls;
using AcePokerSolutions.DataTypes;
using AcePokerSolutions.DataTypes.NotesTreeObjects.ActionsObjects;

namespace AcePokerSolutions.PlayerXRay.UserControls.Notes
{
    /// <summary>
    /// Interaction logic for NotesActionsUC.xaml
    /// </summary>
    public partial class NotesActionsUC
    {
        public NotesActionsUC()
        {
            InitializeComponent();
        }

        public void FillInfo(ActionSettings settings)
        {
            cmbAction1.SelectedIndex = (int) settings.FirstType;
            txtFirstMin.Value = settings.FirstMinValue;
            txtFirstMax.Value = settings.FirstMaxValue;

            cmbAction2.SelectedIndex = (int)settings.SecondType;
            txtSecondMin.Value = settings.SecondMinValue;
            txtSecondMax.Value = settings.SecondMaxValue;

            cmbAction3.SelectedIndex = (int)settings.ThirdType;
            txtThirdMin.Value = settings.ThirdMinValue;
            txtThirdMax.Value = settings.ThirdMaxValue;

            cmbAction4.SelectedIndex = (int)settings.FourthType;
            txtFourthMin.Value = settings.FourthMinValue;
            txtFourthMax.Value = settings.FourthMaxValue;
        }

        public ActionSettings GetInfo()
        {
            ActionSettings settings = new ActionSettings
                                          {
                                              FirstType = (ActionTypeEnum) cmbAction1.SelectedIndex,
                                              FirstMinValue = (double)txtFirstMin.Value,
                                              FirstMaxValue = (double)txtFirstMax.Value,
                                              SecondType = (ActionTypeEnum) cmbAction2.SelectedIndex,
                                              SecondMinValue = (double)txtSecondMin.Value,
                                              SecondMaxValue = (double)txtSecondMax.Value,
                                              ThirdType = (ActionTypeEnum) cmbAction3.SelectedIndex,
                                              ThirdMinValue = (double)txtThirdMin.Value,
                                              ThirdMaxValue = (double)txtThirdMax.Value,
                                              FourthType = (ActionTypeEnum) cmbAction4.SelectedIndex,
                                              FourthMinValue = (double)txtFourthMin.Value,
                                              FourthMaxValue = (double)txtFourthMax.Value
                                          };

            return settings;
        }

        private void CmbAction1SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsInitialized)
                return;
            ActionTypeEnum action = (ActionTypeEnum)cmbAction1.SelectedIndex;

            stackFirstAction.Visibility = Visibility.Visible;
            switch (action)
            {
                case ActionTypeEnum.Check:
                case ActionTypeEnum.Any:
                    stackFirstAction.Visibility = Visibility.Hidden;
                    return;
                default:
                    lblAction1.Text = action.ToString();
                    break;
            }
        }

        private void CmbAction2SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsInitialized)
                return;

            ActionTypeEnum action = (ActionTypeEnum)cmbAction2.SelectedIndex;

            stackSecondAction.Visibility = Visibility.Visible;
            switch (action)
            {
                case ActionTypeEnum.Check:
                case ActionTypeEnum.Any:
                    stackSecondAction.Visibility = Visibility.Hidden;
                    return;
                default:
                    lblAction2.Text = action.ToString();
                    break;
            }
        }

        private void CmbAction3SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsInitialized)
                return;

            ActionTypeEnum action = (ActionTypeEnum)cmbAction3.SelectedIndex;

            stackThirdAction.Visibility = Visibility.Visible;
            switch (action)
            {
                case ActionTypeEnum.Check:
                case ActionTypeEnum.Any:
                    stackThirdAction.Visibility = Visibility.Hidden;
                    return;
                default:
                    lblAction3.Text = action.ToString();
                    break;
            }
        }

        private void CmbAction4SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsInitialized)
                return;

            ActionTypeEnum action = (ActionTypeEnum)cmbAction4.SelectedIndex;

            stackFourthAction.Visibility = Visibility.Visible;
            switch (action)
            {
                case ActionTypeEnum.Check:
                case ActionTypeEnum.Any:
                    stackFourthAction.Visibility = Visibility.Hidden;
                    return;
                default:
                    lblAction4.Text = action.ToString();
                    break;
            }
        }
    }
}
