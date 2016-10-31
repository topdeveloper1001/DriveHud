//-----------------------------------------------------------------------
// <copyright file="HudElementViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Xml.Serialization;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Specialized;
using Model.Enums;
using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Common.Resources;
using ProtoBuf;

namespace DriveHUD.Application.ViewModels
{
    [ProtoContract]
    /// <summary>
    /// This View Model represents size and position of Hud Elements (these black rectangles where stat block is)(not thread-safe)
    /// </summary>    
    public class HudElementViewModel : ViewModelBase, IHudWindowElement
    {
        private static StatInfo BlankStatInfo = new StatInfo { Caption = string.Empty, IsCaptionHidden = true };

        public HudElementViewModel()
        {
            statInfoCollection = new ObservableCollection<StatInfo>();
        }

        public HudElementViewModel(IEnumerable<StatInfo> statInfos)
        {
            statInfoCollection = new ObservableCollection<StatInfo>(statInfos);

            Init();
        }

        private void Init()
        {
            HudViewType = HudViewType.Vertical_1;
        }

        public void UpdateMainStats()
        {
            RaisePropertyChanged(() => Stat1);
            RaisePropertyChanged(() => Stat2);
            RaisePropertyChanged(() => Stat3);
            RaisePropertyChanged(() => Stat4);
        }

        #region Properties

        [ProtoMember(1)]
        private System.Windows.Point position;

        /// <summary>
        /// Position on table
        /// </summary>
        public System.Windows.Point Position
        {
            get
            {
                return position;
            }
            set
            {
                OffsetY = 0;
                OffsetX = 0;

                this.RaiseAndSetIfChanged(ref position, value);
            }
        }

        [ProtoMember(2)]
        private double height;

        /// <summary>
        /// Panel base height
        /// </summary>
        public double Height
        {
            get
            {
                return height;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref height, value);
            }
        }

        [ProtoMember(3)]
        private double width;

        /// <summary>
        /// Panel base width
        /// </summary>
        public double Width
        {
            get
            {
                return width;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref width, value);
            }
        }

        [ProtoMember(4)]
        private double offsetX;

        public double OffsetX
        {
            get
            {
                return offsetX;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref offsetX, value);
            }
        }

        [ProtoMember(5)]
        private double offsetY;

        public double OffsetY
        {
            get
            {
                return offsetY;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref offsetY, value);
            }
        }

        [ProtoMember(6)]
        private int seat;

        /// <summary>
        /// Seat position
        /// </summary>
        public int Seat
        {
            get
            {
                return seat;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref seat, value);
            }
        }

        [ProtoMember(7)]
        private string playerName;

        /// <summary>
        /// Player name
        /// </summary>
        public string PlayerName
        {
            get
            {
                return playerName;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref playerName, value);
            }
        }

        [ProtoMember(8)]
        private short pokerSiteId;

        /// <summary>
        /// Pokersite Id
        /// </summary>
        public short PokerSiteId
        {
            get
            {
                return pokerSiteId;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref pokerSiteId, value);
            }
        }

        [ProtoMember(9)]
        private bool isRightOriented;

        /// <summary>
        /// Determines rich HUD indicator and icon alignment
        /// </summary>
        public bool IsRightOriented
        {
            get
            {
                return isRightOriented;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref isRightOriented, value);
            }
        }

        [ProtoMember(10)]
        private HudViewType hudViewType;

        /// <summary>
        /// Determines rich Hud type
        /// </summary>
        public HudViewType HudViewType
        {
            get
            {
                return hudViewType;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref hudViewType, value);
                this.RaisePropertyChanged(nameof(IsVertical));
            }
        }

        /// <summary>
        /// Determines if rich Hud is vertical
        /// </summary>
        public bool IsVertical
        {
            get
            {
                return HudViewType == HudViewType.Vertical_1
                    || HudViewType == HudViewType.Vertical_2;
            }
        }

        [ProtoMember(11)]
        private decimal tiltMeter;

        /// <summary>
        /// Tilt meter value
        /// </summary>
        public decimal TiltMeter
        {
            get
            {
                return tiltMeter;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref tiltMeter, value);
            }
        }

        [ProtoMember(12)]
        private bool isNoteIconVisible;

        /// <summary>
        /// Determine if note icon is visible
        /// </summary>
        public bool IsNoteIconVisible
        {
            get { return isNoteIconVisible; }
            set
            {
                this.RaiseAndSetIfChanged(ref isNoteIconVisible, value);
                this.RaisePropertyChanged(nameof(NoteMenuItemText));
            }
        }

        /// <summary>
        /// Note menu item name in the Context menu
        /// </summary>
        public string NoteMenuItemText
        {
            get
            {
                return isNoteIconVisible
                    ? CommonResourceManager.Instance.GetResourceString(ResourceStrings.EditNote)
                    : CommonResourceManager.Instance.GetResourceString(ResourceStrings.MakeNote);
            }
        }

        [ProtoMember(13)]
        private decimal? sessionHands;

        /// <summary>
        /// Total hands for current session
        /// </summary>
        public decimal? TotalHands
        {
            get { return sessionHands; }
            set
            {
                this.RaiseAndSetIfChanged(ref sessionHands, value);
            }
        }

        [ProtoMember(14)]
        private ObservableCollection<decimal> sessionMoneyWonCollection;

        public ObservableCollection<decimal> SessionMoneyWonCollection
        {
            get { return sessionMoneyWonCollection; }
            set
            {
                this.RaiseAndSetIfChanged(ref sessionMoneyWonCollection, value);
            }
        }

        /// <summary>
        /// Money won for current session
        /// </summary>
        public decimal? SessionMoneyWon
        {
            get { return SessionMoneyWonCollection?.Sum(); }
        }

        [ProtoMember(15)]
        private ObservableCollection<string> cardsCollection;

        /// <summary>
        /// Collection of cards
        /// </summary>
        public ObservableCollection<string> CardsCollection
        {
            get { return cardsCollection; }
            set
            {
                this.RaiseAndSetIfChanged(ref cardsCollection, value);
            }
        }

        #region Rich HUD stats

        /// <summary>
        /// Second stat for block #2 in rich HUD
        /// </summary>
        public StatInfo Stat1
        {
            get
            {
                var mainStats = GetMainStats();

                if (mainStats.Count < 1)
                {
                    return BlankStatInfo;
                }

                var stat = mainStats[0];

                return stat;
            }
        }

        /// <summary>
        /// Second stat for block #2 in rich HUD
        /// </summary>
        public StatInfo Stat2
        {
            get
            {
                var mainStats = GetMainStats();

                if (mainStats.Count < 2)
                {
                    return BlankStatInfo;
                }

                var stat = mainStats[1];

                return stat;
            }
        }

        /// <summary>
        /// Third stat for block #3 in rich HUD
        /// </summary>
        public StatInfo Stat3
        {
            get
            {
                var mainStats = GetMainStats();

                if (mainStats.Count < 3)
                {
                    return BlankStatInfo;
                }

                var stat = mainStats[2];

                return stat;
            }
        }

        /// <summary>
        /// Fourth stat for block #4 in rich HUD
        /// </summary>
        public StatInfo Stat4
        {
            get
            {
                var mainStats = GetMainStats();

                if (mainStats.Count < 4)
                {
                    return BlankStatInfo;
                }

                var stat = mainStats[3];

                return stat;
            }
        }

        [NonSerialized]
        [ProtoMember(16)]
        private ObservableCollection<StatInfo> statInfoCollection;

        [XmlIgnore]
        /// <summary>
        /// Collection of stats for current panel
        /// </summary>
        public ObservableCollection<StatInfo> StatInfoCollection
        {
            get
            {
                return statInfoCollection;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref statInfoCollection, value);
            }
        }

        [NonSerialized]
        [ProtoMember(17)]
        private ObservableCollection<HudBumperStickerType> stickers;

        public ObservableCollection<HudBumperStickerType> Stickers
        {
            get { return stickers; }
            set
            {
                this.RaiseAndSetIfChanged(ref stickers, value);
            }
        }

        [NonSerialized]
        [ProtoMember(18)]
        private string playerIcon;

        [XmlIgnore]
        public string PlayerIcon
        {
            get
            {
                return playerIcon;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref playerIcon, value);
                IsDefaultImage = string.IsNullOrWhiteSpace(playerIcon);
            }
        }

        [NonSerialized]
        [ProtoMember(19)]
        private string playerIconToolTip;

        [XmlIgnore]
        public string PlayerIconToolTip
        {
            get
            {
                return string.IsNullOrEmpty(playerIconToolTip) ? playerName : playerIconToolTip;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref playerIconToolTip, value);
            }
        }

        [NonSerialized]
        [ProtoMember(20)]
        private bool isDefaultImage = true;

        [XmlIgnore]
        public bool IsDefaultImage
        {
            get
            {
                return isDefaultImage;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref isDefaultImage, value);
            }
        }

        [ProtoMember(21)]
        private HudType hudType;

        public HudType HudType
        {
            get
            {
                return hudType;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref hudType, value);
            }
        }

        #endregion

        #endregion

        #region Infrastructure

        /// <summary>
        /// Clone object
        /// </summary>
        /// <returns>Cloned object</returns>
        public HudElementViewModel Clone()
        {
            var cloned = (HudElementViewModel)MemberwiseClone();

            cloned.StatInfoCollection = new ObservableCollection<StatInfo>();

            return cloned;
        }

        private List<StatInfo> GetMainStats()
        {
            var mainStats = new List<StatInfo>();

            var counter = 0;

            foreach (var statInfo in StatInfoCollection.Where(x => !x.IsNotVisible))
            {
                if (counter > 4)
                {
                    break;
                }

                if (statInfo is StatInfoBreak)
                {
                    continue;
                }

                mainStats.Add(statInfo);

                counter++;
            }

            return mainStats;

        }

        public void Cleanup()
        {
        }

        #endregion
    }
}