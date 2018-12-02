//-----------------------------------------------------------------------
// <copyright file="FilterAdvancedItem.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Newtonsoft.Json;
using Prism.Mvvm;
using System;
using System.Xml.Serialization;

namespace Model.Filters
{
    [Serializable]
    public class FilterAdvancedItem : BindableBase
    {
        public FilterAdvancedItem(AdvancedFilterType filterType)
        {
            FilterType = filterType;
        }

        private string name;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                SetProperty(ref name, value);
            }
        }

        public AdvancedFilterType FilterType { get; }

        private double? filterValue;

        public double? FilterValue
        {
            get
            {
                return filterValue;
            }
            set
            {
                SetProperty(ref filterValue, value);
                RaisePropertyChanged(nameof(ToolTip));
            }
        }

        [JsonIgnore]
        public string ToolTip
        {
            get
            {
                return FilterValue != null ?
                    $"{Name} ({FilterValue})" :
                    Name;
            }
        }

        private bool isSelected;

        [XmlIgnore, JsonIgnore]
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                SetProperty(ref isSelected, value);
            }
        }

        private FilterAdvancedStageType stage;

        public FilterAdvancedStageType Stage
        {
            get
            {
                return stage;
            }
            set
            {
                SetProperty(ref stage, value);
            }
        }

        public FilterAdvancedItem Clone()
        {
            return (FilterAdvancedItem)MemberwiseClone();
        }
    }
}