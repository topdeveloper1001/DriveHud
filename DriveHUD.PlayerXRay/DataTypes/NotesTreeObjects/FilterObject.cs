//-----------------------------------------------------------------------
// <copyright file="FilterObject.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using ReactiveUI;
using System;
using System.Xml.Serialization;

namespace DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects
{
    [Serializable]
    public class FilterObject : ReactiveObject
    {
        private string description;

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref description, value);
            }
        }

        private int tag;

        public int Tag
        {
            get
            {
                return tag;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref tag, value);
            }
        }

        public FilterEnum Filter
        {
            get
            {
                return (FilterEnum)tag;
            }
            set
            {
                Tag = (int)value;
                this.RaisePropertyChanged();
            }
        }

        private double? filterValue;

        public double? Value
        {
            get
            {
                return filterValue;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref filterValue, value);
                this.RaisePropertyChanged(nameof(ToolTip));
            }
        }

        public string ToolTip
        {
            get
            {
                return Value != null ?
                    $"{Description} ({Value})" :
                    Description;
            }
        }

        private bool isSelected;

        [XmlIgnore]
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref isSelected, value);
            }
        }

        private NoteStageType stage;

        public NoteStageType Stage
        {
            get
            {
                return stage;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref stage, value);
            }
        }

        public FilterObject Clone()
        {
            return (FilterObject)MemberwiseClone();
        }
    }
}