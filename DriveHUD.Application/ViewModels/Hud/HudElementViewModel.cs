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
using Model.Stats;
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
    public class HudElementViewModel : ViewModelBase
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

            tools = new ReactiveList<HudBaseToolViewModel>(initialTools.Select(x =>
            {
                var toolViewModel = x.CreateViewModel(this);

                if (toolViewModel is IHudBaseStatToolViewModel)
                {
                    toolViewModel.IsVisible = false;
                }                

                return toolViewModel;
            }));

            Opacity = 100;
        }

        #region Properties

        [ProtoMember(1)]
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

        [ProtoMember(2)]
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

        [ProtoMember(3)]
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

        [ProtoMember(4)]
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

        [ProtoMember(5)]
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

        [ProtoMember(6)]
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

        [ProtoMember(8)]
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
        [ProtoMember(9)]
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

        [NonSerialized]
        [ProtoMember(10)]
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
        [ProtoMember(11)]
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
        [ProtoMember(12, IsRequired = true)]
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

        [ProtoMember(13)]
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

        public IEnumerable<StatInfo> StatInfoCollection
        {
            get
            {
                return Tools != null ?
                    Tools.OfType<IHudStatsToolViewModel>().SelectMany(x => x.Stats).ToArray() :
                    new StatInfo[0];
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