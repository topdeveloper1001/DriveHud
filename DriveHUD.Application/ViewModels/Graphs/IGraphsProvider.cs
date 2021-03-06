﻿//-----------------------------------------------------------------------
// <copyright file="IGraphsProvider.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
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

namespace DriveHUD.Application.ViewModels.Graphs
{
    internal interface IGraphsProvider
    {
        /// <summary>
        /// Initializes provider to build series for the specified <see cref="SerieType"/>
        /// </summary>
        void Initialize(IEnumerable<SerieType> seriesTypes);

        /// <summary>
        /// Processes the specified <see cref="Playerstatistic"/>
        /// </summary>
        /// <param name="statistic"><see cref="Playerstatistic"/> to process</param>
        void Process(Playerstatistic statistic);

        /// <summary>
        /// Gets series
        /// </summary>        
        Dictionary<SerieType, IEnumerable<GraphSerie>> GetSeries();
    }
}