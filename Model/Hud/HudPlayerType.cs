//-----------------------------------------------------------------------
// <copyright file="HudPlayerType.cs" company="Ace Poker Solutions">
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
using Model.Enums;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace Model.Hud
{
    /// <summary>
    /// Hud player profile type
    /// </summary>
    [Serializable]
    public class HudPlayerType : ReactiveObject
    {
        private const int MinSampleDefault = 15;

        public HudPlayerType()
        {
        }

        public HudPlayerType(bool initialize) : this()
        {
            if (!initialize)
            {
                return;
            }

            MinSample = MinSampleDefault;
            EnablePlayerProfile = true;
            DisplayPlayerIcon = true;

            stats = new ObservableCollection<HudPlayerTypeStat>()
            {
                new HudPlayerTypeStat { Stat = Stat.VPIP },
                new HudPlayerTypeStat { Stat = Stat.PFR },
                new HudPlayerTypeStat { Stat = Stat.AGG },
                new HudPlayerTypeStat { Stat = Stat.S3Bet },
                new HudPlayerTypeStat { Stat = Stat.AF },
                new HudPlayerTypeStat { Stat = Stat.CBet },
                new HudPlayerTypeStat { Stat = Stat.FoldToCBet },
                new HudPlayerTypeStat { Stat = Stat.FoldTo3Bet },
                new HudPlayerTypeStat { Stat = Stat.WWSF },
                new HudPlayerTypeStat { Stat = Stat.WTSD },
                new HudPlayerTypeStat { Stat = Stat.Steal },
                new HudPlayerTypeStat { Stat = Stat.DonkBet }
            };
        }

        #region Properties

        private Guid id = Guid.NewGuid();

        [XmlIgnore]
        public Guid Id
        {
            get
            {
                return id;
            }
        }

        private string name;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref name, value);
            }
        }

        private string image;

        [XmlIgnore]
        public string Image
        {
            get
            {
                return image;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref image, value);

            }
        }

        private string imageAlias;

        public string ImageAlias
        {
            get
            {
                return imageAlias;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref imageAlias, value);
            }
        }

        private bool enablePlayerProfile;

        public bool EnablePlayerProfile
        {
            get
            {
                return enablePlayerProfile;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref enablePlayerProfile, value);
            }
        }

        private bool displayPlayerIcon;

        public bool DisplayPlayerIcon
        {
            get
            {
                return displayPlayerIcon;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref displayPlayerIcon, value);
            }
        }

        private int minSample;

        public int MinSample
        {
            get
            {
                return minSample;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref minSample, value);
            }
        }

        private ObservableCollection<HudPlayerTypeStat> stats;

        public ObservableCollection<HudPlayerTypeStat> Stats
        {
            get
            {
                return stats;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref stats, value);
            }
        }

        private bool isInternal;

        [XmlIgnore]
        public bool IsInternal
        {
            get
            {
                return isInternal;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref isInternal, value);
            }
        }

        /// <summary>
        /// Property is used to initialize massive arrays of HudPlayerType with defaults stat list without creation empty stats
        /// Only stats which needs to be initialized with data must be set 
        /// </summary>
        public IEnumerable<HudPlayerTypeStat> StatsToMerge
        {
            set
            {
                if (stats == null || value == null)
                {
                    return;
                }

                var statsToMerge = (from stat in stats
                                    join statToMerge in value on stat.Stat equals statToMerge.Stat into gj
                                    from grouped in gj.DefaultIfEmpty()
                                    where grouped != null
                                    select new { OldStat = stat, NewStat = grouped }).ToArray();

                statsToMerge.ForEach(x =>
                {
                    x.OldStat.Low = x.NewStat.Low;
                    x.OldStat.High = x.NewStat.High;
                });
            }
        }

        #endregion

        /// <summary>
        /// Clone object
        /// </summary>
        /// <returns>Cloned object</returns>
        public HudPlayerType Clone()
        {
            var clone = (HudPlayerType)MemberwiseClone();
            clone.Stats = new ObservableCollection<HudPlayerTypeStat>(clone.Stats.Select(x => (HudPlayerTypeStat)x.Clone()));
            return clone;
        }

        /// <summary>
        /// Merges the current player type with the specified player type
        /// </summary>
        public void MergeWith(HudPlayerType playerType)
        {
            if (playerType == null)
            {
                return;
            }

            MinSample = playerType.MinSample;
            EnablePlayerProfile = playerType.EnablePlayerProfile;
            DisplayPlayerIcon = playerType.DisplayPlayerIcon;
            Name = playerType.Name;
            Image = playerType.Image;
            ImageAlias = playerType.ImageAlias;

            var statsToMerge = (from currentStat in Stats
                                join stat in playerType.Stats on currentStat.Stat equals stat.Stat into gj
                                from grouped in gj.DefaultIfEmpty()
                                where grouped != null
                                select new { CurrentStat = currentStat, Stat = grouped }).ToArray();

            statsToMerge.ForEach(s =>
            {
                s.CurrentStat.Low = s.Stat.Low;
                s.CurrentStat.High = s.Stat.High;
            });
        }     
    }
}