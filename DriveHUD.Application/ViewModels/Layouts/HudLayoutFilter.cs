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

        public int? DataFreshness { get; set; }

        public bool Apply(Playerstatistic playerstatistic)
        {
            var result = (!DataFreshness.HasValue || DataFreshness.HasValue && (DateTime.Now - playerstatistic.Time).Days <= DataFreshness.Value) &&
                (TableTypes == null || !TableTypes.Any() || TableTypes.Contains(playerstatistic.MaxPlayers));

            return result;
        }

        public bool IsDefault
        {
            get
            {
                return !DataFreshness.HasValue && (TableTypes == null || !TableTypes.Any());
            }
        }

        public HudLayoutFilter Clone()
        {
            var clone = new HudLayoutFilter
            {
                DataFreshness = DataFreshness,
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

            var result = (!DataFreshness.HasValue && !filter.DataFreshness.HasValue || DataFreshness == filter.DataFreshness) &&
                CompareTableTypes(this, filter);

            return result;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = 23;
                hashcode += hashcode * 31 + (DataFreshness.HasValue ? DataFreshness.GetHashCode() : 0);

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
            return string.Format("{0},{2}", DataFreshness, TableTypes != null ? string.Join(",", TableTypes.Distinct()) : string.Empty);
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