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

namespace DriveHUD.ViewModels
{
    [XmlInclude(typeof(StatInfoBreak))]
    [Serializable]
    [ProtoContract]
    [ProtoInclude(29, typeof(StatInfoBreak))]
    public class StatInfo : INotifyPropertyChanged
    {
        private const string totalHandFormat = "{0:0}";

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
            _currentValue = -1;
            _settingAppearanceFontSource = "Segoe UI";
            _settingsAppearanceFontFamily = Fonts.SystemFontFamilies.Where(x => x.Source == _settingAppearanceFontSource).FirstOrDefault();
            _currentColor = Colors.Gray;
            _settingsAppearanceFontSize = 10;
            _settingsAppearanceFontBold = FontWeights.Normal;
            _settingsAppearanceFontItalic = FontStyles.Normal;
            _settingsAppearanceFontUnderline = null;
            _settingsAppearanceFontBold_IsChecked = false;
            _settingsAppearanceFontItalic_IsChecked = false;
            _settingsAppearanceFontUnderline_IsChecked = false;

            if (SettingsAppearanceValueRangeCollection != null)
            {
                foreach (var v in SettingsAppearanceValueRangeCollection)
                {
                    v.PropertyChanged -= SettingsAppearanceValueRangeSelectedItem_PropertyChanged;
                }
            }
        }

        #region Properties

        private Guid _id = Guid.NewGuid();

        private string _caption;

        private StatInfoGroup _statInfoGroup;
        private string _category;
        private string _propertyName;
        private string _groupName;

        private Color _currentColor;
        private decimal _currentValue;

        private string _settingAppearanceFontSource;
        private FontFamily _settingsAppearanceFontFamily;
        private int _settingsAppearanceFontSize;
        private FontWeight _settingsAppearanceFontBold;
        private bool _settingsAppearanceFontBold_IsChecked;
        private FontStyle _settingsAppearanceFontItalic;
        private bool _settingsAppearanceFontItalic_IsChecked;
        private TextDecorationCollection _settingsAppearanceFontUnderline;
        private bool _settingsAppearanceFontUnderline_IsChecked;

        private ObservableCollection<StatInfoOptionValueRange> _settingsAppearanceValueRangeCollection;

        private bool _settings_IsAvailable;
        private bool _settingsAppearance_IsChecked;
        private bool _settingsPlayerType_IsChecked;

        private ObservableCollection<StatInfoToolTip> _statInfoTooltipCollection;

        [ProtoMember(1)]
        public Guid Id
        {
            get { return _id; }
            set
            {
                if (value == _id) return;
                _id = value;
                OnPropertyChanged();
            }
        }

        [ProtoMember(2)]
        public string Caption
        {
            get
            {
                return IsCaptionHidden ? string.Empty :
                        (string.IsNullOrWhiteSpace(_caption) ?
                            CommonResourceManager.Instance.GetEnumResource(Stat) :
                            _caption);
            }
            set
            {
                if (value == _caption) return;
                _caption = value;
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
            get { return _statInfoGroup; }
            set
            {
                if (value == _statInfoGroup) return;
                _statInfoGroup = value;
                OnPropertyChanged();
            }
        }

        [ProtoMember(7)]
        public string GroupName
        {
            get
            {
                return _groupName;
            }

            set
            {
                if (value == _groupName) return;
                _groupName = value;
                OnPropertyChanged();
            }
        }

        [ProtoMember(8)]
        public string Category
        {
            get { return _category; }
            set
            {
                if (value == _category) return;
                _category = value;
                OnPropertyChanged();
            }
        }

        [ProtoMember(9)]
        public string PropertyName
        {
            get { return _propertyName; }
            set
            {
                if (value == _propertyName) return;
                _propertyName = value;
                OnPropertyChanged();
            }
        }

        [ProtoMember(10)]
        public Color CurrentColor
        {
            get { return _currentColor; }
            set
            {
                if (value == _currentColor) return;
                _currentColor = value;
                OnPropertyChanged();
            }
        }

        [ProtoMember(11), DefaultValue(-1)]
        public decimal CurrentValue
        {
            get { return _currentValue; }
            set
            {
                if (value == _currentValue) return;
                _currentValue = value;
                OnPropertyChanged();

                ValueSetColor(value);
            }
        }

        [ProtoMember(12)]
        public bool Settings_IsAvailable
        {
            get { return _settings_IsAvailable; }
            set
            {
                if (value == _settings_IsAvailable) return;
                _settings_IsAvailable = value;
                OnPropertyChanged();
            }
        }

        [ProtoMember(13)]
        public bool SettingsAppearance_IsChecked
        {
            get { return _settingsAppearance_IsChecked; }
            set
            {
                if (value == _settingsAppearance_IsChecked) return;
                _settingsAppearance_IsChecked = value;
                OnPropertyChanged();

                if (value) this.SettingsPlayerType_IsChecked = false;
            }
        }

        [ProtoMember(14)]
        public bool SettingsPlayerType_IsChecked
        {
            get { return _settingsPlayerType_IsChecked; }
            set
            {
                if (value == _settingsPlayerType_IsChecked) return;
                _settingsPlayerType_IsChecked = value;
                OnPropertyChanged();

                if (value) this.SettingsAppearance_IsChecked = false;
            }
        }

        [ProtoMember(15)]
        public string SettingsAppearanceFontSource
        {
            get
            {
                return _settingAppearanceFontSource;
            }
            set
            {
                if (_settingAppearanceFontSource == value)
                {
                    return;
                }

                _settingAppearanceFontSource = value;
                OnPropertyChanged();

                SettingsAppearanceFontFamily = Fonts.SystemFontFamilies.Where(x => x.Source == _settingAppearanceFontSource).FirstOrDefault();
            }
        }

        [XmlIgnore]
        public FontFamily SettingsAppearanceFontFamily
        {
            get { return _settingsAppearanceFontFamily; }
            set
            {
                if (value == _settingsAppearanceFontFamily) return;
                _settingsAppearanceFontFamily = value;
                OnPropertyChanged();

                if (_settingsAppearanceFontFamily != null && _settingsAppearanceFontFamily.Source != _settingAppearanceFontSource)
                {
                    _settingAppearanceFontSource = _settingsAppearanceFontFamily.Source;
                }
            }
        }

        [ProtoMember(16)]
        public int SettingsAppearanceFontSize
        {
            get { return _settingsAppearanceFontSize; }
            set
            {
                if (value == _settingsAppearanceFontSize) return;
                _settingsAppearanceFontSize = value;
                OnPropertyChanged();
            }
        }

        public FontWeight SettingsAppearanceFontBold
        {
            get { return _settingsAppearanceFontBold; }
            set
            {
                if (value == _settingsAppearanceFontBold) return;
                _settingsAppearanceFontBold = value;
                OnPropertyChanged();
            }
        }

        [ProtoMember(17)]
        public bool SettingsAppearanceFontBold_IsChecked
        {
            get { return _settingsAppearanceFontBold_IsChecked; }
            set
            {
                if (value == _settingsAppearanceFontBold_IsChecked) return;
                _settingsAppearanceFontBold_IsChecked = value;
                OnPropertyChanged();

                SettingsAppearanceFontBold = value ? FontWeights.Bold : FontWeights.Normal;
            }
        }

        public FontStyle SettingsAppearanceFontItalic
        {
            get { return _settingsAppearanceFontItalic; }
            set
            {
                if (value == _settingsAppearanceFontItalic) return;
                _settingsAppearanceFontItalic = value;
                OnPropertyChanged();
            }
        }

        [ProtoMember(18)]
        public bool SettingsAppearanceFontItalic_IsChecked
        {
            get { return _settingsAppearanceFontItalic_IsChecked; }
            set
            {
                if (value == _settingsAppearanceFontItalic_IsChecked) return;
                _settingsAppearanceFontItalic_IsChecked = value;
                OnPropertyChanged();

                SettingsAppearanceFontItalic = value ? FontStyles.Italic : FontStyles.Normal;
            }
        }

        public TextDecorationCollection SettingsAppearanceFontUnderline
        {
            get { return _settingsAppearanceFontUnderline; }
            set
            {
                if (value == _settingsAppearanceFontUnderline) return;
                _settingsAppearanceFontUnderline = value;
                OnPropertyChanged();
            }
        }

        [ProtoMember(19)]
        public bool SettingsAppearanceFontUnderline_IsChecked
        {
            get { return _settingsAppearanceFontUnderline_IsChecked; }
            set
            {
                if (value == _settingsAppearanceFontUnderline_IsChecked) return;
                _settingsAppearanceFontUnderline_IsChecked = value;
                OnPropertyChanged();

                SettingsAppearanceFontUnderline = value ? new TextDecorationCollection(TextDecorations.Underline) : null;
                SettingsAppearanceFontUnderline?.Freeze();
            }
        }

        [ProtoMember(20)]
        public ObservableCollection<StatInfoOptionValueRange> SettingsAppearanceValueRangeCollection
        {
            get { return _settingsAppearanceValueRangeCollection; }
            set
            {
                if (value == _settingsAppearanceValueRangeCollection) return;
                _settingsAppearanceValueRangeCollection = value;
                OnPropertyChanged();
            }
        }

        public bool IsStatInfoToolTipAvailable
        {
            get { return StatInfoToolTipCollection != null && StatInfoToolTipCollection.Any(); }
        }

        [ProtoMember(21)]
        public ObservableCollection<StatInfoToolTip> StatInfoToolTipCollection
        {
            get
            {
                return _statInfoTooltipCollection;
            }
            set
            {
                if (value == _statInfoTooltipCollection) return;
                _statInfoTooltipCollection = value;
                OnPropertyChanged(nameof(StatInfoToolTipCollection));
            }
        }

        private int minSample = 1;

        [ProtoMember(22), DefaultValue(1)]
        public int MinSample
        {
            get { return minSample; }
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
            get { return label; }
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
        private bool isDuplicateSelected;

        [XmlIgnore]
        [ProtoMember(25)]
        public bool IsDuplicateSelected
        {
            get
            {
                return isDuplicateSelected;
            }
            set
            {
                if (value == isDuplicateSelected) return;
                isDuplicateSelected = value;
                OnPropertyChanged();
            }
        }

        [NonSerialized]
        private bool isListed;

        [XmlIgnore]
        [ProtoMember(26)]
        public bool IsListed
        {
            get { return isListed; }
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
        [ProtoMember(27)]
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
        [ProtoMember(28)]
        public StatInfoMeterModel StatInfoMeter { get; set; }

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
        /// Find relevant Color, defined in the list of ValueRange
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
                this.CurrentColor = currentValueRange.Color;
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

        public StatInfo Clone()
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
            statInfoClone.Settings_IsAvailable = Settings_IsAvailable;
            statInfoClone.StatInfoGroup = StatInfoGroup;
            statInfoClone.IsNotVisible = IsNotVisible;
            statInfoClone.IsDuplicateSelected = IsDuplicateSelected;
            statInfoClone.StatInfoToolTipCollection = new ObservableCollection<StatInfoToolTip>(StatInfoToolTipCollection.Select(x => x.Clone()).ToList());

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
                LogProvider.Log.Error(string.Format("Couldn't find properyName for {0}", Stat));
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
            ValueSetColor(this.CurrentValue);
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
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}