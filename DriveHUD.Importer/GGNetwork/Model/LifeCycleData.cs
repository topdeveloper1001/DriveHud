//-----------------------------------------------------------------------
// <copyright file="LifeCycleData.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DriveHUD.Importers.GGNetwork.Model
{
    public class LifeCycleData
    {
        public DateTime AnnouncementTime { get; set; }

        public DateTime RegistrationTime { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string SeatingDuration { get; set; }

        public int LateRegistrationLevel { get; set; }

        public string LateRegistrationTime { get; set; }

        public string RemainingDuration { get; set; }

        public bool IsFixed { get; set; }

        public DateTime CreatedAt { get; set; }

        public int MinimumRank { get; set; }

        public int MaximumRank { get; set; }

        public IList<ServiceProviderACLItem> ServiceProviderACLItems { get; set; }

        public int DefaultPolicy { get; set; }

        public IList<int> AlarmTimes { get; set; }
    }
}