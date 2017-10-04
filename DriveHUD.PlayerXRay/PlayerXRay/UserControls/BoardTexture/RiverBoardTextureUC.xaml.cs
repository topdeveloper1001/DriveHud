using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DriveHUD.PlayerXRay.DataTypes;
using DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects;
using DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects.TextureObjects;

namespace DriveHUD.PlayerXRay.UserControls.BoardTexture
{
    /// <summary>
    /// Interaction logic for RiverBoardTextureUC.xaml
    /// </summary>
    public partial class RiverBoardTextureUC
    {
        public RiverBoardTextureUC()
        {
            InitializeComponent();
        }

        public void FillSettings(NoteSettingsObject settings)
        {
            RiverTextureSettings texture = settings.RiverTextureSettings;
            if ((bool)chkFlushCards.IsChecked)
            {
                texture.IsFlushCardFilter = true;
                if ((bool)rdoNoFlush.IsChecked)
                    texture.FlushCard = RiverFlushCardsEnum.NoFlush;
                else
                    if ((bool)rdoThreeCardsOneSuit.IsChecked)
                        texture.FlushCard = RiverFlushCardsEnum.ThreeCardsOneSuit;
                    else
                        if ((bool)rdoFourCardsOneSuit.IsChecked)
                            texture.FlushCard = RiverFlushCardsEnum.FourCardsOneSuit;
                        else
                            texture.FlushCard = RiverFlushCardsEnum.FiveCardsOneSuit;
            }
            else
                texture.IsFlushCardFilter = false;

            if ((bool)chkPossibleStraights.IsChecked)
            {
                texture.IsPossibleStraightsFilter = true;
                texture.PossibleStraights =
                    Int32.Parse(((ComboBoxItem)cmbPossibleStraights.SelectedItem).Content.ToString());
                texture.PossibleStraightsCompare =
                    (CompareEnum)cmbPossibleCompare.SelectedIndex;
            }
            else
                texture.IsPossibleStraightsFilter = false;

            if ((bool)chkHighestCard.IsChecked)
            {
                texture.IsHighcardFilter = true;
                texture.HighestCard =
                    ((ComboBoxItem)cmbHighestCard.SelectedItem).Content.ToString();
            }
            else
                texture.IsHighcardFilter = false;

            if ((bool)chkFlopTexture.IsChecked)
            {
                texture.IsCardTextureFilter = true;
                List<string> selectedCards = new List<string>();
                foreach (UIElement elem in stackCards.Children)
                {
                    if (!(elem is CheckBox))
                        continue;
                    if ((bool)((CheckBox)elem).IsChecked)
                        selectedCards.Add(((CheckBox)elem).Content.ToString());
                }
                texture.SelectedCardTextureList = selectedCards;
            }
            else
            {
                texture.SelectedCardTextureList = new List<string>();
                texture.IsCardTextureFilter = false;
            }

            if ((bool)chkFlopIsPaired.IsChecked)
            {
                texture.IsPairedFilter = true;
                texture.IsPairedFilterTrue = (bool)rdoPairedTrue.IsChecked;
            }
            else
                texture.IsPairedFilter = false;
        }

        public void Initialize(NoteSettingsObject settings)
        {
            if (settings.RiverTextureSettings.IsFlushCardFilter)
            {
                chkFlushCards.IsChecked = true;
                switch (settings.RiverTextureSettings.FlushCard)
                {
                    case RiverFlushCardsEnum.NoFlush:
                        rdoNoFlush.IsChecked = true;
                        break;
                    case RiverFlushCardsEnum.FiveCardsOneSuit:
                        rdoFiveCardsOneSuit.IsChecked = true;
                        break;
                    case RiverFlushCardsEnum.FourCardsOneSuit:
                        rdoFourCardsOneSuit.IsChecked = true;
                        break;
                    case RiverFlushCardsEnum.ThreeCardsOneSuit:
                        rdoThreeCardsOneSuit.IsChecked = true;
                        break;
                }
            }
            else
            {
                rdoNoFlush.IsChecked = true;
                chkFlushCards.IsChecked = false;
            }

            if (settings.RiverTextureSettings.IsPossibleStraightsFilter)
            {
                chkPossibleStraights.IsChecked = true;
                cmbPossibleStraights.SelectedIndex = settings.RiverTextureSettings.PossibleStraights;
                cmbPossibleCompare.SelectedIndex = (int)settings.RiverTextureSettings.PossibleStraightsCompare;
            }
            else
            {
                cmbPossibleStraights.SelectedIndex = 0;
                cmbPossibleCompare.SelectedIndex = 0;
                chkPossibleStraights.IsChecked = false;
            }

            if (settings.RiverTextureSettings.IsHighcardFilter)
            {
                chkHighestCard.IsChecked = true;
                foreach (ComboBoxItem item in cmbHighestCard.Items)
                {
                    if (item.Content.ToString() != settings.RiverTextureSettings.HighestCard) continue;
                    cmbHighestCard.SelectedIndex = cmbHighestCard.Items.IndexOf(item);
                    break;
                }
            }
            else
            {
                cmbHighestCard.SelectedIndex = 0;
                chkHighestCard.IsChecked = false;
            }

            if (settings.RiverTextureSettings.IsCardTextureFilter)
            {
                foreach (UIElement element in stackCards.Children)
                {
                    if (!(element is CheckBox))
                        continue;

                    ((CheckBox)element).IsChecked =
                        settings.RiverTextureSettings.SelectedCardTextureList.Contains(((CheckBox)element).Content.ToString());
                }

                chkFlopTexture.IsChecked = true;
            }
            else
            {
                foreach (UIElement element in stackCards.Children)
                {
                    if (!(element is CheckBox))
                        continue;

                    ((CheckBox)element).IsChecked = false;
                }
                chkFlopTexture.IsChecked = false;
            }

            if (settings.RiverTextureSettings.IsPairedFilter)
            {
                chkFlopIsPaired.IsChecked = true;
                if (settings.FlopTextureSettings.IsPairedFilterTrue)
                    rdoPairedTrue.IsChecked = true;
                else
                    rdoPairedFalse.IsChecked = true;
            }
            else
            {
                rdoPairedTrue.IsChecked = true;
                chkFlopIsPaired.IsChecked = false;
            }
        }

        private void RdoAnyTextureChecked(object sender, RoutedEventArgs e)
        {
            if (!IsInitialized)
                return;

            chkFlopIsPaired.IsChecked = false;
            chkFlopTexture.IsChecked = false;
            chkFlushCards.IsChecked = false;
            chkHighestCard.IsChecked = false;
            chkPossibleStraights.IsChecked = false;
        }

        private void RdoFilterByChecked(object sender, RoutedEventArgs e)
        {
            rdoAnyTexture.IsChecked = true;
        }

        private void ChkFilterChecked(object sender, RoutedEventArgs e)
        {
            rdoFilterBy.Checked -= RdoFilterByChecked;
            rdoFilterBy.IsChecked = true;
            rdoFilterBy.Checked += RdoFilterByChecked;
        }

        private void ChkFilterUnchecked(object sender, RoutedEventArgs e)
        {
            if (!(bool)chkFlopIsPaired.IsChecked &&
                !(bool)chkFlopTexture.IsChecked &&
                !(bool)chkFlushCards.IsChecked &&
                !(bool)chkHighestCard.IsChecked &&
                !(bool)chkPossibleStraights.IsChecked)
                rdoAnyTexture.IsChecked = true;
        }
    }
}
