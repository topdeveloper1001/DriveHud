﻿//-----------------------------------------------------------------------
// <copyright file="StatInfoToolTip.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common.Resources;
using Model.Enums;
using ProtoBuf;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace Model.Stats
{
    [ProtoContract]
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

        [ProtoMember(3)]
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

        [ProtoMember(4)]
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

        [ProtoMember(5)]
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

        [ProtoMember(6)]
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

        [ProtoMember(7)]
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

        [ProtoMember(8)]
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

        private object _eventLock = new object();

        private event PropertyChangedEventHandler _propertyChanged;
        
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
            _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}