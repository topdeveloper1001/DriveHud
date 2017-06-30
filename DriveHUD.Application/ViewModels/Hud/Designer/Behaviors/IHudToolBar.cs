//-----------------------------------------------------------------------
// <copyright file="IHudToolBar.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Windows.Input;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// Defines properties and commands to set if the implementing instance supports tool bar
    /// </summary>
    public interface IHudToolBar
    {     
        bool IsSaveVisible { get; }

        bool IsRotateVisible { get; }
      
        ICommand SaveCommand { get; }

        ICommand RotateCommand { get; }
    }
}