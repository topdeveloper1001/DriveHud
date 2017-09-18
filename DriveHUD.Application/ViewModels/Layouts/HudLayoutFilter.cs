//-----------------------------------------------------------------------
// <copyright file="HudLayoutFilter.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Linq;
using DriveHUD.Entities;
using DriveHUD.Importers;
using System;
using System.Linq;

namespace DriveHUD.Application.ViewModels.Layouts
{
    /// <summary>
    /// This class represents HUD layout filter
    /// </summary>
    [Serializable]
    public class HudLayoutFilter : ISessionStatisticFilter
    {
        public string FilterId
        {
            get
            {
                return ToString();
            }
        }

        public int[] TableTypes { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool Apply(Playerstatistic playerstatistic)
        {
            var result = (!StartDate.HasValue || StartDate.HasValue && playerstatistic.Time >= StartDate.Value) &&
                (!EndDate.HasValue || EndDate.HasValue && playerstatistic.Time <= EndDate.Value) &&
                (TableTypes == null || !TableTypes.Any() || TableTypes.Contains(playerstatistic.MaxPlayers));

            return result;
        }

        public bool IsDefault
        {
            get
            {
                return !StartDate.HasValue && !EndDate.HasValue && (TableTypes == null || !TableTypes.Any());
            }
        }

        public HudLayoutFilter Clone()
        {
            var clone = new HudLayoutFilter
            {
                StartDate = StartDate,
                EndDate = EndDate,
                TableTypes = TableTypes.ToArray()
            };

            return clone;
        }

        public override bool Equals(object obj)
        {
            var filter = obj as HudLayoutFilter;

            return Equals(filter);
        }

        public bool Equals(HudLayoutFilter filter)
        {
            if (filter == null)
            {
                return false;
            }

            var result = (!StartDate.HasValue && !filter.StartDate.HasValue || StartDate == filter.StartDate) &&
                (!EndDate.HasValue && !filter.EndDate.HasValue || EndDate == filter.EndDate) &&
                CompareTableTypes(this, filter);

            return result;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = 23;
                hashcode += hashcode * 31 + (StartDate.HasValue ? StartDate.GetHashCode() : 0);
                hashcode += hashcode * 31 + (EndDate.HasValue ? EndDate.GetHashCode() : 0);

                if (TableTypes != null && TableTypes.Any())
                {
                    TableTypes.Distinct().ForEach(x =>
                    {
                        hashcode += hashcode * 31 + x.GetHashCode();
                    });
                }

                return hashcode;
            }
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", StartDate, EndDate, TableTypes != null ? string.Join(",", TableTypes.Distinct()) : string.Empty);
        }

        private static bool CompareTableTypes(HudLayoutFilter filter1, HudLayoutFilter filter2)
        {
            var result = filter1.TableTypes == null && filter2.TableTypes == null ||
                filter1.TableTypes == null && filter2.TableTypes != null && !filter2.TableTypes.Any() ||
                filter2.TableTypes == null && filter1.TableTypes != null && !filter1.TableTypes.Any() ||
                filter1.TableTypes != null && filter2.TableTypes != null && !filter1.TableTypes.Any() && !filter2.TableTypes.Any() ||
                filter1.TableTypes != null && filter2.TableTypes != null && filter1.TableTypes.All(x => filter2.TableTypes.Contains(x)) && filter2.TableTypes.All(x => filter1.TableTypes.Contains(x));

            return result;
        }
    }
}