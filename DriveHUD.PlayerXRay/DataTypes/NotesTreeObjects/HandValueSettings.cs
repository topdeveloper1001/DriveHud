//-----------------------------------------------------------------------
// <copyright file="HandValueSettings.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Linq;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects
{
    public class HandValueSettings : ReactiveObject
    {
        public HandValueSettings()
        {
            AnyHv = true;
            AnyStraightDraws = true;
            AnyFlushDraws = true;

            SelectedFlushDraws = new ObservableCollection<int>();
            SelectedHv = new ObservableCollection<int>();
            SelectedStraighDraws = new ObservableCollection<int>();
        }

        private bool anyHv;

        public bool AnyHv
        {
            get
            {
                return anyHv;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref anyHv, value);
            }
        }

        private bool anyStraightDraws;

        public bool AnyStraightDraws
        {
            get
            {
                return anyStraightDraws;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref anyStraightDraws, value);
            }
        }

        private bool anyFlushDraws;

        public bool AnyFlushDraws
        {
            get
            {
                return anyFlushDraws;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref anyFlushDraws, value);
            }
        }

        public ObservableCollection<int> SelectedHv { get; set; }

        public ObservableCollection<int> SelectedStraighDraws { get; set; }

        public ObservableCollection<int> SelectedFlushDraws { get; set; }

        public override bool Equals(object obj)
        {
            var handValueSettings = obj as HandValueSettings;

            return Equals(handValueSettings);
        }

        public bool Equals(HandValueSettings handValueSettings)
        {
            return handValueSettings != null && handValueSettings.AnyHv == AnyHv &&
                handValueSettings.AnyFlushDraws == AnyFlushDraws &&
                handValueSettings.AnyStraightDraws == AnyStraightDraws &&
                CompareHelpers.CompareIntegerLists(handValueSettings.SelectedFlushDraws, SelectedFlushDraws) &&
                CompareHelpers.CompareIntegerLists(handValueSettings.SelectedHv, SelectedHv) &&
                CompareHelpers.CompareIntegerLists(handValueSettings.SelectedStraighDraws, SelectedStraighDraws);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 19;

                hash = hash * 31 + anyHv.GetHashCode();
                hash = hash * 31 + anyStraightDraws.GetHashCode();
                hash = hash * 31 + anyFlushDraws.GetHashCode();

                if (SelectedHv != null)
                {
                    SelectedHv.ForEach(x => hash = hash * 31 + x);
                }

                if (SelectedStraighDraws != null)
                {
                    SelectedStraighDraws.ForEach(x => hash = hash * 31 + x);
                }

                if (SelectedFlushDraws != null)
                {
                    SelectedFlushDraws.ForEach(x => hash = hash * 31 + x);
                }

                return hash;
            }
        }
    }
}