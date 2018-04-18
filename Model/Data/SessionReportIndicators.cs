//-----------------------------------------------------------------------
// <copyright file="SessionReportIndicators.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Model.Data
{
    public class SessionReportIndicators : ReportIndicators
    {
        private const string gameTypeSeparator = ",";

        public override string GameType { get => string.Join(gameTypeSeparator, GameTypes); set => base.GameType = value; }

        public HashSet<string> GameTypes { get; set; } = new HashSet<string>();

        public virtual void AddIndicator(SessionReportIndicators indicator)
        {
            base.AddIndicator(indicator);

            indicator.GameTypes.ForEach(gameType =>
            {
                if (!GameTypes.Contains(gameType))
                {
                    GameTypes.Add(gameType);
                }
            });
        }
    }
}