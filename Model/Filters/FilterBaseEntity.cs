//-----------------------------------------------------------------------
// <copyright file="FilterBaseEntity.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Prism.Mvvm;
using System;

namespace Model.Filters
{
    [Serializable]
    public abstract class FilterBaseEntity : BindableBase, ICloneable
    {
        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        private Guid _id = Guid.NewGuid();
        private string _name;
        private bool _isActive;

        public Guid Id
        {
            get
            {
                return _id;
            }
            set
            {
                SetProperty(ref _id, value);
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                SetProperty(ref _name, value);
            }
        }

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                SetProperty(ref _isActive, value);
            }
        }
    }
}