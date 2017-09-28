using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using AcePokerSolutions.DataTypes;
using AcePokerSolutions.DataTypes.NotesTreeObjects;

namespace AcePokerSolutions.PlayerXRay.UserControls.Notes
{
    /// <summary>
    /// Interaction logic for NotesHv.xaml
    /// </summary>
    public partial class NotesHv
    {
        private NoteStageType m_stage;

        public NotesHv()
        {
            InitializeComponent();
        }

        public void FillSettingsObject(NoteSettingsObject settings)
        {
            switch (m_stage)
            {
                case NoteStageType.Flop:
                    FillHandValueSettings(settings.FlopHvSettings);
                    break;
                case NoteStageType.Turn:
                    FillHandValueSettings(settings.TurnHvSettings);
                    break;
                case NoteStageType.River:
                    FillHandValueSettings(settings.RiverHvSettings);
                    break;
            }

            ApplyStreetRestrictions();
        }

        private void ApplyStreetRestrictions()
        {
            itmStraightOnBoard.Visibility = m_stage == NoteStageType.River
                                                ? Visibility.Visible
                                                : Visibility.Collapsed;
            itmFullHouseOnBoard.Visibility = m_stage == NoteStageType.River
                                                ? Visibility.Visible
                                                : Visibility.Collapsed;
            itmStraightFlushOnBoard.Visibility = m_stage == NoteStageType.River
                                                ? Visibility.Visible
                                                : Visibility.Collapsed;
            itm4OkOnBoard.Visibility = m_stage == NoteStageType.Flop
                                                ? Visibility.Collapsed
                                                : Visibility.Visible;
            itm2PairOnBoard.Visibility = m_stage == NoteStageType.Flop
                                                ? Visibility.Collapsed
                                                : Visibility.Visible;
            itmFlushOnBoard1.Visibility = m_stage == NoteStageType.River
                                                ? Visibility.Visible
                                                : Visibility.Collapsed;
            itmFlushOnBoard2.Visibility = m_stage == NoteStageType.River
                                                ? Visibility.Visible
                                                : Visibility.Collapsed;
            itmFlushOnBoard3.Visibility = m_stage == NoteStageType.River
                                                ? Visibility.Visible
                                                : Visibility.Collapsed;
        }

        private void FillHandValueSettings(HandValueSettings settings)
        {
            settings.AnyFlushDraws = (bool) chkFlushDrawValue.IsChecked;
            settings.AnyHv = (bool) chkHv.IsChecked;
            settings.AnyStraightDraws = (bool) chkStraightDrawValue.IsChecked;

            if (settings.AnyFlushDraws)
                settings.SelectedFlushDraws.Clear();
            else
            {
                foreach (ListBoxItem item in lstFlushValue.SelectedItems)
                {
                    settings.SelectedFlushDraws.Add(Int32.Parse(item.Tag.ToString()));
                }
            }

            if (settings.AnyHv)
                settings.SelectedHv.Clear();
            else
            {
                foreach (ListBoxItem item in lstHv.SelectedItems)
                {
                    if (item.Visibility == Visibility.Collapsed)
                        continue;

                    settings.SelectedHv.Add(Int32.Parse(item.Tag.ToString()));
                }
            }

            if (settings.AnyStraightDraws)
                settings.SelectedStraighDraws.Clear();
            else
            {
                foreach (ListBoxItem item in lstStraightValue.SelectedItems)
                {
                    settings.SelectedStraighDraws.Add(Int32.Parse(item.Tag.ToString()));
                }
            }
        }

        public void Initialize(NoteSettingsObject settings, NoteStageType stage)
        {
            m_stage = stage;
            switch (stage)
            {
                case NoteStageType.Flop:
                    FillFlopInfo(settings);
                    break;
                case NoteStageType.Turn:
                    FillTurnInfo(settings);
                    break;
                case NoteStageType.River:
                    FillRiverInfo(settings);
                    chkStraightDrawValue.Visibility = System.Windows.Visibility.Hidden;
                    chkFlushDrawValue.Visibility = System.Windows.Visibility.Hidden;
                    lstFlushValue.Visibility = System.Windows.Visibility.Hidden;
                    lstStraightValue.Visibility = System.Windows.Visibility.Hidden;
                    break;
            }
            ApplyStreetRestrictions();
        }

        private void FillFlopInfo(NoteSettingsObject settings)
        {
            FillInfo(settings.FlopHvSettings.SelectedHv, settings.FlopHvSettings.SelectedStraighDraws,
                settings.FlopHvSettings.SelectedFlushDraws, settings.FlopHvSettings.AnyHv,
                settings.FlopHvSettings.AnyStraightDraws, settings.FlopHvSettings.AnyFlushDraws);
        }

        private void FillTurnInfo(NoteSettingsObject settings)
        {
            FillInfo(settings.TurnHvSettings.SelectedHv, settings.TurnHvSettings.SelectedStraighDraws,
               settings.TurnHvSettings.SelectedFlushDraws, settings.TurnHvSettings.AnyHv,
               settings.TurnHvSettings.AnyStraightDraws, settings.TurnHvSettings.AnyFlushDraws);
        }

        private void FillRiverInfo(NoteSettingsObject settings)
        {
            FillInfo(settings.RiverHvSettings.SelectedHv, settings.RiverHvSettings.SelectedStraighDraws,
               settings.RiverHvSettings.SelectedFlushDraws, settings.RiverHvSettings.AnyHv,
               settings.RiverHvSettings.AnyStraightDraws, settings.RiverHvSettings.AnyFlushDraws);
        }

        private void FillInfo(IEnumerable<int> hv, IEnumerable<int> straightHv, IEnumerable<int> flushHv, bool anyHv, bool anyStraight, bool anyFlush)
        {
            lstHv.SelectedItems.Clear();
            lstStraightValue.SelectedItems.Clear();
            lstFlushValue.SelectedItems.Clear();

            chkHv.IsChecked = anyHv;
            chkFlushDrawValue.IsChecked = anyFlush;
            chkStraightDrawValue.IsChecked = anyStraight;

            if (!anyHv)
            {
                foreach (ListBoxItem item in lstHv.Items)
                {
                    foreach (int i in hv)
                    {
                        if (item.Tag.ToString() == i.ToString())
                        {
                            item.IsSelected = true;
                        }
                    }
                }
            }

            if (!anyStraight)
            {
                foreach (ListBoxItem item in lstStraightValue.Items)
                {
                    foreach (int i in straightHv)
                    {
                        if (item.Tag.ToString() == i.ToString())
                        {
                            item.IsSelected = true;
                        }
                    }
                }
            }

            if (anyFlush) return;

            foreach (ListBoxItem item in lstFlushValue.Items)
            {
                foreach (int i in flushHv)
                {
                    if (item.Tag.ToString() == i.ToString())
                    {
                        item.IsSelected = true;
                    }
                }
            }
        }
    }
}
