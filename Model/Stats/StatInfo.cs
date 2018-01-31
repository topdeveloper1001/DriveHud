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
    [ProtoInclude(101, typeof(StatInfoBreak))]
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
            settingsAppearanceFontFamily = Fonts.SystemFontFamilies.FirstOrDefault(x => x.Source == settingAppearanceFontSource);
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
            IsPopupBarNotSupported = false;

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
        private string propertyName;
        private string groupName;

        private Color currentColor;

        [NonSerialized]
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

        [XmlIgnore, ProtoMember(2)]
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

        private Stat stat;

        [ProtoMember(4)]
        public virtual Stat Stat
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

        public string ToolTip
        {
            get
            {
                return IsCaptionHidden ?
                        string.Empty :
                            StatDto != null ?
                                string.Format("{0} ({1}/{2})", !string.IsNullOrEmpty(Label) ? Label : CommonResourceManager.Instance.GetEnumResource(Stat), statDto.Occurred, statDto.CouldOccurred) :
                                !string.IsNullOrEmpty(Label) ? Label : CommonResourceManager.Instance.GetEnumResource(Stat);
            }
        }

        public string IterationsText
        {
            get
            {
                return IsCaptionHidden ?
                        string.Empty :
                            StatDto != null ?
                                string.Format("({0}/{1})", statDto.Occurred, statDto.CouldOccurred) :
                                string.Empty;
            }
        }

        [NonSerialized]
        private string format;

        [XmlIgnore]
        public string Format
        {
            get
            {
                if (Stat == Stat.TotalHands)
                {
                    return totalHandFormat;
                }

                var format = digitsAfterDecimalPoint == 0 ? "{0:0}" : digitsAfterDecimalPoint == 1 ? "{0:0.0}" : "{0:0.00}";

                return format;
            }
        }

        private int digitsAfterDecimalPoint = 1;

        [ProtoMember(5)]
        public int DigitsAfterDecimalPoint
        {
            get
            {
                return digitsAfterDecimalPoint;
            }
            set
            {
                if (digitsAfterDecimalPoint == value || digitsAfterDecimalPoint < 0 || digitsAfterDecimalPoint > 2)
                {
                    return;
                }

                digitsAfterDecimalPoint = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(Format));
            }
        }

        [XmlIgnore]
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

        [XmlIgnore]
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

        [XmlIgnore, ProtoMember(6)]
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

        [ProtoMember(7), DefaultValue(-1), XmlIgnore]
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

        [ProtoMember(8)]
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

        [ProtoMember(9)]
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

        [ProtoMember(10)]
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

                SettingsAppearanceFontFamily = Fonts.SystemFontFamilies.FirstOrDefault(x => x.Source == settingAppearanceFontSource);
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

        [ProtoMember(11)]
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

        [XmlIgnore]
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

        [ProtoMember(12)]
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

        [XmlIgnore]
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

        [ProtoMember(13)]
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

        [XmlIgnore]
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

        [ProtoMember(14)]
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

        [ProtoMember(15)]
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

        private int minSample = 1;

        [ProtoMember(16), DefaultValue(1)]
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

        [ProtoMember(17)]
        public string Label
        {
            get
            {
                return label;
            }
            set
            {
                if (value == label)
                {
                    return;
                }

                label = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(ToolTip));
            }
        }

        [NonSerialized]
        private bool isNotVisible;

        [XmlIgnore]
        [ProtoMember(18)]
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
        [ProtoMember(19)]
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
        [ProtoMember(20)]
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
        [ProtoMember(21)]
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
        [ProtoMember(22)]
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

        private bool isPopupBarNotSupported;

        [ProtoMember(23), DefaultValue(false)]
        public bool IsPopupBarNotSupported
        {
            get
            {
                return isPopupBarNotSupported;
            }
            set
            {
                if (value == isPopupBarNotSupported)
                {
                    return;
                }

                isPopupBarNotSupported = value;
                OnPropertyChanged();
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
            statInfoClone.DigitsAfterDecimalPoint = DigitsAfterDecimalPoint;
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
            statInfoClone.CurrentColor = currentColor;
            statInfoClone.Caption = Caption;
            statInfoClone.Stat = Stat;
            statInfoClone.SettingsPlayerType_IsChecked = SettingsPlayerType_IsChecked;
            statInfoClone.StatInfoGroup = StatInfoGroup;
            statInfoClone.IsNotVisible = IsNotVisible;
            statInfoClone.GraphToolIconSource = GraphToolIconSource;
            statInfoClone.IsPopupBarNotSupported = IsPopupBarNotSupported;

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
            DigitsAfterDecimalPoint = statInfo.DigitsAfterDecimalPoint;
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

        public void AssignStatInfoValues(Indicators source, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                LogProvider.Log.Error(string.Format("Couldn't find propertyName for {0}", Stat));
                return;
            }

            var propName = string.Format("{0}{1}", propertyName, "Object");

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
                propValue = ReflectionHelper.GetPropertyValue(source, propertyName);
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

        #endregion

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}