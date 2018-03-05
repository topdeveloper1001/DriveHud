//-----------------------------------------------------------------------
// <copyright file="PMSettingsModel.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Prism.Mvvm;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DriveHUD.PMCatcher.Settings
{
    [DataContract(Name = "Settings")]
    public class PMSettingsModel
    {
        [DataMember(Name = "AutomaticUpdates")]
        public bool AutomaticUpdatingEnabled { get; set; }

        [DataMember(Name = "Enabled")]
        public bool Enabled { get; set; }

        [DataMember(Name = "Logging")]
        public bool IsAdvancedLoggingEnabled { get; set; }

        [DataMember(Name = "Heroes")]
        public Dictionary<long, string> Heroes { get; set; }

        public PMSettingsModel()
        {
            SetDefaults();
        }

        public PMSettingsModel Clone()
        {
            var clone = (PMSettingsModel)MemberwiseClone();
            clone.Heroes = new Dictionary<long, string>(Heroes);
            return clone;
        }

        private void SetDefaults()
        {
            AutomaticUpdatingEnabled = true;
            IsAdvancedLoggingEnabled = true;
            Enabled = true;
            Heroes = new Dictionary<long, string>();
        }
    }
}