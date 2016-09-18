using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;

using DriveHUD.Common.Annotations;
using Model.Enums;
using DriveHUD.Common.Resources;

namespace DriveHUD.ViewModels
{
    [Serializable]
    public class StatInfoOptionValueRange : INotifyPropertyChanged
    {
        public StatInfoOptionValueRange()
        {
        }

        #region Properties

        private Guid _id = Guid.NewGuid();
        private bool _isEditable = true;
        private bool _isChecked = true;
        private int _sortOrder;
        private decimal _value;
        private bool _value_IsValid;
        private EnumStatInfoValueRangeType _valueRangeType = EnumStatInfoValueRangeType.LessThan;
        private Color _color;

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

        public bool IsEditable
        {
            get { return _isEditable; }

            set
            {
                if (value == _isEditable) return;
                _isEditable = value;
                OnPropertyChanged();
            }
        }

        public bool IsChecked
        {
            get { return _isChecked; }

            set
            {
                if (value == _isChecked) return;
                _isChecked = value;
                OnPropertyChanged();
            }
        }

        public int SortOrder
        {
            get { return _sortOrder; }

            set
            {
                if (value == _sortOrder) return;
                _sortOrder = value;
                OnPropertyChanged();
            }
        }

        public decimal Value
        {
            get { return _value; }
            set
            {
                if (value == _value) return;
                _value = value;
                OnPropertyChanged();
            }
        }

        public bool Value_IsValid
        {
            get { return _value_IsValid; }
            set
            {
                if (value == _value_IsValid) return;
                _value_IsValid = value;
                OnPropertyChanged();
            }
        }

        public EnumStatInfoValueRangeType ValueRangeType
        {
            get { return _valueRangeType; }
            set
            {
                if (value == _valueRangeType) return;
                _valueRangeType = value;
                OnPropertyChanged();
                OnPropertyChanged("Condition");
            }
        }

        public string Condition
        {
            get
            {
                return CommonResourceManager.Instance.GetEnumResource(ValueRangeType);
            }
        }

        public Color Color
        {
            get { return _color; }
            set
            {
                if (value == _color) return;
                _color = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Internal

        public StatInfoOptionValueRange Clone()
        {
            var statInfoOptionValueRange = new StatInfoOptionValueRange();
            statInfoOptionValueRange.Id = this.Id;
            statInfoOptionValueRange.IsEditable = this.IsEditable;
            statInfoOptionValueRange.IsChecked = this.IsChecked;
            statInfoOptionValueRange.SortOrder = this.SortOrder;
            statInfoOptionValueRange.Value = this.Value;
            statInfoOptionValueRange.Value_IsValid = this.Value_IsValid;
            statInfoOptionValueRange.ValueRangeType = this.ValueRangeType;
            statInfoOptionValueRange.Color = this.Color;

            return statInfoOptionValueRange;
        }


        private event PropertyChangedEventHandler _propertyChanged;
        private object _eventLock = new object();
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                lock (_eventLock)
                {
                    _propertyChanged -= value;
                    _propertyChanged += value;
                }
            }
            remove
            {
                lock (_eventLock)
                {
                    _propertyChanged -= value;
                }
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = _propertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
