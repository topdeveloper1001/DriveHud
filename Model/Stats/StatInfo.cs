//-----------------------------------------------------------------------
// <copyright file="StatInfo.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Annotations;
using DriveHUD.Common.Log;
using DriveHUD.Common.Reflection;
using DriveHUD.Common.Resources;
using Model.Data;
using Model.Enums;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Model.Stats
{
    [XmlInclude(typeof(StatInfoBreak))]
    [Serializable]
    [ProtoContract]
    [ProtoInclude(30, typeof(StatInfoBreak))]
    public class StatInfo : INotifyPropertyChanged
    {
        private const string totalHandFormat = "{0:0}";

        private const string DefaultFont = "Segoe UI";

        public StatInfo()
        {
            // default stat format 
            format = "{0:0.0}";
            // list stat by default
            isListed = true;
            // initialize collection
            SettingsAppearanceValueRangeCollection = new ObservableCollection<StatInfoOptionValueRange>();
            // initialize defaults
            Reset();
        }

        public void Reset()
        {
            currentValue = -1;
            settingAppearanceFontSource = DefaultFont;
            settingsAppearanceFontFamily = Fonts.SystemFontFamilies.Where(x => x.Source == settingAppearanceFontSource).FirstOrDefault();
            currentColor = HudDefaultSettings.StatInfoDefaultColor;
            settingsAppearanceFontSize = 10;
            settingsAppearanceFontBold = FontWeights.Normal;
            settingsAppearanceFontItalic = FontStyles.Normal;
            settingsAppearanceFontUnderline = null;
            settingsAppearanceFontBold_IsChecked = false;
            settingsAppearanceFontItalic_IsChecked = false;
            settingsAppearanceFontUnderline_IsChecked = false;
            isSelected = false;
            hasAttachedTools = false;

            if (SettingsAppearanceValueRangeCollection != null)
            {
                foreach (var v in SettingsAppearanceValueRangeCollection)
                {
                    v.PropertyChanged -= SettingsAppearanceValueRangeSelectedItem_PropertyChanged;
                }
            }
        }

        #region Properties

        private Guid id = Guid.NewGuid();

        private string caption;

        private StatInfoGroup statInfoGroup;
        private string category;
        private string propertyName;
        private string groupName;

        private Color currentColor;
        private decimal currentValue;

        private string settingAppearanceFontSource;
        private FontFamily settingsAppearanceFontFamily;
        private int settingsAppearanceFontSize;
        private FontWeight settingsAppearanceFontBold;
        private bool settingsAppearanceFontBold_IsChecked;
        private FontStyle settingsAppearanceFontItalic;
        private bool settingsAppearanceFontItalic_IsChecked;
        private TextDecorationCollection settingsAppearanceFontUnderline;
        private bool settingsAppearanceFontUnderline_IsChecked;

        private ObservableCollection<StatInfoOptionValueRange> settingsAppearanceValueRangeCollection;
        
        private bool settingsAppearance_IsChecked;
        private bool settingsPlayerType_IsChecked;

        private ObservableCollection<StatInfoToolTip> statInfoTooltipCollection;

        [ProtoMember(1)]
        public Guid Id
        {
            get
            {
                return id;
            }
            set
            {
                if (value == id) return;
                id = value;
                OnPropertyChanged();
            }
        }

        [ProtoMember(2)]
        public string Caption
        {
            get
            {
                return IsCaptionHidden ? string.Empty :
                        (string.IsNullOrWhiteSpace(caption) ?
                            CommonResourceManager.Instance.GetEnumResource(Stat) :
                            caption);
            }
            set
            {
                if (value == caption) return;
                caption = value;
                OnPropertyChanged();
            }
        }

        private bool isCaptionHidden;

        [ProtoMember(3)]
        [XmlIgnore]
        public bool IsCaptionHidden
        {
            get
            {
                return isCaptionHidden;
            }
            set
            {
                if (value == isCaptionHidden) return;
                isCaptionHidden = value;
                OnPropertyChanged();
            }
        }

        public string ToolTip
        {
            get
            {
                return IsCaptionHidden ?
                        string.Empty :
                            StatDto != null ?
                                string.Format("{0} ({1}/{2})", CommonResourceManager.Instance.GetEnumResource(Stat), statDto.Occured, statDto.CouldOccured) :
                                CommonResourceManager.Instance.GetEnumResource(Stat);
            }
        }

        private Stat stat;

        [ProtoMember(4)]
        public Stat Stat
        {
            get
            {
                return stat;
            }
            set
            {
                if (value == stat)
                {
                    return;
                }

                stat = value;

                OnPropertyChanged();
            }
        }

        [NonSerialized]
        private string format;

        [ProtoMember(5)]
        [XmlIgnore]
        public string Format
        {
            get
            {
                if (stat == Stat.TotalHands)
                {
                    return totalHandFormat;
                }

                return format;
            }
            set
            {
                if (value == format) return;
                format = value;
                OnPropertyChanged();
            }
        }

        [ProtoMember(6)]
        public StatInfoGroup StatInfoGroup
        {
            get
            {
                return statInfoGroup;
            }
            set
            {
                if (value == statInfoGroup) return;
                statInfoGroup = value;
                OnPropertyChanged();
            }
        }

        [ProtoMember(7)]
        public string GroupName
        {
            get
            {
                return groupName;
            }
            set
            {
                if (value == groupName) return;
                groupName = value;
                OnPropertyChanged();
            }
        }

        [ProtoMember(8)]
        public string Category
        {
            get
            {
                return category;
            }
            set
            {
                if (value == category) return;
                category = value;
                OnPropertyChanged();
            }
        }

        [ProtoMember(9)]
        public string PropertyName
        {
            get
            {
                return propertyName;
            }
            set
            {
                if (value == propertyName) return;
                propertyName = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore, ProtoMember(10)]
        public Color CurrentColor
        {
            get
            {
                if (hasAttachedTools || IsSelected)
                {
                    return HudDefaultSettings.StatInfoActiveColor;
                }

                return currentColor;
            }
            set
            {
                if (value == currentColor) return;
                currentColor = value;
                OnPropertyChanged();
            }
        }

        [ProtoMember(11), DefaultValue(-1)]
        public decimal CurrentValue
        {
            get
            {
                return currentValue;
            }
            set
            {
                if (value == currentValue) return;
                currentValue = value;
                OnPropertyChanged();

                ValueSetColor(value);
            }
        }

        [ProtoMember(13)]
        public bool SettingsAppearance_IsChecked
        {
            get
            {
                return settingsAppearance_IsChecked;
            }
            set
            {
                if (value == settingsAppearance_IsChecked) return;
                settingsAppearance_IsChecked = value;
                OnPropertyChanged();

                if (value) this.SettingsPlayerType_IsChecked = false;
            }
        }

        [ProtoMember(14)]
        public bool SettingsPlayerType_IsChecked
        {
            get
            {
                return settingsPlayerType_IsChecked;
            }
            set
            {
                if (value == settingsPlayerType_IsChecked) return;
                settingsPlayerType_IsChecked = value;
                OnPropertyChanged();

                if (value) this.SettingsAppearance_IsChecked = false;
            }
        }

        [ProtoMember(15)]
        public string SettingsAppearanceFontSource
        {
            get
            {
                return settingAppearanceFontSource;
            }
            set
            {
                if (settingAppearanceFontSource == value)
                {
                    return;
                }

                settingAppearanceFontSource = value;
                OnPropertyChanged();

                SettingsAppearanceFontFamily = Fonts.SystemFontFamilies.Where(x => x.Source == settingAppearanceFontSource).FirstOrDefault();
            }
        }

        [XmlIgnore]
        public FontFamily SettingsAppearanceFontFamily
        {
            get
            {
                return settingsAppearanceFontFamily;
            }
            set
            {
                if (value == settingsAppearanceFontFamily) return;
                settingsAppearanceFontFamily = value;
                OnPropertyChanged();

                if (settingsAppearanceFontFamily != null && settingsAppearanceFontFamily.Source != settingAppearanceFontSource)
                {
                    settingAppearanceFontSource = settingsAppearanceFontFamily.Source;
                }
            }
        }

        [ProtoMember(16)]
        public int SettingsAppearanceFontSize
        {
            get
            {
                return settingsAppearanceFontSize;
            }
            set
            {
                if (value == settingsAppearanceFontSize) return;
                settingsAppearanceFontSize = value;
                OnPropertyChanged();
            }
        }

        public FontWeight SettingsAppearanceFontBold
        {
            get
            {
                return settingsAppearanceFontBold;
            }
            set
            {
                if (value == settingsAppearanceFontBold) return;
                settingsAppearanceFontBold = value;
                OnPropertyChanged();
            }
        }

        [ProtoMember(17)]
        public bool SettingsAppearanceFontBold_IsChecked
        {
            get
            {
                return settingsAppearanceFontBold_IsChecked;
            }
            set
            {
                if (value == settingsAppearanceFontBold_IsChecked) return;
                settingsAppearanceFontBold_IsChecked = value;
                OnPropertyChanged();

                SettingsAppearanceFontBold = value ? FontWeights.Bold : FontWeights.Normal;
            }
        }

        public FontStyle SettingsAppearanceFontItalic
        {
            get
            {
                return settingsAppearanceFontItalic;
            }
            set
            {
                if (value == settingsAppearanceFontItalic) return;
                settingsAppearanceFontItalic = value;
                OnPropertyChanged();
            }
        }

        [ProtoMember(18)]
        public bool SettingsAppearanceFontItalic_IsChecked
        {
            get
            {
                return settingsAppearanceFontItalic_IsChecked;
            }
            set
            {
                if (value == settingsAppearanceFontItalic_IsChecked) return;
                settingsAppearanceFontItalic_IsChecked = value;
                OnPropertyChanged();

                SettingsAppearanceFontItalic = value ? FontStyles.Italic : FontStyles.Normal;
            }
        }

        public TextDecorationCollection SettingsAppearanceFontUnderline
        {
            get
            {
                return settingsAppearanceFontUnderline;
            }
            set
            {
                if (value == settingsAppearanceFontUnderline) return;
                settingsAppearanceFontUnderline = value;
                OnPropertyChanged();
            }
        }

        [ProtoMember(19)]
        public bool SettingsAppearanceFontUnderline_IsChecked
        {
            get
            {
                return settingsAppearanceFontUnderline_IsChecked;
            }
            set
            {
                if (value == settingsAppearanceFontUnderline_IsChecked) return;
                settingsAppearanceFontUnderline_IsChecked = value;
                OnPropertyChanged();

                SettingsAppearanceFontUnderline = value ? new TextDecorationCollection(TextDecorations.Underline) : null;
                SettingsAppearanceFontUnderline?.Freeze();
            }
        }

        [ProtoMember(20)]
        public ObservableCollection<StatInfoOptionValueRange> SettingsAppearanceValueRangeCollection
        {
            get
            {
                return settingsAppearanceValueRangeCollection;
            }
            set
            {
                if (value == settingsAppearanceValueRangeCollection) return;
                settingsAppearanceValueRangeCollection = value;
                OnPropertyChanged();
            }
        }

        public bool IsStatInfoToolTipAvailable
        {
            get
            {
                return StatInfoToolTipCollection != null && StatInfoToolTipCollection.Any();
            }
        }

        [ProtoMember(21)]
        public ObservableCollection<StatInfoToolTip> StatInfoToolTipCollection
        {
            get
            {
                return statInfoTooltipCollection;
            }
            set
            {
                if (value == statInfoTooltipCollection) return;
                statInfoTooltipCollection = value;
                OnPropertyChanged(nameof(StatInfoToolTipCollection));
            }
        }

        private int minSample = 1;

        [ProtoMember(22), DefaultValue(1)]
        public int MinSample
        {
            get
            {
                return minSample;
            }
            set
            {
                if (value == minSample) return;
                minSample = value;
                OnPropertyChanged();
            }
        }

        private string label;

        [ProtoMember(23)]
        public string Label
        {
            get
            {
                return label;
            }
            set
            {
                if (value == label) return;
                label = value;
                OnPropertyChanged();
            }
        }

        [NonSerialized]
        private bool isNotVisible;

        [XmlIgnore]
        [ProtoMember(24)]
        public bool IsNotVisible
        {
            get
            {
                return isNotVisible;
            }
            set
            {
                if (value == isNotVisible) return;
                isNotVisible = value;
                OnPropertyChanged();
            }
        }

        [NonSerialized]
        private bool isListed;

        [XmlIgnore]
        [ProtoMember(25)]
        public bool IsListed
        {
            get
            {
                return isListed;
            }
            set
            {
                if (value == isListed) return;
                isListed = value;
                OnPropertyChanged(nameof(IsListed));
            }
        }

        [NonSerialized]
        private StatDto statDto;

        [XmlIgnore]
        [ProtoMember(26)]
        public StatDto StatDto
        {
            get
            {
                return statDto;
            }
            set
            {
                if (value == statDto) return;
                statDto = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        [ProtoMember(27)]
        public StatInfoMeterModel StatInfoMeter { get; set; }

        [NonSerialized]
        private bool hasAttachedTools;

        /// <summary>
        /// Gets or sets whenever <see cref="StatInfo"/> has any attached tool
        /// </summary>
        [XmlIgnore]
        public bool HasAttachedTools
        {
            get
            {
                return hasAttachedTools;
            }
            set
            {
                if (hasAttachedTools != value)
                {
                    hasAttachedTools = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CurrentColor));
                }
            }
        }

        [NonSerialized]
        private bool isSelected;

        /// <summary>
        /// Gets or sets whenever <see cref="StatInfo"/> is selected
        /// </summary>
        [XmlIgnore]
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CurrentColor));
                }
            }
        }

        [NonSerialized]
        [ProtoMember(28)]
        private string graphToolIconSource;

        /// <summary>
        /// Gets or sets path to icon which is shown on graph tool 
        /// </summary>
        [XmlIgnore]
        public string GraphToolIconSource
        {
            get
            {
                return graphToolIconSource;
            }
            set
            {
                if (graphToolIconSource != value)
                {
                    graphToolIconSource = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Methods

        public void Initialize()
        {
            if (GetType().Equals(typeof(StatInfo)))
            {
                var colorRange = (from num in Enumerable.Range(1, 5)
                                  select new StatInfoOptionValueRange
                                  {
                                      Value = num != 5 ? num * 20 : (num - 1) * 20,
                                      ValueRangeType = num != 5 ? EnumStatInfoValueRangeType.LessThan : EnumStatInfoValueRangeType.MoreThan,
                                      Color = Colors.Gray,
                                      SortOrder = num
                                  }).ToArray();

                SettingsAppearanceValueRangeCollection = new ObservableCollection<StatInfoOptionValueRange>(colorRange);

                foreach (var v in SettingsAppearanceValueRangeCollection)
                {
                    v.PropertyChanged += SettingsAppearanceValueRangeSelectedItem_PropertyChanged;
                }
            }

            ValueSetColor(CurrentValue);
        }

        /// <summary>
        /// Finds relevant Color, defined in the list of ValueRange
        /// </summary>
        /// <param name="value"></param>
        private void ValueSetColor(decimal value)
        {
            StatInfoOptionValueRange currentValueRange;

            // If value falls into 'MoreThan' Range - take the most ValueRange
            currentValueRange = this.SettingsAppearanceValueRangeCollection.Where(x =>
                x.ValueRangeType == EnumStatInfoValueRangeType.MoreThan &&
                x.IsChecked == true &&
                x.Value < value).OrderByDescending(x => x.Value).FirstOrDefault();

            if (currentValueRange != null)
            {
                CurrentColor = currentValueRange.Color;
                return;
            }

            // If value falls into 'LessThan' Range - take the least ValueRange
            currentValueRange = this.SettingsAppearanceValueRangeCollection.Where(x =>
                x.ValueRangeType == EnumStatInfoValueRangeType.LessThan &&
                x.IsChecked == true &&
                x.Value >= value).OrderBy(x => x.Value).FirstOrDefault();

            if (currentValueRange != null)
            {
                this.CurrentColor = currentValueRange.Color;
                return;
            }
        }

        public virtual StatInfo Clone()
        {
            var statInfoClone = new StatInfo();

            statInfoClone.MinSample = MinSample;
            statInfoClone.Label = Label;
            statInfoClone.SettingsAppearanceFontBold = SettingsAppearanceFontBold;
            statInfoClone.SettingsAppearanceFontBold_IsChecked = SettingsAppearanceFontBold_IsChecked;
            statInfoClone.SettingsAppearanceFontFamily = SettingsAppearanceFontFamily;
            statInfoClone.SettingsAppearanceFontItalic = SettingsAppearanceFontItalic;
            statInfoClone.SettingsAppearanceFontItalic_IsChecked = SettingsAppearanceFontItalic_IsChecked;
            statInfoClone.SettingsAppearanceFontSize = SettingsAppearanceFontSize;
            statInfoClone.SettingsAppearanceFontUnderline = SettingsAppearanceFontUnderline;
            statInfoClone.SettingsAppearanceFontUnderline_IsChecked = SettingsAppearanceFontUnderline_IsChecked;
            statInfoClone.GroupName = GroupName;
            statInfoClone.Id = Id;
            statInfoClone.CurrentValue = CurrentValue;
            statInfoClone.CurrentColor = CurrentColor;
            statInfoClone.Caption = Caption;
            statInfoClone.Stat = Stat;
            statInfoClone.Category = Category;
            statInfoClone.Format = Format;
            statInfoClone.PropertyName = PropertyName;
            statInfoClone.SettingsPlayerType_IsChecked = SettingsPlayerType_IsChecked;
            statInfoClone.StatInfoGroup = StatInfoGroup;
            statInfoClone.IsNotVisible = IsNotVisible;
            statInfoClone.GraphToolIconSource = GraphToolIconSource;
            statInfoClone.StatInfoToolTipCollection = StatInfoToolTipCollection != null ?
                                                        new ObservableCollection<StatInfoToolTip>(StatInfoToolTipCollection.Select(x => x.Clone()).ToList()) :
                                                        StatInfoToolTipCollection;

            var colorRangeCloneCollection = SettingsAppearanceValueRangeCollection.Select(x => x.Clone()).OrderBy(x => x.Value).ToArray();
            statInfoClone.SettingsAppearanceValueRangeCollection = new ObservableCollection<StatInfoOptionValueRange>(colorRangeCloneCollection);

            foreach (var v in statInfoClone.SettingsAppearanceValueRangeCollection)
            {
                v.PropertyChanged += statInfoClone.SettingsAppearanceValueRangeSelectedItem_PropertyChanged;
            }

            return statInfoClone;
        }

        public void Merge(StatInfo statInfo)
        {
            MinSample = statInfo.MinSample;
            Label = statInfo.Label;
            SettingsAppearanceFontBold = statInfo.SettingsAppearanceFontBold;
            SettingsAppearanceFontBold_IsChecked = statInfo.SettingsAppearanceFontBold_IsChecked;
            SettingsAppearanceFontFamily = statInfo.SettingsAppearanceFontFamily;
            SettingsAppearanceFontItalic = statInfo.SettingsAppearanceFontItalic;
            SettingsAppearanceFontItalic_IsChecked = statInfo.SettingsAppearanceFontItalic_IsChecked;
            SettingsAppearanceFontSize = statInfo.SettingsAppearanceFontSize;
            SettingsAppearanceFontUnderline = statInfo.SettingsAppearanceFontUnderline;
            SettingsAppearanceFontUnderline_IsChecked = statInfo.SettingsAppearanceFontUnderline_IsChecked;
            SettingsAppearanceValueRangeCollection = statInfo.SettingsAppearanceValueRangeCollection;
        }

        public void AssignStatInfoValues(Indicators source)
        {
            if (string.IsNullOrEmpty(PropertyName))
            {
                LogProvider.Log.Error(string.Format("Couldn't find propertyName for {0}", Stat));
                return;
            }

            var propName = string.Format("{0}{1}", PropertyName, "Object");

            object propValue;

            if (source.HasProperty(propName))
            {
                propValue = ReflectionHelper.GetPropertyValue(source, propName);

                var statDto = propValue as StatDto;

                if (statDto != null)
                {
                    propValue = statDto.Value;
                    StatDto = statDto;
                }
            }
            else
            {
                propValue = ReflectionHelper.GetPropertyValue(source, PropertyName);
            }

            Caption = string.Format(Format, propValue);
            CurrentValue = Convert.ToDecimal(propValue);
        }

        public void UpdateColor()
        {
            ValueSetColor(CurrentValue);
        }
        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void SettingsAppearanceValueRangeSelectedItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Color" || e.PropertyName == "IsChecked")
            {
                ValueSetColor(this.CurrentValue);
            }

            // re-sort color ranges
            if (e.PropertyName == ReflectionHelper.GetPath<StatInfoOptionValueRange>(o => o.Value))
            {
                var orderedItems = SettingsAppearanceValueRangeCollection.OrderBy(x => x.Value).ToArray();

                SettingsAppearanceValueRangeCollection.Clear();

                for (var i = 0; i < orderedItems.Length; i++)
                {
                    SettingsAppearanceValueRangeCollection.Add(orderedItems[i]);

                    if (i < orderedItems.Length - 1)
                    {
                        orderedItems[i].ValueRangeType = EnumStatInfoValueRangeType.LessThan;
                    }
                    else
                    {
                        orderedItems[i].ValueRangeType = EnumStatInfoValueRangeType.MoreThan;
                    }
                }
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}