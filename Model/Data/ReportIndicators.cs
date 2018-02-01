//-----------------------------------------------------------------------
// <copyright file="ReportIndicators.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Model.Data
{
    public class ReportIndicators : LightIndicators
    {
        public ReportIndicators() : base()
        {
        }

        public ReportIndicators(IEnumerable<Playerstatistic> playerStatistic) : base(playerStatistic)
        {
        }

        public ObservableCollection<ReportHandViewModel> ReportHands { get; private set; } = new ObservableCollection<ReportHandViewModel>();

        public override void AddStatistic(Playerstatistic statistic)
        {
            base.AddStatistic(statistic);            

            var reportHandViewModel = new ReportHandViewModel(statistic);
            ReportHands.Add(reportHandViewModel);
        }

        public override void Clean()
        {
            base.Clean();
            ReportHands.Clear();
        }
    }    
}