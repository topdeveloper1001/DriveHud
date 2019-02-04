//-----------------------------------------------------------------------
// <copyright file="IReplayerTableConfigurator.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Telerik.Windows.Controls;
using DriveHUD.Application.ViewModels.Replayer;
using DriveHUD.Application.Controls;

namespace DriveHUD.Application.TableConfigurators
{
    public interface IReplayerTableConfigurator
    {
        void ConfigureTable(RadDiagram diagram, HudDragCanvas dgCanvas, ReplayerViewModel viewModel);
    }
}