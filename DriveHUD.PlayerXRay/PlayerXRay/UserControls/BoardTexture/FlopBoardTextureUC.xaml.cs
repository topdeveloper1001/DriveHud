using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DriveHUD.PlayerXRay.DataTypes;
using DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects;
using DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects.TextureObjects;

namespace DriveHUD.PlayerXRay.UserControls.BoardTexture
{
    public partial class FlopBoardTextureUC
    {
        public FlopBoardTextureUC()
        {
            InitializeComponent();
        }

        public void FillSettings(NoteSettingsObject settings)
        {
            FlopTextureSettings texture = settings.FlopTextureSettings;
            if ((bool)chkFlushCards.IsChecked)
                texture.IsFlushCardFilter = true;
            else
                texture.IsFlushCardFilter = false;

            if ((bool)rdoRainbow.IsChecked)
                texture.FlushCard = FlopFlushCardsEnum.Rainbow;
            else
                if ((bool)rdoThreeOfOneSuit.IsChecked)
                    texture.FlushCard = FlopFlushCardsEnum.ThreeOfOneSuit;
                else
                    texture.FlushCard = FlopFlushCardsEnum.TwoOfOneSuit;

            if ((bool)chkOpenEndedStraight.IsChecked)
            {
                texture.IsOpenEndedStraightDrawsFilter = true;
            }
            else
                texture.IsOpenEndedStraightDrawsFilter = false;

            texture.OpenEndedStraightDraws =
                    Int32.Parse(((ComboBoxItem)cmbOpenEndedStraighDraws.SelectedItem).Content.ToString());

            if ((bool)chkGutshots.IsChecked)
                texture.IsGutshotsFilter = true;
            else
                texture.IsGutshotsFilter = false;

            texture.Gutshots =
                    Int32.Parse(((ComboBoxItem)cmbGutshots.SelectedItem).Content.ToString());

            if ((bool)chkPossibleStraights.IsChecked)
                texture.IsPossibleStraightsFilter = true;
            else
                texture.IsPossibleStraightsFilter = false;

            texture.PossibleStraights =
                    Int32.Parse(((ComboBoxItem)cmbPossibleStraights.SelectedItem).Content.ToString());
            texture.PossibleStraightsCompare =
                (CompareEnum)cmbPossibleCompare.SelectedIndex;

            if ((bool)chkHighestCard.IsChecked)
                texture.IsHighcardFilter = true;
            else
                texture.IsHighcardFilter = false;

            texture.HighestCard =
                    ((ComboBoxItem)cmbHighestCard.SelectedItem).Content.ToString();

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
                texture.SelectedCardTextureList  = new List<string>();
                texture.IsCardTextureFilter = false;
            }

            if ((bool)chkFlopIsPaired.IsChecked)
                texture.IsPairedFilter = true;
            else
                texture.IsPairedFilter = false;

            texture.IsPairedFilterTrue = (bool)rdoPairedTrue.IsChecked;
        }

        public void Initialize(NoteSettingsObject settings)
        {
            if (settings.FlopTextureSettings.IsFlushCardFilter)
            {
                chkFlushCards.IsChecked = true;
                switch (settings.FlopTextureSettings.FlushCard)
                {
                    case FlopFlushCardsEnum.Rainbow:
                        rdoRainbow.IsChecked = true;
                        break;
                    case FlopFlushCardsEnum.ThreeOfOneSuit:
                        rdoThreeOfOneSuit.IsChecked = true;
                        break;
                    case FlopFlushCardsEnum.TwoOfOneSuit:
                        rdoTwoOfOneSuit.IsChecked = true;
                        break;
                }
            }
            else
            {
                rdoRainbow.IsChecked = true;
                chkFlushCards.IsChecked = false;
            }

            if (settings.FlopTextureSettings.IsOpenEndedStraightDrawsFilter)
            {
                chkOpenEndedStraight.IsChecked = true;
                cmbOpenEndedStraighDraws.SelectedIndex = settings.FlopTextureSettings.OpenEndedStraightDraws;
            }
            else
            {
                cmbOpenEndedStraighDraws.SelectedIndex = 0;
                chkOpenEndedStraight.IsChecked = false;
            }

            if (settings.FlopTextureSettings.IsGutshotsFilter)
            {
                chkGutshots.IsChecked = true;
                cmbGutshots.SelectedIndex = settings.FlopTextureSettings.Gutshots;
            }
            else
            {
                cmbGutshots.SelectedIndex = 0;
                chkGutshots.IsChecked = false;
            }

            if (settings.FlopTextureSettings.IsPossibleStraightsFilter)
            {
                chkPossibleStraights.IsChecked = true;
                cmbPossibleStraights.SelectedIndex = settings.FlopTextureSettings.PossibleStraights;
                cmbPossibleCompare.SelectedIndex = (int)settings.FlopTextureSettings.PossibleStraightsCompare;
            }
            else
            {
                cmbPossibleStraights.SelectedIndex = 0;
                cmbPossibleCompare.SelectedIndex = 0;
                chkPossibleStraights.IsChecked = false;
            }

            if (settings.FlopTextureSettings.IsHighcardFilter)
            {
                chkHighestCard.IsChecked = true;
                foreach (ComboBoxItem item in cmbHighestCard.Items)
                {
                    if (item.Content.ToString() != settings.FlopTextureSettings.HighestCard) continue;
                    cmbHighestCard.SelectedIndex = cmbHighestCard.Items.IndexOf(item);
                    break;
                }
            }
            else
            {
                cmbHighestCard.SelectedIndex = 0;
                chkHighestCard.IsChecked = false;
            }

            if (settings.FlopTextureSettings.IsCardTextureFilter)
            {
                foreach (UIElement element in stackCards.Children)
                {
                    if (!(element is CheckBox))
                        continue;

                    ((CheckBox) element).IsChecked =
                        settings.FlopTextureSettings.SelectedCardTextureList.Contains(((CheckBox) element).Content.ToString());
                }

                chkFlopTexture.IsChecked = true;
            }
            else
            {
                foreach (UIElement element in stackCards.Children)
                {
                    if (!(element is CheckBox))
                        continue;

                    ((CheckBox) element).IsChecked = false;
                }
                chkFlopTexture.IsChecked = false;
            }

            if (settings.FlopTextureSettings.IsPairedFilter)
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
            chkGutshots.IsChecked = false;
            chkHighestCard.IsChecked = false;
            chkOpenEndedStraight.IsChecked = false;
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
                !(bool)chkGutshots.IsChecked &&
                !(bool)chkHighestCard.IsChecked &&
                !(bool)chkOpenEndedStraight.IsChecked &&
                !(bool)chkPossibleStraights.IsChecked)
                rdoAnyTexture.IsChecked = true;
        }
    }
}
