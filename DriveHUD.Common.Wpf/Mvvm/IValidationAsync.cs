//-----------------------------------------------------------------------
// <copyright file="IValidationAsync.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.ComponentModel;

namespace DriveHUD.Common.Wpf.Mvvm
{
    public interface IValidationAsync
    {
        bool IsValidating { get; }

        bool GetPropertValidating(string propertyName);

        event EventHandler<DataErrorsChangedEventArgs> PropertyValidating;

        event EventHandler<DataErrorsChangedEventArgs> PropertyValidated;
    }
}