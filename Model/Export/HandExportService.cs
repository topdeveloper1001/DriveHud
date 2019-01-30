//-----------------------------------------------------------------------
// <copyright file="HandExportService.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Log;
using DriveHUD.Common.Progress;
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Model.Export
{
    internal class HandExportService : IHandExportService
    {       
        private const int HandPerQuery = 500;
        
        public async Task ExportHands(string folder, IEnumerable<HandExportInfo> exportInfo, IDHProgress progress, bool useCommonExporter)
        {
            await Task.Run(() =>
            {
                try
                {
                    var handsGroupedBySite = exportInfo
                        .GroupBy(x => x.Site)
                        .ToDictionary(x => x.Key, x => x.Select(p => p.HandNumber).ToArray());

                    var exportedHands = 0;
                    var totalExportedHands = exportInfo.Count();

                    LogProvider.Log.Info(this, $"Starting export of {totalExportedHands} hands.");

                    progress.Report(new LocalizableString("Progress_ExportingHands", exportedHands, totalExportedHands), 0);

                    using (var session = ModelEntities.OpenStatelessSession())
                    {
                        foreach (var siteHands in handsGroupedBySite)
                        {
                            LogProvider.Log.Info(this, $"Exporting {siteHands.Value.Length} hands of {siteHands.Key} site.");

                            var serviceName = useCommonExporter ?
                                HandExportPreparingServiceProvider.Common :
                                HandExportPreparingServiceProvider.GetServiceName(siteHands.Key);

                            var preparingService = ServiceLocator.Current.GetInstance<IHandExportPreparingService>(serviceName);

                            var queriesCount = (int)Math.Ceiling((double)siteHands.Value.Length / HandPerQuery);

                            for (var i = 0; i < queriesCount; i++)
                            {
                                if (progress.CancellationToken.IsCancellationRequested)
                                {
                                    LogProvider.Log.Info(this, "Exporting cancelled by user.");
                                    LogProvider.Log.Info(this, $"Successfully exported {exportedHands} hands.");
                                    return;
                                }

                                var handsToQuery = siteHands.Value.Skip(i * HandPerQuery).Take(HandPerQuery).ToArray();

                                var restriction = Restrictions.Disjunction();

                                restriction.Add(Restrictions.Conjunction()
                                     .Add(Restrictions.On<Handhistory>(x => x.Gamenumber).IsIn(handsToQuery))
                                     .Add(Restrictions.Where<Handhistory>(x => x.PokersiteId == (short)siteHands.Key)));

                                var hands = session.QueryOver<Handhistory>().Where(restriction)
                                      .Select(x => x.HandhistoryVal)
                                      .List<string>();

                                preparingService.WriteHandsToFile(folder, hands, siteHands.Key);

                                exportedHands += hands.Count;

                                progress.Report(new LocalizableString("Progress_ExportingHands", exportedHands, exportInfo.Count()), 0);
                            }
                        }
                    }

                    LogProvider.Log.Info(this, $"Successfully exported {exportedHands} hands.");
                }
                catch (Exception e)
                {
                    throw new DHInternalException(new NonLocalizableString("Export service failed to export hands."), e);
                }
            });
        }
    }
}