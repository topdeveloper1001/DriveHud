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

using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Common;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Wpf.Mvvm;
using DriveHUD.ViewModels;
using ProtoBuf;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace DriveHUD.Application.ViewModels.Hud
{
    [ProtoContract]
    /// <summary>
    /// Represents view model of hud element
    /// </summary>    
    public class HudElementViewModel : ViewModelBase, IHudWindowElement
    {
        private static StatInfo BlankStatInfo = new StatInfo { Caption = string.Empty, IsCaptionHidden = true };

        public HudElementViewModel()
        {
            tools = new ReactiveList<HudBaseToolViewModel>();
            Opacity = 100;
        }

        public HudElementViewModel(IEnumerable<HudLayoutTool> initialTools)
        {
            Check.ArgumentNotNull(() => initialTools);

            tools = new ReactiveList<HudBaseToolViewModel>(initialTools.Select(x => x.CreateViewModel(this)));
            Opacity = 100;
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
        /// Poker site Id
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


        /// <summary>
        /// Determine if note icon is visible
        /// </summary>
        public bool IsNoteIconVisible
        {
            get { return !string.IsNullOrWhiteSpace(noteToolTip); }
        }

        [ProtoMember(12)]
        private string noteToolTip;

        public string NoteToolTip
        {
            get { return noteToolTip; }
            set
            {
                this.RaiseAndSetIfChanged(ref noteToolTip,
                    value.Length > 50 ? string.Format("{0}...", value.Substring(0, 50)) : value);
                this.RaisePropertyChanged(nameof(IsNoteIconVisible));
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
                return IsNoteIconVisible
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

        [NonSerialized]
        [ProtoMember(16)]
        private ReactiveList<HudBaseToolViewModel> tools;

        public ReactiveList<HudBaseToolViewModel> Tools
        {
            get
            {
                return tools;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref tools, value);
            }
        }

        [XmlIgnore]
        /// <summary>
        /// Collection of stats for current panel
        /// </summary>
        public ReadOnlyObservableCollection<StatInfo> StatInfoCollection
        {
            get
            {
                if (tools == null)
                {
                    return new ReadOnlyObservableCollection<StatInfo>(new ObservableCollection<StatInfo>());
                }

                return new ReadOnlyObservableCollection<StatInfo>(new ObservableCollection<StatInfo>());
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
        [ProtoMember(20, IsRequired = true)]
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

        [ProtoMember(22)]
        private double opacity;

        public double Opacity
        {
            get
            {
                return opacity;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref opacity, value);
            }
        }

        #endregion

        #region Infrastructure

        /// <summary>
        /// Clone object
        /// </summary>
        /// <returns>Cloned object</returns>
        public HudElementViewModel Clone()
        {
            var cloned = (HudElementViewModel)MemberwiseClone();
            return cloned;
        }

        public void Cleanup()
        {
        }

        #endregion
    }
}