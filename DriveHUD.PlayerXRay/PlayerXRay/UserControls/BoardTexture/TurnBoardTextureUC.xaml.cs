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
    /// Interaction logic for TurnBoardTextureUC.xaml
    /// </summary>
    public partial class TurnBoardTextureUC
    {
        public TurnBoardTextureUC()
        {
            InitializeComponent();
        }

        public void FillSettings(NoteSettingsObject settings)
        {
            TurnTextureSettings texture = settings.TurnTextureSettings;
            if ((bool)chkFlushCards.IsChecked)
            {
                texture.IsFlushCardFilter = true;
                if ((bool)rdoRainbow.IsChecked)
                    texture.FlushCard = TurnFlushCardsEnum.Rainbow;
                else
                    if ((bool)rdoTwoOfOneSuit.IsChecked)
                        texture.FlushCard = TurnFlushCardsEnum.TwoOfOneSuit;
                    else
                        if ((bool)rdoTwoOfTwoSuits.IsChecked)
                            texture.FlushCard = TurnFlushCardsEnum.TwoOfTwoSuits;
                        else
                            if ((bool)rdoThreeOfOneSuit.IsChecked)
                                texture.FlushCard = TurnFlushCardsEnum.ThreeOfOneSuit;
                            else
                                texture.FlushCard = TurnFlushCardsEnum.FourOfOneSuit;
            }
            else
                texture.IsFlushCardFilter = false;

            if ((bool)chkOpenEndedStraight.IsChecked)
            {
                texture.IsOpenEndedStraightDrawsFilter = true;
                texture.OpenEndedStraightDraws =
                    Int32.Parse(((ComboBoxItem)cmbOpenEndedStraighDraws.SelectedItem).Content.ToString());
            }
            else
                texture.IsOpenEndedStraightDrawsFilter = false;

            if ((bool)chkGutshots.IsChecked)
            {
                texture.IsGutshotsFilter = true;
                texture.Gutshots =
                    Int32.Parse(((ComboBoxItem)cmbGutshots.SelectedItem).Content.ToString());
            }
            else
                texture.IsGutshotsFilter = false;

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
             //   texture.SelectedCardTextureList = selectedCards;
            }
            else
            {
             //   texture.SelectedCardTextureList = new List<string>();
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
            if (settings.TurnTextureSettings.IsFlushCardFilter)
            {
                chkFlushCards.IsChecked = true;
                switch (settings.TurnTextureSettings.FlushCard)
                {
                    case TurnFlushCardsEnum.Rainbow:
                        rdoRainbow.IsChecked = true;
                        break;
                    case TurnFlushCardsEnum.FourOfOneSuit:
                        rdoFourOfOneSuit.IsChecked = true;
                        break;
                    case TurnFlushCardsEnum.TwoOfTwoSuits:
                        rdoTwoOfTwoSuits.IsChecked = true;
                        break;
                    case TurnFlushCardsEnum.ThreeOfOneSuit:
                        rdoThreeOfOneSuit.IsChecked = true;
                        break;
                    case TurnFlushCardsEnum.TwoOfOneSuit:
                        rdoTwoOfOneSuit.IsChecked = true;
                        break;
                }
            }
            else
            {
                rdoRainbow.IsChecked = true;
                chkFlushCards.IsChecked = false;
            }

            if (settings.TurnTextureSettings.IsOpenEndedStraightDrawsFilter)
            {
                chkOpenEndedStraight.IsChecked = true;
                cmbOpenEndedStraighDraws.SelectedIndex = settings.TurnTextureSettings.OpenEndedStraightDraws;
            }
            else
            {
                cmbOpenEndedStraighDraws.SelectedIndex = 0;
                chkOpenEndedStraight.IsChecked = false;
            }

            if (settings.TurnTextureSettings.IsGutshotsFilter)
            {
                chkGutshots.IsChecked = true;
                cmbGutshots.SelectedIndex = settings.TurnTextureSettings.Gutshots;
            }
            else
            {
                cmbGutshots.SelectedIndex = 0;
                chkGutshots.IsChecked = false;
            }

            if (settings.TurnTextureSettings.IsPossibleStraightsFilter)
            {
                chkPossibleStraights.IsChecked = true;
                cmbPossibleStraights.SelectedIndex = settings.TurnTextureSettings.PossibleStraights;
                cmbPossibleCompare.SelectedIndex = (int)settings.TurnTextureSettings.PossibleStraightsCompare;
            }
            else
            {
                cmbPossibleStraights.SelectedIndex = 0;
                cmbPossibleCompare.SelectedIndex = 0;
                chkPossibleStraights.IsChecked = false;
            }

            if (settings.TurnTextureSettings.IsHighcardFilter)
            {
                chkHighestCard.IsChecked = true;
                foreach (ComboBoxItem item in cmbHighestCard.Items)
                {
                    if (item.Content.ToString() != settings.TurnTextureSettings.HighestCard) continue;
                    cmbHighestCard.SelectedIndex = cmbHighestCard.Items.IndexOf(item);
                    break;
                }
            }
            else
            {
                cmbHighestCard.SelectedIndex = 0;
                chkHighestCard.IsChecked = false;
            }

            if (settings.TurnTextureSettings.IsCardTextureFilter)
            {
                foreach (UIElement element in stackCards.Children)
                {
                    if (!(element is CheckBox))
                        continue;

                    ((CheckBox)element).IsChecked =
                        settings.TurnTextureSettings.SelectedCardTextureList.Contains(((CheckBox)element).Content.ToString());
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

            if (settings.TurnTextureSettings.IsPairedFilter)
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

        private void chkFilterUnchecked(object sender, RoutedEventArgs e)
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
